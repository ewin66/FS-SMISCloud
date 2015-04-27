#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorData.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150306 by LINGWENLONG .
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

namespace FS.SMIS_Cloud.NGET.Model
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class SensorData
    {
        [JsonConstructor]
        public SensorData(double[] rawvalues, double[] phyvalues, double[] collPhyValues, double?[] themeValues)
        {
            this._rawValues = rawvalues;
            this._phyValues = phyvalues;
            this._collectPhyValues = collPhyValues;
            this.IsSaveDataOriginal = true;
            this._themsValues = themeValues.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawvalues">原始数据</param>
        /// <param name="phyvalues">计算后物理值(如果不过滤就是主题数据)</param>
        /// <param name="collPhyValues">判断数据量程的采集到的物理值</param>
        public SensorData(double[] rawvalues,double[] phyvalues,double[] collPhyValues)
        {
            this._rawValues = rawvalues;
            this._phyValues = phyvalues;
            this._collectPhyValues = collPhyValues;
            this.IsSaveDataOriginal = true;
            this._themsValues = new List<double?>();
            if (phyvalues != null)
            {
                foreach (double d in phyvalues)
                {
                    this._themsValues.Add(d);
                }
            }
        }

        private readonly double[] _rawValues;
        private readonly double[] _phyValues;
        private readonly double[] _collectPhyValues;
        protected IList<double?> _themsValues { get; set; }
        public bool IsSaveDataOriginal { get; set; }

        public virtual double[] RawValues
        {
            get { return this._rawValues; }
        }

        public virtual double[] PhyValues
        {
            get { return this._phyValues; }
        }

        public virtual double[] CollectPhyValues
        {
            get { return this._collectPhyValues; }
        }

        public virtual IList<double?> ThemeValues
        {
            get { return this._themsValues; }
        }

        public void DropThemeValue(int colphyindex)
        {
            if (colphyindex < this._themsValues.Count)
            {
                this.ThemeValues[colphyindex] = null;
            }
        }

        public string JsonResultData { get; set; }
    }
}