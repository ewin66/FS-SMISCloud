using System;
using System.Threading;
using DataCenter.Model;
using DataCenter.Util;
using ZYB;

namespace DataCenter.Communication.Communication
{
    internal class ModBusTCPIPWrapper : ModBusWrapper, IDisposable
    {
        private Server _svr;

        private Mode _mode = (Mode) Convert.ToInt16(ConfigSettings.ReadConfigValue("Mode", "0"));
        private int _port = Convert.ToInt32(ConfigSettings.ReadConfigValue("Port", "5005"));
        private int _listTimeout = Convert.ToInt32(ConfigSettings.ReadConfigValue("ListTimeout", "200"));
        private readonly object _syncRoot = new object();

        public override bool StartService()
        {
            _svr = new Server(_port, _listTimeout, _mode);
            _svr.ReceiveData += new Server.ZYBEvent(this.svr_ReceiveData);
            _svr.ClientConnect += new Server.ZYBEvent(this.svr_ClientConnect);
            _svr.ClientClose += new Server.ZYBEvent(this.svr_ClientClose);
            try
            {
                _svr.Start();
                Console.WriteLine("开启端口成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine("开启端口失败:" + ex.Message);
                return false;
            }
            return true;
        }

        public override bool StopService()
        {
            try
            {
                _svr.Stop();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override bool Send(object obj)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                var comPackage = obj as ComPackage;
                if (comPackage != null) if (_svr.Send(comPackage.Id, (byte[]) comPackage.Item)) return true;
                return false;
            }
            finally
            {
                Monitor.Exit(_syncRoot);
            }
        }
        
        private void svr_ReceiveData(object sender, ZYBEventArgs e)
        {
            Monitor.Enter(sender);
            try
            {
                if (OnDataReceived != null)
                {
                    OnDataReceived(new ReceiveDataInfo(e.DTU.ID, (byte[])e.DTU.DataByte.Clone()));
                }
            }
            finally
            {
                Monitor.Exit(sender);
            }
        }

        private void svr_ClientConnect(object sender, ZYBEventArgs e)
        {
            Monitor.Enter(sender);
            try
            {
                if (OnConnectionChangedHandler != null)
                {
                    OnConnectionChangedHandler(new DTUConnectionEventArgs
                    {
                        DtuId = e.DTU.ID,
                        Status = ReceiveType.Online,
                        Time = DateTime.Now,
                        Ip = e.DTU.IP,
                        PhoneNumber = e.DTU.PhoneNumber,
                        RefreshTime = e.DTU.RefreshTime,
                        LoginTime = e.DTU.LoginTime
                    });
                }
            }
            finally
            {
                Monitor.Exit(sender);
            }
        }

        private void svr_ClientClose(object sender, ZYBEventArgs e)
        {
            Monitor.Enter(sender);
            try
            {
                if (OnConnectionChangedHandler != null)
                {
                    OnConnectionChangedHandler(new DTUConnectionEventArgs
                    {
                        DtuId = e.DTU.ID,
                        Status = ReceiveType.Offline,
                        Time = DateTime.Now,
                        Ip = e.DTU.IP,
                        PhoneNumber = e.DTU.PhoneNumber,
                        RefreshTime = e.DTU.RefreshTime,
                        LoginTime = e.DTU.LoginTime
                    });
                }
            }
            finally
            {
                Monitor.Exit(sender);
            }
        }

        #region IDisposable 成员

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion IDisposable 成员
    }
}