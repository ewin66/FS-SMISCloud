using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace Agg.Test
{
    using Agg.Comm.Util;

    [TestFixture]
    public class DateTimeHelperTester
    {
        [Test]
        public void GetWeekOfYearTester()
        {
            DateTime time = new DateTime(2015,2,14);
            int w = DateTimeHelper.GetWeekOfYear(time);
            Assert.IsTrue(w == 7);
        }
    }
}
