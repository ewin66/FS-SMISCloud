using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using FS.DbHelper;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Task;
using FS.SMIS_Cloud.DAC.Util;
using log4net;

namespace FS.SMIS_Cloud.DAC.Storage.iSecureCloud
{
    public class MsDbAccessor
    {
        private static readonly ILog Log = LogManager.GetLogger("MsDbAccessor");
        private const string RawDataTable = "T_DATA_ORIGINAL";
        private readonly ISqlHelper _helper;
        private static string _errorPath = ConfigurationManager.AppSettings["StorageError"];
        private static readonly FileService FileService= new FileService();
        private readonly ConcurrentDictionary<string, TableInfo> _tables = new ConcurrentDictionary<string, TableInfo>();
        
        public MsDbAccessor(string connStr)
        {
            _helper = SqlHelperFactory.Create(DbType.MSSQL, connStr);
            if (string.IsNullOrEmpty(_errorPath))
            {
                _errorPath = @"C:\StorageError\DataError.log";
            }
        }

        public void UpdateTables(IList<TableInfo> tables)
        {
            foreach (var table in tables)
                _tables.AddOrUpdate(table.TableName, table, (k, v) => table);
        }

        public int SaveDacResult(DACTaskResult result)
        {
            IList<SensorAcqResult> sresults = result.SensorResults;
            if (sresults == null || sresults.Count <= 0)
                return 0;
            var sqlCmds = new List<SqlCmdInfo>();
            foreach (SensorAcqResult sr in sresults)
            {
                if (!sr.IsOK || sr.Data == null)
                    continue;
                ISensorData data = sr.Data;
                try
                {
                    if (data == null) continue;
                    
                        // 1. 存储到 T_DATA_ORIGINAL, Value 1/2/3/4 对应了主题中的属性值. 
                        // 2. 存储到对应的主题数据.
                        // 入原始数据
                    if (sr.ErrorCode == (int)Errors.SUCCESS)
                    {
                        if (data.IsSaveDataOriginal)
                        {
                            ((BasicSensorData)data).IsSaveDataOriginal = false;
                            string rawValueSql = GenerateAddValueSql(RawDataTable, sr, GetStorageTime(result,sr));
                            if (!string.IsNullOrEmpty(rawValueSql))
                                sqlCmds.Add(new SqlCmdInfo(rawValueSql));
                        }
                        else
                        {
                            // 入主题数据表
                            if (data.ThemeValues.Any(d => d != null))
                            {
                                string themeSql = GenerateAddValueSql(sr.Sensor.FactorTypeTable, sr, GetStorageTime(result, sr));
                                if (!string.IsNullOrEmpty(themeSql))
                                    sqlCmds.Add(new SqlCmdInfo(themeSql));
                            }
                        }
                    }
                    else
                    {
                        FileService.Write(_errorPath,
                            string.Format("Dtu[{3}-{4}] s:{5} - m:{0} - c:{1}, Error Code: {2}", sr.Sensor.ModuleNo,
                                sr.Sensor.ChannelNo, sr.ErrorCode, sr.Sensor.DtuID, sr.DtuCode,sr.Sensor.SensorID));
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Dtu:{0}-{1} s:{2} - m{3} - c{4}, create SQL error:{5}", sr.Sensor.DtuID, sr.DtuCode,
                        sr.Sensor.SensorID, sr.Sensor.ModuleNo,
                        sr.Sensor.ChannelNo, ex.Message);
                }
            }
            return _helper.ExecuteSqlTran(sqlCmds);
        }

        DateTime GetStorageTime(DACTaskResult tr, SensorAcqResult ar)
        {
            switch (tr.StoragedTimeType)
            {
                case SensorAcqResultTimeType.TaskFinishedTime:
                    return tr.Finished;
                case SensorAcqResultTimeType.TaskStartTime:
                    return tr.Started;
                case SensorAcqResultTimeType.SensorRequestTime:
                    return ar.RequestTime;
                case SensorAcqResultTimeType.SensorResponseTime:
                    return ar.ResponseTime;
                default:
                    return tr.Finished;
            }
        }


        /// <summary>
        /// 数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="data"></param>
        /// <param name="acqtime"></param>
        /// <returns></returns>
        public string GenerateAddValueSql(string tableName, SensorAcqResult senacqreslt,DateTime acqtime)
        {
            ISensorData data =senacqreslt.Data;
            double?[] values = tableName == RawDataTable ? Double2NullableDouble(data.RawValues) :  data.ThemeValues.ToArray();
            if (values == null || values.Length == 0)
                return null;
            TableInfo table = _tables.ContainsKey(tableName)
                ? _tables[tableName] : default(TableInfo);
            if (table == null)
            {
                string error = string.Format(" miss table {0} ;", tableName);
                try
                {
                    LogErrorData(senacqreslt);
                }
                catch (Exception ex)
                {
                    error += ex.Message;
                }
                throw new Exception(error);
            }
            string columnStr = _tables[tableName].Colums;
            var valueStr = new StringBuilder();
            string tableCommonColumns = GetTableCommonColumns(tableName, senacqreslt.Sensor, acqtime);
            valueStr.Append(tableCommonColumns);
            int tableCommonColumnscount =
                tableCommonColumns.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Length;
            int columscount = columnStr.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Length;

            if (values.Length + tableCommonColumnscount > columscount)
            {
                string error = string.Format("values's count is out of {0}'s columns's count ",
                    table.TableName);
                try
                {
                    LogErrorData(senacqreslt, tableName);
                }
                catch (Exception ex)
                {
                    error += ex.Message;
                }
                throw new Exception(error);
            }

            for (int i = tableCommonColumnscount; i < columscount; i++)
            {
                int index = i - tableCommonColumnscount;
                if (index >= values.Length || values[index] == null)
                    valueStr.AppendFormat(",{0}", "null");
                else
                    valueStr.AppendFormat(",{0:0.######}", values[index].Value);
            }
            string sql = string.Format(@"insert into {0} ({1}) values ({2})", table.TableName, table.Colums,
                valueStr);
            return sql;
        }

        private string GetTableCommonColumns(string table, Sensor sensor,DateTime acqtime)
        {
            // 由于网站展示数据时，需要时间统一，故入库时统一修改时间data.AcqTime 为入库时间
            string valuecommoncolumns = table == RawDataTable
                ? string.Format("{0},'{1:yyyy-MM-dd HH:mm:ss.fff}'", sensor.SensorID, acqtime)
                : string.Format("{0},{1},'{2:yyyy-MM-dd HH:mm:ss.fff}'", sensor.SensorID, sensor.FactorType,
                    acqtime);
            return valuecommoncolumns;
        }
        
        /// <summary>
        /// 把double[]转成double?[]
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public double?[] Double2NullableDouble(double[] values)
        {
            if(values==null||values.Length==0)
                return null;
            var nulablevalues = new double?[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                nulablevalues[i] = values[i];
            }
            return nulablevalues;
        }

        public bool LogErrorData(SensorAcqResult senacqreslt,string tablename="")
        {
            ISensorData data = senacqreslt.Data;
            var errdata = new StringBuilder();
            errdata.AppendFormat("{0},{1}", senacqreslt.Sensor.SensorID, senacqreslt.ResponseTime);
            foreach (var d in data.RawValues)
            {
                errdata.AppendFormat(",{0:0.######}", d);
            }
            if (!string.IsNullOrEmpty(tablename))
            {
                errdata.AppendFormat(",{0}", tablename);
            }
            FileService.Write(_errorPath, errdata.ToString());
            errdata.Clear();
            errdata.AppendFormat("{0},{1},{2}", senacqreslt.Sensor.SensorID, senacqreslt.ResponseTime, senacqreslt.Sensor.FactorType);
            foreach (var d in data.ThemeValues)
            {
                errdata.AppendFormat(",{0:0.######}", d);
            }
            if (!string.IsNullOrEmpty(tablename))
            {
                errdata.AppendFormat(",{0}", tablename);
            }
            FileService.Write(_errorPath, errdata.ToString());
            return true;
        }
    }
}
