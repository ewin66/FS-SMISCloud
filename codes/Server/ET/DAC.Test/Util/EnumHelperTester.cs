using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DAC.CxxAdapter;
using FS.SMIS_Cloud.DAC.Util;
using NUnit.Framework;

namespace DAC.Test.Util
{
    [TestFixture]
    public class EnumHelperTester
    {
        [Test]
        public void TestGetItemFromDesc()
        {
            string desc = "FS-LF10";
            var range = (VoltageSensorRange) EnumHelper.GetItemFromDesc(typeof (VoltageSensorRange), desc);
            Assert.AreEqual(VoltageSensorRange.FSLF10,range);
        }

        [Test]
        public void TestGetDescription()
        {
             string desc = EnumHelper.GetDescription(VoltageSensorRange.FSLF10);
             Assert.AreEqual("FS-LF10", desc);
        }
    }
}