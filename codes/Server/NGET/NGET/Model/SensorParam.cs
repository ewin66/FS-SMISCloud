
namespace FS.SMIS_Cloud.NGET.Model
{
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