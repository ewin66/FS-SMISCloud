using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Util;
using FS.SMIS_Cloud.Services.Messages;

namespace FS.SMIS_Cloud.ET
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Text;
    using DbHelper;
    using Service;
    using DAC.Consumer;
    using DAC.DAC;
    using DAC.Model;
    using DAC.Task;

    using log4net;

    using Newtonsoft.Json;

    using DbType = FS.DbHelper.DbType;

    public class EtDataStatusConsumer : IDACTaskResultConsumer
    {
        private EtService service;
        private static readonly ILog Log = LogManager.GetLogger("EtDataStatusConsumer");
        /*以后这个Consumer可能作为独立模块,所以把所有内容都内聚在这个cs文件中*/
        private WarningHelper warningHelper = new WarningHelper();
        static string _warningAppName = "WarningManagementProcess";
        // 结构物传感器状态
        private static ConcurrentDictionary<uint, ConcurrentDictionary<uint, SensorStatus>> _senststusdic = new ConcurrentDictionary<uint, ConcurrentDictionary<uint, SensorStatus>>();

        public EtDataStatusConsumer(EtService service)
        {
            this.service = service;
        }

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(DACTaskResult rslt)
        {
            bool isOnline = false;
            if (this.service != null)
                isOnline = this.service.GetDtuStatus(rslt.DtuCode);
            Log.Info("EtDataStatusConsumer  start ...");
            foreach (SensorAcqResult sensorAcqResult in rslt.SensorResults)
            {
                Log.Debug("AddOrUpdateSensorStatus start ...");
                if (isOnline)
                    this.AddOrUpdateSensorStatus(sensorAcqResult);
                Log.Debug("AddOrUpdateSensorStatus end.");
                Log.DebugFormat("JudgeDataStatusIsOk start..");
                if (this.JudgeDataStatusIsOk(sensorAcqResult))
                {
                    Log.DebugFormat("JudgeDataStatusIsOk end.");
                    Log.DebugFormat("GetRangeByProductId start..");
                    var ranges = this.GetRangeByProductId(sensorAcqResult.Sensor.ProductId);
                    Log.DebugFormat("GetRangeByProductId end .");
                    Log.DebugFormat("JudgeDataOverRange start..");
                    var sb = this.JudgeDataOverRange(sensorAcqResult, ranges);
                    Log.DebugFormat("JudgeDataOverRange end.");
                    if (!string.IsNullOrEmpty(sb))
                    {
                        Log.InfoFormat("Sensor:{0} generate a OVER RANGE alarm:{1}, sending..", sensorAcqResult.Sensor.SensorID, sb);
                        this.SendDataOverRangeWarning(sensorAcqResult, sb);
                    }
                }
            }
            Log.Info("EtDataStatusConsumer  end .");
        }

        public string JudgeDataOverRange(SensorAcqResult sensorAcqResult, Range[] range)
        {
            if (range == null)
            {
                return string.Empty;
            }

            var phyValues = sensorAcqResult.Data.CollectPhyValues;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < phyValues.Length; i++)
            {
                try
                {
                    if (i >= range.Length)
                    {
                        break;
                    }

                    if ((range[i].Upper != null && phyValues[i] > range[i].Upper)
                        || (range[i].Lower != null && phyValues[i] < range[i].Lower))
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append(',');
                        }
                        sb.AppendFormat(
                            "{0}采集值:[{1}]超出量程[{2}~{3}]",
                            range[i].Name,
                            phyValues[i],
                            range[i].Lower,
                            range[i].Upper);
                        sensorAcqResult.Data.DropThemeValue(i);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("量程判断失败", e);
                }
            }
            return sb.ToString();
        }

        private void SendDataOverRangeWarning(SensorAcqResult sensorAcqResult, string sb)
        {
            try
            {
                var msgStatus = this.GetOverRangeWarningMessage(sensorAcqResult.Sensor, sb);
                if (msgStatus != null)
                {
                    this.service.Push(msgStatus.Header.D, msgStatus);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("传感器超量程告警推送失败 ： {0}", ex.Message);
            }
        }

        private bool JudgeDataStatusIsOk(SensorAcqResult sensorAcqResult)
        {
            if (!(sensorAcqResult.IsOK && sensorAcqResult.Data != null && sensorAcqResult.ErrorCode == 0))
            {
                try
                {
                    var msgStatus = this.warningHelper.GetSensorMsg(sensorAcqResult);
                    if (msgStatus != null)
                        this.service.Push(msgStatus.Header.D, msgStatus);
                    var msgData = this.warningHelper.DataStatusMsg(sensorAcqResult);
                    if (msgData != null)
                        this.service.Push(msgData.Header.D, msgData);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("传感器采集超时告警推送失败 ： {0}", ex.Message);
                }
                return false;
            }

            return true;
        }

        public Range[] GetRangeByProductId(int productId)
        {
            string sql = string.Format(@"
SELECT [PRODUCT_UPPER_LIMIT1] as u1
      ,[PRODUCT_LOWER_LIMIT1] as l1
      ,[PRODUCT_UPPER_LIMIT2] as u2
      ,[PRODUCT_LOWER_LIMIT2] as l2
      ,[PRODUCT_UPPER_LIMIT3] as u3
      ,[PRODUCT_LOWER_LIMIT3] as l3
      ,[PRODUCT_UPPER_LIMIT4] as u4
      ,[PRODUCT_LOWER_LIMIT4] as l4
      ,[LIMIT_MAP] as map
  FROM [T_DIM_SENSOR_PRODUCT]
  where PRODUCT_ID={0}", productId);
            try
            {
                string cs = ConfigurationManager.AppSettings["SecureCloud"];
                ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
                DataTable dt = sqlHelper.Query(sql).Tables[0];

                if (dt.Rows.Count == 0)
                {
                    return null;
                }

                Range[] ranges = new Range[4];
                var row = dt.Rows[0];
                var map = row[8].ToString().Split(',');
                for (int i = 0; i < 4; i++)
                {
                    ranges[i] = new Range
                                {
                                    Name = i < map.Length ? map[i] : string.Empty,
                                    Upper = DBNull.Value == row[2*i] ? null : (double?)Convert.ToDouble(row[2*i]),
                                    Lower = DBNull.Value == row[2*i+1] ? null : (double?)Convert.ToDouble(row[2*i+1])
                                };
                }

                return ranges;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /*以后这个Consumer可能作为独立模块,所以把所有内容都内聚在这个cs文件中*/
        public FsMessage GetOverRangeWarningMessage(Sensor sensor, string warningContent)
        {
            var msg = new RequestWarningReceivedMessage
            {
                Id = Guid.NewGuid(),
                StructId = (int)sensor.StructId,
                DeviceTypeId = 2,
                DeviceId = (int)sensor.SensorID,
                WarningTypeId = ((int)Errors.ERR_OUT_RANGE).ToString(),
                WarningTime = DateTime.Now,
                DateTime = DateTime.Now,
                WarningContent = string.Format("传感器:{0}-产品型号:{1}:{2}", sensor.Name, sensor.ProductCode, warningContent)
            };
            var warningmsg = new FsMessage
            {
                Header = new FsMessageHeader
                {
                    A = "PUT",
                    R = "/warning/sensor",
                    U = Guid.NewGuid(),
                    T = Guid.NewGuid(),
                    D = _warningAppName,
                    M = "Warning"
                },
                Body = JsonConvert.SerializeObject(msg)
            };
            return warningmsg;
        }
        
        public void AddOrUpdateSensorStatus(SensorAcqResult result)
        {
            try
            {
                if (!_senststusdic.ContainsKey(result.Sensor.StructId))
                    _senststusdic.TryAdd(result.Sensor.StructId, new ConcurrentDictionary<uint, SensorStatus>());
                var senStatus = new SensorStatus
                {
                    StructureId = result.Sensor.StructId,
                    SensorId = result.Sensor.SensorID,
                    DtuCode = result.Sensor.DtuCode,
                    Location = result.Sensor.Name,
                    DacErrcode = result.ErrorCode,
                    ErrMsg = EnumHelper.GetDescription((Errors)result.ErrorCode),
                    AcqTime = result.ResponseTime
                };
                _senststusdic[senStatus.StructureId].AddOrUpdate(senStatus.SensorId, senStatus, (k, v) => senStatus);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("AddOrUpdateSensorStatus error : {0} ", ex.Message);
            }
        }

        public int GetAbnormalSensorCount(uint structId)
        {
            int count = -1;
            if (_senststusdic.ContainsKey(structId) && _senststusdic[structId].Values.Count > 0)
                count = _senststusdic[structId].Values.Count(s => !s.IsOK);
            return count;
        }

        public AbnormalCount[] GetAbnormalSensorCount(uint[] structId)
        {
            AbnormalCount[] abnc = null;
            if (structId != null)
            {
                abnc = new AbnormalCount[structId.Length];
                for (int i = 0; i < structId.Length; i++)
                {
                    abnc[i] = new AbnormalCount
                    {
                        structId = structId[i],
                        abnormalSensorCount = this.GetAbnormalSensorCount(structId[i])
                    };
                }
            }
            return abnc;
        }

        public Senstatus[] GetSensorDacStatus(uint[] sensors, uint structureId)
        {
            if (_senststusdic.ContainsKey(structureId)&&sensors.Length>0)
            {
                Senstatus[] senstatus = new Senstatus[sensors.Length];
                for (int i = 0; i < sensors.Length; i++)
                {
                    senstatus[i]=new Senstatus
                    {
                        sensorId = sensors[i],
                        dacStatus = "N/A"
                    };
                    if (_senststusdic[structureId].ContainsKey(sensors[i]))
                    {
                        senstatus[i].dacStatus = _senststusdic[structureId][sensors[i]].ErrMsg;
                    }
                }
                return senstatus;
            }
            return new[]
            {
                new Senstatus
                {
                    sensorId = 0,
                    dacStatus = "N/A"
                }
            };
        }

        public void ClearOffLineSensorStatus(string dtucode)
        {
            try
            {
                foreach (var sd in _senststusdic)
                {
                    List<uint> sens = (from status in sd.Value.Values where status.DtuCode == dtucode select status.SensorId).ToList();
                    if(sens==null ||sens.Count==0)
                        return;
                    foreach (uint sen in sens)
                    {
                        SensorStatus sstes = null;
                        _senststusdic[sd.Key].TryRemove(sen, out sstes);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Clear offline sensors' status error : [dtu:{0}] ,errormessage :{1}", dtucode,
                    ex.Message);
            }
        }
        
        internal SensorDacStatus[] GetAllSensorDacStatus(uint structId)
        {
            if (_senststusdic.ContainsKey(structId))
            {
                var sens = _senststusdic[structId].Values.GroupBy(s => s.DacErrcode);
                List<SensorDacStatus> sendacsts = sens.Select(s => new SensorDacStatus()
                {
                    dacErrorCode = s.Key, 
                    status = EnumHelper.GetDescription((Errors) s.Key), 
                    sensors = s.Select(info => new sensorInfo
                    {
                        sensorId = info.SensorId, location = info.Location, time = info.AcqTime
                    }).ToArray()
                }).ToList();

                return sendacsts.ToArray();
            }
            return new SensorDacStatus[] { };
        }
    }

    #region 告警Json对象

    public class Range
    {
        public string Name { get; set; }

        public double? Upper { get; set; }

        public double? Lower { get; set; }
    }

    public class AbnormalCount
    {
        public uint structId { get; set; }

        public int abnormalSensorCount { get; set; }
    }
    
    public class Senstatus
    {
        public uint sensorId { get; set; }

        public string dacStatus { get; set; }
    }

    public class SensorDacStatus
    {
        public int dacErrorCode { get; set; }
        public string status { get; set; }
        public int count { get { return this.sensors.Length; } }
        public sensorInfo[] sensors { get; set; }
    }

    public class sensorInfo
    {
        public uint sensorId { get; set; }
        public string location { get; set; }
        public DateTime time { get; set; }
    }

    #endregion
}


