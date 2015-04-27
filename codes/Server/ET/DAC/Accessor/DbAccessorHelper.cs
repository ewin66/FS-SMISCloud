using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Accessor.MSSQL;

namespace FS.SMIS_Cloud.DAC.Accessor
{
    public class DbAccessorHelper
    {

        public static IDbAccessor DbAccessor { get; private set; }
        public static ISaveAttask AtDbAccessor = null;

        public static void Init(IDbAccessor accessor)
        {
            DbAccessor = accessor;
            AtDbAccessor = accessor as ISaveAttask;
        }

        //public static void Init(DbAccessorType type, string connStr, string BaseDir = "")
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory;
        //    string assembly = GetDACStorageAssembly();
        //    if (string.IsNullOrEmpty(assembly)) throw new Exception(" consumers.xml has not DAC.Storage ");
        //    Assembly asm = Assembly.LoadFile(path + "\\" + assembly);
        //    Type[] types = asm.GetTypes();
        //    var helperlst = types.Where(t => new List<Type>(t.GetInterfaces()).Contains(typeof(ISqlHelper))).ToList();
        //    var dbAccessorlst = types.Where(t => new List<Type>(t.GetInterfaces()).Contains(typeof(IDbAccessor))).ToList();
           
        //    if (type == DbAccessorType.MSSQL)
        //    {
        //        CfgSqlHelper = SqlHelper = ObjectUtils.InstantiateType<ISqlHelper>(helperlst[0],new []{typeof(string)}, new object[]{connStr});
        //        var msDbAccessor = ObjectUtils.InstantiateType<IDbAccessor>(dbAccessorlst[0], new[] { typeof(ISqlHelper) }, new object[] { SqlHelper });
                 
        //        DbAccessor = msDbAccessor;
        //        AtDbAccessor = new MsDbAccessor(SqlHelper); 
        //    }
        //    else if (type == DbAccessorType.Access)
        //    {
        //        CfgSqlHelper = SqlHelper = ObjectUtils.InstantiateType<ISqlHelper>(helperlst[0], new[] { typeof(string) }, new object[] { connStr });
        //    }
        //    else
        //    {
        //        // 测试时故障.
        //        string pwd = BaseDir ?? (AppDomain.CurrentDomain.BaseDirectory + "\\");
        //        // System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        //        // @"Data Source=.\\FSUSDB\\FSUSConfigDB.db3;Version=3;Pooling=False;Max Pool Size=100";
        //        // connStr: 逗号分割的2个库文件: 配置/数据, 譬如: ".\\FSUSDB\\fsuscfg.db3,.\\FSUSDB\\FSUSDataValueDB.db3"
        //        string[] dbs = connStr.Split(',');
        //        string connCfg = string.Format(@"Data Source={0}{1};Version=3;Pooling=False;Max Pool Size=100",
        //            pwd, dbs[0]);
        //        string connVal = string.Format(@"Data Source={0}{1};Version=3;Pooling=False;Max Pool Size=100",
        //            pwd, dbs[1]);
        //        var cfgHelper = ObjectUtils.InstantiateType<ISqlHelper>(helperlst[0],new []{typeof(string)}, new object[]{connCfg});
        //        var valHelper = ObjectUtils.InstantiateType<ISqlHelper>(helperlst[0], new[] { typeof(string) }, new object[] { connVal });
        //        DbAccessor = ObjectUtils.InstantiateType<IDbAccessor>(dbAccessorlst[0],
        //            new[] {typeof (ISqlHelper), typeof (ISqlHelper)}, new object[] {cfgHelper, valHelper});
                
        //        SqlHelper = valHelper;
        //        CfgSqlHelper = cfgHelper;
        //    }
        //}

        //private static string GetDACStorageAssembly()
        //{
        //    string filefullpath = AppDomain.CurrentDomain.BaseDirectory + " \\consumers.xml";
        //    if (System.IO.File.Exists(filefullpath))
        //    {
        //        var doc = XDocument.Load(filefullpath);
        //        if (doc.Root != null)
        //        {
        //            var xElement = doc.Root.Element("consumers");
        //            if (xElement != null)
        //            {
        //                var consumerNodes = xElement.Elements();
        //                foreach (
        //                    var assembly in
        //                        from node in consumerNodes
        //                        where node.Attribute("name").Value == "DAC.Storage"
        //                        select node.Attribute("assembly").Value)
        //                {
        //                    return assembly;
        //                }
        //            }
        //        }

        //    }
        //    return string.Empty;
        //}

    }
}
