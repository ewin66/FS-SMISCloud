// // --------------------------------------------------------------------------------------------
// // <copyright file="Factor.cs" company="江苏飞尚安全监测咨询有限公司">
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

namespace ReportGeneratorService.DataModule
{
    public class Factor : MarshalByRefObject
    {
        /// <summary>
        /// 因素编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 因素名称
        /// </summary>
        public string NameCN { get; set; }
        public string NameEN { get; set; }
        /// <summary>
        /// 所属主题编号
        /// </summary>
        public int ThemeId { get; set; }

        /// <summary>
        /// 所属主题名称
        /// </summary>
        public string ThemeName { get; set; }

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

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            Factor key = obj as Factor;
            if (key == null) return false;

            return key.Id == this.Id;
        }

    }
}