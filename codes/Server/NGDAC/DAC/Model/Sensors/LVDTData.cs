#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="LVDTData.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140912 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGDAC.Model.Sensors
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class LVDTData : BasicSensorData
    {
        public LVDTData(double o,double c)
        {
            this.ElongationIndicator = o;
            this.ChangeElongation = c;
            this._themsValues =new List<double?>(new double?[] { c });
            this.IsSaveDataOriginal = true;
        }

        /// <summary>
        /// 伸长量.
        /// </summary>
        public double ElongationIndicator { get; private set; }

        /// <summary>
        /// 变化量.
        /// </summary>
        public double ChangeElongation { get; private set; }

        public override double[] RawValues
        {
            get { return new [] { this.ElongationIndicator}; }
        }

        public override double[] PhyValues
        {
            get { return new [] { this.ChangeElongation }; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { this.ElongationIndicator }; }
        }

        public override void DropThemeValue(int colphyindex)
        {
            if (colphyindex == 0)
            {
                this.ThemeValues[colphyindex] = null;
            }
        }

       
    }
}