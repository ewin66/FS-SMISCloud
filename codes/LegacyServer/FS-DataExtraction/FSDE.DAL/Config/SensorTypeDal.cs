using System.Collections.Generic;
using System.Linq;
using FSDE.IDAL;
using FSDE.Model.Config;
using FSDE.Model.Fixed;
using SqliteORM;

namespace FSDE.DAL.Config
{
    public class SensorTypeDal : ISensorType
    {

        public IList<SensorType> SelectList()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<SensorType> adapter = TableAdapter<SensorType>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }
    }
}