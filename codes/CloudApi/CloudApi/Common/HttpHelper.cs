namespace FreeSun.FS_SMISCloud.Server.CloudApi.Common
{
    using System.Net.Http.Headers;

    public static class HttpHelper
    {
        public static string AnalyzeUserAgent(HttpHeaderValueCollection<ProductInfoHeaderValue> userAgent)
        {
            string clientType;
            var ua = string.Join(",", userAgent);

            if (ua.Contains("Mac OS"))
            {
                if (ua.Contains("iPhone"))
                {
                    clientType = ua.Contains("Safari") ? "iPhone浏览器" : "iPhone客户端";
                }
                else if (ua.Contains("iPad"))
                {
                    clientType = ua.Contains("safari") ? "iPad浏览器" : "iPad客户端";
                }
                else
                {
                    clientType = "Mac OS桌面系统";
                }
            }
            else if (ua.Contains("Linux"))
            {
                if (ua.Contains("Android"))
                {
                    clientType = ua.Contains("Mobile") ? "Android手机" : "Android平板";
                }
                else
                {
                    clientType = "Linux桌面系统";
                }
            }
            else if (ua.Contains("Windows NT"))
            {
                if (ua.Contains("Windows Phone"))
                {
                    clientType = "Windows Phone手机";
                }
                else
                {
                    clientType = "Windows桌面系统";
                }
            }
            else
            {
                clientType = "未知设备";
            }
            
            return clientType;
        }
    }
}