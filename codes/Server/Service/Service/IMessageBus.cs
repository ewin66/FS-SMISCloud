// --------------------------------------------------------------------------------------------
// <copyright file="IMessageBus.cs" company="江苏飞尚安全监测咨询有限公司">
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
    public delegate bool MsgFilter(string msg);
    public delegate void MsgHandler(string msg);
    public delegate void SubMsgDelegate(string topic, string msg);
    public delegate string ReqMsgHandler(string msg);

    public interface IMessageBus
    {
       
        void Start(string cfgFileName);//, IMessageHandler handler);
        void Stop();

        void Push(string service, string msg, int timeoutmsec);
        void Pull(MsgHandler handler);

        void Publish(string topic, string msg);
        void Subscriber(string service, string topic, MsgHandler handler);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service">服务名称</param>
        /// <param name="msg">消息</param>
        /// <param name="timeOutInSeconds">超时时间，单位:秒</param>
        /// <returns>请求的响应</returns>
        string Request(string service, string msg, int timeOutInSeconds); // Req/Resp
        void Response(ReqMsgHandler handler);

    }
}