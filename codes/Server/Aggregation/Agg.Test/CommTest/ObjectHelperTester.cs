using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace Agg.Test
{
    using Agg.Comm.DataModle;
    using Agg.Comm.Util;
    
    [TestFixture]
    public class ObjectHelperTester
    {
        [Test]
        public void AggResultDeepCopyTest()
        {
            AggResult result = AggDataStorageTester.CreateData("20150101", AggType.Day);
            AggResult copy = ObjectHelper.DeepCopy(result);
            Assert.IsNotNull(copy);

            Assert.IsTrue(copy.AggType == result.AggType);
            Assert.IsTrue(copy.StructId == result.StructId);

            copy.LastAggDatas.Clear();
            Assert.IsFalse(copy.LastAggDatas.Count == result.LastAggDatas.Count);
        }
    }
}
