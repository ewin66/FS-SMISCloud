#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SFormulaidSet.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140529 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Model.Config
{
    using SqliteORM;

    [Table(Name = "S_FORMULAID_SET")]
    public class SFormulaidSet : TableBase<SFormulaidSet>
    {
        /// <summary>
        /// 参数配置ID
        /// </summary>
        [PrimaryKey(Name = "FORMULAID_SET_ID")]
        public long FormulaidSetId { get; set; }

        /// <summary>
        ///  对应的公式ID
        /// </summary>
        [Field(Name = "FORMULAID_ID")]
        public int FormulaidId { get; set; }

        /// <summary>
        /// 参数个数
        /// </summary>
        [Field(Name = "ParaCount")]
        public int ParaCount { get; set; }

        /// <summary>
        /// 参数名称ID1
        /// </summary>
        [Field(Name = "PARA_NAME_ID1")]
        public int ParaNameId1 { get; set; }

        /// <summary>
        /// 参数1
        /// </summary>
        [Field(Name = "PARAMETER1")]
        public double Parameter1 { get; set; }

        /// <summary>
        /// 参数名称ID2
        /// </summary>
        [Field(Name = "PARA_NAME_ID2")]
        public int ParaNameId2 { get; set; }

        /// <summary>
        /// 参数2
        /// </summary>
        [Field(Name = "PARAMETER2")]
        public double Parameter2 { get; set; }

        /// <summary>
        /// 参数名称ID3
        /// </summary>
        [Field(Name = "PARA_NAME_ID3")]
        public int ParaNameId3 { get; set; }

        /// <summary>
        /// 参数3
        /// </summary>
        [Field(Name = "PARAMETER3")]
        public double Parameter3 { get; set; }

        /// <summary>
        /// 参数名称ID4
        /// </summary>
        [Field(Name = "PARA_NAME_ID4")]
        public int ParaNameId4 { get; set; }

        /// <summary>
        /// 参数4
        /// </summary>
        [Field(Name = "PARAMETER4")]
        public double Parameter4 { get; set; }

        /// <summary>
        /// 参数名称ID5
        /// </summary>
        [Field(Name = "PARA_NAME_ID5")]
        public int ParaNameId5 { get; set; }

        /// <summary>
        /// 参数5
        /// </summary>
        [Field(Name = "PARAMETER5")]
        public double Parameter5 { get; set; }

        /// <summary>
        /// 参数名称ID6
        /// </summary>
        [Field(Name = "PARA_NAME_ID6")]
        public int ParaNameId6 { get; set; }

        /// <summary>
        /// 参数6
        /// </summary>
        [Field(Name = "PARAMETER6")]
        public double Parameter6 { get; set; }

        /// <summary>
        /// 参数名称ID7
        /// </summary>
        [Field(Name = "PARA_NAME_ID7")]
        public int ParaNameId7 { get; set; }

        /// <summary>
        /// 参数7
        /// </summary>
        [Field(Name = "PARAMETER7")]
        public double Parameter7 { get; set; }

        /// <summary>
        /// 参数名称ID8
        /// </summary>
        [Field(Name = "PARA_NAME_ID8")]
        public int ParaNameId8 { get; set; }

        /// <summary>
        /// 参数8
        /// </summary>
        [Field(Name = "PARAMETER8")]
        public double Parameter8 { get; set; }

        /// <summary>
        /// 参数名称ID9
        /// </summary>
        
        public int ParaNameId9 { get; set; }

        /// <summary>
        /// 参数9
        /// </summary>

        public double Parameter9 { get; set; }


        /// <summary>
        /// 参数名称ID10
        /// </summary>

        public int ParaNameId10 { get; set; }

        /// <summary>
        /// 参数10
        /// </summary>

        public double Parameter10 { get; set; }

    }
}