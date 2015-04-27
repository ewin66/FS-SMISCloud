#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="InclinationData.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System;
using System.Collections.Generic;

namespace FS.SMIS_Cloud.DAC.Model.Sensors
{
    [Serializable]
    public class InclinationData:BasicSensorData
    {
        public InclinationData(double ox,double oy, double cx,double cy)
        {
            this.Angle_X = ox;
            this.Angle_Y = oy;
            this.Change_X = cx;
            this.Change_Y = cy;
            _themsValues = new List<double?>(new double?[] { cx, cy });
            this.IsSaveDataOriginal = true;
        }

        public double Angle_X { get; private set; }

        public double Angle_Y { get; private set; }

        public double Change_X { get; private set; }

        public double Change_Y { get; private set; }

        public override double[] RawValues
        {
            get { return new [] { Angle_X, Angle_Y}; }
        }

        public override double[] PhyValues
        {
            get { return new[] {Change_X, Change_Y}; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { Angle_X, Angle_Y }; }
        }

        public override void DropThemeValue(int colphyindex)
        {
            if (colphyindex == 0 || colphyindex == 1)
            {
                this._themsValues[colphyindex] = null;
            }
        }
    }
}