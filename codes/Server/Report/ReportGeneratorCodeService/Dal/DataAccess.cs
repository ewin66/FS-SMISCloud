// --------------------------------------------------------------------------------------------
// <copyright file="DataAccess.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：报表信息访问
// 
// 创建标识：
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Security;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using ReportGeneratorService.DataModel;
using ReportGeneratorService.DataModule;
using System.Configuration;
using System.Data.SqlClient;

namespace ReportGeneratorService.Dal
{
    public class DataAccess
    {

        /// <summary>
        /// 获取组织信息
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public static Organization GetOrganizationInfo(int? orgId)
        {
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {
                return (from o in db.T_DIM_ORGANIZATION
                        join r in db.T_DIM_REGION on o.REGION_ID equals r.REGION_ID into r1
                        from region in r1.DefaultIfEmpty()
                        where o.ID == orgId
                        select
                            new Organization
                            {
                                Id = o.ID,
                                Name = o.ABB_NAME_CN,
                                Region = region.REGION_FULL_NAME_CN,
                                Address = o.ADDRESS_STREET_CN,
                                Phone = o.FIXED_PHONE_NUMBER,
                                ZipCode = o.ZIPCODE,
                                Website = o.WEBSITE,
                                Email = o.EMAIL,
                                SystemName = o.SystemName
                            }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取结构物信息
        /// </summary>
        /// <param name="structId"></param>
        /// <returns></returns>
        public static Structure GetStructureInfo(int? structId)
        {
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {
                return (from s in db.T_DIM_STRUCTURE
                        join r in db.T_DIM_REGION on s.STRUCTURE_REGION equals r.REGION_ID into r1
                        from region in r1.DefaultIfEmpty()
                        join t in db.T_DIM_STRUCTURE_TYPE on s.STRUCTURE_TYPE_ID equals t.ID into t1
                        from type in t1.DefaultIfEmpty()
                        where s.ID == structId
                        select
                            new Structure
                            {
                                Id = s.ID,
                                Name = s.STRUCTURE_NAME_CN,
                                Region = region.REGION_FULL_NAME_CN,
                                Address = s.STRUCTURE_DETAIL_ADDRESS,
                                ConstructionCompany = s.CONSTRUCTION_COMPANY_NAME,
                                Type = type.NAME_STRUCTURE_TYPE_CN
                            }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取监测因素信息
        /// </summary>
        /// <param name="factorId"></param>
        /// <returns></returns>

        public static Factor GetFactorInfoById(int? factorId)
        {
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {
                var data = (from f in db.T_DIM_SAFETY_FACTOR_TYPE
                            join fp in db.T_DIM_SAFETY_FACTOR_TYPE on f.SAFETY_FACTOR_TYPE_PARENT_ID equals
                                fp.SAFETY_FACTOR_TYPE_ID into t
                            from theme in t.DefaultIfEmpty()
                            where f.SAFETY_FACTOR_TYPE_ID == factorId
                            select new
                            {
                                f,
                                theme
                            }).ToList();

                return (from a in data
                        select
                            new Factor
                            {
                                Id = a.f.SAFETY_FACTOR_TYPE_ID,
                                NameCN = a.f.SAFETY_FACTOR_TYPE_NAME,
                                NameEN = a.f.SAFETY_FACTOR_TYPE_NAME_EN,
                                DisplayNumber = a.f.FACTOR_VALUE_COLUMN_NUMBER ?? 1,
                                Columns = a.f.THEMES_COLUMNS.Split(','),
                                Table = a.f.THEMES_TABLE_NAME,
                                Display = a.f.FACTOR_VALUE_COLUMNS.Split(','),
                                Unit = a.f.FACTOR_VALUE_UNIT.Split(','),
                                DecimalPlaces =
                                    a.f.FACTOR_VALUE_DECIMAL_PLACES.Split(',').Select(d => Convert.ToInt32(d)).ToArray(),
                                ThemeId = a.theme.SAFETY_FACTOR_TYPE_ID,
                                ThemeName = a.theme.SAFETY_FACTOR_TYPE_NAME
                            }).FirstOrDefault();
            }
        }


        /// <summary>
        /// 获取传感器监测因素编号
        /// </summary>
        /// <param name="sensors"> 传感器编号数组 </param>
        /// <returns> The <see cref="Dictionary{TKey,TValue}"/>. </returns>
        public static IEnumerable<FactorMapping> GetFactorId(int[] sensors)
        {
            IEnumerable<FactorMapping> factors;
            using (var entity = new DW_iSecureCloud_EmptyEntities())
            {
                factors =
                    entity.T_DIM_SENSOR
                        .Where(s => sensors.Contains(s.SENSOR_ID)).ToList()
                        .GroupBy(s => s.SAFETY_FACTOR_TYPE_ID)
                        .Select(s => new FactorMapping
                        {
                            FactorId = Convert.ToInt32(s.Key),
                            Sensors = s.Select(i => i.SENSOR_ID)
                        });
            }

            return factors;
        }

        /// <summary>
        /// 构造分区字符串
        /// </summary>
        /// <param name="interval"> 时间间隔 </param>
        /// <param name="datename"> 时间单位 </param>
        /// <returns> The <see cref="string"/>. </returns>
        public static string BuilderPatitionString(int interval, string datename)
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


        public static List<SensorData> GetSensorData(int[] sensors, DateTime start, DateTime end)
        {
            int interval = 1;
            string dateUnit = "minute";

            var data = new List<SensorData>();
            foreach (int sensorId in sensors)
            {
                // 查询传感器监测类型
                int? factorId = null;
                factorId = GetFactorId(sensorId);
                if (factorId == null)
                {
                    break;
                }

                // 读取配置文件
                IEnumerable<Factor> config = GetConfigByFactors(new int[] { Convert.ToInt32(factorId) });

                // 从配置中查找列
                var factorConfig = config.FirstOrDefault(c => c.Id == factorId);
                if (factorConfig == null)
                {
                    throw new ConfigurationErrorsException(string.Format("缺少 factorid:{0} 的配置数据", factorId));
                }

                // 构造sql语句
                var colums = new string[factorConfig.DisplayNumber];
                for (int i = 0; i < factorConfig.DisplayNumber; i++)
                {
                    colums[i] = string.Format("ROUND(t.{0},{1}) as {0}", factorConfig.Columns[i], factorConfig.DecimalPlaces[i]);
                }

                string values = string.Join(",", colums);

                var abs = new string[factorConfig.DisplayNumber];
                for (int i = 0; i < factorConfig.DisplayNumber; i++)
                {
                    abs[i] = string.Format("abs({0})", factorConfig.Columns[i]);
                }

                string absStr = string.Join(",", abs);
                // 采点条件
                string patition = BuilderPatitionString(interval, dateUnit); // 构造采集规则
                //                string sqlStr = string.Format(
                //                      @"SELECT s.SENSOR_ID,t.ACQUISITION_DATETIME,s.SENSOR_LOCATION_DESCRIPTION,{0}
                //                                                FROM {1} t
                //                                                join T_DIM_SENSOR s on t.SENSOR_ID=s.SENSOR_ID
                //                                                where t.SENSOR_ID = {2}
                //                                                and t.ACQUISITION_DATETIME between '{3}' and '{4}'
                //                                                order by t.ACQUISITION_DATETIME",
                //                    values,
                //                    factorConfig.Table,
                //                    sensorId,
                //                    start,
                //                    end);
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
                       start,
                       end,
                       sensorId,
                       patition,
                       absStr);
                // 读取数据
                GetValueFromDb(sqlStr, data, factorConfig);
            }
            return data;
        }

        public static IEnumerable<Factor> GetConfigByFactors(int[] factors)
        {
            using (var entity = new DW_iSecureCloud_EmptyEntities())
            {

                return entity.T_DIM_SAFETY_FACTOR_TYPE
                    .Where(f => factors.Contains(f.SAFETY_FACTOR_TYPE_ID))
                    .ToList()
                    .Select(f => new Factor
                    {
                        Id = f.SAFETY_FACTOR_TYPE_ID,
                        NameCN = f.SAFETY_FACTOR_TYPE_NAME,
                        NameEN = f.SAFETY_FACTOR_TYPE_NAME_EN,
                        DisplayNumber = f.FACTOR_VALUE_COLUMN_NUMBER ?? 1,
                        Columns = f.THEMES_COLUMNS.Split(','),
                        Table = f.THEMES_TABLE_NAME,
                        Display = f.FACTOR_VALUE_COLUMNS.Split(','),
                        Unit = f.FACTOR_VALUE_UNIT.Split(','),
                        DecimalPlaces =
                            f.FACTOR_VALUE_DECIMAL_PLACES.Split(',').Select(d => Convert.ToInt32(d)).ToArray()
                    });


            }
        }

        /// <summary>
        /// 将监测数据读入到数据集合
        /// </summary>
        /// <param name="sqlStr"> sql语句 </param>
        /// <param name="data"> 数据集合 </param>
        /// <param name="config">配置</param>
        public static void GetValueFromDb(string sqlStr, IList<SensorData> data, Factor config)
        {
            SensorData temp;

            DataSet ds = SqlHelper.ExecuteDataSetText(sqlStr, null);
            if (ds == null)
            {
                return;
            }

            // ds里面的表存储的是sqlstr查询的结果,按照sqlstr查询语句中字段顺序来确定缓存结果中的表的个字段

            if (ds.Tables.Count != 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    temp = new SensorData();
                    temp.SensorId = Convert.ToInt32(row[0]);
                    temp.AcquisitionTime = Convert.ToDateTime(row[1]);
                    temp.Location = Convert.ToString(row[2]);

                    temp.Values = new decimal[row.ItemArray.Length - 3];
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
        }

     

        /// <summary>
        /// 通用数据提取
        /// </summary>
        public static List<MonitorData> GetMonitorData(int[] sensors, DateTime start, DateTime end)
        {
            List<SensorData> list = GetSensorData(sensors, start, end);
            return list.OrderBy(l => l.SensorId)
                .GroupBy(s => new { s.SensorId, s.Location, s.Columns, s.Unit })
                  .Select(
                   d =>
                       new MonitorData
                       {
                           SensorId = d.Key.SensorId,
                           Location = d.Key.Location,
                           Columns = d.Key.Columns.ToList(),
                           Unit = d.Key.Unit.ToList(),
                           Data =
                           d.Select(
                           g =>
                               new Data
                               {
                                   Values = g.Values.ToList(),
                                   AcquisitionTime = g.AcquisitionTime
                               }).ToList()
                       }

                  ).ToList();

        }
        /// <summary>
        /// 3-12
        /// </summary>
        /// <param name="sensors"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<SensorData> GetAllData(int[] sensors, DateTime start, DateTime end)
        {
            List<SensorData> list = GetSensorData(sensors, start, end);
            return list;
        }

        /// <summary>
        /// 获取传感器监测因素编号
        /// </summary>
        /// <param name="sensorId"> 传感器编号 </param>
        /// <returns> <see cref="int?"/> 监测因素编号 </returns>
        public static int? GetFactorId(int sensorId)
        {
            int? factorId;
            using (var entity = new DW_iSecureCloud_EmptyEntities())
            {
                factorId =
                    entity.T_DIM_SENSOR.Where(s => s.SENSOR_ID == sensorId)
                        .Select(s => s.SAFETY_FACTOR_TYPE_ID)
                        .FirstOrDefault();
            }

            return factorId;
        }



        /// <summary>
        /// Get struct/{structid}/factors
        /// </summary>
        /// <param name="structId">结构物编号（只能是数字）</param>
        /// <returns>监测因素</returns>     
        public static List<Theme> FindFactorsByStruct(int structId)
        {

            using (var entity = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from stc in entity.T_DIM_STRUCTURE
                            from sf in entity.T_DIM_STRUCTURE_FACTOR
                            from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                            from t in entity.T_DIM_SAFETY_FACTOR_TYPE
                            where stc.ID == structId
                                  && stc.ID == sf.STRUCTURE_ID
                                  && sf.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                                  && f.SAFETY_FACTOR_TYPE_PARENT_ID == t.SAFETY_FACTOR_TYPE_ID
                            select new
                            {
                                FactorId = f.SAFETY_FACTOR_TYPE_ID,
                                FactorName = f.SAFETY_FACTOR_TYPE_NAME,
                                NameCN = f.SAFETY_FACTOR_TYPE_NAME,
                                NameEN = f.SAFETY_FACTOR_TYPE_NAME_EN,
                                ThemeId = f.SAFETY_FACTOR_TYPE_PARENT_ID,
                                ThemeName = f.SAFETY_FACTOR_TYPE_NAME,
                                Table = f.THEMES_TABLE_NAME,
                                Columns = f.THEMES_COLUMNS,
                                Display = f.FACTOR_VALUE_COLUMNS,
                                DecimalPlaces = f.FACTOR_VALUE_DECIMAL_PLACES,
                                Unit = f.FACTOR_VALUE_UNIT,
                                DisplayNumber = f.FACTOR_VALUE_COLUMN_NUMBER ?? 1
                            };

                var list = query.ToList();
                return (from l in list
                        group l by new { l.FactorId, l.FactorName }
                            into ss
                            select new Theme()
                            {
                                FactorId = ss.Key.FactorId,
                                FactorName = ss.Key.FactorName,
                                Children = (from s in ss
                                            select new Factor
                                            {
                                                Id = s.FactorId,
                                                NameCN = s.NameCN,
                                                NameEN = s.NameEN,
                                                ThemeId = (int)s.ThemeId,
                                                ThemeName = s.ThemeName,
                                                Table = s.Table,
                                                Columns = s.Columns.Split(','),
                                                Display = s.Display.Split(','),
                                                DecimalPlaces = s.DecimalPlaces.Split(',').Select(d => Convert.ToInt32(d)).ToArray(),
                                                Unit = s.Unit.Split(','),
                                                DisplayNumber = s.DisplayNumber

                                            }).ToList()
                            }).ToList();

            }
        }

        /// <summary>
        /// 获取结构物下的传感器列表
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 传感器列表 </returns>
        public static object FindSensorsByStruct(int structId)
        {
            string gps = ConfigurationManager.AppSettings["GPSBaseStation"];
            using (var entity = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from s in entity.T_DIM_SENSOR

                            from f in entity.T_DIM_SAFETY_FACTOR_TYPE
                            from p in entity.T_DIM_SENSOR_PRODUCT
                            where
                                s.PRODUCT_SENSOR_ID == p.PRODUCT_ID
                                && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                                && s.STRUCT_ID == structId && s.IsDeleted == false
                                && p.PRODUCT_NAME != gps
                            select
                                new
                                {
                                    sensorId = s.SENSOR_ID,
                                    factorName = f.SAFETY_FACTOR_TYPE_NAME,
                                    factorId = f.SAFETY_FACTOR_TYPE_ID,
                                    SensorType = p.PRODUCT_NAME,
                                    location = s.SENSOR_LOCATION_DESCRIPTION
                                };
                return query.ToList();
            }
        }

        /// <summary>
        /// 根据结构体和因素获取监测点 struct/{structid}/factor/{factorid}/sensors
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <param name="factorId"> 因素编号 </param>
        /// <returns> 监测点列表 <see cref="object"/>.
        /// </returns>

        public static List<SensorList> FindSensorsByStructAndFactor(int structId, int factorId)
        {
            string baseStation = ConfigurationManager.AppSettings["GPSBaseStation"];
            using (var entities = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from s in entities.T_DIM_SENSOR
                            from f in entities.T_DIM_SAFETY_FACTOR_TYPE
                            from p in entities.T_DIM_SENSOR_PRODUCT
                            where s.PRODUCT_SENSOR_ID == p.PRODUCT_ID
                                  && s.STRUCT_ID == structId
                                  && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                                  && f.SAFETY_FACTOR_TYPE_ID == factorId
                                  && p.PRODUCT_NAME != baseStation
                                  && s.IsDeleted == false &&(s.Identification==0||s.Identification==2)
                            select new
                            {
                                sensorid = s.SENSOR_ID,
                                location = s.SENSOR_LOCATION_DESCRIPTION,
                                sensortype = p.PRODUCT_NAME
                            };
                var list = query.ToList();

                return (from l in list
                        group l by l.sensortype
                            into g
                            select new SensorList()
                            {
                                SensorType = g.Key,
                                Sensors = (from s in g
                                           select new Sensor()
                                           {
                                               SensorId = s.sensorid,
                                               Location = s.location
                                           }).OrderBy(l => l.Location).ToList()
                            }).ToList();
            }
        }

        public static List<SensorList> FindSensorsByStructAndFactor(int structId, int factorId, int type)
        {
            string baseStation = ConfigurationManager.AppSettings["GPSBaseStation"];
            using (var entities = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from s in entities.T_DIM_SENSOR
                            from f in entities.T_DIM_SAFETY_FACTOR_TYPE
                            from p in entities.T_DIM_SENSOR_PRODUCT
                            where s.PRODUCT_SENSOR_ID == p.PRODUCT_ID
                                  && s.STRUCT_ID == structId
                                  && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                                  && f.SAFETY_FACTOR_TYPE_ID == factorId
                                  && p.PRODUCT_NAME != baseStation
                                  && s.Identification == type
                                  && s.IsDeleted == false
                            select new
                            {
                                sensorid = s.SENSOR_ID,
                                location = s.SENSOR_LOCATION_DESCRIPTION,
                                sensortype = p.PRODUCT_NAME
                            };
                var list = query.ToList();

                return (from l in list
                        group l by l.sensortype
                            into g
                            select new SensorList()
                            {
                                SensorType = g.Key,
                                Sensors = (from s in g
                                           select new Sensor()
                                           {
                                               SensorId = s.sensorid,
                                               Location = s.location
                                           }).ToList()
                            }).ToList();
            }
        }

        /// <summary>
        /// struct/{structid}/factor/deep-displace/sensors
        /// </summary>
        /// <param name="structId">结构物编号（只能是数字）</param>
        /// <returns>传感器组列表</returns>

        public static List<Group> FindDeepDisplaceSensorsByStruct(int structId)
        {
            int factorId = int.Parse(ConfigurationManager.AppSettings["DeepDisplaceFactorId"]);

            using (var entities = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from s in entities.T_DIM_SENSOR
                            from f in entities.T_DIM_SAFETY_FACTOR_TYPE
                            from sg in entities.T_DIM_SENSOR_GROUP_CEXIE
                            from g in entities.T_DIM_GROUP
                            where s.STRUCT_ID == structId
                                  && s.SAFETY_FACTOR_TYPE_ID == factorId
                                  && sg.SENSOR_ID == s.SENSOR_ID
                                  && g.GROUP_ID == sg.GROUP_ID
                                  && s.IsDeleted == false
                            select new
                            {
                                groupid = g.GROUP_ID,
                                groupname = g.GROUP_NAME,
                                depth = sg.DEPTH
                            };
                return (from q in query
                        group q by new { q.groupid, q.groupname }
                            into g
                            select new Group
                            {
                                GroupId = g.Key.groupid,
                                GroupName = g.Key.groupname,
                                MaxDepth = g.Min(o => o.depth)
                            }).ToList();
            }
        }

        /// <summary>
        /// 根据结构体和因素获取监测点分组 struct/{structId}/factor/{factorId}/groups
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <param name="factorId">因素编号</param>
        /// <returns></returns>

        public static List<Group> FindGroupsByStructAndFactor(int structId, int factorId)
        {
            using (var entities = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from s in entities.T_DIM_SENSOR
                            from f in entities.T_DIM_SAFETY_FACTOR_TYPE
                            from sg in entities.T_DIM_SENSOR_GROUP_CEXIE
                            from g in entities.T_DIM_GROUP
                            where s.STRUCT_ID == structId
                                  && s.SAFETY_FACTOR_TYPE_ID == factorId
                                  && sg.SENSOR_ID == s.SENSOR_ID
                                  && g.GROUP_ID == sg.GROUP_ID
                                  && s.IsDeleted == false
                            select new Group
                            {
                                GroupId = g.GROUP_ID,
                                GroupName = g.GROUP_NAME,
                                MaxDepth = sg.DEPTH
                            };
                return query.ToList();



            }
        }
        /// <summary>
        /// 获取沉降分组 3-11
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="factorId"></param>
        /// <returns></returns>
        public static List<Group> FindCjGroupsByStructAndFactor(int structId, int factorId)
        {
            using (var entities = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from s in entities.T_DIM_SENSOR
                            from f in entities.T_DIM_SAFETY_FACTOR_TYPE
                            from sg in entities.T_DIM_SENSOR_GROUP_CHENJIANG
                            from g in entities.T_DIM_GROUP
                            where s.STRUCT_ID == structId
                                  && s.SAFETY_FACTOR_TYPE_ID == factorId
                                  && sg.SENSOR_ID == s.SENSOR_ID
                                  && g.GROUP_ID == sg.GROUP_ID
                                  && s.IsDeleted == false
                                  
                            select new Group
                            {
                                GroupId = g.GROUP_ID,
                                GroupName = g.GROUP_NAME
                            };
                List<Group> list = query.GroupBy(g => new { g.GroupId, g.GroupName }).Select(g => new Group()
                {
                    GroupId = g.Key.GroupId,
                    GroupName = g.Key.GroupName
                }).ToList();
                   return list;
            }   
          }
        
        /// <summary>
        /// 获取沉降分组中的传感器
        /// </summary>
        /// <param name="groupId">分组编号</param>
        /// <returns>传感器列表</returns>

        public static List<Group> FindCjSensorsByGroup(int groupId)
        {
            using (var entity = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from sg in entity.T_DIM_SENSOR_GROUP_CHENJIANG
                            from s in entity.T_DIM_SENSOR
                            where sg.GROUP_ID == groupId && sg.SENSOR_ID == s.SENSOR_ID && s.IsDeleted == false
                            orderby sg.SENSOR_ID
                            select new Group
                            {
                                GroupId = s.SENSOR_ID,
                                GroupName = s.SENSOR_LOCATION_DESCRIPTION
                            };
                return query.ToList();
            }

        }
        /// <summary>
        /// 获取分组中的传感器
        /// </summary>
        /// <param name="groupId">分组编号</param>
        /// <returns>传感器列表</returns>

        public static List<Group> FindSensorsByGroup(int groupId)
        {
            using (var entity = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from sg in entity.T_DIM_SENSOR_GROUP_CEXIE
                            from s in entity.T_DIM_SENSOR
                            where sg.GROUP_ID == groupId && sg.SENSOR_ID == s.SENSOR_ID && s.IsDeleted == false
                            orderby sg.DEPTH
                            select new Group
                            {
                                GroupId = s.SENSOR_ID,
                                GroupName = s.SENSOR_LOCATION_DESCRIPTION,
                                MaxDepth = sg.DEPTH
                            };
                return query.ToList();
            }

        }

        /// <summary>
        /// 测斜累计变化趋势： 按照时间分组获取数据
        /// </summary>
        /// <param name="groupId">分组编号</param>
        /// <returns>测斜监测数据</returns>
        public static List<CxData> GetByGroupDirectAndDateGroupByTime(int groupId, DateTime startdate, DateTime enddate)
        {
            using (var entities = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from sg in entities.T_DIM_SENSOR_GROUP_CEXIE
                            join d in
                                (
                                    from data in entities.T_THEMES_DEFORMATION_DEEP_DISPLACEMENT_DAILY
                                    where data.ACQUISITION_DATETIME >= startdate && data.ACQUISITION_DATETIME <= enddate
                                    select data)
                            on sg.SENSOR_ID equals d.SENSOR_ID into data
                            from d in data.DefaultIfEmpty()
                            where sg.GROUP_ID == groupId
                            select new
                            {
                                depth = sg.DEPTH,
                                dataId = (int?)d.ID,
                                xvalue = d.DEEP_CUMULATIVEDISPLACEMENT_X_VALUE,
                                yvalue = d.DEEP_CUMULATIVEDISPLACEMENT_Y_VALUE,
                                acquistiontime = d.ACQUISITION_DATETIME
                            };
                var list = query.ToList();

                return
                    list.GroupBy(d => d.acquistiontime)
                        .OrderBy(d => d.Key)
                        .Select(
                            d =>
                            new CxData
                            {
                                DateTime = d.Key,
                                Data =
                                    d.Any() && d.First().dataId != null ?
                                    d.Select(
                                        g =>
                                        new CxInernalData
                                        {
                                            Depth = g.depth,
                                            XValue = g.xvalue,
                                            YValue = g.yvalue
                                        }).OrderBy(g => g.Depth).ToList()
                                        : new List<CxInernalData>()
                            })
                        .ToList();
            }
        }
        /// <summary>
        /// 测斜趋势图：按深度分组获取数据 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public static List<CxDataByDepth> GetByGroupDirectAndDateGroupByDepth(int groupId, DateTime startdate, DateTime enddate)
        {
            using (var entities = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from sg in entities.T_DIM_SENSOR_GROUP_CEXIE
                            join d in
                                (
                                    from data in entities.T_THEMES_DEFORMATION_DEEP_DISPLACEMENT_DAILY
                                    where data.ACQUISITION_DATETIME >= startdate
                                          && data.ACQUISITION_DATETIME <= enddate
                                    select data)
                                on sg.SENSOR_ID equals d.SENSOR_ID into data
                            from d in data.DefaultIfEmpty()
                            where sg.GROUP_ID == groupId
                            select new
                            {
                                Depth = sg.DEPTH,
                                dataId = (int?) d.ID,
                                Xvalue = d.DEEP_CUMULATIVEDISPLACEMENT_X_VALUE,
                                Yvalue = d.DEEP_CUMULATIVEDISPLACEMENT_Y_VALUE,
                                Acquisitiontime = d.ACQUISITION_DATETIME
                            };
             
                var list = query.ToList();

                return
                list.GroupBy(l => l.Depth)
                    .OrderBy(l => l.Key)
                    .Select(
                        l =>
                        new CxDataByDepth
                        {
                            Depth = l.Key,
                            Values =
                                l.Any() && l.First().dataId != null
                                ?
                                l.Select(
                                    g =>
                                    new CxDataWithTime()
                                    {
                                        Acquisitiontime = (DateTime)g.Acquisitiontime,
                                        Xvalue = g.Xvalue,
                                        Yvalue = g.Yvalue
                                    }).OrderBy(g => g.Acquisitiontime).ToList()
                                    : new List<CxDataWithTime>()
                        })
                    .ToList();
            }
        }
        /// <summary>
        /// 深度获取数据月 3-12
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="direct"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public static List<CxDataByDepth> GetByGroupDirectAndDateGroupByDepth(int groupId, string direct, DateTime startdate, DateTime enddate)
        {
            using (var entities = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from d in entities.T_THEMES_DEFORMATION_DEEP_DISPLACEMENT_DAILY
                            from sg in entities.T_DIM_SENSOR_GROUP_CEXIE
                            where d.SENSOR_ID == sg.SENSOR_ID
                                  && sg.GROUP_ID == groupId
                                  && d.ACQUISITION_DATETIME >= startdate && d.ACQUISITION_DATETIME <= enddate
                            select new
                            {
                                depth = sg.DEPTH,
                                xvalue = d.DEEP_DISPLACEMENT_X_VALUE,
                                yvalue = d.DEEP_DISPLACEMENT_Y_VALUE,
                                acquistiontime = d.ACQUISITION_DATETIME
                            };
                var list = query.ToList();
                return list.GroupBy(l => l.depth).OrderBy(l => l.Key).Select(
                    l =>
                    new CxDataByDepth()
                    {
                        Depth = l.Key,
                        Values = l.OrderBy(v => v.acquistiontime).Select(v =>
                                                                         new CxDataWithTime()
                                                                         {
                                                                             Acquisitiontime =
                                                                                 (DateTime) v.acquistiontime,
                                                                             Xvalue = v.xvalue,
                                                                             Yvalue = v.yvalue
                                                                         }
                        ).ToList()
                    }
                    ).ToList();
            }
        }
        /// <summary>
        /// 深度获取数据月 3-12
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="direct"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public static List<CxDataByDepth> GetByGroupDirectAndDateGroupByDepthW(int groupId, string direct, DateTime startdate, DateTime enddate)
        {
            using (var entities = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from d in entities.T_THEMES_DEFORMATION_DEEP_DISPLACEMENT
                            from sg in entities.T_DIM_SENSOR_GROUP_CEXIE
                            where d.SENSOR_ID == sg.SENSOR_ID
                                  && sg.GROUP_ID == groupId
                                  && d.ACQUISITION_DATETIME >= startdate && d.ACQUISITION_DATETIME <= enddate
                            select new
                            {
                                depth = sg.DEPTH,
                                xvalue = d.DEEP_DISPLACEMENT_X_VALUE,
                                yvalue = d.DEEP_DISPLACEMENT_Y_VALUE,
                                acquistiontime = d.ACQUISITION_DATETIME
                            };
                var list = query.ToList();
                return list.GroupBy(l => l.depth).OrderBy(l => l.Key).Select(
                    l =>
                    new CxDataByDepth()
                    {
                        Depth = l.Key,
                        Values = l.OrderBy(v => v.acquistiontime).Select(v =>
                                                                         new CxDataWithTime()
                                                                         {
                                                                             Acquisitiontime =
                                                                                 (DateTime)v.acquistiontime,
                                                                             Xvalue = v.xvalue,
                                                                             Yvalue = v.yvalue
                                                                         }
                        ).ToList()
                    }
                    ).ToList();
            }
        }
        /// <summary>
        /// 通用数据提取
        /// </summary>
        /// <param name="sensors"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="bl">最大or最小</param>
        /// <returns></returns>
        public static Dictionary<string, MonitorData> GetUnifyData(int[] sensors, DateTime start, DateTime end, bool bl)
        {
            List<SensorData> list;
            if (bl)
            {
                list = GetMaxSensorData(sensors, start, end);
            }
            else
            {
                list = GetMinSensorData(sensors, start, end);
            }

            List<MonitorData> tpm = list.OrderBy(l => l.SensorId)
               .GroupBy(s => new { s.SensorId, s.Location, s.Columns, s.Unit })
                 .Select(
                  d =>
                      new MonitorData
                      {
                          SensorId = d.Key.SensorId,
                          Location = d.Key.Location,
                          Columns = d.Key.Columns.ToList(),
                          Unit = d.Key.Unit.ToList(),
                          Data =
                          d.Select(
                          g =>
                              new Data
                              {
                                  Values = g.Values.ToList(),
                                  AcquisitionTime = g.AcquisitionTime
                              }).ToList()
                      }

                 ).ToList();
            Dictionary<string, MonitorData> dt = tpm.ToDictionary(p => p.Location, q => q);
            return dt;
        }

        public static List<SensorData> GetMinSensorData(int[] sensors, DateTime start, DateTime end)
        {

            var data = new List<SensorData>();
            foreach (int sensorId in sensors)
            {
                // 查询传感器监测类型
                int? factorId = null;
                factorId = GetFactorId(sensorId);
                if (factorId == null)
                {
                    break;
                }

                // 读取配置文件
                IEnumerable<Factor> config = GetConfigByFactors(new int[] { Convert.ToInt32(factorId) });

                // 从配置中查找列
                var factorConfig = config.FirstOrDefault(c => c.Id == factorId);
                if (factorConfig == null)
                {
                    throw new ConfigurationErrorsException(string.Format("缺少 factorid:{0} 的配置数据", factorId));
                }

                // 构造sql语句
                var colums = new string[factorConfig.DisplayNumber];
                for (int i = 0; i < factorConfig.DisplayNumber; i++)
                {
                    colums[i] = string.Format("ROUND(t.{0},{1}) as {0}", factorConfig.Columns[i], factorConfig.DecimalPlaces[i]);
                }

                string values = string.Join(",", colums);

                var abs = new string[factorConfig.DisplayNumber];
                for (int i = 0; i < factorConfig.DisplayNumber; i++)
                {
                    abs[i] = string.Format("abs({0})", factorConfig.Columns[i]);
                }

                string absStr = string.Join(",", abs);

                string sqlStr = string.Format(@"SELECT s.SENSOR_ID,t.ACQUISITION_DATETIME,s.SENSOR_LOCATION_DESCRIPTION,{0}
                                                FROM {1} t
                                                join T_DIM_SENSOR s on t.SENSOR_ID=s.SENSOR_ID
                                                where t.ID in(select MIN(ID) from {2} m where m.SENSOR_ID = {3}
                                                and m.ACQUISITION_DATETIME between '{4}' and '{5}'
                                                group by m.SENSOR_ID)
                                                ",
                    values,
                    factorConfig.Table,
                    factorConfig.Table,
                    sensorId,
                    start,
                    end);



                // 读取数据
                GetValueFromDb(sqlStr, data, factorConfig);
            }
            return data;
        }

        public static List<SensorData> GetMaxSensorData(int[] sensors, DateTime start, DateTime end)
        {

            var data = new List<SensorData>();
            foreach (int sensorId in sensors)
            {
                // 查询传感器监测类型
                int? factorId = null;
                factorId = GetFactorId(sensorId);
                if (factorId == null)
                {
                    break;
                }

                // 读取配置文件
                IEnumerable<Factor> config = GetConfigByFactors(new int[] { Convert.ToInt32(factorId) });

                // 从配置中查找列
                var factorConfig = config.FirstOrDefault(c => c.Id == factorId);
                if (factorConfig == null)
                {
                    throw new ConfigurationErrorsException(string.Format("缺少 factorid:{0} 的配置数据", factorId));
                }

                // 构造sql语句
                var colums = new string[factorConfig.DisplayNumber];
                for (int i = 0; i < factorConfig.DisplayNumber; i++)
                {
                    colums[i] = string.Format("ROUND(t.{0},{1}) as {0}", factorConfig.Columns[i], factorConfig.DecimalPlaces[i]);
                }

                string values = string.Join(",", colums);

                var abs = new string[factorConfig.DisplayNumber];
                for (int i = 0; i < factorConfig.DisplayNumber; i++)
                {
                    abs[i] = string.Format("abs({0})", factorConfig.Columns[i]);
                }

                string absStr = string.Join(",", abs);

                string sqlStr = string.Format(@"SELECT s.SENSOR_ID,t.ACQUISITION_DATETIME,s.SENSOR_LOCATION_DESCRIPTION,{0}
                                                FROM {1} t
                                                join T_DIM_SENSOR s on t.SENSOR_ID=s.SENSOR_ID
                                                where t.ID in(select MAX(ID) from {2} m where m.SENSOR_ID = {3}
                                                and m.ACQUISITION_DATETIME between '{4}' and '{5}'
                                                group by m.SENSOR_ID)
                                                ",
                    values,
                    factorConfig.Table,
                    factorConfig.Table,
                    sensorId,
                    start,
                    end);



                // 读取数据
                GetValueFromDb(sqlStr, data, factorConfig);
            }
            return data;
        }



        /// <summary>
        /// 测斜数据
        /// </summary>
        /// <param name="groupId">传感器组编号（只能是数字）</param>        
        /// <param name="startdate">开始时间（ISO时间）</param>
        /// <param name="enddate">结束时间（ISO时间）</param>
        /// <param name="bl"></param>
        /// <returns>深部位移数据</returns>
        public static List<CxInernalData> GetByGroupAndDateGroupByTime(int groupId, DateTime startdate, DateTime enddate, bool bl)
        {
            List<CxInernalData> list = new List<CxInernalData>();
            DataSet ds;
            if (bl)
            {
                ds = SqlHelper.ExecuteDataSetText("select * from dbo.T_THEMES_DEFORMATION_DEEP_DISPLACEMENT_DAILY a,dbo.T_DIM_SENSOR_GROUP_CEXIE b where a.SENSOR_ID=b.SENSOR_ID and a.ID in (select MAX(ID) from dbo.T_THEMES_DEFORMATION_DEEP_DISPLACEMENT_DAILY m,dbo.T_DIM_SENSOR_GROUP_CEXIE n where m.SENSOR_ID=n.SENSOR_ID and n.GROUP_ID=@a and m.ACQUISITION_DATETIME>=@b and m.ACQUISITION_DATETIME<@c group by m.SENSOR_ID)", new SqlParameter[] { new SqlParameter("@a", groupId), new SqlParameter("@b", startdate), new SqlParameter("@c", enddate) });
            }
            else
            {
                ds = SqlHelper.ExecuteDataSetText("select * from dbo.T_THEMES_DEFORMATION_DEEP_DISPLACEMENT_DAILY a,dbo.T_DIM_SENSOR_GROUP_CEXIE b where a.SENSOR_ID=b.SENSOR_ID and a.ID in (select MIN(ID) from dbo.T_THEMES_DEFORMATION_DEEP_DISPLACEMENT_DAILY m,dbo.T_DIM_SENSOR_GROUP_CEXIE n where m.SENSOR_ID=n.SENSOR_ID and n.GROUP_ID=@a and m.ACQUISITION_DATETIME>=@b and m.ACQUISITION_DATETIME<@c group by m.SENSOR_ID)", new SqlParameter[] { new SqlParameter("@a", groupId), new SqlParameter("@b", startdate), new SqlParameter("@c", enddate) });

            }
            if (ds.Tables.Count != 0)
            {
                foreach (var row in ds.Tables[0].AsEnumerable())
                {
                    list.Add(new CxInernalData()
                    {
                        Depth = Convert.ToDecimal(row["DEPTH"]),
                        XValue = Convert.ToDecimal(row["DEEP_CUMULATIVEDISPLACEMENT_X_VALUE"]),
                        YValue = Convert.ToDecimal(row["DEEP_CUMULATIVEDISPLACEMENT_Y_VALUE"])
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 获取设备分组
        /// </summary>
        /// <param name="stcId"></param>
        /// <param name="fctId"></param>
        /// <returns></returns>
        public static List<CxGroup> GetGroupByStuAndFac(int stcId, int fctId)
        {
            var groups = new List<CxGroup>();
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {

                groups = (from s in db.T_DIM_SENSOR
                          from sg in db.T_DIM_SENSOR_GROUP_CEXIE
                          from g in db.T_DIM_GROUP
                          where
                              s.SENSOR_ID == sg.SENSOR_ID && sg.GROUP_ID == g.GROUP_ID && s.STRUCT_ID == stcId
                              && s.SAFETY_FACTOR_TYPE_ID == fctId
                          select new { g.GROUP_ID, g.GROUP_NAME, sg.DEPTH }).ToList()
                    .GroupBy(g => new { g.GROUP_ID, g.GROUP_NAME })
                    .Select(
                        s =>
                        new CxGroup
                        {
                            Id = s.Key.GROUP_ID,
                            Name = s.Key.GROUP_NAME,
                            Depth = s.Select(d => d.DEPTH * -1).OrderBy(d => d).ToArray()
                        })
                    .ToList();
            }
            return groups;
        }

        /// <summary>
        /// 获取传感器设备信息
        /// </summary>
        /// <param name="StructId"></param>
        /// <param name="FactorId"></param>
        /// <returns></returns>
        public static SensorProductInfo GetProductInfo(int StructId, int FactorId)
        {
            SensorProductInfo Product = new SensorProductInfo();
            var ds = SqlHelper.ExecuteDataSetText("select * from dbo.T_DIM_SENSOR_PRODUCT where PRODUCT_ID in(select distinct PRODUCT_SENSOR_ID from dbo.T_DIM_SENSOR where IsDeleted=0 and SAFETY_FACTOR_TYPE_ID=@a and STRUCT_ID=@b)", new SqlParameter[] { new SqlParameter("@a", FactorId), new SqlParameter("@b", StructId) });
            if (ds.Tables.Count != 0)
            {
                foreach (var item in ds.Tables[0].AsEnumerable())
                {
                    Product.ProductId = Convert.ToInt32(item["PRODUCT_ID"]);
                    Product.ProductTypeId = Convert.ToInt32(item["PRODUCT_TYPE_ID"]);
                    Product.ProductName = Convert.ToString(item["PRODUCT_NAME"]);
                    Product.ProductCode = Convert.ToString(item["PRODUCT_CODE"]);
                }
            }
            return Product;

        }

        /// <summary>
        /// 获取截面下的传感器
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="factorId"></param>
        /// <returns></returns>
        public static List<SectionSensors> GetSectionSensors(int structId, int factorId)
        {
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from s in db.T_DIM_STRUCTURE
                            join ss in db.T_DIM_SECTION on s.ID equals ss.StructId into sss

                            from t in sss.DefaultIfEmpty()
                            join tt in db.T_DIM_HOTSPOT on t.SectionId equals tt.SECTION_ID into ttt

                            from a in ttt.DefaultIfEmpty()
                            join aa in db.T_DIM_SENSOR on a.SENSOR_ID equals aa.SENSOR_ID into aaa
                            from b in aaa.DefaultIfEmpty()
                            where s.ID == structId && b.SAFETY_FACTOR_TYPE_ID == factorId
                                  && s.IsDelete == 0 && b.IsDeleted == false
                                  && b.Identification != 1
                            select new
                            {
                                sectionId = t.SectionId,
                                sectionName = t.SectionName,
                                sensorId = b.SENSOR_ID,
                                sensorLocation = b.SENSOR_LOCATION_DESCRIPTION
                            };


                var list = query.ToList();
                return
                    list
                        .GroupBy(g => new { g.sectionId, g.sectionName })
                        .Select(
                            o =>
                                new SectionSensors
                                {
                                    SectionId = o.Key.sectionId,
                                    SectionName = o.Key.sectionName,
                                    Sensors = o.Select(
                                    d =>
                                        new Sensor
                                        {
                                            SensorId = d.sensorId,
                                            Location = d.sensorLocation
                                        }
                                    ).ToList()
                                }
                        ).OrderBy(l => l.SectionId).ToList();
            }
        }


        /// <summary>
        /// 获取处于施工状态中的截面下的传感器
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="factorId"></param>
        /// <param name="flag">传感器标识: 组合2   数据1  实体0</param>
        /// <param name="status">施工状态: 0：未施工，1：施工中，2：施工完成</param>
        /// <returns></returns>

        public static List<SectionSensors> GetProcessingSensor(int structId, int factorId, int flag, int status)
        {
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {
                var query = from s in db.T_DIM_STRUCTURE
                            join ss in db.T_DIM_SECTION on s.ID equals ss.StructId into sss

                            from t in sss.DefaultIfEmpty()
                            join tt in db.T_DIM_HOTSPOT on t.SectionId equals tt.SECTION_ID into ttt

                            from a in ttt.DefaultIfEmpty()
                            join aa in db.T_DIM_SENSOR on a.SENSOR_ID equals aa.SENSOR_ID into aaa
                            from b in aaa.DefaultIfEmpty()
                            where s.ID == structId && b.SAFETY_FACTOR_TYPE_ID == factorId
                                  && s.IsDelete == 0 && b.IsDeleted == false
                                  && b.Identification == flag && t.SectionStatus == status
                            select new
                            {
                                sectionId = t.SectionId,
                                sectionName = t.SectionName,
                                sensorId = b.SENSOR_ID,
                                sensorLocation = b.SENSOR_LOCATION_DESCRIPTION
                            };


                var list = query.ToList();
                return
                    list
                        .GroupBy(g => new { g.sectionId, g.sectionName })
                        .Select(
                            o =>
                                new SectionSensors
                                {
                                    SectionId = o.Key.sectionId,
                                    SectionName = o.Key.sectionName,
                                    Sensors = o.Select(
                                    d =>
                                        new Sensor
                                        {
                                            SensorId = d.sensorId,
                                            Location = d.sensorLocation
                                        }
                                    ).ToList()
                                }
                        ).OrderBy(l => l.SectionId).ToList();
            }
        }

        /// <summary>
        /// 获取测点的初始值
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public static InitValues GetSensorInitValue(int sensorId)
        {
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {
                return (from s in db.T_DIM_FORMULAID_SET
                        where s.SENSOR_ID == sensorId
                        select new InitValues
                        {
                            len0 = s.Parameter1,
                            h0 = s.Parameter2
                        }).FirstOrDefault();

            }
        }
     
        /// <summary>
        /// 获取起始时间段内监测数据的平均值
        /// </summary>
        /// <param name="sensors"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Dictionary<string, MonitorData> GetAvgData(int[] sensors, DateTime start, DateTime end)
        {
            List<SensorData> list = GetGeneralSensorData(sensors, start, end);
            List<MonitorData> tpm = list.OrderBy(l => l.SensorId)
               .GroupBy(s => new { s.SensorId, s.Location, s.Columns, s.Unit })
                 .Select(
                  d =>
                      new MonitorData
                      {
                          SensorId = d.Key.SensorId,
                          Location = d.Key.Location,
                          Columns = d.Key.Columns.ToList(),
                          Unit = d.Key.Unit.ToList(),
                          Data =
                          d.Select(
                          g =>
                              new Data
                              {
                                  Values = g.Values.ToList(),
                                  AcquisitionTime = g.AcquisitionTime
                              }).ToList()
                      }

                 ).ToList();

            if (tpm.Any())
            {
                foreach (var monitorData in tpm)
                {
                    var data = monitorData.Data;
                    if (data.Any())
                    {
                        decimal sum = data.Sum(d => d.Values[0]);
                        decimal avg = sum / data.Count;
                        data[0].Values[0] = avg;
                        data[0].AcquisitionTime = data[0].AcquisitionTime.Date;
                    }
                }
                Dictionary<string, MonitorData> dt = tpm.ToDictionary(p => p.Location, q => q);
                return dt;
            }
           return new Dictionary<string, MonitorData>();
        }

        /// <summary>
        /// 获得通用数据(小数位不进行取舍)
        /// </summary>
        /// <param name="sensors"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<SensorData> GetGeneralSensorData(int[] sensors, DateTime start, DateTime end)
        {

            var data = new List<SensorData>();
            foreach (int sensorId in sensors)
            {
                // 查询传感器监测类型
                int? factorId = null;
                factorId = GetFactorId(sensorId);
                if (factorId == null)
                {
                    break;
                }

                // 读取配置文件
                IEnumerable<Factor> config = GetConfigByFactors(new int[] { Convert.ToInt32(factorId) });

                // 从配置中查找列
                var factorConfig = config.FirstOrDefault(c => c.Id == factorId);
                if (factorConfig == null)
                {
                    throw new ConfigurationErrorsException(string.Format("缺少 factorid:{0} 的配置数据", factorId));
                }

                // 构造sql语句
                var colums = new string[factorConfig.DisplayNumber];
                for (int i = 0; i < factorConfig.DisplayNumber; i++)
                {
                    //colums[i] = string.Format("ROUND(t.{0},{1}) as {0}", factorConfig.Columns[i], factorConfig.DecimalPlaces[i]);
                    colums[i] =  factorConfig.Columns[i];
                    
                }

                string values = string.Join(",", colums);

                var abs = new string[factorConfig.DisplayNumber];
                for (int i = 0; i < factorConfig.DisplayNumber; i++)
                {
                    abs[i] = string.Format("abs({0})", factorConfig.Columns[i]);
                }
                string sqlStr = string.Format(
                      @"SELECT s.SENSOR_ID,t.ACQUISITION_DATETIME,s.SENSOR_LOCATION_DESCRIPTION,{0}
                                                                                FROM {1} t
                                                                                join T_DIM_SENSOR s on t.SENSOR_ID=s.SENSOR_ID
                                                                                where t.SENSOR_ID = {2}
                                                                                and t.ACQUISITION_DATETIME between '{3}' and '{4}'
                                                                                order by t.ACQUISITION_DATETIME",
                    values,
                    factorConfig.Table,
                    sensorId,
                    start,
                    end);
               
                // 读取数据
                GetOriginalValueFromDb(sqlStr, data, factorConfig);
            }
            return data;
        }
/// <summary>
///  获得数据库里的原始数据,小数精度和数据库一致,小数位不进行取舍
/// </summary>
/// <param name="sqlStr"></param>
/// <param name="data"></param>
/// <param name="config"></param>
        public static void GetOriginalValueFromDb(string sqlStr, IList<SensorData> data, Factor config)
        {
            SensorData temp;

            DataSet ds = SqlHelper.ExecuteDataSetText(sqlStr, null);
            if (ds == null)
            {
                return;
            }

            // ds里面的表存储的是sqlstr查询的结果,按照sqlstr查询语句中字段顺序来确定缓存结果中的表的个字段

            if (ds.Tables.Count != 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    temp = new SensorData();
                    temp.SensorId = Convert.ToInt32(row[0]);
                    temp.AcquisitionTime = Convert.ToDateTime(row[1]);
                    temp.Location = Convert.ToString(row[2]);

                    temp.Values = new decimal[row.ItemArray.Length - 3];
                    for (int i = 3; i < row.ItemArray.Length; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            temp.Values[i - 3] =
                                // Convert.ToDecimal(Convert.ToDecimal(row[i]).ToString("f" + config.DecimalPlaces[i - 3]));
                              Convert.ToDecimal(row[i]);

                        }
                    }

                    temp.Columns = config.Display;
                    temp.Unit = config.Unit;
                    data.Add(temp);
                }
            }
        }

        public static decimal GetSensorParm(int SensorId)
        {
            decimal result;
            using (var db = new DW_iSecureCloud_EmptyEntities())
            {
                List<decimal?> query = (from s in db.T_DIM_FORMULAID_SET
                                        where s.SENSOR_ID == SensorId
                                        select s.Parameter1).ToList();
                result = decimal.TryParse(query[0].ToString(), out result) ? result : 0;
            }
            return result;
        }

        public static decimal? GetWaterLevelInit(int sensorId)
        {
            decimal? avg = null;
            string cmdStr =
                "select AVG(WATER_LEVEL_CUMULATIVEVALUE) as avg from dbo.T_THEMES_ENVI_WATER_LEVEL where WATER_LEVEL_CUMULATIVEVALUE in (SELECT TOP 5  WATER_LEVEL_CUMULATIVEVALUE FROM dbo.T_THEMES_ENVI_WATER_LEVEL where SENSOR_ID = @a order by ACQUISITION_DATETIME asc)";
            var ds = SqlHelper.ExecuteDataSetText(cmdStr, new SqlParameter[] { new SqlParameter("@a", sensorId) });
            if (ds.Tables.Count != 0)
            {
                
                avg = Convert.ToDecimal(ds.Tables[0].Rows[0][0]);
            }
            return avg;
        }
        


    }

}
