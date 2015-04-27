namespace NGDAC.Test.DAC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using FS.SMIS_Cloud.NGDAC.Util;

    using NUnit.Framework;

    [TestFixture]
    public class PerfTest
    {
        private static Queue<int> myQueue = null;
        private static PerformanceCounter c1 = null;

        [SetUp]
        void Setup()
        {
            c1 = Performance.GetCounter("Test.PoolSize");
            myQueue = new Queue<int>();  
        }


        [Test]
        [Category("MANUAL")]
        public void TestPerformance()
        {
            while (true)
            {
                EnqueueQueue();
                System.Threading.Thread.Sleep(1000);
                DequeueQueue();
            }  
        }

        static void EnqueueQueue()
        {
            int C = new Random().Next(10);
            for (int i = 0; i < C; i++)
            {
                myQueue.Enqueue(i);
                //计数器加一  
                c1.Increment();
            }
        }

        static void DequeueQueue()
        {
            int C = new Random().Next(20);
            for (int i = 0; i < C; i++)
            {
                if (myQueue.Count == 0)
                    break;
                myQueue.Dequeue();
                //计数器减一  
                c1.Decrement();
            }
        }  
    }
}
