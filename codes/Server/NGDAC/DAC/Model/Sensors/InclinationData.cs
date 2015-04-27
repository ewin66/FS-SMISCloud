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

namespace FS.SMIS_Cloud.NGDAC.Model.Sensors
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class InclinationData:BasicSensorData
    {
        public InclinationData(double ox,double oy, double cx,double cy)
        {
            this.Angle_X = ox;
            this.Angle_Y = oy;
            this.Change_X = cx;
            this.Change_Y = cy;
            this._themsValues = new List<double?>(new double?[] { cx, cy });
            this.IsSaveDataOriginal = true;
        }

        public double Angle_X { get; private set; }

        public double Angle_Y { get; private set; }

        public double Change_X { get; private set; }

        public double Change_Y { get; private set; }

        public override double[] RawValues
        {
            get { return new [] { this.Angle_X, this.Angle_Y}; }
        }

        public override double[] PhyValues
        {
            get { return new[] {this.Change_X, this.Change_Y}; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { this.Angle_X, this.Angle_Y }; }
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