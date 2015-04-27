// --------------------------------------------------------------------------------------------
// <copyright file="Config.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2015 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20150329
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.NGET
{
    public static class GlobalConfig
    {
        static GlobalConfig()
        {
            FileScanInterval = 10000;
        }

        public static string ConnectionString { get; set; }

        public static string DataSourcePath { get; set; }

        public static string ErrorFilePath { get; set; }

        public static string ParsedFilePath { get; set; }

        public static int FileScanInterval { get; set; }
    }
}