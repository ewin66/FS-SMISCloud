using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;


namespace Agg.Storage
{
    using System.Configuration;

    using log4net;

    public class SeclureCloudDbHelper
    {
        private volatile static SeclureCloudDbHelper _instance = null;
        private static readonly object lockHelper = new object();
        //private static string FilePath = AppDomain.CurrentDomain.BaseDirectory + " \\iSecureCloudConfig.xml";
        private static readonly ILog Log = LogManager.GetLogger("SeclureCloudDbHelper");
        private static string ConnectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        //private Timer timer = new Timer();

        public MsDbAccessor Accessor;

        //private DbConfigXml DbConfig; 
        private SeclureCloudDbHelper()
        {
            //DbConfig = new DbConfigXml(FilePath);
            Accessor = new MsDbAccessor(ConnectionString);
            //DbConfig.UpdateThemeTableInfo(Accessor.ThemeTableInfos);
            //StartTimer(); // todo：定期检查XML配置，更新主题表配置
        }

        //private void StartTimer()
        //{
        //    timer.Interval = 30 * 60 * 1000;  // 30分钟
        //    timer.Elapsed += TimerOnElapsed;
        //    timer.Start();
        //}

        //private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        //{
        //    if (DbConfig.IsFileUpdate())
        //    {
        //        //Accessor.UpdateTables(DbConfig.GeTableInfos());
        //    }
        //}

        public static SeclureCloudDbHelper Instance()
        {
            if(_instance == null)
            {
                lock(lockHelper)
                {
                    if (_instance == null)
                    {
                        try
                        {
                            _instance = new SeclureCloudDbHelper();
                        }
                        catch (Exception)
                        {
                            Log.ErrorFormat("SeclureCloudDbHelper初始化失败");
                        }
                    }
                    
                }
            }
            return _instance;
        }
    }
}
