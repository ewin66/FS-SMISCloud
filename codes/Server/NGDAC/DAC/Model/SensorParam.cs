
namespace FS.SMIS_Cloud.NGDAC.Model
{
    using System;

    // 传感器解码参数.
    [Serializable]
    public class SensorParam
    {
        public FormulaParam FormulaParam { get; set; }
        public double Value { get; set; }

        public SensorParam(FormulaParam temp)
        {
            this.FormulaParam = temp;
        }
    }
}
