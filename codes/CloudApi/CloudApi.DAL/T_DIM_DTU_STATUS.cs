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
    
    public partial class T_DIM_DTU_STATUS
    {
        public int Id { get; set; }
        public int DtuId { get; set; }
        public bool Status { get; set; }
        public System.DateTime Time { get; set; }
    
        public virtual T_DIM_REMOTE_DTU T_DIM_REMOTE_DTU { get; set; }
    }
}