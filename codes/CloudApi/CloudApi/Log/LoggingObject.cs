namespace FreeSun.FS_SMISCloud.Server.CloudApi.Log
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// 日志对象
    /// </summary>
    public class LoggingObject
    {
        public DateTime LogTime { get; set; }

        public string ClientIp { get; set; }

        public string UserAgent { get; set; }

        public string ClientType { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        public string Parameter { get; set; }

        public string ParameterShow { get; set; }

        public string Content { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public int StatusCode { get; set; }

        public decimal Duration { get; set; }

        public int? UserNo { get; set; }

        public string SessionId { get; set; }

        public bool IsVisible { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}