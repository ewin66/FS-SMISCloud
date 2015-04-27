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
    public class AggDataChangeCaculateTester
    {     
        [Test]
        public void NormalTester()
        {
            AggResult result = new AggResult(1,1,"20150110",AggType.Day,1);
            result.AggDatas = new List<AggData>();
            result.LastAggDatas = new List<AggData>();
            AggData data = new AggData();
            AggData lastData = new AggData();
            data.SensorId = 100;
            data.Values = new List<double>();
            lastData.SensorId = 100;
            lastData.Values = new List<double>();

            for (int i = 0; i < 3; i++)
            {
                data.Values.Add((i+1) * 10);
                lastData.Values.Add((i+1) * 20);
            }
            result.AggDatas.Add(data);
            result.LastAggDatas.Add(lastData);
            
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
        }

        [Test]
        public void MissSomeLastAggDataTester()
        {
            AggResult result = new AggResult(1, 1, "20150110",AggType.Day,1);
            result.AggDatas = new List<AggData>();
            result.LastAggDatas = new List<AggData>();
            AggData data = new AggData();
            AggData lastData = new AggData();
            data.SensorId = 100;
            data.Values = new List<double>();
            lastData.SensorId = 100;
            lastData.Values = new List<double>();

            for (int i = 0; i < 3; i++)
            {
                data.Values.Add((i + 1) * 10);
                
            }
            for (int i = 0; i < 2; i++)
            {
                lastData.Values.Add((i + 1) * 20);
            }
            result.AggDatas.Add(data);
            result.LastAggDatas.Add(lastData);

            AggDataChangeCaculate caculate = new AggDataChangeCaculate();
            caculate.ProcessAggResult(ref result);

            Assert.IsNotNull(result.AggDataChanges);
            Assert.IsTrue(result.AggDataChanges.Count == result.AggDatas.Count);

           // int num = 1;
            foreach (var aggdata in result.AggDataChanges)
            {
                for(int i=0; i<aggdata.Values.Count; i++)
                {
                    if (i != aggdata.Values.Count - 1)
                    {
                        Assert.IsTrue(aggdata.Values[i] == Convert.ToDouble(-10 * (i + 1)));
                    }
                    else
                    {
                        Assert.IsTrue(aggdata.Values[i] == Convert.ToDouble(0));
                    }
                }
            }
        }


        [Test]
        public void NoLastestDataTester()
        {
            AggResult result = new AggResult(1, 1, "20150110", AggType.Day,1);
            result.AggDatas = new List<AggData>();

            AggData data = new AggData();

            data.SensorId = 100;
            data.Values = new List<double>();
           
            for (int i = 0; i < 3; i++)
            {
                data.Values.Add((i + 1) * 10);
               
            }
            result.AggDatas.Add(data);

            AggDataChangeCaculate caculate = new AggDataChangeCaculate();
            caculate.ProcessAggResult(ref result);

            Assert.IsNotNull(result.AggDataChanges);
            Assert.IsTrue(result.AggDataChanges.Count == result.AggDatas.Count);

            foreach (var aggdata in result.AggDataChanges)
            {
                foreach (var val in aggdata.Values)
                {
                    Assert.IsTrue(val == Convert.ToDouble(0));
                }
            }
        }
    }
}
