namespace NGDAC.Test.Tran
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;

    using FS.SMIS_Cloud.NGDAC.Tran;
    using FS.SMIS_Cloud.NGDAC.Tran.Db;
    using FS.SMIS_Cloud.NGDAC.Tran.Vib;
    using FS.SMIS_Cloud.NGDAC.Util;

    using NUnit.Framework;

    [TestFixture]
    class TranDataSenderTester
    {

        //private static string connstr = ".\\FSUSDB\\fsuscfg.db3,.\\FSUSDB\\FSUSDataValueDB.db3";
        private string path = @".\Tran\DbMapping_SQLite.xml";

        // 测试方法: 手工启动本用例, 串口工具连接COM3匹配的虚拟口, 定时回送下列内容
        // fa 00 01 00 01 08 00 ff 32 30 31 32 30 30 34 39 7c af
        [Test]
        [Category("MANUAL")]
        public void TestSend()
        {
            // DAC.Test.Tran.TranDataSenderTester.TestSend
            Dictionary<string, string> args = new Dictionary<string, string>();
            args["PortName"] = "COM1"; //COM6-COM13
            args["BaudRate"] = "9600";
            args["Parity"] = Convert.ToString((int)Parity.None);
            args["DataBits"] = "8";
            args["StopBits"] = Convert.ToString((int)StopBits.One);
            args["ReadTimeOut"] = "1"; // 1ms
            args["sqlitedbcongxml"] = this.path; //SQLite用
            args["DataPath"] = "VibData";//振动用

            var provider1 = new DbDacDataProvider();
            ITranDataSendDelegator comSender = new ComDataSender
            {
                DtuCode = 20120049
            };
            comSender.Init(args);
            provider1.Init_Sqlite(args);
            ITranDataProvider provider2 = new VibFileDataProvider();
            provider2.Init(args);

            TranDataSender sender = new TranDataSender(comSender, provider1, provider2);

            sender.OnMessageSent += (TranMsg req, TranMsg resp) =>
            {
                int len = resp.LoadSize;
                Console.WriteLine("Data received. {0}: {1}", len, ValueHelper.BytesToHexStr(resp.Data));
            };
            sender.DoWork();

            Console.ReadLine();
            // SQLiteDataSpliter
        }
    }
}
