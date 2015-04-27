using System;
using System.Collections.Generic;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Entity.Config
{
    // MSSQL: [T_DIM_SENSOR], SQLite: [S_SENSOR_SET]
    [Serializable]
    public class Sensor
    {
        // 归属 Node
        public uint DtuID { get; set; }

        public string DtuCode { get; set; }

        /// sensorId
        public uint SensorID { get; set; }

        /// 模块号
        public uint ModuleNo { get; set; }

        ///  通道号
        public uint ChannelNo { get; set; }

        public string Name { get; set; }

        /// 协议类型
        public uint ProtocolType { get; set; }

        // 计算公式ID
        public uint FormulaID { get; set; }

        // 归属主题（数据类型）
        public uint FactorType { get; set; }

        ///  计算参数
        public IList<SensorParam> Parameters { get; private set; }

        //public ushort ParamCount { get; set; }

        // 监测因素主题表
        public string FactorTypeTable { get; set; }

        public string TableColums { get; set; }

        public int ProductId { get; set; }

        //产品型号
        public string ProductCode { get; set; }

        //结构物Id
        public uint StructId { get; set; }

        //采集粒度
        public uint AcqInterval { get; set; }

        public DateTime LastTime { get; set; }
        //3-3
        public bool UnEnable { get; set; }

        public Sensor()
        {
            Parameters = new List<SensorParam>();
            SensorType = SensorType.EntityType;
        }

        public SensorType SensorType { get; set; }

        public void AddParameter(SensorParam p)
        {
            Parameters.Add(p);
        }
    }
}
