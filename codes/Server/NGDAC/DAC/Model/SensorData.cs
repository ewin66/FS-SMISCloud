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

namespace FS.SMIS_Cloud.NGDAC.Model
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SensorData:BasicSensorData
    {
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
            foreach (double d in phyvalues)
            {
                this._themsValues.Add(d);
            }
        }

        private readonly double[] _rawValues;
        private readonly double[] _phyValues;
        private readonly double[] _collectPhyValues;

        public override double[] RawValues
        {
            get { return this._rawValues; }
        }

        public override double[] PhyValues
        {
            get { return this._phyValues; }
        }

        public override double[] CollectPhyValues
        {
            get { return this._collectPhyValues; }
        }

        public override void DropThemeValue(int colphyindex)
        {
            if (colphyindex < this._themsValues.Count)
            {
                this.ThemeValues[colphyindex] = null;
            }
        }
    }
}