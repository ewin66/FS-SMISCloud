namespace FS.SMIS_Cloud.NGET.Model
{
    using System.Collections.Generic;

    public class Sensor
    {
        /// sensorId
        public uint SensorID { get; set; }

        public string Name { get; set; }

        /// 协议类型
        public uint ProtocolType { get; set; }

        // 计算公式ID
        public uint FormulaID { get; set; }

        // 归属主题（数据类型）
        public uint FactorType { get; set; }

        ///  计算参数
        public IList<SensorParam> Parameters { get; private set; }

        // 监测因素主题表
        public string FactorTypeTable { get; set; }

        public string TableColums { get; set; }

        public int ProductId { get; set; }

        //产品型号
        public string ProductCode { get; set; }

        //结构物Id
        public uint StructId { get; set; }
        
        //是否启用
        public bool Enabled { get; set; }

        public Sensor()
        {
            this.Parameters = new List<SensorParam>();
            this.SensorType = SensorType.Entity;
        }

        public SensorType SensorType { get; set; }

        public void AddParameter(SensorParam p)
        {
            this.Parameters.Add(p);
        }
    }
}
