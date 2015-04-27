namespace NGDAC.Test.Model
{
    using System;
    using System.Data;

    using FS.DbHelper;

    using NUnit.Framework;

    using DbType = FS.DbHelper.DbType;

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
