using System.Collections.Generic;
using FS.SMIS_Cloud.DAC.Model;

namespace FS.SMIS_Cloud.DAC.DataCalc.Model
{
    public class VirtualSensor : BasicSensorData
    {
        /// <summary>
        /// 数据库中物理量数据占位个数
        /// </summary>
        public int NullPhysicValsCount = 0;

        public VirtualSensor(double? themeVal)
        {
            _themsValues = new List<double?>(new double?[] { themeVal });
            this.IsSaveDataOriginal = false;
        }

        public VirtualSensor(double? themeVal1, double? themeVal2)
        {
            _themsValues = new List<double?>(new double?[] { themeVal1, themeVal2 });
        }

        public VirtualSensor(double? themeVal1, double? themeVal2, double? themeVal3)
        {
            _themsValues = new List<double?>(new double?[] { themeVal1, themeVal2, themeVal3 });
        }

        public override double[] RawValues
        {
            get { return null; }
        }

        public override double[] PhyValues
        {
            get { return new double[NullPhysicValsCount]; }
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