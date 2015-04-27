using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers
{
   public class SensorCond
    {
        public double cond;

        public double Cond
        {
            get { return cond; }
            set { cond = value; }
        }

        public double[] xyzt;
        public string structId;

        public DateTime collectTime { get; set; }
        public string batchId { get; set; }

        public List<ShockData> items;
    }
}
