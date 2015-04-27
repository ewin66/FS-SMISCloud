#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright  company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：              
//                  1.日 常 任 务 调度：jobname : DTU_ + DTU编号  ; trigger：T_ + dtu.DtuId ; group :Node .
//                  2.即时采集任务调度：jobname : DTU_INST_JOB_ + dtu.DtuId ; trigger：T_INSTANT_ + dtu.DtuId; group :Node .
//  创建标识：Created in 20141111 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System.Linq;
using FS.SMIS_Cloud.DAC.Accessor;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.File;
using FS.SMIS_Cloud.DAC.Gprs;
using FS.SMIS_Cloud.DAC.Gprs.Cmd;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;
using log4net;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using FS.SMIS_Cloud.DAC.Consumer;

namespace FS.SMIS_Cloud.DAC.Task
{
    using System.Text;

    // 
    public delegate void DACTaskResultListener (DACTaskResult result);

    public delegate void DTUConnectionStatusListener(DTUConnectionStatusChangedMsg msg);

    public delegate void SensorCollectMsgListener(CollectState state, SensorAcqResult acqResult);

    internal delegate DtuNode GetDtuNodeHandler(uint dtuid);

    internal interface WorkFinder
    {
        DACWorker FindWorker(uint dtuId);
    }

    public class DACTaskManager :  WorkFinder
    {
        private const int MIN_POOL_SIZE = 20;
        private const int MAX_POOL_SIZE = 256;

        private const string DailyJobName = "DTU_JOB_"; // DTU_ + DTU编号
        private const string DailyTrigger = "T_"; // T_ + dtu.DtuId
        private const string InstantJobName = "DTU_INST_JOB_"; // DTU_INST_JOB_ + dtu.DtuId
        private const string InstantTrigger = "T_INSTANT_"; // T_INSTANT_ + dtu.DtuId
        private const string JobGroup = "Node";
        internal static ILog Log = LogManager.GetLogger("TaskManager");
        private static ConcurrentDictionary<uint, DACWorker> _workers = new ConcurrentDictionary<uint, DACWorker>();
        private static ConcurrentDictionary<uint, DtuNode> _dtus = new ConcurrentDictionary<uint, DtuNode>();
        private static ConcurrentDictionary<string, DtuNode> _dtuCodeMap = new ConcurrentDictionary<string, DtuNode>();
        private static ConcurrentDictionary<uint, DACTask> _rtasks = new ConcurrentDictionary<uint, DACTask>();

        private static SynchronizedList<DtuGroup> dtuGroups = new SynchronizedList<DtuGroup>(); 

        private static IScheduler _schedule;
        private static IAdapterManager _adapterManager;
        private IDtuServer _dtuServer;
        private IDtuServer _fileServer;
        private static DtuType DtuType;
        public event SensorCollectMsgListener OnSensorCollectMsgHandler;

        public static event DTUConnectionStatusListener OnDTUConnectionStatusChanged;

        private SensorConfigUpdateServer _senconfChangedServer;

        // 匹配传感器是否需要采集. 
        public static SensorFilter SensorMatcher;

        /// <summary>
        /// </summary>
        /// <param name="dtus">所有DTU信息</param>
        /// <param name="utasks">未完成的实时采集任务（存储于数据库中，以DTUID为key）</param>
        public DACTaskManager(IDtuServer dtuServer, IList<DtuNode> dtus, IList<DACTask> utasks, DtuType type=DtuType.Gprs)
        {
            OnDTUConnectionStatusChanged = null;
            this._dtuServer = dtuServer;
            DACTaskPersistent.Init();
            foreach (DtuNode d in dtus)
            {
                _dtus[d.DtuId] = d;
                _dtuCodeMap[d.DtuCode] = d;
            }
            Log.DebugFormat("{0} dtus ready to work.", _dtus.Count);
            DtuType = type;
            if (DtuType.Gprs == type)
                dtuServer.OnConnectStatusChanged += OnConnectionStatusChanged;

            _adapterManager = SensorAdapterManager.InitializeManager();

            if (utasks != null)
                foreach (DACTask t in utasks)
                {
                    if (t.DtuID == 0) continue;
                    _rtasks[t.DtuID] = t;
                    ArrangeInstantTask(t.TID, t.DtuID, t.Sensors, null, true);
                }
        }

        public void SetFileServer(IDtuServer fileserver)
        {
            _fileServer = fileserver;
            //_fileServer.OnConnectStatusChanged += OnConnectionStatusChanged;
        }

        public void AddDtu(DtuNode dtu)
        {
            if (dtu == null) return;
            _dtus[dtu.DtuId] = dtu;
            _dtuCodeMap[dtu.DtuCode] = dtu;
            ScheduleJob(dtu, DateBuilder.NextGivenMinuteDate(null, (int) dtu.DacInterval/60));
            if (!_schedule.IsStarted)
                _schedule.Start();
        }

        // Node 状态变更。
        public void OnConnectionStatusChanged(IDtuConnection c, WorkingStatus oldStat, WorkingStatus newStat)
        {
            string dtuCode = c.DtuID;
            if (newStat == WorkingStatus.IDLE) {
                // Online;
                if (OnDTUConnectionStatusChanged != null)
                {
                    OnDTUConnectionStatusChanged.Invoke(new DTUConnectionStatusChangedMsg
                    {
                        DTUID = dtuCode,
                        IsOnline = true,
                        TimeStatusChanged = DateTime.Now //c.LoginTime
                    });
                }
                if (c is GprsDtuConnection)
                {
                    var cg = c as GprsDtuConnection;
                    Log.InfoFormat("Gprs Node Online: {0}, ip={1}, phone={2}.", cg.DtuID, cg.IP, cg.PhoneNumber);
                }
                else if (c is FileDtuConnection)
                {
                    var cf = c as FileDtuConnection;
                    Log.InfoFormat("File Node Online: {0}, path={1}.", cf.DtuID, cf.FilePath);
                }
                else
                {
                    Log.WarnFormat("Node Unkown Type Online");
                }
                CheckDtuInfo(c);
                DACWorker w = FindWorker(dtuCode);

                if (w != null)
                {
                    DacTaskContext ctx = w.GetContext();
                    ctx.UpdateConnection(c);
                    w.AssignContext(ctx);
                }
            }
            else if (newStat == WorkingStatus.NA)
            {
                if (c is GprsDtuConnection)
                {
                    var cg = c as GprsDtuConnection;
                    Log.InfoFormat("Gprs Node Offline: {0}, ip={1}, phone={2}.", cg.DtuID, cg.IP, cg.PhoneNumber);
                }
                else if (c is FileDtuConnection)
                {
                    var cf = c as FileDtuConnection;
                    Log.InfoFormat("File Node Offline: {0}, path={1}.", cf.DtuID, cf.FilePath);
                }
                else
                {
                    Log.WarnFormat("Node Unkown Type Offline");
                }
                // offline;
                if (OnDTUConnectionStatusChanged != null)
                {
                    OnDTUConnectionStatusChanged.Invoke(new DTUConnectionStatusChangedMsg
                    {
                        DTUID = dtuCode,
                        IsOnline = false,
                        TimeStatusChanged = DateTime.Now
                    });
                }
            }
        }

        private void CheckDtuInfo(IDtuConnection c)
        {
            if (!_dtuCodeMap.ContainsKey(c.DtuID))
            {
                // 新DTU 上线.
                AddDtu(DbAccessorHelper.DbAccessor.QueryDtuNode(c.DtuID));
            }
        }

        private static void OnTimedTaskFinished(DACTaskResult result)
        {
            int successedCount = 0;
            var sbR = new StringBuilder();
            var sbP = new StringBuilder();
            foreach (var sr in result.SensorResults)
            {
                if (sr.IsOK && sr.Data != null)
                {
                    successedCount++;
                }
                else
                {
                    if (sr.IsOK && sr.Data == null)
                    {
                        sbP.Append(sr.Sensor.SensorID).Append(",");
                    }
                    sbR.Append(sr.Sensor.SensorID).Append(",");
                }
            }
            try
            {
                Log.InfoFormat(
                    "Dtu {7}-{8} Cron task finished. result={0}-{1}, {2} sensors,successed: {4},failed sensors:{5}, parse error sensors:{6}; cost={3} ms",
                    result.ErrorCode,
                    result.ErrorMsg,
                    result.GetSensorCount(),
                    result.Elapsed,
                    successedCount,
                    sbR.Length != 0 ? sbR.ToString().Substring(0, sbR.Length - 1) : string.Empty,
                    sbP.Length != 0 ? sbP.ToString().Substring(0, sbP.Length - 1) : string.Empty, result.Task.DtuID,
                    result.DtuCode);
                DACTaskResultConsumerService.OnDacTaskResultProduced(result);
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Consumer error : {0}", ex.Message);
            }
        }

        class JobInfo
        {
            public int ID { get; set; }
            public WorkFinder Finder { get; set; }
            public String TID { get; set; }
            public DtuNode Dtu { get; set; }
            public TaskResultConsumer Consumer { get; set; }
            public IList<uint> Sensors { get; set; }
            public TaskType Type { get; set; }
            public override string ToString()
            {
                return string.Format("{1}-{0}", Dtu.DtuCode, Type);
            }
        }

        class DTUDacJob : IJob
        {
            private static ILog log = DACTaskManager.Log;

            public void Execute(IJobExecutionContext context)
            {
                JobInfo ji = GetJobInfo(context.JobDetail);

                if (ji.Type == TaskType.INSTANT)
                {
                    Console.WriteLine("remove job {1} {0}", DateTime.Now, ji.Dtu.DtuId);
                    TryRemoveJob(ji.Dtu.DtuId);
                }
                log.InfoFormat("Execute Job {0} , type={1}", context.JobDetail.Key, ji.Type);
                try
                {
                    DACWorker worker = ji.Finder.FindWorker(ji.Dtu.DtuId);
                    DACTask task = new DACTask(ji.TID, ji.Dtu.DtuId, ji.Sensors, ji.Type, ji.Consumer, ji.ID);
                    if (ji.Type != TaskType.INSTANT)
                    {
                        task.TID = GetTaskGuid(ji.Dtu.DtuCode);
                    }
                    if (!IsWorkerAvaliable(worker)) 
                    {
                        log.InfoFormat("Worker not avaliable: job=[{0}], worker=[{1}]", ji, GetWorkerInfo(worker));
                        if (ji.Type == TaskType.INSTANT)
                        {
                            int errorcode;
                            if (worker != null && (worker.GetContext() == null || (worker.GetContext().DtuConnection == null) || (worker.GetContext().DtuConnection != null && !worker.GetContext().DtuConnection.IsOnline)))
                                errorcode = (int) Errors.ERR_NOT_CONNECTED;
                            else if (worker != null && !worker.IsIdle())
                                errorcode = (int) Errors.ERR_DTU_BUSY;
                            else
                                errorcode = (int) Errors.ERR_UNKNOW;
                            OnInstantTaskFinished(new FailedDACTaskResult(errorcode, task)
                            {
                                Finished = DateTime.Now
                            });
                        }
                        return;
                    }
                    log.InfoFormat("Worker start working. job={0}, worker=[{1}]", ji, GetWorkerInfo(worker));
                    
                    if (task.Type == TaskType.INSTANT)
                    {
                        task.Consumer += OnInstantTaskFinished;
                    }
                    worker.NewTask(task);
                }
                catch (Exception e)
                {
                    log.Error("DTUDacJob Execute ERR", e);
                }
            }
        }

        internal static string GetTaskGuid(string dtucode)
        {
            DtuGroup dg = dtuGroups.FirstOrDefault(sg => sg.Exists(dtucode));
            if (dg == null)
            {
                return Guid.NewGuid().ToString();
            }
            return dg.GetGuid(dtucode, DateTime.Now);
        }

        internal void InitializeDtuGroups()
        {
            try
            {
                var groups = DbAccessorHelper.DbAccessor.QuerySensorGroups();
                foreach (SensorGroup sg in groups)
                {
                    DtuGroup dg = dtuGroups.FirstOrDefault(g => g.Exists(sg.GroupId) || g.Exists(sg.DtuId));
                    if (dg != null)
                        dg.Add(sg);
                    else
                    {
                        DtuGroup dgp = new DtuGroup();
                        dgp.Add(sg);
                        dtuGroups.Add(dgp);
                    }
                }
            }
            catch (Exception e)
            {
                Log.WarnFormat("InitializeDtuGroups error: {0}",e);
            }
        }

        // 即时采集任务固定回调, 用于处理数据库任务记录
        private static void OnInstantTaskFinished(DACTaskResult result)
        {
            // 
            Log.DebugFormat("OnInstaceTaskFinished :t={0}, r={1}, status={2}", result.Task.ID, result.ErrorCode, result.Task.Status);
            DACTaskPersistent.SaveTaskResult(result);
            if (result.Task.Status == DACTaskStatus.DONE)
            {
                uint dtuId = result.Task.DtuID;
                DACTask task;
                if (_rtasks.ContainsKey(dtuId))
                {
                    _rtasks.TryRemove(dtuId, out task);
                }
                TryRemoveJob(dtuId);
                Console.WriteLine("remove job {1} {0}", DateTime.Now, dtuId);
            }
        }
        
        private static string GetJobKey(uint dtuId)
        {
            return string.Format("{0}{1}", InstantJobName, dtuId); ;
        }

        private static void TryRemoveJob(uint dtuId)
        {
            JobKey jk = new JobKey(GetJobKey(dtuId), JobGroup);
            _schedule.DeleteJob(jk);
        }

        // worker idle && connection isAvaliable
        private static bool IsWorkerAvaliable(DACWorker worker){
            Log.DebugFormat("{0}", worker == null ? "worker is null" : worker.ToString());
            return worker != null
                && worker.GetContext() != null
                && worker.IsIdle()
                && worker.GetContext().IsAvaliable();
        }

        private static string GetWorkerInfo(DACWorker worker){
            string info =null;
            if (worker == null)
            {
                info = "null";
            }
            else
            {
                DacTaskContext ctx = worker.GetContext();
                if (ctx != null)
                {
                    IDtuConnection c = ctx.DtuConnection;
                    DtuNode d = ctx.Node;
                    info = string.Format("Node={0}, conn={1}", d.DtuCode, c != null ? (c.IsOnline ? "online" : "offline") : "null");
                }
                else
                {
                    info = "Context null";
                }
            }
            return info;
        }
        
        private static void AssignJobInfo(WorkFinder finder, TaskType type, IJobDetail job, string tid, DtuNode dtu, IList<uint> sensors, TaskResultConsumer consumer,int taskid=0)
        {
            job.JobDataMap.Put("INFO", new JobInfo { Finder = finder, Type = type, TID = tid, Dtu = dtu, Sensors = sensors, Consumer = consumer, ID = taskid });
        }

        private static JobInfo GetJobInfo(IJobDetail job)
        {
            return (JobInfo) job.JobDataMap.Get("INFO");
        }
        
        private void CreateSchedule(int poolSize)
        {
            if (_schedule == null || _schedule.IsShutdown)
            {
                poolSize = Math.Min(Math.Max(poolSize, MIN_POOL_SIZE), MAX_POOL_SIZE); //to [min,max]
                Log.InfoFormat("TaskManager PoolSize = {0}", poolSize);
                NameValueCollection props = new NameValueCollection();
                //简单线程池管理.
                props["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool,Quartz";
                //内存存储
                props["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore,Quartz";
                props["quartz.threadPool.threadCount"]  = ""+poolSize; // 线程池数量, 合理数量: DTU 活动DTU

                ISchedulerFactory sf = new StdSchedulerFactory(props);
                _schedule = sf.GetScheduler();
            }
        }

        // 处理 日常采集任务
        public void ArrangeTimedTask()
        {
            lock (this)
            {
                this.CreateSchedule(_dtus.Count);
                this.InitializeDtuGroups();
                IList<DtuNode> nodes = _dtus.Values.ToList();
                DateTimeOffset startTime = DateBuilder.NextGivenMinuteDate(null, 1);
                foreach (DtuNode dtu in nodes)
                {
                    DtuGroup dgp = dtuGroups.FirstOrDefault(d => d.Exists(dtu.DtuCode));
                    this.ScheduleJob(dtu, startTime, dgp != null ? dgp.DacInterval : dtu.DacInterval);
                }
                _schedule.Start();
            }
        }

        private void ScheduleJob(DtuNode dtu, DateTimeOffset startTime, uint dacinterval = DtuNode.DefaultDacInterval, string group = JobGroup)
        {
            if (_schedule.CheckExists(new JobKey(string.Format("{0}{1}", DailyJobName, dtu.DtuCode), group)))
            {
                Log.DebugFormat("Exist Job: {0}, {1},{2}", dtu.DtuId, dtu.Name, dtu.DtuCode);
                return;
            }
            Log.DebugFormat("Arrange Timed Job: {0}, {1},{2}", dtu.DtuId, dtu.Name, dtu.DtuCode);
            // 任务
            IJobDetail job = JobBuilder.Create<DTUDacJob>()
                .WithIdentity(string.Format("{0}{1}", DailyJobName, dtu.DtuCode), group)
                .Build();

            AssignJobInfo(this, TaskType.TIMED, job, new Guid().ToString(), dtu, null, OnTimedTaskFinished);

            //// 调度策略
            //// DateTimeOffset startTime = DateBuilder.NextGivenMinuteDate(null, 1); //开始时间.
            //// 重复间隔: dtu.DacInterval (秒);
            //// 重复次数: 无限
            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity(string.Format("{0}{1}", DailyTrigger, dtu.DtuCode), group)
                .WithSimpleSchedule(x => x.WithInterval(new TimeSpan(0, 0, (int)dacinterval)).RepeatForever())
                .StartAt(startTime)
                .Build();
            // TODO 任务重入!!!
            _schedule.ScheduleJob(job, trigger);
        }
        
        DACTask CreateInstantDACTask(string tid, uint dtuId, IList<uint> sensors, TaskResultConsumer consumer, bool activeFromDB)
        {
            if (DtuType == Model.DtuType.Com)
            {
                Log.ErrorFormat("COM dtu doesn't support instance task.");
                return null;
            }
            DACTask task = null;
            if (activeFromDB)
            {
                if (HasUnfinishedTask(dtuId, out task))
                    return task;
            }
            else
            {
                if (!_dtus.ContainsKey(dtuId))
                    return null;
                task = new DACTask(tid, dtuId, sensors, TaskType.INSTANT, null);
                DACTaskPersistent.SaveTaskImmedate(task);
                _rtasks[dtuId] = task;
            }
            return task;
        }

        /// <summary>
        /// 处理 即时采集任务
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="dtuId">要采集的DTU</param>
        /// <param name="sensors">要采集的传感器，为空或Count=0时，指全部传感器。</param>
        /// <param name="consumer"></param>
        /// <param name="reactive">从数据库重新激活任务,默认=false</param>
        /// <returns> -1 </returns>
        public int ArrangeInstantTask(string tid, uint dtuId, IList<uint> sensors, TaskResultConsumer consumer, bool activeFromDB = false) 
        {
            DtuNode dtu = null;
            DACTask task = CreateInstantDACTask(tid, dtuId, sensors, consumer, activeFromDB);
            if (task == null) {
                return -1; //无法创建任务.
            }
            dtu = _dtus[dtuId];

            bool ScheduleStarted = true;
            if (_schedule == null)
            {
                CreateSchedule(_dtus.Count);
                ScheduleStarted = false;
            }
            // Arrange an instant task to schedule
            string jobKey = GetJobKey(dtuId);
            JobKey jk = new JobKey(jobKey, JobGroup);
            if (_schedule.GetJobDetail(jk) != null)
            {
                Log.DebugFormat("Task already exist? {0}", jk);
                return -1;
            }
            IJobDetail job = JobBuilder.Create<DTUDacJob>()
                .WithIdentity(jobKey, JobGroup)
                .Build();
            AssignJobInfo(this, TaskType.INSTANT, job, tid, dtu, sensors, consumer,task.ID);
            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity(string.Format("{0}{1}", InstantTrigger, dtu.DtuId), JobGroup)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(5000)
                    .WithRepeatCount(1))
                    .StartNow()
               //.StartAt(DateBuilder.NextGivenMinuteDate(null, 10))
                .Build();
            _schedule.ScheduleJob(job,  trigger);
            Console.WriteLine("Add job {1} {0}", DateTime.Now, job.Key.Name);
            if (!ScheduleStarted)
                _schedule.Start();
            Log.DebugFormat("Instance DAC Job created: {0}, task.ID={1}", jobKey, task.ID);
            //try
            //{
            //    _schedule.TriggerJob(jk);
            //}
            //catch (Exception ex)
            //{
            //    OnInstantTaskFinished(new FailedDACTaskResult(Errors.ERR_NOT_CONNECTED)
            //    {
            //        Task = task,
            //        Finished = DateTime.Now
            //    });
            //    Log.Error(ex.Message);
            //}
            return task.ID;
        }

        private bool HasUnfinishedTask(uint dtuId, out DACTask t)
        {
            if (_rtasks.ContainsKey(dtuId))
            {
                t = _rtasks[dtuId];
                return true;
            }
            else
            {
                t = null;
                return false;
            }
        }

        public DACWorker FindWorker(uint dtuId)
        {
            if (_workers.ContainsKey(dtuId)&&_workers[dtuId]!=null)
            {
                return _workers[dtuId];
            }else{
                DACWorker w = new DACWorker(_adapterManager, OnSensorCollectMsgHandler);
                w.AssignContext(CreateContext(dtuId));
                _workers[dtuId] = w;
                w.StartWork();
                return w;
            }
        }

        private DACWorker FindWorker(string dtuCode)
        {
            if (_dtuCodeMap.ContainsKey(dtuCode)) {
                DtuNode d = _dtuCodeMap[dtuCode];
                return FindWorker(d.DtuId);
            }
            return null;
        }

        private DacTaskContext CreateContext(uint dtuId)
        {
            DacTaskContext _ctx = new DacTaskContext { Node = _dtus[dtuId], SensorMatcher = SensorMatcher };
            if (_ctx.Node.NetworkType == NetworkType.hclocal)
                _ctx.DtuConnection = _fileServer.GetConnection(_ctx.Node);
            else
                _ctx.DtuConnection = _dtuServer.GetConnection(_ctx.Node);
            return _ctx;
        }

        public void Stop()
        {
            _schedule.Shutdown();
            foreach (DACWorker w in _workers.Values)
            {
                w.Stop();
            }
            _schedule.Clear();
            _workers.Clear();
            DACTaskPersistent.Stop();
            ATTaskPersistent.Stop();
        }

        public int ArrangeInstantTask(string tid, uint dtuId, JObject[] cmdobjs, ATTaskResultConsumer consumer)
        {
            ATTask task = new ATTask(tid, dtuId, cmdobjs, TaskType.INSTANT, consumer);
            ATTaskPersistent.Init();
            ATTaskPersistent.SaveTaskImmedate(task);
            task.Consumer += OnAtInstantTaskFinished;
            DacTaskContext context = null;
            if (_workers.ContainsKey(dtuId))
            {
                if (!IsWorkerAvaliable(_workers[dtuId]))
                {
                    _workers[dtuId].Stop();
                }
                context = _workers[dtuId].GetContext();
            }
            else
            {
                context = CreateContext(dtuId);
            }
            var atworker = new AtCommandWorker(task,context);
            atworker.StartWork();
            return task.ID;
        }

        //DTU 配置回调, 用于处理数据库任务记录
        private static void OnAtInstantTaskFinished(ExecuteResult result)
        {
            ATTaskPersistent.SaveTaskResult(result);
            Log.DebugFormat("OnInstaceTaskFinished :t={0}, r={1}, status={2}", result.Task.ID, result.ErrorCode, result.Task.Status);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        public void UpdateSensorConfig(SensorOperation operation)
        {
            if (_senconfChangedServer == null)
            {
                _senconfChangedServer = new SensorConfigUpdateServer();
                _senconfChangedServer.GetDtuNodeListener += senconfChangedServer_GetDtuNodeListener;
                _senconfChangedServer.StartWork();
            }
            _senconfChangedServer.TryAddNewSensorOperation(operation);
        }

        DtuNode senconfChangedServer_GetDtuNodeListener(uint dtuid)
        {
            if (_dtus.ContainsKey(dtuid))
                return _dtus[dtuid];
            return null;
        }
        
        public bool DtuConfigChanged(ChangedStatus cmd, DtuNode dtu,string dtucode=null)
        {
            switch (cmd)
            {
                case ChangedStatus.Add:
                    return dtu.NetworkType == NetworkType.gprs ? _dtuServer.Disconnect(dtu.DtuCode) : _fileServer.Disconnect(dtu);
                case ChangedStatus.Modify:
                   return ModifyDtu(dtu, dtucode);
                case ChangedStatus.Delete:
                   return RemoveDtu(dtu);
            }
            return false;
        }

        public bool UpdateSensorConfigInfo()
        {
            try
            {
                IList<DtuNode> dtus = DbAccessorHelper.DbAccessor.QueryDtuNodes();
                _schedule.Shutdown(true);
                while (!_schedule.IsShutdown)
                {
                    Thread.Sleep(1000);
                }
                Console.WriteLine("schedule is shutdown");
                this.Clear();
                foreach (DtuNode d in dtus)
                {
                    _dtus[d.DtuId] = d;
                    _dtuCodeMap[d.DtuCode] = d;
                }

                ArrangeTimedTask();
                Console.WriteLine("update {0} dtus , ready to work.", _dtus.Count);
                Log.InfoFormat("update {0} dtus , ready to work.", _dtus.Count);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Reload sensors' info,error : {0}", ex.Message);
                Log.ErrorFormat("Reload sensors' info,error : {0}",ex.Message);
                return false;
            }
        }

        private void Clear()
        {
            _workers.Clear();
            _dtus.Clear();
            _dtuCodeMap.Clear();
            _rtasks.Clear();
            dtuGroups.Clear();
        }

        /// <summary>
        /// 删除DTU
        /// </summary>
        /// <param name="dtu"></param>
        private bool RemoveDtu(DtuNode dtu)
        {
            DtuNode dn;
            if (_dtus.ContainsKey(dtu.DtuId))
            {
                _dtus.TryRemove(dtu.DtuId, out dn);
            }
            if (_dtuCodeMap.ContainsKey(dtu.DtuCode))
            {
                _dtuCodeMap.TryRemove(dtu.DtuCode, out dn);
            }
            TryRemoveJob(dtu.DtuCode);
            return true;
        }

        /// <summary>
        /// 修改DTU
        /// </summary>
        /// <param name="dtu"></param>
        private bool ModifyDtu(DtuNode dtu,string dtucode)
        {
            //dtu编号没有改变
            if (dtu.DtuCode == dtucode)
            {
                dtu.DacInterval = dtu.DacInterval*60;
                _dtus[dtu.DtuId].Name = dtu.Name;
                _dtuCodeMap[dtu.DtuCode].Name = dtu.Name;
                if (_dtus[dtu.DtuId].DacInterval != dtu.DacInterval)
                {
                    _dtuCodeMap[dtu.DtuCode].DacInterval = dtu.DacInterval;
                    // 重新调度该DTU任务
                    ModifyJobTime(dtu);
                }
                return true;
            }
            if (_dtuCodeMap.ContainsKey(dtu.DtuCode))
            {
                DtuNode dn;
                _dtuCodeMap.TryRemove(dtu.DtuCode, out dn);
            }
            return TryRemoveJob(dtu.DtuCode);
        }

        public string GetDailyJobKey(string dtuCode)
        {
            return string.Format("{0}{1}", DailyJobName, dtuCode);
        }

        public string GetDailyTriggerKey(uint dtuId)
        {
            return string.Format("{0}{1}", DailyTrigger, dtuId);
        }

        /// <summary>
        /// 从日常采集任务中移除
        /// </summary>
        /// <param name="dtuCode"></param>
        private bool TryRemoveJob(string dtuCode)
        {
            var jk = new JobKey(GetDailyJobKey(dtuCode), JobGroup);
            return _schedule.DeleteJob(jk);
        }

        public bool ModifyJobTime(DtuNode dtu)
        {
            if (_schedule == null)
                return false;
            string triggerKeystr = GetDailyTriggerKey(dtu.DtuId);
            ITrigger trigger = _schedule.GetTrigger(new TriggerKey(triggerKeystr, JobGroup));
            if (trigger != null)
            {
                _schedule.PauseJob(new JobKey(GetDailyJobKey(dtu.DtuCode), JobGroup));
                TryRemoveJob(dtu.DtuCode);
                _schedule.UnscheduleJob(new TriggerKey(triggerKeystr, JobGroup));
                ScheduleJob(dtu, DateBuilder.NextGivenMinuteDate(null, (int) dtu.DacInterval/60));


                //IJobDetail job = JobBuilder.Create<DTUDacJob>()
                //    .WithIdentity(string.Format("{0}{1}", DailyJobName, dtu.DtuCode), JobGroup)
                //    .Build();
                //AssignJobInfo(this, TaskType.TIMED, job, new Guid().ToString(), dtu, null, OnTimedTaskFinished);
                //// 调度策略
                //DateTimeOffset startTime = DateBuilder.NextGivenMinuteDate(null, (int) dtu.DacInterval/60); //开始时间.
                //// 重复间隔: dtu.DacInterval (秒);
                //// 重复次数: 无限
                //trigger = (ISimpleTrigger) TriggerBuilder.Create()
                //    .WithIdentity(triggerKeystr, JobGroup)
                //    .WithSimpleSchedule(x => x.WithInterval(new TimeSpan(0, 0, (int) dtu.DacInterval)).RepeatForever())
                //    .StartAt(startTime)
                //    .Build();
                //_schedule.ScheduleJob(job, trigger);
                if (!_schedule.IsStarted)
                    _schedule.Start();
                return true;
            }
            return false;
        }

        public bool GetDtuStatus(string dtucode)
        {
            if (_dtuCodeMap.ContainsKey(dtucode))
                return this._dtuServer.GetConnection(_dtuCodeMap[dtucode]).IsOnline;
            return false;
        }

        public void AddNewDtu()
        {
            
        }

    }
}
