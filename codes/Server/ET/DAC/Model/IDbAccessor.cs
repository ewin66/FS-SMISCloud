using FS.SMIS_Cloud.DAC.Task;
using System.Collections.Generic;

namespace FS.SMIS_Cloud.DAC.Model
{
    // Table <=> Model
    public interface IDbAccessor
    {
        IList<DtuNode> QueryDtuNodes(string dtuCode = null, NetworkType? networkType = null);

        DtuNode QueryDtuNode(string dtuCode);
        
        IList<DACTask> GetUnfinishedTasks();
        
        // return new task's id.
        int SaveInstantTask(DACTask task);

        int UpdateInstantTask(DACTaskResult result);

        IList<SensorGroup> QuerySensorGroups();
    }
}
