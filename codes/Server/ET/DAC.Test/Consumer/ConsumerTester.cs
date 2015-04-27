using System;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model.Sensors;

namespace DAC.Test.Consumer
{
    using System.Threading;

    using FS.SMIS_Cloud.DAC.Consumer;
    using FS.SMIS_Cloud.DAC.DataAnalyzer;
    using FS.SMIS_Cloud.DAC.Model;
    using FS.SMIS_Cloud.DAC.Task;

    using NUnit.Framework;

    [TestFixture]
    internal class ConsumerTester
    {
        [TestFixtureSetUp]       
        public void Init()
        {
            DACTaskResultConsumerService.RegisterConsumer("test", typeof(DataAnalyzer));
            DACTaskResultConsumerService.RegisterConsumer("C1", typeof(C1));
            DACTaskResultConsumerService.RegisterConsumer("C2", typeof(C2));
            DACTaskResultConsumerService.RegisterConsumer("C3", typeof(C3));
            DACTaskResultConsumerService.RegisterConsumer("CE", typeof(Ce));
            DACTaskResultConsumerService.Queues.Clear();
        }

        [Test]
        public void TestRegisterConsumer()
        {            
            Assert.IsNotNull(DACTaskResultConsumerService.GetConsumer("test"));
        }

        [Test]
        public void TestGetConsumer()
        {            
            Assert.AreEqual(
                "FS.SMIS_Cloud.DAC.DataAnalyzer.DataAnalyzer",
                DACTaskResultConsumerService.GetConsumer("test").GetType().ToString());
        }

       
        [Category("MANUAL")]
        [Test]
        public void TestInit()
        {
            DACTaskResultConsumerService.Init();

            Assert.AreEqual(
                DACTaskResultConsumerService.GetConsumer("DataAnalyzer").GetType().ToString(),
                "FS.SMIS_Cloud.DAC.DataAnalyzer.DataAnalyzer");

            Assert.AreEqual(
                DACTaskResultConsumerService.GetConsumer("DataAnalyzer").GetType(),
                DACTaskResultConsumerService.Queues[0][5].GetType());
        }

        [Test]
        public void TestSyncQueueInvoke()
        {
            var syncQueue = new DACTaskResultConsumerQueue(ConsumeType.Sync);
            syncQueue.Enqueue(DACTaskResultConsumerService.GetConsumer("C1"));
            syncQueue.Enqueue(DACTaskResultConsumerService.GetConsumer("C2"));
            syncQueue.Enqueue(DACTaskResultConsumerService.GetConsumer("C3"));

            DACTaskResultConsumerService.AddComsumerQueue(syncQueue);

            var rslt1 = new DACTaskResult(); 
            rslt1.AddSensorResult(new SensorAcqResult()
            {
                Data = new Gps3dData(65,45,87,12,23,56)
            });
            rslt1.ErrorMsg = "队列1";
            DACTaskResultConsumerService.OnDacTaskResultProduced(rslt1);
            //Thread.Sleep(8000);
            Console.WriteLine("source: {0}", rslt1.ErrorMsg);
            Assert.AreNotEqual("C---1C---2C---3", rslt1.ErrorMsg);


            //var rslt2 = new DACTaskResult();
            //rslt2.ErrorMsg = "队列2";
            //DACTaskResultConsumerService.OnDacTaskResultProduced(rslt2);
            ////Thread.Sleep(8000);
            //Console.WriteLine("source: {0}", rslt2.ErrorMsg);
            //Assert.AreNotEqual("C---1C---2C---3", rslt2.ErrorMsg);
            Thread.Sleep(5000);
        }

        [Test]
        public void TestAsyncQueueInvoke()
        {          
            var syncQueue = new DACTaskResultConsumerQueue(ConsumeType.Async);
            syncQueue.Enqueue(DACTaskResultConsumerService.GetConsumer("C1"));
            syncQueue.Enqueue(DACTaskResultConsumerService.GetConsumer("C2"));
            syncQueue.Enqueue(DACTaskResultConsumerService.GetConsumer("C3"));

            DACTaskResultConsumerService.AddComsumerQueue(syncQueue);

            var rslt1 = new DACTaskResult();
            
            rslt1.ErrorMsg = "队列1";
            DACTaskResultConsumerService.OnDacTaskResultProduced(rslt1);
            //Thread.Sleep(8000);
            Console.WriteLine("source: {0}", rslt1.ErrorMsg);
            Assert.AreNotEqual("C---1C---2C---3", rslt1.ErrorMsg);


            var rslt2 = new DACTaskResult();
            rslt2.ErrorMsg = "队列2";
            DACTaskResultConsumerService.OnDacTaskResultProduced(rslt2);
            //Thread.Sleep(8000);
            Console.WriteLine("source: {0}", rslt2.ErrorMsg);
            Assert.AreNotEqual("C---1C---2C---3", rslt2.ErrorMsg);
            Thread.Sleep(5000);
        }

        [Test]
        public void TestSyncQueueInvoke_WithException()
        {
            var syncQueue = new DACTaskResultConsumerQueue(ConsumeType.Sync);
            syncQueue.Enqueue(DACTaskResultConsumerService.GetConsumer("C1"));
            syncQueue.Enqueue(DACTaskResultConsumerService.GetConsumer("C2"));
            syncQueue.Enqueue(DACTaskResultConsumerService.GetConsumer("CE"));
            syncQueue.Enqueue(DACTaskResultConsumerService.GetConsumer("C3"));

            DACTaskResultConsumerService.AddComsumerQueue(syncQueue);

            var rslt1 = new DACTaskResult();
            rslt1.AddSensorResult(new SensorAcqResult()
            {
                Data = new Gps3dData(65, 45, 87, 12, 23, 56)
            });
            rslt1.ErrorMsg = "队列1";
            DACTaskResultConsumerService.OnDacTaskResultProduced(rslt1);
            //Thread.Sleep(8000);
            Console.WriteLine("source: {0}", rslt1.ErrorMsg);
            Assert.AreNotEqual("C---1C---2C---3", rslt1.ErrorMsg);


            //var rslt2 = new DACTaskResult();
            //rslt2.ErrorMsg = "队列2";
            //DACTaskResultConsumerService.OnDacTaskResultProduced(rslt2);
            ////Thread.Sleep(8000);
            //Console.WriteLine("source: {0}", rslt2.ErrorMsg);
            //Assert.AreNotEqual("C---1C---2C---3", rslt2.ErrorMsg);
            Thread.Sleep(5000);
        }
    }

    public class C1 : IDACTaskResultConsumer
    {
        private static int count;

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(DACTaskResult source)
        {
            Thread.Sleep(1000);
            source.ErrorMsg += "C---1";
            Console.WriteLine("C---1:{0},rslt.msg:{1}", ++count, source.ErrorMsg);
            source.SensorResults[0].Data.ThemeValues[0]=1000;
            //data.UpdateThemeValues(1000, 0);
            //source.SensorResults[0].Data.ThemeValues[0] = 1000;
            Console.WriteLine(source.SensorResults[0].Data.ThemeValues[0]);
           // Assert.AreEqual(1000,source.SensorResults[0].Data.ThemeValues[0]);
        }
    }

    public class C2 : IDACTaskResultConsumer
    {
        private static int count;

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(DACTaskResult source)
        {
            Thread.Sleep(500);
            source.ErrorMsg += "C---2";
            Console.WriteLine("C---2:{0},rslt.msg:{1}", ++count, source.ErrorMsg);
            Console.WriteLine(source.SensorResults[0].Data.ThemeValues[0]);
        }
    }

    public class C3 : IDACTaskResultConsumer
    {
        private static int count;

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(DACTaskResult source)
        {
            Thread.Sleep(100);
            source.ErrorMsg += "C---3"; 
            Console.WriteLine("C---3:{0},rslt.msg:{1}", ++count, source.ErrorMsg);
        }
    }

    public class Ce : IDACTaskResultConsumer
    {
        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(DACTaskResult source)
        {
            throw new Exception("error");
        }
    }
}
