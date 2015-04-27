namespace FreeSun.FS_SMISCloud.Server.CloudApi.Common
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// JsonpFormatter
    /// </summary>
    public class JsonpMediaTypeFormatter : JsonMediaTypeFormatter
    {
        private string callbackQueryParameter;

        /// <summary>
        /// 构造器
        /// </summary>
        public JsonpMediaTypeFormatter()
        {
            this.SupportedMediaTypes.Add(DefaultMediaType);
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));

            this.MediaTypeMappings.Add(new UriPathExtensionMapping("jsonp", DefaultMediaType));
        }

        /// <summary>
        /// 回调查询参数
        /// </summary>
        public string CallbackQueryParameter
        {
            get { return this.callbackQueryParameter ?? "callback"; }
            set { this.callbackQueryParameter = value; }
        }

        /// <summary>
        /// 异步写入流
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="value">值</param>
        /// <param name="stream">流</param>
        /// <param name="content">内容</param>
        /// <param name="transportContext">transport上下文</param>
        /// <returns>异步任务</returns>
        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            string callback;

            if (this.IsJsonpRequest(out callback))
            {
                return Task.Factory.StartNew(() =>
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(callback + "(");
                    writer.Flush();

                    base.WriteToStreamAsync(type, value, stream, content, transportContext).Wait();

                    writer.Write(")");
                    writer.Flush();
                });
            }

            return base.WriteToStreamAsync(type, value, stream, content, transportContext);
        }

        /// <summary>
        /// 是否是Jsonp请求
        /// </summary>
        /// <param name="callback">回调参数</param>
        /// <returns>是否是jsonp请求</returns>
        private bool IsJsonpRequest(out string callback)
        {
            callback = null;

            if (HttpContext.Current.Request.HttpMethod != "GET")
            {
                return false;
            }

            callback = HttpContext.Current.Request.QueryString[this.CallbackQueryParameter];

            return !string.IsNullOrEmpty(callback);
        }
    }
}