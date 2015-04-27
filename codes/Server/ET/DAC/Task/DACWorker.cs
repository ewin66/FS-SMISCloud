
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Node;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace FS.SMIS_Cloud.DAC.Task
{
    // 矿工
    public class DACWorker
    {
        private static ILog Log = LogManager.GetLogger("Worker");
        // 资源；
        private DacTaskContext _context; //
        // 任务；
        private ConcurrentQueue<DACTask> taskPool = new ConcurrentQueue<DACTask>();
        private bool working;
        private CancellationTokenSource _source;
        private CancellationToken _token;
        private IAdapterManager adapter;
        private SensorCollectMsgListener _sensorCollectMsgHandler;


        public DACWorker(IAdapterManager adapter, SensorCollectMsgListener listener = null)
        {
            this.adapter = adapter;
            this._source = new CancellationTokenSource();
            this._token = this._source.Token;
            this._sensorCollectMsgHandler = listener;
        }
        
        public void StartWork() {
            System.Threading.Tasks.Task.Factory.StartNew(this._DoWork, this._token);
        }

        private void _DoWork()
        {
            while (!_source.IsCancellationRequested)
            {
                DACTask task;
                if (!this.taskPool.IsEmpty && this.taskPool.TryDequeue(out task))
                {
                    LetsDoIt(task);
                }
                Thread.Sleep(10);
            }
            Log.Debug("Worker stopped.");
        }
 
        private void LetsDoIt(DACTask t)
        {
            this.working = true;
            DACTaskResult result = null;
            if (_context!=null&&_context.IsAvaliable()&&_context.Node!=null)
            {
                try
                {
                    UpdateContext();
                    Log.Info("_context valid, start robot");
                    result = new DACTaskExecutor(adapter, _sensorCollectMsgHandler).Run(t, _context);
                    result.ErrorMsg = "OK";
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("DTU context : {0}--{1}, error {2}", t.DtuID, this._context != null && this._context.Node != null ? this._context.Node.DtuCode : string.Empty, ex.Message);
                    if (result == null)
                    {
                        result = new DACTaskResult
                        {
                            DtuCode = _context.Node.DtuCode,
                            Task = t,
                            ErrorCode = (int)Errors.ERR_UNKNOW,
                            ErrorMsg = "FAILED",
                            Finished = DateTime.Now
                        };
                        result.Task.Status = DACTaskStatus.FAIL;
                    }
                }
            }
            else
            {
                Log.Error("_context Busy, can't run.");
                int errcode;
                if (_context != null && _context.DtuConnection != null && _context.DtuConnection.IsOnline)
                    errcode = (int)Errors.ERR_DTU_BUSY;
                else if (_context != null && _context.DtuConnection != null && !_context.DtuConnection.IsOnline)
                    errcode = (int)Errors.ERR_NOT_CONNECTED;
                else
                    errcode = (int)Errors.ERR_INVALID_DTU;
                result = new FailedDACTaskResult(errcode,t);
            }
            if (t.Consumer != null)
            {
                t.Consumer(result);
            }
            this.working = false;
        }

        public void Stop()
        {
            _source.Cancel();
            this.working = false;
            Log.Debug("Terminating work");
        }

        // TODO 插队任务。
        public bool TryJoinTask(DACTask newTask)
        {
            return false;
        }

        internal void NewTask(DACTask t)
        {
            this.taskPool.Enqueue(t);
        }

        public bool IsIdle()
        {
            return !working;
        }

        public void AssignContext(DacTaskContext context)
        {
            this._context = context;
        }

        public DacTaskContext GetContext()
        {
            return _context;
        }

        private void UpdateContext()
        {
            if (_context != null && _context.Node != null)
            {
                try
                {
                    _context.Node.UpDateSensor();
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Dtu {0} Update sensorinfo error : {1}",_context.Node.DtuCode,ex.Message);
                }
            }
        }

        public override string ToString()
        {
            try
            {
                if (_context == null)
                    return string.Format("worker's context is null");
                if (_context.DtuConnection == null)
                    return string.Format("worker's context's DtuConnection is null");
                return string.Format("worker's context {0} DtuConnection's status is {1}, worker is {2}", _context.DtuConnection.DtuID,
                    _context.DtuConnection.Status, working ? "busy" : "idel");
            }
            catch (Exception ex)
            {
                return string.Format("work to string error : {0}", ex.Message);
            }
        }
    }
}
