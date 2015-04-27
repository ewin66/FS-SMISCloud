using FS.Service;
using log4net;

namespace FreeSun.FS_SMISCloud.Server.WarningSenderTest
{
    class WarningSourceSimulator : Service
    {
        private static ILog log = LogManager.GetLogger("WarningSourceSimulator");
        public WarningSourceSimulator()
            : base("WarningSenderTest", "WarningSenderTest.xml", System.AppDomain.CurrentDomain.BaseDirectory)
        {
        }

        public override void PowerOn()
        {
            log.Info("PowerOn");
        }
    }
}