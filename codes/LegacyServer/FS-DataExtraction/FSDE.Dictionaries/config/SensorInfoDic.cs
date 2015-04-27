#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorInfoDic.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System;

namespace FSDE.Dictionaries.config
{
    using System.Collections.Generic;
    using System.Linq;

    using FSDE.BLL.Config;
    using FSDE.Model.Config;

    public class SensorInfoDic
    {
        private Dictionary<int, SensorInfo> sensorInfos;

        private static SensorInfoDic sensorInfoDic=new SensorInfoDic();

        public static SensorInfoDic GetSensorInfoDic()
        {
            return sensorInfoDic;
        }

        public List<SensorInfo> GetDicValues()
        {
            return sensorInfos.Values.ToList();
        }

        public Dictionary<int, SensorInfo> GetSensorInfos()
        {
            return sensorInfos;
        }

        private SensorInfoDic()
        {
            if (null == this.sensorInfos)
            {
                this.sensorInfos=new Dictionary<int, SensorInfo>();
                var bll = new SensorInfoBll();
                IList<SensorInfo> list = bll.SelectList();
                foreach (SensorInfo info in list)
                {
                    sensorInfos.Add(Convert.ToInt32(info.ID),info);
                }
            }
        }

        public bool Add(SensorInfo sensorInfo)
        {
            var bll = new SensorInfoBll();
            int id = bll.AddSensorInfo(sensorInfo);
            if (id>0)
            {
                sensorInfo.ID = id;
                sensorInfoDic.sensorInfos.Add(id, sensorInfo);
                return true;
            }
               return false;
        }

        public bool CheckAdd(SensorInfo sensorInfo)
        {
            bool flag = false;
            int groupId = 0;
            List<SensorInfo> sensorInfosList = sensorInfos .Values.ToList();
            for (int i = 0; i < sensorInfosList.Count; i++)
            {
                if (sensorInfosList[i].ChannelId == sensorInfo.ChannelId
                    && sensorInfosList[i].ModuleNo == sensorInfo.ModuleNo
                    && sensorInfosList[i].Safetyfactortypeid == sensorInfo.Safetyfactortypeid
                    && sensorInfosList[i].DataBaseId == sensorInfo.DataBaseId)
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                var bll = new SensorInfoBll();
                int id = bll.AddSensorInfo(sensorInfo);
                if (id > 0)
                {
                    sensorInfo.ID = id;
                    sensorInfoDic.sensorInfos.Add(id, sensorInfo);
                    return true;
                }
            }
            return false;
        }

        public bool UpdateSensorInfo(SensorInfo sensorInfo)
        {
            var bll = new SensorInfoBll();

            if (bll.UpdateSensorInfo(sensorInfo))
            {
                sensorInfoDic.sensorInfos[(int)sensorInfo.ID] = sensorInfo;
                return true;
            }
            return false;
        }

        public bool Delete(int id)
        {
           var bll = new SensorInfoBll();
            return bll.Delete(id);
        }

        public bool DeleteSensorInfos(int startId, int endId)
        {
            var bll = new SensorInfoBll();
            return bll.DeleteSensorInfos(startId,endId);
        }

        public IList<SensorInfo> SelectList()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 获取一个传感器实体
        /// </summary>
        /// <param name="dataBaseId">
        /// 所属数据库ID
        /// </param>
        /// <param name="moduleNo">
        /// 模块号
        /// </param>
        /// <param name="channelId">
        /// 通道号
        /// </param>
        /// <returns>
        /// 查询到的传感器实体
        /// </returns>
        public SensorInfo GetSensorInfo(int dataBaseId, string moduleNo, int channelId)
        {
            foreach (SensorInfo sensorInfo in this.sensorInfos.Values.Where(sensorInfo => sensorInfo.DataBaseId == dataBaseId && sensorInfo.ModuleNo == moduleNo
                                                                                     && sensorInfo.ChannelId == channelId))
            {
                return sensorInfo;
            }

            return new SensorInfo();
        }

        
    }
}