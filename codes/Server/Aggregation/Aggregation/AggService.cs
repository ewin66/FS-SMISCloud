using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aggregation
{
    using Agg.DataPool;
    using Agg.Storage;

    using FS.Service;
    using log4net;

    public class AggService:Service
    {
        private const string MyName = "agg";

        private AggJobManage jobManage;
        private static readonly ILog Log = LogManager.GetLogger("AggService");
        public AggService(string svrConfig, string busPath)
            : base(MyName, svrConfig, busPath)
        {
            Pull(OnMessageReceived);
            this.Start();
        }

        ~AggService()
        {
            this.Stop();
            jobManage.StopWork();
        } 

        public override void PowerOn()
        {
            Log.Info("PowerOn");
            List<BaseAggConfig> configs = BaseAggConfig.Create(SeclureCloudDbHelper.Instance().Accessor.GetConfig());
            jobManage = new AggJobManage(configs);
            jobManage.StartWork();
            
        } 

        public void OnMessageReceived(string buff)
        {
            try
            {
                FsMessage msg = FsMessage.FromJson(buff);
                // real-time DAC: // body:{"dtu":1,"sensors":[17,20]}
                Log.InfoFormat("pull({0})", buff);
                if (msg != null && msg.Header.R == @"/agg/config/")
                {
                    jobManage.StopWork();
                    List<BaseAggConfig> configs = BaseAggConfig.Create(SeclureCloudDbHelper.Instance().Accessor.GetConfig());
                    jobManage.ReStart(configs);
                }
            }
            catch (Exception ce)
            {
                Log.ErrorFormat("err {0}!", ce.Message);
            }
        }
    }
}
