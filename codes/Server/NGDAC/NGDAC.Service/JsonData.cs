// --------------------------------------------------------------------------------------------
// <copyright file="JsonData.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2015 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20150331
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.NGDAC
{
    public class JsonData
    {
        public uint S { get; set; }

        public int R { get; set; }

        public int N { get; set; }

        public double T { get; set; }

        public string[] Q { get; set; }

        public string[] A { get; set; }

        public int FI { get; set; }

        public double[] RV { get; set; }

        public double[] LV { get; set; }

        public double[] PV { get; set; }

        public double?[] TV { get; set; }
    }
}