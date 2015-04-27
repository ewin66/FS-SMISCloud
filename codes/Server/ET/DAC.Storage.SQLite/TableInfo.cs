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
namespace FS.SMIS_Cloud.DAC.Storage.SQLite
{
    public class TableInfo
    {
        public TableInfo(string name,string colums)
        {
            this.TableName = name;
            this.Colums = colums;
        }

        public string TableName { get; private set; }

        public string Colums { get; private set; }
    }    
}