using System.Collections.Generic;
using System.Linq;
using FSDE.IDAL;
using FSDE.Model.Fixed;
using SqliteORM;

namespace FSDE.DAL.Config
{
    public class FormulaInfoDal : IFormulaInfo
    {

        public IList<FormulaInfo> SelectList()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<FormulaInfo> adapter = TableAdapter<FormulaInfo>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }
    }
}