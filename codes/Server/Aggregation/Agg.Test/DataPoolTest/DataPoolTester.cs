using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace Agg.Test
{
    using System.Diagnostics;

    using Agg.Comm.DataModle;
    using Agg.DataPool;

    [TestFixture]
    public class DataPoolTester
    {
        private static BaseAggConfig CreateCommonConfig(AggType type)
        {
            BaseAggConfig config = new BaseAggConfig(new AggTaskKey(90,42,type));
            /// 家乐福项目配置
          //  config.TimeRange = new AggTimeRange();
        
           
            return config;
        }

        [Test]
        public void CreateDataPoolTester()
        {
            BaseAggConfig config = CreateCommonConfig(AggType.Day);
          
            config.TimeRange = new AggTimeRange{DataBeginHour = 1,DataEndHour = 2,DateBegin = 0,DateEnd = 0};
            DataPoolFactory.Init();
            IDataPool dataPool = DataPoolFactory.GetDataPool(config);
            Assert.IsNotNull(dataPool);

            config = CreateCommonConfig(AggType.Week); 
            dataPool = DataPoolFactory.GetDataPool(config);
            Assert.IsNotNull(dataPool);

            config = CreateCommonConfig(AggType.Month); 
            dataPool = DataPoolFactory.GetDataPool(config);
            Assert.IsNotNull(dataPool);
        }

        [Test]
        public void DayDataPoolTester()
        {
            BaseAggConfig config = CreateCommonConfig(AggType.Day);
            config.TimeRange = new AggTimeRange{DataBeginHour = 1,DataEndHour = 2,DateBegin = 0,DateEnd = 0};

            DataPoolFactory.Init();
            IDataPool dataPool = DataPoolFactory.GetDataPool(config);
            Assert.IsNotNull(dataPool);

            DateTime time = new DateTime(2015, 2, 15);
            AggRawData data = dataPool.GetAggRawData(time);
            int count = 0;
            foreach (var tmp in data.Datas) 
            {
                count += tmp.Values.Count;
            }
            Console.WriteLine(string.Format("共读取{0}条数据", count));
            Assert.IsTrue(data.TimeTag == "20150214");
            Assert.IsTrue(data.FactorId == config.FactorId);
            Assert.IsTrue(data.Datas.Count > 0);
            //Assert.IsTrue(data.LastAggDatas != null);
        }

        [Test]
        public void WeekDataPoolTester()
        {
            BaseAggConfig config = CreateCommonConfig(AggType.Week);

            config.TimeRange = new AggTimeRange { DataBeginHour = 1, DataEndHour = 2, DateBegin = 1, DateEnd = 1 };

            DataPoolFactory.Init();
            IDataPool dataPool = DataPoolFactory.GetDataPool(config);
            Assert.IsNotNull(dataPool);

            DateTime time = new DateTime(2015, 2, 15);
            AggRawData data = dataPool.GetAggRawData(time);
            int count = 0;
            foreach (var tmp in data.Datas)
            {
                count += tmp.Values.Count;
            }
            Console.WriteLine(string.Format("共读取{0}条数据", count));
            Assert.IsTrue(data.TimeTag == "2015W7");
            Assert.IsTrue(data.FactorId == config.FactorId);
            Assert.IsTrue(data.Datas.Count > 0);
            //Assert.IsTrue(data.LastAggDatas != null);
        }

        [Test]
        public void MonthDataPoolTester()
        {
            BaseAggConfig config = CreateCommonConfig(AggType.Month);

            config.TimeRange = new AggTimeRange { DataBeginHour = 1, DataEndHour = 3, DateBegin = 1, DateEnd = 1 };

            DataPoolFactory.Init();
            IDataPool dataPool = DataPoolFactory.GetDataPool(config);
            Assert.IsNotNull(dataPool);

            DateTime time = new DateTime(2015, 2, 15);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            AggRawData data = dataPool.GetAggRawData(time);
            sw.Stop();
            Console.WriteLine(string.Format("MonthDataPoolTester 耗时:{0}ms",sw.ElapsedMilliseconds));
            int count = 0;
            foreach (var tmp in data.Datas)
            {
                count += tmp.Values.Count;
            }
            Console.WriteLine(string.Format("共读取{0}条数据", count));
            Assert.IsTrue(data.TimeTag == "201502");
            Assert.IsTrue(data.FactorId == config.FactorId);
            Assert.IsTrue(data.Datas.Count > 0);
            //Assert.IsTrue(data.LastAggDatas != null);
        }
    }
}
