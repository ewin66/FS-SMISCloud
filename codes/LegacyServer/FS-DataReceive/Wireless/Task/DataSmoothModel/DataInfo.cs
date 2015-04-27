//  --------------------------------------------------------------------------------------------
//  <copyright file="DataInfo.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace DataCenter.Task.DataSmoothModel
{
    /// <summary>
    /// The data info.
    /// </summary>
    internal class DataInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataInfo"/> class.
        /// </summary>
        public DataInfo()
        {
            this.dataValue = new DataQueue();
        }

        /// <summary>
        /// Gets or sets the sensor id.
        /// </summary>
        public int SensorID { get; set; }

        /// <summary>
        /// Gets or sets the structure id.
        /// </summary>
        public int StructureID { get; set; }

        /// <summary>
        /// Gets or sets the module id.
        /// </summary>
        public string ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the channel id.
        /// </summary>
        public int ChannelId { get; set; }

        private DataQueue dataValue;

        /// <summary>
        /// Gets or sets the obj.
        /// </summary>
        public object Obj { get; set; }

        /// <summary>
        /// The in queue.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void InQueue(float item)
        {
            this.dataValue.InQueue(item);
        }

        /// <summary>
        /// The out queue.
        /// </summary>
        public void OutQueue()
        {
            this.dataValue.OutQueue();
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
        public float GetAverage(int startIndex, int endIndex)
        {
           return this.dataValue.GetAverage(startIndex, endIndex);
        }

        /// <summary>
        /// The get average.
        /// </summary>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public float GetAverage()
        {
            return this.dataValue.GetAverage();
        }

        /// <summary>
        /// The get count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetCount()
        {
            return this.dataValue.Count();
        }

    }
}