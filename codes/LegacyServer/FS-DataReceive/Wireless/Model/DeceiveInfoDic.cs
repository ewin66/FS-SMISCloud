using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using DataCenter.Accessor.ViewBLL;
using log4net;

namespace DataCenter.Model
{
    public class DeceiveInfoDic
    {
        //                  <dtuid ,<  structid  producttype ,       <modudle,   list<sensor>>> 
        public readonly ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<string, IList<SensorInfo>>>> DeceiveInfos = new ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<string, IList<SensorInfo>>>>();

        public ConcurrentDictionary<string, DateTime> LastAcqTime = new ConcurrentDictionary<string, DateTime>();
        
        public ConcurrentDictionary<string, ConcurrentDictionary<string, DateTime>> LastDateTime = new ConcurrentDictionary<string, ConcurrentDictionary<string, DateTime>>();
        
        private static readonly ILog Log = LogManager.GetLogger(typeof(DeceiveInfoDic));

        private DeceiveInfoDic()
        {
            var bll = new DeviceInfoBll();

            IList<SensorInfo> devices = bll.GetAllDeviceInfo();
            foreach (SensorInfo sensor in devices)
            {
                if (!this.DeceiveInfos.ContainsKey(sensor.RemoteDtuNumber))
                {
                    this.DeceiveInfos.TryAdd(
                        sensor.RemoteDtuNumber,
                        new ConcurrentDictionary<int, ConcurrentDictionary<string, IList<SensorInfo>>>());
                }
                if (!this.DeceiveInfos[sensor.RemoteDtuNumber].ContainsKey(sensor.ProductTypeKey))
                {
                    this.DeceiveInfos[sensor.RemoteDtuNumber].TryAdd(
                        sensor.ProductTypeKey,
                        new ConcurrentDictionary<string, IList<SensorInfo>>());
                }
                if (!this.DeceiveInfos[sensor.RemoteDtuNumber][sensor.ProductTypeKey].ContainsKey(sensor.DaiModuleNo))
                {
                    this.DeceiveInfos[sensor.RemoteDtuNumber][sensor.ProductTypeKey].TryAdd(
                        sensor.DaiModuleNo,
                        new List<SensorInfo>());
                }

                this.DeceiveInfos[sensor.RemoteDtuNumber][sensor.ProductTypeKey][sensor.DaiModuleNo].Add(sensor);
            }
        }

        private static object obj = new object();

        private static DeceiveInfoDic deceiveInfoDic;

        public static DeceiveInfoDic GetDeceiveInfoDic()
        {
            if (deceiveInfoDic == null)
            {
                lock (obj)
                {
                    if (deceiveInfoDic == null)
                    {
                        deceiveInfoDic=new DeceiveInfoDic();
                    }
                }
            }

            return deceiveInfoDic;
        }
        
        public IList<SensorInfo> GeSensorInfosByChannel(
    string dtunum,
    int sensortype,
    string moduleId,
    int channelid)
        {
            IList<SensorInfo> sensorlist = new List<SensorInfo>();
            if (this.DeceiveInfos.ContainsKey(dtunum) && DeceiveInfos[dtunum].ContainsKey(sensortype)
                && DeceiveInfos[dtunum][sensortype].ContainsKey(moduleId))
            {
                foreach (SensorInfo sensor in DeceiveInfos[dtunum][sensortype][moduleId])
                {
                    if (sensor.DaiChannelNumber == channelid)
                    {
                        sensorlist.Add(sensor);
                    }
                }
            }
            else
            {
                StringBuilder str = new StringBuilder();
                str.Append("未找到该传感器:").Append(string.Format("[{0},{1},{2}]", dtunum, moduleId, channelid));
                Log.ErrorFormat(str.ToString());
            }

            return sensorlist;
        }


    }
}
