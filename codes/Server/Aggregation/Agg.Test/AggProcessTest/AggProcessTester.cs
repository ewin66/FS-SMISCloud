using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agg.Test
{
    using Agg.Comm.DataModle;
    using Agg.Process;

    using NUnit.Framework;

    [TestFixture]
    class AggProcessTester
    {
        [Test]
        public void ProcessCreateTester()
        {
            ProcessFactory.Init();
            IAggProcess process = ProcessFactory.GetAggProcess(AggWay.Avg);
            Assert.IsNotNull(process);
            process = ProcessFactory.GetAggProcess(AggWay.Min);
            Assert.IsNotNull(process);
            process = ProcessFactory.GetAggProcess(AggWay.Max);
            Assert.IsNotNull(process);
        }

        [Test]
        public void AvgProcessTester()
        {
            ProcessFactory.Init();
            IAggProcess process = ProcessFactory.GetAggProcess(AggWay.Avg);

            AggResult result = process.AggProcess(CreateAggRawData());

            Assert.IsNotNull(result);

            foreach (var aggdata in result.AggDatas)
            {
                Assert.IsTrue(aggdata.Values.Count==3);
                foreach (var value in aggdata.Values)
                {
                    Assert.IsTrue(value == Convert.ToDouble(200));
                }
            }

        }

        [Test]
        public void MaxProcessTester()
        {
            ProcessFactory.Init();
            IAggProcess process = ProcessFactory.GetAggProcess(AggWay.Max);

            AggResult result = process.AggProcess(CreateAggRawData());

            Assert.IsNotNull(result);

            foreach (var aggdata in result.AggDatas)
            {
                Assert.IsTrue(aggdata.Values.Count == 3);
                foreach (var value in aggdata.Values)
                {
                    Assert.IsTrue(value == Convert.ToDouble(300));
                }
            }

        }

        [Test]
        public void MinProcessTester()
        {
            ProcessFactory.Init();
            IAggProcess process = ProcessFactory.GetAggProcess(AggWay.Min);

            AggResult result = process.AggProcess(CreateAggRawData());

            Assert.IsNotNull(result);

            foreach (var aggdata in result.AggDatas)
            {
                Assert.IsTrue(aggdata.Values.Count == 3);
                foreach (var value in aggdata.Values)
                {
                    Assert.IsTrue(value == Convert.ToDouble(100));
                }
            }

        }

        private AggRawData CreateAggRawData()
        {
            AggRawData aggdata = new AggRawData(new AggTaskKey(1,1,AggType.Day),1, "201501");
            aggdata.Datas = new List<RawData>();
            RawData data = new RawData();
            data.SensorId = 100;
            data.Values = new List<List<double>>();
            
            for (int  i= 0; i < 3; i++)
            {
                List<double> item = new List<double>();
                for (int j = 0; j < 3; j++)
                {
                    item.Add((i+1)*100);
                }
                data.Values.Add(item);
            }
            aggdata.Datas.Add(data);
            return aggdata;
        }
    }
}
