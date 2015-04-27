#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="CheckType.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述： 采集时存放发送数据包，和返回的数据包
// 
//  创建标识：20140227 created by Win
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace ET.Common.Check
{
    using System.ComponentModel;

    /// <summary>
    /// The check type.
    /// </summary>
    public enum CheckType
   {
       /// <summary>
       /// The none.
       /// </summary>
       [Description("无校验")]
       NONE = 1,

       /// <summary>
       /// The cr c 8.
       /// </summary>
       [Description("CRC8校验")]
       CRC8,

       /// <summary>
       /// The cr c 16 high byte first.
       /// </summary>
       [Description("CRC16校验高字节在前")]
       CRC16HighByteFirst,

       /// <summary>
       /// The cr c 16 low byte first.
       /// </summary>
       [Description("CRC16校验低字节在前")]
       CRC16LowByteFirst,

       /// <summary>
       /// The or sum.
       /// </summary>
       [Description("加总异或")]
       XorSum,

       /// <summary>
       /// The plus sum.
       /// </summary>
       [Description("加总和")]
       PlusSum,

        /// <summary>
        /// 异或
        /// </summary>
        [Description("异或")]
        Xor
   }
}
