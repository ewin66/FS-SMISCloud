#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SelectFSUSDBDal.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140603 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.DAL.Select
{
    using System.Data;
    using System.Data.SQLite;
    using System.Text;

    using FreeSun.Common.DB;

    using FSDE.Dictionaries.config;
    using FSDE.IDAL;
    using FSDE.Model.Config;

    public class SelectFSUSDBDal : ISelectTablesDal
    {
        public DataSet Select(DataBaseName database)
        {
            string connstr = Connectionstring.GetConnectionString(database);
            var sqlstr = new StringBuilder();

            sqlstr.Append("SELECT @ProjectCode as ProjectCode, @databaseId as DataBaseNameID, SENSOR_SET_ID, ModuleNo, ChannelID, ACQUISITION_DATETIME, 14 as SAFETY_FACTOR_TYPE_ID, AngleOriginalX, AngleOriginalY, AngleOffsetX, AngleOffsetY FROM D_OriginalInclinationData where ACQUISITION_DATETIME >@ACQUISITION_DATETIME1 order by ID ASC")
                  .Append(";")
                  .Append("SELECT @ProjectCode as ProjectCode, @databaseId as DataBaseNameID, SENSOR_SET_ID, ModuleNo, ChannelID, ACQUISITION_DATETIME, 12 as SAFETY_FACTOR_TYPE_ID, OriginalDisplayment, OffsetDisplayment FROM D_OriginalLVDTData where ACQUISITION_DATETIME >@ACQUISITION_DATETIME2 order by ID ASC")
                  .Append(";")
                  .Append("SELECT @ProjectCode as ProjectCode, @databaseId as DataBaseNameID, SENSOR_Set_ID, ModuleNo, ChannelID, ACQUISITION_DATETIME, 6 as SAFETY_FACTOR_TYPE_ID, OrgVoltage, HUMILITY_VALUE, Mechan_Value FROM D_OriginalMagneticFluxData where ACQUISITION_DATETIME >@ACQUISITION_DATETIME3 order by ID ASC")
                  .Append(";")
                  .Append("SELECT @ProjectCode as ProjectCode, @databaseId as DataBaseNameID, SENSOR_Set_ID, ModuleNo, ChannelID, ACQUISITION_DATETIME, 11 as SAFETY_FACTOR_TYPE_ID, ColPressureValue, CulcPressureValue FROM D_OriginalPressureData where ACQUISITION_DATETIME >@ACQUISITION_DATETIME4 order by ID ASC")
                  .Append(";")
                  .Append("SELECT @ProjectCode as ProjectCode, @databaseId as DataBaseNameID, SENSOR_Set_ID, ModuleNo, ChannelID, ACQUISITION_DATETIME, 3 as SAFETY_FACTOR_TYPE_ID, RainFall FROM D_OriginalRainFallData where ACQUISITION_DATETIME >@ACQUISITION_DATETIME5 order by ID ASC")
                  .Append(";")
                  .Append("SELECT @ProjectCode as ProjectCode, @databaseId as DataBaseNameID, SENSOR_Set_ID, ModuleNo, ChannelID, ACQUISITION_DATETIME, 9 as SAFETY_FACTOR_TYPE_ID, TEMPERATURE_VALUE, HUMILITY_VALUE FROM D_OriginalTempHumiData where ACQUISITION_DATETIME >@ACQUISITION_DATETIME6 order by ID ASC")
                  .Append(";")
                  .Append("SELECT @ProjectCode as ProjectCode, @databaseId as DataBaseNameID, SENSOR_SET_ID, ModuleNo, ChannelID, ACQUISITION_DATETIME, 2 as SAFETY_FACTOR_TYPE_ID, Frequency_VALUE, TEMPERATURE_VALUE, PhysicalValue FROM D_OriginalVibratingWireData where ACQUISITION_DATETIME >@ACQUISITION_DATETIME7 order by ID ASC")
                  .Append(";")
                  .Append("SELECT @ProjectCode as ProjectCode, @databaseId as DataBaseNameID, SENSOR_Set_ID, ModuleNo, ChannelID, ACQUISITION_DATETIME, 1 as SAFETY_FACTOR_TYPE_ID, OrgVoltage, displayment FROM D_OriginalVoltageData where ACQUISITION_DATETIME >@ACQUISITION_DATETIME8 order by ID ASC")
                  .Append(";")
                  .Append("SELECT @ProjectCode as ProjectCode, @databaseId as DataBaseNameID, SENSOR_Set_ID, ModuleNo, ChannelID, ACQUISITION_DATETIME, 5 as SAFETY_FACTOR_TYPE_ID, WIND_SPEED_VALUE, WIND_DIRECTION_VALUE, WIND_ELEVATION_VALUE, TEMPERATURE_VALUE FROM D_OriginalWindData where ACQUISITION_DATETIME >@ACQUISITION_DATETIME9 order by ID ASC");

             var para = new[]
                           {
                               new SQLiteParameter("@ProjectCode",ProjectInfoDic.GetInstance().GetProjectInfo().ProjectCode), 
                               new SQLiteParameter("@databaseId", database.ID),
                               new SQLiteParameter("@ACQUISITION_DATETIME1",ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)database.ID,"D_OriginalInclinationData").Acqtime),
                               new SQLiteParameter("@ACQUISITION_DATETIME2",ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)database.ID,"D_OriginalLVDTData").Acqtime),
                               new SQLiteParameter("@ACQUISITION_DATETIME3",ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)database.ID,"D_OriginalMagneticFluxData").Acqtime),
                               new SQLiteParameter("@ACQUISITION_DATETIME4",ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)database.ID,"D_OriginalPressureData").Acqtime),
                               new SQLiteParameter("@ACQUISITION_DATETIME5",ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)database.ID,"D_OriginalRainFallData").Acqtime),
                               new SQLiteParameter("@ACQUISITION_DATETIME6",ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)database.ID,"D_OriginalTempHumiData").Acqtime),
                               new SQLiteParameter("@ACQUISITION_DATETIME7",ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)database.ID,"D_OriginalVibratingWireData").Acqtime),
                               new SQLiteParameter("@ACQUISITION_DATETIME8",ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)database.ID,"D_OriginalVoltageData").Acqtime),
                               new SQLiteParameter("@ACQUISITION_DATETIME9",ExtractionConfigDic.GetExtractionConfigDic().GetExtractionConfig((int)database.ID,"D_OriginalWindData").Acqtime)
                           };
            var dbhelper = new DbHelperSqLiteP(connstr);
            return dbhelper.Query(sqlstr.ToString(), para);
        }
    }
}