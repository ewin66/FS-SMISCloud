using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.Calculation;
using NUnit.Framework;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.nunit
{
    [TestFixture]
    class VibrationFileParserTester
    {
        private string filepath = @"nunit\Resource\6145_4_20000321031626.dat";
        private string fileloss = @"nunit\Resource\6145_4_20000321031626LOSS.dat";
        [Test]
        public void TestParse()
        {
            Stopwatch watch=new Stopwatch();
            watch.Start();
            var res = VibrationFileParser.Parse(1, filepath);
            Assert.AreEqual(1,res.Error);
            Assert.IsTrue(new DateTime(2000,3,21,3,16,26)==res.Time);
            Assert.AreEqual(0,res.Affects);
            watch.Stop();
            Console.WriteLine(string.Format("Time Elapse:{0} ms", watch.ElapsedMilliseconds));

            watch.Start();
            res = VibrationFileParser.Parse(87, filepath);
            Assert.AreEqual(0, res.Error);
            Assert.AreEqual(2048+1025, res.Affects);
            watch.Stop();
            Console.WriteLine(string.Format("Time Elapse:{0} ms", watch.ElapsedMilliseconds));

            res = VibrationFileParser.Parse(88, filepath);
            Assert.AreEqual(0, res.Error);
            Assert.AreEqual(1998 + 513, res.Affects);
        }
    }
}
