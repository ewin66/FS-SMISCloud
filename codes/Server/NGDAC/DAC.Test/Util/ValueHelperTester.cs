namespace NGDAC.Test.Util
{
    using System.Collections.Generic;

    using FS.SMIS_Cloud.NGDAC.Util;

    using NUnit.Framework;

    [TestFixture]
    public class ValueHelperTester
    {
        [Test]
        public void TestBytes2HexStr()
        {
            byte[] bs = new byte[] { 0,1,2,3,4,5,6,7,8,9,0, (byte)'a',(byte)'b',(byte)'c',(byte)'D'};
            Assert.AreEqual("00 01 02 03 04 05 06 07 08 09 00 61 62 63 44", ValueHelper.BytesToHexStr(bs));

            Assert.AreEqual("000102030405060708090061626344", ValueHelper.BytesToHexStr(bs,0,bs.Length,""));

            Assert.AreEqual("00", ValueHelper.BytesToHexStr(bs, 0, 1, ""));
            Assert.AreEqual("", ValueHelper.BytesToHexStr(bs, 0, 0, ""));

            Assert.AreEqual("44", ValueHelper.BytesToHexStr(bs, bs.Length-1, 1, ""));
            Assert.AreEqual("63a44", ValueHelper.BytesToHexStr(bs, bs.Length - 2, 2 , "a"));
        }

        [Test]
        public void TestIsEqualZero()
        {
            double x = 0;
            float y = 0;
            Assert.IsTrue(ValueHelper.IsEqualZero(x));
            Assert.IsTrue(ValueHelper.IsEqualZero(y));
            x = 0.0000000000000001;
            Assert.IsTrue(ValueHelper.IsEqualZero(x));
            x = 0.000000000000001;
            Assert.IsFalse(ValueHelper.IsEqualZero(x));
            y = 0.0000001f;
            Assert.IsTrue(ValueHelper.IsEqualZero(y));
            y = 0.000001f;
            Assert.IsFalse(ValueHelper.IsEqualZero(y));
        }

        [Test]
        public void TestValueAtDouble()
        {
            double[] vs = new double[] { 0, 1, 2 };
            Assert.AreEqual(0, ValueHelper.ValueAt(vs, 0, 99));
            Assert.AreEqual(1, ValueHelper.ValueAt(vs, 1, 99));
            Assert.AreEqual(2, ValueHelper.ValueAt(vs, 2, 99));
            Assert.AreEqual(99, ValueHelper.ValueAt(vs, 3, 99));
            Assert.AreEqual(99, ValueHelper.ValueAt(vs, 21, 99));
        }



        [Test]
        public void TestToStr()
        {
            List<uint> ss = new List<uint>();
            ss.Add(0);
            ss.Add(1);
            Assert.AreEqual("0,1", ValueHelper.ToStr(ss));
            Assert.AreEqual("0~1", ValueHelper.ToStr(ss,'~'));
            Assert.AreEqual("0 1", ValueHelper.ToStr(ss, (char)0x20));
            List<uint> s1 = null;

            Assert.AreEqual("", ValueHelper.ToStr(s1));
            s1 = new List<uint>();
            Assert.AreEqual("", ValueHelper.ToStr(s1));

        }

    }
}
