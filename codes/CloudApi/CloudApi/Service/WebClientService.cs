namespace FreeSun.FS_SMISCloud.Server.CloudApi.Service
{
    using System;
    using FS.Service;
    using log4net;

    public class WebClientService : Service
    {
        private static WebClientService _instance;
        private static ILog log = LogManager.GetLogger("WebClientService");

        public override void PowerOn()
        {
        }

        public static void TryInit(string binPath)
        {
            if (_instance == null)
            {
                _instance = new WebClientService(binPath);
                _instance.Start();
            }
        }

        public static WebClientService GetService()
        {
            return _instance;
        }

        private WebClientService(string binPath)
            : base("WebClient", binPath + "WebClientService.xml", binPath)
        {
            Pull(OnPulledFromET);
            //Response(OnResponsedFromET);
        }

        public static FsMessage GetMessageFromET(FsMessage msg)
        {
            return msg;
        }

        private static void OnPulledFromET(string buffer)
        {
            try
            {
                FsMessage msg = FsMessage.FromJson(buffer);
                log.DebugFormat("pull({0})", buffer);
                if (msg == null)
                {
                    log.Error("pull.OnPulledFromET, msg is NULL!");
                    return;
                }
                string url = msg.Header.R;
                if (url == "/et/dtu/instant/at") // DTU远程配置
                {
                    // 获取DTU远程配置消息 
                    GetMessageFromET(msg);
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("error: {0}", ex.Message);
            }
        }

        public static void SendToET(FsMessage msg)
        {
            if (_instance != null && msg != null)
            {
                try
                {
                    _instance.Push("et", msg);
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Send to et msg falied : msg: {0},error:{1}", msg.ToJson(), ex);
                }
                
            }
        }

        public static void SendToAgg(FsMessage msg)
        {
            if (_instance != null && msg != null)
            {
                try
                {
                    _instance.Push("agg", msg);
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Send to agg msg falied : msg: {0},error:{1}", msg.ToJson(), ex);
                }
            }
        }

        public static FsMessage RequestToET(FsMessage msg, int timeoutInMilliseconds)
        {
            if (_instance != null && msg != null)
            {
                return _instance.Request("et", msg.ToJson(), timeoutInMilliseconds);
            }
            else
            {
                return null;
            }
        }

        private void OnResponsedFromET(string buffer)
        {
            try
            {
                FsMessage msg = FsMessage.FromJson(buffer);
                log.DebugFormat("response({0})", buffer);
                if (msg == null)
                {
                    log.Error("response.OnResponsedFromET, msg is NULL!");
                    return;
                }
                string url = msg.Header.R;
                if (url == "/et/status/structs/abnormalSensorCount") // 从ET Response 结构物下异常传感器个数
                {
                    // 获取消息: 结构物下异常传感器个数
                    GetMessageFromET(msg);
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("error: {0}", ex.Message);
            }
        }

        public static void TryStop()
        {
            if (_instance != null)
            {
                try
                {
                    _instance.Stop();
                }
                catch (Exception e)
                {
                    log.ErrorFormat("web service stop failed", e);
                }
            }
        }
    }
}
