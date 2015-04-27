#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ReloveFiberGratingData.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140619 by WIN .
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
using System.Collections.Generic;
using DataCenter.Model;

namespace DataCenter.WirelessCustomTransmission
{
    /// <summary>
    /// 光栅光纤数据
    /// </summary>
    public class ReloveFiberGratingData:IReloveTransData
    {
        private const int ProjectCodeOfFrame = 2;
        private const int TypeOfDataOfFrame = 4;
        private const int LengthOfFrame = 5;
        private const int ModuleNumOfFrame = 7;
        private const int ChannelIdOfFrame = 9;
        private const int AcqTimeOfFrame = 20;
        private const int DataValueOfFrame = 28;


        public void ReloveReceivedData(string dtuid, byte[] bytes)
        {
            string dtunum = dtuid;
            int sensortype = bytes[TypeOfDataOfFrame];
            string moduleId = BitConverter.ToInt16(bytes, ModuleNumOfFrame).ToString();
            int channelid = bytes[ChannelIdOfFrame];
            short wavelengthIndex = 0;


            IList<SensorInfo> sensorlist = new List<SensorInfo>();
            foreach (SensorInfo sensor in DeceiveInfoDic.GetDeceiveInfoDic().DeceiveInfos[dtunum][sensortype][moduleId])
            {
                if (sensor.DaiChannelNumber == channelid&&short.Parse(sensor.UniqueSign)==wavelengthIndex)
                {
                    sensorlist.Add(sensor);
                }
            }

            if (sensorlist.Count == 1)
            {
                int sensorid = sensorlist[0].SensorId;
                int safetype = sensorlist[0].SafetyFactorTypeId;
                int count = bytes[10];
                int index = bytes[11];
                int floatcount = (bytes.Length - 31) / 4;
                var values = new List<float>();
                for (int i = 0; i < floatcount; i++)
                {
                    float value = BitConverter.ToSingle(bytes, DataValueOfFrame + (4 * i));
                    values.Add(value);
                }
            }
            
            //光栅光纤数据

        }
    }
}