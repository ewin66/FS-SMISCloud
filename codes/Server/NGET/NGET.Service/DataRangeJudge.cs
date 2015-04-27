namespace FS.SMIS_Cloud.NGET
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Text;

    using FS.DbHelper;
    using FS.Service;
    using FS.SMIS_Cloud.NGET.Consumer;
    using FS.SMIS_Cloud.NGET.Model;
    using FS.SMIS_Cloud.Services.Messages;

    using log4net;

    using Newtonsoft.Json;

    using DbType = FS.DbHelper.DbType;

    public class DataRangeJudge : IDacTaskResultConsumer
    {
        private Service service;
        private static readonly ILog Log = LogManager.GetLogger("DataRangeJudge");
        static string _warningAppName = "WarningManagementProcess";

        public DataRangeJudge(Service service)
        {
            this.service = service;
        }

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(List<SensorAcqResult> rslt)
        {
            Log.Info("DataRangeJudge  start ...");
            foreach (SensorAcqResult sensorAcqResult in rslt)
            {
                if (sensorAcqResult.IsOK && sensorAcqResult.Data.CollectPhyValues != null)
                {
                    Log.DebugFormat("GetRangeByProductId start..");
                    var ranges = this.GetRangeByProductId(sensorAcqResult.Sensor.ProductId);
                    Log.DebugFormat("GetRangeByProductId end .");
                    Log.DebugFormat("JudgeDataOverRange start..");
                    var sb = this.JudgeDataOverRange(sensorAcqResult, ranges);
                    Log.DebugFormat("JudgeDataOverRange end.");
                    if (!string.IsNullOrEmpty(sb))
                    {
                        Log.InfoFormat(
                            "Sensor:{0} generate a OVER RANGE alarm:{1}, sending..",
                            sensorAcqResult.Sensor.SensorID,
                            sb);
                        this.SendDataOverRangeWarning(sensorAcqResult, sb);
                    }
                }
            }
            Log.Info("DataRangeJudge  end .");
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
    }

    #region 告警Json对象

    public class Range
    {
        public string Name { get; set; }

        public double? Upper { get; set; }

        public double? Lower { get; set; }
    }

    #endregion
}


