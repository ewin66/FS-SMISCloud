namespace NGDAC.Test.DAC
{
    using FS.SMIS_Cloud.DAC.DAC.CxxAdapter;
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    using NUnit.Framework;

    [TestFixture]
    public class Wind_CFFTester
    {
         private SensorAcqResult GetSensorAcqResult()
         {
             var r = new SensorAcqResult
             {
                 Request = null,
                 Response = null,
                 Data = null,
                 ErrorCode = (int)Errors.ERR_DEFAULT
             };
             return r;
         }

         [Test]
         public void TestRequest()
         {
             string str = "23 30 32 72 0d";

             ISensorAdapter sa = new Wind_CFF_SensorAdapter();
             Sensor s = new Sensor()
             {
                 ModuleNo = 2,
                 TableColums = "AirSpeed,WindDirection,ElevationAngle"
             };
             byte[] expected = ValueHelper.StrToToHexByte(str);
             int err;
             var r = this.GetSensorAcqResult();
             r.Sensor = s;
             sa.Request(ref r);
             byte[] buff = r.Request;
             if (buff.Length == expected.Length)
             {
                 bool result = false;
                 for (int i = 0; i < expected.Length; i++)
                 {
                     result = buff[i] == expected[i];
                     if (!result)
                         break;
                 }
                 Assert.IsTrue(result);
             }
             else
             {
                 Assert.Fail();
             }

         }

         [Test]
         public void TestParseResponse()
         {
             string str = "3e 31 2e 35 2c 36 30 2e 32 0d 0a";
             ISensorAdapter sa = new Wind_CFF_SensorAdapter();

             byte[] buff = ValueHelper.StrToToHexByte(str);
             SensorAcqResult r = new SensorAcqResult
             {
                 Response = buff,
                 Sensor = new Sensor()
                 {
                     ModuleNo = 2,
                     ChannelNo = 1,
                     TableColums = "AirSpeed,WindDirection"
                 }
             };

             sa.ParseResult(ref r);
             var data =  r.Data;
             Assert.IsNotNull(data);
             Assert.AreEqual("1.5", data.RawValues[0].ToString("f1"));
             Assert.AreEqual("60.20", data.RawValues[1].ToString("f2"));

         }

    }
}
