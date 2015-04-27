using FS.SMIS_Cloud.DAC.Accessor;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Tran;
using FS.SMIS_Cloud.DAC.Tran.Db;
using NUnit.Framework;

namespace DAC.Test.Tran
{
    using FS.DbHelper;

    [TestFixture]
    public class LoadDbConfigXmlTester
    {
          string path = ".\\Tran\\DbMapping.xml";

          [Test]
          public void TestLoadDbConfigxml()
          {
              var ldcx = new LoadDbConfigXml(path);
              DataSourseTableInfo[] tables = ldcx.GetDataSourseTableInfo("/config/databases");
              if (tables.Length != 1)
              {
                  Assert.Fail();
                  return;
              }
              Assert.AreEqual(tables[0].DataBaseName, "DW_iSecureCloud_Empty");
              Assert.AreEqual(tables[0].DbType, DbType.MSSQL);
              string connectionString =
                  "Initial Catalog=DW_iSecureCloud_Empty;Data Source=192.168.1.128;User Id=sa;Password=861004";
              Assert.AreEqual(tables[0].ConnectionString, connectionString);
              Assert.AreEqual(tables[0].TableName, "T_THEMES_ENVI_WIND");
              Assert.AreEqual(tables[0].Type,ProtocolType.GPS_ZHD);
              Assert.AreEqual(tables[0].Colums.Length, 6);
              if (tables[0].Colums.Length==6)
              {
                  Assert.AreEqual(tables[0].Colums[0], "ID");
                  Assert.AreEqual(tables[0].StandardFields[0] , "ID");
                  Assert.AreEqual(tables[0].Colums[1], "SENSOR_ID");
                  Assert.AreEqual(tables[0].StandardFields[1] , "SID");
                  Assert.AreEqual(tables[0].Colums[2], "ACQUISITION_DATETIME");
                  Assert.AreEqual(tables[0].StandardFields[2] , "AcqTime");
                  Assert.AreEqual(tables[0].Colums[3], "WIND_SPEED_VALUE");
                  Assert.AreEqual(tables[0].StandardFields[3] , "Value1");
                  Assert.AreEqual(tables[0].Colums[4], "WIND_DIRECTION_VALUE");
                  Assert.AreEqual(tables[0].StandardFields[4] , "Value2");
                  Assert.AreEqual(tables[0].Colums[5], "WIND_ELEVATION_VALUE");
                  Assert.AreEqual(tables[0].StandardFields[5], "Value3");
              }
              Assert.AreEqual(tables[0].DataCount,3);
              Assert.AreEqual(tables[0].Filter, "where lastSyncTime is null");
          }
    }
}