using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FS.DbHelper;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using FS.SMIS_Cloud.DAC.Model;
using SensorGroup = FS.SMIS_Cloud.DAC.DataCalc.Model.SensorGroup;

namespace FS.SMIS_Cloud.DAC.DataCalc.Accessor
{
    public class MsDbAccessorAdv
    {
        private readonly ISqlHelper _dbHelper = null;

        public MsDbAccessorAdv(String conn)
        {
            _dbHelper = SqlHelperFactory.Create(DbHelper.DbType.MSSQL, conn);
        }

        public IList<SensorGroup> QuerySensorGroupsByDtuid(uint dtuid)
        {
            var sensorgroups = new List<SensorGroup>();
            IList<SensorGroup> settlementWithVirtualGroup = QuerySettlementWithVirtualGroup(dtuid); // 组合分组中有虚拟分组的情况
            if (settlementWithVirtualGroup.Count > 0)
            {
                sensorgroups.AddRange(settlementWithVirtualGroup);
            }

            IList<SensorGroup> virtualsens = QueryVirtualSensors(dtuid); // 查询虚拟传感器
            if (virtualsens.Count > 0)
            {
                sensorgroups.AddRange(virtualsens);
            }

            IList<SensorGroup> groups = QuerySensorGroups(dtuid);
            if (groups.Count > 0)
            {
                sensorgroups.AddRange(groups);
            }
            return sensorgroups;
        }

        private IList<SensorGroup> QuerySettlementWithVirtualGroup(uint dtuid)
        {
            IList<SensorGroup> virtualsens = new List<SensorGroup>();

            String virtualSensorQuerySql = string.Format(@"
select cg.*,s.DTU_ID,corr.CorrentSensorId,
s.SAFETY_FACTOR_TYPE_ID,s.STRUCT_ID,sv.DTU_ID as VDTU_ID,p.FORMAULAID,p.PROTOCOL_ID,
f.Parameter1,f.Parameter2,f.Parameter3,f.Parameter4,f.Parameter5,f.Parameter6,f.Parameter7,
sf.THEMES_TABLE_NAME,sf.THEMES_COLUMNS from T_DIM_SENSOR_GROUP_CHENJIANG cg
left join T_DIM_SENSOR_CORRENT corr on corr.SensorId=cg.SENSOR_ID
left join T_DIM_SENSOR s on cg.SENSOR_ID=s.SENSOR_ID
left join T_DIM_SENSOR sv on sv.SENSOR_ID=corr.CorrentSensorId
left join T_DIM_SENSOR_PRODUCT p on s.PRODUCT_SENSOR_ID = p.PRODUCT_ID
left join T_DIM_FORMULAID_SET f on corr.SensorId=f.SENSOR_ID
left join T_DIM_SAFETY_FACTOR_TYPE sf on sf.SAFETY_FACTOR_TYPE_ID=s.SAFETY_FACTOR_TYPE_ID
where cg.GROUP_ID in
(
select A.GROUP_ID from 
(
select sg.* from T_DIM_SENSOR_GROUP_CHENJIANG sg
left join T_DIM_GROUP g on sg.GROUP_ID=g.GROUP_ID 
left join T_DIM_SENSOR s on sg.SENSOR_ID=s.SENSOR_ID 
left join T_DIM_SENSOR_CORRENT corr on corr.SensorId=sg.SENSOR_ID 
left join T_DIM_SENSOR sv on sv.SENSOR_ID=corr.CorrentSensorId
where 
(
sg.GROUP_ID in(
select distinct(GROUP_ID) from T_DIM_SENSOR_GROUP_CHENJIANG sg,T_DIM_SENSOR s
where s.SENSOR_ID=sg.SENSOR_ID and s.IsDeleted=0 and s.DTU_ID={0})
or sv.DTU_ID={0}
)
and corr.SensorId is not null 
) as A
group by GROUP_ID
)
", dtuid);

            DataTable table = _dbHelper.Query(virtualSensorQuerySql).Tables[0];

            if (table.Rows.Count > 0)
            {
                var groups = from g in table.AsEnumerable()
                             group g by g.Field<int>("GROUP_ID");
                foreach (var gitem in groups)
                {
                    if (!gitem.Any()) continue;
                    var agroup = new SensorGroup(gitem.Key, GroupType.Settlement);
                    var virtuals = from vg in gitem
                        group vg by vg.Field<int>("SENSOR_ID");
                    foreach (var vitem in virtuals)
                    {
                        if(!vitem.Any()) continue;

                        var gp = new GroupItem
                        {
                            SensorId = Convert.ToInt32(vitem.First()["SENSOR_ID"]),
                            DtuId = Convert.ToUInt32(vitem.First()["DTU_ID"])
                        };
                        gp.Paramters.Add("IsBase", Convert.ToByte(vitem.First()["isJIZHUNDIAN"]));

                        if (vitem.First()["CorrentSensorId"] != DBNull.Value)
                        {
                            gp.VirtualGroup = QueryVirtual(vitem);
                        }

                        agroup.Items.Add(gp);
                    }
                    virtualsens.Add(agroup);
                }
            }
            return virtualsens;
        }

        private IList<SensorGroup> QueryVirtualSensors(uint dtuid)
        {
            String virtualSensorQuerySql = string.Format(
@"
select s.SENSOR_ID,s.SAFETY_FACTOR_TYPE_ID,s.STRUCT_ID,s.DTU_ID,ss.DTU_ID as VDTU_ID,p.FORMAULAID,p.PROTOCOL_ID,
f.Parameter1,f.Parameter2,f.Parameter3,f.Parameter4,f.Parameter5,f.Parameter6,f.Parameter7,c.CorrentSensorId,
sf.THEMES_TABLE_NAME,sf.THEMES_COLUMNS  
from T_DIM_SENSOR_CORRENT c
left join T_DIM_SENSOR s on s.SENSOR_ID=c.SensorId
left join T_DIM_SENSOR_PRODUCT p on s.PRODUCT_SENSOR_ID = p.PRODUCT_ID
left join T_DIM_FORMULAID_SET f on c.SensorId=f.SENSOR_ID
left join T_DIM_SAFETY_FACTOR_TYPE sf on sf.SAFETY_FACTOR_TYPE_ID=s.SAFETY_FACTOR_TYPE_ID
left join T_DIM_SENSOR ss on ss.SENSOR_ID=c.CorrentSensorId
where c.SensorId in(
	select distinct(c.SensorId) from T_DIM_SENSOR s,[T_DIM_SENSOR_CORRENT] c 
	where s.SENSOR_ID=c.CorrentSensorId and s.IsDeleted=0 and s.DTU_ID={0}
)
and c.SensorId not in (
	select SENSOR_ID from T_DIM_SENSOR_GROUP_CHENJIANG
)
", dtuid);

            DataTable table = _dbHelper.Query(virtualSensorQuerySql).Tables[0];
            return QueryVirtuals(table);
        }

        /// <summary>
        /// 根据虚拟分组查询结果构造SensorGroup实例数组
        /// </summary>
        /// <param name="table">查询结果</param>
        /// <returns></returns>
        private IList<SensorGroup> QueryVirtuals(DataTable table)
        {
            IList<SensorGroup> virlst = new List<SensorGroup>();
            if (table == null || table.Rows.Count <= 0) return virlst;

            var groups = from g in table.AsEnumerable()
                         group g by g.Field<int>("SENSOR_ID");
            foreach (IGrouping<int,DataRow> item in groups)
            {
                SensorGroup agroup = null;
                try
                {
                    agroup = QueryVirtual(item);
                }
                catch (Exception ex)
                {
                    throw new Exception("构造虚拟传感器分组时异常," + ex.Message);
                }
                if (agroup != null)
                    virlst.Add(agroup);
            }
            return virlst;
        }

        private SensorGroup QueryVirtual(IGrouping<int, DataRow> item)
        {
            if (!item.Any()) return null;
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
                        DtuId = Convert.ToUInt32(dr["VDTU_ID"])
                    });
            }

            // 虚拟传感器的基础信息
            var virtualSensor = new Sensor
            {
                DtuID = Convert.ToUInt32(row["DTU_ID"]),
                SensorID = Convert.ToUInt32(row["SENSOR_ID"]),
                FormulaID = row.IsNull("FORMAULAID") ? 0 : Convert.ToUInt32(row["FORMAULAID"]),
                FactorType = Convert.ToUInt32(row["SAFETY_FACTOR_TYPE_ID"]),
                ProtocolType = row.IsNull("PROTOCOL_ID") ? 0 : Convert.ToUInt32(row["PROTOCOL_ID"]),
                FactorTypeTable = Convert.ToString(row["THEMES_TABLE_NAME"]),
                TableColums = Convert.ToString(row["THEMES_COLUMNS"]),
                SensorType = SensorType.Virtual,
                StructId = Convert.ToUInt32(row["STRUCT_ID"])
            };
            agroup.VirtualSensor = virtualSensor;

            return agroup;
        }

        private IList<SensorGroup> QuerySensorGroups(uint dtuid)
        {
            var groupdts =
                (from t in
                     _dbHelper.Query(string.Format("select * from [dbo].[T_DIM_GROUP_TYPE]")).Tables[0].AsEnumerable()
                 orderby t.Field<int>("GROUP_TYPE_ID")
                 select t.Field<string>("SENSOR_GROUP_TABLENAME")).ToList();

            var sqlstr = new StringBuilder();

            foreach (var groupdt in groupdts)
            {
                string sql = string.Format(@"
select sg.*,g.GROUP_TYPE_ID,s.DTU_ID from {0} sg,T_DIM_GROUP g,T_DIM_SENSOR s
where sg.GROUP_ID=g.GROUP_ID and sg.SENSOR_ID=s.SENSOR_ID and sg.GROUP_ID in(
select distinct(GROUP_ID) from {0} sg,T_DIM_SENSOR s
where s.SENSOR_ID=sg.SENSOR_ID and s.IsDeleted=0 and s.DTU_ID={1})
and sg.GROUP_ID not in(
select A.GROUP_ID from 
(
select sg.* from T_DIM_SENSOR_GROUP_CHENJIANG sg
left join T_DIM_GROUP g on sg.GROUP_ID=g.GROUP_ID 
left join T_DIM_SENSOR s on sg.SENSOR_ID=s.SENSOR_ID 
left join T_DIM_SENSOR_CORRENT corr on corr.SensorId=sg.SENSOR_ID 
left join T_DIM_SENSOR sv on sv.SENSOR_ID=corr.CorrentSensorId
where 
(
sg.GROUP_ID in(
select distinct(GROUP_ID) from T_DIM_SENSOR_GROUP_CHENJIANG sg,T_DIM_SENSOR s
where s.SENSOR_ID=sg.SENSOR_ID and s.IsDeleted=0 and s.DTU_ID={1})
or sv.DTU_ID={1}
)
and corr.SensorId is not null 
) as A
group by GROUP_ID
)
", groupdt, dtuid);
                sqlstr.Append(sql).Append(";");
            }

            DataSet groupds = _dbHelper.Query(sqlstr.ToString());
            var sengroups = new List<SensorGroup>();
            if (groupds == null || groupds.Tables.Count < 3) return sengroups;
            foreach (IList<SensorGroup> groups in groupds.Tables.Cast<DataTable>().Select(QueryGroups).Where(groups => groups.Count > 0))
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
                                DtuId = Convert.ToUInt32(row["DTU_ID"])
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
                                DtuId = Convert.ToUInt32(row["DTU_ID"])
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
                                DtuId = Convert.ToUInt32(row["DTU_ID"])
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
