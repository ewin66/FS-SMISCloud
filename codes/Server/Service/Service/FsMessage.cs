// --------------------------------------------------------------------------------------------
// <copyright file="FsMessage.cs" company="江苏飞尚安全监测咨询有限公司">
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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class FsMessage : IDisposable
    {

        public const int ERR_NOT_CONNECT = 1000;
        public const int ERR_TIMEOUT = 1001;
        public const int ERR_FORMAT =  1002;
        public const int ERR_NO_RESP = 1003;

        /// <summary>
        /// 消息头
        /// </summary>
        public FsMessageHeader Header { get; set; }

        public FsMessage()
        {
            this.Header = new FsMessageHeader();
        }
        /// <summary>
        /// 消息体
        /// </summary>
        public object Body { get; set; }

        public static FsMessage FromJson(string str)
        {
            return (FsMessage)JsonConvert.DeserializeObject<FsMessage>(str);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void Dispose()
        {
        }

        // get after FromJson.
        public T BodyValue<T>(string key)
        {
            JObject jo = (JObject)Body;
            if (jo != null)
            {
                JToken jt;
                if (jo.TryGetValue(key, out jt))
                {
                    return jt.Value<T>();
                }
            }
            return default(T);
        }

        // get after FromJson.
        public T[] BodyValues<T>(string key)
        {
            JObject jo = (JObject)Body;
            if (jo != null)
            {
                JToken jt;
                if (jo.TryGetValue(key, out jt))
                {
                    IEnumerable<T> vs = jt.Values<T>();
                    List<T> va = new List<T>();
                    if (vs != null)
                    {
                        foreach (T t in vs)
                        {
                            va.Add(t);
                        }
                    }
                    return va.ToArray();
                }
            }
            return default(T[]);
        }

        internal byte[] GetBytes()
        {
            string json=JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }

        internal static FsMessage FromJson(byte[] buff)
        {
            string json = Encoding.UTF8.GetString(buff);
            return (FsMessage)JsonConvert.DeserializeObject<FsMessage>(json);
        }
    }

    public class FsMessageHeader:IDisposable
    {
        /// <summary>
        /// 动 作: POST/GET/PUT/DELETE 四种, 遵 从 RESTful格式;"
        /// </summary>
        public string A { get; set; }

        /// <summary>
        /// 发送者服务名称
        /// </summary>
        public string S { get; set; }

        /// <summary>
        /// 目的服务名称
        /// </summary>
        public string D { get; set; }

        /// <summary>
        /// 符合RESTful格式的Url请求
        /// </summary>
        public string R { get; set; }

        /// <summary>
        /// 消息编号
        /// </summary>
        public Guid U { get; set; }

        /// <summary>
        /// 消息长度
        /// </summary>
        public int L { get; set; }

        /// <summary>
        /// 鉴权Token
        /// </summary>
        public Guid T { get; set; }

        /// <summary>
        /// 错误码， =0标识成功。
        /// </summary>
        public int E { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string M { get; set; }

        public void Dispose()
        {
        }
    }
 
}