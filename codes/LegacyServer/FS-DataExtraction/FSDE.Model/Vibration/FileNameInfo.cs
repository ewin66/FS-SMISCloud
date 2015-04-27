#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="FileNameInfo.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace DAAS
{
    using System;
    [Serializable]
    public class FileNameInfo
    {
        /// <summary>
        /// 试验名
        /// </summary>
        public string Experiment;
        /// <summary>
        /// 测点号
        /// </summary>
        public string StationName;
        /// <summary>
        /// 测试时间
        /// </summary>
        public DateTime Time;

        public FileNameInfo() { }

        public FileNameInfo(string filename)
        {
            try
            {
                string f = filename;
                if (filename.Contains(".sdb") || filename.Contains(".odb"))
                {
                    f = System.IO.Path.GetFileNameWithoutExtension(filename);
                }
                string[] parts = f.Split('_');
                Experiment = parts[0];
                StationName = parts[1];
                Time = DateTime.ParseExact(parts[2], "yyyyMMddHHmmssfff", null);
            }
            catch (System.Exception ex)
            {
                Experiment = string.Empty;
                StationName = string.Empty;
                Time = DateTime.MinValue;
            }
        }
    }
}