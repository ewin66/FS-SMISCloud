﻿#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="PacketsToSend.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140611 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Model.Config
{
    using SqliteORM;

    [Table(Name = "CacheDataPacket")]
    public class PacketsToSend : TableBase<PacketsToSend>
    {
        [PrimaryKey(Name = "ID")]
        public long ID { get; set; }

        [Field(Name = "DataPacket")]
        public string DataPacket { get; set; }
    }
}