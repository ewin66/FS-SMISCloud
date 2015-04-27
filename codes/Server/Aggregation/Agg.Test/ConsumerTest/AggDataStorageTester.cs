using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace Agg.Test
{
    using Agg.Comm.DataModle;
    using Agg.Process;

    [TestFixture]
    public class AggDataStorageTester
    {
        [Test]
        [Category("MANUAL")]
        public void SaveDayAggDataTester()
        {
            AggResult result = CreateData("20150110", AggType.Day);

            AggDataChangeCaculate caculate = new AggDataChangeCaculate();
            caculate.ProcessAggResult(ref result);

            Assert.IsNotNull(result.AggDataChanges);
            Assert.IsTrue(result.AggDataChanges.Count == result.AggDatas.Count);

            int num = 1;
            foreach (var aggdata in result.AggDataChanges)
            {
                foreach (var val in aggdata.Values)
                {
                    Assert.IsTrue(val == Convert.ToDouble(-10 * num));
                    num++;
                }
            }
            AggDataStorage storage = new AggDataStorage();
            Assert.IsTrue(storage.ProcessAggResult(ref result)); /// insert test
            Assert.IsTrue(storage.ProcessAggResult(ref result)); /// updatetest
        }

        [Test]
        [Category("MANUAL")]
        public void SaveWeekAggDataTester()
        {
            AggResult result = CreateData("2015W2", AggType.Week);

            AggDataChangeCaculate caculate = new AggDataChangeCaculate();
            caculate.ProcessAggResult(ref result);

            Assert.IsNotNull(result.AggDataChanges);
            Assert.IsTrue(result.AggDataChanges.Count == result.AggDatas.Count);

            int num = 1;
            foreach (var aggdata in result.AggDataChanges)
            {
                foreach (var val in aggdata.Values)
                {
                    Assert.IsTrue(val == Convert.ToDouble(-10 * num));
                    num++;
                }
            }
            AggDataStorage storage = new AggDataStorage();
            Assert.IsTrue(storage.ProcessAggResult(ref result)); /// insert test
            Assert.IsTrue(storage.ProcessAggResult(ref result)); /// updatetest
        }

        [Test]
        [Category("MANUAL")]
        public void SaveMonthAggDataTester()
        {
            AggResult result = CreateData("201502", AggType.Month);

            AggDataChangeCaculate caculate = new AggDataChangeCaculate();
            caculate.ProcessAggResult(ref result);

            Assert.IsNotNull(result.AggDataChanges);
            Assert.IsTrue(result.AggDataChanges.Count == result.AggDatas.Count);

            int num = 1;
            foreach (var aggdata in result.AggDataChanges)
            {
                foreach (var val in aggdata.Values)
                {
                    Assert.IsTrue(val == Convert.ToDouble(-10 * num));
                    num++;
                }
            }
            AggDataStorage storage = new AggDataStorage();
            Assert.IsTrue(storage.ProcessAggResult(ref result)); /// insert test
            Assert.IsTrue(storage.ProcessAggResult(ref result)); /// updatetest
        }

        public static AggResult CreateData(string timeTag, AggType type)
        {
            AggResult result = new AggResult(90, 42, timeTag, type,1);
            result.AggDatas = new List<AggData>();
            result.LastAggDatas = new List<AggData>();
            AggData data = new AggData();
            AggData lastData = new AggData();
            data.SensorId = 1758;
            data.Values = new List<double>();
            lastData.SensorId = 1758;
            lastData.Values = new List<double>();

            for (int i = 0; i < 2; i++)
            {
                data.Values.Add((i + 1) * 10);
                lastData.Values.Add((i + 1) * 20);
            }
            result.AggDatas.Add(data);
            result.LastAggDatas.Add(lastData);
            return result;
        }
    }
}
