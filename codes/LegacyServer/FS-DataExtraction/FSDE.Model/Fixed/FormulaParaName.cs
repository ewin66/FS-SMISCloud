// // --------------------------------------------------------------------------------------------
// // <copyright file="FormulaParaName.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140609
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace FSDE.Model.Fixed
{
    using SqliteORM;

    [Table(Name = "C_FORMULA_PARA_NAME")]
    public class FormulaParaName : TableBase<FormulaParaName>
    {
        [PrimaryKey(Name = "PARA_NAME_ID")]
        public long ParaNameId { get; set; }

        [Field(Name = "PARA_ALIAS")]
        public string ParaAlias { get; set; }

        [Field(Name = "PARA_NAME")]
        public string ParaName { get; set; }

        [Field(Name = "IsShow")]
        public string IsShow{ get; set; }

    }
}