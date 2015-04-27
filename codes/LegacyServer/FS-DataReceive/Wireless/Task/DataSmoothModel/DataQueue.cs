//  --------------------------------------------------------------------------------------------
//  <copyright file="DataQueue.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2013 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：20131223
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataCenter.Task.DataSmoothModel
{
    /// <summary>
    /// The data queue.
    /// </summary>
    internal class DataQueue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataQueue"/> class.
        /// </summary>
        public DataQueue()
        {
            this._dataValueQueue = new Queue<float>();
            this._dataList = new ArrayList(this._dataValueQueue.ToArray()); 
        }

        private Queue<float> _dataValueQueue;

        /// <summary>
        /// Gets the data value queue.
        /// </summary>
        public Queue<float> DataValueQueue
        {
            get { return this._dataValueQueue; }
        }

        /// <summary>
        /// The count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Count()
        {
            return this._dataValueQueue.Count;
        }

        /// <summary>
        /// The in queue.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void InQueue(float item)
        {
            this._dataValueQueue.Enqueue(item);
            this._dataList.Add(item);
            this._dataList.Sort();
        }

        /// <summary>
        /// The out queue.
        /// </summary>
        public void OutQueue()
        {
            var n = this._dataList.BinarySearch(this._dataValueQueue.Peek());
            this._dataValueQueue.Dequeue();
            this._dataList.RemoveAt(n);
        }

        private ArrayList _dataList;

        public ArrayList DataList
        {
            get { return this._dataList; }
        }

        /// <summary>
        /// The get average.
        /// </summary>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="endIndex">
        /// The end index.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public float GetAverage(int startIndex, int endIndex)
        {
            if (this._dataList.Count < endIndex)
            {
                throw new Exception("数据数量不足");
            }

            float aver = 0;
            for (var i = startIndex; i <= endIndex; i++)
            {
                aver += (float)this._dataList[i];
            }

            aver = aver / (endIndex - startIndex + 1);
            return aver;
        }

        /// <summary>
        /// The get average.
        /// </summary>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public float GetAverage()
        {
            if (this._dataList.Count <= 0)
            {
                throw new Exception("数据数量不足");
            }

            // foreach (float f in this._dataList)
            // {
            //    aver += f;
            // }
            float aver = this._dataList.Cast<float>().Sum();
            aver = aver / this._dataList.Count;
            return aver;
        }

    }
}