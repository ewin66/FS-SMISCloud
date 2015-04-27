using System;
using System.Collections.Generic;

namespace Agg.Process
{
    using System.Collections.Concurrent;

    using Agg.Comm.DataModle;

    using log4net;

    public abstract class ProcessFactory
    {
        private static ConcurrentDictionary<AggWay,IAggProcess> processes = new ConcurrentDictionary<AggWay, IAggProcess>();

        private static Dictionary<string, Type> processTypes = new Dictionary<string, Type>(); 
        private static string ConfigFileName = AppDomain.CurrentDomain.BaseDirectory + " \\AggProcess.xml";

        private static ILog Log = LogManager.GetLogger("ProcessFactory");
        public static void Init()
        {
            processTypes = ProcessConfig.FromXML(ConfigFileName);
        }


        public static IAggProcess GetAggProcess(AggWay way)
        {
            IAggProcess process = null;
            if (processes.ContainsKey(way))
            {
                processes.TryGetValue(way, out process);
            }
            else
            {
                process = CreateAggProcess(way);
                if (process != null)
                {
                    processes.TryAdd(way, process);
                }
            }
            return process;
        }

        private static IAggProcess CreateAggProcess(AggWay way)
        {
            IAggProcess iProcess = null;
            if (!processTypes.ContainsKey(way.ToString()))
            {
                iProcess = null;         
                return iProcess;
            }
            Type type = processTypes[way.ToString()];

            try
            {
                iProcess = Activator.CreateInstance(type) as IAggProcess;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("create aggprocess failed, type:{0},error:{1},stacktrace{2}", way.ToString(), e.Message, e.StackTrace);
            }
            return iProcess;

        }
    }
}
