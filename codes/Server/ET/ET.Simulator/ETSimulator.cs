using log4net;


namespace ET.Simulator
{
    class ETSimulator : FS.Service.Service
    {
        private static ILog log = LogManager.GetLogger("ET.Sim");

        public ETSimulator()
            : base("et.sim", "ET.Sim.xml", System.AppDomain.CurrentDomain.BaseDirectory)
        {
        }

        public override void PowerOn(){
            log.Info("PowerOn");
        }

    }
}
