using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agg.Comm.DataModle
{
    public class Sensor
    {
        /// sensorId
        public uint SensorID { get; set; }

        // 归属主题（数据类型）
        public uint FactorType { get; set; }

        // 监测因素主题表
        public string FactorTypeTable { get; set; }

        //结构物Id
        public uint StructId { get; set; }

    }
}
