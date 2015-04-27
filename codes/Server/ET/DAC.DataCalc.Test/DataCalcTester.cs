using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using FS.DbHelper;
using NUnit.Framework;

namespace DAC.DataCalc.Test
{
    [TestFixture]
    class DataCalcTester
    {
        private static readonly string Connstr = ConfigurationManager.AppSettings["SecureCloud"];
        private static readonly ISqlHelper Dbhelper = SqlHelperFactory.Create(DbType.MSSQL, Connstr);

        [SetUp]
        public void SetUp()
        {
            // TODO whole process test
            /*List<String> sqls=new List<string>();
            // virtual laser settlement
            sqls.Add(@"insert into [T_DIM_SENSOR] ([SENSOR_ID],[PRODUCT_SENSOR_ID],[DAI_CHANNEL_NUMBER],[IsDeleted],[SAFETY_FACTOR_TYPE_ID],[DTU_ID],[Identification])
                       values 10000,1,1,0,40,147,2");
            sqls.Add(@"insert into [T_DIM_SENSOR] ([SENSOR_ID],[PRODUCT_SENSOR_ID],[DAI_CHANNEL_NUMBER],[IsDeleted],[SAFETY_FACTOR_TYPE_ID],[DTU_ID],[Identification])
                       values 10000,1,1,0,40,147,2");
            sqls.Add(@"insert into [T_DIM_SENSOR] ([SENSOR_ID],[PRODUCT_SENSOR_ID],[DAI_CHANNEL_NUMBER],[IsDeleted],[SAFETY_FACTOR_TYPE_ID],[DTU_ID],[Identification])
                       values 10000,1,1,0,40,147,2");
            // deepdisp inclination
            sqls.Add(@"insert into [T_DIM_SENSOR] ([SENSOR_ID],[PRODUCT_SENSOR_ID],[DAI_CHANNEL_NUMBER],[IsDeleted],[SAFETY_FACTOR_TYPE_ID],[DTU_ID],[Identification])
                       values 10000,1,1,0,40,147,2");
            sqls.Add(@"insert into [T_DIM_SENSOR] ([SENSOR_ID],[PRODUCT_SENSOR_ID],[DAI_CHANNEL_NUMBER],[IsDeleted],[SAFETY_FACTOR_TYPE_ID],[DTU_ID],[Identification])
                       values 10000,1,1,0,40,147,2");
            sqls.Add(@"insert into [T_DIM_SENSOR] ([SENSOR_ID],[PRODUCT_SENSOR_ID],[DAI_CHANNEL_NUMBER],[IsDeleted],[SAFETY_FACTOR_TYPE_ID],[DTU_ID],[Identification])
                       values 10000,1,1,0,40,147,2");*/

        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
