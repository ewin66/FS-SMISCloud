using System.Collections;
using System.Collections.Generic;
using FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Scripts
{
    public class Sort_82_40 : IComparer<SimpleSensor>
    {
        public int Compare(SimpleSensor x, SimpleSensor y)
        {
            if (x != null && y != null)
                return x.sensorid.CompareTo(y.sensorid) * -1;
            else
                return Comparer.Default.Compare(x, y);
        }
    }
}