#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="ISaveAttask.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：lonwin lonwin ling20141019
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FS.SMIS_Cloud.NGDAC.Accessor.MSSQL
{
    using FS.SMIS_Cloud.NGDAC.Task;

    public interface ISaveAttask
    {
        // 仅GPRS通信使用
        int SaveInstantTask(ATTask task);

        int UpdateInstantTask(Gprs.Cmd.ExecuteResult result); 
    }
}