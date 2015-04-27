using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportGeneratorService.DataModule
{
   
        public class FactorConfig
        {
            /// <summary>
            /// 子因素编号
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// 子因素中文名称
            /// </summary>
            public string NameCN { get; set; }

            /// <summary>
            /// 子因素英文名称
            /// </summary>
            public string NameEN { get; set; }

            /// <summary>
            /// 数据库表名
            /// </summary>
            public string Table { get; set; }

            /// <summary>
            /// 数据库数据列
            /// </summary>
            public string[] Columns { get; set; }

            /// <summary>
            /// 显示的列名
            /// </summary>
            public string[] Display { get; set; }

            /// <summary>
            /// 小数点位数
            /// </summary>
            public int[] DecimalPlaces { get; set; }

            /// <summary>
            /// 单位
            /// </summary>
            public string[] Unit { get; set; }

            /// <summary>
            /// 显示列数
            /// </summary>
            public int DisplayNumber { get; set; }
        
    }
}
