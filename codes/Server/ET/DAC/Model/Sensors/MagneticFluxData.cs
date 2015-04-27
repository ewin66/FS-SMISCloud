#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="MagneticFluxData.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System;
using System.Collections.Generic;

namespace FS.SMIS_Cloud.DAC.Model.Sensors
{
    [Serializable]
    public class MagneticFluxData : BasicSensorData
    {
        public MagneticFluxData(double v,double t,double f)
        {
            this.Voltage = v;
            this.Temperature = t;
            this.Force = f;
            this._themsValues =new List<double?>(new double?[] { f });
            this.IsSaveDataOriginal = true;
        }

        public double Voltage { get;private set; }

        public double Temperature { get; private set; }

        public double Force { get; private set; }

        public override double[] RawValues
        {
            get { return new [] { Voltage, Temperature }; }
        }

        public override double[] PhyValues
        {
            get { return new [] { Force }; }
        }

        public override double[] CollectPhyValues
        {
            get {  return new [] { Force }; }
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