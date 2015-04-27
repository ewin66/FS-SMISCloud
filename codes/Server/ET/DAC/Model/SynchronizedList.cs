#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="SynchronizedList.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150409 by LINGWENLONG .
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FS.SMIS_Cloud.DAC.Model
{
    public class SynchronizedList<T>
    {
        private readonly HashSet<T> _list;

        public SynchronizedList()
        {
            this._list = new HashSet<T>();
        }

        public SynchronizedList(IEnumerable<T> collection)
        {
            this._list = new HashSet<T>(collection);
        }

        public object SyncRoot = new object();

        public int Count
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._list.Count;
                }
            }            
        }

        public void Add(T item)
        {
            lock (this.SyncRoot)
            {
                this._list.Add(item);
            }
        }

        public void Clear()
        {
            lock (this.SyncRoot)
            {
                this._list.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (this.SyncRoot)
            {
                return this._list.Contains(item);
            }
        }

        public bool Remove(T item)
        {
            lock (this.SyncRoot)
            {
              return  this._list.Remove(item);
            }
        }

        public int RemoveWhere(Predicate<T> match)
        {
            lock (this.SyncRoot)
            {
                return this._list.RemoveWhere(match);
            }
        }

        public HashSet<T>.Enumerator GetEnumerator()
        {
            return this._list.GetEnumerator();
        }

        public T FirstOrDefault()
        {
            lock (this.SyncRoot)
                return this._list.FirstOrDefault();
        }

        public T FirstOrDefault(Func<T, bool> predicate)
        {
            lock (this.SyncRoot)
                return this._list.FirstOrDefault(predicate);
        }
        
        public bool Any(Func<T, bool> predicate)
        {
            lock (this.SyncRoot)
                return this._list.Any(predicate);
        }
    }
}