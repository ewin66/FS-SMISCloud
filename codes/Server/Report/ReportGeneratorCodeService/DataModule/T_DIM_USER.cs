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
    
    public partial class T_DIM_USER
    {
        public T_DIM_USER()
        {
            this.T_DIM_PERMISSION = new HashSet<T_DIM_PERMISSION>();
            this.T_WARNING_DEALDETAILS = new HashSet<T_WARNING_DEALDETAILS>();
            this.T_WARNING_SMS_RECIEVER = new HashSet<T_WARNING_SMS_RECIEVER>();
            this.T_DIM_USER_ORG = new HashSet<T_DIM_USER_ORG>();
        }
    
        public int USER_NO { get; set; }
        public string USER_PWD { get; set; }
        public string USER_NAME { get; set; }
        public string USER_EMAIL { get; set; }
        public Nullable<int> ROLE_ID { get; set; }
        public bool USER_IS_TRIL { get; set; }
        public bool USER_IS_ENABLED { get; set; }
        public string SYSTEM_NAME { get; set; }
        public string USER_PHONE { get; set; }
    
        public virtual ICollection<T_DIM_PERMISSION> T_DIM_PERMISSION { get; set; }
        public virtual ICollection<T_WARNING_DEALDETAILS> T_WARNING_DEALDETAILS { get; set; }
        public virtual ICollection<T_WARNING_SMS_RECIEVER> T_WARNING_SMS_RECIEVER { get; set; }
        public virtual ICollection<T_DIM_USER_ORG> T_DIM_USER_ORG { get; set; }
    }
}