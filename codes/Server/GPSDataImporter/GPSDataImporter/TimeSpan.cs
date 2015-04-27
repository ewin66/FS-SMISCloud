// --------------------------------------------------------------------------------------------
// <copyright file="TimeSpan.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：时间段
// 
// 创建标识：20140527
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace GPSDataImporter
{
    using System;

    public class TimeSpan
    {
        public DateTime? SourceUpdateTime
        {
            get;
            set;
        }

        public DateTime? DestinationUpdateTime
        {
            get;
            set;
        }
    }
}