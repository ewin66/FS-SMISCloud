namespace NGDAC.Test.DAC
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using FS.SMIS_Cloud.NGDAC.Accessor;
    using FS.SMIS_Cloud.NGDAC.Accessor.MSSQL;
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.DAC.CxxAdapter;
    using FS.SMIS_Cloud.NGDAC.File;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Model.Sensors;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Task;

    using NUnit.Framework;

    [TestFixture]
    public class HcGpsTester
    {
        static string connstr = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004";

        [Test]
        public void TestHcGpsReadData()
        {
            DbAccessorHelper.Init(new MsDbAccessor(connstr));
            string filePath = @"FileData\Net01.csv";

            IFileSensorAdapter sa = new Gps_HC_SensorAdapter();
            int err;
            //Assert.IsNull(sa.Request(null,out err));

            Sensor s1 = new Sensor { SensorID = 1, ModuleNo = 1 };
            var dat1 = sa.ReadData(s1, filePath);
            Assert.IsEmpty(dat1);

            Sensor s2 = new Sensor { SensorID = 2, ModuleNo = 2 };
            var dat2 = sa.ReadData(s2, filePath);
            Assert.IsEmpty(dat2);

            Sensor s3 = new Sensor { SensorID = 3, ModuleNo = 3 };
            var dat3 = sa.ReadData(s3, filePath);
            var act3 = Encoding.ASCII.GetString(dat3.First());
            var exp3 =
                "2,3,2014/8/14  6:54:43,30:49:59.31026N,121:31:23.58789E,3639542.8400,2680979.7673,28.0908,30:49:59.30982N,121:31:23.58559E,3639542.8124,2680979.7071,28.1878";
            Assert.AreEqual(exp3, act3);
        }

        [Test]
        public void TestHcGpsParseData()
        {
            byte[] data =
                Encoding.ASCII.GetBytes(
                    "2,3,2014/8/14  6:54:43,30:49:59.31026N,121:31:23.58789E,3639542.8400,2680979.7673,28.0908,30:49:59.30982N,121:31:23.58559E,3639542.8124,2680979.7071,28.1878");
            IFileSensorAdapter sa = new Gps_HC_SensorAdapter();

            var sensor = new Sensor { ModuleNo = 3, TableColums = "height" };
            sensor.AddParameter(new SensorParam(null) { Value = 1 });
            sensor.AddParameter(new SensorParam(null) { Value = 1 });
            sensor.AddParameter(new SensorParam(null) { Value = 1 });
            sensor.AddParameter(new SensorParam(null) { Value = 0 });
            var r = new SensorAcqResult { Response = data, ErrorCode = (int)Errors.SUCCESS, Sensor = sensor };
             sa.ParseResult(ref r) ;
             var gpsData = r.Data as GpsHeightData;
             Assert.IsNotNull(gpsData);
             //Assert.AreEqual(Convert.ToDateTime("2014-8-14  6:54:43"), gpsData.AcqTime);
             Assert.AreEqual(28187.8, gpsData.CoordHeight);
             Assert.AreEqual(28186.8, gpsData.ChangeHeight);
        }

        [Test]
        public void TestGetSensorLastAcqTime()
        {
            if (File.Exists("lastAcqDate.dat"))
            {
                File.Delete("lastAcqDate.dat");
            }
            var adpt = new Gps_HC_SensorAdapter();

            Assert.IsNull(adpt.GetSensorLastDataAcqTime(1));

            adpt.UpdateSensorLastDataAcqTime(1, new DateTime(2014, 10, 1, 1, 1, 1));

            Assert.AreEqual(new DateTime(2014, 10, 1, 1, 1, 1), adpt.GetSensorLastDataAcqTime(1));

            adpt.UpdateSensorLastDataAcqTime(1, new DateTime(2014, 10, 2, 1, 1, 1));

            Assert.AreEqual(new DateTime(2014, 10, 2, 1, 1, 1), adpt.GetSensorLastDataAcqTime(1));

            if (File.Exists("lastAcqDate.dat"))
            {
                File.Delete("lastAcqDate.dat");
            }
        }


        [Test]
        public void TestSnGpsTask()
        {
            if (System.IO.File.Exists("lastAcqDate.dat"))
            {
                System.IO.File.Delete("lastAcqDate.dat");
            }

            (new Gps_SN_SensorAdapter() as GpsBaseAdapter).UpdateSensorLastDataAcqTime(1, new DateTime(2014, 8, 14, 6, 49, 43));

            var adapterManager = SensorAdapterManager.InitializeManager();
            var taskexcutor = new DACTaskExecutor(adapterManager);
            var dtusens = new List<uint>();
            dtusens.Add(1);
            var task = new DACTask("1", 100, dtusens, TaskType.TIMED, null);
            var dtunode = new DtuNode
            {
                DtuId = 100,
                Type = DtuType.File,
                NetworkType = NetworkType.hclocal
            };
            var s = new Sensor
            {
                SensorID = 1,
                ModuleNo = 3,
                ProtocolType = 9402,
                ChannelNo = 1,
                TableColums = "SURFACE_DISPLACEMENT_X_VALUE,SURFACE_DISPLACEMENT_Y_VALUE,SURFACE_DISPLACEMENT_Z_VALUE"
            };
            s.AddParameter(new SensorParam(null) { Value = 1 });
            s.AddParameter(new SensorParam(null) { Value = 2 });
            s.AddParameter(new SensorParam(null) { Value = 3 });
            s.AddParameter(new SensorParam(null) { Value = 0 });
            dtunode.AddSensor(s);
            dtunode.AddProperty("param1", @"FileData\Net01.csv");
            var conn = new FileDtuConnection(dtunode);
            var contxt = new DacTaskContext()
            {
                Node = dtunode,
                DtuConnection = conn
            };

            var dactaskresult = taskexcutor.Run(task, contxt);

            Assert.IsNotNull(dactaskresult);
            Assert.IsTrue(dactaskresult.Task.Status == DACTaskStatus.DONE);
            var sensorresults = dactaskresult.SensorResults;
            Assert.NotNull(sensorresults);
            Assert.IsNotEmpty(sensorresults);
            var senres = sensorresults[0];
            Assert.IsTrue(senres.IsOK);
            var sendata = senres.Data;
            Assert.IsTrue(sendata is Gps3dData);
            Assert.AreEqual(sendata.RawValues[0], 3639542.8124 * 1000, 0.0000001);
            Assert.AreEqual(sendata.RawValues[1], 2680979.7071 * 1000, 0.0000001);
            Assert.AreEqual(sendata.RawValues[2], 28.1878 * 1000, 0.0000001);
            Assert.AreEqual(sendata.ThemeValues[0].Value, 3639542.8124 * 1000 - 1, 0.0000001);
            Assert.AreEqual(sendata.ThemeValues[1].Value, 2680979.7071 * 1000 - 2, 0.0000001);
            Assert.AreEqual(sendata.ThemeValues[2].Value, 28.1878 * 1000 - 3, 0.0000001);
        }
    }
}
