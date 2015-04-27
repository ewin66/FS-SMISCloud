using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using FS.DbHelper;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Task;
using FS.SMIS_Cloud.DAC.Util;
using log4net;

namespace FS.SMIS_Cloud.DAC.Storage.SQLite
{
    public class SQLiteDbAccessor
    {
        private static readonly ILog Log = LogManager.GetLogger("SQLiteDbAccessor");
        private readonly ISqlHelper _helper;
        private static string _errorPath = ConfigurationManager.AppSettings["StorageError"];
        private static readonly FileService FileService = new FileService();
        private readonly ConcurrentDictionary<uint, TableInfo> _tables = new ConcurrentDictionary<uint, TableInfo>();

        public SQLiteDbAccessor(string connStr)
        {
            _helper = SqlHelperFactory.Create(DbType.SQLite, connStr);
            if (string.IsNullOrEmpty(_errorPath))
            {
                _errorPath = @"C:\StorageError\DataError.log";
            }
        }

        public void UpdateTables(IDictionary<uint, TableInfo> tables)
        {
            foreach (var table in tables)
            {
                _tables.AddOrUpdate(table.Key, table.Value, (k, v) => table.Value);
            }
        }

        public int SaveDacResult(DACTaskResult result)
        {
            IList<SensorAcqResult> sresults = result.SensorResults;
            if (sresults == null || sresults.Count <= 0)
                return 0;
            var sqlCmds = new List<SqlCmdInfo>();
            foreach (SensorAcqResult sr in sresults)
            {

                ISensorData data = sr.Data;
                try
                {
                    // 2. 存储到对应的主题数据.
                    // 入原始数据
                    // 入主题数据表
                    if (data == null) continue;
                    string themeSql = GenerateAddValueSql(sr);
                    if (!string.IsNullOrEmpty(themeSql))
                        sqlCmds.Add(new SqlCmdInfo(themeSql));
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("{0},产生SQL语句错误:" + ex.Message, sr.Sensor.SensorID);
                }
            }
            return _helper.ExecuteSqlTran(sqlCmds);
        }

        /// <summary>
        /// 数据
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GenerateAddValueSql(SensorAcqResult sr)
        {
            ISensorData data = sr.Data;
            double?[] values = MergeRawvaluesAndThemeValues(data.RawValues,data.ThemeValues.ToArray());
            if (values == null || values.Length == 0)
                return null;
            TableInfo table = _tables.ContainsKey(sr.Sensor.ProtocolType)
                ? _tables[sr.Sensor.ProtocolType] : default(TableInfo);
            if (table == null)
            {
                // TODO 记录数据
                string error = string.Format(" miss protocol {0} ;", sr.Sensor.ProtocolType);
                try
                {
                    LogErrorData(sr,values);
                }
                catch (Exception ex)
                {
                    error += ex.Message;
                }
                throw new Exception(error);
            }
            string columnStr = _tables[sr.Sensor.ProtocolType].Colums;
            var valueStr = new StringBuilder();
            string tableCommonColumns = GetTableCommonColumns(sr);
            valueStr.Append(tableCommonColumns);
            int tableCommonColumnscount =
                tableCommonColumns.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Length;
            int columscount = columnStr.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Length;
            try
            {
                for (int i = tableCommonColumnscount; i < columscount; i++)
                {
                    int index = i - tableCommonColumnscount;
                    if (index >= values.Length || values[index] == null)
                        valueStr.AppendFormat(",{0}", "null");
                    else
                        valueStr.AppendFormat(",{0:0.######}", values[index].Value);
                }
            }
            catch (Exception ex)
            {
                string error = string.Format(" {0} ;", ex.Message);
                try
                {
                    LogErrorData(sr, values, table.TableName);
                }
                catch (Exception e)
                {
                    error += e.Message;
                }
                throw new Exception(error);
            }


            string sql = string.Format(@"insert into {0} ({1}) values ({2})", table.TableName, table.Colums,
                valueStr);
            return sql;
        }

        private string GetTableCommonColumns(SensorAcqResult sar)
        {
            string valuecommoncolumns =
                string.Format("{0},{1},{2},'{3}','{4:yyyy-MM-dd HH:mm:ss.fff}',{5},{6}", sar.Sensor.SensorID,
                    sar.Sensor.ModuleNo, sar.Sensor.ChannelNo, sar.Sensor.Name, sar.ResponseTime, sar.Sensor.DtuID,
                    sar.Sensor.FactorType);
            return valuecommoncolumns;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawvalues"></param>
        /// <param name="themevalues"></param>
        /// <returns></returns>
        public double?[] MergeRawvaluesAndThemeValues(double[] rawvalues, double?[] themevalues)
        {
            if (rawvalues == null || rawvalues.Length == 0)
                return themevalues;
            var values = rawvalues.Select(value => (double?)value).ToList();
            if(themevalues!=null)
                values.AddRange(themevalues);
            return values.ToArray();
        }

        public bool LogErrorData(SensorAcqResult sar, double?[] values, string tablename = "")
        {
            var errdata = new StringBuilder();
            errdata.AppendFormat("{0},{1},{2},'{3}','{4:yyyy-MM-dd HH:mm:ss.fff}',{5},{6}", sar.Sensor.SensorID,
                sar.Sensor.ModuleNo, sar.Sensor.ChannelNo, sar.Sensor.Name, sar.ResponseTime, sar.Sensor.DtuID,
                sar.Sensor.FactorType);
            foreach (var d in values)
            {
                if (d != null)
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

