using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;
using Newtonsoft.Json.Linq;
using log4net;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers
{
    public class SimpleSensor : MarshalByRefObject
    {
        public int sensorid { get; set; }
        public string location { get; set; }
        public string sensortype { get; set; }
    }

    public class FactorSensorController : ApiController
    {
        static readonly ILog Log = LogManager.GetLogger("FactorSensorController");

        private static void Sort<T>(ref List<T> list, int structId, int factorId)
        {
            AppDomain appDomain = AppDomain.CreateDomain("DynamicCompiler-" + Guid.NewGuid().ToString(), null,
                AppDomain.CurrentDomain.SetupInformation);
            try
            {
                ListSortHelper<T> sort =
                    (ListSortHelper<T>)
                        appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                            typeof(ListSortHelper<T>).FullName);
                const string directory = @".\ExtScripts";
                var script = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    string.Format(directory + @"\Sort_{0}_{1}.cs", structId, factorId));
                sort.LoadSortScript(script);
                sort.Sort(ref list);
                Log.Error("Sorted");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            finally
            {
                Log.Debug("Unloading AppDomain");
                AppDomain.Unload(appDomain);
            }
        }

        /// <summary>
        /// 根据结构体和因素获取监测点 struct/{structid}/factor/{factorid}/sensors
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <param name="factorId"> 因素编号 </param>
        /// <returns>
        ///     监测点列表 <see cref="object" />.
        /// </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物监测因素的传感器", false)]
        public object FindSensorsByStructAndFactor(int structId, int factorId)
        {
            var baseStation = ConfigurationManager.AppSettings["GPSBaseStation"];
            using (var entities = new SecureCloud_Entities())
            {
                var query = from s in entities.T_DIM_SENSOR
                            from f in entities.T_DIM_SAFETY_FACTOR_TYPE
                            from p in entities.T_DIM_SENSOR_PRODUCT
                            where s.PRODUCT_SENSOR_ID == p.PRODUCT_ID
                                  && s.STRUCT_ID == structId
                                  && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                                  && f.SAFETY_FACTOR_TYPE_ID == factorId
                                  && p.PRODUCT_NAME != baseStation
                                  && s.IsDeleted == false
                                  && s.Identification != 1
                            select new SimpleSensor
                                    {
                                        sensorid = s.SENSOR_ID,
                                        location = s.SENSOR_LOCATION_DESCRIPTION,
                                        sensortype = p.PRODUCT_NAME
                                    };
                var list = query.ToList();
                try
                {
                    Sort(ref list, structId, factorId); //自定义排序规则
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                    list.OrderBy(l => l.sensorid);
                }
                return new JArray(list.GroupBy(o => o.sensortype).Select(o =>
                    new JObject(
                        new JProperty("sensorType", o.Key),
                        new JProperty(
                            "sensors",
                            new JArray(o.Select(sensor =>
                                new JObject(
                                    new JProperty("sensorId", sensor.sensorid),
                                    new JProperty("location", sensor.location))))))));
            }
        }

        /// <summary>
        /// 根据结构体和因素获取非虚拟监测点 struct/{structid}/factor/{factorid}/non-virtual/sensors
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <param name="factorId"> 因素编号 </param>
        /// <returns> 监测点列表 <see cref="object"/>.
        /// </returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物监测因素的非虚拟传感器", false)]
        [Authorization(AuthorizationCode.S_DataOriginal)]
        public object FindNonVirtualSensorsByStructAndFactor(int structId, int factorId)
        {
            string baseStation = ConfigurationManager.AppSettings["GPSBaseStation"];
            using (SecureCloud_Entities entities = new SecureCloud_Entities())
            {
                var query = from s in entities.T_DIM_SENSOR
                            from f in entities.T_DIM_SAFETY_FACTOR_TYPE
                            from p in entities.T_DIM_SENSOR_PRODUCT
                            where s.PRODUCT_SENSOR_ID == p.PRODUCT_ID
                                  && s.STRUCT_ID == structId
                                  && s.SAFETY_FACTOR_TYPE_ID == f.SAFETY_FACTOR_TYPE_ID
                                  && f.SAFETY_FACTOR_TYPE_ID == factorId
                                  && p.PRODUCT_NAME != baseStation
                                  && s.IsDeleted == false
                                  && s.Identification != 2
                            select new
                            {
                                sensorid = s.SENSOR_ID,
                                location = s.SENSOR_LOCATION_DESCRIPTION,
                                sensortype = p.PRODUCT_NAME
                            };
                var list = query.ToList().OrderBy(l => l.sensorid);
                return new JArray(list.GroupBy(o => o.sensortype).Select(o =>
                    new JObject(
                        new JProperty("sensorType", o.Key),
                        new JProperty(
                            "sensors",
                            new JArray(o.Select(sensor =>
                                new JObject(
                                    new JProperty("sensorId", sensor.sensorid),
                                    new JProperty("location", sensor.location))))))));
            }
        }

        /// <summary>
        /// struct/{structid}/factor/deep-displace/sensors
        /// </summary>
        /// <param name="structId">结构物编号（只能是数字）</param>
        /// <returns>传感器组列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物内部位移分组", false)]
        public object FindDeepDisplaceSensorsByStruct(int structId)
        {
            var factorId = int.Parse(ConfigurationManager.AppSettings["DeepDisplaceFactorId"]);

            using (var entities = new SecureCloud_Entities())
            {
                var query = from s in entities.T_DIM_SENSOR
                            from f in entities.T_DIM_SAFETY_FACTOR_TYPE
                            from sg in entities.T_DIM_SENSOR_GROUP_CEXIE
                            from g in entities.T_DIM_GROUP
                            where s.STRUCT_ID == structId
                                  && s.SAFETY_FACTOR_TYPE_ID == factorId
                                  && sg.SENSOR_ID == s.SENSOR_ID
                                  && g.GROUP_ID == sg.GROUP_ID
                                  && s.IsDeleted == false
                                  && s.Identification != 1
                            select new
                            {
                                groupid = g.GROUP_ID,
                                groupname = g.GROUP_NAME,
                                depth = sg.DEPTH
                            };
                var list = (from q in query
                            group q by new { q.groupid, q.groupname }
                                into g
                                select new
                                {
                                    g.Key.groupid,
                                    g.Key.groupname,
                                    maxdepth = g.Min(o => o.depth)
                                }).ToList();

                return new JArray(list.Select(g =>
                new JObject(
                    new JProperty("groupId", g.groupid),
                    new JProperty("groupName", g.groupname),
                    new JProperty("maxDepth", g.maxdepth))));
            }
        }

        /// <summary>
        ///     根据结构体和因素获取监测点分组 struct/{structId}/factor/{factorId}/groups
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <param name="factorId">因素编号</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取结构物监测因素的分组", false)]
        public object FindGroupsByStructAndFactor(int structId, int factorId)
        {
            using (var entities = new SecureCloud_Entities())
            {
                var query = from s in entities.T_DIM_SENSOR
                            from f in entities.T_DIM_SAFETY_FACTOR_TYPE
                            from sg in entities.T_DIM_SENSOR_GROUP_CEXIE
                            from g in entities.T_DIM_GROUP
                            where s.STRUCT_ID == structId
                                  && s.SAFETY_FACTOR_TYPE_ID == factorId
                                  && sg.SENSOR_ID == s.SENSOR_ID
                                  && g.GROUP_ID == sg.GROUP_ID
                                  && s.IsDeleted == false
                                  && s.Identification != 1
                            select new
                            {
                                groupid = g.GROUP_ID,
                                groupname = g.GROUP_NAME
                            };

                var list = query.GroupBy(g => new { g.groupid, g.groupname }).Select(g => g).ToList();

                return new JArray(list.Select(g =>
                    new JObject(
                        new JProperty("groupId", g.Key.groupid),
                        new JProperty("groupName", g.Key.groupname))));
            }
        }

        /// <summary>
        ///     获取分组中的传感器// TODO:修改已弃用
        /// </summary>
        /// <param name="groupId">分组编号</param>
        /// <returns>传感器列表</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取分组中的传感器", false)]
        public object FindSensorsByGroup(int groupId)
        {
            using (var entity = new SecureCloud_Entities())
            {
                var query = from sg in entity.T_DIM_SENSOR_GROUP_CEXIE
                            from s in entity.T_DIM_SENSOR
                            where
                                sg.GROUP_ID == groupId && sg.SENSOR_ID == s.SENSOR_ID && s.IsDeleted == false &&
                                s.Identification != 1
                            orderby sg.DEPTH
                            select new
                                       {
                                           s.SENSOR_ID,
                                           s.SENSOR_LOCATION_DESCRIPTION,
                                           sg.DEPTH
                                       };
                return new JObject(
                    new JProperty("groupId", groupId),
                    new JProperty(
                        "sensors",
                        query.ToList()
                            .Select(
                                s =>
                                new JObject(
                                    new JProperty("sensorId", s.SENSOR_ID),
                                    new JProperty("depth", s.DEPTH),
                                    new JProperty("location", s.SENSOR_LOCATION_DESCRIPTION)))));
            }
        }
    }
}
