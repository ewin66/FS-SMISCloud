namespace FS.SMIS_Cloud.NGDAC.Tran.Db
{
    using FS.DbHelper;

    public class DataSourseTableInfo
    {
        /// <summary>
        /// 数据库名
        /// </summary>
        public string DataBaseName { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        public string TableName { get; set; }

        public int DataCount { get; set; }

        public string[] Colums { get; set; }

        public string[] StandardFields { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public uint Type { get; set; }

        /// <summary>
        /// 过滤
        /// </summary>
        public string Filter { get; set; }
    }
}