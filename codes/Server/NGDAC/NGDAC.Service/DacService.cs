// --------------------------------------------------------------------------------------------
// <copyright file="AlarmService.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20140902
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

namespace FS.SMIS_Cloud.NGDAC
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;

    using FS.Service;
    using FS.SMIS_Cloud.NGDAC.Accessor;
    using FS.SMIS_Cloud.NGDAC.Accessor.MSSQL;
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.File;
    using FS.SMIS_Cloud.NGDAC.Gprs;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Task;
    using FS.SMIS_Cloud.NGDAC.Util;
    using FS.SMIS_Cloud.Services.ConsoleCtrlManager;

    using log4net;

    using Newtonsoft.Json;

    public class DacService :   Service
    {
        private const string MyName = "ngdac";
        private static readonly ILog Log = LogManager.GetLogger("NGDAC");
        internal DACTaskManager Dtm;
        private readonly IDtuServer _dacServer; // _DacReceiver;
        private readonly IDtuServer _fileServer; // _FileReceiver
        private readonly WarningHelper _warningHelper;
        private static DataStatusConsumer etDataStatus;
        private readonly MsgDealHelper _msgHelper = new MsgDealHelper();
        public DacService(string svrConfig, string busPath)
            : base(MyName, svrConfig, busPath)
        {
            string cs = ConfigurationManager.AppSettings["SecureCloud"];
            int dacServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["Port"], 10); 
            Log.InfoFormat("DBConnecting: {0}", cs);
            try
            {
                DbAccessorHelper.Init(new MsDbAccessor(cs));// 配置
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("系统初始化失败:{0}", ex.Message);
                return;
            }

            etDataStatus = new DataStatusConsumer(this);
            DACTaskManager.OnDacTaskFinished += etDataStatus.ProcessResult;
            DACTaskManager.OnDacTaskFinished += this.WriteDacTaskResultToJsonFile;

            this._warningHelper=new WarningHelper();
            this._dacServer = new GprsDtuServer(dacServerPort);
            this._fileServer = new FileDtuServer();
            this.Dtm = new DACTaskManager(this._dacServer, DbAccessorHelper.DbAccessor.QueryDtuNodes(), null); // DbAccessorHelper.DbAccessor.GetUnfinishedTasks()
            this.Dtm.SetFileServer(this._fileServer);
            // Push/Pull mode.
            this.Pull(null, this.OnMessageReceived);
            this.Response(this.ResponseHandler);
            this.Subscribe("cm", "/cm/datachanged/dtu/sensor/", this.OnSubscribed);
            ConsoleCtrlManager.Instance.RegCmd("reload", this.LoadHandler); // 注册重新加载传感器信息
            ConsoleCtrlManager.Instance.Exit += this.Instance_Exit;
            this._warningHelper.UpdateAllDtuStatus();
        }

        private string LoadHandler(string[] args)
        {
            if (this.Dtm.UpdateSensorConfigInfo())
            {
                return string.Format("成功刷新传感器信息");
            }

            return string.Format("刷新传感器信息失败");
        }

        public void OnSubscribed(string msg)
        {
        }

        public override void PowerOn()
        {
            Log.Info("PowerOn");
            this._dacServer.Start();
            this._fileServer.Start();
            DACTaskManager.OnDTUConnectionStatusChanged += DTUConnectionStatusChangedHandler;
            this.Dtm.OnSensorCollectMsgHandler += dtm_OnSensorCollectMsgHandler;
            this.Dtm.ArrangeTimedTask();
        }

        private void dtm_OnSensorCollectMsgHandler(CollectState state, DAC.SensorAcqResult acqResult)
        {
            switch (state)
            {
                case CollectState.Request:
                    Log.InfoFormat("======> Dtu:{4} sensor:{0} p={1},m={2},channel={3}", acqResult.Sensor.SensorID,
                        acqResult.Sensor.ProtocolType, acqResult.Sensor.ModuleNo, acqResult.Sensor.ChannelNo,acqResult.DtuCode);
                    return;
                case CollectState.Response:
                    Log.InfoFormat("<====== Dtu:{4} result='sensor:{0} m={5},channel={6} [{1}-{2}]', data = [{3}]",
                        acqResult.Sensor.SensorID, acqResult.ErrorCode, acqResult.ErrorMsg,
                        acqResult.Data == null ? "null" : acqResult.Data.JsonResultData, acqResult.DtuCode,
                        acqResult.Sensor.ModuleNo, acqResult.Sensor.ChannelNo);
                    break;
            }
        }

        private void WriteDacTaskResultToJsonFile(DACTaskResult rslt)
        {
            if (rslt.IsOK && rslt.SensorResults != null && rslt.SensorResults.Count > 0)
            {
                Log.Info("start to write data file..");
                try
                {
                    var dir = ConfigurationManager.AppSettings["RecordPath"];
                    var fileName = rslt.DtuCode + "-" + rslt.Finished.ToString("yyyyMMddHHmmss") + ".json";

                    using(var fs = new FileStream(dir + "\\" + fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (var sw = new StreamWriter(fs))
                        {
                            foreach (SensorAcqResult sensorResult in rslt.SensorResults)
                            {
                                var json = this.CastSensorResultToJson(rslt, sensorResult);
                                if (json != null)
                                {
                                    sw.WriteLine(json);
                                }
                            }
                        }
                    }

                    Log.InfoFormat("data file: [{0}] has been writen", fileName);
                }
                catch (Exception e)
                {
                    Log.Error("acq file record error", e);
                }
            }
        }

        private string CastSensorResultToJson(DACTaskResult rslt, SensorAcqResult sensorResult)
        {
            try
            {
                DateTime acqTime;
                switch (rslt.StoragedTimeType)
                {
                    case SensorAcqResultTimeType.TaskFinishedTime:
                        acqTime = rslt.Finished;
                        break;
                    case SensorAcqResultTimeType.TaskStartTime:
                        acqTime = rslt.Started;
                        break;
                    case SensorAcqResultTimeType.SensorRequestTime:
                        acqTime = sensorResult.RequestTime;
                        break;
                    case SensorAcqResultTimeType.SensorResponseTime:
                        acqTime = sensorResult.ResponseTime;
                        break;
                    default:
                        acqTime = rslt.Finished;
                        break;
                }

                var S = sensorResult.Sensor == null ? 0 : sensorResult.Sensor.SensorID;
                var R = sensorResult.ErrorCode;
                var N = (int)(acqTime - new DateTime(2010, 1, 1)).TotalSeconds;
                var T = (acqTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
                var Q = sensorResult.Request == null
                            ? new string[0]
                            : new[] { ValueHelper.BytesToHexStr(sensorResult.Request) };
                var A = sensorResult.Response == null
                            ? new string[0]
                            : new[] { ValueHelper.BytesToHexStr(sensorResult.Response) };
                var FI = sensorResult.Sensor == null ? 0 : (int)sensorResult.Sensor.FormulaID;
                var RV = sensorResult.Data.RawValues;
                var LV = sensorResult.Data.CollectPhyValues;
                var PV = sensorResult.Data.PhyValues;
                var TV = sensorResult.Data.ThemeValues == null ? null : sensorResult.Data.ThemeValues.ToArray();

                JsonData data = new JsonData
                                    {
                                        S = S,
                                        R = R,
                                        N = N,
                                        T = T,
                                        Q = Q,
                                        A = A,
                                        FI = FI,
                                        RV = RV,
                                        LV = LV,
                                        PV = PV,
                                        TV = TV
                                    };

                var json = JsonConvert.SerializeObject(data);
                return json;
            }
            catch (Exception e)
            {
                Log.Error("Sensor Acq Result Cast Error", e);
                return null;
            }
        }

        public void OnMessageReceived(string buff)
        {
            try
            {
                FsMessage msg = FsMessage.FromJson(buff);
                // real-time DAC: // body:{"dtu":1,"sensors":[17,20]}
                Log.InfoFormat("pull({0})", buff);
                this._msgHelper.DealMsg(msg, this);
            }
            catch (Exception ce)
            {
                Log.ErrorFormat("err {0}!", ce.Message);
            }
        }

        public void Push(FsMessage msg)
        {
            if (msg != null)
            {
                msg.Header.S = MyName;
                this.Push(msg.Header.D, msg);
            }
        }

        public string ResponseHandler(string msg)
        {
            Log.InfoFormat("Response({0})", msg);
            return this._msgHelper.DealMsg(msg, this); 
        }

        /// <summary>
        /// DTU状态变更告警
        /// </summary>
        /// <param name="msg"></param>
        private void DTUConnectionStatusChangedHandler(DTUConnectionStatusChangedMsg msg)
        {
            try
            {
                FsMessage fsmsg = this._warningHelper.GetDtuStatusMsg(msg);
                if (fsmsg != null)
                    this.Push(fsmsg);
                if (!msg.IsOnline)
                {
                    etDataStatus.ClearOffLineSensorStatus(msg.DTUID);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("DTU状态变更告警推送失败 ： {0}", ex.Message);
            }
        }

        void Instance_Exit()
        {
            if (this._dacServer != null)
            {
                try
                {
                    Log.InfoFormat("NGDAC 服务停止，更新所有在线DTU状态");
                    List<string> dtus = ((GprsDtuServer)this._dacServer).GetDtuOnLineList();
                    this._warningHelper.UpdateDtuStatus(dtus);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("update DTUs's status error ： {0}", ex.Message);
                }
            }
        }

        public AbnormalCount[] GetAbnormalSensorCount(uint[] structs)
        {
            return etDataStatus.GetAbnormalSensorCount(structs);
        }


        internal Senstatus[] GetSensorDacStatus(uint[] senIds, uint structureId = 0)
        {
            return etDataStatus.GetSensorDacStatus(senIds, structureId);
        }

        internal SensorDacStatus[] GetAllSensorDacStatus(uint structId)
        {
            return etDataStatus.GetAllSensorDacStatus(structId);
        }

        internal bool GetDtuStatus(string dtucode)
        {
            return this.Dtm.GetDtuStatus(dtucode);
        }
    }
}