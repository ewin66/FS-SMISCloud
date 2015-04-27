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

namespace FS.SMIS_Cloud.NGET.DataCalc.Model
{
    using System.Collections.Generic;

    using FS.SMIS_Cloud.NGET.Model;

    public class SensorGroup
    {
        internal SensorGroup(int groupId, GroupType groupType)
        {
            this.GroupId = groupId;
            this.GroupType = groupType;
            this.Items = new List<GroupItem>();
            this.FormulaParams = new List<double>();
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
            this.Items.Add(item);
        }

        /// <summary>
        /// 清除组内数据信息
        /// </summary>
        public void ClearParams()
        {
            foreach (var groupItem in Items)
            {
                if (groupItem.Paramters.ContainsKey("value"))
                    groupItem.Paramters.Remove("value");
                if (groupItem.Paramters.ContainsKey("hasvalue"))
                    groupItem.Paramters.Remove("hasvalue");
            }
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
            if (this.Items != null && this.Items.Count > 1)
            {
                for (int i = 1; i < this.Items.Count; i++)
                {
                    if (this.Items[i - 1].DTUId != this.Items[i].DTUId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}