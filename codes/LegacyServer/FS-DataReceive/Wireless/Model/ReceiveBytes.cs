#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ReceiveBytes.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140915 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using log4net;

namespace DataCenter.Model
{
    public class ReceiveBytes
    {
        private ConcurrentDictionary<string, DataList<byte>> ReceivedBytes;
        private static readonly ILog Log = LogManager.GetLogger(typeof (ReceiveBytes));

        private ReceiveBytes()
        {
            ReceivedBytes = new ConcurrentDictionary<string, DataList<byte>>();
        }

        private static ReceiveBytes _receiveBytes = new ReceiveBytes();

        public static ReceiveBytes GetReceiveBytes()
        {
            return _receiveBytes;
        }

        public void AddReceiveBytes(string dtuid, byte[] data)
        {
            if (!this.ReceivedBytes.ContainsKey(dtuid))
            {
                this.ReceivedBytes.TryAdd(dtuid, new DataList<byte>());
                if (_addNewDtuReceiveDataEventArgs != null)
                {
                    this._addNewDtuReceiveDataEventArgs(this, new AddNewDTUDataArgs {DtuId = dtuid});
                }
            }
            this.ReceivedBytes[dtuid].AddRange(data);
        }

        public ReceiveDataInfo GetReceivePackage(string dtuid)
        {
            try
            {
                if (this.ReceivedBytes.ContainsKey(dtuid))
                {
                    if (this.ReceivedBytes[dtuid].Count > 7)
                    {
                        int i = 0;
                        while (this.ReceivedBytes[dtuid].Count > 0 &&
                               (Convert.ToInt16((this.ReceivedBytes[dtuid][0])) != 0xFE
                                || Convert.ToInt16((this.ReceivedBytes[dtuid][1])) != 0xEF))
                        {
                            this.ReceivedBytes[dtuid].RemoveAt(0);
                            i++;
                            if (this.ReceivedBytes[dtuid].Count <= 0 || i >= 500)
                            {
                                return null;
                            }
                        }
                        byte[] lenbytes = {(byte) this.ReceivedBytes[dtuid][5], (byte) this.ReceivedBytes[dtuid][6]};
                        var len = BitConverter.ToInt16(lenbytes, 0);
                        if (len <= 0 || len >= 1024)
                        {
                            this.ReceivedBytes[dtuid].RemoveAt(0);
                        }
                        else
                        {
                            if (this.ReceivedBytes[dtuid].Count >= len)
                            {
                                if (Convert.ToInt16(this.ReceivedBytes[dtuid][len - 1]) == 0x0D)
                                {
                                    var by = new byte[len];
                                    this.ReceivedBytes[dtuid].CopyTo(0, by, 0, len);
                                    this.ReceivedBytes[dtuid].RemoveRange(0, len);
                                    return new ReceiveDataInfo(dtuid, by);
                                }
                                this.ReceivedBytes[dtuid].RemoveAt(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("{0}\n\r{1}", ex.Message, ex.StackTrace);
            }
            return null;
        }

        public int GetDtuReceivedBytesCount(string dtuid)
        {
            return this.ReceivedBytes[dtuid].Count;
        }

        /// <summary>
        /// The _dtu on off line log.
        /// </summary>
        private event EventHandler<AddNewDTUDataArgs> _addNewDtuReceiveDataEventArgs;

        /// <summary>
        /// The dtu on off line log.
        /// </summary>
        public event EventHandler<AddNewDTUDataArgs> AddNewDtuReceiveDataEventArgs
        {
            add { _addNewDtuReceiveDataEventArgs = value; }
            remove { _addNewDtuReceiveDataEventArgs -= value; }
        }
    }

    public class DataList<T>
    {
        private List<T> _list = new List<T>();

        public int Count
        {
            get
            {
                lock (this)
                {
                    return this._list.Count;
                }
            }
        }

        public T this[int index]
        {
            get
            {
                lock (this)
                {
                    return this._list[index];
                }
            }

            set
            {
                lock (this)
                {
                    this._list[index] = value;
                }
            }
        }

        public void Add(T item)
        {
            lock (this)
            {
                this._list.Add(item);
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            lock (this)
            {
                this._list.AddRange(collection);
            }
        }

        public void RemoveAt(int index)
        {
            lock (this)
            {
                this._list.RemoveAt(index);
            }
        }

        public void RemoveRange(int index, int count)
        {
            lock (this)
            {
                this._list.RemoveRange(index, count);
            }
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            lock (this)
            {
                try
                {
                    Array.Copy(this._list.ToArray(), index, array, arrayIndex, count);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}