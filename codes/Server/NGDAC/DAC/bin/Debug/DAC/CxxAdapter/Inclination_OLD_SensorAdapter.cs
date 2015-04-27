#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="Inclination_OLD_SensorAdapter.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140912 by WIN .
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
using System.Reflection;

using log4net;

namespace FS.SMIS_Cloud.DAC.DAC.CxxAdapter
{
    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    //协议参考文档： Documents/PMO/02 产品协议/我司产品协议/通信协议/09固定测斜协议.xls
     [SensorAdapter(Protocol = ProtocolType.Inclinometer_OLD)]
    public class Inclination_OLD_SensorAdapter : ISensorAdapter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int IDX_FrameH = 0;  // 帧头 0xfe 0x43 0x58
        private const int IDX_DEST = 3;    // 接收端地址（高地址在前）
        private const int IDX_SOURCE = 5;  // 发送端地址（高地址在前）
                                           // 当发送端或接收端为PC时，地址为 00 
        private const int IDX_CMD = 7;     // 数据采集命令 == 0x00
        private const int IDX_CHSUM = 15;  // 校验码
        private const int IDX_FRAMEL = 16; // 帧尾=0xef
                                           // 数据包
        private const int IDX_XDATA = 7;   // X方向角度
        private const int IDX_YDATA = 11;  // Y方向角度

        public void Request(ref SensorAcqResult sensorAcq)
        {
            try
            {
                if (sensorAcq.Sensor.ModuleNo > ushort.MaxValue)
                {
                    sensorAcq.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                    sensorAcq.Request = null;
                    return;
                }
                var package = new byte[17];
                package[IDX_FrameH] = 0xfe;
                package[IDX_FrameH + 1] = 0x43;
                package[IDX_FrameH + 2] = 0x58;
                package[IDX_FRAMEL] = 0xef;
                ValueHelper.WriteUShort_BE(package, IDX_DEST, (ushort)sensorAcq.Sensor.ModuleNo);
                package[IDX_CHSUM] = ValueHelper.CheckXor(package, 0, IDX_CHSUM);
                sensorAcq.ErrorCode = (int)Errors.SUCCESS;
                sensorAcq.Request = package;
            }
            catch 
            {
                sensorAcq.Request = null;
                sensorAcq.ErrorCode = (int)Errors.ERR_UNKNOW;
            }
        }

         public void ParseResult(ref SensorAcqResult rawData)
         {
             rawData.ErrorCode = IsValid(rawData.Response);
             if (rawData.ErrorCode != (int)Errors.SUCCESS)
                 return;
             int module = ((rawData.Response[IDX_SOURCE] << 8) | (rawData.Response[IDX_SOURCE + 1]));
             if (module != rawData.Sensor.ModuleNo)
             {
                 rawData.ErrorCode = (int)Errors.ERR_INVALID_MODULE;
                 return;
             }
             try
             {
                 byte xminus = rawData.Response[IDX_XDATA];
                 double xdegree = rawData.Response[IDX_XDATA + 1];
                 double xcent = rawData.Response[IDX_XDATA + 2]/60.0;
                 double xsec = rawData.Response[IDX_XDATA + 3]/3600.0;

                 byte yminus = rawData.Response[IDX_YDATA];
                 double ydegree = rawData.Response[IDX_YDATA + 1];
                 double ycent = rawData.Response[IDX_YDATA + 2]/60.0;
                 double ysec = rawData.Response[IDX_YDATA + 3]/3600.0;

                 double angleX = xdegree + xcent + xsec;
                 double angleY = ydegree + ycent + ysec;

                 if (xminus == 0x0D)
                 {
                     angleX = -angleX;
                 }
                 if (yminus == 0x0D)
                 {
                     angleY = -angleY;
                 }
                 double changedx = 0;
                 double changedy = 0;
                 double r = 0;

                 if (rawData.Sensor.FormulaID == 2 && rawData.Sensor.Parameters != null &&
                     rawData.Sensor.ParamCount == 3)
                 {
                     double xinit = rawData.Sensor.Parameters[0].Value;
                     double yinit = rawData.Sensor.Parameters[1].Value;
                     double len = rawData.Sensor.Parameters[2].Value;
                     var xvalue = (len*Math.Sin(angleX*Math.PI/180)) - (len*Math.Sin(xinit*Math.PI/180));
                     var yvalue = (len*Math.Sin(angleY*Math.PI/180)) - (len*Math.Sin(yinit*Math.PI/180));
                     changedx = xvalue;
                     changedy = yvalue;
                 }
                 else if (rawData.Sensor.FormulaID == 6 && rawData.Sensor.Parameters != null &&
                          rawData.Sensor.ParamCount == 2)
                 {
                     double xinit = rawData.Sensor.Parameters[0].Value;
                     double yinit = rawData.Sensor.Parameters[1].Value;
                     var xvalue = angleX - xinit;
                     var yvalue = angleY - yinit;
                     changedx = xvalue;
                     changedy = yvalue;
                 }
                 else if (rawData.Sensor.FormulaID == 30 && rawData.Sensor.Parameters != null &&
                          rawData.Sensor.ParamCount == 3)
                 {
                     double xinit = rawData.Sensor.Parameters[0].Value;
                     double yinit = rawData.Sensor.Parameters[1].Value;
                     double len = rawData.Sensor.Parameters[2].Value*1000;
                     var xvalue = (len * Math.Sin(angleX * Math.PI / 180)) - (len * Math.Sin(xinit * Math.PI / 180));
                     var yvalue = (len * Math.Sin(angleY * Math.PI / 180)) - (len * Math.Sin(yinit * Math.PI / 180));

                     r = Math.Sqrt(xvalue * xvalue + yvalue * yvalue);
                     changedx = xvalue;
                     changedy = yvalue;
                 }

                 var raws = new double[] { angleX, angleY };
                 var phys = new double[] { changedx, changedy, r };

                 rawData.Data = new SensorData(raws, phys, raws)
                 {
                     JsonResultData = string.Format("{0}\"sensorId\":{1},\"data\":\"X方向角度:{2} °,Y方向角度:{3} °\"{4}", '{', rawData.Sensor.SensorID, angleX, angleY, '}')
                 };

                 //rawData.Data = new InclinationData(angleX, angleY, changedx, changedy)
                 //{
                 //    JsonResultData =
                 //        string.Format("{0}\"sensorId\":{1},\"data\":\"X方向角度:{2} °,Y方向角度:{3} °\"{4}", '{',
                 //            rawData.Sensor.SensorID, angleX, angleY, '}')
                 //};
             }
             catch (Exception ex)
             {
                 rawData.ErrorCode = (int)Errors.ERR_DATA_PARSEFAILED;
                 _logger.ErrorFormat("old Inclination sensor [Id:{0} m: {1}] parsedfailed,received bytes{2}, ERROR: {3}",
                         rawData.Sensor.SensorID, rawData.Sensor.ModuleNo, ValueHelper.BytesToHexStr(rawData.Response),
                         ex.Message);
             }
         }

         private int IsValid(byte[] data)
        {
            if (data.Length != 17)
                return (int)Errors.ERR_INVALID_DATA;
            if (data[IDX_FrameH] != 0xfe || data[IDX_FrameH + 1] != 0x43 || data[IDX_FrameH+2] != 0x58 || data[IDX_FRAMEL] != 0xef)
                return (int)Errors.ERR_INVALID_DATA;
            byte check = data[IDX_CHSUM];
            if (check != ValueHelper.CheckXor(data, 0, IDX_CHSUM))
                return (int)Errors.ERR_CRC;
            return (int)Errors.SUCCESS;
        }
    }
}