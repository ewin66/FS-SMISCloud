namespace FreeSun.FS_SMISCloud.Server.CloudApi.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Text;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Entity;    
    

    /// <summary>
    /// 通用数据提取
    /// </summary>
    public class Data
    {
        /// <summary>
        /// The get monitor data.
        /// </summary>
        /// <param name="sensors"> 传感器 </param>
        /// <param name="startDate"> 开始时间 </param>
        /// <param name="endDate"> 结束时间 </param>
        /// <param name="interval"> 采点间隔 </param>
        /// <param name="datename"> 间隔单位 </param>
        /// <returns>
        /// The <see cref="IList"/>. 
        /// </returns>
        public IList<MonitorData> GetMonitorData(string sensors, DateTime startDate, DateTime endDate, int interval, string datename)
        {            
            IList<MonitorData> data = new List<MonitorData>();
            // 分解传感器列表
            string[] sens = sensors.Split(',');
            var sensorArray = sens.Select(s => Convert.ToInt32(s));

            foreach (int sensorId in sensorArray)
            {
                // 查询传感器监测类型
                int? factorId = null;
                factorId = this.GetFactorId(sensorId);

                int structId = 0;
                structId = this.GetStructId(sensorId);
                if (factorId == null)
                {
                    break;
                }

                // 读取配置文件
                IEnumerable<FactorConfig> config = Config.GetConfigByFactors(new int[] { Convert.ToInt32(factorId) }, structId);

                // 从配置中查找列
                var factorConfig = config.FirstOrDefault(c => c.Id == factorId);
                if (factorConfig == null)
                {
                    throw new ConfigurationErrorsException(string.Format("缺少 factorid:{0} 的配置数据", factorId));
                }

                // 构造sql语句
                string[] colums = new string[factorConfig.DisplayNumber];
                for (int i = 0; i < factorConfig.DisplayNumber; i++)
                {
                    colums[i] = string.Format("ROUND(t.{0},{1}) as {0}", factorConfig.Columns[i], factorConfig.DecimalPlaces[i]);
                }

                string values = string.Join(",", colums);

                string[] abs = new string[factorConfig.DisplayNumber];
                for (int i = 0; i < factorConfig.DisplayNumber; i++)
                {
                    abs[i] = string.Format("abs({0})", factorConfig.Columns[i]);
                }

                string absStr = string.Join(",", abs);
                // 采点条件
                string patition = this.BuilderPatitionString(interval, datename); // 构造采集规则

                string sqlStr =
                    string.Format(
                        @"select s.SENSOR_ID, t.ACQUISITION_DATETIME, s.SENSOR_LOCATION_DESCRIPTION, {0}
                            from(
                                select SENSOR_ID, {1}, ACQUISITION_DATETIME,
                                ROW_NUMBER() over(
	                                PARTITION by {6} 
	                                order by {7} desc) as rownum
                                from {2}
                                where SENSOR_ID={5} and ACQUISITION_DATETIME between '{3}' and '{4}') t
                            join T_DIM_SENSOR s on t.SENSOR_ID=s.SENSOR_ID
                        where t.rownum = 1
                        order by t.ACQUISITION_DATETIME",
                        values,
                        string.Join(",", factorConfig.Columns),
                        factorConfig.Table,
                        startDate,
                        endDate,
                        sensorId,
                        patition,
                        absStr);

                // 读取数据
                this.GetValueFromDB(sqlStr, data, factorConfig);
            }

            return data;
        }

        //新增获取结构物Id
        private int GetStructId(int sensorId)
        {
            int? structId;
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                structId =
                    entity.T_DIM_SENSOR.Where(s => s.SENSOR_ID == sensorId)
                        .Select(s => s.STRUCT_ID)
                        .FirstOrDefault();
            }

            return Convert.ToInt32(structId);
        }

        /// <summary>
        /// 获取传感器原始数据
        /// </summary>
        /// <param name="sensors"> 传感器 </param>
        /// <param name="startDate"> 开始时间 </param>
        /// <param name="endDate"> 结束时间 </param>
        /// <param name="interval"> 采点间隔 </param>
        /// <param name="datename"> 间隔单位 </param>
        /// <returns>
        /// The <see cref="IList"/>. 
        /// </returns>
        public IList<OriginalData> GetOriginalData(string sensors, DateTime startDate, DateTime endDate, int interval, string datename)
        {
            IList<OriginalData> data = new List<OriginalData>();
            // 分解传感器列表
            string[] sens = sensors.Split(',');
            var sensorArray = sens.Select(s => Convert.ToInt32(s));

            foreach (int sensorId in sensorArray)
            {
                // 查询传感器监测类型
                int? productId = null;
                productId = this.GetProductId(sensorId);
                if (productId == null)
                {
                    break;
                }

                // 读取配置文件
                IEnumerable<OriginalConfig> config = Config.GetConfigByProduct(Convert.ToInt32(productId));

                // 从配置中查找列
                var originalConfig = config.FirstOrDefault(c => c.ProductId == productId);
                if (originalConfig == null)
                {
                    throw new ConfigurationErrorsException(string.Format("缺少 productId:{0} 的配置数据", productId));
                }

                // 构造sql语句
                string[] colums = new string[originalConfig.DisplayNumber];
                for (int i = 0; i < originalConfig.DisplayNumber; i++)
                {
                    colums[i] = string.Format("ROUND(t.{0},{1}) as {0}", originalConfig.Columns[i], originalConfig.DecimalDigits[i]);
                }

                string values = string.Join(",", colums);

                string[] abs = new string[originalConfig.DisplayNumber];
                for (int i = 0; i < originalConfig.DisplayNumber; i++)
                {
                    abs[i] = string.Format("abs({0})", originalConfig.Columns[i]);
                }

                string absStr = string.Join(",", abs);
                // 采点条件
                string patition = this.BuilderPatitionString("CollectTime", interval, datename); // 构造采集规则

                string sqlStr =
                    string.Format(
                        @"select s.SENSOR_ID, t.CollectTime, s.SENSOR_LOCATION_DESCRIPTION, {0}
                            from(
                                select SensorId, {1}, CollectTime, 
                                ROW_NUMBER() over(
	                                PARTITION by {6} 
	                                order by {7} desc) as rownum
                                from {2}
                                where SensorId={5} and CollectTime between '{3}' and '{4}') t
                            join T_DIM_SENSOR s on t.SensorId=s.SENSOR_ID
                        where t.rownum = 1
                        order by t.CollectTime",
                        values,
                        string.Join(",", originalConfig.Columns),
                        originalConfig.Table,
                        startDate,
                        endDate,
                        sensorId,
                        patition,
                        absStr);

                // 读取数据
                this.GetOriginalValueFromDB(sqlStr, originalConfig, data);
            }

            return data;
        }

        /// <summary>
        /// 构造分区字符串
        /// </summary>
        /// <param name="interval"> 时间间隔 </param>
        /// <param name="datename"> 时间单位 </param>
        /// <returns> The <see cref="string"/>. </returns>
        private string BuilderPatitionString(int interval, string datename)
        {
            string[] patitions =
            {
                "YEAR(ACQUISITION_DATETIME)",
                "MONTH(ACQUISITION_DATETIME)",
                "DAY(ACQUISITION_DATETIME)",
                "DATEPART(hh,ACQUISITION_DATETIME)",
                "DATEPART(mi,ACQUISITION_DATETIME)",
                "DATEPART(ss,ACQUISITION_DATETIME)"
            };

            Dictionary<string, int> mapping = new Dictionary<string, int>(6);
            mapping.Add("year", 0);
            mapping.Add("month", 1);
            mapping.Add("day", 2);
            mapping.Add("hour", 3);
            mapping.Add("minute", 4);
            mapping.Add("second", 5);

            string[] list = new string[mapping[datename] + 1];
            for (int i = 0; i < mapping[datename]; i++)
            {
                list[i] = patitions[i];
            }

            list[mapping[datename]] = string.Format("{0}/{1}", patitions[mapping[datename]], interval);

            return string.Join(",", list);
        }

        /// <summary>
        /// 构造分区字符串
        /// </summary>
        /// <param name="interval"> 时间间隔 </param>
        /// <param name="datename"> 时间单位 </param>
        /// <returns> The <see cref="string"/>. </returns>
        private string BuilderPatitionString(string fieldTime, int interval, string datename)
        {
            string[] patitions =
            {
                "YEAR(" + fieldTime + ")",
                "MONTH(" + fieldTime + ")",
                "DAY(" + fieldTime + ")",
                "DATEPART(hh," + fieldTime + ")",
                "DATEPART(mi," + fieldTime + ")",
                "DATEPART(ss," + fieldTime + ")"
            };

            Dictionary<string, int> mapping = new Dictionary<string, int>(6);
            mapping.Add("year", 0);
            mapping.Add("month", 1);
            mapping.Add("day", 2);
            mapping.Add("hour", 3);
            mapping.Add("minute", 4);
            mapping.Add("second", 5);

            string[] list = new string[mapping[datename] + 1];
            for (int i = 0; i < mapping[datename]; i++)
            {
                list[i] = patitions[i];
            }

            list[mapping[datename]] = string.Format("{0}/{1}", patitions[mapping[datename]], interval);

            return string.Join(",", list);
        }

        /// <summary>
        /// The get last monitor data.
        /// </summary>
        /// <param name="sensors"> The sensors. </param>
        /// <returns> The <see cref="IList"/>. </returns>
        /// <exception cref="ConfigurationException"> 配置缺失异常 </exception>
        public IList<MonitorData> GetLastMonitorData(int[] sensors)
        {
            IList<MonitorData> data = new List<MonitorData>();                       
            // 查询传感器监测类型
            IEnumerable<FactorMapping> factors = null;
            factors = this.GetFactorId(sensors);

            var structId = Config.GetStructId(sensors[0]);
            IEnumerable<FactorConfig> config = Config.GetConfigByFactors(factors.Select(m => m.FactorId).ToArray(), structId);

            foreach (FactorMapping fm in factors)
            {
                // 从配置中查找列
                var factorConfig = config.FirstOrDefault(c => c.Id == fm.FactorId);
                if (factorConfig == null)
                {
                    throw new ConfigurationErrorsException(string.Format("缺少 factorid:{0} 的配置数据", fm.FactorId));
                }
                // 构造sql语句
                string[] colums = new string[factorConfig.DisplayNumber];
                for (int i = 0; i < factorConfig.DisplayNumber; i++)
                {
                    colums[i] = string.Format("d.{0}", factorConfig.Columns[i]);
                }

                StringBuilder sens = new StringBuilder(100);
                foreach (var sensor in fm.Sensors)
                {
                    sens.Append(sensor).Append(",");
                }

                sens.Remove(sens.Length - 1, 1);

                string values = string.Join(",", colums);

                string sql = string.Format(@"select MAX(ID) from {0}
	                            where SENSOR_ID in ({1})
	                            group by SENSOR_ID", factorConfig.Table, sens);

                var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnectionString, CommandType.Text, sql, null);

                var topIds = new List<int>(sens.Length);

                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        topIds.Add(rdr.GetInt32(0));
                    }

                    string sqlStr =
                        string.Format(@"select s.SENSOR_ID, d.ACQUISITION_DATETIME, s.SENSOR_LOCATION_DESCRIPTION, {0}
                          from T_DIM_SENSOR s
                          JOIN {1} d on d.SENSOR_ID= s.SENSOR_ID
                            and d.ID in({2})", values, factorConfig.Table, string.Join(",", topIds));
                    // 读取数据
                    this.GetValueFromDB(sqlStr, data, factorConfig);
                }
            }

            return data;
        }

        /// <summary>
        /// 将监测数据读入到数据集合
        /// </summary>
        /// <param name="sqlStr"> sql语句 </param>
        /// <param name="data"> 数据集合 </param>
        /// <param name="config">配置</param>
        private void GetValueFromDB(string sqlStr, IList<MonitorData> data, FactorConfig config)
        {
            MonitorData temp;
            DataTable dt = SqlHelper.ExecuteDataSetText(sqlStr, null).Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                temp = new MonitorData();
                temp.SensorId = Convert.ToInt32(row[0]);
                temp.AcquisitionTime = Convert.ToDateTime(row[1]);
                temp.Location = Convert.ToString(row[2]);

                temp.Values = new decimal?[row.ItemArray.Length - 3];
                for (int i = 3; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        temp.Values[i - 3] =
                            Convert.ToDecimal(Convert.ToDecimal(row[i]).ToString("f" + config.DecimalPlaces[i - 3]));
                    }
                }
                
                temp.Columns = config.Display;
                temp.Unit = config.Unit;
                data.Add(temp);
            }
        }

        /// <summary>
        /// 将原始数据读入到数据集合
        /// </summary>
        /// <param name="sqlStr"> sql语句 </param>
        /// <param name="data"> 数据集合 </param>
        /// <param name="config">配置</param>
        private void GetOriginalValueFromDB(string sqlStr, OriginalConfig config, IList<OriginalData> data)
        {
            OriginalData od;
            DataTable dt = SqlHelper.ExecuteDataSetText(sqlStr, null).Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                od = new OriginalData();
                od.SensorId = Convert.ToInt32(row[0]);
                od.AcquisitionTime = Convert.ToDateTime(row[1]);
                od.Location = Convert.ToString(row[2]);

                od.Values = new decimal[row.ItemArray.Length - 3];
                for (int i = 3; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        od.Values[i - 3] =
                            Convert.ToDecimal(Convert.ToDecimal(row[i]).ToString("f" + config.DecimalDigits[i - 3]));
                    }
                }

                od.Columns = config.Display;
                od.Unit = config.Unit;
                data.Add(od);
            }
        }

        /// <summary>
        /// 获取传感器监测因素编号
        /// </summary>
        /// <param name="sensorId"> 传感器编号 </param>
        /// <returns> <see cref="int?"/> 监测因素编号 </returns>
        private int? GetFactorId(int sensorId)
        {
            int? factorId;
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                factorId =
                    entity.T_DIM_SENSOR.Where(s => s.SENSOR_ID == sensorId)
                        .Select(s => s.SAFETY_FACTOR_TYPE_ID)
                        .FirstOrDefault();
            }

            return factorId;
        }

        /// <summary>
        /// 获取传感器产品编号
        /// </summary>
        /// <param name="sensorId"> 传感器编号 </param>
        /// <returns> <see cref="int?"/> 监测因素编号 </returns>
        private int? GetProductId(int sensorId)
        {
            int? ProductCatagoryId;
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                ProductCatagoryId = (from sensor in entity.T_DIM_SENSOR
                                        from sp in entity.T_DIM_SENSOR_PRODUCT
                                        where
                                            sensor.SENSOR_ID == sensorId && sensor.PRODUCT_SENSOR_ID == sp.PRODUCT_ID
                                        select sp.PRODUCT_TYPE_KEY).ToList().FirstOrDefault();
            }

            return ProductCatagoryId;
        }

        /// <summary>
        /// 获取传感器监测因素编号
        /// </summary>
        /// <param name="sensors"> 传感器 </param>
        /// <returns> The <see cref="Dictionary"/>. </returns>
        private IEnumerable<FactorMapping> GetFactorId(int[] sensors)
        {
            IEnumerable<FactorMapping> factors;
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                factors =
                    entity.T_DIM_SENSOR
                        .Where(s => sensors.Contains(s.SENSOR_ID)).ToList()
                        .GroupBy(s => new
                        {
                            factorID = s.SAFETY_FACTOR_TYPE_ID,
                            structId = s.STRUCT_ID
                        })
                        .Select(s => new FactorMapping
                        {
                            FactorId = Convert.ToInt32(s.Key.factorID),
                            Sensors = s.Select(i => i.SENSOR_ID),
                            StructId = Convert.ToInt32(s.Key.structId)
                        });
            }

            return factors;
        }
    }

    internal class FactorMapping
    {
        public int FactorId { get; set; }

        public IEnumerable<int> Sensors { get; set; }

        public int StructId { get; set; }
    }
}