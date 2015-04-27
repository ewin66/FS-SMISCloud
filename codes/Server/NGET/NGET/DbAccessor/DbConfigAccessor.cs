// --------------------------------------------------------------------------------------------
// <copyright file="SensorDbConfig.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2015 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20150329
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.NGET.DbAccessor
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using FS.DbHelper;
    using FS.SMIS_Cloud.NGET.Model;

    using DbType = FS.DbHelper.DbType;

    public class DbConfigAccessor
    {
        private static ISqlHelper _sqlHelper;

        static DbConfigAccessor()
        {
            _sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, GlobalConfig.ConnectionString);
        }

        public static Sensor GetSensorInfo(uint sensorId)
        {
            string sql = string.Format(@"
SELECT 
	s.STRUCT_ID structId,
    s.SENSOR_ID sid,
    s.SENSOR_LOCATION_DESCRIPTION name,
    s.SAFETY_FACTOR_TYPE_ID factor,
    factor.THEMES_TABLE_NAME tablename,
    factor.THEMES_COLUMNS columns,
    s.Identification sensortype,
    s.Enable enable,
    product.PRODUCT_ID pid,
    product.PRODUCT_CODE pcode,
    protocol.PROTOCOL_CODE protocol,
    product.FORMAULAID formulaid
FROM T_DIM_SENSOR as s
join T_DIM_SENSOR_PRODUCT product on s.PRODUCT_SENSOR_ID=product.PRODUCT_ID
left join T_DIM_PROTOCOL_TYPE protocol on product.PROTOCOL_ID=protocol.PROTOCOL_ID
join T_DIM_SAFETY_FACTOR_TYPE factor on s.SAFETY_FACTOR_TYPE_ID=factor.SAFETY_FACTOR_TYPE_ID
WHERE s.SENSOR_ID = {0}
", sensorId);

            try
            {
                var dt = _sqlHelper.Query(sql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("sensor: [" + sensorId + "] config is incomplete");
                }

                Sensor sensor = new Sensor()
                                    {
                                        StructId = Convert.ToUInt32(dt.Rows[0]["structId"]),
                                        SensorID = Convert.ToUInt32(dt.Rows[0]["sid"]),
                                        Name = dt.Rows[0]["name"].ToString(),
                                        FactorType = Convert.ToUInt32(dt.Rows[0]["factor"]),
                                        FactorTypeTable = dt.Rows[0]["tablename"].ToString(),
                                        TableColums = dt.Rows[0]["columns"].ToString(),
                                        SensorType = (SensorType)dt.Rows[0]["sensortype"],
                                        Enabled = !Convert.ToBoolean(dt.Rows[0]["enable"]),
                                        ProductId = Convert.ToInt32(dt.Rows[0]["pid"]),
                                        ProductCode = dt.Rows[0]["pcode"].ToString(),
                                        ProtocolType =
                                            DBNull.Value.Equals(dt.Rows[0]["protocol"])
                                                ? 0
                                                : Convert.ToUInt32(dt.Rows[0]["protocol"]),
                                        FormulaID =
                                            DBNull.Value.Equals(dt.Rows[0]["formulaid"])
                                                ? 0
                                                : Convert.ToUInt32(dt.Rows[0]["formulaid"])
                                    };

                if (sensor.FormulaID != 0)
                {
                    foreach (SensorParam param in GetSensorParam(sensor))
                    {
                        sensor.AddParameter(param);
                    }
                }

                return sensor;
            }
            catch (SqlException e)
            {
                throw new Exception("sql exception when querying sensor: [" + sensorId + "] info", e);
            }
            catch (Exception e)
            {
                throw new Exception("query sensor: [" + sensorId + "] config info error", e);
            }
        }

        public static List<SensorParam> GetSensorParam(Sensor sensor)
        {
            var param = new List<SensorParam>();

            // 查询公式信息
            string sql = string.Format(@"
SELECT
	  P.FormulaID fid,
      P.[Order],
	  P.FormulaParaID pid,
      N.ParaName name, N.ParaAlias alias
  FROM  T_DIM_FORMULA_PARA P
  JOIN T_DIM_FORMULA_PARA_NAME N ON P.ParaNameID = N.ParaNameID
  where P.FormulaID={0}
  ORDER BY [Order]
", sensor.FormulaID);
            var dic = new Dictionary<int, FormulaParam>();
            foreach (DataRow row in _sqlHelper.Query(sql).Tables[0].Rows)
            {
                dic.Add(
                    Convert.ToInt32(row["pid"]),
                    new FormulaParam
                        {
                            FID = Convert.ToInt32(row["fid"]),
                            PID = Convert.ToInt32(row["pid"]),
                            Index = Convert.ToInt32(row["Order"]),
                            Name = row["name"].ToString(),
                            Alias = row["alias"].ToString()
                        });
            }

            // 查询公式配置
            string sql2 = string.Format(@"
SELECT * 
FROM T_DIM_FORMULAID_SET
WHERE SENSOR_ID={0}
", sensor.SensorID);

            int i = 1;

            var dt = _sqlHelper.Query(sql2).Tables[0];
            if (dt.Rows.Count == 0)
            {
                foreach (var formulaParam in dic)
                {
                    param.Add(new SensorParam(formulaParam.Value) { Value = 0 });
                }
            }
            else
            {
                var fpSet = dt.Rows[0];
                foreach (var formulaParam in dic)
                {
                    string value = string.Format("Parameter{0}", i);
                    var fpValue = DBNull.Value.Equals(fpSet[value]) ? 0 : Convert.ToDouble(fpSet[value]);

                    param.Add(new SensorParam(formulaParam.Value) { Value = fpValue });

                    i++;
                }
            }

            return param;
        }
    }
}