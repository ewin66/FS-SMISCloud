// --------------------------------------------------------------------------------------------
// <copyright file="BasicService.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20140902
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.Service
{
    using log4net;
    using System;
    using System.Collections.Concurrent;
    using System.IO;

    public abstract class Service
    {
        class PullHandler
        {
            public MsgFilter filter;
            public MsgHandler handler;
        }
 
        private static ILog log = LogManager.GetLogger("Service");

        // delegate Declare.

        private ConcurrentBag<PullHandler> Pulled;

        private bool _Working = false;
        private bool _busReady = true;
        private string _myName; // myname.
        private IMessageBus MsgBus;
        public string SvrConfig { get; set; }

        public string ServiceName { get { return _myName; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svrName"></param>
        /// <param name="svrConfig"></param>
        /// <param name="busPath">为空表示当前目录. </param>
        public Service(string svrName, string svrConfig, string busPath=null)
        {
            this._myName = svrName;
            this.SvrConfig = svrConfig;
            this.Pulled = new ConcurrentBag<PullHandler>();
            
            // this.MsgBus = ServiceIocHelper.FindMessageBus(busConfig, "Service.NetMQ", "FS.Service.NetMQMessageBus");
            log.InfoFormat("Starting service [{0}]:\n\tdir: '{1}'\n\tsvc: '{2}'", svrName, busPath != null ? busPath : "current.", svrConfig);

            this.MsgBus = ServiceIocHelper.FindMessageBus(busPath);
            if (MsgBus == null)
            {
                _busReady = false;
                log.ErrorFormat("Can't initialize MessageBus from {0}", busPath);
            }
            else
            {
                // Delegate to pullers
                MsgBus.Pull((msg) =>
                {
                    bool msgmatched;
                    foreach (PullHandler h in Pulled)
                    {
                        if (h.filter != null)
                            msgmatched = h.filter(msg);
                        else
                            msgmatched = true;
                        if (msgmatched)
                            h.handler(msg);
                    }
                });
            }
        }

        public abstract void PowerOn();

        /// <summary>
        /// 启动服务
        /// </summary>        
        public void Start()
        {
            PowerOn();
            if (_busReady)
            {
                this.MsgBus.Start(SvrConfig);
                _Working = true;
            }
            else
                log.ErrorFormat("Msgbus is null.");
        }

        public bool IsWorking()
        {
            return _Working;
        }

        public void Stop()
        {
            if (_busReady)
            {
                this.MsgBus.Stop();
            }
            else
                log.ErrorFormat("Msgbus is null.");
            _Working = false;
        }

        public bool IsConnected(string service)
        {
            return true;
        }

        public void Push(string service, FsMessage msg, int timeoutmsec=100)
        {
            if (_busReady && msg != null)
                MsgBus.Push(service, msg.ToJson(), timeoutmsec);
            else
                log.ErrorFormat("Msgbus or message is null.");
        }
        
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="service">服务名称</param>
        /// <param name="request">消息</param>
        /// <param name="timeoutmsec">请求超时时间，单位:豪秒</param>
        /// <returns></returns>
        public FsMessage Request(string service, string request, int timeoutmsec)
        {
            if (_busReady && request != null)
            {
                string rsp = MsgBus.Request(service, request, timeoutmsec);
                return FsMessage.FromJson(rsp);
            }
            else
            {
                log.ErrorFormat("Msgbus or request is null.");
                return null;
            }
        }
        
        public void Response(ReqMsgHandler handler)
        {
            if (_busReady && handler != null)
            {
                MsgBus.Response(handler);
            }
            else
            {
                log.ErrorFormat("Msgbus or ReqMsgHandler is null.");
            }
        }

        public void Publish(string topic, FsMessage msg)
        {
            if (_busReady && msg != null)
            {
                MsgBus.Publish(topic, msg.ToJson());
            }
            else
            {
                log.ErrorFormat("Msgbus or msg is null.");
                return;
            }
        }

        public void Pull(MsgFilter _filter, MsgHandler _handler)
        {
            Pulled.Add(new PullHandler
            {
                filter = _filter,
                handler = _handler,
            });
        }

        public void Pull(MsgHandler _handler)
        {
            Pulled.Add(new PullHandler
            {
                filter = null,
                handler = _handler,
            });
        }

        public void Subscribe(string service, string topic, MsgHandler handler)
        {
            if (MsgBus != null)
            {
                MsgBus.Subscriber(service, topic, handler);
            }
        }

    }
 
}