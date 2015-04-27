namespace FS.SMIS_Cloud.NGET.DataRationalFilter
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;

    using FS.DbHelper;
    using FS.SMIS_Cloud.NGET.Consumer;
    using FS.SMIS_Cloud.NGET.Model;

    using log4net;

    using DbType = FS.DbHelper.DbType;

    public class DataRationalFilter : IDacTaskResultConsumer
    {
        private static readonly ILog Log = LogManager.GetLogger("DataRationalFilter");

        private static string cs = ConfigurationManager.AppSettings["SecureCloud"];
        private static ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(List<SensorAcqResult> source)
        {
            Log.Info("DataRationalFilter has recieved DACTaskResult, starts to filter..");
            foreach (var sensorResult in source)
            {

                if (sensorResult.IsOK && sensorResult.Data != null && sensorResult.ErrorCode == 0)
                {
                    if (this.SensorTypeFilter == null) return;
                    if (this.SensorTypeFilter.Contains(sensorResult.Sensor.SensorType))
                    {
                        var ranges = this.GetRationalRangeBySensorId(sensorResult.Sensor.SensorID);
                        this.JudgeDataOverRange(sensorResult, ranges);
                    }
                }
            }
        }

        private IList<RationalRange> GetRationalRangeBySensorId(uint sensorId)
        {
            string sql = string.Format(@"
select ItemId,Enabled,RationalLower as Lower,RationalUpper as Upper
from dbo.T_DATA_RATIONAL_FILTER_CONFIG
where SensorId = {0}", sensorId);
            try
            {
                DataTable dt = sqlHelper.Query(sql).Tables[0];

                if (dt.Rows.Count == 0)
                {
                    return null;
                }

                var list = new List<RationalRange>();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Lower"] == DBNull.Value || row["Upper"] == DBNull.Value)
                    {
                        if (Convert.ToBoolean(row["Enabled"]))
                        {
                            Log.WarnFormat(
                                "sensor:{0}-itemId:{1} rational filter enabled, but lower is {2} and upper is {3}",
                                sensorId,
                                row["ItemId"],
                                row["Lower"],
                                row["Upper"]);
                        }
                        continue;
                    }
                    var r = new RationalRange
                    {
                        ItemId = Convert.ToInt32(row["ItemId"]),
                        Enabled = Convert.ToBoolean(row["Enabled"]),
                        Lower = Convert.ToDouble(row["Lower"]),
                        Upper = Convert.ToDouble(row["Upper"])
                    };

                    list.Add(r);
                }
                return list;
            }
            catch (Exception e)
            {
                Log.Error("query rational ranges error:\n" + sql, e);
                return null;
            }
        }

        private void JudgeDataOverRange(SensorAcqResult sensorResult, IList<RationalRange> ranges)
        {
            if (ranges == null)
            {
                return;
            }

            try
            {
                var themeValues = sensorResult.Data.ThemeValues;
                for (int i = 0; i < sensorResult.Data.ThemeValues.Count; i ++)
                {
                    var range = ranges.FirstOrDefault(r => r.ItemId == i + 1);

                    if (range != null && range.Enabled && themeValues[i] != null)
                    {
                        if (themeValues[i].Value < range.Lower || themeValues[i].Value > range.Upper)
                        {
                            Log.InfoFormat(
                                "sensor:{0}-item:{3} over RATIONAL VALUE range[{1},{2}]",
                                sensorResult.Sensor.SensorID,
                                range.Lower,
                                range.Upper,
                                range.ItemId);
                            sensorResult.Data.ThemeValues[i] = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("judge over rational range error", e);
            }
        }
    }
}
