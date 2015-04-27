using FS.SMIS_Cloud.DAC.Task;
using System.Collections.Generic;

namespace FS.SMIS_Cloud.DAC.Model
{
    using System;

    using FS.SMIS_Cloud.DAC.Config;

    // Table <=> Model
    public interface IConfigAccessor
    {
        IList<DtuNode> QueryDtuNodes(string dtuCode = null, NetworkType? networkType = null);
        DataMetaInfo[] GetDataMetas();

        DtuNode QueryDtuNode(string dtuCode);
        
        IList<DACTask> GetUnfinishedTasks();

        //int SaveDacResult(DACTaskResult result);

        // return new task's id.
        int SaveInstantTask(DACTask task);
        int UpdateInstantTask(DACTaskResult result);

        DateTime? GetSensorLastDataAcqTime(uint sensorId);
    }
}
