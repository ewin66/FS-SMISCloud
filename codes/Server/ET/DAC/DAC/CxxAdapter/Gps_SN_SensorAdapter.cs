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
    [SensorAdapter(Protocol = ProtocolType.GPS_SN)]
    public class Gps_SN_SensorAdapter : GpsBaseAdapter
    {
        public Gps_SN_SensorAdapter()
        {
            _idxModule = 1;
            _idxTime = 4;
            _idxDX = 10;
            _idxDY = 11;
            _idxDH = 12;
            _log = LogManager.GetLogger("GpsSnAdapter");
            _dtformat = null;
        }

        public override IEnumerable<byte[]> ReadData(Sensor si, string filePath)
        {
            var rslt = new List<byte[]>();
            DateTime sLastTime = this.GetSensorLastDataAcqTime(si.SensorID) ?? new DateTime();
            var mod = si.ModuleNo.ToString(CultureInfo.InvariantCulture).Trim();
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(fs))
                {
                    string data;
                    while ((data = reader.ReadLine()) != null)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(data)) continue;
                            string[] d = data.Split(',');
                            string fId = d[_idxModule].Trim();
                            DateTime fTime = Convert.ToDateTime(d[0] + " " + d[_idxTime], _dtformat);
                            if (StringComparer.OrdinalIgnoreCase.Compare(mod, fId) == 0 && fTime > sLastTime)
                            {
                                data = data.Replace(d[_idxTime], fTime.ToString(_dtformat));
                                rslt.Add(System.Text.Encoding.Default.GetBytes(data));
                                this.UpdateSensorLastDataAcqTime(si.SensorID, fTime);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WarnFormat("GPS数据解析异常:{0} {1}", data, ex.Message);
                        }
                    }
                    reader.Close();
                }
                fs.Close();
            }
            return rslt;
        }
    }
}