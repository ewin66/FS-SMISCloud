//  --------------------------------------------------------------------------------------------
//  <copyright file="DeviceInfoBll.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2013 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：lwl 20131223
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using DataCenter.Accessor.ViewDal;
using DataCenter.Model;

namespace DataCenter.Accessor.ViewBLL
{
    /// <summary>
    /// The device info bll.
    /// </summary>
    public class DeviceInfoBll
    {
        /// <summary>
        /// The get structure sensor device info.
        /// </summary>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public DataTable GetPressStructureSensorDeviceInfo()
        {
            using (DataTable dt = DeviceInfoDal.GetPressStructureSensorDeviceInfo())
            {
                return dt;
            }
        }

        /// <summary>
        /// The get pressure data by sensor.
        /// </summary>
        /// <param name="sensorId">
        /// The sensor id.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public DataTable GetPressureDataBySensor(int sensorId, int count)
        {
            using (DataTable dt = DeviceInfoDal.GetPressureDataBySensor(sensorId, count))
            {
                return dt;
            }
        }

        /// <summary>
        /// The get gps structure sensor device info.
        /// </summary>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public DataTable GetGpsStructureSensorDeviceInfo()
        {
            using (DataTable dt = DeviceInfoDal.GetGpsStructureSensorDeviceInfo())
            {
                return dt;
            }
        }


        public IList<SensorInfo> GetAllDeviceInfo()
        {
            return DeviceInfoDal.GetAllDeviceInfo();
        }
    }
}