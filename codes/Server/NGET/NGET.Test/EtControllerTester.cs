// --------------------------------------------------------------------------------------------
// <copyright file="EtControllerTester.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2015 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20150330
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace NGET.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    using FS.DbHelper;
    using FS.SMIS_Cloud.NGET;
    using FS.SMIS_Cloud.NGET.DataParser;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using DbType = FS.DbHelper.DbType;

    [TestFixture]
    public class EtControllerTester
    {
        [Test]
        [Category("MAMUAL")]
        public void TestFileParseWork()
        {
            log4net.Config.XmlConfigurator.Configure();

            string connStr = "server=.;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            var sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, connStr);
            string path = "F:\\ET\\datas\\";
            // query sensors
            string delSql = @"DELETE FROM [T_DATA_ORIGINAL]";
            sqlHelper.ExecuteSql(delSql);

            string querySql = @"
select s.SENSOR_ID, f.FACTOR_VALUE_COLUMN_NUMBER
from T_DIM_SENSOR s
left join T_DIM_SAFETY_FACTOR_TYPE f on s.SAFETY_FACTOR_TYPE_ID=f.SAFETY_FACTOR_TYPE_ID
where s.IsDeleted=0";
            var dt = sqlHelper.Query(querySql).Tables[0];
            Console.WriteLine("{0} sensors", dt.Rows.Count);
            // create file
            Console.WriteLine("begin to create files");
            Random ran = new Random();
            for (int i = 0; i < dt.Rows.Count; i += 50)
            {
                var fs = new FileStream(path + (i / 50) + ".json", FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);

                for (int j = i; j < i + 50; j++)
                {
                    if (j >= dt.Rows.Count) break;

                    var number = Convert.ToInt32(dt.Rows[j][1]);
                    List<double> d = new List<double>();
                    for (int k = 0; k < number; k++)
                    {
                        d.Add(ran.NextDouble());
                    }
                    var data = new JsonData
                                   {
                                       S = Convert.ToUInt32(dt.Rows[j][0]),
                                       R = 0,
                                       N = 0,
                                       T =
                                           (DateTime.Now - new DateTime(2000, 1, 1))
                                           .TotalMilliseconds,
                                       A = new string[0],
                                       RV = d.ToArray(),
                                       PV = d.ToArray()
                                   };
                    var json = JsonConvert.SerializeObject(data);
                    sw.WriteLine(json);
                }

                sw.Close();
                sw.Dispose();
                fs.Close();
                fs.Close();
            }
            Console.WriteLine("creating files finished");

            // parse
            GlobalConfig.ConnectionString = connStr;
            GlobalConfig.DataSourcePath = path;
            GlobalConfig.ErrorFilePath = "F:\\ET\\error\\";
            GlobalConfig.ParsedFilePath = "F:\\ET\\parsed\\";
            EtController ctrl = new EtController(null);
            Stopwatch s = new Stopwatch();
            s.Start();
            ctrl.DoFileParseWork();
            s.Stop();
            Console.WriteLine(s.Elapsed.TotalSeconds);
        }
    }
}