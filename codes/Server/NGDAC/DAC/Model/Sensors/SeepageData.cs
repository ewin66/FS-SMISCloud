#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="SeepageData.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141201 by LINGWENLONG .
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
    using System.Collections.Generic;

    public class SeepageData : BasicSensorData
    {
        public SeepageData(double value)
        {
            this.Seepagevalue = value;
            this._themsValues =new List<double?>(new double?[] { this.Seepagevalue });
            this.IsSaveDataOriginal = true;
        }

        public double Seepagevalue { get; private set; }

        public override double[] RawValues
        {
            get { return null; }
        }

        public override double[] PhyValues
        {
            get { return null; }
        }

        public override double[] CollectPhyValues
        {
            get { return null; }
        }

        public override void DropThemeValue(int colphyindex)
        {
        }
    }
}