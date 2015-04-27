#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="Common.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140526 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.DAL
{
    using System.Configuration;
    using System.Reflection;

    using SqliteORM;

    public class Common
    {
        public static string connectionString;
        static Common()
        {
            var file = new ExeConfigurationFileMap { ExeConfigFilename = @".\config\Params.config" };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            connectionString = config.AppSettings.Settings["ConnectionStringFixed"].Value;
        }

        public static void Initialise()
        {
             DbConnection.Initialise(connectionString, Assembly.Load("FSDE.Model"));
        }
    }
}