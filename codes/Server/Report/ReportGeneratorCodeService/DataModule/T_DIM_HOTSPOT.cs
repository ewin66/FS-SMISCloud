//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ReportGeneratorService.DataModule
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_DIM_HOTSPOT
    {
        public int ID { get; set; }
        public int SENSOR_ID { get; set; }
        public Nullable<decimal> SPOT_X_AXIS { get; set; }
        public Nullable<decimal> SPOT_Y_AXIS { get; set; }
        public bool IS_REALTIME_SENSOR { get; set; }
        public Nullable<decimal> INFO_X_AXIS { get; set; }
        public Nullable<decimal> INFO_Y_AXIS { get; set; }
        public string SPOT_PATH { get; set; }
        public Nullable<int> SECTION_ID { get; set; }
    
        public virtual T_DIM_SECTION T_DIM_SECTION { get; set; }
    }
}