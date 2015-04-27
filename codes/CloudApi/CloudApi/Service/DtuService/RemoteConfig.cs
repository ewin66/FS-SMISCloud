using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Service.DtuService
{
    /// <summary>
    /// DTU远程配置
    /// </summary>
    public class RemoteConfig
    {
        /// <summary>
        /// 中心IP数量 (目前数量为2个)
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip1 { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int? Port1 { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip2 { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int? Port2 { get; set; }

        /// <summary>
        /// DTU工作模式
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 封包间隔时间
        /// </summary>
        public int? ByteInterval { get; set; }

        /// <summary>
        /// 重连次数
        /// </summary>
        public int? Retry { get; set; }
    }
}