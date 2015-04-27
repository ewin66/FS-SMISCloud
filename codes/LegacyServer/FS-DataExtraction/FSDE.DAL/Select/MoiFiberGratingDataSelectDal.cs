#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="MoiFiberGratingDataSelectDal.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System.Linq;
using System.Windows.Forms;

namespace FSDE.DAL.Select
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using FreeSun.Common.Utils.FileCache;
    using FSDE.Dictionaries.config;
    using FSDE.IDAL;
    using FSDE.Model;
    using FSDE.Model.Config;

    public class MoiFiberGratingDataSelectDal : ITextOrBinarySelectDal
    {
        public IList<Data> TextOrBinarySelect(DataBaseName path)
        {
            IList<Data> list = new List<Data>();
            if (path.DataBaseType == (int)DataBaseType.Fiber)
            {
                DateTime readtime = new DateTime();
                string[] timeformats =
                    {
                        "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss",
                        "yyyy-MM-dd HH:mm:ss.fff", "yyyyMMddHHmmss", "yyyyMMddHHmmss.fff",
                        "yyyy-MM-dd h:mm:ss"
                    };

                var dir = new DirectoryInfo(path.Location);
                DirectoryInfo[] childDirectories;
                try
                {
                    childDirectories = dir.GetDirectories("FBG_*");
                }
                catch (Exception ex)
                {

                    return list;
                }
                

                var fbgList=new List<string>();
                if (childDirectories!=null && childDirectories.Length > 0)
                {
                    foreach (DirectoryInfo childDirectory in childDirectories)
                    {
                        fbgList.Add(childDirectory.FullName);
                    }
                }

                if (fbgList!=null && fbgList.Count > 0)
                {
                    foreach (string fbg in fbgList) //遍历所有光栅光纤文件夹
                    {
                        bool isSuccess = false;
                        try
                        {
                            if (ExtractionConfigDic.GetExtractionConfigDic()
                                .GetExtractionConfig((int) path.ID).Count > 0)
                            {
                                isSuccess =
                                DateTime.TryParseExact(
                                    ExtractionConfigDic.GetExtractionConfigDic()
                                        .GetExtractionConfig((int)path.ID)[0]
                                        .Acqtime,
                                    timeformats,
                                    CultureInfo.CurrentCulture,
                                    DateTimeStyles.None,
                                    out readtime);
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        if (!isSuccess)
                        {
                            readtime=new DateTime(2013,1,1);
                        }
                        TimeSpan ts = DateTime.Now.Date.Subtract(readtime.Date);
                        var strdays = new string[ts.Days + 1];
                        for (int i = 0; i < strdays.Length; i++)
                        {
                            strdays[i] = readtime.AddDays(i).ToString("yyyyMMdd");
                        }

                        string moudleId = Path.GetFileNameWithoutExtension(fbg).Remove(0, 4);

                        foreach (string strday in strdays) // 遍历一个模块下某一天的所有通道的数据
                        {
                            string filter = "CH_*_" + strday + "_*.txt";
                            string[] files = Directory.GetFiles(fbg, filter, SearchOption.TopDirectoryOnly);

                            foreach (string file in files)// 遍历某通道的所有数据
                            {
                                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                                int channel =
                                    int.Parse(
                                        fileNameWithoutExtension.Split(
                                            new[] { '_' },
                                            StringSplitOptions.RemoveEmptyEntries)[1].Trim());

                                DateTime lasTime;
                                
                                 isSuccess =
                                DateTime.TryParseExact(
                                    ExtractionConfigDic.GetExtractionConfigDic()
                                        .GetExtractionConfig((int)path.ID,channel.ToString())
                                        .Acqtime,
                                    timeformats,
                                    CultureInfo.CurrentCulture,
                                    DateTimeStyles.None,
                                    out lasTime);
                                try
                                {
                                    using (var tfc = new ZetaTemporaryFileCloner(file))
                                    {
                                        using (var sr = new StreamReader(tfc.FilePath))
                                        {
                                            string linestr = string.Empty;
                                            DateTime time = DateTime.Now;
                                            while ((linestr=sr.ReadLine())!=null)
                                            {
                                                string[] pars = linestr.Split(
                                                    new[] { '\t' },
                                                    StringSplitOptions.RemoveEmptyEntries);
                                                
                                                string[] tempTime1 = pars[0].Remove(0, 1).Split(new char[]{'-',' ','.',':'});
                                                string tempTime = null;
                                                for (int i = 0; i < tempTime1.Count()-1; i++)
                                                {
                                                    tempTime += tempTime1[i];
                                                }
                                                if (!DateTime.TryParseExact(tempTime, "yyyyMMddHHmmss",
                                                    CultureInfo.CurrentCulture, DateTimeStyles.None, out time))
                                                {
                                                    //MessageBox.Show("时间转换失败");
                                                }
                                                if (time > lasTime)
                                                {
                                                    Data data = new Data();
                                                    data.MoudleNo = moudleId;
                                                    data.ChannelId = channel;
                                                    data.CollectTime = time;
                                                    data.DataBaseId = (int)path.ID;
                                                    data.SafeTypeId = 11;
                                                    data.DataSet = new List<double>();
                                                    try
                                                    {
                                                        for (int i = 1; i < pars.Length; i++)
                                                        {
                                                            data.DataSet.Add(double.Parse(pars[i].Trim()));
                                                        }
                                                        list.Add(data);
                                                    }
                                                    catch
                                                    {
                                                        continue;
                                                    }
                                                }
                                                ExtractionConfigDic.GetExtractionConfigDic()
                                                    .UpdateExtractionConfig(
                                                        new ExtractionConfig
                                                            {
                                                                DataBaseId = (int)path.ID,
                                                                TableName = channel.ToString(),
                                                                Acqtime =
                                                                    time.ToString("yyyyMMddHHmmss.fff")
                                                            });
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw e;
                                }
                            }
                        }

                        Console.WriteLine("AAA");
                    }
                    ExtractionConfigDic.GetExtractionConfigDic()
                                                    .UpdateExtractionConfig(
                                                        new ExtractionConfig
                                                        {
                                                            DataBaseId = (int)path.ID,
                                                            TableName = string.Empty,
                                                            Acqtime =
                                                                DateTime.Now.ToString("yyyyMMddHHmmss.fff")
                                                        });
                }
            }
            return list;
        }
    }
}