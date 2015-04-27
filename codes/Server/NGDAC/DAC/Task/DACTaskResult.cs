namespace FS.SMIS_Cloud.NGDAC.Task
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using FS.SMIS_Cloud.NGDAC.DAC;
    using FS.SMIS_Cloud.NGDAC.Model;
    using FS.SMIS_Cloud.NGDAC.Node;

    /// <summary>
    /// DTU一轮采集的结果
    /// </summary>
    [Serializable]
    public class DACTaskResult
    {
        public DACTask Task { get; set; }
        //DTU
        public string DtuCode { get; set; }
        // 错误码
        public int ErrorCode { get; set; }
        // 错误信息
        public string ErrorMsg { get; set; }
        public long Elapsed { get; set; } // 耗时

        // 采集任务结束时间
        public DateTime Finished { get; set; }

        // 采集任务开始时间
        public DateTime Started { get; set; }

        /// <summary>
        /// SensorID - byte[] 
        /// </summary>
        public IList<SensorAcqResult> SensorResults { get; private set; }
        public bool IsOK { get { return this.ErrorCode == (int)Errors.SUCCESS; } }

        public DACTaskResult()
        {
            this.SensorResults = new List<SensorAcqResult>();
            this.ErrorCode = (int)Errors.SUCCESS;
            this.StoragedTimeType = SensorAcqResultTimeType.TaskFinishedTime;
        }

        public int GetSensorCount()
        {
            return this.SensorResults!=null? this.SensorResults.Count:0;
        }

        public void AddSensorResult(SensorAcqResult r)
        {
            this.SensorResults.Add(r);
        }

        public string GetJsonResult()
        {
            if (this.SensorResults.Count > 0)
            {
                var jsonstr=new StringBuilder();
                for (int i = 0; i < this.SensorResults.Count; i++)
                {
                    if (this.SensorResults[i].Data != null)
                    {
                        if (jsonstr.Length > 0)
                            jsonstr.Append(",");
                        jsonstr.Append(this.SensorResults[i].Data.JsonResultData);
                    }

                }
                if (jsonstr.Length == 0)
                {
                    return null;
                }
                jsonstr.Insert(0, "[");
                jsonstr.Append("]");
                return jsonstr.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 入库时使用时间类型
        /// </summary>
       public SensorAcqResultTimeType StoragedTimeType { get; set; }
    }
     
}
