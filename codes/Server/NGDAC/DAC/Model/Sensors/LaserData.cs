#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="LaserData.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141111 by LINGWENLONG .
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
    public class LaserData : BasicSensorData
    {
        public LaserData(double coll,double chl)
        {
            this.CollectLen = coll;
            this.ChangedLen = chl;
            this._themsValues =new List<double?>(new double?[] { chl });
            this.IsSaveDataOriginal = true;
        }

        public double CollectLen { get; private set; }

        public double ChangedLen { get; private set; }

        public override double[] RawValues
        {
            get { return new[] {this.CollectLen}; }
        }

        public override double[] PhyValues
        {
            get { return new[] { this.ChangedLen }; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { this.CollectLen }; }
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