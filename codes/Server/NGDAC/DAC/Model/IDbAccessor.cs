namespace FS.SMIS_Cloud.NGDAC.Model
{
    using System.Collections.Generic;

    using FS.SMIS_Cloud.NGDAC.Task;

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
