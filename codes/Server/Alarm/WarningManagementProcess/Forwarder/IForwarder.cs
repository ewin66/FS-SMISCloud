using System.Configuration;

namespace FS.SMIS_Cloud.Alarm.Forwarder
{
    public interface IForwarder
    {
        void Init(ConfigurationElement config);
        void Start();

        /**
         * @Todo 完整的消息驱动的方式
         * Alarm(AlarmInfo alarm) 
         * ConfirmAlarm(AlarmInfo alarm)
         * RestoreAlarm(AlarmInfo alarm)
         * 从告警服务获取不同级别的告警数量，而不是数据库
         * 
         */
    }
}