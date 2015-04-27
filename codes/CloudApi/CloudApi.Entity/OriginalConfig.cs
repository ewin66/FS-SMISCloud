namespace FreeSun.FS_SMISCloud.Server.CloudApi.Entity
{
    /// <summary>
    /// 因素配置
    /// </summary>
    public class OriginalConfig
    {
        /// <summary>
        /// 原始数据表编号
        /// </summary>
        public int? ProductId { get; set; }

        /// <summary>
        /// 数据库表名
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// 显示列数
        /// </summary>
        public int DisplayNumber { get; set; }

        /// <summary>
        /// 显示的列名
        /// </summary>
        public string[] Display { get; set; }

        /// <summary>
        /// 数据库数据列
        /// </summary>
        public string[] Columns { get; set; }

        /// <summary>
        /// 小数点位数
        /// </summary>
        public int[] DecimalDigits { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string[] Unit { get; set; }
    }
}
