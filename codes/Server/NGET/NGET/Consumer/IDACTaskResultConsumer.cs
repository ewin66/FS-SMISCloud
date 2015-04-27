﻿// --------------------------------------------------------------------------------------------
// <copyright file="IDACTaskResultHandler.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20141105
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.NGET.Consumer
{
    using System.Collections.Generic;

    using FS.SMIS_Cloud.NGET.Model;

    public interface IDacTaskResultConsumer
    {
        SensorType[] SensorTypeFilter { get; set; }

        void ProcessResult(List<SensorAcqResult> source);
    }
}