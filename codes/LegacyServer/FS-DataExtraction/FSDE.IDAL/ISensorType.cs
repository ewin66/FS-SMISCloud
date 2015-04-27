using System.Collections;
using System.Collections.Generic;
using FSDE.Model.Fixed;

namespace FSDE.IDAL
{
    public interface ISensorType
    {
        IList<SensorType> SelectList();
    }
}