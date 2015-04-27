using System;
using System.Threading;
using ZYB;

namespace DataCenter.Communication.comm
{
  public class Communication:IDisposable
    {
        public EventHandler ReceiveEvent;
        private Communication(){}
        private static volatile Communication _communication;
        private static readonly object SyncRoot =new object();
       // private static readonly ILog Log = LogManager.GetLogger(typeof(Communication));
        public static Communication GetInstance()
        {
            if (_communication == null)
            {
                lock (SyncRoot)
                {
                    if (_communication == null)
                    {
                        _communication = new Communication();
                    }
                }
            }
            return _communication;
        }

       public Server GetServer
       {
           get {return _svr;}
       }

       private Server _svr;
       public bool StartService(int port, int timeout, Mode mode)
        {
            try
            {
                _svr = new Server(port, timeout, mode);
                _svr.ReceiveData += _svr_ReceiveData;
                _svr.ClientConnect += _svr_ClientConnect;
                _svr.ClientClose += _svr_ClientClose;
                _svr.Start();
                return true;
            }
            catch (Exception ex)
            {
              //  Log.Error(ex.Message);
                throw;
            }
        }

       public bool StopService()
       {
           try
           {
               if (_svr != null)
               {
                   _svr.Stop();
                   return true;
               }
           }
           catch (Exception ex)
           {
              // Log.Error(ex.Message);
               throw; 
           }
           return false;
       }

       public bool Send(ComPacket e)
       {
           Monitor.Enter(this);
           try
           {
               if (e != null)
               {
                   if (!string.IsNullOrEmpty(e.DtuId) && e.Item != null)
                       try
                       {
                           //Log.Debug(e.DtuId + " # # : " + ValueHelper.ByteToHexStr(e.Item));
                           return _svr.Send(e.DtuId, e.Item);
                       }
                       catch (Exception ex)
                       {
                           throw ex;
                       }
               }
               return false;
           }
           finally
           {
               Monitor.Exit(this);
           }
       }

       void _svr_ClientClose(object sender, ZYBEventArgs e)
        {
            Monitor.Enter(sender);
            try
            {
                if (ReceiveEvent != null)
                    ReceiveEvent(sender, new ComPacketEventArgs((byte)ReceiveType.Offline, e));
            }
            finally
            {
                Monitor.Exit(sender);
            }
        }

        void _svr_ClientConnect(object sender, ZYBEventArgs e)
        {
            Monitor.Enter(sender);
            try
            {
                if(ReceiveEvent!=null)
                    ReceiveEvent(sender, new ComPacketEventArgs((byte)ReceiveType.Online, e));
            }
            finally 
            {
                Monitor.Exit(sender);
            }
        }

        void _svr_ReceiveData(object sender, ZYBEventArgs e)
        {
            Monitor.Enter(sender);
            try
            {
                if (ReceiveEvent != null)
                {
                    var dtuid =(string) e.DTU.ID.Clone();
                    var data = (byte[]) e.DTU.DataByte.Clone();
                    ReceiveEvent(sender,
                        new ComPacket((byte) ReceiveType.Data, data,dtuid));
                  //  Log.Debug(dtuid + " @ @ : " + ValueHelper.ByteToHexStr(data));
                }
            }
            finally
            {
                Monitor.Exit(sender);
            }
        }

       public void Dispose()
       {
           throw new NotImplementedException();
       }
    }
}
