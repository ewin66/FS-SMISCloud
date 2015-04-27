using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using FS.DbHelper;
using FS.SMIS_Cloud.DAC.Consumer;
using FS.SMIS_Cloud.DAC.DataReplaceFilter.Model;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Task;
using System.Data;
using log4net;

namespace FS.SMIS_Cloud.DAC.DataReplaceFilter
{
    public class DataReplaceFilter : IDACTaskResultConsumer
    {
        private static readonly ILog Log = LogManager.GetLogger("DataReplaceFilter");
        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(DACTaskResult source)
        {
            Log.Info("DataReplaceFilter has recieved DACTaskResult, starts to replace..");
            if (source == null || source.SensorResults == null)
            {
                return;
            }
            Dictionary<uint, AbnormalConfigInfo> configInfos=GetAbnormalConfigInfo(source);
            foreach (var sensorResult in source.SensorResults)
            {
                if (sensorResult.IsOK && sensorResult.Data != null)
                {
                    var sensor = sensorResult.Sensor;
                    var sensorId = sensor.SensorID;
                    if (configInfos.ContainsKey(sensorId) && Convert.ToBoolean(configInfos[sensorId].IsEnabled))
                    {
                        if (sensorResult.Data.ThemeValues != null && sensorResult.Data.ThemeValues.Count != 0)
                        {
                            for (var i = 0; i < sensorResult.Data.ThemeValues.Count; i++)
                            {
                                //TODO
                                if (configInfos[sensorId].ReplaceValues[i] != null)
                                {
                                    sensorResult.Data.ThemeValues[i] =
                                        Convert.ToDouble(configInfos[sensorId].ReplaceValues[i]);
                                }
                            }
                        }
                    }
                }
            }
           
        }

        public Dictionary<uint, AbnormalConfigInfo>  GetAbnormalConfigInfo(DACTaskResult source)
        {
            var configInfos = new Dictionary<uint, AbnormalConfigInfo>();
            if (source != null && source.SensorResults != null)
            {
                List<uint> sensorIdList =
                    source.SensorResults.Select(m => m.Sensor.SensorID).ToList();
                if (sensorIdList.Count > 0)
                {
                    try
                    {
                        ISqlHelper sqlHelper =
                            SqlHelperFactory.Create(FS.DbHelper.DbType.MSSQL, ConfigurationManager.AppSettings["SecureCloud"]);
                        string sql = string.Format(@"
                      select [SensorId]          as sensorId,
                                [ReplaceValue1] as replaceValue1,
                                [ReplaceValue2] as replaceValue2,
                                [ReplaceValue3] as replaceValue3,
                                [ReplaceValue4] as replaceValue4,
                                [IsEnabled]         as isEnabled
                     from [dbo].[T_DIM_ABNORMALSENSOR_CONFIG]
                     where SensorId in ({0})
                   ", string.Join(",", sensorIdList));
                        DataSet ds = sqlHelper.Query(sql);
                        if (ds.Tables.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            string colum = "replaceValue";
                            foreach (DataRow item in dt.Rows)
                            {
                                if (!configInfos.ContainsKey(Convert.ToUInt32(item[0])))
                                {
                                    uint senId = Convert.ToUInt32(item[0]);
                                    configInfos.Add(senId, new AbnormalConfigInfo()
                                    {
                                        SensorId = senId,
                                        IsEnabled = Convert.ToBoolean(item[5])
                                    });
                                    for (int i = 0; i < dt.Columns.Count-2; i++)
                                    {
                                        string str = string.Format("{0}{1}", colum, i + 1);
                     
                                       configInfos[senId].ReplaceValues.Add(item.IsNull(str) ? null : (decimal?)Convert.ToDecimal(item[str]));
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return configInfos;
        }
    }
}
