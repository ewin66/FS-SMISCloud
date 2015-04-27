using System.Text;
using FS.SMIS_Cloud.DAC.DAC;
using System;
using System.Collections.Generic;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;

namespace FS.SMIS_Cloud.DAC.Task
{
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
        public bool IsOK { get { return ErrorCode == (int)Errors.SUCCESS; } }

        public DACTaskResult()
        {
            SensorResults = new List<SensorAcqResult>();
            ErrorCode = (int)Errors.SUCCESS;
            StoragedTimeType = SensorAcqResultTimeType.TaskFinishedTime;
        }

        public int GetSensorCount()
        {
            return SensorResults!=null? SensorResults.Count:0;
        }

        public void AddSensorResult(SensorAcqResult r)
        {
            SensorResults.Add(r);
        }

        public string GetJsonResult()
        {
            if (this.SensorResults.Count > 0)
            {
                var jsonstr=new StringBuilder();
                for (int i = 0; i < SensorResults.Count; i++)
                {
                    if (SensorResults[i].Data != null)
                    {
                        if (jsonstr.Length > 0)
                            jsonstr.Append(",");
                        jsonstr.Append(SensorResults[i].Data.JsonResultData);
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
