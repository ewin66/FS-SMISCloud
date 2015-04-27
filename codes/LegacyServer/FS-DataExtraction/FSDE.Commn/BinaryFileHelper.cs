#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="BinaryFileHelper.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140620 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Commn
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using FSDE.Model;

    public class BinaryFileHelper
    {
        public static int AD_LSB_COUNT = 65536; // LSB码数量

        public static int AD_LSB_HALF = 32768; // 当取偏移码时，其原点位置

        public static bool Read(string filename, out ArtAccDatum data, out string err)
        {
            bool flag = true;
            data = new ArtAccDatum();
            err = "";
            try
            {
                if (!File.Exists(filename))
                {
                    err = "数据文件不存在";
                    return false;
                }
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var br = new BinaryReader(fs);
                    data.HeadSizeBytes = br.ReadInt32();
                    data.FileHeader =
                        (FileHeader)
                        Serial2StructConverter.BytesToStruct(br.ReadBytes(data.HeadSizeBytes - 4), typeof(FileHeader));
                    int nChannelCnt = data.FileHeader.ADPara.LastChannel - data.FileHeader.ADPara.FirstChannel + 1;
                    float PerLsbVolt =
                        (float)((data.FileHeader.VoltTopRange - data.FileHeader.VoltBottomRange) / (float)AD_LSB_COUNT);
                    long DatumCnt = (fs.Length - data.HeadSizeBytes) / 2 / nChannelCnt; //计算数据项个数
                    data.ChannelCount = nChannelCnt;
                    data.DatumCount = DatumCnt;
                    data.AllocSpace(); //分配数据空间
                    for (int i = 0; i < DatumCnt; i++)
                    {
                        for (int ch = 0; ch < nChannelCnt; ch++)
                        {
                            data.MilliVolt[ch][i] = (br.ReadUInt16() - AD_LSB_HALF) * PerLsbVolt;
                        }
                    }
                    Console.WriteLine(data.MilliVolt[nChannelCnt - 1][DatumCnt - 1]); //DEBUG最后一条数据
                    fs.Close();
                }

            }
            catch (Exception ex)
            {
                flag = false;
                err = ex.Message;
            }
            return flag;
        }

        public static string[] GetFileNames(string path, string format, DateTime time, TxtDateType type)
        {
            var res = new List<string>();
            try
            {
                string[] files = Directory.GetFiles(path, format, SearchOption.TopDirectoryOnly);
                //查找根目录下的所有保存的振动数据"*.usb"
                foreach (string fileName in files)
                {
                    try
                    {
                        DateTime recordTime = GetFileRecordTime(fileName, type);

                        if (recordTime > time)
                        {
                            File.Open(fileName, FileMode.Open, FileAccess.ReadWrite).Close(); //文件正在被写入，无法提取
                            res.Add(fileName);
                        }
                    }
                    catch
                    {
                    }
                }
                return res.ToArray();
            }
            catch
            {
                return new string[] { };
            }
        }

        public static string[] GetFileNames(string path, string format,TxtDateType type)
        {
            var res = new List<string>();
            try
            {
                string[] files = Directory.GetFiles(path, format, SearchOption.TopDirectoryOnly);
                    //查找根目录下的所有保存的振动数据"*.usb"
                foreach (string fileName in files)
                {
                    try
                    {
                        File.Open(fileName, FileMode.Open, FileAccess.ReadWrite).Close(); //文件正在被写入，无法提取
                        res.Add(fileName);
                    }
                    catch
                    {
                    }
                }
                return res.ToArray();
            }
            catch
            {
                return new string[]{};
            }
        }

        public static DateTime GetFileRecordTime(string filepath,TxtDateType type)
        {
            DateTime time;
            switch (type)
            {
                case TxtDateType.Vibration:
                    return DateTime.ParseExact(
                        Path.GetFileNameWithoutExtension(filepath).Remove(0, 4),
                        "yyyyMMddHHmmss",
                        null);

                //case TxtDateType.FiberGrating:
                //    return DateTime.ParseExact(
                //        Path.GetFileNameWithoutExtension(filepath).Remove(0, 4),
                //        "yyyyMMddHHmmss",
                //        null);

                default:
                    return new DateTime(2013,1,1);
                    break;
            }
        }


        

    }


    public enum TxtDateType
    {
        Vibration,
        FiberGrating
    }

}