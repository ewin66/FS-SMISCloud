// --------------------------------------------------------------------------------------------
// <copyright file="GPSData.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20141024
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

namespace FS.SMIS_Cloud.NGDAC.Model.Sensors
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Gps3dData : BasicSensorData
    {
        public Gps3dData(double orx, double ory, double orz,double cx,double cy,double cz)
        {
            this.CoordX = orx;
            this.CoordY = ory;
            this.CoordHeight = orz;
            this.ChangeX = cx;
            this.ChangeY = cy;
            this.ChangeHeight = cz;
            this._themsValues = new List<double?>(new double?[] { this.ChangeX, this.ChangeY, this.ChangeHeight });
            this.IsSaveDataOriginal = true;
        }

        public double CoordX { get;private set; }

        public double CoordY { get; private set; }

        public double CoordHeight { get; private set; }

        public double ChangeX { get; private set; }

        public double ChangeY { get; private set; }

        public double ChangeHeight { get; private set; }
        
        public override double[] RawValues
        {
            get
            {
                return new [] { this.CoordX, this.CoordY, this.CoordHeight };
            }
        }

        public override double[] PhyValues
        {
            get { return new[] {this.ChangeX, this.ChangeY, this.ChangeHeight}; }
        }

        public override double[] CollectPhyValues
        {
            get { return new[] { this.CoordX, this.CoordY, this.CoordHeight }; }
        }

        public override void DropThemeValue(int colphyindex)
        {
            if (colphyindex == 0 || colphyindex == 1 || colphyindex == 2)
            {
                this._themsValues[colphyindex] = null;
            }
        }
    }
}