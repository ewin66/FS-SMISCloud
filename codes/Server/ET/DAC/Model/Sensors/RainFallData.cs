#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="RainFallData.cs" company="江苏飞尚安全监测咨询有限公司">
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
    public class RainFallData : BasicSensorData
    {
        public RainFallData(double rf)
        {
            this.Rainfall = rf;
            this._themsValues =new List<double?>(new double?[] { rf });
            this.IsSaveDataOriginal = true;
        }
        public double Rainfall { get; private set; }

        public override double[] RawValues
        {
            get { return new [] { Rainfall }; }
        }

        public override double[] PhyValues
        {
            get { return new [] { Rainfall }; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { Rainfall }; }
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