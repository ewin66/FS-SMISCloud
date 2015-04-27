#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="OurVibration.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140722 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System.Configuration;

namespace FSDE.DAL.Select
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Commn;
    using Dictionaries.config;
    using IDAL;
    using Model;
    using Model.Config;
    using Model.Vibration;

    public class OurVibration : ITextOrBinarySelectDal
    {
        private readonly bool _isExtractTimeSamplingData;
        public OurVibration()
        {
            var file = new ExeConfigurationFileMap { ExeConfigFilename = @".\config\Params.config" };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            string connectionString = config.AppSettings.Settings["IsExtractTimeSamplingData"].Value;
            if (!Boolean.TryParse(connectionString, out _isExtractTimeSamplingData))
            {
                _isExtractTimeSamplingData = false;
            }
        }
       //  =Boolean.Parse()
        public IList<Data> TextOrBinarySelect(DataBaseName path)
        {
            var list = new List<Data>();
            if (path.DataBaseType == (int)DataBaseType.Vibration)
            {
                //DateTime readtime;
                //bool isSuccess = ValueHelper.String2Time(
                //    ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)path.ID, string.Empty).Acqtime,out readtime);
                //if (!isSuccess)
                //{
                //    readtime = new DateTime(2013, 1, 1);
                //}

                string[][] files = GetFileNames(path.Location, "*.*", TxtDateType.Vibration);

                if (files.Length > 0)
                {
                    foreach (string file in files[0])
                    {
                        if (file.EndsWith("odb"))
                        {
                            string parastr = file.Remove(file.Length - 3, 3);
                            parastr += "sdb";
                            if (files[1].Contains(parastr))
                            {
                                var fi = new FileInfo(file);
                                FileParamStruct para = ReadParamS(parastr);
                                double[] datas = ReadDatumS(file);
                                Data data = new Data();
                                string[] strs =fi.Name.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                                string strch = strs[1];
                                data.MoudleNo = strs[0];
                                data.ChannelId = int.Parse(strch);
                                data.OFlag = para.diSampleFreq;
                                data.DataBaseId = (int)path.ID;
                                data.SafeTypeId = (int)SensorCategory.Vibration;
                                data.ProjectCode = (short)ProjectInfoDic.GetInstance().GetProjectInfo().ProjectCode;
                                data.DataSet = datas.ToList();
                                DateTime acqtime;
                                string str = strs[2].Trim();
                                if (strs[2].Contains(".sdb") || strs[2].Contains(".odb"))
                                    str = strs[2].Substring(0, strs[2].Length - 4);
                                ValueHelper.String2Time(str, out acqtime);
                                data.CollectTime = acqtime;
                                data.Reserve = 0;
                                if (strs.Length == 4)
                                {
                                    // 触发采样
                                    data.Reserve = 1;
                                }
                                list.Add(data);
                                //ExtractionConfigDic.GetExtractionConfigDic()
                                //                        .UpdateExtractionConfig(
                                //                            new ExtractionConfig
                                //                            {
                                //                                DataBaseId = (int)path.ID,
                                //                                TableName = string.Empty,
                                //                                Acqtime =
                                //                                    fi.LastWriteTime.ToString("yyyyMMddHHmmss.fff")
                                //                            });
                                MoveFile(file);
                                MoveFile(parastr);
                            }
                        }
                        
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 读取配置文件信息
        /// </summary>
        /// <returns></returns>
        private static FileParamStruct ReadParamS(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var br = new BinaryReader(fs);
                var fps = (FileParamStruct)BytesToStruct(br.ReadBytes((int)fs.Length), typeof(FileParamStruct));
                return fps;
            }
        }

        public static Object BytesToStruct(Byte[] bytes, Type strcutType)
        {
            Int32 size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }


        /// <summary>
        /// 读取数据文件中的所有数据
        /// </summary>
        private static double[] ReadDatumS(string path)
        {
            try
            {
                double[] ret = null;
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    long n = fs.Length / sizeof(double);
                    ret = new double[n];
                    BinaryReader br = new BinaryReader(fs);
                    for (int i = 0; i < n; i++)
                    {
                        ret[i] = br.ReadDouble();
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        ///// <summary>
        ///// 根据文件名读取参数
        ///// </summary>
        ///// <param name="filename"></param>
        ///// <returns></returns>
        //public FileParam ReadParamFromFileName(string filename)
        //{
        //    string file = Path.Combine(FilePath, filename + @".sdb");
        //    return ReadParamS(file);
        //}

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="format"></param>
        /// <param name="time"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string[][] GetFileNames(string path,string format,TxtDateType type)
        {
            var paras = new List<string>();
            var res = new List<string>();

            //根据时间从小到大排序
            var tempList =new SortedList<DateTime, List<string>>();
            string[] files = Directory.GetFiles(path, format, SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                if (file.EndsWith("odb") || file.EndsWith("sdb"))
                {
                    if (!this._isExtractTimeSamplingData)
                    {
                        if (!file.Contains("_TRI"))
                        {
                            continue;
                        }
                    }
                    try
                    {
                        DateTime recordTime = GetFileRecordTime(file, TxtDateType.Vibration);
                        //if (DateTime.Compare(recordTime, time) > 0)
                        //{
                            File.Open(file,FileMode.Open,FileAccess.ReadWrite).Close();
                            if (!tempList.ContainsKey(recordTime))
                            {
                                tempList.Add(recordTime, new List<string>()); 
                            }
                            tempList[recordTime].Add(file);
                       // }
                    }
                    catch{
                    }
                }
            }

            foreach (DateTime key in tempList.Keys)
            {
                foreach (string file in tempList[key])
                {
                    if (file.EndsWith("odb"))
                    {
                        res.Add(file);
                    }
                    else if(file.EndsWith("sdb"))
                    {
                        paras.Add(file);
                    }
                }
            }

            return new[] {res.ToArray(),paras.ToArray()};
        }

        /// <summary>
        /// 获取文件的最后写入时间
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private DateTime GetFileRecordTime(string filename, TxtDateType type)
        {
            var fi = new FileInfo(filename);
            // DateTime readtime;
            // ValueHelper.String2Time(fi.LastWriteTime.ToString("yyyyMMddHHmmss.fff"), out readtime);
            return fi.LastWriteTime;
            //string[] timerstrs = filename.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            //if (timerstrs.Length >= 3)
            //{
            //    DateTime time;
            //    if (ValueHelper.String2Time(timerstrs[2], out time))
            //    {
            //        return time;
            //    }

            //    throw new Exception("时间格式错误！");
            //}

            //throw new Exception("时间格式错误！");
        }
        
        private void MoveFile(string file)
        {
            var fi = new FileInfo(file);
            string bakpath = fi.DirectoryName + @"\backup";
            DirectoryInfo dir = new DirectoryInfo(bakpath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            fi.MoveTo(bakpath + @"\" + fi.Name);
        }
         
    }
}