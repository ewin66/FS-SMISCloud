using System.Collections.Generic;
using System;

namespace Agg.Comm.DataModle
{
    using System;
    using System.Collections.Generic;

    public class AggRawData
    {
        public string TimeTag{ get; set; }

        public AggTaskKey key;

        public List<RawData> Datas { get; set; }

        public List<AggData> LastAggDatas { get; set; }

        private int configId;

        /// <summary>
        /// 待聚集原始数据
        /// </summary>
        /// <param name="structId">结构物Id</param>
        /// <param name="factorId">监测因素Id</param>
        /// <param name="type">聚集时间类型</param>
        /// <param name="timeTag">时间标签</param>
        public AggRawData(AggTaskKey key, int configId, string timeTag)
        {
            this.Datas = new List<RawData>();
            this.key = key;
            this.TimeTag = timeTag;
            this.configId = configId;
        }

        public int ConfigId {
            get
            {
                return this.configId;
            }
        }

        public int FactorId 
        {
            get
            {
                return this.key.FactorId;
            }
        }

        public int StructId 
        {
            get
            {
                return this.key.StructId;
            }
        }

        public AggType Type
        {
            get
            {
                return this.key.Type;
            }
        }
    }
}
