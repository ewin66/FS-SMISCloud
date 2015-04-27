using System;
using System.Data;
using FS.SMIS_Cloud.DAC.Accessor;
using NUnit.Framework;

namespace DAC.Test.Model
{
    using FS.DbHelper;
    using FS.SMIS_Cloud.DAC.Accessor.MSSQL;
    using FS.SMIS_Cloud.DAC.Accessor.SQLite;

    [TestFixture]
    public class SQLiteTester
    {
        [Test]
        public void TestGetPortConfig()
        {
            // SQLite 3 DtuConnection str: 逗号分割的三个库路径;
            string connStr = ".\\FSUSDB\\fsuscfg.db3";
            //string connStr2 = ".\\FSUSDB\\FSUSDataValueDB.db3";
            var sqlHelper = SqlHelperFactory.Create(DbType.SQLite, connStr);
            DataSet ds = sqlHelper.Query("select count(*) as CNT from PortConfig");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Assert.IsTrue(Convert.ToInt32(dr["CNT"]) ==8 );
            }
        }


    }
}
