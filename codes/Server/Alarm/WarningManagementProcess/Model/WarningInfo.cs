using System;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Model
{
    public class WarningInfo
    {
        public int Id { get; set; }
        public string WarningTypeId { get; set; }
        public string StructId { get; set; }
        public int DeviceId { get; set; }
        public int DeviceTypeId { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public string WarningLevel { get; set; }
        public string Content { get; set; }
    }

    public class WarningType
    {
        public string TypeId { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public string WarningLevel { get; set; }
    }

    public class RemoteDtu
    {
        public int Id { get; set; }
        public string RemoteDtuNumber { get; set; }
        public string Description { get; set; }
    }

    public class Sensor
    {
        public int Sensor_Id { get; set; }
        public string SensorLocationDs { get; set; }
        public string Dtu_Id { get; set; }
        public string Module_No { get; set; }
        public string Dai_Channel_Number { get; set; }
    }

    public enum WarningLevel
    {
        /// <summary>
        ///     告警级别1
        /// </summary>
        Critical = 1,

        /// <summary>
        ///     告警级别2
        /// </summary>
        Major = 2,

        /// <summary>
        ///     告警级别3
        /// </summary>
        Minor = 3,

        /// <summary>
        ///     告警级别4
        /// </summary>
        Warning = 4
    }

    public enum WarningStatus
    {
        WaitToSendToSupport = 1,
        NoSupportReceiverOrEnqueue = 2,
        SupportReceived = 3,
        WaitToSendToClient = 4,
        NoClientReceiverOrEnqueue = 5,
        ClientReceived = 6,
    }

    public enum DealFlag
    {
        UnConfiremBySupport = 1,
        ConfiremedBySupport = 2,
        UnConfiremByClient = 3,
        ConfiremedByClient = 4,
    }

    public enum DeviceType
    {
        Dtu = 1,
        Sensor = 2,
    }
}