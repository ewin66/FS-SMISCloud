#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="TableInfo.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141127 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace Agg.Storage
{
    public class TableInfo
    {
        public TableInfo(string name,string colums)
        {
            this.TableName = name;
            this.ColumnNames = colums;
            this.ColumnNum = this.ColumnNames.Split(',').Length;
        }

        public int ColumnNum { get; private set; }

        public string TableName { get; private set; }

        /// <summary>
        /// 主题表列名称，以","分割
        /// </summary>
        public string ColumnNames { get; private set; }
    }    
}