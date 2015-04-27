using System.Data;
using System.Linq;
using FS.SMIS_Cloud.DAC.Model;
using System;
using System.Collections.Generic;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Task;
using FS.SMIS_Cloud.DAC.Tran.Db;
using FS.SMIS_Cloud.DAC.Util;
using log4net;

namespace FS.SMIS_Cloud.DAC.Accessor.MSSQL
{
    using DbHelper;

    using Gprs.Cmd;

    public class MsDbAccessor : IDbAccessor, ISaveAttask
    {
        private static readonly ILog Log = LogManager.GetLogger("MsDbAccessor");

        // 公式参数字典
        //private Dictionary<int, FormulaParam> _formulaParamDict;
        private readonly ISqlHelper _helper;

        public MsDbAccessor(string connStr)
        {
            _helper = SqlHelperFactory.Create(DbType.MSSQL, connStr);
            GetFormulaParamDict();
            //ComposeSerializers();
        }

        /// <summary>
        /// 获取DTU列表. 
        /// </summary>
        /// <param name="dtuCode">要查询的DTU编码</param>
        /// <param name="networkType">网络类型</param>
        /// <returns></returns>
        public IList<DtuNode> QueryDtuNodes(string dtuCode = null, NetworkType? networkType = null)
        {
            List<DtuNode> dtus = new List<DtuNode>();
            DataSet ds = _helper.Query(string.Format(@"
select 
    D.[ID] id, 
    D.[REMOTE_DTU_NUMBER] code, 
    D.[REMOTE_DTU_SUBSCRIBER] subscriber,
    D.[REMOTE_DTU_GRANULARITY] interval, 
    D.[DESCRIPTION] as [desc] ,
    D.[ProductDtuId],
    P.[NetworkType],
    D.[P1],
    D.[P2],
    D.[P3],
    D.[P4]
from T_DIM_REMOTE_DTU D 
left join [T_DIM_DTU_PRODUCT] P on P.ProductId = D.ProductDtuId
where D.ProductDtuId !=3 {0} {1}
order by ID", 
            string.IsNullOrEmpty(dtuCode)? "":  string.Format("and D.REMOTE_DTU_NUMBER={0}", dtuCode) ,
            networkType == null ? "": string.Format("and P.NetworkType='{0}'", networkType)
            ));
            if (ds != null)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        NetworkType nwType;
                        bool success = Enum.TryParse(Convert.ToString(dr["NetworkType"]), out nwType);

                        DtuNode di = new DtuNode
                        {
                            DtuId = (uint) Convert.ToInt32(dr["id"]),
                            //StructId = dr.IsNull("sid") ? 0 : Convert.ToUInt32(dr["sid"]),
                            DtuCode = Convert.ToString(dr["code"]),
                            DacInterval = dr.IsNull("interval")
                                ? DtuNode.DefaultDacInterval
                                : (uint) (Convert.ToInt32(dr["interval"])*60),
                            Name = Convert.ToString(dr["desc"]),
                            NetworkType = success ? (NetworkType?) nwType : null
                        };
                        di.AddProperty("param1", Convert.ToString(dr["P1"]));
                        di.AddProperty("param2", Convert.ToString(dr["P2"]));
                        di.AddProperty("param3", Convert.ToString(dr["P3"]));
                        di.AddProperty("param4", Convert.ToString(dr["P4"]));
                        dtus.Add(di);
                    }
                    catch (Exception ex)
                    {
                        Log.WarnFormat("DTU :{0}-{1}  error:{2}", Convert.ToInt32(dr["id"]), Convert.ToString(dr["code"]), ex.Message);
                    }
                }
            }
            ds = _helper.Query(@"
select
    s.STRUCT_ID structId,
    s.DTU_ID dtuId,
    s.SENSOR_ID sid,
    s.MODULE_NO mno,
    s.DAI_CHANNEL_NUMBER cno,
    s.SAFETY_FACTOR_TYPE_ID factor, 
    s.PRODUCT_SENSOR_ID pid,
    s.Identification sensortype,
    s.Enable enable,
    protocol.PROTOCOL_CODE protocol_type,
    s.SENSOR_LOCATION_DESCRIPTION name,
    factorType.THEMES_TABLE_NAME factorTypeTable,
    factorType.THEMES_COLUMNS tableColums,
    product.PRODUCT_ID productId,
    product.PRODUCT_CODE productCode
from T_DIM_SENSOR s ,dbo.T_DIM_SENSOR_PRODUCT product, dbo.T_DIM_PROTOCOL_TYPE protocol,dbo.T_DIM_SAFETY_FACTOR_TYPE factorType
where s.PRODUCT_SENSOR_ID = product.PRODUCT_ID AND product.PROTOCOL_ID = protocol.PROTOCOL_ID AND s.SAFETY_FACTOR_TYPE_ID = factorType.SAFETY_FACTOR_TYPE_ID AND s.MODULE_NO is not null AND s.IsDeleted = 0 
order by SENSOR_ID");
            DataTable table = ds.Tables[0];
            var formulaParamDict = GetFormulaParamDict(); // 获取参数定义
            DataSet paramSet = _helper.Query(@"select * from T_DIM_FORMULAID_SET"); //公式配置。
            DataTable paramTable = paramSet.Tables[0];

            foreach (DtuNode dtu in dtus)
            {
                if (dtu == null) continue;
                // filter to Node
                var sr = from r in table.AsEnumerable()
                    where !r.IsNull("dtuId") && r.Field<int>("dtuId") == dtu.DtuId
                    orderby r["sid"]
                    select r;
                if (!sr.Any()) continue;
                foreach (DataRow row in sr)
                {
                    try
                    {
                        var sensor = new Sensor
                        {
                            DtuID = dtu.DtuId,
                            DtuCode = dtu.DtuCode,
                            StructId = Convert.ToUInt32(row["structId"]),
                            SensorID = Convert.ToUInt32(row["sid"]),
                            ChannelNo = row.IsNull("cno") ? 1 : Convert.ToUInt32(row["cno"]),
                            ModuleNo = row.IsNull("mno") ? 0 : Convert.ToUInt32(row["mno"]),
                            ProtocolType = Convert.ToUInt16(row["protocol_type"]),
                            Name = Convert.ToString(row["name"]),
                            FactorType = Convert.ToUInt32(row["factor"]),
                            FactorTypeTable = Convert.ToString(row["factorTypeTable"]),
                            TableColums = Convert.ToString(row["tableColums"]),
                            ProductId = Convert.ToInt32(row["productId"]),
                            ProductCode = row["productCode"].ToString().Trim(),
                            AcqInterval = dtu.DacInterval / 60, // 分钟
                            LastTime = DateTime.MinValue,
                            SensorType = (SensorType)Convert.ToByte(row["sensortype"]),
                            UnEnable = Convert.ToBoolean(row["enable"])
                        };
                        GetParameters(paramTable, sensor, formulaParamDict);
                        if (sensor.Parameters.Count > 0)
                        {
                            sensor.FormulaID = (uint)sensor.Parameters[0].FormulaParam.FID;
                        }
                        dtu.AddSensor(sensor);
                    }
                    catch (Exception ex)
                    {
                        Log.WarnFormat("Sensor :{0}  error:{1}",Convert.ToUInt32(row["sid"]),ex.Message);
                    }
                }
            }
            return dtus;
        }

        public IList<SensorGroup> QuerySensorGroups()
        {
            List<SensorGroup> groups = new List<SensorGroup>();
            DataSet ds = this._helper.Query(@"
select DISTINCT G.*,S2.DTU_ID,D.REMOTE_DTU_NUMBER,D.REMOTE_DTU_GRANULARITY
 from
(select A.GROUP_ID ,COUNT(DISTINCT S.DTU_ID) as DtuNum
  from (SELECT [GROUP_ID]
      ,[SENSOR_ID]
  FROM [dbo].[T_DIM_SENSOR_GROUP_CEXIE]
UNION ALL
SELECT [GROUP_ID]
      ,[SENSOR_ID]
  FROM [dbo].[T_DIM_SENSOR_GROUP_CHENJIANG]
UNION ALL
SELECT [GROUP_ID]
      ,[SENSOR_ID]
  FROM [dbo].[T_DIM_SENSOR_GROUP_JINRUNXIAN]) as A
  join [dbo].[T_DIM_SENSOR] S on A.SENSOR_ID=S.SENSOR_ID 
  group by A.GROUP_ID
  having COUNT(DISTINCT S.DTU_ID) > 1 ) as G 
join
(SELECT [GROUP_ID]
      ,[SENSOR_ID]
  FROM [dbo].[T_DIM_SENSOR_GROUP_CEXIE]
UNION ALL
SELECT [GROUP_ID]
      ,[SENSOR_ID]
  FROM [dbo].[T_DIM_SENSOR_GROUP_CHENJIANG]
UNION ALL
SELECT [GROUP_ID]
      ,[SENSOR_ID]
  FROM [dbo].[T_DIM_SENSOR_GROUP_JINRUNXIAN]) as B on G.GROUP_ID=B.GROUP_ID
  join [dbo].[T_DIM_SENSOR] S2 on B.SENSOR_ID=S2.SENSOR_ID 
	join [dbo].[T_DIM_REMOTE_DTU] D on S2.DTU_ID=D.ID and D.ProductDtuId!=3
order by G.GROUP_ID;");
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return groups;
            groups.AddRange(from DataRow row in ds.Tables[0].Rows
                select new SensorGroup
                {
                    DtuCode = row["REMOTE_DTU_NUMBER"].ToString(),
                    DtuId = Convert.ToUInt32(row["DTU_ID"]),
                    GroupId = Convert.ToInt32(row["GROUP_ID"]),
                    DacInterval = Convert.ToUInt32(row["REMOTE_DTU_GRANULARITY"])*60
                });

            // -- 虚拟分组
            ds = this._helper.Query(@"
  select distinct SensorId,DTU_ID,REMOTE_DTU_NUMBER, REMOTE_DTU_GRANULARITY from T_DIM_SENSOR_CORRENT corr
  join T_DIM_SENSOR sens on sens.SENSOR_ID=corr.CorrentSensorId
  join T_DIM_REMOTE_DTU dtu on dtu.ID=sens.DTU_ID
  where corr.SensorId in(
  select c.SensorId from T_DIM_SENSOR_CORRENT c
  left join T_DIM_SENSOR s on s.SENSOR_ID=c.CorrentSensorId
  group by c.SensorId having COUNT(distinct(s.DTU_ID))>1)
  
");
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return groups;
            groups.AddRange(from DataRow row in ds.Tables[0].Rows
                            select new SensorGroup
                            {
                                DtuCode = row["REMOTE_DTU_NUMBER"].ToString(),
                                DtuId = Convert.ToUInt32(row["DTU_ID"]),
                                GroupId = -Convert.ToInt32(row["SensorId"]),   // 此处用负数是为了避免和分组ID冲突 
                                DacInterval = Convert.ToUInt32(row["REMOTE_DTU_GRANULARITY"]) * 60
                            });

            return groups;
        }
        
        // 获取参数定义
        private Dictionary<int, FormulaParam> GetFormulaParamDict()
        {
           var formulaParamDict = new Dictionary<int, FormulaParam>();
            using
                (DataSet ds = _helper.Query(@"
SELECT
	  P.FormulaID fid,
      P.[Order] ,
	  P.FormulaParaID pid,
      N.ParaName name, N.ParaAlias alias
  FROM  T_DIM_FORMULA_PARA P,T_DIM_FORMULA_PARA_NAME N
  where P.ParaNameID = N.ParaNameID
  order by FormulaID, P.[Order]"))
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    FormulaParam pi = new FormulaParam
                    {
                        PID = Convert.ToInt32(row["pid"]),
                        FID = Convert.ToInt32(row["fid"]),
                        Index = Convert.ToInt32(row["Order"]),
                        Name = Convert.ToString(row["name"]),
                        Alias = Convert.ToString(row["alias"])
                    };
                    formulaParamDict[pi.PID] = pi;
                }
            }
            return formulaParamDict;
        }

        // 获取传感器参数定义。
        private void GetParameters(DataTable table, Sensor sensor, Dictionary<int, FormulaParam> formulaParamDict)
        {
            var sr = from r in table.AsEnumerable()
                where r.Field<int>("SENSOR_ID") == sensor.SensorID
                select r;
            if (!sr.Any())
                return;
            foreach (DataRow row in sr)
            {
                for (int i = 1; i < 7; i++)
                {
                    string pName = string.Format("FormulaParaID{0}", i);
                    string pValue = string.Format("Parameter{0}", i);
                    if (row.IsNull(pName))
                        break;
                    int pid = Convert.ToInt32(row[pName]);
                    FormulaParam temp = GetFormulaParam(pid, formulaParamDict);
                    var pi = new SensorParam(temp);
                    if (row.IsNull(pValue))
                    {
                        Console.WriteLine("Sensor: {0}'s param: {1}-{2} 's value is NULL!", sensor.SensorID, i,
                            temp.Name);
                    }
                    pi.Value = row.IsNull(pValue) ? 0 : Convert.ToDouble(row[pValue]);
                    sensor.AddParameter(pi);
                }
            }
        }

        private FormulaParam GetFormulaParam(int paramId, Dictionary<int, FormulaParam> formulaParamDict)
        {
            FormulaParam fp;
            formulaParamDict.TryGetValue(paramId, out fp);
            return fp;
        }

        // 
        public DtuNode QueryDtuNode(string dtuCode)
        {
            IList<DtuNode> dtus = QueryDtuNodes(dtuCode);
            if (dtus != null && dtus.Count > 0)
            {
                return dtus[0];
            }
            return null;
        }


        public IList<DACTask> GetUnfinishedTasks()
        {
            DataSet ds = _helper.Query(@"
SELECT *
  FROM  T_TASK_INSTANT
  where STATUS=0
  order by ID"); // RUNNING = 0, DONE=1
            List<DACTask> tasks = new List<DACTask>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DACTask t = new DACTask();
                t.DtuID = Convert.ToUInt32(row["DTU_ID"]);
                t.ID = Convert.ToInt32(row["ID"]);
                t.Saved = true;
                t.Sensors = ValueHelper.ToIntArray(Convert.ToString(row["SENSORS"]));
                if (!Convert.IsDBNull(row["REQUESTED"]))
                    t.Requested =  Convert.ToDateTime(row["REQUESTED"]);
                t.Status = (DACTaskStatus)Convert.ToInt32(row["STATUS"]);
                t.Requester = Convert.ToString("REQUESTER");
                tasks.Add(t);
            }
            return tasks;
        }


        public int SaveInstantTask(DACTask task)
        {
            Log.DebugFormat("Saving task result for {0}", task.DtuID);

            string sql;
            if (task.Saved)
            {
                sql = string.Format("update T_TASK_INSTANT set status={0}, finished='{1}' where ID={2}", (int)task.Status, String.Format("{0:yyyy-M-d HH:mm:ss}", task.Finished), task.ID);
            }
            else
            {
                sql = string.Format(@"insert into T_TASK_INSTANT 
    (DTU_ID,SENSORS,TASK_NAME,REQUESTER,REQUESTED,STATUS,MSG_ID) values 
    ({0},'{1}','{2}','{3}','{4}',{5},'{6}')",
                task.DtuID,
                ValueHelper.ToStr(task.Sensors),
                "Instant",
                "admin",
                String.Format("{0:yyyy-M-d HH:mm:ss}", task.Requested),
                (int)task.Status,
                task.TID
              );
            }
            Console.WriteLine(sql);
            int effectedRowsOrId = _helper.ExecuteSql(sql);
            if (!task.Saved)
            {
                task.ID = effectedRowsOrId;
            }
            task.Saved = true;
            return effectedRowsOrId;
        }

        public int UpdateInstantTask(DACTaskResult result)
        {
            // always UpdateInstantTask
            Log.DebugFormat("Updating task result for {0} - {1}", result.Task.DtuID, result.Task.ID);
            string sql = string.Format(
@"update T_TASK_INSTANT set 
STATUS={0}, FINISHED='{1}', ELAPSED={2}, RESULT_CODE={3}, RESULT_MSG='{4}', RESULT_JSON='{5}' where ID={6}",
                (int) result.Task.Status,  //0
                string.Format("{0:yyyy-M-d HH:mm:ss}",result.Finished),  //1
                result.Elapsed,
                string.IsNullOrEmpty(result.GetJsonResult()) ? (int)Errors.ERR_UNKNOW : result.ErrorCode, //3
                string.IsNullOrEmpty(result.ErrorMsg) || string.IsNullOrEmpty(result.GetJsonResult()) ? "FAILED" : result.ErrorMsg, //4
                result.GetJsonResult(), // JsonConvert.SerializeObject(result.SensorResults) 5
                result.Task.ID //2
                );
            // Console.WriteLine(sql);
            int rows = _helper.ExecuteSql(sql);
            Console.WriteLine("{0} rows updated", rows);
            return rows;
        }

        public List<DacErrorCode> QueryDacErrorCodes()
        {
            DataSet ds = _helper.Query("select * from T_DIM_DAC_ERROR_CODE");
            var dacerrorcodes = new List<DacErrorCode>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                dacerrorcodes = (from e in dt.AsEnumerable()
                    select (new DacErrorCode
                    {
                        ErrorCode = e.Field<int>("ERROR_CODE"),
                        ErrorNameUs = e.Field<string>("ERROR_NAME_US").Trim(),
                        ErrorDescription = e.Field<string>("ERROR_DESCRIPTION").Trim()
                    })).ToList();
            }
            return dacerrorcodes;
        }
        

        #region Implementation of ISaveAttask

        public int SaveInstantTask(ATTask task)
        {
            Log.DebugFormat("Saving task result for {0}", task.DtuID);

            string sql;
            if (task.Saved)
            {
                sql = string.Format("update T_TASK_INSTANT set status={0}, finished='{1}' where ID={2}", (int)task.Status, String.Format("{0:yyyy-M-d HH:mm:ss}", task.Finished), task.ID);
            }
            else
            {
                sql = string.Format(@"insert into T_TASK_INSTANT 
    (DTU_ID,TASK_NAME,REQUESTER,REQUESTED,STATUS,MSG_ID) values 
    ({0},'{1}','{2}','{3}',{4},'{5}')",
                task.DtuID,
                "Instant",
                "admin",
                String.Format("{0:yyyy-M-d HH:mm:ss}", task.Requested),
                (int)task.Status,
                task.TID
              );
            }
            Console.WriteLine(sql);
            int effectedRowsOrId = _helper.ExecuteSql(sql);
            if (!task.Saved)
            {
                task.ID = effectedRowsOrId;
            }
            task.Saved = true;
            return effectedRowsOrId;
        }

        public int UpdateInstantTask(ExecuteResult result)
        {
            // always UpdateInstantTask
            Log.DebugFormat("Updating task result for {0} - {1}", result.Task.DtuID, result.Task.ID);
            string sql = string.Format(
@"update T_TASK_INSTANT set 
STATUS={0}, FINISHED='{1}', ELAPSED={2}, RESULT_CODE={3}, RESULT_MSG='{4}', RESULT_JSON='{5}' where ID={6}",
                (int)result.Task.Status,  //0
                string.Format("{0:yyyy-M-d HH:mm:ss}", result.Finished),  //1
                result.Elapsed,
                result.ErrorCode, //3
                result.ErrorMsg ?? "FAILED", //4
                result.ToJsonString(), //5
                result.Task.ID //2
                );
            // Console.WriteLine(sql);
            int rows = _helper.ExecuteSql(sql);
            Console.WriteLine("{0} rows updated", rows);
            return rows;
        }

        #endregion
    }
}
