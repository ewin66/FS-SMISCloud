using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using DataCenter.Accessor;
using DataCenter.Communication.Communication;
using DataCenter.Model;
using DataCenter.Util;
using ET.Common;
using ET.Common.Check;
using log4net;


namespace DataCenter.Task
{
    public class CollectThread
    {
        private static CollectThread collectThread = null;

        private static readonly object SyncRoot = new object();
        private static readonly ILog Log = LogManager.GetLogger(typeof(CollectThread));
        private static readonly ConcurrentBag<string> onlinedtus = new ConcurrentBag<string>();


        /// <summary>
        /// Prevents a default instance of the <see cref="CollectThread"/> class from being created.
        /// </summary>
        private CollectThread()
         {
         }

        /// <summary>
        /// The create collect thread instance.
        /// </summary>
        /// <returns>
        /// The <see cref="CollectThread"/>.
        /// </returns>
        public static CollectThread CreateCollectThreadInstance()
        {
            if (collectThread == null)
            {
                lock (SyncRoot)
                {
                    if (collectThread == null)
                    {
                        return collectThread = new CollectThread();
                    }
                }
            }

            return collectThread;
        }
        
        private ModBusWrapper _modbusWrapper;

        /// <summary>
        /// The resolve thread.
        /// </summary>
        public void StartService()
        {
            try
            {
                SqlDal.UpdateAllDtuStatus();
                this._modbusWrapper = ModBusWrapper.CreateInstance(Protocol.TCPIP);
                this._modbusWrapper.OnDataReceived += this.Wrapper_ReceiveData;
                this._modbusWrapper.OnConnectionChangedHandler += OnConnectionChangedEvevt;
                this._modbusWrapper.StartService();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public void OnConnectionChangedEvevt(DTUConnectionEventArgs args)
        {
            if (DtuOnOffLineLogEventArgs != null)
            {
                DtuOnOffLineLogEventArgs(args);
            }
            SqlDal.UpdateDTUStatus(args);
            updateonlineDtus(args);
        }

        private void updateonlineDtus(DTUConnectionEventArgs args)
        {
            if (args.Status == ReceiveType.Online)
            {
                if (!onlinedtus.Contains(args.DtuId))
                    onlinedtus.Add(args.DtuId);
            }
            else
            {

                if (onlinedtus.Contains(args.DtuId))
                {
                    var dtu = args.DtuId;
                    onlinedtus.TryTake(out dtu);
                }
            }
        }

        public static void UpdateAllOnlineDtus()
        {
           SqlDal.UpdateAllDtuStatus(onlinedtus.ToList());
        }

        public void StopService()
        {
            try
            {
                if (this._modbusWrapper != null)
                {
                    this._modbusWrapper.OnDataReceived -= this.Wrapper_ReceiveData;
                    this._modbusWrapper.OnConnectionChangedHandler -= OnConnectionChangedEvevt;
                    this._modbusWrapper.StopService();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// The wrapper_ receive data.
        /// </summary>
        /// <param name="buff"></param>
        private void Wrapper_ReceiveData(ReceiveDataInfo buff)
        {
            try
            {
                if (buff != null)
                {

                    if (DataHexShowStringLogEventArgs != null)
                    {
                        DataHexShowStringLogEventArgs(buff);
                    }
                    var str = new StringBuilder();
                    str.Append(buff.Sender).Append(ValueHelper.ByteToHexStr(buff.PackagesBytes));
                    Log.Debug(str.ToString());
                    ReceiveBytes.GetReceiveBytes().AddReceiveBytes(buff.Sender, buff.PackagesBytes);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public void Send(ComPackage ce)
        {
            this._modbusWrapper.Send(ce);
        }

        #region Dtu上下线事件

        /// <summary>
        /// The dtu on off line log.
        /// </summary>
        public static event OnConnectionChangedListener DtuOnOffLineLogEventArgs;
        #endregion Dtu上下线事件

        #region 界面数据显示事件

        public static OnDataHexShowStringLog DataHexShowStringLogEventArgs;

        #endregion 界面数据显示事件

        public byte[] CreateSatateBytes(byte i, string dtuid)
        {
            byte[] packBytes = new byte[7];
            packBytes[0] = 0xfe;
            packBytes[1] = 0xef;
            // Array.Copy(,0,packBytes,2,2);
            packBytes[4] = i;
            byte[] crc16 = CheckModeResult.GetCheckResult(packBytes, 0, 3, CheckType.CRC16HighByteFirst);
            packBytes[5] = crc16[0];
            packBytes[6] = crc16[1];
            return packBytes;
        }
    }

    public delegate void OnDataHexShowStringLog(ReceiveDataInfo dataargs);
}