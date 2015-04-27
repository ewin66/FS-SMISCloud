using System;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Node;

namespace FS.SMIS_Cloud.DAC.DAC
{
    /// <summary>
    ///  一次采集动作返回的传感器数据
    /// </summary>
    [Serializable]
    public class SensorAcqResult
    {
        //DTU
        public string DtuCode { get; set; }
        
        // Sensor
        public Sensor Sensor{ get; set; }

        // 错误码
        public int ErrorCode { get; set; }

        // 错误信息提示
        public string ErrorMsg { get; set; }

        // 请求串
        public byte[] Request { get; set; }

        // 数据请求时间
        public DateTime RequestTime { get; set; }

        // 应答串
        public byte[] Response { get; set; }

        // 应答数据时间
        public DateTime ResponseTime { get; set; }

        // 实际耗时. (ms)
        public long Elapsed { get; set; }

        // 解码后数据.
        public ISensorData Data { get; set; }

        public SensorAcqResult()
        {
            ErrorCode = (int)Errors.ERR_DEFAULT;
        }

        public bool IsOK { get { return ErrorCode == 0; } }

        public int CalcPlanState = 0;
    }
}
