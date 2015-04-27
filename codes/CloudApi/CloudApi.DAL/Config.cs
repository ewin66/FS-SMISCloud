namespace FreeSun.FS_SMISCloud.Server.CloudApi.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Entity;

    public class Config
    {
        public static IEnumerable<FactorConfig> GetConfigByFactors(int[] factors, int structId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                try
                {
                    return entity.T_DIM_SAFETY_FACTOR_TYPE
                        .Where(f => factors.Contains(f.SAFETY_FACTOR_TYPE_ID))
                        .ToList()
                        .Select(f => new FactorConfig
                        {
                            Id = f.SAFETY_FACTOR_TYPE_ID,
                            NameCN = f.SAFETY_FACTOR_TYPE_NAME,
                            NameEN = f.SAFETY_FACTOR_TYPE_NAME_EN,
                            DisplayNumber = f.FACTOR_VALUE_COLUMN_NUMBER ?? 1,
                            Columns = f.THEMES_COLUMNS.Split(','),
                            Table = f.THEMES_TABLE_NAME,
                            Display = f.FACTOR_VALUE_COLUMNS.Split(','),
                            //Unit = f.FACTOR_VALUE_UNIT.Split(','),
                            Unit = GetStructConfigUnit(structId, f.SAFETY_FACTOR_TYPE_ID),

                            DecimalPlaces =
                                f.FACTOR_VALUE_DECIMAL_PLACES.Split(',').Select(d => Convert.ToInt32(d)).ToArray()
                        });
                }
                catch (NullReferenceException ex)
                {
                    throw new ConfigurationErrorsException("配置信息不完整");
                }
            }
        }
        //根据结构物编号和监测因素编号
        public static string[] GetStructConfigUnit(int structId, int factorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var unitList = from q in db.T_DIM_STRUCT_FACTOR_UNIT
                               where q.STRUCT_ID == structId && q.FACTOR_ID == factorId
                               select new
                               {
                                   unitId = q.SUB_FACTOR_ID
                               };
                var query = from a in unitList
                            from b in db.T_DIM_FACTOR_UNIT_INT
                            where a.unitId == b.ID
                            select new
                            {
                                unit = b.UNIT
                            };
                var unitString = query.ToList();

                return unitString.Select(m => (m.unit)).ToArray();//按照索引获得1-2-3-4
            }
        }

        //根据传感器编号和监测因素编号
        public static string[] GetUnitByFactorId(int sensorId, int factId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var sensor = from s in db.T_DIM_SENSOR
                             where s.SENSOR_ID == sensorId
                             select new
                             {
                                 structId = s.STRUCT_ID
                             };

                var unitList = from q in db.T_DIM_STRUCT_FACTOR_UNIT
                               from a1 in sensor
                               where q.STRUCT_ID == a1.structId && q.FACTOR_ID == factId
                               select new
                               {
                                   unitId = q.SUB_FACTOR_ID
                               };
                var query = from a in unitList
                            from b in db.T_DIM_FACTOR_UNIT_INT
                            where a.unitId == b.ID
                            select new
                            {
                                unit = b.UNIT
                            };
                var unitString = query.ToList();

                return unitString.Select(m => (m.unit)).ToArray();////按照索引获得1-2-3-4
            }
        }


        //根据传感器Id
        public static string[] GetUnitBySensorID(int sensorId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var sensor = from s in db.T_DIM_SENSOR
                             where s.SENSOR_ID == sensorId
                             select new
                             {
                                 structId = s.STRUCT_ID,
                                 factorId = s.SAFETY_FACTOR_TYPE_ID
                             };

                var unitList = from q in db.T_DIM_STRUCT_FACTOR_UNIT
                               from a1 in sensor
                               where q.STRUCT_ID == a1.structId && q.FACTOR_ID == a1.factorId
                               select new
                               {
                                   unitId = q.SUB_FACTOR_ID
                               };
                var query = from a in unitList
                            from b in db.T_DIM_FACTOR_UNIT_INT
                            where a.unitId == b.ID
                            select new
                            {
                                unit = b.UNIT
                            };
                var unitString = query.ToList();

                return unitString.Select(m => (m.unit)).ToArray();////按照索引获得1-2-3-4
            }

        }

        //新增获取结构物Id
        public static int GetStructId(int sensorId)
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

        public static IEnumerable<OriginalConfig> GetConfigByProduct(int productId)
        {
            using (SecureCloud_Entities entity = new SecureCloud_Entities())
            {
                try
                {
                    return entity.T_DIM_PRODUCTCATAGORY_ORIGINALDATA
                        .Where(w => w.ProductCatagoryId == productId)
                        .ToList()
                        .Select(s => new OriginalConfig
                        {
                            ProductId = s.ProductCatagoryId,
                            Table = s.OriginalTableName,
                            DisplayNumber = s.ColumnNumber ?? 1,
                            Display = s.ValueColumnName.Split(','),
                            Columns = s.ValueColumn.Split(','),
                            DecimalDigits = s.ValueDecimalDigits.Split(',').Select(d => Convert.ToInt32(d)).ToArray(),
                            Unit = s.ValueUnit.Split(',')
                        });
                }
                catch (NullReferenceException ex)
                {
                    throw new ConfigurationErrorsException("原始配置信息不完整");
                }
            }
        }


    }
}