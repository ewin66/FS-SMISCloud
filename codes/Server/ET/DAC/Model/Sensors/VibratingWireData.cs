#region File Header
// --------------------------------------------------------------------------------------------
//  <copyright file="VibratingWireData.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：lonwin lonwin ling20140913
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
    public class VibratingWireData : BasicSensorData
    {
        public VibratingWireData(double f, double t, double phy, double colphy)
        {
            this.Frequency = f;
            this.Temperature = t;
            this.PhysicalQuantity = phy;
            this._themsValues =new List<double?>(new double?[] { phy });
            this.ColPhy = colphy;
            this.IsSaveDataOriginal = true;
        }
        /// <summary>
        /// 频率
        /// </summary>
        public double Frequency { get;private set; }

        /// <summary>
        /// 温度
        /// </summary>
        public double Temperature { get;private set; }

        public double ColPhy { get; private set; }

        /// <summary>
        /// 根据公式计算后得出的物理量值
        /// </summary>
        public double PhysicalQuantity { get;private set; }

        public override double[] RawValues
        {
            get { return new [] { Frequency, Temperature}; }
        }

        public override double[] PhyValues
        {
            get
            {
                if (Math.Abs(ColPhy - PhysicalQuantity) > 0.00000001)
                    return new[] {ColPhy, PhysicalQuantity};
                return new[] { PhysicalQuantity };
            }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { ColPhy }; }
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