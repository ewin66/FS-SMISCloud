namespace FS.SMIS_Cloud.NGET.Model
{
    using System;

    /// <summary>
    ///  一次采集动作返回的传感器数据
    /// </summary>
    public class SensorAcqResult
    {
        // Sensor
        public Sensor Sensor{ get; set; }

        // 错误码
        public bool IsOK { get { return this.ErrorCode == 0; } }

        public int ErrorCode { get; set; }

        public int AcqNum { get; set; }

        // 采集时间
        public DateTime AcqTime { get; set; }

        public string[] Request { get; set; }

        public string[] Response { get; set; }

        // 解码后数据.
        public SensorData Data { get; set; }
    }
}