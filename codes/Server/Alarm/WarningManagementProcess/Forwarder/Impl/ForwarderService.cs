using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using FS.SMIS_Cloud.Alarm.Forwarder.Config;
using log4net;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Impl
{
    public class ForwarderService : IForwarder
    {
        private readonly List<IForwarder> _fowarders = new List<IForwarder>();
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
        private ForwarderServiceConfig _config;

        public void Init(ConfigurationElement config)
        {
            if (config is ForwarderServiceConfig)
            {
                _config = config as ForwarderServiceConfig;
                Init();
            }
        }

        public void Start()
        {
            foreach (IForwarder forwarder in _fowarders)
            {
                forwarder.Start();
            }
        }

        private void Init()
        {
            Init(_config.SerialSmSList);
            Init(_config.SerialSoundAndLightList);
            Init(_config.TcpSmsList);
        }


        private void Init(SerialSMSList serialSmsList)
        {
            foreach (SerialSMS serialsms in serialSmsList)
            {
                IForwarder serialSmsAbstractForwarder = new SerialSMSForwarder();
                try
                {
                    serialSmsAbstractForwarder.Init(serialsms);
                    _fowarders.Add(serialSmsAbstractForwarder);
                }
                catch (ConfigErrorException e)
                {
                    logger.Error("Failed to init serail sms forwarder.", e);
                }
            }
        }

        private void Init(SerialSoundAndLightList serialSoundAndLightList)
        {
            foreach (object serialsmandlight in serialSoundAndLightList)
            {
                IForwarder soundAndLightForwarder = new SoundAndLightForwarder();
                try
                {
                    soundAndLightForwarder.Init((ConfigurationElement) serialsmandlight);
                    _fowarders.Add(soundAndLightForwarder);
                }
                catch (ConfigErrorException e)
                {
                    logger.Error("Failed to init serail sound and light forwarder.", e);
                }
            }
        }

        private void Init(TcpSoundAndLightList tcpSmsList)
        {
            foreach (object tcpsms in tcpSmsList)
            {
                IForwarder tcpSmsForwarder = new TcpSMSForwarder();
                try
                {
                    tcpSmsForwarder.Init((ConfigurationElement) tcpsms);
                    _fowarders.Add(tcpSmsForwarder);
                }
                catch (ConfigErrorException e)
                {
                    logger.Error("Failed to init tcp sound and ligth forwarder.", e);
                }
            }
        }
    }
}