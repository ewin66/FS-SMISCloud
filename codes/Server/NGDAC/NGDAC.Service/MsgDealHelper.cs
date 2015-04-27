#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="MsgDealHelper.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150306 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGDAC
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using FS.Service;
    using FS.SMIS_Cloud.NGDAC.Model;

    using log4net;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class MsgDealHelper
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void DealMsg(FsMessage msg,DacService service)
        {
            try
            {
                if (msg == null)
                {
                    this._log.Error("pull.OnMessageReceived, msg is NULL!");
                    return;
                }
                if (msg.Header.R == @"/et/dtu/instant/dac")
                {
                    this.InstantCol(msg, service);
                }
                if (msg.Header.R.StartsWith(@"/et/dtu/instant/at"))
                {
                    this.InstantAtCommand(msg, service);
                }
                if (msg.Header.R.StartsWith(@"/et/config"))
                {
                    this.ColInfoConfigChanged(msg, service);
                }
            }
            catch (Exception ce)
            {
                this._log.ErrorFormat("err {0}!", ce.Message);
            }
        }

        /// <summary>
        /// 即时采集
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="service"></param>
        private void InstantCol(FsMessage msg, DacService service)
        {
            string tid = msg.Header.U.ToString();
            uint dtu = msg.BodyValue<uint>("dtu");
            uint[] sensors = msg.BodyValues<uint>("sensors");
            var sensorList = new List<uint>(sensors);
            int taskId = service.Dtm.ArrangeInstantTask(tid, dtu, sensorList, null);
            msg.Body = JObject.FromObject(new { tid = taskId });
            this._log.DebugFormat("Resp.OnMessageReceived: {0}-{1}, dtu = {2}, sensors={3},resp={4}", msg.Header.S, msg.Header.R, dtu, sensors.Length, msg.Body);
            service.Push(msg.Header.S, msg);
        }

        /// <summary>
        /// AT指令
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="service"></param>
        private void InstantAtCommand(FsMessage msg, DacService service)
        {
            string tid = msg.Header.U.ToString();
            uint dtu = msg.BodyValue<uint>("dtuId");
            JObject[] cmds = msg.BodyValues<JObject>("cmds");
            int taskId = service.Dtm.ArrangeInstantTask(tid, dtu, cmds, null);
            msg.Body = JObject.FromObject(new { tid = taskId });
            service.Push(msg.Header.S, msg);
        }
        
        private void ColInfoConfigChanged(FsMessage msg, DacService service)
        {
            string r = msg.Header.R;
            if (r.Contains("dtu"))
            {
                this.DtuChangedHandler(msg,service);
            }
            if (r.Contains("sensor"))
            {
                this.SensorChangedHandler(msg,service);
            }
        }

        private void SensorChangedHandler(FsMessage msg, DacService service)
        {
            //string r = msg.Header.R;
            try
            {
                var senopera = ObjectDeserializeHelper.Json2SensorOperation(msg.Body.ToString());
                service.Dtm.UpdateSensorConfig(senopera);
            }
            catch (Exception ex)
            {
                this._log.ErrorFormat("Update sensor info error :{0}", ex.Message);
            }
        }

        private void DtuChangedHandler(FsMessage msg, DacService service)
        {
            string r = msg.Header.R;
            var dtu = JsonConvert.DeserializeObject<DtuNode>(msg.Body.ToString());
            if(dtu==null) return;
            if (r.EndsWith("add"))
            {
                this._log.InfoFormat("Add new DTU :{0}-{1}", dtu.DtuId, dtu.DtuCode);
                service.Dtm.DtuConfigChanged(ChangedStatus.Add, dtu);
            }
            else if (r.EndsWith("mod"))
            {
                this._log.InfoFormat("Update DTU :{0}-{1}", dtu.DtuId, dtu.DtuCode);
                string dtucode =
                    msg.Header.M.Split(new[] { ':', ',', '，', ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                service.Dtm.DtuConfigChanged(ChangedStatus.Modify, dtu, dtucode);
            }
            else
            {
                this._log.InfoFormat("Delete DTU :{0}-{1}", dtu.DtuId, dtu.DtuCode);
                service.Dtm.DtuConfigChanged(ChangedStatus.Delete, dtu);
            }
        }
        
        public string DealMsg(string buff, DacService service)
        {
            FsMessage msg = FsMessage.FromJson(buff);
            if (String.Compare(msg.Header.R, "/et/status/structs/abnormalsensorCount", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return this.GetAbnormalSensorCount(msg, service);
            }
            if (String.Compare(msg.Header.R, "/et/status/sensors",
                    StringComparison.OrdinalIgnoreCase) == 0)
            {
                return this.GetSensorStatus(msg, service);
            }
            if (String.Compare(msg.Header.R, "/et/status/struct/allSensors",
                StringComparison.OrdinalIgnoreCase) == 0)
            {
                return this.GetAllSensors(msg, service);
            }
            msg.Body = string.Empty;
            return msg.ToJson();
        }

        private string GetAbnormalSensorCount(FsMessage msg, DacService service)
        {
            try
            {
                StructureIds obj = JsonConvert.DeserializeObject<StructureIds>(msg.Body.ToString());
                if (obj == null || obj.structIds == null || obj.structIds.Length == 0)
                {
                    msg.Body =
                        JsonConvert.SerializeObject(new[]
                        {new {structId = 0, abnormalSensorCount = -1}});
                    return msg.ToJson();
                }
                AbnormalCount[] counts = service.GetAbnormalSensorCount(obj.structIds);
                string msgbody = JsonConvert.SerializeObject(counts);
                msg.Body = msgbody;
                return msg.ToJson();
            }
            catch (Exception ex)
            {
                this._log.ErrorFormat("GetAbnormalSensorCount error: {0}", ex.Message);
                return string.Empty;
            }
        }

        private string GetSensorStatus(FsMessage msg, DacService service)
        {
            SensorIds obj = JsonConvert.DeserializeObject<SensorIds>(msg.Body.ToString());
            if (obj == null || obj.sensorIds == null || obj.sensorIds.Length == 0)
            {
                msg.Body =
                    JsonConvert.SerializeObject(new[] { new { sensorId = 0, dacStatus = "N/A" } });
                return msg.ToJson();
            }
            Senstatus[] statuses = service.GetSensorDacStatus(obj.sensorIds, obj.structId);
            msg.Body = JsonConvert.SerializeObject(statuses);
            return msg.ToJson();
        }

        private string GetAllSensors(FsMessage msg, DacService service)
        {
            uint structId = msg.BodyValue<uint>("structId");
            SensorDacStatus[] allstatuses = service.GetAllSensorDacStatus(structId);
            msg.Body = JsonConvert.SerializeObject(allstatuses);
            return msg.ToJson();
        }
    }

    #region 告警Json对象

    public class StructureIds
    {
        public uint[] structIds { get; set; }
    }

    public class SensorIds
    {
        public uint structId { get; set; }
        public uint[] sensorIds { get; set; }
    }
    
    #endregion
}