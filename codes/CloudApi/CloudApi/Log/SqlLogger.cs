namespace FreeSun.FS_SMISCloud.Server.CloudApi.Log
{
    using System.Data.SqlClient;
    using System.Reflection;
    using System.Threading.Tasks;

    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;

    using log4net;

    /// <summary>
    /// SQL Server日志类
    /// </summary>
    public class SqlLogger
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 写入sql日志
        /// </summary>
        /// <param name="info">日志对象</param>
        public void WriteLogAsyn(LoggingObject info)
        {
            Task task = new Task(
                () =>
                {
                    string sqlString = 
                        @"insert into T_API_LOG(LogTime,ClientIP,UserAgent,ClientType,Url,Method,Parameter,ParameterShow,
                                Content,Controller,Action,StatusCode,Duration,UserNo,SessionId,IsVisible)
                          values(@LogTime,@ClientIP,@UserAgent,@ClientType,@Url,@Method,@Parameter,@ParameterShow,
                                @Content,@Controller,@Action,@StatusCode,@Duration,@UserNo,@SessionId,@IsVisible)";
                    SqlParameter[] paras =
                    {
                        new SqlParameter("@LogTime",info.LogTime),
                        new SqlParameter("@ClientIP", info.ClientIp),
                        new SqlParameter("@UserAgent", info.UserAgent),
                        new SqlParameter("@ClientType", info.ClientType),
                        new SqlParameter("@Url", info.Url),
                        new SqlParameter("@Method", info.Method),
                        new SqlParameter("@Parameter", info.Parameter),
                        new SqlParameter("@ParameterShow", info.ParameterShow),
                        new SqlParameter("@Content", info.Content),
                        new SqlParameter("@Controller", info.Controller),
                        new SqlParameter("@Action", info.Action),
                        new SqlParameter("@StatusCode", info.StatusCode),
                        new SqlParameter("@Duration", info.Duration),
                        new SqlParameter("@UserNo", info.UserNo),
                        new SqlParameter("@SessionId", info.SessionId),
                        new SqlParameter("@IsVisible", info.IsVisible)
                    };
                    SqlHelper.ExecteNonQuery(System.Data.CommandType.Text, sqlString, paras);
                });
            task.Start();
            task.ContinueWith(
                t =>
                    {
                        if (t.Exception != null)
                        {
                            log.Error("日志记录失败:" + info.ToString(), t.Exception);
                        }
                    });
        }
    }
}