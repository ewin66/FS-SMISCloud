using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FS.SMIS_Cloud.DAC.Accessor;
using FS.SMIS_Cloud.DAC.Accessor.MSSQL;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Util;
using NUnit.Framework;

namespace DAC.Test.Model
{
    using System;

    using FS.DbHelper;

    [TestFixture]
    public class MsDbAccessTester
    {


        static string connstr = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004";

        //[Test]
        //public void TestDbAccessor()
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory;
        //    string assembly = GetDACStorageAssembly();
        //    if (string.IsNullOrEmpty(assembly)) throw new Exception(" consumers.xml has not DAC.Storage ");
        //    Assembly asm = Assembly.LoadFile(path + "\\" + assembly);
        //    Type[] types = asm.GetTypes();
        //    var helperlst = types.Where(t => new List<Type>(t.GetInterfaces()).Contains(typeof(ISqlHelper))).ToList();
        //    var dbAccessorlst = types.Where(t => new List<Type>(t.GetInterfaces()).Contains(typeof(IDbAccessor))).ToList();
        //    var SqlHelper = ObjectUtils.InstantiateType<ISqlHelper>(helperlst[0], new[] { typeof(string) }, new object[] { connstr });
        //    var msDbAccessor = ObjectUtils.InstantiateType<IDbAccessor>(dbAccessorlst[0], new[] { typeof(ISqlHelper) }, new object[] { SqlHelper });
        //    Assert.AreEqual("FS.SMIS_Cloud.DAC.Storage.iSecureCloud.MsSqlHelper", SqlHelper.GetType().FullName);
        //    Assert.AreEqual("FS.SMIS_Cloud.DAC.Storage.iSecureCloud.MsDbAccessor", msDbAccessor.GetType().FullName);
        //}

        //private static string GetDACStorageAssembly()
        //{
        //    string filefullpath = AppDomain.CurrentDomain.BaseDirectory + " \\consumers.xml";
        //    if (System.IO.File.Exists(filefullpath))
        //    {
        //        var doc = XDocument.Load(filefullpath);
        //        if (doc.Root != null)
        //        {
        //            var xElement = doc.Root.Element("consumers");
        //            if (xElement != null)
        //            {
        //                var consumerNodes = xElement.Elements();
        //                foreach (
        //                    var assembly in
        //                        from node in consumerNodes
        //                        where node.Attribute("name").Value == "DAC.Storage"
        //                        select node.Attribute("assembly").Value)
        //                {
        //                    return assembly;
        //                }
        //            }
        //        }

        //    }
        //    return string.Empty;
        //}

        [Test]
        public void TestGetDtus()
        {
            DbAccessorHelper.Init(new MsDbAccessor(connstr));

            IList<DtuNode> dtus = DbAccessorHelper.DbAccessor.QueryDtuNodes();
            Assert.IsTrue(dtus != null);

            Assert.IsTrue(dtus.Count > 0);
            return;
            DtuNode d0 = dtus[0];
            Assert.AreEqual((ushort)1, d0.DtuId);
            Assert.AreEqual("20120049", d0.DtuCode);
            Assert.AreEqual("K765集中采集站箱内", d0.Name);
            Assert.AreEqual((uint)300, d0.DacInterval); // 5m
            // dtuId	sid	mno	cno	factor	PRODUCT_SENSOR_ID	PROTOCOL_CODE	name
            //  1	    17	9596	1	10	82	1503	K765左侧一阶平台1号测斜孔-下

            IList<Sensor> sensors = d0.Sensors;
            Assert.IsTrue(sensors.Count > 0);
            Sensor s1 = sensors[0];
            Assert.AreEqual((uint)1, s1.DtuID);
            Assert.AreEqual((uint)17, s1.SensorID);
            Assert.AreEqual((uint)9596, s1.ModuleNo);
            
            // param.
            SensorParam sp1 = s1.Parameters[0];
            Assert.AreEqual(-0.2969320000000, sp1.Value);

            SensorParam sp3 = s1.Parameters[2];
            Assert.AreEqual(600, sp3.Value);

            Assert.AreEqual("GPRS", d0.NetworkType.ToString().ToUpper());

            DtuNode dx = dtus.First(d => d.DtuCode == "20141015");
            //Assert.AreEqual("GNSS_NMEA0183", dx.NetworkType);
            Assert.AreEqual("hclocal", dx.NetworkType.ToString());
            Assert.AreEqual(@"C:\华测\HCMonitor 1.0\HCMonitor 1.0\Resultcsv\Net01.csv", dx.GetProperty("param1"));
            Assert.AreEqual(string.Empty, dx.GetProperty("param2"));
        }

        [Test]
        public void TestGetDtuByNetworkType()
        {
            DbAccessorHelper.Init(new MsDbAccessor(connstr));
            IList<DtuNode> dtus = DbAccessorHelper.DbAccessor.QueryDtuNodes(null, NetworkType.gprs);
            Assert.IsTrue(dtus.Count > 0);
            //Assert.IsTrue(dtus.Count == 0);
        }

         [Test]
         public void TestGetDtuByCode()
         {
             DbAccessorHelper.Init(new MsDbAccessor(connstr));
             DtuNode d0 = DbAccessorHelper.DbAccessor.QueryDtuNode("20120049");
             Assert.AreEqual((ushort)1, d0.DtuId);
             Assert.AreEqual("20120049", d0.DtuCode);
             Assert.AreEqual("K765集中采集站箱内", d0.Name);
         }
    }
}
