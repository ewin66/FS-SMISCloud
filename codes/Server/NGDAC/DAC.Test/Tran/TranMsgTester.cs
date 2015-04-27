namespace NGDAC.Test.Tran
{
    using System;

    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Tran;
    using FS.SMIS_Cloud.NGDAC.Util;

    using NUnit.Framework;

    [TestFixture]
    public class TranMsgTester
    {

        [Test]
        public void TestMarshallerTranMsg()
        {
            TranMsg msg = new TranMsg
            {
                PackageIndex = 1,
                PackageCount = 2,
                LoadSize = 4,
                Data = new byte[] {1, 2, 3, 4},
                Type = TranMsgType.Vib
            };
            byte[] bs = msg.Marshall();
            // fa 00 0b 01 02 00 04 ff 01 02 03 04 e3 af
            string bss = ValueHelper.BytesToHexStr(bs);
            Assert.IsNotNull(bss);
            Console.WriteLine(bss);

            TranMsg msg2 = new TranMsg();
            msg2.Unmarshall(ValueHelper.StrToToHexByte("fb 00 0b 01 02 00 04 ff 01 02 03 04 e3 af"));
            Assert.AreEqual((int)Errors.ERR_INVALID_DATA, msg2.ErrorCode);
            msg2.Unmarshall(ValueHelper.StrToToHexByte("fa 00 0b 01 02 00 04 ff 01 02 03 04 e3 ef"));
            Assert.AreEqual((int)Errors.ERR_INVALID_DATA, msg2.ErrorCode);
            msg2.Unmarshall(ValueHelper.StrToToHexByte("fa 00 0b 01 02 00 04 ff 01 02 03 04 ef af"));
            Assert.AreEqual((int)Errors.ERR_INVALID_DATA, msg2.ErrorCode);
            msg2.Unmarshall(ValueHelper.StrToToHexByte("fa 00 Eb 01 02 00 04 ff 01 02 03 04 e3 af"));
            Assert.AreEqual((int)Errors.ERR_INVALID_DATA, msg2.ErrorCode);

            msg2.Unmarshall(ValueHelper.StrToToHexByte(bss));


            Assert.AreEqual(msg.PackageCount, msg2.PackageCount);
            Assert.AreEqual(msg.PackageIndex, msg2.PackageIndex);
            Assert.AreEqual(msg.LoadSize, msg2.LoadSize);
            Assert.AreEqual(msg.ID, msg2.ID);
            Assert.AreEqual(msg.Type, msg2.Type);
        }

        [Test]
        public void TestHeartBeatTranMsg()
        {
            HeartBeatTranMsg msg2 = new HeartBeatTranMsg(20120049, 10);
            string str = ValueHelper.BytesToHexStr(msg2.Marshall());
            Assert.IsNotNull(str);
            // fa 00 01 00 01 0a 00 ff 32 30 31 32 30 30 34 39 0a 00 4b af
            Console.WriteLine(str);
        }

        [Test]
        public void TestUnmarshallerTranMsg()
        {
            string ack =
                @"FA 0A 0C 00 01 36 01 FF 00 00 02 63 00 39 30 38 56 7A 28 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 38 56 7A 28 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 B8 70 80 28 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 B8 70 80 28 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 60 BA 01 29 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 60 BA 01 29 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 00 28 1C 29 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 00 28 1C 29 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 30 13 B0 29 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 63 00 39 30 30 13 B0 29 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 81 AF";
            TranMsg msg = new TranMsg(ValueHelper.StrToToHexByte(ack));
            Assert.AreEqual(10, msg.ID);
            Assert.AreEqual(310, msg.LoadSize);
            Assert.AreEqual(1, msg.PackageCount);
            Assert.AreEqual(0, msg.PackageIndex);
            Assert.AreEqual(TranMsgType.Dac, msg.Type);
            Assert.AreEqual(310, msg.Data.Length);
        }
    }
}
