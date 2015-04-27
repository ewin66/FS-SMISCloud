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
namespace FSDE.DAL.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FSDE.IDAL;
    using FSDE.Model.Config;

    using SqliteORM;

    public class SensorInfoDal : ISensorInfoDal
    {
        public int AddSensorInfo(SensorInfo sensorInfo)
        {
            using (DbConnection conn = new DbConnection())
            {
               return sensorInfo.Save();
            }
        }

        public bool UpdateSensorInfo(SensorInfo sensorInfo)
        {
            using (DbConnection conn = new DbConnection())
            {
                if (sensorInfo.Save() > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool Delete(int id)
        {
            using (DbConnection conn = new DbConnection())
            {
                try
                {
                    SensorInfo.Delete(Where.Equal("ID", id));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool DeleteSensorInfos(int startId,int endId)
        {
            using (DbConnection conn = new DbConnection())
            {
                try
                {
                    SensorInfo.Delete(Where.And(Where.GreaterOrEqual("ID", startId), Where.LessOrEqual("ID", endId)));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public IList<SensorInfo> SelectList()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<SensorInfo> adapter = TableAdapter<SensorInfo>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }
    }
}