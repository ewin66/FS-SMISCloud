// // --------------------------------------------------------------------------------------------
// // <copyright file="FormulaParaNameDic.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System;
using System.Collections.Generic;
using FSDE.BLL;
using FSDE.Model.Fixed;

namespace FSDE.Dictionaries
{
    public class FormulaParaNameDic
    {
        public Dictionary<int, FormulaParaName> formulaParaNames;

        public static FormulaParaNameDic formulaParaNameDic = new FormulaParaNameDic();

        public static FormulaParaNameDic GetFormulaParaNameDic()
        {
            return formulaParaNameDic;
        }

        private FormulaParaNameDic()
        {
            if (null == formulaParaNames)
            {
                formulaParaNames = new Dictionary<int, FormulaParaName>();
                var bll = new FormulaParaNameBll();
                IList<FormulaParaName> list = bll.SelectList();
                foreach (var formulaParaName in list)
                {
                    formulaParaNames.Add(Convert.ToInt32(formulaParaName.ParaNameId),formulaParaName);
                }
            }
        }

    }
}