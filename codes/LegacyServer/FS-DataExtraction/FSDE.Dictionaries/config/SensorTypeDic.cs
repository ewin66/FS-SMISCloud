// // --------------------------------------------------------------------------------------------
// // <copyright file="SensorTypeDic.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140606
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FSDE.BLL.Config;
using FSDE.Model.Fixed;

namespace FSDE.Dictionaries
{
    public class SensorTypeDic
    {
        private Dictionary<int, SensorType> sensorTypeInfos;

        public static SensorTypeDic sensorTypeDic = new SensorTypeDic();

        public static SensorTypeDic GetSensorTypeDic()
        {
            return sensorTypeDic;
        }

        private SensorTypeDic()
        {
            if (sensorTypeInfos == null)
            {
                sensorTypeInfos = new Dictionary<int, SensorType>();
                var bll = new SensorTypeBll();
                List<SensorType> list = bll.SelectList().ToList();
                foreach (var type in list)
                {
                    sensorTypeInfos.Add(Convert.ToInt32(type.Id),type);
                }
            }
        }

        //获取表SensorType中的数据
        public List<SensorType> GetAllSensorType()
        {
            return sensorTypeInfos.Values.ToList();
        }
    }
}