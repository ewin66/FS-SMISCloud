using log4net;

namespace DataCenter.Task
{
    public class WorkThread
    {
        private static readonly object SyncRoot = new object();
        private static readonly ILog Log = LogManager.GetLogger(typeof(WorkThread));

        private static WorkThread workThread = null;
        private WorkThread()
        {
            
        }

        public static WorkThread GetWorkThreadInstance()
        {
            if (workThread == null)
            {
                lock (SyncRoot)
                {
                    return workThread = new WorkThread();
                }
            }

            return workThread;
        }

        private CollectThread collectThread = Task.CollectThread.CreateCollectThreadInstance();
        private readonly ResolveThread resolveThread = ResolveThread.GetResolveThread();
         
        public void StartService()
         {
             this.collectThread.StartService();
             this.resolveThread.StartSerive();
            MonitoringSensorData.GetInstance.Start();
         }

        public void StopService()
        {
            this.collectThread.StopService();
            this.resolveThread.StopService();
        }
    }


}