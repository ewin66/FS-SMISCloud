namespace NGDAC.Test.Task
{
    using FS.SMIS_Cloud.NGDAC.Accessor;
    using FS.SMIS_Cloud.NGDAC.Gprs;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Task;

    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class ATCommandTask
    {
        JObject[] cmds = new[]
            {
                JObject.Parse("{\"cmd\": \"setCount\",\"param\":{\"count\":2}}"),
                JObject.Parse("{\"cmd\": \"setIP1\",\"param\": {\"ip\":\"223.4.212.14\"}}"),
                JObject.Parse("{\"cmd\": \"setPort1\",\"param\":{\"port\":4050}}"),
                JObject.Parse("{\"cmd\": \"setIP2\",\"param\": {\"ip\":\"218.3.150.107\"}}"),
                JObject.Parse("{\"cmd\": \"setPort2\",\"param\":{\"port\":5001}}"),
                JObject.Parse("{\"cmd\": \"setMode\",\"param\": {\"mode\":\"TRNS\"}}"),
                JObject.Parse("{\"cmd\": \"setByteInterval\",\"param\":{\"byteInterval\":100}}"),
                JObject.Parse("{\"cmd\": \"setRetry\",\"param\":{\"retry\":3}}")
            };
        
        public void TestATCommandWorker()
        {
            IDtuServer _DacServer = new GprsDtuServer(5055);
            DACTaskManager dtm = new DACTaskManager(_DacServer, DbAccessorHelper.DbAccessor.QueryDtuNodes(), DbAccessorHelper.DbAccessor.GetUnfinishedTasks());
           // _DacServer.Start();
           // _DacServer.OnConnectStatusChanged += dtm.OnConnectionStatusChanged;
            string tid = "8d516ed8-568a-4228-bc53-2132c37cd7ce";
            uint dtu = 90;
          //  int taskId = dtm.ArrangeInstantTask(tid, dtu, cmds, null);
        }
    }
}