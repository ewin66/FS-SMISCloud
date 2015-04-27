using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Push
{
    public class WarningPushInfo
    {
        public int WarningId { get; set; }

        public int Level { get; set; }

        public DateTime WarningTime { get; set; }

        public int StructId { get; set; }

        public string StructName { get; set; }
    }
}