namespace FS.SMIS_Cloud.NGDAC.Accessor.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO.Ports;
    using System.Linq;

    using FS.DbHelper;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Task;

    public class SQLiteDbAccessor : IDbAccessor
    {
       // private ConcurrentDictionary<ProtocolType, IDataSerializer> _serializers;

        // 公式参数字典
        private Dictionary<int, FormulaParam> _formulaParamDict;
        private readonly ISqlHelper _cfgHelper;
        
        public SQLiteDbAccessor(string csCfg)
        {
            this._cfgHelper = SqlHelperFactory.Create(FS.DbHelper.DbType.SQLite, csCfg);
            this.GetFormulaParamDict();
        }


        public IList<DtuNode> QueryDtuNodes(string dtuCode = null, NetworkType? networkType = null)
        {

            List<DtuNode> dtus = new List<DtuNode>();
            DataSet ds = this._cfgHelper.Query(string.Format(@"
select 
    ID id, 
    PortName code, 
    CircleIntervalTime as 'interval',
    [BaudRate],
    [Parity],
    [DataBits],
    [StopBits],
    [ReadTimeOut]
from PortConfig
{0}
order by ID", dtuCode != null ? string.Format("where PortName='{0}'", dtuCode) : ""));
            if (ds != null)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DtuNode di = new DtuNode
                    {
                        DtuId = (uint) Convert.ToInt32(dr["id"]),
                        DtuCode = Convert.ToString(dr["code"]),
                        DacInterval = dr.IsNull("interval")
                            ? DtuNode.DefaultDacInterval
                            : (uint) (Convert.ToInt32(dr["interval"])*60),
                        Name = "", // Column Not Exist.
                        Type = DtuType.Com
                    };
                    di.AddProperty("serial", new SerialPort
                    {
                        PortName = di.DtuCode,
                        BaudRate = Convert.ToInt32(dr["BaudRate"]),
                        Parity = (Parity) Convert.ToInt32(dr["Parity"]),
                        DataBits = Convert.ToInt32(dr["DataBits"]),
                        StopBits = (StopBits) Convert.ToInt32(dr["StopBits"]),
                        ReadTimeout = Convert.ToInt32(dr["ReadTimeOut"])
                    });
                    dtus.Add(di);
                }
            }
            // All Sensors
            ds = this._cfgHelper.Query(@"
select
    d.IP_DTU_SERIALPORT dtuId,
    s.SENSOR_SET_ID sid,
    d.MODULE_NO mno,
    s.CHANNEL_ID cno,
    s.SafeTypeId factor, 
    s.SENSOR_PRODUCT_ID pid,
    product.PROTOCOL_ID protocol_type,
    s.SENSORLOCATION_DESCRIPTION name,
    'DONOTUSE' factorTypeTable,
    'DONOTUSE' tableColums
from S_SENSOR_SET s , C_DAI_PRODUCT product,  SAFETY_FACTOR_TYPE factorType, S_DAI_SET d
where d.[DAI_SET_ID] = s.[DAI_SET_ID] AND s.SENSOR_PRODUCT_ID = product.DAI_PRODUCT_ID AND  s.SafeTypeId = factorType.SAFETY_FACTOR_TYPE_ID 
order by sid");
            DataTable table = ds.Tables[0];

            DataSet paramSet = this._cfgHelper.Query(@"select * from S_FORMULAID_SET"); //公式配置。
            DataTable paramTable = paramSet.Tables[0];

            foreach (DtuNode dtu in dtus)
            {
                // filter to Node
                var sr = from r in table.AsEnumerable()
                    where !r.IsNull("dtuId") && r.Field<string>("dtuId") == dtu.DtuCode
                    orderby r["sid"]
                    select r;
                if (!sr.Any()) continue;
                foreach (DataRow row in sr)
                {
                    var sensor = new Sensor
                    {
                        DtuID = dtu.DtuId,
                        SensorID = Convert.ToUInt32(row["sid"]),
                        ChannelNo = row.IsNull("cno") ? 1 : Convert.ToUInt32(row["cno"]),
                        ModuleNo = row.IsNull("mno") ? 0 : Convert.ToUInt32(row["mno"]),
                        ProtocolType = Convert.ToUInt16(row["protocol_type"]),
                        Name = Convert.ToString(row["name"]),
                        FactorType = Convert.ToUInt32(row["factor"]),
                        FactorTypeTable = Convert.ToString(row["factorTypeTable"]),
                        TableColums = Convert.ToString(row["tableColums"])
                    };
                    this.GetParameters(paramTable, sensor);
                    if (sensor.Parameters.Count > 0)
                    {
                        sensor.FormulaID = (uint) sensor.Parameters[0].FormulaParam.FID;
                    }
                    dtu.AddSensor(sensor);
                }
            }
            return dtus;
        }

        //获取参数定义
        private void GetFormulaParamDict()
        {
            this._formulaParamDict = new Dictionary<int, FormulaParam>();
            DataSet ds = this._cfgHelper.Query(@"
SELECT
	  P.FORMULA_ID fid,
	  P.Formula_Para_ID pid,
    N.Para_Name name, N.Para_Alias alias
  FROM  C_FORMULA_PARA P,C_FORMULA_PARA_NAME N
  where P.Para_Name_ID = N.Para_Name_ID  
order by fid, pid");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                FormulaParam pi = new FormulaParam
                {
                    PID = Convert.ToInt32(row["pid"]),
                    FID = Convert.ToInt32(row["fid"]),
                    Index = 0, // 表中未定义.
                    Name = Convert.ToString(row["name"]),
                    Alias = Convert.ToString(row["alias"])
                };
                this._formulaParamDict[pi.PID] = pi;
            }
        }

        // 获取传感器参数定义。
        private void GetParameters(DataTable table, Sensor sensor)
        {
            var sr = from r in table.AsEnumerable()
                where r.Field<int>("SENSOR_SET_ID") == sensor.SensorID
                select r;
            if (!sr.Any())
                return;
            foreach (DataRow row in sr)
            {
                ushort paraCount = Convert.ToUInt16(row["ParaCount"]);
                for (int i = 1; i <= paraCount; i++)
                {
                    string pName = string.Format("PARA_NAME_ID{0}", i);
                    string pValue = string.Format("PARAMETER{0}", i);
                    if (row.IsNull(pName))
                        break;
                    int pid = Convert.ToInt32(row[pName]);
                    FormulaParam temp = this.GetFormulaParam(pid);
                    SensorParam pi = new SensorParam(temp);
                    if (row.IsNull(pValue))
                    {
                        // TODO write Log.
                        Console.WriteLine("Sensor: {0}'s param: {1}-{2} 's value is NULL!", sensor.SensorID, i,
                            temp.Name);
                    }
                    pi.Value = row.IsNull(pValue) ? 0 : Convert.ToDouble(row[pValue]);
                    sensor.AddParameter(pi);
                }
            }
        }

        private FormulaParam GetFormulaParam(int paramId)
        {
            FormulaParam fp = null;
            this._formulaParamDict.TryGetValue(paramId, out fp);
            return fp;
        }

        // 
        public DtuNode QueryDtuNode(string dtuCode)
        {
            IList<DtuNode> dtus = this.QueryDtuNodes(dtuCode);
            if (dtus != null && dtus.Count > 0)
            {
                return dtus[0];
            }
            else
            {
                return null;
            }
        }

        public IList<DACTask> GetUnfinishedTasks()
        {
            return null; 
        }

        public int SaveInstantTask(DACTask task)
        {
            return 0;
        }

        public int UpdateInstantTask(DACTaskResult result)
        {
            return 0;
        }

        public IList<SensorGroup> QuerySensorGroups()
        {
            throw new NotImplementedException();
        }

        public List<DacErrorCode> QueryDacErrorCodes()
        {
            return null;
        }

        //public DataMetaInfo[] GetDataMetas()
        //{
        //    List<DataMetaInfo> dms = new List<DataMetaInfo>();
        //    foreach (IDataSerializer si in _serializers.Values)
        //    {
        //        if (!dms.Contains(si.MetaInfo))
        //            dms.Add(si.MetaInfo);
        //    }
        //    return dms.ToArray();
        //}
        
    }
}

