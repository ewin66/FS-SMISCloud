#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="PressureData.cs" company="江苏飞尚安全监测咨询有限公司">
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
    public class PressureData : BasicSensorData
    {
        public PressureData(double p,double phy)
        {
            this.Pressure = p;
            this.PhysicalQuantity = phy;
            this._themsValues =new List<double?>(new double?[] {phy});
            this.IsSaveDataOriginal = true;
        }
        public double Pressure { get; private set; }

        /// <summary>
        /// 根据公式计算后得出的物理量值
        /// </summary>
        public double PhysicalQuantity { get; private set; }

        public override double[] RawValues
        {
            get { return new [] { this.Pressure}; }
        }

        public override double[] PhyValues
        {
            get { return new [] { this.PhysicalQuantity }; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { this.Pressure }; }
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