using System.Linq;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Log.Controllers
{
    using System.Data.Objects.SqlClient;

    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    public class SysLogController : ApiController
    {
        /// <summary>
        /// syslog
        /// </summary>        
        /// <param name="keyWords">内容关键字</param>
        /// <param name="startRow">开始行数</param>
        /// <param name="endRow">结束行数</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取系统日志", false)]
        [Authorization(AuthorizationCode.S_SysLog)]
        public object GetSysLog(string keyWords = null, int? startRow = null, int? endRow = null)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from l in db.T_SYSTEM_LOG select l;

                if (keyWords != null)
                {
                    keyWords = keyWords.ToLower();
                    query =
                        query.Where(
                            l =>
                            l.ProcessName.ToLower().Contains(keyWords) 
                            || l.Level.ToLower().Contains(keyWords)
                            || l.FileName.ToLower().Contains(keyWords)
                            || l.Message.ToLower().Contains(keyWords)
                            || l.Exception.ToLower().Contains(keyWords)
                            || SqlFunctions.StringConvert((double)l.RecordTime.Year).Contains(keyWords)
                            || SqlFunctions.StringConvert((double)l.RecordTime.Month).Contains(keyWords)
                            || SqlFunctions.StringConvert((double)l.RecordTime.Day).Contains(keyWords));
                }

                query = query.OrderByDescending(l => l.RecordTime);

                if (startRow != null && endRow != null)
                {
                    var skip = (int)startRow - 1;
                    var len = (int)endRow - (int)startRow + 1;
                    query = query.Skip(skip).Take(len);
                }

                return
                    query.ToList()
                        .Select(
                            l =>
                            new
                                {
                                    id = l.Id,
                                    logTime = l.RecordTime,
                                    logLevel = l.Level,
                                    processName = l.ProcessName,
                                    fileName = l.FileName,
                                    lineNo = l.LineNo,
                                    message = l.Message,
                                    exception = l.Exception ?? string.Empty
                                });
            }
        }

        /// <summary>
        /// syslog-count
        /// </summary>        
        /// <param name="keyWords">关键字</param>
        /// <returns>日志数量</returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取系统日志数量", false)]
        [Authorization(AuthorizationCode.S_SysLog)]
        public object GetSysLogCount(string keyWords = null)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from l in db.T_SYSTEM_LOG select l;

                if (keyWords != null)
                {
                    keyWords = keyWords.ToLower();
                    query =
                        query.Where(
                            l =>
                            l.ProcessName.ToLower().Contains(keyWords)
                            || l.Level.ToLower().Contains(keyWords)
                            || l.FileName.ToLower().Contains(keyWords)
                            || l.Message.ToLower().Contains(keyWords)
                            || l.Exception.ToLower().Contains(keyWords)
                            || SqlFunctions.StringConvert((double)l.RecordTime.Year).Contains(keyWords)
                            || SqlFunctions.StringConvert((double)l.RecordTime.Month).Contains(keyWords)
                            || SqlFunctions.StringConvert((double)l.RecordTime.Day).Contains(keyWords));
                }

                var count = query.Count();

                return new { count };
            }
        }
    }
}
