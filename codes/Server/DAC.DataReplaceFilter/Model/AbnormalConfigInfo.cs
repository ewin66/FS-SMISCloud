using System.Collections.Generic;

namespace FS.SMIS_Cloud.DAC.DataReplaceFilter.Model
{
    public class AbnormalConfigInfo
    {
        public AbnormalConfigInfo()
        {
            ReplaceValues = new List<decimal?>();
        }

        public uint SensorId { get; set; }
        public List<decimal?> ReplaceValues { get; set; }
        /// <summary>
        /// 可用-1 不可用-0
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
