#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="IReloveTransData.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140619 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace DataCenter.WirelessCustomTransmission
{
    public interface IReloveTransData
    {
        void ReloveReceivedData(string dtuid,byte[] bytes);
    }
}