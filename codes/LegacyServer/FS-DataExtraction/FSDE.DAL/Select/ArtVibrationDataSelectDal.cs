#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="TextOrBinarySelectDal.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140623 by WIN .
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
using System.IO;
using System.Linq;

namespace FSDE.DAL.Select
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using FSDE.Commn;
    using FSDE.Dictionaries.config;
    using FSDE.IDAL;
    using FSDE.Model;
    using FSDE.Model.Config;

    public class ArtVibrationDataSelectDal : ITextOrBinarySelectDal
    {
        public IList<Data> TextOrBinarySelect(DataBaseName path)
        {
            var file = new ExeConfigurationFileMap { ExeConfigFilename = @".\config\Params.config" };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            var movefileIfExtactSuccessful = config.AppSettings.Settings["MoveFileIfExtractSuccessful"].Value;
            var movefileafterextract = true;
            if (!Boolean.TryParse(movefileIfExtactSuccessful, out movefileafterextract))
            {
                movefileafterextract = true;
            }

            if (movefileafterextract)
            {
                return SelectAndMove(path);
            }
            else
            {
                return SelectByTime(path);
            }
        }

        private IList<Data> SelectAndMove(DataBaseName path)
        {

            if (path.DataBaseType == (int)DataBaseType.Shake)
            {
                ArtAccDatum artdata = null;
                string errorstr = string.Empty;
                var list = new List<Data>();

                string[] files = BinaryFileHelper.GetFileNames(path.Location, "*.usb", TxtDateType.Vibration);
                //string[] files = Directory.GetFiles(path.Location,"*.usb");

                foreach (string file in files)
                {
                    DateTime lasTime;

                    DateTime acqtime = BinaryFileHelper.GetFileRecordTime(file, TxtDateType.Vibration);
                    //if (acqtime > lasTime)
                    {
                        if (BinaryFileHelper.Read(file, out artdata, out errorstr))
                        {
                            var pts = Path.GetDirectoryName(file).Split(Path.DirectorySeparatorChar);
                            int mod = -1;
                            int.TryParse(pts.Last(), out mod);
                            //DateTime acqtime = BinaryFileHelper.GetFileRecordTime(file, TxtDateType.Vibration);
                            for (int i = 0; i < artdata.MilliVolt.Length; i++)
                            {
                                Data data = new Data();
                                data.ChannelId = i + 1;
                                //data.MoudleNo = artdata.FileHeader.DeviceNum.ToString().Trim();
                                data.MoudleNo = mod.ToString();
                                data.OFlag = artdata.FileHeader.ADPara.Frequency;
                                data.DataBaseId = (int)path.ID;
                                data.SafeTypeId = (int)SensorCategory.Vibration;
                                data.ProjectCode = (short)ProjectInfoDic.GetInstance().GetProjectInfo().ProjectCode;
                                data.DataSet = new List<double>();

                                foreach (float value in artdata.MilliVolt[i])
                                {
                                    data.DataSet.Add(value);
                                }
                                data.CollectTime = acqtime;
                                list.Add(data);
                            }

                            MoveFile(file);
                        }
                    }

                }
                return list;
            }

            return new List<Data>();
        }

        public IList<Data> SelectByTime(DataBaseName path)
        {
            if (path.DataBaseType == (int)DataBaseType.Shake)
            {
                ArtAccDatum artdata = null;
                string errorstr = string.Empty;
                var list = new List<Data>();
                DateTime readtime;
                DateTime extratime = new DateTime(2013, 1, 1);
                string[] timeformats =
                    {
                        "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss",
                        "yyyy-MM-dd HH:mm:ss.fff", "yyyyMMddHHmmss", "yyyyMMddHHmmss.fff",
                        "yyyy-MM-dd h:mm:ss"
                    };//TIM_ 2014 05 03 16 17 07
                bool isSuccess = DateTime.TryParseExact(
                    ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)path.ID, 0.ToString(CultureInfo.InvariantCulture)).Acqtime,
                    timeformats,
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.None,
                    out readtime);
                if (!isSuccess)
                {
                    readtime = new DateTime(2013, 1, 1);
                }
                string[] files = BinaryFileHelper.GetFileNames(path.Location, "*.usb", readtime, TxtDateType.Vibration);

                foreach (string file in files)
                {
                    if (BinaryFileHelper.Read(file, out artdata, out errorstr))
                    {
                        var acqtime = BinaryFileHelper.GetFileRecordTime(file, TxtDateType.Vibration);
                        if (acqtime > extratime) extratime = acqtime;
                        var directoryName = Path.GetDirectoryName(file);
                        if (directoryName != null)
                        {
                            var pts = directoryName.Split(Path.DirectorySeparatorChar);
                            int mod = -1;
                            int.TryParse(pts.Last(), out mod);
                            for (var i = 0; i < artdata.MilliVolt.Length; i++)
                            {
                                var data = new Data
                                {
                                    ChannelId = i + 1,
                                    MoudleNo = mod.ToString(CultureInfo.InvariantCulture),
                                    OFlag = artdata.FileHeader.ADPara.Frequency,
                                    DataBaseId = (int) path.ID,
                                    SafeTypeId = (int) SensorCategory.Vibration,
                                    ProjectCode = (short) ProjectInfoDic.GetInstance().GetProjectInfo().ProjectCode,
                                    DataSet = new List<double>()
                                };

                                foreach (var value in artdata.MilliVolt[i])
                                {
                                    data.DataSet.Add(value);
                                }
                                data.CollectTime = acqtime;
                                list.Add(data);
                            }
                        }
                    }

                }// foreach
                if (extratime > new DateTime(2013, 1, 1))
                {
                    ExtractionConfigDic.GetExtractionConfigDic()
                                                .UpdateExtractionConfig(
                                                    new ExtractionConfig
                                                    {
                                                        DataBaseId = (int)path.ID,
                                                        TableName = 0.ToString(CultureInfo.InvariantCulture),
                                                        Acqtime = extratime.ToString("yyyyMMddHHmmss.fff")
                                                    });
                }
                return list;
            }

            return new List<Data>();
        }

        private void MoveFile(string file)
        {
            var fi = new FileInfo(file);
            string bakpath = fi.DirectoryName + @"\backup";
            var dir = new DirectoryInfo(bakpath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            var des = bakpath + @"\" + fi.Name;
            if (File.Exists(des))
                File.Delete(des);
            fi.MoveTo(des);
        }
    }
}