#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="FormulaidSetDic.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System;
using System.Linq;
using System.Runtime.InteropServices;
using FSDE.BLL;

namespace FSDE.Dictionaries.config
{
    using System.Collections.Generic;

    using FSDE.Model.Config;

    public class FormulaidSetDic
    {
        private static FormulaidSetDic formulaidSetDic = new FormulaidSetDic();

        private Dictionary<int, SFormulaidSet> formulaidSets;

        private FormulaidSetDic()
        {
            if (formulaidSets == null)
            {
                formulaidSets = new Dictionary<int, SFormulaidSet>();
                var bll = new SFormulaidSetBll();
                IList<SFormulaidSet> list = bll.SelectList();
                foreach (var set in list)
                {
                    formulaidSets.Add(Convert.ToInt32(set.FormulaidSetId),set);
                }
            } 
        }

        public int CheckAdd(SFormulaidSet sformulaidSet)
        {
            bool flag = false;
            foreach (var set in formulaidSets)
            {
                if (set.Value.FormulaidId == sformulaidSet.FormulaidId
                    && set.Value.ParaCount == sformulaidSet.ParaCount
                    && set.Value.Parameter1 == sformulaidSet.Parameter1
                    && set.Value.Parameter2 == sformulaidSet.Parameter2
                    && set.Value.Parameter3 == sformulaidSet.Parameter3
                    && set.Value.Parameter4 == sformulaidSet.Parameter4
                    && set.Value.Parameter5 == sformulaidSet.Parameter5
                    && set.Value.Parameter6 == sformulaidSet.Parameter6
                    && set.Value.Parameter7 == sformulaidSet.Parameter7
                    && set.Value.Parameter8 == sformulaidSet.Parameter8
                    && set.Value.ParaNameId1 == sformulaidSet.ParaNameId1
                    && set.Value.ParaNameId2 == sformulaidSet.ParaNameId2
                    && set.Value.ParaNameId3 == sformulaidSet.ParaNameId3
                    && set.Value.ParaNameId4 == sformulaidSet.ParaNameId4
                    && set.Value.ParaNameId5 == sformulaidSet.ParaNameId5
                    && set.Value.ParaNameId6 == sformulaidSet.ParaNameId6
                    )
                {
                    flag = true;
                }
            }

            if (!flag)
            {
                var bll = new SFormulaidSetBll();
                int id = bll.Add(sformulaidSet);
                if (id > 0)
                {
                    sformulaidSet.FormulaidSetId = id;
                    this.formulaidSets.Add(id, sformulaidSet);
                    return id;
                }
            }
            return -1;
        }

        public int Add(SFormulaidSet sformulaidSet)
        {
            var bll = new SFormulaidSetBll();
            int id = bll.Add(sformulaidSet);
            if (id > 0)
            {
                sformulaidSet.FormulaidSetId = id;
                this.formulaidSets.Add(id, sformulaidSet);
                return id;
            }
            return -1;
        }

        public bool Delete(int id)
        {
            var bll = new SFormulaidSetBll();
            formulaidSetDic.formulaidSets.Remove(id);
            return bll.Delete(id);
        }

        public bool Update(SFormulaidSet sFormulaidSet)
        {
            var bll = new SFormulaidSetBll();
            if (bll.Update(sFormulaidSet))
            {
                formulaidSetDic.formulaidSets[(int)sFormulaidSet.FormulaidSetId] = sFormulaidSet;
                return true;
            }
            return false;
        }

        public static FormulaidSetDic GetFormulaidSetDic()
        {
            return formulaidSetDic;
        }

        public SFormulaidSet GetFormulaidSet(int formulaidSetId)
        {
            return formulaidSets[formulaidSetId];
        }

        public List<SFormulaidSet> GetDicList()
        {
            return formulaidSets.Values.ToList();
        }







    }
}