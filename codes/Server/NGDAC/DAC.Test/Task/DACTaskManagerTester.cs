namespace NGDAC.Test.Task
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using FS.SMIS_Cloud.NGDAC.Accessor;
    using FS.SMIS_Cloud.NGDAC.Accessor.MSSQL;
    using FS.SMIS_Cloud.NGDAC.Accessor.SQLite;
    using FS.SMIS_Cloud.NGDAC.Com;
    using FS.SMIS_Cloud.NGDAC.Gprs;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Task;
    using FS.SMIS_Cloud.NGDAC.Util;

    using log4net;

    using NUnit.Framework;

    [TestFixture]
    public class DACTaskManagerTester
    {
        private ILog Log = LogManager.GetLogger("Test");

        private static byte[] result = new byte[1024];
        static DtuClient client;
        static string AutoReply = "fc0100230000010000000000000000000000000000000000000000000000a4cf";
        static DACTaskManager tm;
        bool taskFinished = false;
        string mssqlConnStr = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004";
        string sqliteConStr = ".\\FSUSDB\\fsuscfg.db3,.\\FSUSDB\\FSUSDataValueDB.db3";

        public static void OnMsg(byte[] buff, int len)
        {
            Console.WriteLine("< {0}", ValueHelper.BytesToHexStr(buff, 0, len));
            Console.WriteLine("> {0}", AutoReply);
            client.Send(AutoReply);
        }

        [Test]
        [Category("MANUAL")]
        public void TestRemoteDtuRead()
        {
            this.Log.Debug("TestRemoteDtuRead");
            IList<DtuNode> _nodes = new List<DtuNode>();
            DtuNode dn1 = new DtuNode
            {
                DtuCode = "20140168",
                DacTimeout = 2, // 10s timeout
                DacInterval = 10, //30s interval
                DtuId = 999,
                NetworkType = NetworkType.gprs,
                Type = DtuType.Gprs
            };
            DtuNode dn2 = new DtuNode
            {
                DtuCode = "20140167",
                DacTimeout = 6, // 10s timeout
                DacInterval = 20, //30s interval
                DtuId = 998,
                NetworkType = NetworkType.gprs,
                Type = DtuType.Gprs
            };
            Sensor s02 = new Sensor
            {
                ProtocolType = (uint) ProtocolType.Pressure_MPM,
                DtuID = 999,
                SensorID = 1,
                ModuleNo = 2,
                ChannelNo = 0,
                FactorType = (uint) SafetyFactor.StressStrainPoreWaterPressure,
                Name="Pressure 02"
            };
            Sensor s27 = new Sensor
            {
                ProtocolType = (uint)ProtocolType.Pressure_MPM,
                DtuID = 999,
                SensorID = 2,
                ModuleNo = 27,
                ChannelNo = 0,
                FactorType = (uint)SafetyFactor.StressStrainPoreWaterPressure,
                Name = "Pressure 27"
            };

            Sensor s5135= new Sensor
            {
                ProtocolType = (uint)ProtocolType.VibratingWire_OLD, //
                DtuID = 998,
                SensorID = 3,
                ModuleNo = 5135,
                ChannelNo = 1,
                FactorType = (uint)SafetyFactor.Forcesteelbar,
                Name = "Pressure 5135"
            };

            _nodes.Add(dn1);
            //_nodes.Add(dn2);
            dn1.AddSensor(s02);
            dn1.AddSensor(s27);
            dn1.AddSensor(s5135);

            GprsDtuServer _server = new GprsDtuServer(5056);
            this.Log.Debug("Server started.");
            string sqlconn = "server=localhost;database=iSecureCloud;uid=tester;pwd=Fas123";
            // sqlconn = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004";
            this.Log.DebugFormat("Connect to db: {0}", sqlconn);            
            DbAccessorHelper.Init(new MsDbAccessor(sqlconn));

            _server.Start();

            tm = new DACTaskManager(_server, _nodes, null, DtuType.Gprs);
            tm.ArrangeTimedTask();
            Console.ReadLine();
        }


        [Test]
        [Category("MANUAL")]
        [Timeout(2)]
        public void TestMsSqlInstantTask()
        {
            // Client Simulator
            client = new DtuClient("127.0.0.1", 5055);
            client.OnReceived += OnMsg;
            GprsDtuServer _server = new GprsDtuServer(5055);
            _server.Start();
            client.Connect(20120049, "18651895100", "192.168.1.42");
            DbAccessorHelper.Init(new MsDbAccessor(this.mssqlConnStr));
            // Thread.Sleep(8000);
            string tid = new Guid().ToString();
            List<uint> sensors = new List<uint> { (uint) 17 };
            DACTask ut = new DACTask(tid, 1, sensors, TaskType.INSTANT, this.OnTaskFinished); //
            tm = new DACTaskManager(_server, DbAccessorHelper.DbAccessor.QueryDtuNodes(), DbAccessorHelper.DbAccessor.GetUnfinishedTasks());
           //  tm.DealDailyWork();
 //           Thread.Sleep(8000);
            int r = tm.ArrangeInstantTask(tid, 1, sensors, this.OnTaskFinished, false);
            System.Console.WriteLine("result = {0}", r);
        }


        [Test]
        [Category("MANUAL")]
        public void TestSQLiteRegularTask()
        {
            // Client Simulator
            var cs = this.sqliteConStr.Split(',');
            DbAccessorHelper.Init(new SQLiteDbAccessor(cs[0]));
            // Thread.Sleep(8000);
            ComDtuServer _server = new ComDtuServer();
            _server.Start();

            string tid = new Guid().ToString();
            List<uint> sensors = new List<uint> { (uint)17 };
            DACTask ut = new DACTask(tid, 1, sensors, TaskType.INSTANT, this.OnTaskFinished); //
            tm = new DACTaskManager(_server, DbAccessorHelper.DbAccessor.QueryDtuNodes(), null, DtuType.Com);
            DACTaskManager.SensorMatcher = (sensor) =>
            {
                return true;
            };
            //DACTaskManager.OnTimerDacFinished = (DACTaskResult r) =>
            //{
            //};
            
            tm.ArrangeTimedTask();
            Thread.Sleep(30000);
//            int r = tm.ArrangeInstantTask(tid, 1, sensors, this.OnTaskFinished, false);
//            System.Console.WriteLine("result = {0}", r);
        }

        public void OnTaskFinished(DACTaskResult r)
        {
            System.Console.WriteLine("Instance Task finished: {0}-{1}, elapsed={2}", r.ErrorCode, r.ErrorMsg, r.Elapsed);
            this.taskFinished = true;
           // tm.Stop();
           // client.Close();
        }

        public void DoWork()
        {
            while (!this.taskFinished)
            {
                Thread.Sleep(100);
            }
        }
    }
}
