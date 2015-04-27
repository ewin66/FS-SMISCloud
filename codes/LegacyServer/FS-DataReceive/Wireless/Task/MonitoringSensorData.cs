#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="MonitoringSensorData.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150325 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Timers;
using DataCenter.Accessor;
using log4net;
using Newtonsoft.Json;
using System.IO;

namespace DataCenter.Task
{
    public class MonitoringSensorData
    {

        private static Lazy<MonitoringSensorData> Instance = new Lazy<MonitoringSensorData>(() => new MonitoringSensorData());

        private string depDescFile = string.Empty;

        private MonitoringSensorData()
        {
            depDescFile = string.Format(AppDomain.CurrentDomain.BaseDirectory.EndsWith("\\") ? "{0}config.json" : "{0}\\config.json", AppDomain.CurrentDomain.BaseDirectory);
        }

        public static MonitoringSensorData GetInstance
        {
            get
            {
                return Instance.Value;
            }
        }
        
        private ConcurrentDictionary<int, ConcurrentDictionary<int, Sensor>> senLastTimes = new ConcurrentDictionary<int, ConcurrentDictionary<int, Sensor>>();
        
        private static readonly ILog Log = LogManager.GetLogger(typeof(MonitoringSensorData));
        
        private Timer _timer;
       
        public void Start()
        {
            if (_timer == null)
            {
                _timer = new Timer();
            }
            double interval = 3600000; //60分钟
            string str = ConfigurationManager.AppSettings["timeinterval"];
            if (!string.IsNullOrEmpty(str))
            {
                interval = Convert.ToDouble(str)*60*1000; // 分钟
            }
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Interval = interval;
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
            Scanning();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Scanning();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("timer_Elapsed Scanning error : {0}",ex.Message);
            }
        }

        private void Scanning()
        {
            var config = JsonConvert.DeserializeObject<DepConfig>(File.ReadAllText(depDescFile));
            List<int> structs = new List<int>(config.structIds);
            double interval = Convert.ToDouble(config.warninterval);
            DateTime thisTime = DateTime.Now;
            foreach (var structId in structs)
            {
                try
                {
                    List<int> senlst = SqlDal.GetAllSensorIds(structId);

                    var ds = SqlDal.GetAllSensorData(senlst, thisTime.AddMinutes(0 - interval), thisTime);
                    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    {
                        SqlDal.InsertWarningMsg(structId, senlst);
                        continue;
                    }

                    foreach (var senId in senlst)
                    {
                        try
                        {
                            if (!senLastTimes.ContainsKey(structId))
                            {
                                senLastTimes.TryAdd(structId, new ConcurrentDictionary<int, Sensor>());
                            }
                            if (!senLastTimes[structId].ContainsKey(senId))
                            {
                                senLastTimes[structId].TryAdd(senId, new Sensor { SensorId = senId, LastTime = DateTime.Now });
                            }

                            DateTime time = (from r in ds.Tables[0].AsEnumerable()
                                             where r.Field<int>("SensorId") == senId
                                             orderby r.Field<DateTime>("CollectTime") descending
                                             select r.Field<DateTime>("CollectTime")).FirstOrDefault();

                            if (time != DateTime.MinValue)
                                senLastTimes[structId][senId].LastTime = time;
                            else
                            {
                                SqlDal.InsertWarningMsg(structId, senId);
                                continue;
                            }

                            if ((DateTime.Now - senLastTimes[structId][senId].LastTime).TotalMinutes > interval)
                            {
                                SqlDal.InsertWarningMsg(structId, senId);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.ErrorFormat("Scanning senlst error : {0}", ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Scanning structs error : {0}", ex.Message);
                }
            }
        }
    }

    public class Sensor
    {
        public int SensorId { get; set; }

        public DateTime LastTime { get; set; }
    }
}