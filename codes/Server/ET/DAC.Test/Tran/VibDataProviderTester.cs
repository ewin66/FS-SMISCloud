using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.Tran;
using FS.SMIS_Cloud.DAC.Util;
using NUnit.Framework;

namespace DAC.Test.Tran
{
    [TestFixture]
    class VibDataProviderTester
    {
        private VibFileDataProvider provider;

        [SetUp]
        public void SetUp()
        {
            provider = new VibFileDataProvider();
            var args = new Dictionary<string, string>();
            args["DataPath"] = "VibData";
            provider.Init(args);
        }

        [Test]
        public void TestHasMoreData()
        {
            Assert.IsTrue(provider.HasMoreData());
        }

        [Test]
        public void TestNextPackage()
        {
            int len;
            provider.HasMoreData();
            TranMsg[] msgs = provider.NextPackages(out len);
            Assert.IsTrue(len > 0);
            Assert.IsNotNull(msgs);
        }

        [Test]
        public void TestNextPackageData() {
            int len;
            provider.HasMoreData();
            TranMsg[] msgs = provider.NextPackages(out len);
            Assert.AreEqual(5, msgs.Length);
            TranMsg msg1 = TranMsg.Combine(msgs);
            byte[] buff = msg1.Data;

            short fiLen = ValueHelper.GetShort(buff, 0);
            short fdLen = ValueHelper.GetShort(buff, 0 + 2 + fiLen);
            Assert.AreEqual(55, fiLen);
            Assert.AreEqual(4670, fdLen);

            byte[] fib = CompressHelper.Decompress(buff, 2, fiLen);
            byte[] fdb = CompressHelper.Decompress(buff, 4 + fiLen, fdLen);
            using (FileStream writer = new FileStream("vibi.data", FileMode.Create, FileAccess.Write))
            {
                writer.Write(fib,0, fib.Length);
            }
            using (FileStream writer = new FileStream("vibd.data", FileMode.Create, FileAccess.Write))
            {
                writer.Write(fdb, 0, fdb.Length);
            }

            provider.OnPackageSent();

            Assert.AreEqual(1, provider.Remainder);

        }
    }
}
