#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="FileParam.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140722 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Model.Vibration
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct FileParamStruct
    {   
        /// <summary>
        /// 采样数据标识
        /// </summary>
         [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string bfType;                           // 采样数据标识

        /// <summary>
        ///  版本号
        /// </summary>
        public Int16 diVer;                             // 版本号

        /// <summary>
        /// 采样频率
        /// </summary>
        public double diSampleFreq;                     // 采样频率

        /// <summary>
        /// 采样点数
        /// </summary>
        public Int32 diSize;                            // 采样点数

        /// <summary>
        ///  灵敏度
        /// </summary>
        public double diSensitivity;                    // 灵敏度

        /// <summary>
        /// 传感器类型
        /// </summary>
        public byte diSensorType;                       // 传感器类型

        /// <summary>
        /// 测点号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string diTestPointNum;                   // 测点号

        /// <summary>
        /// 放大倍数
        /// </summary>
        public Int32 diMultiple;                        // 放大倍数

        /// <summary>
        /// 滤波频率
        /// </summary>
        public double diFilter;                         // 滤波频率

        /// <summary>
        /// 工程单位
        /// </summary>
        public byte diUnit;                             // 工程单位

        /// <summary>
        /// AD精度
        /// </summary>
        public Int16 diADBit;                           // AD精度

        /// <summary>
        /// 采样方式
        /// </summary>
        public byte diMethod;                           // 采样方式

        /// <summary>
        /// 备注
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string diRemark;                         // 备注
    }


}