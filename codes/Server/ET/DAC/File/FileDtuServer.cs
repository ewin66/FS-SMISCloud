// --------------------------------------------------------------------------------------------
// <copyright file="FileDtuServer.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述:
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
namespace FS.SMIS_Cloud.DAC.File
{
    using System.Collections.Concurrent;
    using Model;
    using Node;

    public class FileDtuServer : IDtuServer
    {
        private ConcurrentDictionary<string, FileDtuConnection> _connectPool = new ConcurrentDictionary<string, FileDtuConnection>();

        public ConnectStatusEventHandler OnConnectStatusChanged { get; set; }

        public IDtuConnection GetConnection(DtuNode node)
        {
            FileDtuConnection fc = null;
            string dtuId = node.DtuCode;
            if (!_connectPool.ContainsKey(dtuId))
            {
                fc = new FileDtuConnection(node);
                _connectPool[dtuId] = fc;
            }
            else
            {
                fc = _connectPool[dtuId];
            }
            return fc;
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }

        public bool Send(string dtuid, byte[] buffer)
        {
            return true;
        }

        public bool Disconnect(object dtu)
        {
            FileDtuConnection fc = GetConnection(dtu as DtuNode) as FileDtuConnection;
            if (OnConnectStatusChanged != null)
            {
                OnConnectStatusChanged(fc, WorkingStatus.NA, WorkingStatus.IDLE);
            }
            return true;
        }
    }
}