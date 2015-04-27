using FS.SMIS_Cloud.NGET.Model;

namespace FS.SMIS_Cloud.NGET.DataCalc.Accessor
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    using FS.DbHelper;
    using FS.SMIS_Cloud.NGET.DataCalc.Model;

    public class MsDbAccessorAdv
    {
        private readonly ISqlHelper _dbHelper = null;

        public MsDbAccessorAdv(String conn)
        {
            this._dbHelper = SqlHelperFactory.Create(DbHelper.DbType.MSSQL, conn);
        }

        public IList<SensorGroup> QuerySensorGroupsByDtuid(uint dtuid)
        {
            var sensorgroups = new List<SensorGroup>();
            IList<SensorGroup> virtualsens = this.QueryVirtualSensors(dtuid); // 查询虚拟传感器
            if (virtualsens.Count > 0)
            {
                sensorgroups.AddRange(virtualsens);
            }
            IList<SensorGroup> groups = this.QuerySensorGroups(dtuid);
            if (groups.Count > 0)
            {
                sensorgroups.AddRange(groups);
            }
            return sensorgroups;
        }

        private IList<SensorGroup> QueryVirtualSensors(uint dtuid)
        {
            IList<SensorGroup> virtualsens = new List<SensorGroup>();

            String virtualSensorQuerySql = string.Format(
@"select s.SENSOR_ID,s.SAFETY_FACTOR_TYPE_ID,s.STRUCT_ID,ss.DTU_ID,p.FORMAULAID,p.PROTOCOL_ID,
f.Parameter1,f.Parameter2,f.Parameter3,f.Parameter4,f.Parameter5,f.Parameter6,f.Parameter7,c.CorrentSensorId,
sf.THEMES_TABLE_NAME,sf.THEMES_COLUMNS  from [T_DIM_SENSOR_CORRENT] c
left join T_DIM_SENSOR s on s.SENSOR_ID=c.SensorId
left join T_DIM_SENSOR_PRODUCT p on s.PRODUCT_SENSOR_ID = p.PRODUCT_ID
left join T_DIM_FORMULAID_SET f on c.SensorId=f.SENSOR_ID
left join T_DIM_SAFETY_FACTOR_TYPE sf on sf.SAFETY_FACTOR_TYPE_ID=s.SAFETY_FACTOR_TYPE_ID
left join T_DIM_SENSOR ss on ss.SENSOR_ID=c.CorrentSensorId
where c.SensorId in(
select distinct(c.SensorId) from T_DIM_SENSOR s,[T_DIM_SENSOR_CORRENT] c 
where s.SENSOR_ID=c.CorrentSensorId and s.IsDeleted=0 and s.DTU_ID={0})", dtuid);

            DataTable table = this._dbHelper.Query(virtualSensorQuerySql).Tables[0];

            if (table.Rows.Count > 0)
            {
                var groups = from g in table.AsEnumerable()
                             group g by g.Field<int>("SENSOR_ID");
                foreach (var item in groups)
                {
                    if (!item.Any()) continue;
                    var agroup = new SensorGroup(item.Key, GroupType.VirtualSensor);
                    DataRow row = item.First();

                    // 虚拟传感器的分组信息(公式、参数、子集)
                    agroup.FormulaId = row["FORMAULAID"] == DBNull.Value ? -1 : Convert.ToInt32(row["FORMAULAID"]);
                    agroup.FactorTypeId = row["SAFETY_FACTOR_TYPE_ID"] == DBNull.Value ? -1 : Convert.ToInt32(row["SAFETY_FACTOR_TYPE_ID"]);
                    agroup.FactorTypeTable = row["THEMES_TABLE_NAME"] == DBNull.Value ? "" : Convert.ToString(row["THEMES_TABLE_NAME"]);
                    agroup.TableColums = row["THEMES_COLUMNS"] == DBNull.Value ? "" : Convert.ToString(row["THEMES_COLUMNS"]);
                    int paraIndex = 1;
                    while (!row.IsNull("Parameter" + paraIndex))
                    {
                        agroup.FormulaParams.Add(Convert.ToDouble(row["Parameter" + paraIndex]));
                        paraIndex++;
                    }
                    foreach (DataRow dr in item)
                    {
                        if (!dr.IsNull("CorrentSensorId"))
                            agroup.AddItem(new GroupItem
                            {
                                SensorId = Convert.ToInt32(dr["CorrentSensorId"]),
                                DTUId = Convert.ToUInt32(dr["DTU_ID"])
                            });
                    }

                    // 虚拟传感器的基础信息
                    var virtualSensor = new Sensor();
                    virtualSensor.SensorID = Convert.ToUInt32(row["SENSOR_ID"]);
                    virtualSensor.FormulaID = row.IsNull("FORMAULAID") ? 0 : Convert.ToUInt32(row["FORMAULAID"]);
                    virtualSensor.FactorType = Convert.ToUInt32(row["SAFETY_FACTOR_TYPE_ID"]);
                    virtualSensor.ProtocolType = row.IsNull("PROTOCOL_ID") ? 0 : Convert.ToUInt32(row["PROTOCOL_ID"]);
                    virtualSensor.FactorTypeTable = Convert.ToString(row["THEMES_TABLE_NAME"]);
                    virtualSensor.TableColums = Convert.ToString(row["THEMES_COLUMNS"]);
                    virtualSensor.StructId = Convert.ToUInt32(row["STRUCT_ID"]);
                    virtualSensor.SensorType = SensorType.Virtual;
                    agroup.VirtualSensor = virtualSensor;

                    virtualsens.Add(agroup);
                }
            }

            return virtualsens;
        }

        private IList<SensorGroup> QuerySensorGroups(uint dtuid)
        {
            var groupdts =
                (from t in
                     this._dbHelper.Query(string.Format("select * from [dbo].[T_DIM_GROUP_TYPE]")).Tables[0].AsEnumerable()
                 orderby t.Field<int>("GROUP_TYPE_ID")
                 select t.Field<string>("SENSOR_GROUP_TABLENAME")).ToList();

            var sqlstr = new StringBuilder();

            foreach (var groupdt in groupdts)
            {
                string sql = string.Format(@"
select sg.*,g.GROUP_TYPE_ID,s.DTU_ID from {0} sg,T_DIM_GROUP g,T_DIM_SENSOR s
where sg.GROUP_ID=g.GROUP_ID and sg.SENSOR_ID=s.SENSOR_ID and sg.GROUP_ID in(
select distinct(GROUP_ID) from {0} sg,T_DIM_SENSOR s
where s.SENSOR_ID=sg.SENSOR_ID and s.IsDeleted=0 and s.DTU_ID={1})", groupdt, dtuid);
                sqlstr.Append(sql).Append(";");
            }

            DataSet groupds = this._dbHelper.Query(sqlstr.ToString());
            var sengroups = new List<SensorGroup>();
            if (groupds == null || groupds.Tables.Count < 3) return sengroups; 
            foreach (IList<SensorGroup> groups in groupds.Tables.Cast<DataTable>().Select(this.QueryGroups).Where(groups => groups.Count > 0))
            {
                sengroups.AddRange(groups);
            }
            return sengroups;
        }

        /// <summary>
        /// 查询所有虚拟传感器组合和传感器分组信息
        /// </summary>
        /// <returns></returns>
        public IList<SensorGroup> QueryGroups()
        {
            var sensorgroups = new List<SensorGroup>();
            IList<SensorGroup> virtualsens = this.QueryVirtualSensors(); // 查询虚拟传感器
            if (virtualsens.Count > 0)
            {
                sensorgroups.AddRange(virtualsens);
            }
            IList<SensorGroup> groups = this.QuerySensorGroups();
            if (groups.Count > 0)
            {
                sensorgroups.AddRange(groups);
            }
            return sensorgroups;
        }

        /// <summary>
        /// 查询所有虚拟传感器组合信息
        /// </summary>
        /// <returns></returns>
        private IList<SensorGroup> QueryVirtualSensors()
        {
            IList<SensorGroup> virtualsens = new List<SensorGroup>();

            String virtualSensorQuerySql =
@"select s.SENSOR_ID,s.SAFETY_FACTOR_TYPE_ID,s.STRUCT_ID,ss.DTU_ID,p.FORMAULAID,p.PROTOCOL_ID,
f.Parameter1,f.Parameter2,f.Parameter3,f.Parameter4,f.Parameter5,f.Parameter6,f.Parameter7,c.CorrentSensorId,
sf.THEMES_TABLE_NAME,sf.THEMES_COLUMNS  from [T_DIM_SENSOR_CORRENT] c
left join T_DIM_SENSOR s on s.SENSOR_ID=c.SensorId
left join T_DIM_SENSOR_PRODUCT p on s.PRODUCT_SENSOR_ID = p.PRODUCT_ID
left join T_DIM_FORMULAID_SET f on c.SensorId=f.SENSOR_ID
left join T_DIM_SAFETY_FACTOR_TYPE sf on sf.SAFETY_FACTOR_TYPE_ID=s.SAFETY_FACTOR_TYPE_ID
left join T_DIM_SENSOR ss on ss.SENSOR_ID=c.CorrentSensorId
where c.SensorId in(
select distinct(c.SensorId) from T_DIM_SENSOR s,[T_DIM_SENSOR_CORRENT] c 
where s.SENSOR_ID=c.CorrentSensorId and s.IsDeleted=0)";

            DataTable table = this._dbHelper.Query(virtualSensorQuerySql).Tables[0];

            if (table.Rows.Count > 0)
            {
                var groups = from g in table.AsEnumerable()
                             group g by g.Field<int>("SENSOR_ID");
                foreach (var item in groups)
                {
                    if (!item.Any()) continue;
                    var agroup = new SensorGroup(item.Key, GroupType.VirtualSensor);
                    DataRow row = item.First();

                    // 虚拟传感器的分组信息(公式、参数、子集)
                    agroup.FormulaId = row["FORMAULAID"] == DBNull.Value ? -1 : Convert.ToInt32(row["FORMAULAID"]);
                    agroup.FactorTypeId = row["SAFETY_FACTOR_TYPE_ID"] == DBNull.Value ? -1 : Convert.ToInt32(row["SAFETY_FACTOR_TYPE_ID"]);
                    agroup.FactorTypeTable = row["THEMES_TABLE_NAME"] == DBNull.Value ? "" : Convert.ToString(row["THEMES_TABLE_NAME"]);
                    agroup.TableColums = row["THEMES_COLUMNS"] == DBNull.Value ? "" : Convert.ToString(row["THEMES_COLUMNS"]);
                    int paraIndex = 1;
                    while (!row.IsNull("Parameter" + paraIndex))
                    {
                        agroup.FormulaParams.Add(Convert.ToDouble(row["Parameter" + paraIndex]));
                        paraIndex++;
                    }
                    foreach (DataRow dr in item)
                    {
                        if (!dr.IsNull("CorrentSensorId"))
                            agroup.AddItem(new GroupItem
                            {
                                SensorId = Convert.ToInt32(dr["CorrentSensorId"]),
                                DTUId = Convert.ToUInt32(dr["DTU_ID"])
                            });
                    }

                    // 虚拟传感器的基础信息
                    var virtualSensor = new Sensor
                    {
                        SensorID = Convert.ToUInt32(row["SENSOR_ID"]),
                        FormulaID = row.IsNull("FORMAULAID") ? 0 : Convert.ToUInt32(row["FORMAULAID"]),
                        FactorType = Convert.ToUInt32(row["SAFETY_FACTOR_TYPE_ID"]),
                        ProtocolType = row.IsNull("PROTOCOL_ID") ? 0 : Convert.ToUInt32(row["PROTOCOL_ID"]),
                        FactorTypeTable = Convert.ToString(row["THEMES_TABLE_NAME"]),
                        TableColums = Convert.ToString(row["THEMES_COLUMNS"]),
                        StructId = Convert.ToUInt32(row["STRUCT_ID"]),
                        SensorType = SensorType.Virtual
                    };
                    agroup.VirtualSensor = virtualSensor;

                    virtualsens.Add(agroup);
                }
            }

            return virtualsens;
        }

        /// <summary>
        /// 查询所有分组信息
        /// </summary>
        /// <returns></returns>
        private IList<SensorGroup> QuerySensorGroups()
        {
            var groupdts =
                (from t in
                     this._dbHelper.Query(string.Format("select * from [dbo].[T_DIM_GROUP_TYPE]")).Tables[0].AsEnumerable()
                 orderby t.Field<int>("GROUP_TYPE_ID")
                 select t.Field<string>("SENSOR_GROUP_TABLENAME")).ToList();

            var sqlstr = new StringBuilder();

            foreach (var groupdt in groupdts)
            {
                string sql = string.Format(@"
select sg.*,g.GROUP_TYPE_ID,s.DTU_ID from {0} sg,T_DIM_GROUP g,T_DIM_SENSOR s
where sg.GROUP_ID=g.GROUP_ID and sg.SENSOR_ID=s.SENSOR_ID and sg.GROUP_ID in(
select distinct(GROUP_ID) from {0} sg,T_DIM_SENSOR s
where s.SENSOR_ID=sg.SENSOR_ID and s.IsDeleted=0)", groupdt);
                sqlstr.Append(sql).Append(";");
            }

            DataSet groupds = this._dbHelper.Query(sqlstr.ToString());
            var sengroups = new List<SensorGroup>();
            if (groupds == null || groupds.Tables.Count < 3) return sengroups;
            foreach (IList<SensorGroup> groups in groupds.Tables.Cast<DataTable>().Select(this.QueryGroups).Where(groups => groups.Count > 0))
            {
                sengroups.AddRange(groups);
            }
            return sengroups;
        }

        private IList<SensorGroup> QueryGroups(DataTable table)
        {
            IList<SensorGroup> grouplst = new List<SensorGroup>();
            if (table == null || table.Rows.Count <= 0) return grouplst;
            var groupType = (GroupType)table.AsEnumerable().First().Field<int>("GROUP_TYPE_ID");
            var groups = from g in table.AsEnumerable()
                         group g by g.Field<int>("GROUP_ID");
            foreach (var g in groups)
            {
                SensorGroup group;
                switch (groupType)
                {
                    case GroupType.Inclination:
                        group = new SensorGroup(g.Key, groupType);
                        foreach (DataRow row in g)
                        {
                            var gp = new GroupItem
                            {
                                SensorId = Convert.ToInt32(row["SENSOR_ID"]),
                                DTUId = Convert.ToUInt32(row["DTU_ID"])
                            };
                            gp.Paramters.Add("DEPTH", Convert.ToDouble(row["DEPTH"]));
                            group.Items.Add(gp);
                        }
                        grouplst.Add(group);
                        break;
                    case GroupType.Settlement:
                        group = new SensorGroup(g.Key, groupType);
                        foreach (DataRow row in g)
                        {
                            var gp = new GroupItem
                            {
                                SensorId = Convert.ToInt32(row["SENSOR_ID"]),
                                DTUId = Convert.ToUInt32(row["DTU_ID"])
                            };
                            gp.Paramters.Add("IsBase", Convert.ToByte(row["isJIZHUNDIAN"]));
                            group.Items.Add(gp);
                        }
                        grouplst.Add(group);
                        break;
                    case GroupType.SaturationLine:
                        group = new SensorGroup(g.Key, groupType);
                        foreach (DataRow row in g)
                        {
                            var gp = new GroupItem
                            {
                                SensorId = Convert.ToInt32(row["SENSOR_ID"]),
                                DTUId = Convert.ToUInt32(row["DTU_ID"])
                            };
                            gp.Paramters.Add("HEIGHT", Convert.ToDouble(row["HEIGHT"]));
                            group.Items.Add(gp);
                        }
                        grouplst.Add(group);
                        break;
                    default:
                        break;
                }
            }

            return grouplst;
        }
    }
}
