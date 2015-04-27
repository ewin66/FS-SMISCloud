namespace FS.SMIS_Cloud.NGDAC
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Task;
    using FS.SMIS_Cloud.NGDAC.Util;

    using log4net;

    public class DataStatusConsumer
    {
        private DacService service;
        private static readonly ILog Log = LogManager.GetLogger("DacDataStatusConsumer");
        static string _warningAppName = "WarningManagementProcess";
        // 结构物传感器状态
        private static ConcurrentDictionary<uint, ConcurrentDictionary<uint, SensorStatus>> _senststusdic = new ConcurrentDictionary<uint, ConcurrentDictionary<uint, SensorStatus>>();

        public DataStatusConsumer(DacService service)
        {
            this.service = service;
        }

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
            }
            Log.Info("EtDataStatusConsumer  end .");
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


