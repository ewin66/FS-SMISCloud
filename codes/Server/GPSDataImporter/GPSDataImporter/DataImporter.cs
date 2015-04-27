// --------------------------------------------------------------------------------------------
// <copyright file="DataImporter.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20140527
// 
// 修改标识：刘歆毅 20140912
// 修改描述：实现数据源可配置
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace GPSDataImporter
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using log4net;

    internal class DataImporter
    {        
        // 初值缓存
        private static Dictionary<string, InitialValue> initialValues = new Dictionary<string, InitialValue>();
        // logger
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // 数据目标
        private string destinationDBConnStr = ConfigurationManager.ConnectionStrings["DestinationDB"].ConnectionString;
        // 目标表
        private string destinationTable = "dbo.T_THEMES_DEFORMATION_SURFACE_DISPLACEMENT";
        // 单例
        private static DataImporter instance;
        private DataImporter()
        {
        }
        public static DataImporter GetInstance()
        {
            DataImporter importer;
            if ((importer = DataImporter.instance) == null)
            {
                importer = (DataImporter.instance = new DataImporter());
            }
            return importer;
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public void ImportData()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("开始导入数据..");
            Console.WriteLine("查询映射..");
            List<SourceMapping> mapping = this.GetMapping();
            Console.WriteLine("完成");
            Console.WriteLine("查询数据未更新时间跨度..");
            Dictionary<string, TimeSpan> timeSpan = this.GetTimeSpan(mapping);
            Console.WriteLine("完成");
            foreach (var m in mapping)
            {
                Console.WriteLine("{0},{1},{2}", m.SensorId, m.SourceSqlConn, m.SourceTable);
            }
            Console.WriteLine("查询初值..");
            Dictionary<string, InitialValue> initialValue = this.GetInitialValue(mapping);
            foreach (var value in initialValue)
            {
                Console.WriteLine("id:{0},初值:{1},{2},{3}", value.Key, value.Value.X, value.Value.Y, value.Value.Z);
            }
            Console.WriteLine("完成");
            Console.WriteLine("查询需要更新的数据..");
            DataTable data2Update = this.GetData2Update(mapping, initialValue, timeSpan);
            Console.WriteLine("完成");
            Console.WriteLine("正在导入目标数据库..");
            using (SqlConnection sqlConnection = new SqlConnection(this.destinationDBConnStr))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlConnection.ConnectionString, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    sqlBulkCopy.ColumnMappings.Add(0, "SENSOR_ID");
                    sqlBulkCopy.ColumnMappings.Add(1, "SAFETY_FACTOR_TYPE_ID");
                    sqlBulkCopy.ColumnMappings.Add(2, "SURFACE_DISPLACEMENT_X_VALUE");
                    sqlBulkCopy.ColumnMappings.Add(3, "SURFACE_DISPLACEMENT_Y_VALUE");
                    sqlBulkCopy.ColumnMappings.Add(4, "SURFACE_DISPLACEMENT_Z_VALUE");
                    sqlBulkCopy.ColumnMappings.Add(5, "ACQUISITION_DATETIME");
                    sqlBulkCopy.DestinationTableName = this.destinationTable;
                    sqlBulkCopy.BulkCopyTimeout = 1800;
                    sqlBulkCopy.WriteToServer(data2Update);
                }
                sqlConnection.Close();
            }
            stopwatch.Stop();
            string text = string.Format("导入结束,导入{0}条数据,用时：{1}秒", data2Update.Rows.Count, stopwatch.Elapsed.TotalSeconds);
            Console.WriteLine(text);
            this.logger.Info(text);
        }

        /// <summary>
        /// 获取要更新的数据
        /// </summary>
        /// <param name="mapping">数据映射</param>
        /// <param name="initialValue">初值</param>
        /// <param name="timeSpan">时间间隔</param>
        /// <returns></returns>
        private DataTable GetData2Update(List<SourceMapping> mapping, Dictionary<string, InitialValue> initialValue, Dictionary<string, TimeSpan> timeSpan)
        {
            DataTable rslt = new DataTable();
            foreach (var current in mapping)
            {
                string key = current.SensorId;
                InitialValue initialValue2 = initialValue[key];
                double? x = initialValue2.X;
                string num = x.HasValue ? "-" + x.GetValueOrDefault() : string.Empty;
                double? y = initialValue2.Y;
                string num2 = y.HasValue ? "-" + y.GetValueOrDefault() : string.Empty;
                double? z = initialValue2.Z;
                string num3 = z.HasValue ? "-" + z.GetValueOrDefault() : string.Empty;
                string text = string.Empty;
                if (timeSpan.ContainsKey(key) && timeSpan[key].DestinationUpdateTime.HasValue && timeSpan[key].SourceUpdateTime.HasValue)
                {
                    text = string.Format(" where aDatetime > '{0}' ", timeSpan[key].DestinationUpdateTime);
                }
                string sql = string.Format(
                    "select '{0}','9',X{1},Y{2},Height{3},aDatetime from {4} {5}",
                    new object[] { key, num, num2, num3, current.SourceTable, text });
                var table = SqlHelper.ExecuteDataSet(current.SourceSqlConn, CommandType.Text, sql, null).Tables[0];
                rslt.Merge(table);
            }
            return rslt;
        }

        /// <summary>
        /// 获取映射配置 传感器id-对应表
        /// </summary>
        /// <returns></returns>
        private List<SourceMapping> GetMapping()
        {
            var mapping = new List<SourceMapping>(20);
            string cmdText = @"select SENSOR_ID,GPS_TABLE,GPS_CONN from T_DIM_SENSOR_GPS";
            DataTable dt = SqlHelper.ExecuteDataSet(this.destinationDBConnStr, CommandType.Text, cmdText, null).Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                mapping.Add(
                    new SourceMapping
                        {
                            SensorId = row[0].ToString(),
                            SourceTable = row[1].ToString(),
                            SourceSqlConn = row[2].ToString()
                        });
            }
            return mapping;
        }

        #region 时间差异
        /// <summary>
        /// 获取时间段
        /// </summary>
        /// <param name="mapping">数据源配置</param>
        /// <returns></returns>
        private Dictionary<string, TimeSpan> GetTimeSpan(List<SourceMapping> mapping)
        {
            // 获取数据源最后更新时间
            Dictionary<string, DateTime?> sourceUpdateTime = this.GetSourceUpdateTime(mapping);
            // 获取目标最后更新时间
            Dictionary<string, DateTime?> destinationUpdateTime = this.GetDestinationUpdateTime(mapping);
            // 获取 传感器id-时间段 字典
            Dictionary<string, TimeSpan> dictionary = new Dictionary<string, TimeSpan>(mapping.Count);
            // 无效配置列表
            List<SourceMapping> errorMapping = new List<SourceMapping>();

            foreach (var m in mapping)
            {
                // 检查无效配置
                if (!sourceUpdateTime.Keys.Contains(m.SensorId))
                {
                    errorMapping.Add(m);
                    continue;
                }
                dictionary.Add(
                    m.SensorId,
                    new TimeSpan
                        {
                            SourceUpdateTime = sourceUpdateTime[m.SensorId],
                            DestinationUpdateTime =
                                destinationUpdateTime.Keys.Contains(m.SensorId)
                                    ? destinationUpdateTime[m.SensorId]
                                    : null
                        });
            }
            // 移除无效配置
            foreach (var error in errorMapping)
            {
                mapping.Remove(error);
            }

            return dictionary;
        }

        // 获取目标最后更新时间
        private Dictionary<string, DateTime?> GetDestinationUpdateTime(List<SourceMapping> mapping)
        {
            string sensors = string.Join(",", mapping.Select(m => m.SensorId).ToArray());
            string cmdText = string.Format(@"select SENSOR_ID,MAX(ACQUISITION_DATETIME)
                                            from {1}
                                            where SENSOR_ID in ({0})
                                            group by SENSOR_ID", sensors, this.destinationTable);
            Dictionary<string, DateTime?> dictionary = new Dictionary<string, DateTime?>(mapping.Count);
            var dt = SqlHelper.ExecuteDataSet(this.destinationDBConnStr, CommandType.Text, cmdText, null).Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                dictionary.Add(row[0].ToString(), (row[1] == DBNull.Value) ? null : new DateTime?(Convert.ToDateTime(row[1])));
            }
            return dictionary;
        }

        // 获取源最后更新时间
        private Dictionary<string, DateTime?> GetSourceUpdateTime(List<SourceMapping> mapping)
        {
            Dictionary<string, DateTime?> dictionary = new Dictionary<string, DateTime?>(mapping.Count);

            foreach (var m in mapping)
            {
                string sql = string.Format("select '{0}',MAX(aDatetime) from {1}", m.SensorId, m.SourceTable);
                try
                {
                    var dt = SqlHelper.ExecuteDataSet(m.SourceSqlConn, CommandType.Text, sql, null).Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        dictionary.Add(row[0].ToString(), (row[1] == DBNull.Value) ? null : new DateTime?(Convert.ToDateTime(row[1])));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("警告:\t传感器:{2}配置,数据源:{0},数据表:{1}无效", m.SourceSqlConn, m.SourceTable, m.SensorId);
                }
            }

            return dictionary;
        }
        #endregion

        #region 初值
        /// <summary>
        /// 获取初值
        /// </summary>
        /// <param name="mapping">传感器id-对应表</param>
        /// <returns></returns>
        private Dictionary<string, InitialValue> GetInitialValue(List<SourceMapping> mapping)
        {
            int count = mapping.Count;
            Dictionary<string, InitialValue> dictionary = new Dictionary<string, InitialValue>(count);
            // 从初值缓存中读取
            foreach (KeyValuePair<string, InitialValue> current in DataImporter.initialValues)
            {
                dictionary.Add(current.Key, current.Value);
            }
            if (dictionary.Count == count)
            {
                return dictionary;
            }
            // 查找没有缓存初值的传感器id
            IList<string> sensors = new List<string>();
            foreach (string sensor in mapping.Select(m => m.SensorId))
            {
                if (!dictionary.ContainsKey(sensor))
                {
                    sensors.Add(sensor);
                }
            }
            // 获取数据库配置的初值
            Dictionary<string, InitialValue> initialValueConfig = this.GetInitialValueConfig(sensors);
            //Console.WriteLine("配置的初值:");
            //foreach (var initialValue in initialValueConfig)
            //{                
            //    Console.WriteLine("{0},{1},{2},{3}",initialValue.Key, initialValue.Value.X, initialValue.Value.Y, initialValue.Value.Z);
            //}
            // 获取数据源初值
            Dictionary<string, InitialValue> sourceInitialValue = this.GetSourceInitialValue(sensors, mapping);
            //Console.WriteLine("数据源的初值:");
            //foreach (var initialValue in sourceInitialValue)
            //{                
            //    Console.WriteLine("{0},{1},{2},{3}", initialValue.Key, initialValue.Value.X, initialValue.Value.Y, initialValue.Value.Z);
            //}
            // 更新初值配置            
            Dictionary<string, InitialValue> dictionary2 = this.UpdateInitialValueConfig(
                initialValueConfig,
                sourceInitialValue);
            foreach (KeyValuePair<string, InitialValue> current3 in dictionary2)
            {
                dictionary.Add(current3.Key, current3.Value);
                if (current3.Value.X == null || current3.Value.Y == null || current3.Value.Z == null)
                {
                    continue;
                }
                DataImporter.initialValues.Add(current3.Key, current3.Value);                
            }
            return dictionary;
        }

        /// <summary>
        /// 更新初值配置
        /// </summary>        
        /// <param name="sensors">传感器列表</param>
        /// <param name="initialConfig">配置的初值</param>
        /// <param name="sourceInitialValue">源数据初值</param>        
        private Dictionary<string, InitialValue> UpdateInitialValueConfig(Dictionary<string, InitialValue> initialConfig, Dictionary<string, InitialValue> sourceInitialValue)
        {
            Dictionary<string, InitialValue> dictionary = new Dictionary<string, InitialValue>();
            IList<string> sqlX = new List<string>();
            IList<string> sqlY = new List<string>();
            IList<string> sqlZ = new List<string>();
            // 遍历配置的初值
            foreach (var current in initialConfig)
            {
                // 是否配置了x方向初值
                double? x = null;
                if (!initialConfig[current.Key].X.HasValue)
                {
                    // 源数据表是否有初值                    
                    if (sourceInitialValue.Keys.Contains(current.Key) && sourceInitialValue[current.Key].X.HasValue)
                    {
                        x = sourceInitialValue[current.Key].X;
                        sqlX.Add(string.Format("when {0} then {1}", current.Key, x));
                    }
                }
                else
                {
                    x = initialConfig[current.Key].X;
                }

                double? y = null;
                if (!initialConfig[current.Key].Y.HasValue)
                {

                    if (sourceInitialValue.Keys.Contains(current.Key) && sourceInitialValue[current.Key].Y.HasValue)
                    {
                        y = sourceInitialValue[current.Key].Y;
                        sqlY.Add(string.Format("when {0} then {1}", current.Key, y));
                    }
                }
                else
                {
                    y = initialConfig[current.Key].Y;
                }

                double? z = null;
                if (!initialConfig[current.Key].Z.HasValue)
                {                    
                    if (sourceInitialValue.Keys.Contains(current.Key) && sourceInitialValue[current.Key].Z.HasValue)
                    {
                        z = sourceInitialValue[current.Key].Z;
                        sqlZ.Add(string.Format("when {0} then {1}", current.Key, z));
                    }
                }
                else
                {
                    z = initialConfig[current.Key].Z;
                }

                InitialValue value = new InitialValue
                {
                    X = x,
                    Y = y,
                    Z = z
                };
                dictionary.Add(current.Key, value);
            }

            if (sqlX.Count == 0 && sqlY.Count == 0 && sqlZ.Count == 0)
            {
                return dictionary;
            }
            string arg = (sqlX.Count == 0) ? string.Empty : string.Format("INITIAL_X = case SENSOR_ID {0} else INITIAL_X end,", string.Join(" ", sqlX));
            string arg2 = (sqlY.Count == 0) ? string.Empty : string.Format("INITIAL_Y = case SENSOR_ID {0} else INITIAL_Y end,", string.Join(" ", sqlY));
            string arg3 = (sqlZ.Count == 0) ? string.Empty : string.Format("INITIAL_Z = case SENSOR_ID {0} else INITIAL_Z end,", string.Join(" ", sqlZ));
            string text = string.Format(@"update T_DIM_SENSOR_GPS
                                          set {0} {1} {2}", arg, arg2, arg3);
            text = text.Remove(text.LastIndexOf(','), 1);
            SqlHelper.ExecteNonQuery(this.destinationDBConnStr, CommandType.Text, text, null);
            return dictionary;
        }

        /// <summary>
        /// 获取源数据库初值
        /// </summary>
        /// <param name="sensors">传感器列表</param>
        /// <param name="mapping">传感器id-对应表</param>        
        private Dictionary<string, InitialValue> GetSourceInitialValue(IList<string> sensors, List<SourceMapping> mapping)
        {
            Dictionary<string, InitialValue> dictionary = new Dictionary<string, InitialValue>();
            foreach (var sensor in sensors)
            {
                var config = mapping.FirstOrDefault(m => m.SensorId == sensor);
                if (config == null)
                {
                    Console.WriteLine("警告:传感器{0}无数据源配置");
                    continue;
                }
                string sql =
                    string.Format(
                        "select '{0}',X,Y,Height from {1} where GPSIndex = (SELECT MIN(GPSIndex) FROM {1})",
                        sensor,
                        config.SourceTable);
                try
                {
                    var dt = SqlHelper.ExecuteDataSet(config.SourceSqlConn, CommandType.Text, sql, null).Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        InitialValue value = new InitialValue
                                                 {
                                                     X =
                                                         (row[1] == DBNull.Value)
                                                             ? null
                                                             : (double?)Convert.ToDouble(row[1]),
                                                     Y =
                                                         (row[2] == DBNull.Value)
                                                             ? null
                                                             : (double?)Convert.ToDouble(row[2]),
                                                     Z =
                                                         (row[3] == DBNull.Value)
                                                             ? null
                                                             : (double?)Convert.ToDouble(row[3])
                                                 };
                        dictionary.Add(row[0].ToString(), value);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("警告:\t传感器:{2}配置,数据源:{0},数据表:{1}无效", config.SourceSqlConn, config.SourceTable, config.SensorId);
                }
            }
            return dictionary;
        }

        /// <summary>
        /// 获取数据库配置的初值
        /// </summary>
        /// <param name="sensors">传感器列表</param>        
        private Dictionary<string, InitialValue> GetInitialValueConfig(IList<string> sensors)
        {
            Dictionary<string, InitialValue> dictionary = new Dictionary<string, InitialValue>();
            string arg = string.Join(",", sensors);
            string cmdText = string.Format(@"select SENSOR_ID,INITIAL_X,INITIAL_Y,INITIAL_Z
                                                from dbo.T_DIM_SENSOR_GPS
                                                where SENSOR_ID in ({0})", arg);
            var dt = SqlHelper.ExecuteDataSet(this.destinationDBConnStr, CommandType.Text, cmdText, null).Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                InitialValue value = new InitialValue
                {
                    X = (row[1] == DBNull.Value) ? null : new double?(Convert.ToDouble(row[1])),
                    Y = (row[2] == DBNull.Value) ? null : new double?(Convert.ToDouble(row[2])),
                    Z = (row[3] == DBNull.Value) ? null : new double?(Convert.ToDouble(row[3]))
                };
                dictionary.Add(row[0].ToString(), value);
            }
            return dictionary;
        }
        #endregion
    }
}