// --------------------------------------------------------------------------------------------
// <copyright file="GPSData.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20141024
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace FS.SMIS_Cloud.DAC.Model.Sensors
{
    [Serializable]
    public class GpsHeightData : BasicSensorData
    {
        public GpsHeightData(double oh,double ch)
        {
            this.CoordHeight = oh;
            this.ChangeHeight = ch;
            this._themsValues = new List<double?>(new double?[] {ch});
            this.IsSaveDataOriginal = true;
        }

        public double CoordHeight { get; private set; }

        public double ChangeHeight { get; private set; }

        public override double[] RawValues
        {
            get
            {
                return new [] { CoordHeight };
            }
        }

        public override double[] PhyValues
        {
            get { return new[] { ChangeHeight }; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { CoordHeight }; }
        }

        public override void DropThemeValue(int colphyindex)
        {
            if (colphyindex == 0)
            {
                this._themsValues[0] = null;
            }
        }
    }
}