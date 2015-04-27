#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="WindData.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：lonwin lonwin ling20140914
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
    public class Wind2dData : BasicSensorData
    {
        public Wind2dData(double s,double d)
        {
            this.AirSpeed = s;
            this.WindDirection = d;
            this._themsValues =new List<double?>(new double?[] { s, d });
            this.IsSaveDataOriginal = true;
        }

        public double AirSpeed { get; private set; }

        public double WindDirection { get; private set; }
        
        public override double[] RawValues
        {
            get { return new[] { this.AirSpeed, this.WindDirection }; }
        }

        public override double[] PhyValues
        {
            get { return new[] { this.AirSpeed, this.WindDirection }; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { this.AirSpeed, this.WindDirection }; }
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