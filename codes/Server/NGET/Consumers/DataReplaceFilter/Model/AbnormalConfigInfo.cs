namespace FS.SMIS_Cloud.NGET.DataReplaceFilter.Model
{
    using System.Collections.Generic;

    public class AbnormalConfigInfo
    {
        public AbnormalConfigInfo()
        {
            this.ReplaceValues = new List<decimal?>();
        }

        public uint SensorId { get; set; }
        public List<decimal?> ReplaceValues { get; set; }
        /// <summary>
        /// 可用-1 不可用-0
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
