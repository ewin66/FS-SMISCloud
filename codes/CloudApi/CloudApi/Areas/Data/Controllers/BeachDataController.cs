using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Data.Controllers
{
    using System.Configuration;

    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

    public class BeachDataController : ApiController
    {
        /// <summary>
        /// 获取干滩最新数据
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取干滩最新数据",false)]
        public IEnumerable<BeachData> GetLastData(int structId)
        {
            int beachFactorId = int.Parse(ConfigurationManager.AppSettings["BeachFactorId"]);

            using (var db = new SecureCloud_Entities())
            {
                var query = from b in db.T_THEMES_ENVI_BEACH
                            where b.ID == (from b2 in db.T_THEMES_ENVI_BEACH                                           
                                           where b2.SENSOR_ID == (
                                                from s in db.T_DIM_SENSOR
                                                where !s.IsDeleted && s.STRUCT_ID == structId
                                                && s.SAFETY_FACTOR_TYPE_ID == beachFactorId
                                                select s.SENSOR_ID
                                           ).Max()
                                           select b2.ID).Max()
                            select new BeachData { Length = b.BEACH_LENGTH ?? 0, WaterLevel = b.WATER_LEVEL ?? 0 };

                return query.ToList();
            }
        }
    }

    public class BeachData
    {
        public decimal Length { get; set; }

        public decimal WaterLevel { get; set; }
    }
}
