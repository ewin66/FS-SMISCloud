using System;
using System.Collections.Generic;
using FS.SMIS_Cloud.DAC.Accessor;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Tran;
using FS.SMIS_Cloud.DAC.Tran.Db;
using FS.SMIS_Cloud.DAC.Util;
using NUnit.Framework;

namespace DAC.Test.Tran
{
    using FS.DbHelper;

    [TestFixture]
    public class DbDacDataProviderTester
     {
         private string path = ".\\Tran\\DbMapping.xml";
        
         [Test]
         public void TestDataToSegment()
         {
             DbDacDataProvider dp=new DbDacDataProvider();
             Dictionary<string,string> args =new Dictionary<string, string>();
             args.Add("dbcongxml", path);
             dp.Init(args);

             var d = new SensorOriginalData
             {
                 AcqTime = DateTime.Now,
                 SID = 108,
                 Type = ProtocolType.GPS_ZHD,
                 Values = new double[]
                 {
                     2.134, 1.945, 1.07
                 }
             };
             byte[] bytes=new byte[39];
             int writed = dp.DataToSegment(d, bytes, 0);
             Assert.AreEqual(39, writed);

             Assert.AreEqual(ProtocolType.GPS_ZHD, (uint)ValueHelper.GetShort(bytes, 0));
             Assert.AreEqual(2.134f, ValueHelper.GetDouble(bytes, 15));
             Assert.AreEqual(1.945f, ValueHelper.GetDouble(bytes, 23));
             Assert.AreEqual(1.07f, ValueHelper.GetDouble(bytes, 31));

             Assert.AreEqual(23, dp.CalcDataLength(1));
             Assert.AreEqual(31, dp.CalcDataLength(2));
             Assert.AreEqual(39, dp.CalcDataLength(3));
             Assert.AreEqual(47, dp.CalcDataLength(4));
             Assert.AreEqual(55, dp.CalcDataLength(5));

             Console.WriteLine("buff={0}", ValueHelper.BytesToHexStr(bytes));
         }

         [Test]
         public void TestSpliter()
         {
             DbDacDataProvider p = new DbDacDataProvider();
             Dictionary<string, string> args = new Dictionary<string, string>();
             args.Add("dbcongxml", path);
             p.Init(args);
             var d = new SensorOriginalData
             {
                 AcqTime = DateTime.Now,
                 SID = 108,
                 Type = ProtocolType.GPS_ZHD,
                 Values = new double[]
                 {
                     2.134, 1.945, 1.07
                 }
             };
             byte[] buff = new byte[39];
             int writed = p.DataToSegment(d, buff, 0);
             TranMsg[] msgs = p.Splite(buff, writed);
             Assert.AreEqual(1, msgs.Length);
             Console.WriteLine("MSG1={0}", ValueHelper.BytesToHexStr(msgs[0].Marshall()));
         }

         [Test]
         public void TestHasMoreData()
         {

             DbDacDataProvider p = new DbDacDataProvider();
             Dictionary<string, string> args = new Dictionary<string, string>();
             args.Add("dbcongxml", path);
             p.Init(args);
             // 5.
             ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, "Initial Catalog=DW_iSecureCloud_Empty;Data Source=192.168.1.128;User Id=sa;Password=861004");
             sqlHelper.Query("update T_THEMES_ENVI_WIND set lastSyncTime = null");
             int count = 0;
             Assert.IsTrue(p.HasMoreData());
             Console.WriteLine("TotalReminder: {0} records.", p.Remainder);
             Assert.IsTrue(p.Remainder > 0);
             count = p.Remainder;
             int len = 0;
             TranMsg[] msgs = p.NextPackages(out len);
             Assert.IsNotNull(msgs);
             byte[] buff = msgs[0].Data;
             Console.WriteLine(ValueHelper.BytesToHexStr(buff, 0, len, ""));
             // Record 4.
             Assert.IsTrue(len > 0);
             Console.WriteLine("len: {0}",len);
             // Data Sent
             p.OnPackageSent();
             var ds =
               sqlHelper.Query("select count(ID) from T_THEMES_ENVI_WIND where lastSyncTime is null");
             int count2 = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
             int rows = 1024/(3*8 + 15);

             Assert.AreEqual(rows, count-count2+1);

         }
    }
}