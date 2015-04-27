using System;
using System.Collections.Generic;

namespace FS.SMIS_Cloud.DAC.Model
{
    [Serializable]
    public abstract class BasicSensorData:ISensorData
    {
        public abstract double[] RawValues { get; }

        protected IList<double?> _themsValues { get; set; }

        public abstract double[] PhyValues { get; }

        public virtual IList<double?> ThemeValues
        {
            get { return _themsValues; }
        }
        
        public string JsonResultData { get; set; }

        public abstract double[] CollectPhyValues { get; }

        public abstract void DropThemeValue(int colphyindex);

        public bool IsSaveDataOriginal { get; set; }
    }
}
