#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="MsDbAccessor.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141111 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

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
    public class MsDbAccessor
    {
        private readonly ISqlHelper _dbHelper = null;

        public MsDbAccessor(String connstr)
        {
            _dbHelper = SqlHelperFactory.Create(DbHelper.DbType.MSSQL, connstr);
        }

        public IList<SensorGroup> QuerySensorGroupsByDtuid(uint dtuid)
        {
            var sensorgroups = new List<SensorGroup>(); 
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

        private IList<SensorGroup> QueryVirtualSensors(uint dtuid)
        {
            IList<SensorGroup> virtualsens = new List<SensorGroup>();

            String virtualSensorQuerySql = string.Format(
@"select s.SENSOR_ID,s.SAFETY_FACTOR_TYPE_ID,p.FORMAULAID,p.PROTOCOL_ID,
f.Parameter1,f.Parameter2,f.Parameter3,f.Parameter4,f.Parameter5,f.Parameter6,f.Parameter7,c.CorrentSensorId,
sf.THEMES_TABLE_NAME,sf.THEMES_COLUMNS 
from [dbo].[T_DIM_SENSOR] s 
left join [dbo].[T_DIM_SENSOR_PRODUCT] p on s.PRODUCT_SENSOR_ID = p.PRODUCT_ID 
left join [dbo].[T_DIM_FORMULAID_SET] f on s.SENSOR_ID=f.SENSOR_ID 
left join [dbo].[T_DIM_SENSOR_CORRENT] c on c.SensorId=s.SENSOR_ID
left join [dbo].[T_DIM_SAFETY_FACTOR_TYPE] sf on sf.SAFETY_FACTOR_TYPE_ID=s.SAFETY_FACTOR_TYPE_ID
where s.IsDeleted=0 and s.DTU_ID={0} and s.Identification=2", dtuid);

            DataTable table = _dbHelper.Query(virtualSensorQuerySql).Tables[0];

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
                    agroup.FormulaId = Convert.ToInt32(row["FORMAULAID"]);
                    agroup.FactorTypeId = Convert.ToInt32(row["SAFETY_FACTOR_TYPE_ID"]);
                    agroup.FactorTypeTable = Convert.ToString(row["THEMES_TABLE_NAME"]);
                    agroup.TableColums = Convert.ToString(row["THEMES_COLUMNS"]);
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
                                SensorId = Convert.ToInt32(dr["CorrentSensorId"])
                            });
                    }

                    // 虚拟传感器的基础信息
                    var virtualSensor = new Sensor();
                    virtualSensor.DtuID = dtuid;
                    virtualSensor.SensorID = Convert.ToUInt32(row["SENSOR_ID"]);
                    virtualSensor.FormulaID = row.IsNull("FORMAULAID") ? 0 : Convert.ToUInt32(row["FORMAULAID"]);
                    virtualSensor.FactorType = Convert.ToUInt32(row["SAFETY_FACTOR_TYPE_ID"]);
                    virtualSensor.ProtocolType = row.IsNull("PROTOCOL_ID") ? 0 : Convert.ToUInt32(row["PROTOCOL_ID"]);
                    virtualSensor.FactorTypeTable = Convert.ToString(row["THEMES_TABLE_NAME"]);
                    virtualSensor.TableColums = Convert.ToString(row["THEMES_COLUMNS"]);
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
                    _dbHelper.Query(string.Format("select * from [dbo].[T_DIM_GROUP_TYPE]")).Tables[0].AsEnumerable()
                    orderby t.Field<int>("GROUP_TYPE_ID")
                    select t.Field<string>("SENSOR_GROUP_TABLENAME")).ToList();

            var sqlstr = new StringBuilder();
            foreach (var groupdt in groupdts)
            {
                string sql = string.Format(@"
select sg.* ,
       g.GROUP_TYPE_ID
from {0} sg, [dbo].[T_DIM_SENSOR] s,T_DIM_GROUP g
where s.IsDeleted = 0 and s.SENSOR_ID = sg.SENSOR_ID and DTU_ID={1} and sg.GROUP_ID=g.GROUP_ID
order by sg.GROUP_ID,SENSOR_GROUP_ID
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
                                SensorId = Convert.ToInt32(row["SENSOR_ID"])
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
                                SensorId = Convert.ToInt32(row["SENSOR_ID"])
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
                                SensorId = Convert.ToInt32(row["SENSOR_ID"])
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
        
        /// <summary>
        /// 获取虚拟传感器计算需要的传感器Id
        /// </summary>
        /// <returns></returns>
        public List<int> GetEntrySensors()
        {
            DataSet culsends =
                _dbHelper.Query(
                    @"SELECT DISTINCT [CorrentSensorId] FROM [dbo].[T_DIM_SENSOR_CORRENT] where CorrentSensorId is not null order by CorrentSensorId");
            DataTable culcsendt = culsends.Tables[0];
            var senlst = (from s in culcsendt.AsEnumerable()
                          orderby s.Field<int>("CorrentSensorId")
                          select s.Field<int>("CorrentSensorId")).ToList();
            return senlst;
        }

    }
}