#define READ_GPS_FIRST_NO
// --------------------------------------------------------------------------------------------
// <copyright file="Gps_HC_SensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20141024
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FS.SMIS_Cloud.DAC.Model;
using log4net;

namespace FS.SMIS_Cloud.DAC.DAC.CxxAdapter
{
    [SensorAdapter(Protocol = ProtocolType.GPS_HC)]
    public class Gps_HC_SensorAdapter : GpsBaseAdapter
    {
        public Gps_HC_SensorAdapter()
        {
            _idxModule = 1;
            _idxTime = 2;
            _idxDX = 10;
            _idxDY = 11;
            _idxDH = 12;
            _log = LogManager.GetLogger("GpsHcAdapter");
            _dtformat = new DateTimeFormatInfo { ShortDatePattern = "yyyy/MM/dd" };
        }
#if READ_GPS_FIRST
        public override IEnumerable<byte[]> ReadData(Sensor si, string filePath)
        {
            var rslt = new List<byte[]>();
            var sLastTime = this.GetSensorLastDataAcqTime(si.SensorID);
            if (sLastTime == null)
            {
                sLastTime = DateTime.Now;
                this.UpdateSensorLastDataAcqTime(si.SensorID, sLastTime.Value);
            }

            string[] datas = System.IO.File.ReadAllLines(filePath);
            for (int linenum = datas.Length - 1; linenum >= 0; linenum--)
            {
                var data = datas[linenum];
                if (string.IsNullOrEmpty(data)) continue;
                string[] d = data.Split(',');
                string fId = d[_idxModule];
                if (fId == si.ModuleNo.ToString())
                {
                    DateTime fTime = Convert.ToDateTime(d[_idxTime], _dtformat);
                    if (fTime > sLastTime)
                    {
                        rslt.Add(System.Text.Encoding.Default.GetBytes(data));
                        this.UpdateSensorLastDataAcqTime(si.SensorID, fTime);
                    }
                    break;
                }
            }
            return rslt;
        }
#else
        public override IEnumerable<byte[]> ReadData(Sensor si, string filePath)
        {
            var rslt = new List<byte[]>();
            DateTime sLastTime = this.GetSensorLastDataAcqTime(si.SensorID) ?? new DateTime();
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(fs))
                {
                    reader.ReadLine();
                    string data;
                    while ((data = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(data)) continue;
                        string[] d = data.Split(',');
                        string fId = d[_idxModule];
                        DateTime fTime = Convert.ToDateTime(d[_idxTime], _dtformat);
                        if (fId == si.ModuleNo.ToString() && fTime > sLastTime)
                        {
                            rslt.Add(System.Text.Encoding.Default.GetBytes(data));
                            this.UpdateSensorLastDataAcqTime(si.SensorID, fTime);
                        }
                    }
                    reader.Close();
                }
                fs.Close();
            }
            return rslt;
        }
#endif
    }
}