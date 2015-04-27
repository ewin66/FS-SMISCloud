#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ReloveBigDataTransData.cs" company="江苏飞尚安全监测咨询有限公司">
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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DataCenter.Model;
using ET.Common;
using log4net;
using R.WirelessCustomTransmission;

namespace DataCenter.WirelessCustomTransmission
{
    public class ReloveBigDataTransData : IReloveTransData
    {
        private const int ProjectCodeOfFrame = 2;
        private const int TypeOfDataOfFrame = 4;
        private const int LengthOfFrame = 5;
        private const int ModuleNumOfFrame = 7;
        private const int ChannelIdOfFrame = 9;

        private const int Structure = 18;
        private const int AcqTimeOfFrame = 22;
        private const int DataValueOfFrame = 30;

        private static readonly ILog Log = LogManager.GetLogger(typeof(ReloveBigDataTransData));
        private static string path = System.Configuration.ConfigurationManager.AppSettings["VibrationPath"];

        public ReloveBigDataTransData()
        {
            if (!path.EndsWith(@"\"))
            {
                path += @"\";
            }
        }

        public void ReloveReceivedData(string dtuid, byte[] bytes)
        {
            int sensortype = bytes[TypeOfDataOfFrame];
            string moduleId = BitConverter.ToInt16(bytes, ModuleNumOfFrame).ToString();
            int channelid = bytes[ChannelIdOfFrame];

            IList<SensorInfo> sensorlist = DeceiveInfoDic.GetDeceiveInfoDic().GeSensorInfosByChannel(dtuid, sensortype, moduleId, channelid);


            if (sensorlist.Count == 1)
            {
                int sensorid = sensorlist[0].SensorId;
                int safetype = sensorlist[0].SafetyFactorTypeId;
                int count = bytes[10];
                int index = bytes[11];
                double fre = BitConverter.ToSingle(bytes, 12);
                // ValueHelper.GetFloat(bytes, 12);//计算时间？

                int structureId = BitConverter.ToInt32(bytes, Structure);
                long ticks = BitConverter.ToInt64(bytes, AcqTimeOfFrame);
                var time = new DateTime(ticks);
                int floatcount = (bytes.Length - 33) / 4;
                double[] data = new double[floatcount];
                for (int i = 0; i < floatcount; i++)
                {
                    data[i] = BitConverter.ToSingle(bytes, DataValueOfFrame + (4 * i)); //ValueHelper.GetFloat(bytes, DataValueOfFrame + (4 * i));
                }
                var fileParam = new FileParamStruct();
                fileParam.diSampleFreq = fre;
                fileParam.diTestPointNum = moduleId + "_" + channelid;
                StringBuilder filename = new StringBuilder();
                filename.Append(fileParam.diTestPointNum).Append("_").Append(time.ToString("yyyyMMddHHmmssfff"));
                if (bytes[16] == 1)
                {
                    filename.Append("_TRI");
                }
                try
                {
                    //Directory.CreateDirectory(path);
                    DirectoryInfo dir = new DirectoryInfo(path);
                    if (!dir.Exists)
                    {
                        dir.Create();
                    }
                    string filePath = path + filename;
                    CreateParamsFile(filePath + ".sdb", fileParam);
                    CreateDataFile(filePath + ".odb", data);
                }
                catch (Exception ex)
                {
                    Log.FatalFormat(ex.Message);
                }

                if (index == count - 1)
                {
                    string filePath = path + filename;
                    MakeMsgToDataCalc.MakeMsgToRequestDataCalc(sensorid, filePath, time);
                }
            }
            else
            {
                Log.Error("不能确定唯一传感器:" + ValueHelper.ByteToHexStr(bytes));
            }
        }

        private static Byte[] StructToBytes(Object structure)
        {
            Int32 size = Marshal.SizeOf(structure);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, buffer, false);
                Byte[] bytes = new Byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private void CreateParamsFile(string strFilenameWithoutExt, object obj)
        {
            if (File.Exists(strFilenameWithoutExt))
                return;
            using (FileStream fs = new FileStream(strFilenameWithoutExt, FileMode.Create))
            {

                var bs = new BinaryWriter(fs);
                bs.Write(StructToBytes(obj));
                bs.Close();
            }
        }

        private void CreateDataFile(string strFilenameWithoutExt, double[] data)
        {
            using (FileStream fs = new FileStream(strFilenameWithoutExt, FileMode.Append))
            {
                var bs = new BinaryWriter(fs);
                foreach (double f in data)
                {
                    bs.Write(f);
                }
                bs.Close();
            }
        }
    }
}