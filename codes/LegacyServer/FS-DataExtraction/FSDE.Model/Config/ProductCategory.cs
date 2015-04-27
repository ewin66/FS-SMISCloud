// // --------------------------------------------------------------------------------------------
// // <copyright file="ProductCategory.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140604
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace FSDE.Model.Config
{
    using SqliteORM;

    [Table(Name = "C_PRODUCT_CATEGORY")]
    public class ProductCategory : TableBase<ProductCategory>
    {
        [PrimaryKey(Name = "CATEGORY_ID")]
        public long CatagoryId { get; set; }

        [Field(Name = "CATEGORY_NAME")]
        public string CatagoryName { get; set; }
    }
}