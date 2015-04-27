#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="DACErrorCode.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150204 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FS.SMIS_Cloud.DAC.Model
{
    public class DacErrorCode
    {
        public int ErrorCode { get; set; } 
        public string ErrorNameUs { get; set; }
        public string ErrorDescription { get; set; }
    }
}