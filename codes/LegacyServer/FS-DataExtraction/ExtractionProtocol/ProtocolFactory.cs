﻿#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ProtocolFactory.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140609 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace ExtractionProtocol
{
    using System;

    public class ProtocolFactory
    {
        public static IMakeDataTransportPacket CreateDataTransportPacket(TransportDataType type)
        {
            switch (type)
            {
                    case TransportDataType.General:
                    return new GeneralDataTransportProtocol();
                    case TransportDataType.BigData:
                    return new BigDataTransportProtocol();
                default:
                    throw new Exception("没有找到匹配的协议");
            }
        }
    }

    public enum TransportDataType
    {
        General,
        BigData
    }

}