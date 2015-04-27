using System;
using System.Web.Http;
using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
using Newtonsoft.Json;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Log.Controllers
{
    using System.Data.Objects.SqlClient;
    using System.Linq;

    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    public class LogController : ApiController
    {       
        /// <summary>
        /// log/{userid}/{startRow}/{endRow}
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="keyWords">内容关键字</param>
        /// <param name="startRow">开始行数</param>
        /// <param name="endRow">结束行数</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取用户日志", false)]
        [Authorization(AuthorizationCode.S_UserLog)]
        [Authorization(AuthorizationCode.U_Common)]
        public object GetLogByUserId(int userId, string keyWords = null, int? startRow = null, int? endRow = null)
        {          
            using (var db = new SecureCloud_Entities())
            {
                var query = from l in db.T_API_LOG
                            where l.UserNo == userId && l.IsVisible                            
                            select
                                new LogInfo
                                    {
                                        LogTime = l.LogTime,
                                        ClientType = l.ClientType,
                                        Content = l.Content,
                                        Parameter = l.ParameterShow
                                    };

                if (keyWords != null)
                {
                    keyWords = keyWords.ToLower();
                    query =
                        query.Where(
                            l =>
                            l.Content.ToLower().Contains(keyWords) || l.ClientType.ToLower().Contains(keyWords)
                            || SqlFunctions.StringConvert((double)l.LogTime.Year).Contains(keyWords)
                            || SqlFunctions.StringConvert((double)l.LogTime.Month).Contains(keyWords)
                            || SqlFunctions.StringConvert((double)l.LogTime.Day).Contains(keyWords));
                }

                query = query.OrderByDescending(l => l.LogTime);

                if (startRow != null && endRow != null)
                {
                    var skip = (int)startRow - 1;
                    var len = (int)endRow - (int)startRow + 1;
                    query = query.Skip(skip).Take(len);
                }

                return query.ToList();
            }
        }

        /// <summary>
        /// log-count/{userId}
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="keyWords">关键字</param>
        /// <returns>日志数量</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取用户日志数量", false)]
        [Authorization(AuthorizationCode.S_UserLog)]
        [Authorization(AuthorizationCode.U_Common)]
        public object GetLogCountById(int userId, string keyWords = null)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = db.T_API_LOG.Where(l => l.UserNo == userId && l.IsVisible);

                if (keyWords != null)
                {
                    keyWords = keyWords.ToLower();
                    query =
                        query.Where(
                            l =>
                            l.Content.ToLower().Contains(keyWords) || l.ClientType.ToLower().Contains(keyWords)
                            || SqlFunctions.StringConvert((double)l.LogTime.Year).Contains(keyWords)
                            || SqlFunctions.StringConvert((double)l.LogTime.Month).Contains(keyWords)
                            || SqlFunctions.StringConvert((double)l.LogTime.Day).Contains(keyWords));
                }

                var count = query.Count();

                return new Data { Count = count };
            }
        }
    }

    public class LogInfo
    {
        [JsonProperty("logTime")]
        public DateTime LogTime { get; set; }

        [JsonProperty("clientType")]
        public string ClientType { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("parameter")]
        public string Parameter { get; set; }


    }

    public class Data
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }

}
