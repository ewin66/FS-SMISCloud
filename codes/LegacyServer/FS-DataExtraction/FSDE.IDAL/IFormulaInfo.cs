using System.Collections;
using System.Collections.Generic;
using FSDE.Model.Fixed;

namespace FSDE.IDAL
{
    public interface IFormulaInfo
    {
        IList<FormulaInfo> SelectList();
    }
}