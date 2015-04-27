// // --------------------------------------------------------------------------------------------
// // <copyright file="Structure.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20141020
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace ReportGeneratorService.DataModule
{
    public class Structure : MarshalByRefObject
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 结构物类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 结构物名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 施工方
        /// </summary>
        public string ConstructionCompany { get; set; }
    }
}