namespace NGDAC.Test.DAC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.DAC.CxxAdapter;
    using FS.SMIS_Cloud.NGDAC.File;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Model.Sensors;
    using FS.SMIS_Cloud.NGDAC.Task;

    using NUnit.Framework;

    [TestFixture]
    public class GpsSnTester
    {
        static string connstr = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004";
        private string filepath = @"FileData\CDMonitorSession.csv";

        [Test]
        public void TestSnGpsReadData()
        {
            if (System.IO.File.Exists("lastAcqDate.dat"))
            {
                System.IO.File.Delete("lastAcqDate.dat");
            }
            IFileSensorAdapter sa = new Gps_SN_SensorAdapter();
            int err;
            SensorAcqResult r = null;
            sa.Request(ref r);
            

            Sensor s1 = new Sensor { SensorID = 1, ModuleNo = 2 };
            var dat1 = sa.ReadData(s1, this.filepath);
            Assert.IsEmpty(dat1);

            Sensor s2 = new Sensor { SensorID = 1, ModuleNo = 9003 };
            var dat2 = sa.ReadData(s2, this.filepath);
            Assert.IsNotEmpty(dat2);
            Assert.AreEqual(101, dat2.Count());
            var exptime = new DateTime(2015, 3, 9, 16, 47, 30);
            Assert.IsTrue(exptime == (sa as GpsBaseAdapter).GetSensorLastDataAcqTime(1));

            (sa as GpsBaseAdapter).UpdateSensorLastDataAcqTime(1, new DateTime(2015, 3, 10));
            dat2 = sa.ReadData(s2, this.filepath);
            Assert.IsEmpty(dat2);

            (sa as GpsBaseAdapter).UpdateSensorLastDataAcqTime(1, new DateTime(2015, 3, 9, 0, 0, 0));
            dat2 = sa.ReadData(s2, this.filepath);
            Assert.AreEqual(17,dat2.Count());

            (sa as GpsBaseAdapter).UpdateSensorLastDataAcqTime(1, new DateTime(2015, 3, 9, 16, 0, 0));
            dat2 = sa.ReadData(s2, this.filepath);
            Assert.AreEqual(1, dat2.Count());
        }

        [Test]
        public void TestSnGpsTask()
        {
            if (System.IO.File.Exists("lastAcqDate.dat"))
            {
                System.IO.File.Delete("lastAcqDate.dat");
            }

            (new Gps_SN_SensorAdapter() as GpsBaseAdapter).UpdateSensorLastDataAcqTime(1, new DateTime(2015, 1, 28, 23, 51, 35));

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
                ModuleNo = 9003,
                ProtocolType = 9403,
                ChannelNo = 1,
                TableColums = "SURFACE_DISPLACEMENT_X_VALUE,SURFACE_DISPLACEMENT_Y_VALUE,SURFACE_DISPLACEMENT_Z_VALUE"
            };
            s.AddParameter(new SensorParam(null) { Value = 2 });
            s.AddParameter(new SensorParam(null) { Value = 2 });
            s.AddParameter(new SensorParam(null) { Value = 2 });
            s.AddParameter(new SensorParam(null) { Value = 0 });
            dtunode.AddSensor(s);
            dtunode.AddProperty("param1", this.filepath);
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
            Assert.AreEqual(101, sensorresults.Count);
            var senres = sensorresults[0];
            Assert.IsTrue(senres.IsOK);
            var sendata = senres.Data;
            Assert.IsTrue(sendata is Gps3dData);
            Assert.AreEqual(sendata.RawValues[0], 4435175.4523 * 1000, 0.0000001);
            Assert.AreEqual(sendata.RawValues[1], 524121.0006 * 1000, 0.0000001);
            Assert.AreEqual(sendata.RawValues[2], 82.2112 * 1000, 0.0000001);
            Assert.AreEqual(sendata.ThemeValues[0].Value, 4435175.4523 * 1000 - 2, 0.0000001);
            Assert.AreEqual(sendata.ThemeValues[1].Value, 524121.0006 * 1000 - 2, 0.0000001);
            Assert.AreEqual(sendata.ThemeValues[2].Value, 82.2112 * 1000 - 2, 0.0000001);
        }
    }
}
