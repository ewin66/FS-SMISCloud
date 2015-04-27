#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="VoltageData.cs" company="江苏飞尚安全监测咨询有限公司">
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
    public class VoltageData : BasicSensorData
    {
        public VoltageData(double v, double c)
        {
            this.Voltage = v;
            this.Crack = c;
            this._themsValues =new List<double?>(new double?[] { c });
            this.IsSaveDataOriginal = true;
        }

        public double Voltage { get;private set; }

        public double Crack { get;private set; }
        
        public override double[] RawValues
        {
            get { return new [] { this.Voltage }; }
        }

        public override double[] PhyValues
        {
            get { return new [] { this.Crack }; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { this.Voltage }; }
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