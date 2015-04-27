
namespace FreeSun.FS_SMISCloud.Server.CloudApi.Entity.Config
{
    // 传感器解码参数.
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
