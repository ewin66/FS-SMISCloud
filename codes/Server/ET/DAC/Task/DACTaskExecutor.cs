using System;
using System.IO;
using System.Linq;
using System.Threading;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;
using log4net;
using System.Collections.Generic;
using FS.DynamicScript;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using FS.SMIS_Cloud.DAC.Util;

namespace FS.SMIS_Cloud.DAC.Task
{
    using System.Diagnostics;
    using File;

    /// <summary>
    /// Node 采集任务执行器.
    /// </summary>
    public class DACTaskExecutor
    {
        private static ILog log = LogManager.GetLogger("TaskRobot");
        private IAdapterManager _adapterManager = null;
        private const int DELAY_BEFORE_NEW_COMMAND_SPECIAL = 10000;
        private const int DELAY_BEFORE_NEW_COMMAND = 50;
        private const int FILE_SIZE_LIMIT = 1000;

        public DACTaskExecutor(IAdapterManager _adapterManager, SensorCollectMsgListener listener=null)
        {
            this._adapterManager = _adapterManager;
            this._sensorCollectMsgHandler = listener;
        }

        private readonly SensorCollectMsgListener _sensorCollectMsgHandler;

        public DACTaskResult Run(DACTask task, DacTaskContext context)
        {
            DACTaskResult rslt;
            DtuNode dtu = context.Node;

            switch (dtu.NetworkType)
            {
                case NetworkType.gprs:
                    rslt = this.ExcuteGprsDacTask(task, context);
                    break;
                case NetworkType.hclocal:
                    rslt = this.ExcuteLocalDacTask(task, context);
                    break;
                default:
                    log.ErrorFormat("Dtu:{0}-Network Type:{1} : Error, Executor Is Not Implemented", dtu.DtuCode, dtu.NetworkType);
                    rslt = new DACTaskResult();
                    rslt.Task = task;
                    rslt.Task.Status = DACTaskStatus.DONE;
                    rslt.ErrorCode = (int)Errors.ERR_INVALID_DTU;
                    rslt.ErrorMsg = string.Format("Dtu:{0},Network Type Error", dtu.DtuCode);
                    rslt.Finished = System.DateTime.Now;
                    break;
            }

            return rslt;
        }

        private DACTaskResult ExcuteLocalDacTask(DACTask task, DacTaskContext context)
        {
            DtuNode dtu = context.Node;
            DACTaskResult rslt = new DACTaskResult();
            rslt.Task = task;
            rslt.Task.Status = DACTaskStatus.RUNNING;
            rslt.ErrorMsg = "OK";
            rslt.StoragedTimeType = SensorAcqResultTimeType.SensorResponseTime;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var dtuConn = context.DtuConnection as FileDtuConnection; // 强转成FileDtuConnection
            if (dtuConn != null && dtuConn.Connect())
            {
                try
                {
                    foreach (Sensor si in dtu.Sensors)
                    {
                        if (!this.IsSensorRequired(context, task.Sensors, si))
                        {
                            continue;
                        }

                        var result = this.RequestFileSensor(si, dtuConn);// 请求文件传感器
                        foreach (SensorAcqResult r in result)
                        {
                            rslt.AddSensorResult(r);
                        }
                    }
                    CheckFileAndBackup(dtuConn.FilePath);
                }
                catch (System.Exception e)
                {
                    rslt.ErrorCode = (int)Errors.ERR_NOT_CONNECTED;
                    rslt.ErrorMsg = string.Format(
                        "DTU:{0},FilePath:{1},Failed To Read the File,Msg:{2}",
                        dtu.DtuCode,
                        dtuConn.FilePath,
                        e.Message);
                    log.ErrorFormat(
                        "dtu:{0} network={1},file={2},Failed To Read the File,Msg:{3}",
                        dtu.DtuCode,
                        dtu.NetworkType,
                        dtuConn.FilePath,
                        e.Message);
                }
            }
            else
            {
                rslt.ErrorCode = (int)Errors.ERR_NOT_CONNECTED;
                rslt.ErrorMsg = string.Format(
                    "DTU:{0},FilePath:{1},File Not Exists",
                    dtu.DtuCode,
                    dtuConn != null ? dtuConn.FilePath : null);
                log.ErrorFormat("dtu:{0} network={1},file={2},File Not Exists", dtu.DtuCode, dtu.NetworkType, dtuConn != null ? dtuConn.FilePath : null);
            }
            sw.Stop();
            rslt.Finished = System.DateTime.Now;
            rslt.Elapsed = sw.ElapsedMilliseconds;
            rslt.Task.Status = DACTaskStatus.DONE;
            context.DtuConnection.Disconnect();

            return rslt;
        }

        private IEnumerable<SensorAcqResult> RequestFileSensor(Sensor si, FileDtuConnection conn)
        {
            log.InfoFormat(
                "======> sensor:{0}-{1} p={2},m={3},channel={4},filepath={5}",
                conn.DtuID,
                si.SensorID,
                si.ProtocolType,
                si.ModuleNo,
                si.ChannelNo,
                conn.FilePath);
            var _adapter = this._adapterManager.GetSensorAdapter(si.ProtocolType) as IFileSensorAdapter;// 强转成FileSensorAdapter
            var rslt = new List<SensorAcqResult>();

            if (_adapter != null)
            {
                var data = _adapter.ReadData(si, conn.FilePath);// 从文件识别数据行
                foreach (byte[] d in data)
                {
                    var r = new SensorAcqResult { Request = null, Sensor = si, RequestTime = System.DateTime.Now, Response = d };
                    _adapter.ParseResult(ref r);// 解析数据行
                    r.ErrorCode = (int)Errors.SUCCESS;
                    r.ErrorMsg = "OK!";

                    rslt.Add(r);
                    log.InfoFormat(
                        "<====== result='{0}-{1}', data = {2}",
                        r.ErrorCode,
                        r.ErrorMsg,
                        r.Data == null ? "null" : r.Data.JsonResultData);
                }
            }
            else
            {
                var r = new SensorAcqResult
                {
                    Request = null,
                    Sensor = si,
                    RequestTime = System.DateTime.Now,
                    Response = null,
                    ErrorCode = (int)Errors.ERR_UNKNOW_PROTOCOL,
                    ErrorMsg = "传感器无编码器",
                    Data = null
                };
                rslt.Add(r);

                log.InfoFormat(
                    "<====== result='{0}-{1}', data = {2}",
                    r.ErrorCode,
                    r.ErrorMsg,
                    r.Data == null ? "null" : r.Data.JsonResultData);
            }
            return rslt;
        }

        private DACTaskResult ExcuteGprsDacTask(DACTask task, DacTaskContext context)
        {
            DtuNode dtu = context.Node;
            var r = new DACTaskResult {Task = task};
            r.Task.Status = DACTaskStatus.RUNNING;
            r.ErrorMsg = "OK";
            r.DtuCode = dtu.DtuCode;
            // 循环发送数据
            long totalElapsed = 0;
            context.DtuConnection.Connect(); // Connect to Resource.
            r.Started = DateTime.Now;
            foreach (Sensor si in dtu.Sensors)
            {
                SensorAcqResult resp = null;
                if (!this.IsSensorRequired(context, task.Sensors, si))
                {
                    continue;
                }
                
                try
                {
                    resp = this.RequestGprsSensor(si, context.DtuConnection, (int)dtu.DacTimeout);
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("[DACTaskExecutor] : unknown exceptions {0}", ex.Message);
                    if (resp == null)
                    {
                        resp = this.CreateAcqResult(context.DtuConnection.DtuID, si, (int)Errors.ERR_UNKNOW, "unknown exceptions");
                    }
                }
                if (resp.Data == null)
                {
                    resp.Data = new SensorErrorData(si.SensorID, resp.ErrorCode);
                }
                r.AddSensorResult(resp);
                totalElapsed += resp.Elapsed;
                if (!r.IsOK)
                {
                    r.ErrorCode = resp.ErrorCode;
                    r.ErrorMsg = resp.ErrorMsg;
                    break; // ERROR.
                }

                Thread.Sleep(DELAY_BEFORE_NEW_COMMAND); //延迟.
            }
            r.Finished = DateTime.Now;
            r.Elapsed = totalElapsed;
            r.Task.Status = DACTaskStatus.DONE;
            context.DtuConnection.Disconnect();

            return r;
        }

        private static List<uint> SENSOR_TO_DELAY = new List<uint> { ProtocolType.TempHumidity, ProtocolType.VibratingWire, ProtocolType.VibratingWire_OLD, ProtocolType .FTM50SSLaser};

        private bool isSensorRequireDelay(uint st)
        {
            return SENSOR_TO_DELAY.Contains(st);
        }

        private SensorAcqResult RequestGprsSensor(Sensor si, IDtuConnection conn, int timeout)
        {
            var r = new SensorAcqResult
            {
                DtuCode = conn.DtuID,
                Sensor = si,
                Request = null,
                Response = null,
                Data = null,
                ErrorCode = (int)Errors.ERR_DEFAULT
            };
            SendSensorCollectMsg(CollectState.Request, r);
             var senadapter = _adapterManager.GetAdapter(si.ProtocolType);
            //var senadapter = _adapterManager.GetSensorAdapter(si.ProtocolType);
            if (senadapter==null)
            {
                return CreateAcqResult(conn.DtuID, si, (int)Errors.ERR_UNKNOW_PROTOCOL, "Sensor has no ProtocolCode");
            }
            
            try
            {
                //senadapter.Request(ref r);
                var mp = new object[] { r };
                object[] cp = null;
                CrossDomainCompiler.Call(senadapter.ScriptPath, typeof(ISensorAdapter), senadapter.ClassName, "Request", ref cp, ref mp);
                r = mp[0] as SensorAcqResult;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("dtu{0} sensor:{1} create cmd error {2}", conn.DtuID, si.SensorID, ex.Message);
                return CreateAcqResult(conn.DtuID, si, (int)Errors.ERR_COMPILED, "internal error: SensorAdapter ERROR");
            }
            if (r == null)
            {
                return CreateAcqResult(conn.DtuID, si, (int)Errors.ERR_CREATE_CMD, "create cmd error: SensorAdapter ERROR");
            }
            r.RequestTime = DateTime.Now;
            if(r.ErrorCode != (int)Errors.SUCCESS)
                return CreateAcqResult(conn.DtuID, si, r.ErrorCode, "Sensor has no SensorAdapter");
            // send 
            if (r.ErrorCode == (int)Errors.SUCCESS && r.Request != null)
            {
                DtuMsg msg = conn.Ssend(r.Request, timeout);
                if (msg == null)
                {
                    return CreateAcqResult(conn.DtuID, si, (int)Errors.ERR_NULL_RECEIVED_DATA, "Receive buff is null !");
                }
                r.ResponseTime = msg.Refreshtime; // 若结果错误， 该时间无意义。
                // Parse
                if (msg.IsOK())
                {
                    try
                    {
                        r.Response = msg.Databuffer;
                        if (r.Response != null)
                        {
                            //senadapter.ParseResult(ref r);
                            var mp = new object[] { r };
                            object[] cp = null;
                            CrossDomainCompiler.Call(senadapter.ScriptPath, typeof(ISensorAdapter), senadapter.ClassName, "ParseResult", ref cp, ref mp);
                            r = mp[0] as SensorAcqResult;
                        }
                        else
                            log.ErrorFormat("sensor:{0}, error Received buff is null", r.Sensor.SensorID);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("dtu:{0},s[sid:{2}-m:{3}-c:{4}] , ERR_COMPILED {1}", conn.DtuID, ex.Message,si.SensorID,si.ModuleNo,si.ChannelNo);
                        return CreateAcqResult(conn.DtuID, si, (int)Errors.ERR_COMPILED, "internal error: COMPILED ERROR");
                    }
                    if (r == null)
                    {
                        return CreateAcqResult(conn.DtuID, si, (int)Errors.ERR_DATA_PARSEFAILED, "internal error: SensorAdapter ERROR");
                    }

                    r.ErrorMsg = r.ErrorCode == (int)Errors.SUCCESS ? "OK!" : ValueHelper.CreateJsonResultStr(si.SensorID, EnumHelper.GetDescription((Errors)r.ErrorCode));
                }
                else
                {
                    r.ErrorCode = msg.ErrorCode;
                    r.ErrorMsg = msg.ErrorMsg;
                }
                r.Elapsed = msg.Elapsed;
            }
            else
            {
                if (r.ErrorCode == (int)Errors.ERR_DEFAULT)
                    r.ErrorCode = (int)Errors.ERR_UNKNOW;
                r.ErrorMsg = "create cmd error";
            }
            if (r.ErrorCode != (int)Errors.SUCCESS)
                r.Data = new SensorErrorData(r.Sensor.SensorID, r.ErrorCode);
            SendSensorCollectMsg(CollectState.Response, r);
            return r;
        }

        private SensorAcqResult CreateAcqResult(string dtuid, Sensor si, int errcode, string errmsg)
        {
            var r = new SensorAcqResult
            {
                DtuCode = dtuid,
                Sensor = si,
                Request = null,
                Response = null,
                Data = null,
                ErrorCode = errcode,
                ErrorMsg = errmsg
            };
            r.Data = new SensorErrorData(r.Sensor.SensorID, r.ErrorCode);
            SendSensorCollectMsg(CollectState.Response, r);
            return r;
        }

        public bool IsSensorRequired(DacTaskContext ctx, IList<uint> list, Sensor s)
        {
            if (ctx.SensorMatcher != null)
            {
                if (!ctx.SensorMatcher(s))
                {
                    return false;
                }
            }
            if (list == null || list.Count <= 0)
            {
                return !s.UnEnable && s.SensorType != SensorType.Virtual;
            }
            //是否使能 false-是 true-否 //&&(!s.UnEnable)
            return list.Contains(s.SensorID) && s.SensorType != SensorType.Virtual;
        }

        private bool IsTime2Acq(Sensor s)
        {
            // 时间误差在2分钟以内
            if (Math.Abs(DateTime.Now.Subtract(s.LastTime).Minutes - s.AcqInterval) <= 2 ||
                Math.Abs(DateTime.Now.Subtract(s.LastTime).Minutes) > s.AcqInterval || s.LastTime == DateTime.MinValue)
            {
                s.LastTime = DateTime.Now;
                return true;
            }
            return false;
        }

        private void SendSensorCollectMsg(CollectState state,SensorAcqResult acqResult)
        {
            if (_sensorCollectMsgHandler != null)
            {
                this._sensorCollectMsgHandler.Invoke(state, acqResult);
            }
        }

        /// <summary>
        /// 判断文件大小，如果超过文件最大限制则移动备份文件
        /// </summary>
        private void CheckFileAndBackup(string file)
        {
            try
            {
                int linecount = 0;
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        linecount = reader.ReadToEnd().Split('\n').Length;
                        reader.Close();
                    }
                    fs.Close();
                }
                if (linecount > FILE_SIZE_LIMIT)
                {
                    BackupAndRemoveFile(file, DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                log.Warn(ex);
            }
        }

        /// <summary>
        /// 将文件移至Backup目录，依据时间重命名
        /// </summary>
        private void BackupAndRemoveFile(string file, DateTime datatime)
        {
            var destdir = Path.Combine(file.Substring(0, file.LastIndexOf(Path.DirectorySeparatorChar)), "Backup");
            if (!Directory.Exists(destdir))
                Directory.CreateDirectory(destdir);
            var destfile = Path.Combine(destdir, Path.GetFileName(file) + "." + datatime.ToString("yyyyMMddHHmmss"));
            try
            {
                if (System.IO.File.Exists(destfile))
                    System.IO.File.Delete(destfile);
                System.IO.File.Move(file, destfile);
            }
            catch (Exception ex)
            {
                log.DebugFormat("备份GPS文件时发生异常:{0}", ex.Message);
            }
        }
    }
}