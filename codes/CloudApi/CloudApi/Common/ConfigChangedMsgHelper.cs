#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="ConfigChangedMsgHelper.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141105 by LINGWENLONG .
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
using FreeSun.FS_SMISCloud.Server.CloudApi.Entity.Config;
using FS.Service;
using Newtonsoft.Json;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Common
{
    public class ConfigChangedMsgHelper
    {
        
        public static FsMessage GetSensorConfigChangedMsg(ChangedStatus changedStatus,Sensor sensor,int dtuid=0)
        {
            var msg = new FsMessage
            {
                Header = new FsMessageHeader
                {
                    A = "PUT",
                    R = "/et/config/sensor/",
                    U = Guid.NewGuid(),
                    T = Guid.NewGuid(),
                    D = "et",
                    M = "config"
                },
                Body = JsonConvert.SerializeObject(sensor)
            };

            switch (changedStatus)
            {
                case ChangedStatus.Add:
                    msg.Header.R = "/et/config/sensor/add";
                    break;
                case ChangedStatus.Modify:
                    msg.Header.M = "previous:" + dtuid;
                    msg.Header.R = "/et/config/sensor/mod";
                    break;
                case ChangedStatus.Delete:
                    msg.Header.R = "/et/config/sensor/del";
                    break;
            }

            return msg;
        }

        public static FsMessage GetSensorConfigChangedMsg(SensorOperation senopera)
        {
            var msg = new FsMessage
            {
                Header = new FsMessageHeader
                {
                    A = "PUT",
                    R = "/et/config/sensor/",
                    U = Guid.NewGuid(),
                    T = Guid.NewGuid(),
                    D = "et",
                    M = "config"
                },
                Body = JsonConvert.SerializeObject(senopera)
            };

            return msg;
        }

        public static FsMessage GetDtuConfigChangedMsg(ChangedStatus changedStatus, DtuNode dtu,string dtucode=null)
        {
            var msg = new FsMessage
            {
                Header = new FsMessageHeader
                {
                    A = "PUT",
                    R = "/et/config/dtu/",
                    U = Guid.NewGuid(),
                    T = Guid.NewGuid(),
                    D = "et",
                    M = "config"
                },
                Body = JsonConvert.SerializeObject(dtu)
            };
            switch (changedStatus)
            {
                case ChangedStatus.Add:
                    msg.Header.R = "/et/config/dtu/add";
                    break;
                case ChangedStatus.Modify:
                    msg.Header.M = "previous:" + dtucode;
                    msg.Header.R = "/et/config/dtu/mod";
                    break;
                case ChangedStatus.Delete:
                    msg.Header.R = "/et/config/dtu/del";
                    break;
            }
            return msg;
        }
        public static FsMessage GetAggConfigChangedMsg()
        {
            var msg = new FsMessage
            {
                Header = new FsMessageHeader
                {
                    A = "PUT",
                    R = "/agg/config/",
                    U = Guid.NewGuid(),
                    T = Guid.NewGuid(),
                    D = "agg",
                    M = "config"
                },
                
            };
           
            return msg;
        }


        public static FsMessage GetSensorGroupMessage()
        {
            var msg = new FsMessage
            {
                Header = new FsMessageHeader
                {
                    A = "PUT",
                    R = "/et/config/sengroup",
                    U = Guid.NewGuid(),
                    T = Guid.NewGuid(),
                    D = "et",
                    M = "config"
                },
                Body = JsonConvert.SerializeObject(null)
            };

            return msg;
        }

    }
}