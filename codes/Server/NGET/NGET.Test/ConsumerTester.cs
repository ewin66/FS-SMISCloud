namespace NGET.Test.Consumer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using FS.SMIS_Cloud.NGET.Consumer;
    using FS.SMIS_Cloud.NGET.DataAnalyzer;
    using FS.SMIS_Cloud.NGET.Model;

    using NUnit.Framework;

    [TestFixture]
    internal class ConsumerTester
    {
        [TestFixtureSetUp]       
        public void Init()
        {
            DacTaskResultConsumerService.RegisterConsumer("test", typeof(DataAnalyzer));
            DacTaskResultConsumerService.RegisterConsumer("C1", typeof(C1));
            DacTaskResultConsumerService.RegisterConsumer("C2", typeof(C2));
            DacTaskResultConsumerService.RegisterConsumer("C3", typeof(C3));
            DacTaskResultConsumerService.RegisterConsumer("CE", typeof(Ce));
            DacTaskResultConsumerService.Queues.Clear();
        }

        [Test]
        public void TestRegisterConsumer()
        {            
            Assert.IsNotNull(DacTaskResultConsumerService.GetConsumer("test"));
        }

        [Test]
        public void TestGetConsumer()
        {            
            Assert.AreEqual(
                "FS.SMIS_Cloud.DAC.DataAnalyzer.DataAnalyzer",
                DacTaskResultConsumerService.GetConsumer("test").GetType().ToString());
        }

       
        [Category("MANUAL")]
        [Test]
        public void TestInit()
        {
            DacTaskResultConsumerService.Init();

            Assert.AreEqual(
                DacTaskResultConsumerService.GetConsumer("DataAnalyzer").GetType().ToString(),
                "FS.SMIS_Cloud.DAC.DataAnalyzer.DataAnalyzer");

            Assert.AreEqual(
                DacTaskResultConsumerService.GetConsumer("DataAnalyzer").GetType(),
                DacTaskResultConsumerService.Queues[0][5].GetType());
        }

        [Test]
        public void TestSyncQueueInvoke()
        {
            var syncQueue = new DacTaskResultConsumerQueue(ConsumeType.Sync);
            syncQueue.Enqueue(DacTaskResultConsumerService.GetConsumer("C1"));
            syncQueue.Enqueue(DacTaskResultConsumerService.GetConsumer("C2"));
            syncQueue.Enqueue(DacTaskResultConsumerService.GetConsumer("C3"));

            DacTaskResultConsumerService.AddComsumerQueue(syncQueue);

            var rslt1 = new List<SensorAcqResult>();
            rslt1.Add(new SensorAcqResult()
            {
                Data = new SensorData(new double[] { 65, 45, 87 }, new double[] { 12, 23, 56 }, new double[] { 65, 45, 87 })
            });
            rslt1[0].ErrorCode = 1;
            DacTaskResultConsumerService.OnDacTaskResultProduced(rslt1);
            //Thread.Sleep(8000);
            Console.WriteLine("source: {0}", rslt1[0].ErrorCode);
            Assert.AreNotEqual(7, rslt1[0].ErrorCode);


            //var rslt2 = new DACTaskResult();
            //rslt2.ErrorMsg = "队列2";
            //DacTaskResultConsumerService.OnDacTaskResultProduced(rslt2);
            ////Thread.Sleep(8000);
            //Console.WriteLine("source: {0}", rslt2.ErrorMsg);
            //Assert.AreNotEqual("C---1C---2C---3", rslt2.ErrorMsg);
            Thread.Sleep(5000);
        }

        [Test]
        public void TestAsyncQueueInvoke()
        {          
            var syncQueue = new DacTaskResultConsumerQueue(ConsumeType.Async);
            syncQueue.Enqueue(DacTaskResultConsumerService.GetConsumer("C1"));
            syncQueue.Enqueue(DacTaskResultConsumerService.GetConsumer("C2"));
            syncQueue.Enqueue(DacTaskResultConsumerService.GetConsumer("C3"));

            DacTaskResultConsumerService.AddComsumerQueue(syncQueue);

            var rslt1 = new List<SensorAcqResult>();
            rslt1.Add(new SensorAcqResult()
            {
                Data = new SensorData(new double[] { 65, 45, 87 }, new double[] { 12, 23, 56 }, new double[] { 65, 45, 87 })
            });
            rslt1[0].ErrorCode = 1;
            DacTaskResultConsumerService.OnDacTaskResultProduced(rslt1);
            //Thread.Sleep(8000);
            Console.WriteLine("source: {0}", rslt1[0].ErrorCode);
            Assert.AreNotEqual(7, rslt1[0].ErrorCode);


            var rslt2 = new List<SensorAcqResult>();
            rslt2.Add(new SensorAcqResult()
            {
                Data = new SensorData(new double[] { 65, 45, 87 }, new double[] { 12, 23, 56 }, new double[] { 65, 45, 87 })
            });
            rslt2[0].ErrorCode = 2;
            DacTaskResultConsumerService.OnDacTaskResultProduced(rslt2);
            //Thread.Sleep(8000);
            Console.WriteLine("source: {0}", rslt2[0].ErrorCode);
            Assert.AreNotEqual(7, rslt2[0].ErrorCode);
            Thread.Sleep(5000);
        }

        [Test]
        public void TestSyncQueueInvoke_WithException()
        {
            var syncQueue = new DacTaskResultConsumerQueue(ConsumeType.Sync);
            syncQueue.Enqueue(DacTaskResultConsumerService.GetConsumer("C1"));
            syncQueue.Enqueue(DacTaskResultConsumerService.GetConsumer("C2"));
            syncQueue.Enqueue(DacTaskResultConsumerService.GetConsumer("CE"));
            syncQueue.Enqueue(DacTaskResultConsumerService.GetConsumer("C3"));

            DacTaskResultConsumerService.AddComsumerQueue(syncQueue);

            var rslt1 = new List<SensorAcqResult>();
            rslt1.Add(new SensorAcqResult()
            {
                Data = new SensorData(new double[] { 65, 45, 87 }, new double[] { 12, 23, 56 }, new double[] { 65, 45, 87 })
            });
            rslt1[0].ErrorCode = 1;
            DacTaskResultConsumerService.OnDacTaskResultProduced(rslt1);
            //Thread.Sleep(8000);
            Console.WriteLine("source: {0}", rslt1[0].ErrorCode);
            Assert.AreNotEqual(7, rslt1[0].ErrorCode);


            //var rslt2 = new DACTaskResult();
            //rslt2.ErrorMsg = "队列2";
            //DacTaskResultConsumerService.OnDacTaskResultProduced(rslt2);
            ////Thread.Sleep(8000);
            //Console.WriteLine("source: {0}", rslt2.ErrorMsg);
            //Assert.AreNotEqual("C---1C---2C---3", rslt2.ErrorMsg);
            Thread.Sleep(5000);
        }
    }

    public class C1 : IDacTaskResultConsumer
    {
        private static int count;

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(List<SensorAcqResult> source)
        {
            Thread.Sleep(1000);
            source[0].ErrorCode += 1;
            Console.WriteLine("C---1:{0},rslt.msg:{1}", ++count, source[0].ErrorCode);
            source[0].Data.ThemeValues[0]=1000;
            //data.UpdateThemeValues(1000, 0);
            //source.SensorResults[0].Data.ThemeValues[0] = 1000;
            Console.WriteLine(source[0].Data.ThemeValues[0]);
           // Assert.AreEqual(1000,source.SensorResults[0].Data.ThemeValues[0]);
        }
    }

    public class C2 : IDacTaskResultConsumer
    {
        private static int count;

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(List<SensorAcqResult> source)
        {
            Thread.Sleep(500);
            source[0].ErrorCode += 2;
            Console.WriteLine("C---2:{0},rslt.msg:{1}", ++count, source[0].ErrorCode);
            Console.WriteLine(source[0].Data.ThemeValues[0]);
        }
    }

    public class C3 : IDacTaskResultConsumer
    {
        private static int count;

        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(List<SensorAcqResult> source)
        {
            Thread.Sleep(100);
            source[0].ErrorCode += 3;
            Console.WriteLine("C---3:{0},rslt.msg:{1}", ++count, source[0].ErrorCode);
        }
    }

    public class Ce : IDacTaskResultConsumer
    {
        public SensorType[] SensorTypeFilter { get; set; }

        public void ProcessResult(List<SensorAcqResult> source)
        {
            throw new Exception("error");
        }
    }
}
