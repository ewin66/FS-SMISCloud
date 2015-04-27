using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregation
{
    using log4net;
    using Agg.Storage;
    using Agg.DataPool;
    class Program
    {
        private static ILog log = LogManager.GetLogger("Program");
        static void Main(string[] args)
        {
            log.Info("Start...");
            AggService service = new AggService("AggService.xml", AppDomain.CurrentDomain.BaseDirectory);
            Console.ReadKey();
        }
    }
}
