namespace FS.SMIS_Cloud.NGET.Storage.iSecureCloud
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;

    using FS.SMIS_Cloud.NGET.Consumer;
    using FS.SMIS_Cloud.NGET.Model;

    using log4net;

    public class SeclureCloudStorge : IDacTaskResultConsumer
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
                this._msDbAccessor = new MsDbAccessor(loadxml.GetSqlConnectionStrings());
                this._msDbAccessor.UpdateTables(loadxml.GeTableInfos());
                updateDbtimer.Elapsed += this.updateDbtimer_Elapsed;
                updateDbtimer.Start();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(" initialization MsDbAccessor is error {0}",ex.StackTrace));
            }
        }

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(List<SensorAcqResult> source)
        {
            var sensors = source.Select(s => s.Sensor.SensorID).ToArray();
            var senStr = string.Join(",", sensors);
            try
            {
                Log.InfoFormat("Sensors: [{0}] :start Storage....", senStr);
               int count= this._msDbAccessor.SaveDacResult(source);
               Log.InfoFormat("Sensors: [{0}]  :Storage successed {1} ", senStr, count);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Sensors: [{0}]  Storage ERROR:{1}", senStr, ex.Message);
                Log.ErrorFormat("Sensors: [{0}] Storage ErrorStackTrace:{1}", senStr, ex.StackTrace);
            }
        }

        void updateDbtimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var loadxml = new LoadDbConfigXml(Xmlpath);
                this._msDbAccessor.UpdateTables(loadxml.GeTableInfos());
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(" initialization MsDbAccessor is error {0}", ex.StackTrace));
            }
        }
        
    }
}
