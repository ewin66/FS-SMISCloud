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

using System;
using System.Collections.Generic;
using System.Configuration;
using FS.Service;
using FS.SMIS_Cloud.DAC.Accessor;
using FS.SMIS_Cloud.DAC.File;
using FS.SMIS_Cloud.DAC.Gprs;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Task;
using FS.SMIS_Cloud.Services.ConsoleCtrlManager;
using log4net;

namespace FS.SMIS_Cloud.ET
{
    using DAC.Accessor.MSSQL;
    using DAC.Consumer;

    public class EtService :   Service.Service
    {
        private const string MyName = "et";
        private static readonly ILog Log = LogManager.GetLogger("ET");
        internal DACTaskManager Dtm;
        private readonly IDtuServer _dacServer; // _DacReceiver;
        private readonly IDtuServer _fileServer; // _FileReceiver
        private readonly WarningHelper _warningHelper;
        private static EtDataStatusConsumer etDataStatus;
        private readonly MsgDealHelper _msgHelper = new MsgDealHelper();
        public EtService(string svrConfig, string busPath)
            : base(MyName, svrConfig, busPath)
        {
            string cs = ConfigurationManager.AppSettings["SecureCloud"];
            int dacServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["Port"], 10); 
            Log.InfoFormat("DBConnecting: {0}", cs);
            try
            {
                DbAccessorHelper.Init(new MsDbAccessor(cs));// 配置
                DACTaskResultConsumerService.Init();// 消费
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("系统初始化失败:{0}", ex.Message);
                return;
            }

            etDataStatus = new EtDataStatusConsumer(this);
            // 自定义消费者
            if (DACTaskResultConsumerService.Queues.Count == 0)
            {
                var queue = new DACTaskResultConsumerQueue(ConsumeType.Async);
                queue.Enqueue(etDataStatus);
                DACTaskResultConsumerService.AddComsumerQueue(queue);
            }
            else
            {
                DACTaskResultConsumerService.InsertComsumer(0, 2, etDataStatus);
                //DACTaskResultConsumerService.Queues[0].Insert(etDataStatus, 1);
            }

            _warningHelper=new WarningHelper();
            _dacServer = new GprsDtuServer(dacServerPort);
            _fileServer = new FileDtuServer();
            Dtm = new DACTaskManager(_dacServer, DbAccessorHelper.DbAccessor.QueryDtuNodes(), null); // DbAccessorHelper.DbAccessor.GetUnfinishedTasks()
            Dtm.SetFileServer(_fileServer);
            // Push/Pull mode.
            Pull(null, OnMessageReceived);
            Response(ResponseHandler);
            Subscribe("cm", "/cm/datachanged/dtu/sensor/", OnSubscribed);
            ConsoleCtrlManager.Instance.RegCmd("reload", LoadHandler); // 注册重新加载传感器信息
            ConsoleCtrlManager.Instance.Exit += Instance_Exit;
            _warningHelper.UpdateAllDtuStatus();
        }

        private string LoadHandler(string[] args)
        {
            return this.ReloadConfigInfo();
        }

        public string ReloadConfigInfo()
        {
            if (Dtm.UpdateSensorConfigInfo())
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
            _dacServer.Start();
            _fileServer.Start();
            DACTaskManager.OnDTUConnectionStatusChanged += DTUConnectionStatusChangedHandler;
            Dtm.OnSensorCollectMsgHandler += dtm_OnSensorCollectMsgHandler;
            Dtm.ArrangeTimedTask();
        }

        private void dtm_OnSensorCollectMsgHandler(CollectState state, DAC.DAC.SensorAcqResult acqResult)
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

        public void OnMessageReceived(string buff)
        {
            try
            {
                FsMessage msg = FsMessage.FromJson(buff);
                // real-time DAC: // body:{"dtu":1,"sensors":[17,20]}
                Log.InfoFormat("pull({0})", buff);
                _msgHelper.DealMsg(msg, this);
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
                Log.DebugFormat("push msg {0}",msg.ToJson());
                try
                {
                    Push(msg.Header.D, msg);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("push msg failed : msg {0}, error: {1}", msg.ToJson(), ex);
                }
            }
        }

        public string ResponseHandler(string msg)
        {
            Log.InfoFormat("Response({0})", msg);
            return _msgHelper.DealMsg(msg, this); 
        }

        /// <summary>
        /// DTU状态变更告警
        /// </summary>
        /// <param name="msg"></param>
        private void DTUConnectionStatusChangedHandler(DTUConnectionStatusChangedMsg msg)
        {
            try
            {
                FsMessage fsmsg = _warningHelper.GetDtuStatusMsg(msg);
                if (fsmsg != null)
                    Push(fsmsg);
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
            if (_dacServer != null)
            {
                try
                {
                    Log.InfoFormat("ET 服务停止，更新所有在线DTU状态");
                    List<string> dtus = ((GprsDtuServer)_dacServer).GetDtuOnLineList();
                    _warningHelper.UpdateDtuStatus(dtus);
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