using System;
using System.Timers;
using FS.SMIS_Cloud.DAC.Consumer;
using FS.SMIS_Cloud.DAC.Task;
using log4net;

namespace FS.SMIS_Cloud.DAC.Storage.iSecureCloud
{
    using Model;

    public class SeclureCloudStorge : IDACTaskResultConsumer
    {
        private readonly MsDbAccessor _msDbAccessor;
        private const string Xmlpath = ".\\ThemeTables_iSecureCloud.xml";
        private static readonly ILog Log = LogManager.GetLogger("SeclureCloudStorge");
        private const int Interval = 1000 * 60 * 60; // 一个小时

        public SeclureCloudStorge()
        {
            var loadxml = new LoadDbConfigXml(Xmlpath);
            var updateDbtimer = new Timer(Interval);
            try
            {
                _msDbAccessor = new MsDbAccessor(loadxml.GetSqlConnectionStrings());
                _msDbAccessor.UpdateTables(loadxml.GeTableInfos());
                updateDbtimer.Elapsed += updateDbtimer_Elapsed;
                updateDbtimer.Start();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(" initialization MsDbAccessor is error {0}",ex.StackTrace));
            }
        }

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(DACTaskResult source)
        {
            try
            {
               Log.InfoFormat("Dtu {0}-{1} :start Storage....",source.Task.DtuID,source.DtuCode);
               int count= _msDbAccessor.SaveDacResult(source);
               Log.InfoFormat("Dtu {0}-{1}  :Storage successed {2} ", source.Task.DtuID, source.DtuCode, count);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat(" Dtu {0}-{1}  Storage ERROR:{2}", source.Task.DtuID, source.DtuCode, ex.Message);
                Log.ErrorFormat(" Dtu {0}-{1}  Storage ErrorStackTrace:{2}", source.Task.DtuID, source.DtuCode, ex.StackTrace);
            }
        }

        void updateDbtimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var loadxml = new LoadDbConfigXml(Xmlpath);
                _msDbAccessor.UpdateTables(loadxml.GeTableInfos());
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(" initialization MsDbAccessor is error {0}", ex.StackTrace));
            }
        }
        
    }
}
