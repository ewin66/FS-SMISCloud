using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Threading;
using FS.SMIS_Cloud.Alarm.Forwarder.Config;
using FS.SMIS_Cloud.Alarm.Forwarder.Dal;
using FS.SMIS_Cloud.Alarm.Forwarder.Model;
using log4net;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Impl
{
    public class SoundAndLightForwarder : IForwarder
    {
        private const byte B4Mask = 0x30; // 00110000
        private const byte B5Mask = 0x30; // 00110000
        private const byte B5MaskFilter = 0x01; // 0000001
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
        private List<string> _organizationids = new List<string>();
        private SerialSoundAndLight _serialParam;
        private SerialPort _serialPort;
        private List<string> _structureId = new List<string>();
        private Thread _thread;

        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="config"></param>
        public void Init(ConfigurationElement config)
        {
            if (config is SerialSoundAndLight)
            {
                _serialParam = config as SerialSoundAndLight;
                _serialPort = new SerialPort(_serialParam.Name, _serialParam.Baudrate);
                _organizationids.Clear();
                _organizationids = _serialParam.Organizationids.Replace(" ", "").Trim(',').Split(',').ToList();
                if (OpenPort())
                {
                    logger.Info(_serialParam.Name + " is successfully openned");
                }
                else
                {
                    logger.Info(_serialParam.Name + " abnormal");
                }
            }
            else
            {
                throw new ConfigErrorException("Invalid serial configurationn in soundAndLightAbstractForwarder.");
            }
        }

        public void Start()
        {
            _thread = new Thread(Run);
            _thread.Start();
        }

        private void Run()
        {
            while (true)
            {
                if (_organizationids.Count > 0 && _organizationids[0] != string.Empty && _organizationids[0] != " ")
                {
                    _structureId = DataAccess.GetStructureIdsByOrganizationId(_organizationids);
                }
                else
                {
                    _structureId.Clear();
                }
                SlWarningInfo();
                Thread.Sleep(_serialParam.Interval);
            }
        }

        /// <summary>
        ///     开启串口
        /// </summary>
        /// <returns></returns>
        private bool OpenPort()
        {
            if (!_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                }
                catch (Exception e)
                {
                    logger.Error(string.Format("Failed to open com port: {0}", _serialPort.PortName), e);
                }
            }
            return _serialPort.IsOpen;
        }

        /// <summary>
        ///     告警条数-灯控
        /// </summary>
        private void SlWarningInfo()
        {
            int criticalCount = _structureId.Count > 0
                                    ? DataAccess.GetWarningCntByLevel((int) WarningLevel.Critical, _structureId)
                                    : 0;
            int minorCount = _structureId.Count > 0
                                 ? DataAccess.GetWarningCntByLevel((int) WarningLevel.Minor, _structureId)
                                 : 0;
            int majorCount = _structureId.Count > 0
                                 ? DataAccess.GetWarningCntByLevel((int) WarningLevel.Major, _structureId)
                                 : 0;
            int warningCount = _structureId.Count > 0
                                   ? DataAccess.GetWarningCntByLevel((int) WarningLevel.Warning, _structureId)
                                   : 0;
            logger.Debug(string.Format("Alarm count: Critical({0})    Major({1})   Minor({2})   Warning({3})",
                                       criticalCount, majorCount, minorCount, warningCount));
            ControlLamp(_serialParam.WarningMode.ModeInt, false);
            ControlLamp(_serialParam.MinorMode.ModeInt, false);
            ControlLamp(_serialParam.MajorMode.ModeInt, false);
            ControlLamp(_serialParam.CriticalMode.ModeInt, false);
            if (warningCount > 0)
            {
                ControlLamp(_serialParam.WarningMode.ModeInt, true);
            }
            if (minorCount > 0)
            {
                ControlLamp(_serialParam.MinorMode.ModeInt, true);
            }
            if (majorCount > 0)
            {
                ControlLamp(_serialParam.MajorMode.ModeInt, true);
            }
            if (criticalCount > 0)
            {
                ControlLamp(_serialParam.CriticalMode.ModeInt, true);
            }
        }

        /// <summary>
        ///     组包
        /// </summary>
        /// <param name="str"></param>
        /// <param name="switchFlag">true：打开，false:关闭</param>
        /// <returns></returns>
        private byte[] CreateATCommond(int mode, bool switchFlag)
        {
            var package = new byte[7];
            package[0] = 0x40;
            package[1] = 0x3F;
            package[2] = 0x3F;
            package[3] = (byte) (switchFlag ? 0x31 : 0x30);
            package[4] = (byte) (B4Mask | (mode >> 1));
            package[5] = (byte) (B5Mask | ((mode & B5MaskFilter) << 3));
            package[6] = 0x21;
            return package;
        }

        private void ControlLamp(int str, bool switchFlag)
        {
            byte[] atCmd = CreateATCommond(str, switchFlag);
            OpenPort();
            try
            {
                _serialPort.Write(atCmd, 0, atCmd.Length);
            }
            catch (Exception e)
            {
                logger.Error("Failed to control alarm lamp.", e);
            }
            Thread.Sleep(200);
        }
    }
}