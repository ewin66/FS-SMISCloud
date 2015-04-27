namespace NGDAC.Test.Tran
{
    using System;
    using System.Collections.Generic;

    using FS.DbHelper;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Tran;
    using FS.SMIS_Cloud.NGDAC.Tran.Db;
    using FS.SMIS_Cloud.NGDAC.Util;

    using NUnit.Framework;

    [TestFixture]
    public class SQLiteDacDataProviderTester
    {
        //private static string connstr = ".\\FSUSDB\\fsuscfg.db3,.\\FSUSDB\\FSUSDataValueDB.db3";
        private string path = @".\Tran\DbMapping_SQLite.xml";
        private static Sensor NewSensor(uint type)
        {
            return new Sensor
            {
                SensorID = 999,
                ProtocolType = (uint) type,
                ModuleNo = 99,
                ChannelNo = 12345,
                DtuID = 123,
                Name = "TEST",
            };
        }

        [Test]
        public void TestDataToSegment()
        {
            DbDacDataProvider p = new DbDacDataProvider();
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("sqlitedbcongxml", this.path);
            p.Init_Sqlite(args);
            var d = new SensorOriginalData
            {
                AcqTime = System.DateTime.Now,
                ModuleNo = 12,
                ChannelNo = 23,
                Type = ProtocolType.VibratingWire,
                Values = new double[] {1924, 12.340000d, 2.340000d}
            };
            byte[] buff = new byte[39];
            int writed = p.DataToSegment(d, buff, 0);
            Assert.AreEqual(39, writed);
            Assert.AreEqual(ProtocolType.VibratingWire, (uint)ValueHelper.GetShort(buff, 0));
            Assert.AreEqual(1924, ValueHelper.GetDouble(buff, 15));
            Assert.AreEqual(12.34f, ValueHelper.GetDouble(buff, 23));
            Assert.AreEqual(2.34f, ValueHelper.GetDouble(buff, 31));

            Assert.AreEqual(23, p.CalcDataLength(1));
            Assert.AreEqual(31, p.CalcDataLength(2));
            Assert.AreEqual(39, p.CalcDataLength(3));
            Assert.AreEqual(47, p.CalcDataLength(4));
            Assert.AreEqual(55, p.CalcDataLength(5));

            Console.WriteLine("buff={0}", ValueHelper.BytesToHexStr(buff));

        }

        [Test]
        public void TestSpliter()
        {
            DbDacDataProvider p = new DbDacDataProvider();
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("sqlitedbcongxml", this.path);
            p.Init_Sqlite(args);
            var d = new SensorOriginalData
            {
                AcqTime = System.DateTime.Now,
                ModuleNo = 12,
                ChannelNo = 23,
                Type = ProtocolType.VibratingWire,
                Values = new double[] {1924, 12.340000d, 2.340000d}
            };
            byte[] buff = new byte[39];
            int writed = p.DataToSegment(d, buff, 0);
            TranMsg[] msgs = p.Splite(buff, writed);
            Assert.AreEqual(1, msgs.Length);
            Console.WriteLine("MSG1={0}", ValueHelper.BytesToHexStr(msgs[0].Marshall()));
        }

        // 测试依赖: 表中有些数据.
        [Test]
        public void TestHasMoreData()
        {
            DbDacDataProvider p = new DbDacDataProvider();
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("sqlitedbcongxml", this.path);
            p.Init_Sqlite(args);
            // 5.
           // var cs = connstr.Split(',');
            LoadDbConfigXml lc = new LoadDbConfigXml(this.path);
            string[] sonstr=lc.GetSqlConnectionStrings("/config/databases");

            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.SQLite, sonstr[0]);
            sqlHelper.Query("update D_OriginalWindData set lastSyncTime = null");

            Assert.IsTrue(p.HasMoreData());
            Console.WriteLine("TotalReminder: {0} records.", p.Remainder);
            Assert.IsTrue(p.Remainder > 0);

            int len = 0;
            TranMsg[] msgs = p.NextPackages(out len);
            Assert.IsNotNull(msgs);
            byte[] buff = msgs[0].Data;
            Console.WriteLine(ValueHelper.BytesToHexStr(buff, 0, len, ""));
            // Record 4.
            Assert.IsTrue(len > 0);

            // Data Sent
            p.OnPackageSent();

            var ds =
                sqlHelper.Query("select count(ID) from D_OriginalWindData where lastSyncTime is null");
            Assert.AreEqual(0, Convert.ToInt32(ds.Tables[0].Rows[0][0]));
        }
    }
}
