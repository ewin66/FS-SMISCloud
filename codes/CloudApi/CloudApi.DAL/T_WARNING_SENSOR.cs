//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace FreeSun.FS_SMISCloud.Server.CloudApi.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_WARNING_SENSOR
    {
        public T_WARNING_SENSOR()
        {
            this.T_WARNING_DEALDETAILS = new HashSet<T_WARNING_DEALDETAILS>();
        }
    
        public int Id { get; set; }
        public string WarningTypeId { get; set; }
        public int StructId { get; set; }
        public int DeviceTypeId { get; set; }
        public int DeviceId { get; set; }
        public string Content { get; set; }
        public System.DateTime Time { get; set; }
        public int DealFlag { get; set; }
        public int WarningStatus { get; set; }
    
        public virtual T_DIM_STRUCTURE T_DIM_STRUCTURE { get; set; }
        public virtual T_DIM_WARNING_DEVICETYPE T_DIM_WARNING_DEVICETYPE { get; set; }
        public virtual T_DIM_WARNING_TYPE T_DIM_WARNING_TYPE { get; set; }
        public virtual ICollection<T_WARNING_DEALDETAILS> T_WARNING_DEALDETAILS { get; set; }
    }
}