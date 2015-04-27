#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorInfoDal.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140526 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.BLL.Config
{
    using System;
    using System.Collections.Generic;

    using FSDE.DALFactory;
    using FSDE.IDAL;
    using FSDE.Model.Config;

    public class SensorInfoBll
    {
        private readonly ISensorInfoDal Dal = DataAccess.CreateSensorInfoDal();
        public int AddSensorInfo(SensorInfo sensorInfo)
        {
            return Dal.AddSensorInfo(sensorInfo);
        }

        public bool UpdateSensorInfo(SensorInfo sensorInfo)
        {
            return Dal.UpdateSensorInfo(sensorInfo);
        }

        public bool Delete(int id)
        {
            return Dal.Delete(id);
        }

        public bool DeleteSensorInfos(int startId,int endId)
        {
            return Dal.DeleteSensorInfos(startId, endId);
        }

        public IList<SensorInfo> SelectList()
        {
            return Dal.SelectList();
        }
    }
}