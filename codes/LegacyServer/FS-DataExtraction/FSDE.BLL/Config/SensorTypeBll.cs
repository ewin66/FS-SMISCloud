using System.Collections.Generic;
using FSDE.DALFactory;
using FSDE.IDAL;
using FSDE.Model.Config;
using FSDE.Model.Fixed;

namespace FSDE.BLL.Config
{
    public class SensorTypeBll
    {
        private readonly ISensorType Dal = DataAccess.CreateSensorTypeDal();

        public IList<SensorType> SelectList()
        {
            return Dal.SelectList();
        }
    }
}