/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：ReportConfigDal.cs
// 功能描述：
// 
// 创建标识： 2014/10/21 13:41:30
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
// </summary>

//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGeneratorService.ReportModule
{
    public class ReportConfig
    {
        public int Id { get; set; }

        public int? OrgId { get; set; }

        public int? StructId { get; set; }

       // public int? FactorId { get; set; }

        public DateType DateType { get; set; }

        public string ReportName { get; set; }

        public string CreateInterval { get; set; }

        /// <summary>
        /// 报告是否需要确认。注：需要确认时报表状态为半完成，不需要确认时报表状态为完成
        /// </summary>
        public bool IsNeedConfirmed { get; set; }

        public string GetDataTime { get; set;}

        public override string ToString()
        {
            return string.Format(
                "报表文件名称:{0},时间类型:{1},组织id:{2},结构物id:{3}，生成周期:{4}",
                this.ReportName,
                //(DateType)this.DateType,
                this.OrgId,
                this.StructId,
                this.CreateInterval
               );
        }
    }
}
