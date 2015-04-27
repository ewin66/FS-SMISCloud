//  --------------------------------------------------------------------------------------------
//  <copyright file="DeviceInfoDal.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2013 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：20131223
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataCenter.Model;

namespace DataCenter.Accessor.ViewDal
{
    /// <summary>
    /// The device info dal.
    /// </summary>
    public class DeviceInfoDal
    {
        /// <summary>
        /// 梁段挠度
        /// </summary>
        private const int BridgeDeflection = 21;

        private const int BridgeSettleGps = 19;

        /// <summary>
        /// The get structure sensor device info.
        /// </summary>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable GetPressStructureSensorDeviceInfo()
        {
            const string Sqlstr = "SELECT DISTINCT [STRUCT_ID],[SENSOR_ID],[SAFETY_FACTOR_TYPE_ID],[MODULE_NO],[DAI_CHANNEL_NUMBER],[Parameter1] from [V_DEVICE_INFO]  where SAFETY_FACTOR_TYPE_ID=@SAFETY_FACTOR_TYPE_ID and STRUCT_ID is not null"
                                  + " and SENSOR_ID is not null and SAFETY_FACTOR_TYPE_ID is not null and MODULE_NO is not null";
            var parms = new[] { new SqlParameter("@SAFETY_FACTOR_TYPE_ID", BridgeDeflection) }; // 梁段挠度
            using (DataSet dst = DbHelperSQL.Query(Sqlstr, parms))
            {
                return dst.Tables[0];
            }
        }

        /// <summary>
        /// The get pressure data by sensor.
        /// </summary>
        /// <param name="sensorId">
        /// The sensor id.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable GetPressureDataBySensor(int sensorId, int count)
        {
            string sqlstr =
                string.Format(
                    "SELECT TOP {0} [ID],[STRUCTURE_ID],[SAFETY_FACTOR_TYPE_ID],[SENSOR_ID],[ACQUISITION_DATETIME],[MODULE_NO],[CHANNEL_NUMBER],[CollectOriginalValue1] from [T_COL_ORIGINAL_DATAVALUE] where SENSOR_ID=@SENSOR_ID order by ACQUISITION_DATETIME ",
                    count);
            var parms = new[] { new SqlParameter("@SENSOR_ID", sensorId) };
            using (DataSet dst = DbHelperSQL.Query(sqlstr, parms))
            {
                return dst.Tables[0];
            }
        }

        /// <summary>
        /// The get gps structure sensor device info.
        /// </summary>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable GetGpsStructureSensorDeviceInfo()
        {
            var sqlstr =
                "SELECT DISTINCT [STRUCT_ID],[SENSOR_ID],[SAFETY_FACTOR_TYPE_ID],[MODULE_NO],[DAI_CHANNEL_NUMBER],[Parameter1],[Parameter2],[Parameter3] from [V_DEVICE_INFO]  where SAFETY_FACTOR_TYPE_ID=@SAFETY_FACTOR_TYPE_ID and STRUCT_ID is not null"
                + " and SENSOR_ID is not null and SAFETY_FACTOR_TYPE_ID is not null and MODULE_NO is not null";
            var parms = new[] { new SqlParameter("@SAFETY_FACTOR_TYPE_ID", BridgeSettleGps) }; // 桥墩沉降
            using (DataSet dst = DbHelperSQL.Query(sqlstr, parms))
            {
                return dst.Tables[0];
            }
        }

        public static IList<SensorInfo> GetAllDeviceInfo()
        {
            var sqlstr = @"
SELECT     dbo.T_DIM_SENSOR.STRUCT_ID, dbo.T_DIM_REMOTE_DTU.REMOTE_DTU_NUMBER, dbo.T_DIM_REMOTE_DTU.REMOTE_DTU_GRANULARITY,dbo.T_DIM_SENSOR.MODULE_NO, dbo.T_DIM_SENSOR.SENSOR_ID,dbo.T_DIM_SENSOR.DAI_CHANNEL_NUMBER, 
            Para1.ParaName AS ParaName1, dbo.T_DIM_FORMULAID_SET.Parameter1, Para2.ParaName AS ParaName2, dbo.T_DIM_FORMULAID_SET.Parameter2, Para3.ParaName AS ParaName3,
            dbo.T_DIM_FORMULAID_SET.Parameter3, Para4.ParaName AS ParaName4, dbo.T_DIM_FORMULAID_SET.Parameter4, Para5.ParaName AS ParaName5, dbo.T_DIM_FORMULAID_SET.Parameter5,
            Para6.ParaName AS ParaName6, dbo.T_DIM_FORMULAID_SET.Parameter6, Para7.ParaName AS ParaName7, dbo.T_DIM_FORMULAID_SET.Parameter7, Para8.ParaName AS ParaName8,
            dbo.T_DIM_FORMULAID_SET.Parameter8, dbo.T_DIM_SENSOR_PRODUCT.FORMAULAID, dbo.T_DIM_SENSOR.SAFETY_FACTOR_TYPE_ID, dbo.T_DIM_PROTOCOL_TYPE.PROTOCOL_CODE,
            dbo.T_DIM_REMOTE_DTU.REMOTE_DTU_SUBSCRIBER,dbo.T_DIM_SENSOR_PRODUCT.PRODUCT_TYPE_KEY
FROM         dbo.T_DIM_SENSOR INNER JOIN
                      dbo.T_DIM_REMOTE_DTU ON dbo.T_DIM_SENSOR.DTU_ID = dbo.T_DIM_REMOTE_DTU.ID INNER JOIN
                      dbo.T_DIM_SENSOR_PRODUCT ON dbo.T_DIM_SENSOR.PRODUCT_SENSOR_ID = dbo.T_DIM_SENSOR_PRODUCT.PRODUCT_ID INNER JOIN
                      dbo.T_DIM_PROTOCOL_TYPE ON dbo.T_DIM_SENSOR_PRODUCT.PROTOCOL_ID = dbo.T_DIM_PROTOCOL_TYPE.PROTOCOL_ID LEFT OUTER JOIN
                      dbo.T_DIM_FORMULAID_SET ON dbo.T_DIM_SENSOR.SENSOR_ID = dbo.T_DIM_FORMULAID_SET.SENSOR_ID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para1 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID1 = Para1.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para2 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID2 = Para2.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para3 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID3 = Para3.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para4 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID4 = Para4.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para5 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID5 = Para5.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para6 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID6 = Para6.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para7 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID7 = Para7.FormulaParaID LEFT OUTER JOIN
                      dbo.V_FORMULA AS Para8 ON dbo.T_DIM_FORMULAID_SET.FormulaParaID8 = Para8.FormulaParaID
WHERE     (dbo.T_DIM_SENSOR.IsDeleted = 0) and SAFETY_FACTOR_TYPE_ID is not null and T_DIM_SENSOR.STRUCT_ID is not null
and T_DIM_SENSOR.SENSOR_ID is not null and T_DIM_SENSOR.MODULE_NO is not null and T_DIM_SENSOR_PRODUCT.PRODUCT_TYPE_KEY is not null";

            //var parms = new[] { new SqlParameter("@SAFETY_FACTOR_TYPE_ID", BridgeSettleGps) }; // 桥墩沉降
            using (DataSet dst = DbHelperSQL.Query(sqlstr))
            {
               // return dst.Tables[0];
                IList<SensorInfo> list = new List<SensorInfo>();
                foreach (DataRow row in dst.Tables[0].Rows)
                {
                    var sensor = new SensorInfo
                                     {
                                         StructureId = int.Parse(row["STRUCT_ID"].ToString().Trim()),
                                         RemoteDtuNumber = row["REMOTE_DTU_NUMBER"].ToString().Trim(),
                                         ProductTypeKey = int.Parse(row["PRODUCT_TYPE_KEY"].ToString().Trim()),
                                         DaiModuleNo = row["MODULE_NO"].ToString().Trim(),
                                         DaiChannelNumber =
                                             int.Parse(row["DAI_CHANNEL_NUMBER"].ToString().Trim()),
                                         ProtocolCode = int.Parse(row["PROTOCOL_CODE"].ToString().Trim()),
                                         SafetyFactorTypeId =
                                             int.Parse(row["SAFETY_FACTOR_TYPE_ID"].ToString().Trim()),
                                         SensorId = int.Parse(row["SENSOR_ID"].ToString().Trim())
                                     };
                    try
                    {
                        sensor.UniqueSign = row["UniqueSign"].ToString().Trim();
                    }
                    catch
                    {
                    }

                    int formulaId;
                    if (int.TryParse(row["FORMAULAID"].ToString().Trim(), out formulaId))
                    {
                        sensor.Formaulaid = formulaId;
                        double parameter;
                        if (double.TryParse(row["Parameter1"].ToString().Trim(), out parameter))
                        {
                            sensor.Parameter1 = parameter;
                        }

                        if (double.TryParse(row["Parameter2"].ToString().Trim(), out parameter))
                        {
                            sensor.Parameter2 = parameter;
                        }

                        if (double.TryParse(row["Parameter3"].ToString().Trim(), out parameter))
                        {
                            sensor.Parameter3 = parameter;
                        }
                        if (double.TryParse(row["Parameter4"].ToString().Trim(), out parameter))
                        {
                            sensor.Parameter4 = parameter;
                        }
                        if (double.TryParse(row["Parameter5"].ToString().Trim(), out parameter))
                        {
                            sensor.Parameter5 = parameter;
                        }
                        if (double.TryParse(row["Parameter6"].ToString().Trim(), out parameter))
                        {
                            sensor.Parameter6 = parameter;
                        }

                        if (double.TryParse(row["Parameter7"].ToString().Trim(), out parameter))
                        {
                            sensor.Parameter7 = parameter;
                        }
                    }
                    list.Add(sensor);
                }
                return list;
            }
        }
    }
}