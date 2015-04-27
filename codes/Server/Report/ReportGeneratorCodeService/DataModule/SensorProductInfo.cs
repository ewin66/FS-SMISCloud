using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportGeneratorService.DataModel
{
    public class SensorProductInfo : MarshalByRefObject
    {
        /// <summary>
        /// 传感器设备信息
        /// </summary>
        public int ProductId { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }

    }
}
