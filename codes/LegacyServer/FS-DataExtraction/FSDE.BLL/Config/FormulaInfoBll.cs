using System.Collections.Generic;
using FSDE.DALFactory;
using FSDE.IDAL;
using FSDE.Model.Fixed;

namespace FSDE.BLL.Config
{
    public class FormulaInfoBll
    {
        private readonly IFormulaInfo Dal = DataAccess.CreateFormulaInfoDal();

        public IList<FormulaInfo> SelectList()
        {
            return Dal.SelectList();
        }
    }
}