using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using FS.SMIS_Cloud.Alarm.Forwarder.Model;
using log4net;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Dal
{
    public class DataAccess
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());

        /// <summary>
        ///     获取告警信息
        /// </summary>
        /// <param name="top"></param>
        /// <param name="warningstatus"></param>
        /// <param name="dealflag"></param>
        /// <returns></returns>
        public static List<WarningInfo> GetWarningInfo(int top, int warningstatus, int dealflag)
        {
            var list = new List<WarningInfo>();
            try
            {
                DataSet ds =
                    SqlHelper.ExecuteDataSetText(
                        "select top " + top.ToString() +
                        " * from dbo.T_DIM_WARNING_TYPE a,dbo.T_WARNING_SENSOR b where a.TypeId=b.WarningTypeId and b.WarningStatus=@b and b.DealFlag=@c  order by b.Time desc",
                        new[]
                        {
                            new SqlParameter("@b", warningstatus),
                            new SqlParameter("@c", dealflag)
                        });
                if (ds.Tables.Count != 0)
                {
                    foreach (DataRow item in ds.Tables[0].AsEnumerable())
                    {
                        list.Add(new WarningInfo
                        {
                            Id = Convert.ToInt32(item["Id"]),
                            WarningTypeId = Convert.ToString(item["WarningTypeId"]),
                            StructId = Convert.ToString(item["StructId"]),
                            DeviceTypeId = Convert.ToInt32(item["DeviceTypeId"]),
                            DeviceId = Convert.ToInt32(item["DeviceId"]),
                            Time = Convert.ToDateTime(item["Time"]),
                            Description = Convert.ToString(item["Description"]),
                            Content = Convert.ToString(item["Content"]),
                            WarningLevel = Convert.ToString(item["WarningLevel"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }
            return list;
        }

        /// <summary>
        ///     通过结构物筛选告警信息
        /// </summary>
        /// <param name="top"></param>
        /// <param name="warningstatus"></param>
        /// <param name="dealflag"></param>
        /// <param name="structids"></param>
        /// <returns></returns>
        public static List<WarningInfo> GetWarningInfo(int top, int warningstatus, int dealflag, List<string> structids)
        {
            string ids = string.Empty;
            ids = string.Join(",", structids);
            var list = new List<WarningInfo>();
            try
            {
                DataSet ds =
                    SqlHelper.ExecuteDataSetText(
                        "select top " + top.ToString() +
                        " * from dbo.T_DIM_WARNING_TYPE a,dbo.T_WARNING_SENSOR b where a.TypeId=b.WarningTypeId and b.StructId in (" +
                        ids + ") and b.WarningStatus=@b and b.DealFlag=@c  order by b.Time desc",
                        new[]
                        {
                            new SqlParameter("@b", warningstatus),
                            new SqlParameter("@c", dealflag)
                        });
                if (ds.Tables.Count != 0)
                {
                    foreach (DataRow item in ds.Tables[0].AsEnumerable())
                    {
                        list.Add(new WarningInfo
                        {
                            Id = Convert.ToInt32(item["Id"]),
                            WarningTypeId = Convert.ToString(item["WarningTypeId"]),
                            StructId = Convert.ToString(item["StructId"]),
                            DeviceTypeId = Convert.ToInt32(item["DeviceTypeId"]),
                            DeviceId = Convert.ToInt32(item["DeviceId"]),
                            Time = Convert.ToDateTime(item["Time"]),
                            Description = Convert.ToString(item["Description"]),
                            Content = Convert.ToString(item["Content"]),
                            WarningLevel = Convert.ToString(item["WarningLevel"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }
            return list;
        }

        /// <summary>
        ///     检索未确认各级别告警条数
        /// </summary>
        /// <returns></returns>
        public static int GetWarningCntByLevel(int level)
        {
            int cnt = 0;
            try
            {
                DataSet ds =
                    SqlHelper.ExecuteDataSetText(
                        "select count(*) as cnt from dbo.T_DIM_WARNING_TYPE a,dbo.T_WARNING_SENSOR b where a.TypeId=b.WarningTypeId and b.WarningStatus>=4 and b.DealFlag=3 and a.WarningLevel=@a",
                        new[] {new SqlParameter("@a", level),});
                if (ds.Tables.Count != 0)
                {
                    foreach (DataRow item in ds.Tables[0].AsEnumerable())
                    {
                        cnt = Convert.ToInt32(item["cnt"]);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }

            return cnt;
        }

        /// <summary>
        ///     检索未确认各级别告警条数(通过结构物id获取对应结构的预警)
        /// </summary>
        /// <returns></returns>
        public static int GetWarningCntByLevel(int level, List<string> std)
        {
            int cnt = 0;
            string ids = string.Empty;
            ids = string.Join(",", std);
            try
            {
                DataSet ds =
                    SqlHelper.ExecuteDataSetText(
                        "select count(*) as cnt from dbo.T_DIM_WARNING_TYPE a,dbo.T_WARNING_SENSOR b where a.TypeId=b.WarningTypeId and b.WarningStatus>=4 and b.DealFlag=3 and a.WarningLevel=@a and b.StructId in (" +
                        ids + ")",
                        new[] {new SqlParameter("@a", level),});
                if (ds.Tables.Count != 0)
                {
                    if (ds.Tables[0].AsEnumerable().Any())
                    {
                        cnt = Convert.ToInt32(ds.Tables[0].AsEnumerable().First()["cnt"]);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }
            return cnt;
        }

        /// <summary>
        ///     获取接收人信息
        /// </summary>
        /// <param name="structId"></param>
        /// <param name="roleId"></param>
        /// <param name="warninglevel"></param>
        /// <returns></returns>
        public static List<ContactsInfo> GetReceiverInfo()
        {
            var list = new List<ContactsInfo>();
            try
            {
                DataSet ds =
                    SqlHelper.ExecuteDataSetText("select * from dbo.T_WARNING_SMS_RECIEVER  where ReceiveMode='true' ");
                if (ds.Tables.Count != 0)
                {
                    foreach (DataRow item in ds.Tables[0].AsEnumerable())
                    {
                        list.Add(new ContactsInfo
                        {
                            ReceiverName = Convert.ToString(item["RecieverName"]),
                            ReceiverPhone = Convert.ToString(item["RecieverPhone"]),
                            FilterLevel = Convert.ToInt32(item["FilterLevel"]),
                            UserNo = Convert.ToInt32(item["UserNo"]),
                            RoleId = Convert.ToInt32(item["RoleId"]),
                            ReceiverMail = Convert.ToString(item["RecieverMail"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }

            return list;
        }

        /// <summary>
        ///     获取用户和结构对应信息
        /// </summary>
        /// <returns></returns>
        public static List<UserStructure> GetUserStructureInfo()
        {
            var list = new List<UserStructure>();
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSetText("select * from dbo.T_DIM_USER_STRUCTURE");
                if (ds.Tables.Count != 0)
                {
                    foreach (DataRow item in ds.Tables[0].AsEnumerable())
                    {
                        list.Add(new UserStructure
                        {
                            User_No = Convert.ToInt32(item["USER_NO"]),
                            Structure_Id = Convert.ToInt32(item["STRUCTURE_ID"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }

            return list;
        }

        /// <summary>
        ///     更新技术支持告警状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static int UpdateStatusById(List<int> list, int status)
        {
            string ids = string.Empty;
            ids = string.Join(",", list);
            int cnt = 0;
            try
            {
                cnt = SqlHelper.ExecteNonQueryText(
                    "update dbo.T_WARNING_SENSOR set WarningStatus=@a where Id in (" + ids + ")",
                    new[] {new SqlParameter("@a", status)});
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }
            return cnt;
        }

        public static int UpdateStatusToSendedById(List<int> sendedAlarms)
        {
            string ids = string.Empty;
            ids = string.Join(",", sendedAlarms);
            int cnt = 0;
            try
            {
                cnt = SqlHelper.ExecteNonQueryText(
                    "update dbo.T_WARNING_SENSOR set WarningStatus=WarningStatus+1 where Id in (" + ids + ")");
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }
            return cnt;
        }

        /// <summary>
        ///     获取结构物信息
        /// </summary>
        /// <returns></returns>
        public static List<StructureInfo> GetStructureInfo()
        {
            var list = new List<StructureInfo>();
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSetText("select * from dbo.T_DIM_STRUCTURE");
                if (ds.Tables.Count != 0)
                {
                    foreach (DataRow item in ds.Tables[0].AsEnumerable())
                    {
                        list.Add(new StructureInfo
                        {
                            Id = Convert.ToInt32(item["ID"]),
                            StructureTypeId = Convert.ToString(item["STRUCTURE_TYPE_ID"]),
                            StructureNameCn = Convert.ToString(item["STRUCTURE_NAME_CN"]),
                            StructureNameEn = Convert.ToString(item["STRUCTURE_NAME_EN"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }

            return list;
        }

        /// <summary>
        ///     获取告警类型信息
        /// </summary>
        /// <returns></returns>
        public static List<WarningType> GetWarningTypeInfo()
        {
            var list = new List<WarningType>();
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSetText("select * from dbo.T_DIM_WARNING_TYPE");
                if (ds.Tables.Count != 0)
                {
                    foreach (DataRow item in ds.Tables[0].AsEnumerable())
                    {
                        list.Add(new WarningType
                        {
                            TypeId = Convert.ToString(item["TypeId"]),
                            Description = Convert.ToString(item["Description"]),
                            Reason = Convert.ToString(item["Reason"]),
                            WarningLevel = Convert.ToString(item["WarningLevel"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }

            return list;
        }

        /// <summary>
        ///     获取DTU信息
        /// </summary>
        /// <returns></returns>
        public static List<RemoteDtu> GetRemoteDtuInfo()
        {
            var list = new List<RemoteDtu>();
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSetText("select * from dbo.T_DIM_REMOTE_DTU");
                if (ds.Tables.Count != 0)
                {
                    foreach (DataRow item in ds.Tables[0].AsEnumerable())
                    {
                        list.Add(new RemoteDtu
                        {
                            Id = Convert.ToInt32(item["ID"]),
                            RemoteDtuNumber = Convert.ToString(item["REMOTE_DTU_NUMBER"]),
                            Description = Convert.ToString(item["DESCRIPTION"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }

            return list;
        }

        /// <summary>
        ///     获取传感器信息
        /// </summary>
        /// <returns></returns>
        public static List<Sensor> GetSensorInfo()
        {
            var list = new List<Sensor>();
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSetText("select * from dbo.T_DIM_SENSOR");
                if (ds.Tables.Count != 0)
                {
                    foreach (DataRow item in ds.Tables[0].AsEnumerable())
                    {
                        list.Add(new Sensor
                        {
                            Sensor_Id = Convert.ToInt32(item["SENSOR_ID"]),
                            SensorLocationDs = Convert.ToString(item["SENSOR_LOCATION_DESCRIPTION"]),
                            Dtu_Id = Convert.ToString(item["DTU_ID"]),
                            Dai_Channel_Number = Convert.ToString(item["DAI_CHANNEL_NUMBER"]),
                            Module_No = Convert.ToString(item["MODULE_NO"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }

            return list;
        }


        /// <summary>
        ///     获取结构物id
        /// </summary>
        /// <param name="orgs">组织id</param>
        /// <returns></returns>
        public static List<string> GetStructureIdsByOrganizationId(List<string> orgs)
        {
            var list = new List<string>();
            string ids = string.Empty;
            ids = string.Join(",", orgs);
            try
            {
                DataSet ds =
                    SqlHelper.ExecuteDataSetText(
                        "select STRUCTURE_ID from dbo.T_DIM_ORG_STUCTURE where ORGANIZATION_ID in(" + ids + ")");

                if (ds.Tables.Count != 0)
                {
                    foreach (DataRow item in ds.Tables[0].AsEnumerable())
                    {
                        list.Add(item["STRUCTURE_ID"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }

            return list;
        }
    }
}