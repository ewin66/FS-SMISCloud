using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPSDataImporter
{
    using System.Configuration;
    using System.Reflection;
    using System.Threading;

    using log4net;

    class Program
    {
        private static bool isImporting = false;
        private static Timer timer;
        [STAThread]
        private static void Main(string[] args)
        {
            int period = Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);
            Program.timer = new Timer(new TimerCallback(Program.timer_Elapsed), null, 0, period);
            Console.ReadLine();
        }
        private static void timer_Elapsed(object sender)
        {
            if (Program.isImporting)
            {
                return;
            }
            Program.isImporting = true;
            Console.WriteLine("{0}:", DateTime.Now);
            Console.WriteLine("==================================================================");
            ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            DataImporter instance = DataImporter.GetInstance();
            try
            {
                instance.ImportData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生异常：\n{0}\n请查看日志", ex.Message);
                logger.Error("异常", ex);
            }
            finally
            {
                Program.isImporting = false;
            }
            Console.WriteLine("==================================================================");
        }
    }
}
