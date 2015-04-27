// // --------------------------------------------------------------------------------------------
// // <copyright file="ViewFormulaInfoDic.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140606
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
using System.Linq;
using FSDE.BLL.Config;
using FSDE.Model.Fixed;

namespace FSDE.Dictionaries.config
{
    public class ViewFormulaInfoDic
    {
        private static ViewFormulaInfoDic viewFormulaInfoDic = new ViewFormulaInfoDic();

        private Dictionary<int, FormulaInfo> viewFormulaInfoDics;

        public static ViewFormulaInfoDic GetViewFormulaInfoDic()
        {
            return viewFormulaInfoDic;
        }

        public ViewFormulaInfoDic()
        {
            if (viewFormulaInfoDics == null)
            {
                viewFormulaInfoDics = new Dictionary<int, FormulaInfo>();
                var bll = new FormulaInfoBll();
                List<FormulaInfo> list = bll.SelectList().ToList();
                foreach (var info in list)
                {
                    viewFormulaInfoDics.Add(Convert.ToInt32(info.FormulaParaId),info);
                }
            }
        }

        public List<FormulaInfo> GetAllFuFormulaInfos()
        {
            return viewFormulaInfoDics.Values.ToList();
        }

        public List<FormulaInfo> GetFormulaInfo(int formulaID)
        {
            if (this.viewFormulaInfoDics != null && this.viewFormulaInfoDics.Count > 0)
            {
                if (this.viewFormulaInfoDics.ContainsKey(formulaID))
                {
                    return this.viewFormulaInfoDics.Values.Where(formulaInfo => formulaInfo.FormulaId == formulaID).ToList();
                }
            }

            return null;
        }

    }
}