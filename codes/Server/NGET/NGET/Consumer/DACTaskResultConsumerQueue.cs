// --------------------------------------------------------------------------------------------
// <copyright file="ConsumerQueue.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20141106
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.NGET.Consumer
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 消费者队列
    /// </summary>
    public class DacTaskResultConsumerQueue
    {
        private List<IDacTaskResultConsumer> list;

        public ConsumeType ComsumeType { get; private set; }

        public int Length { get { return this.list.Count; } }

        public DacTaskResultConsumerQueue(ConsumeType consumeType)
        {
            this.ComsumeType = consumeType;
            this.list = new List<IDacTaskResultConsumer>();
        }

        public IDacTaskResultConsumer this[int index]
        {
            get
            {
                return this.list[index];
            }
        }

        /// <summary>
        /// 插队  (在队列固定位置插入消费者,插入后,其后的消费者序号向后移动)
        /// </summary>
        /// <param name="consumer">消费者</param>
        /// <param name="queueNum">队列序号(从1开始)</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="queueNum"/> 小于 1。 - 或 - 
        /// <paramref name="queueNum"/> 大于 <see cref="P:FS.SMIS_Cloud.DAC.ConsumerQueue.Length"/>。
        /// </exception>
        public void Insert(IDacTaskResultConsumer consumer, int queueNum)
        {
            lock (((ICollection)this.list).SyncRoot)
            {
                if (this.list.Count == 0)
                {
                    this.list.Add(consumer);
                }
                else
                {
                    this.list.Insert(queueNum - 1, consumer);
                }
            }
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="consumer"></param>
        public void Enqueue(IDacTaskResultConsumer consumer)
        {
            lock (((ICollection)this.list).SyncRoot)
            {
                this.list.Add(consumer);
            }
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <returns></returns>
        public IDacTaskResultConsumer Dequeue()
        {
            lock (((ICollection)this.list).SyncRoot)
            {
                if (this.list.Count > 0)
                {
                    var consumer = this.list[0];
                    this.list.RemoveAt(0);

                    return consumer;
                }
                return null;
            }
        }
    }

    public enum ConsumeType
    {
        /// <summary>
        /// 异步消费
        /// </summary>
        Async, 
        /// <summary>
        /// 同步消费
        /// </summary>
        Sync
    }
}