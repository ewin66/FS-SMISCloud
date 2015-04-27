#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="BaseSensorGroup.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System.Collections.Generic;
using System.Linq;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.DataCalc.Model
{
    public class SensorGroup
    {
        internal SensorGroup(int groupId, GroupType groupType)
        {
            this.GroupId = groupId;
            this.GroupType = groupType;
            Items = new List<GroupItem>();
            FormulaParams = new List<double>();
        }

        /// <summary>
        /// 组ID
        /// </summary>
        public int GroupId { get; private set; }

        /// <summary>
        /// 组类别
        /// </summary>
        public GroupType GroupType { get; private set; }

        /// <summary>
        /// 传感器组表名
        /// </summary>
        public string GroupTableName { get; private set; }

        /// <summary>
        /// 监测因素ID
        /// </summary>
        public int FactorTypeId { get; set; }

        /// <summary>
        /// 监测因素表
        /// </summary>
        public string FactorTypeTable { get; set; }

        /// <summary>
        /// 监测因素的列名
        /// </summary>
        public string TableColums { get; set; }

        /// <summary>
        /// 组内元素
        /// </summary>
        public IList<GroupItem> Items { get; private set; }

        /// <summary>
        /// 组的计算公式
        /// 分组类的计算不需要计算公式
        /// 虚拟类的计算由此公式进行计算
        /// </summary>
        public int FormulaId { get; set; }

        /// <summary>
        /// 组的计算公式中的参数
        /// </summary>
        public IList<double> FormulaParams { get; set; }

        /// <summary>
        /// 增加组内的传感器配置信息
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(GroupItem item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// 虚拟传感器信息(仅适用与虚拟传感器分组)
        /// </summary>
        public Sensor VirtualSensor { get; set; }

        /// <summary>
        /// 该分组中是否含多DTU任务
        /// </summary>
        /// <returns></returns>
        public bool HasMultiDtuJob()
        {
            var dtus = GetDtuIdList();
            return dtus.Count() > 1;
        }

        public IEnumerable<uint> GetDtuIdList()
        {
            var items = GetAllItems();
            return (from s in items
                    select s.DtuId).Distinct();
        }

        /// <summary>
        /// 获取所有子项(虚拟子项取出其中的实体子集)
        /// </summary>
        public IEnumerable<GroupItem> GetAllItems()
        {
            List<GroupItem> items = new List<GroupItem>();
            if (Items != null && Items.Count > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].VirtualGroup == null)
                        items.Add(Items[i]);
                    else
                        items.AddRange(Items[i].VirtualGroup.GetAllItems());
                }
            }
            return items;
        }

        /// <summary>
        /// 获取所有虚拟组内元素
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GroupItem> GetVirtualItems()
        {
            List<GroupItem> items = new List<GroupItem>();
            if (Items != null && Items.Count > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].VirtualGroup != null)
                        items.Add(Items[i]);
                }
            }
            return items;
        }

        public override string ToString()
        {
            return string.Format("[GROUPID:{0} TYPE:{1} ITEMCNT:{2}]", GroupId, GroupType, GetAllItems().Count());
        }
    }
}