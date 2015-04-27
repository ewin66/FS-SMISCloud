namespace NGDAC.Test.Model
{
    using System;
    using System.Data;

    using FS.DbHelper;

    using NUnit.Framework;

    using DbType = FS.DbHelper.DbType;

    [TestFixture]
    public class MsSQLTester
    {
        static string connstr = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004";
        [Test]
        public void TestMSSQLConnection()
        {
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, connstr);

            DataSet ds = sqlHelper.Query("select CONVERT(varchar(100), GETDATE(), 23)"); //yyyy-MM-dd
            DataRow dr = ds.Tables[0].Rows[0];
            DateTime now = System.DateTime.Now;
            string sqlDate = Convert.ToString(dr[0]);
            Assert.AreEqual(now.ToString("yyyy-MM-dd"), sqlDate);
        }
    }
}
