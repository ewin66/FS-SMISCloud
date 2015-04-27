
using System.Diagnostics;

namespace FS.SMIS_Cloud.DAC.Util
{
    public class Performance
    {
        private  static string CNT_DAC = "FSDAC";
        /// <summary>
        /// 获取计数器: 名称.实例, 勿用, 有安全异常.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PerformanceCounter GetCounter(string name)
        {

            string[] ns = name.Split('.');
            var categoryName = CNT_DAC;//ns[0];
            var counterName = ns[0];
            var instantName = ns.Length >= 2 ? ns[1] : "";

            // new PerformanceCounter(“计数器类型名称”，“计数器名称”，“计数器实例名称”）
            if (!PerformanceCounterCategory.Exists(CNT_DAC))
            {
                CounterCreationDataCollection ccdc = new CounterCreationDataCollection();
                //计数器  
                CounterCreationData ccd = new CounterCreationData();//
                ccd.CounterType = PerformanceCounterType.NumberOfItems64;
                ccd.CounterName = counterName;
                ccdc.Add(ccd);
                PerformanceCounterCategory.Create(
                    CNT_DAC,
                    "Desc of FSDAC", 
                    instantName==""? PerformanceCounterCategoryType.SingleInstance: PerformanceCounterCategoryType.MultiInstance, 
                    ccdc);  
            }
            PerformanceCounter counter = new PerformanceCounter()
            {
                CategoryName = categoryName,
                CounterName = counterName,
                InstanceName =  instantName,
                InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
                ReadOnly = false,
                RawValue = 0
            };
            return counter;
        }
    }
}
