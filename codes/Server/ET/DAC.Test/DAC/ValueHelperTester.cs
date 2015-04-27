using FS.SMIS_Cloud.DAC.Util;
using NUnit.Framework;

namespace DAC.Test.DAC
{
    [TestFixture]
    public class ValueHelperTester
    {
        [Test]
        public void TestToBytes()
        {
            string bs = "FC0101230000010064123456780000000000000000000000000000000000EECF";
            byte[] buff = ValueHelper.ToBytes(bs);
            Assert.AreEqual(0xfc, buff[0]);
            Assert.AreEqual(0xcf, buff[31]);
            Assert.AreEqual(32, buff.Length);
        }

        [Test]
        public void TestToShortOrUShort()
        {
            uint moduleNo = 5482;
            byte[] temp = new byte[2];
            ValueHelper.WriteShort_BE(temp, 0, (short)moduleNo);

             byte[] temp2 = new byte[2];
             ValueHelper.WriteUShort_BE(temp2, 0, (ushort)moduleNo);

             Assert.AreEqual(temp[0], temp2[0]);
             Assert.AreEqual(temp[1], temp2[1]);

            var m1=ValueHelper.GetShort(temp, 0);
            var m2 = ValueHelper.GetUShort(temp, 0);
            Assert.AreEqual(m1,m2);
        }


    }
}
