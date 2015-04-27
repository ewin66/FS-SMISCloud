#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="DataAccess.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System;

namespace FSDE.DALFactory
{
    using System.Configuration;
    using System.Reflection;

    using FSDE.IDAL;

    public class DataAccess
    {
        private static readonly string Path;

        static DataAccess()
        {
            var file = new ExeConfigurationFileMap { ExeConfigFilename = "config/Params.config" };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            Path = config.AppSettings.Settings["FSDEDAL"].Value;
        }

        #region 配置信息

        public static ISensorInfoDal CreateSensorInfoDal()
        {
            string className = Path + ".Config.SensorInfoDal";
            return (ISensorInfoDal)Assembly.Load(Path).CreateInstance(className);
        }

        public static IGroupInfoDal CreateGroupInfoDal()
        {
            string className = Path + ".Config.GroupInfoDal";
            return (IGroupInfoDal)Assembly.Load(Path).CreateInstance(className);
        }

        public static IDataBaseName CreateDataBaseNameDal()
        {
            string className = Path + ".Config.DataBaseNameDal";
            return (IDataBaseName)Assembly.Load(Path).CreateInstance(className);
        }

        public static IGroupSensors CreateGroupSensorsDal()
        {
            string className = Path + ".Config.GroupSensorsDal";
            return (IGroupSensors)Assembly.Load(Path).CreateInstance(className);
        }

        public static IDataFilterType CreateDataFilterTypeDal()
        {
            string className = Path + ".Config.DataFilterTypeDal";
            return (IDataFilterType)Assembly.Load(Path).CreateInstance(className);
        }

        public static IDataFilter CreateDataFilterDal()
        {
            string className = Path + ".Config.DataFilterDal";
            return (IDataFilter)Assembly.Load(Path).CreateInstance(className);
        }

        public static IProductCategory CreateProductCategoryDal()
        {
            string className = Path + ".Config.ProductCategoryDal";
            return (IProductCategory)Assembly.Load(Path).CreateInstance(className);
        }

        public static IExtractValueName CreateExtractValueNameDal()
        {
            string className = Path + ".Config.ExtractValueNameDal";
            return (IExtractValueName)Assembly.Load(Path).CreateInstance(className);
        }

        public static ITableFieldInfo CreateTableFieldInfoDal()
        {
            string className = Path + ".Config.TableFieldInfoDal";
            return (ITableFieldInfo)Assembly.Load(Path).CreateInstance(className);
        }
        public static IProjectInfo CreateProjectInfoDal()
        {
            string className = Path + ".Config.ProjectInfoDal";
            return (IProjectInfo)Assembly.Load(Path).CreateInstance(className);
        }

        public static ISFormulaidSet CreateSFormulaidSetDal()
        {
            string className = Path + ".Config.SFormulaidSetDal";
            return (ISFormulaidSet)Assembly.Load(Path).CreateInstance(className);
        }

        public static IFormulaParaName CreateFormulaParaNameDal()
        {
            string className = Path + ".Config.FormulaParaNameDal";
            return (IFormulaParaName)Assembly.Load(Path).CreateInstance(className);
        }

        public static ISensorType CreateSensorTypeDal()
        {
            string className = Path + ".Config.SensorTypeDal";
            return (ISensorType)Assembly.Load(Path).CreateInstance(className);
        }

        public static IFormulaInfo CreateFormulaInfoDal()
        {
            string className = Path + ".Config.FormulaInfoDal";
            return (IFormulaInfo)Assembly.Load(Path).CreateInstance(className);
        }

        public static IConfigTable CreateConfigTableDal()
        {
            string className = Path + ".Config.ConfigTableDal";
            return (IConfigTable)Assembly.Load(Path).CreateInstance(className);
        }

        #endregion 配置信息



        public static IConnectTest CreateConnectTestDal()
        {
            string className = Path + ".ConnectTestDal";
            return (IConnectTest)Assembly.Load(Path).CreateInstance(className);
        }

        #region 未发送数据
        public static ICacheDataPackets CreateCacheDataPacketsDal()
        {
            string className = Path + ".Select.CacheDataPacketDal";
            return (ICacheDataPackets)Assembly.Load(Path).CreateInstance(className);
        }
        #endregion 未发送数据

        #region 数据查询

        /// <summary>
        /// 查询其他数据库DAl
        /// </summary>
        /// <returns></returns>
        public static ISelectTablesDal CreateSelectOtherTablesDal()
        {
            string className = Path + ".Select.SelectOtherDBDal";
            return (ISelectTablesDal)Assembly.Load(Path).CreateInstance(className);
        }

        /// <summary>
        /// 查询统一采集软件数据库DAl
        /// </summary>
        /// <returns></returns>
        public static ISelectTablesDal CreateSelectFSUSDBTablesDal()
        {
            string className = Path + ".Select.SelectFSUSDBDal";
            return (ISelectTablesDal)Assembly.Load(Path).CreateInstance(className);
        }

        public static ISelectTablesDal CreateSelectConfigTableInfoDal()
        {
            string className = Path + ".Select.SelectConfigTableInfoDal";
            return (ISelectTablesDal)Assembly.Load(Path).CreateInstance(className);
        }

        public static IExtractionConfigDal CreatExtractionConfigDal()
        {
            string className = Path + ".Select.ExtractionConfigDal";
            return (IExtractionConfigDal)Assembly.Load(Path).CreateInstance(className);
        }

        public static ITextOrBinarySelectDal CreateArtVibrationDataSelectDal()
        {
            string className = Path + ".Select.ArtVibrationDataSelectDal";
            return (ITextOrBinarySelectDal)Assembly.Load(Path).CreateInstance(className);
        }

        public static ITextOrBinarySelectDal CreateMoiFiberGratingDataSelectDal()
        {
            string className = Path + ".Select.MoiFiberGratingDataSelectDal";
            return (ITextOrBinarySelectDal)Assembly.Load(Path).CreateInstance(className);
        }

        /// <summary>
        /// 提取自家振动数据
        /// </summary>
        /// <returns></returns>
        public static ITextOrBinarySelectDal CreateOurVibrationtDal()
        {
            string className = Path + ".Select.OurVibration";
            return (ITextOrBinarySelectDal)Assembly.Load(Path).CreateInstance(className);
        }

        #endregion 数据查询

    }
}