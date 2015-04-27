#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="EnterConfig.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140909 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGDAC.Gprs.Cmd
{
    public class EnterConfig:ATCommand
    {
        public string ToATString()
        {
            return string.Format("***COMMIT CONFIG***");
        }
    }
}