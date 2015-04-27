using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using FS.SMIS_Cloud.Alarm.Forwarder.Config;
using FS.SMIS_Cloud.Alarm.Forwarder.Dal;
using FS.SMIS_Cloud.Alarm.Forwarder.Model;
using ZYB;
using log4net;
using Mode = ZYB.Mode;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Impl
{
    public class TcpSMSForwarder : IForwarder
    {
        private const byte B4Mask = 0x30; // 00110000
        private const byte B5Mask = 0x30; // 00110000
        private const byte B5MaskFilter = 0x01; // 0000001
        private readonly object _syncRoot = new object();
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
        private string _deviceId = string.Empty;
        private List<string> _organizationids = new List<string>();
        private TcpSoundAndLight _serialParam;
        private List<string> _structureId = new List<string>();
        private Server _svr;
        private Thread _thread;
        private Hashtable table = new Hashtable();

        public void Init(ConfigurationElement config)
        {
            if (config is TcpSoundAndLight)
            {
                _serialParam = config as TcpSoundAndLight;
                _organizationids.Clear();
                table.Clear();
                _organizationids = _serialParam.Organizationids.Replace(" ", "").Trim(',').Split(',').ToList();
            }
            else
            {
                throw new ConfigErrorException("Invalid serial configurationn in TcpSoundAndLightForwarder.");
            }
        }


        public void Start()
        {
            _svr = new Server(_serialParam.Port, _serialParam.Listtimeout, (Mode) _serialParam.Mode);
            _svr.ClientConnect += ClientConnect;
            _svr.ClientClose += ClientClose;
            try
            {
                _svr.Start();
                logger.Info(_serialParam.Port.ToString() + " port successfully openned");
            }
            catch (Exception ex)
            {
                logger.Error(
                    string.Format("Failed to open  {0}  port,please change the port restart", _serialParam.Port) +
                    ex.Message);
            }
        }

        private void ClientConnect(object sender, ZYBEventArgs e)
        {
            Monitor.Enter(sender);
            try
            {
                _deviceId = e.DTU.ID;
                logger.Info(string.Format("DTU {0} is online", e.DTU.ID));
                //_thread = new Thread(Run);
                //_thread.Start();
                _thread = new Thread(new ParameterizedThreadStart(Run));
                _thread.Name = e.DTU.ID;
                if (!table.ContainsKey(e.DTU.ID))
                {
                    table.Add(e.DTU.ID, _thread);
                    _thread.Start(e.DTU.ID);
                }
            }
            finally
            {
                Monitor.Exit(sender);
            }
        }

        private void ClientClose(object sender, ZYBEventArgs e)
        {
            Monitor.Enter(sender);
            try
            {
                //_thread.Abort();
                if (table.ContainsKey(e.DTU.ID))
                {
                    Thread t = (Thread)table[e.DTU.ID];
                    t.Abort();
                    table.Remove(e.DTU.ID);
                }
                logger.Info(string.Format("DTU {0} is offline", e.DTU.ID));
            }
            finally
            {
                Monitor.Exit(sender);
            }
        }

        private void Run(object id)
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
                SlWarningInfo(id);
                Thread.Sleep(_serialParam.Interval);
            }
        }

        /// <summary>
        ///     告警条数-灯控
        /// </summary>
        private void SlWarningInfo(object id)
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
            Console.WriteLine(string.Format("Alarm count: Critical({0})    Major({1})   Minor({2})   Warning({3})",
                                            criticalCount, majorCount, minorCount, warningCount));
            ControlLamp(id,_serialParam.Warning.ModeInt, false);
            ControlLamp(id,_serialParam.Minor.ModeInt, false);
            ControlLamp(id,_serialParam.Major.ModeInt, false);
            ControlLamp(id,_serialParam.Critical.ModeInt, false);
            if (warningCount > 0)
            {
                ControlLamp(id,_serialParam.Warning.ModeInt, true);
            }
            if (minorCount > 0)
            {
                ControlLamp(id,_serialParam.Minor.ModeInt, true);
            }
            if (majorCount > 0)
            {
                ControlLamp(id,_serialParam.Major.ModeInt, true);
            }
            if (criticalCount > 0)
            {
                ControlLamp(id,_serialParam.Critical.ModeInt, true);
            }
        }

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

        private void ControlLamp(object id,int str, bool switchFlag)
        {
            byte[] atCmd = CreateATCommond(str, switchFlag);
            try
            {
                if (atCmd != null)
                {
                    if (_svr.Send(id.ToString(), atCmd))
                    {
                        logger.Info(string.Format("{0} send success", ByteToHexStr(atCmd)));
                    }
                    else
                    {
                        logger.Info(string.Format("{0} send failed", ByteToHexStr(atCmd)));
                    }
                }
                else
                {
                    logger.Info("the package is null");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to control alarm lamp.", e);
            }
            Thread.Sleep(200);
        }

        private string ByteToHexStr(byte[] by)
        {
            string text = "";
            for (int i = 0; i < by.Length; i++)
            {
                text = text + Convert.ToString(by[i], 16).PadLeft(2, '0') + " ";
            }
            return text;
        }
    }
}