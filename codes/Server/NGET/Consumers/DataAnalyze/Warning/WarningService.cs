namespace FS.SMIS_Cloud.NGET.DataAnalyzer.Warning
{
    using FS.Service;

    class WarningService : Service
    {
        public WarningService(string svrName, string svrConfig, string busPath = null)
            : base(svrName, svrConfig, busPath)
        {
        }

        public override void PowerOn()
        {

        }
    }
}
