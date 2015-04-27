#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorInfo.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace DataCenter.Model
{
    public class SensorInfo
    {
        public int StructureId { get; set; }

        public string RemoteDtuNumber { get; set; }

        public int ProductTypeKey { get; set; }

        public string DaiModuleNo { get; set; }

        public int SensorId { get; set; }

        public string UniqueSign { get; set; }

        public int DaiChannelNumber { get; set; }

        public int Formaulaid { get; set; }

        public int SafetyFactorTypeId { get; set; }

        public int ProtocolCode { get; set; }

        public double Parameter1 { get; set; }

        public double Parameter2 { get; set; }

        public double Parameter3 { get; set; }

        public double Parameter4 { get; set; }

        public double Parameter5 { get; set; }

        public double Parameter6 { get; set; }

        public double Parameter7 { get; set; }
    }
}