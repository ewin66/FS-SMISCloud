// --------------------------------------------------------------------------------------------
// <copyright file="NetMQMessageBus.cs" company="江苏飞尚安全监测咨询有限公司">
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

using log4net;
using NetMQ;
using NetMQ.Monitoring;
using NetMQ.zmq;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace FS.Service
{
    using System.Collections.Generic;
    using System.Text;

    public class NetMQMessageBus : IMessageBus
    {
        private static ILog log = LogManager.GetLogger("NetMqMsgBus");
        private CancellationTokenSource source_pp, source_ps, source_rr;
        private CancellationToken token_pp, token_ps, token_rr;
        private ServiceConfig config;
        // handler
        private MsgHandler pullMsgHandler;
        private SubMsgDelegate subMsgHandler;
        private ReqMsgHandler reqMsgHandler;
        private NetMQSocket puller = null, publisher = null, responser = null;

        // 推送至X，订阅X，向X请求
        private ConcurrentDictionary<string, Link> _push_links, _sub_links;
        private ConcurrentDictionary<string, ConcurrentDictionary<string, MsgHandler>> Subscribed;

        private ConcurrentDictionary<string, string> responseMsgs;

        private List<Task> tasks; 

        public bool IsConnected(string service)
        {
            return true;
        }

        public NetMQMessageBus()
        {
            _push_links = new ConcurrentDictionary<string, Link>();
            _sub_links = new ConcurrentDictionary<string, Link>();
            //_req_links = new ConcurrentDictionary<string, Link>();
            Subscribed = new ConcurrentDictionary<string, ConcurrentDictionary<string, MsgHandler>>();
            responseMsgs = new ConcurrentDictionary<string, string>();
            tasks = new List<Task>();
            source_pp = null;
            source_ps = null;
            source_rr = null;
        }

        public void Start(string configFile)
        {
            config = ServiceConfig.FromXml(configFile);
            log.DebugFormat("Start from {0}", configFile);

            if (config.PushPull != null)
            {
                log.InfoFormat("Starting listening for Pull: {0}", config.PushPull.ListenAddress);
                NetMQContext ctx1 = NetMQContext.Create();
                InitLinks(ctx1, _push_links, config.PushPull, ZmqSocketType.Push);
                puller = ctx1.CreatePullSocket();
                puller.ReceiveReady += this.PullerReceiveReady;
                puller.Bind(config.PushPull.ListenAddress);
                this.source_pp = new CancellationTokenSource();
                this.token_pp = this.source_pp.Token;
                Task newTask = Task.Factory.StartNew(this.OnPull, this.token_pp);
                this.tasks.Add(newTask);
            }
            if (config.PubSub != null)
            {
                log.InfoFormat("Starting listening for subscriber: {0}", config.PubSub.ListenAddress);
                NetMQContext ctx2 = NetMQContext.Create();
                InitLinks(ctx2, _sub_links, config.PubSub, ZmqSocketType.Sub);
                foreach (var link in _sub_links)
                {
                    link.Value.socket.ReceiveReady += SubOnReceiveReady;
                }
                publisher = ctx2.CreatePublisherSocket();
                publisher.Bind(config.PubSub.ListenAddress);
                this.source_ps = new CancellationTokenSource();
                this.token_ps = this.source_ps.Token;
                Task newTask = Task.Factory.StartNew(this.OnSubscribe, this.token_ps);
                this.tasks.Add(newTask);
            }
            if (config.ReqRep != null)
            {
                log.InfoFormat("Starting listening for Request: {0}", config.ReqRep.ListenAddress);
                NetMQContext ctx3 = NetMQContext.Create();
                //InitLinks(ctx3, _req_links, config.ReqRep, ZmqSocketType.Req); delete by xsw 2014-09-17
                responser = ctx3.CreateResponseSocket();
                responser.ReceiveReady += this.RepReceiveReady;
                responser.Bind(config.ReqRep.ListenAddress);
                this.source_rr = new CancellationTokenSource();
                this.token_rr = this.source_rr.Token;
                Task newTask = Task.Factory.StartNew(this.OnResponse, this.token_rr);
                this.tasks.Add(newTask);
            }
        }

        private void RepReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            NetMQSocket socket = e.Socket;
            bool hasMore;
            try
            {
                string msg = Encoding.UTF8.GetString(socket.Receive(SendReceiveOptions.DontWait, out hasMore));
                if (msg.Length > 0)
                {
                    string resp = reqMsgHandler(msg);
                    byte[] data = Encoding.UTF8.GetBytes(resp);
                    socket.Send(data, data.Length);
                }
            }
            catch (Exception ce)
            {
                Console.WriteLine("OnResponse Error: {0}", ce.Message);
                log.Error("Response Message error: ", ce);
            }
        }

        private void SubOnReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            NetMQSocket socket = e.Socket;
            bool hasMore;
            try
            {
                string msg = Encoding.UTF8.GetString(socket.Receive(SendReceiveOptions.DontWait, out hasMore));
                if (msg.Length > 0)
                {
                    string topic, body, serviceName;
                    if (SpliteMsg(msg, out topic, out body))
                    {
                        serviceName = GetServiceName(_sub_links, Encoding.UTF8.GetString(socket.Options.Identity));
                        MsgHandler h = this.GetTopicSubscriber(serviceName, topic);
                        if (h != null)
                            h(msg);
                    }
                }
            }
            catch (Exception ce)
            {
                Console.WriteLine("onsubscribe Error: {0}", ce.Message);
                log.Error("onsubscribe message error: ", ce);
            }
        }

        private void PullerReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            NetMQSocket socket = e.Socket;
            bool hasMore;
            try
            {
                string msg = Encoding.UTF8.GetString(socket.Receive(SendReceiveOptions.DontWait, out hasMore));
                if (msg.Length > 0 && this.pullMsgHandler != null)
                {
                    this.pullMsgHandler(msg);
                }
            }
            catch (Exception ce)
            {
                Console.WriteLine("OnPull Error: {0}", ce.Message);
                log.Error("Pulling Message error: ", ce);
            }
        }

        private void InitLinks(NetMQContext ctx, ConcurrentDictionary<string, Link> links, QueueLinks queue, ZmqSocketType socketType)
        {
            foreach (LinkAddress sa in queue.Links)
            {
                log.InfoFormat("Connecting to {0}({1})", sa.Name, sa.Address);
                Link link = new Link
                {
                    service = sa.Name,
                    address = sa.Address,
                    context = ctx,
                    socket = ctx.CreateSocket(socketType),
                };
                link.socket.Options.Identity = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()); 
                link.Connect();
                links[sa.Name] = link;
            }

        }

        // A link is a push-connect to a service
        class Link
        {
            public string service { get; set; }
            public string address { get; set; }
            public NetMQContext context { get; set; }
            public NetMQSocket socket { get; set; }
            public bool IsReady { get; set; }
            public void Connect()
            {
                IsReady = false;
                socket.Connect(address);
                socket.IgnoreErrors = true;
                // TODO
                //NetMQMonitor monitor = new NetMQMonitor(context, socket, "inproc://push" , SocketEvent.Accepted);
                //monitor.Accepted += (s, e) =>
                //{
                //    log.Info("monitor");
                //};
                //socket.Monitor("inproc://push", SocketEvent.All);
            }
        }

        private void CloseLinks(ConcurrentDictionary<string, Link> links)
        {
            foreach (var link in links)
            {
                /// 已在socket内部判断了状态，外部无需维护
                link.Value.socket.Close();
            }
        }

        public void Stop()
        {
            if (source_pp != null)
            {
                source_pp.Cancel();
            }

            if (source_ps != null)
            {
                source_ps.Cancel();
            }

            if (source_rr != null)
            {
                source_rr.Cancel();
            }

            
            Task.WaitAll(tasks.ToArray());
            CloseLinks(_push_links);   
            _push_links.Clear();
            CloseLinks(_sub_links);
            _sub_links.Clear();
           

            log.Info("Stop");
        }

        // on message published to me.
        private void OnSubscribe()
        {
            while (!this.token_ps.IsCancellationRequested)
            {
                foreach (Link link in _sub_links.Values)
                {
                    link.socket.Poll(TimeSpan.FromMilliseconds(100));
                }
                //try
                //{
                //    foreach (Link link in _sub_links.Values)
                //    {
                //        while (hasMore)
                //        {
                //            sb.Append(
                //                Encoding.UTF8.GetString(link.socket.Receive(SendReceiveOptions.DontWait, out hasMore)));
                //        }
                       
                //        string msg = sb.ToString();
                //        sb.Clear();
                //        if (msg.Length > 0)
                //        {
                //            string topic, body;
                //            if (SpliteMsg(msg, out topic, out body)) {
                //                MsgHandler h = GetTopicSubscriber(link.service, topic);
                //                if (h!=null) 
                //                    h(msg);
                //            }
                //        }
                //    }
                //}
                //catch (Exception ce)
                //{
                //    Console.WriteLine("onsubscribe Error: {0}", ce.Message);
                //    log.Error("onsubscribe message error: ", ce);
                //}
            }
            log.Debug("subscriber Closed");
            publisher.Close();
        }

        private bool SpliteMsg(string msg, out string topic, out string body)
        {
            int spacePos = msg.IndexOf(' ');
            if (spacePos > 0)
            {
                topic = msg.Substring(0, spacePos);
                body = msg.Substring(spacePos);
            }
            else
            {
                topic = null;
                body = null;
                log.ErrorFormat("Invalid pub message: {0}", msg);
            }
            return spacePos > 0;
        } 
        // on msg requested to me
        private void OnResponse()
        {
            StringBuilder sb = new StringBuilder();
            bool hasMore = true;
            while (!this.token_rr.IsCancellationRequested)
            {
                this.responser.Poll(TimeSpan.FromMilliseconds(100));
                //while (hasMore)
                //{
                //    sb.Append(Encoding.UTF8.GetString(this.responser.Receive(SendReceiveOptions.DontWait, out hasMore)));
                //}
                //string msg = sb.ToString();
                //sb.Clear();
                //try
                //{
                //    if (msg.Length > 0)
                //    {
                //        string resp = reqMsgHandler(msg);
                //        byte []data = Encoding.UTF8.GetBytes(msg);
                //        responser.Send(data, data.Length);
                //    }
                //}
                //catch (Exception ce)
                //{
                //    Console.WriteLine("OnResponse Error: {0}", ce.Message);
                //    log.Error("Response Message error: ", ce);
                //}
            }
            log.Debug("Responser Closed");
            responser.Close();
        }

        // on message pushed to me.
        private void OnPull()
        {
            while (!this.token_pp.IsCancellationRequested)
            {
                puller.Poll(TimeSpan.FromMilliseconds(100));
                //try
                //{
                //    while (hasMore)
                //    {
                //        sb.Append(Encoding.UTF8.GetString(this.puller.Receive(SendReceiveOptions.DontWait, out hasMore)));
                //    }

                //    string msg = sb.ToString();
                //    sb.Clear();
                //    if (msg.Length > 0)
                //    {
                //        pullMsgHandler(msg);
                //    }
                //}
                //catch (Exception ce)
                //{
                //    Console.WriteLine("OnPull Error: {0}", ce.Message);
                //    log.Error("Pulling Message error: ", ce);
                //}
            }
            log.Debug("Puller Closed");
            puller.Close();
        }

        Link GetLink(ConcurrentDictionary<string, Link> lnks, string service)
        {
            Link target = null;
            if (lnks.ContainsKey(service))
            {
                target = lnks[service];
            }
            return target;
        }

        private string GetServiceName(ConcurrentDictionary<string, Link> lnks, string linkId)
        {
            string name = string.Empty;
            string id;
            foreach (var link in lnks)
            {
                id = Encoding.UTF8.GetString(link.Value.socket.Options.Identity);
                if (id == linkId)
                {
                    name = link.Key;
                    break;
                }
            }
            return name;
        }

        // Push/Pull
        public void Push(string service, string msg, int timeoutmsec)
        {
            Link serv = GetLink(_push_links, service);
            if (serv != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(msg);
                serv.socket.Options.SendTimeout = TimeSpan.FromMilliseconds(timeoutmsec);
                serv.socket.Send(data,data.Length);
            }
            else
            {
                log.ErrorFormat("Service {0} Not found, can't Push.", service);
            }
        }

        public void Publish(string topic, string msg)
        {
            //publisher.Send(topic + " " + msg);
            publisher.Send(Encoding.UTF8.GetBytes(topic + " " + msg));
        }

        public string Request(string service, string msg, int timeoutmsec)
        {
            string resp = string.Empty;
            LinkAddress address = config.ReqRep.FindLinkAddress(service);
            if (address == null)
                return resp;

            bool pollResult = false;
            string requestId = Guid.NewGuid().ToString();
            using (NetMQContext context = NetMQContext.Create())
            {
                NetMQSocket client = context.CreateRequestSocket();
                client.Options.Linger = TimeSpan.Zero;
                client.Options.Identity = Encoding.UTF8.GetBytes(requestId);  
                client.Connect(address.Address);
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(msg);
                    client.Send(data);
                }
                catch (Exception)
                {
                    client.Disconnect(address.Address);
                    client.Dispose();
                    return resp;
                }
                
                client.ReceiveReady += ClientOnReceiveReady;
                pollResult = client.Poll(TimeSpan.FromMilliseconds(timeoutmsec));
                client.ReceiveReady -= ClientOnReceiveReady;
                client.Disconnect(address.Address);
                client.Dispose();
            }

            if (pollResult)
            {
                if (responseMsgs.ContainsKey(requestId))
                {
                    responseMsgs.TryRemove(requestId, out resp);
                }
            }

            return resp;
        }

        private void ClientOnReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            bool hasMore = true;
            byte[] replybuffer = e.Socket.Receive(out hasMore);
            string reply = Encoding.UTF8.GetString(replybuffer, 0, replybuffer.Length);
            string requestId = Encoding.UTF8.GetString(e.Socket.Options.Identity);
            responseMsgs.TryAdd(requestId, reply);
        }

        //private bool IsTimeout(long started, int timeout)
        //{
        //    return (System.DateTime.Now.Ticks - started) / 1000 / 10000 >= timeout;
        //}

        public void Response(ReqMsgHandler handler)
        {
            this.reqMsgHandler = handler;
        }

        public void Pull(MsgHandler handler)
        {
            this.pullMsgHandler = handler;
        }

        public void Subscriber(string service, string topic, MsgHandler handler)
        {
            ConcurrentDictionary<string, MsgHandler> topicSubs = null;
            if (Subscribed.ContainsKey(service)) {
                topicSubs = Subscribed[service];
            }else{
                Subscribed[service] = topicSubs = new ConcurrentDictionary<string, MsgHandler>();
            }
            topicSubs[topic] = handler;
            Link server = GetLink(_sub_links, service);
            if (server != null)
            {
                server.socket.Subscribe(Encoding.UTF8.GetBytes(topic));
            }
        }

        private MsgHandler GetTopicSubscriber(string service, string topic)
        {
            ConcurrentDictionary<string, MsgHandler> topicSubs = null;
            if (Subscribed.ContainsKey(service))
            {
                topicSubs = Subscribed[service];
                if (topicSubs.ContainsKey(topic))
                {
                    return topicSubs[topic];
                }
            }
            return null;
        }
    }
}