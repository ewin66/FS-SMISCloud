using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Data.Controllers
{
    public class StressStrainDataController : ApiController
    {
        /// <summary>
        /// 获取杆件组合下所有传感器
        /// </summary>
        /// <param name="messageId">消息id</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取杆件组合下所有传感器", false)]
        public object GetCombinedSensors(int sensorId)
        {
            // 
            using (var entity = new SecureCloud_Entities())
            {
                var query = from s in entity.T_DIM_SENSOR_CORRENT
                            from ss in entity.T_DIM_SENSOR
                            where s.SensorId == sensorId && ss.SENSOR_ID==s.CorrentSensorId
                            select new {
                                s.CorrentSensorId,
                                ss.SENSOR_LOCATION_DESCRIPTION
                            };
                
                var list = query.ToList();

                return list;


            }
        }

    }
}
