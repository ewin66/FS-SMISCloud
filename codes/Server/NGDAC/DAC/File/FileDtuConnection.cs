// --------------------------------------------------------------------------------------------
// <copyright file="FileDtuConnection.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20141023
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.NGDAC.File
{
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;

    public class FileDtuConnection: IDtuConnection
    {
        public FileDtuConnection(DtuNode dtuNode)
        {
            this.DtuID = dtuNode.DtuCode;
            this.Status = WorkingStatus.IDLE;
            this.FilePath = dtuNode.GetProperty("param1").ToString();
            this.IsOnline = this.FilePath != null && (System.IO.File.Exists(this.FilePath) || System.IO.Directory.Exists(this.FilePath));
        }

        public string DtuID { get; private set; }
        public bool IsOnline { get; private set; }
        public WorkingStatus Status { get; private set; }

        public string FilePath { get; set; }

        public bool IsAvaliable()
        {
            return WorkingStatus.IDLE == this.Status && this.IsOnline;
        }

        public bool Connect()
        {
            return this.FilePath != null && (System.IO.File.Exists(this.FilePath) || System.IO.Directory.Exists(this.FilePath));
        }

        public void Disconnect()
        {
            
        }

        public DtuMsg Ssend(byte[] buffer, int timeout)
        {
            throw new System.NotImplementedException();
        }

        public bool Asend(byte[] buff)
        {
            throw new System.NotImplementedException();
        }
    }
}