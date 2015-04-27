// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveThread.cs" company="Free-Sun">
//   Add by lwl  2013.12.16
// </copyright>
// <summary>
//   The resolve thread.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using DataCenter.Model;
using DataCenter.WirelessCustomTransmission;
using log4net;

namespace DataCenter.Task
{
    /// <summary>
    ///  协 议 解 析线程.
    /// </summary>
    public class ResolveThread
    {
        private static ResolveThread _resolveThread;

        private static readonly ILog Log = LogManager.GetLogger(typeof(ResolveThread));

        private static readonly object SyncObject = new object();

        public static ResolveThread GetResolveThread()
        {
            if (_resolveThread == null)
            {
                lock (SyncObject)
                {
                    if (_resolveThread == null)
                    {
                        return _resolveThread = new ResolveThread();
                    }
                }
            }

            return _resolveThread;
        }
        
        private ResolveThread()
        {
            
        }

        public void StartSerive()
        {
            ReceiveBytes.GetReceiveBytes().AddNewDtuReceiveDataEventArgs += ResolveThread_AddNewDtuReceiveDataEventArgs;
        }

        void ResolveThread_AddNewDtuReceiveDataEventArgs(object sender, AddNewDTUDataArgs e)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((new ResolveReceiveBag()).GetAndResolveReceiveBag), e.DtuId);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public void StopService()
        {
            ReceiveBytes.GetReceiveBytes().AddNewDtuReceiveDataEventArgs -= ResolveThread_AddNewDtuReceiveDataEventArgs;
        }
        
    }
    
    internal class ResolveReceiveBag
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ResolveReceiveBag));

        private string dtuid { get; set; }

        public void GetAndResolveReceiveBag(object obj)
        {
            this.dtuid = obj as string;
            ReceiveBytes receivedata = ReceiveBytes.GetReceiveBytes();
            while (true)
            {
                if (receivedata.GetDtuReceivedBytesCount(this.dtuid) > 7)
                {
                    ReceiveDataInfo recedata = receivedata.GetReceivePackage(this.dtuid);
                    if (recedata != null)
                    {
                        try
                        {
                            var bt = new object();
                            bt = ReloveCustomTransData.GetInstance.ProtocolOrder(recedata);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.Message);
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}