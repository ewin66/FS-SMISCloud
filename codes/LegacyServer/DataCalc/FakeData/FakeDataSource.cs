// --------------------------------------------------------------------------------------------
// <copyright file="FakeDataSource.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：模拟数据源
//               用于测试计算流程和算法逻辑，实际运行中使用数据接口。
// 
// 创建标识：刘歆毅20140219
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

using System;
using System.Data;

namespace FakeData
{
    public static class FakeDataSource
    {
        #region 构造数据
        /// <summary>
        /// ET清洗后的原始数据
        /// </summary>
        private static DataTable rawData;
        private static void BuildRawData(DateTime dateTime)
        {
            rawData = new DataTable();
            // 构建列
            rawData.Columns.Add("SensorId").DataType = typeof (int);
            rawData.Columns.Add("Data1").DataType = typeof (double);
            rawData.Columns.Add("Data2").DataType = typeof (double);
            rawData.Columns.Add("Data3").DataType = typeof (double);
            rawData.Columns.Add("Data4").DataType = typeof (double);
            rawData.Columns.Add("Data5").DataType = typeof (double);
            rawData.Columns.Add("Data6").DataType = typeof (double);
            rawData.Columns.Add("Time").DataType = typeof (DateTime);

            // 添加一轮数据
            // 一个测斜管组
            rawData.Rows.Add(1, 0.1, 1.1)["Time"] = dateTime;
            rawData.Rows.Add(2, 0.2, 2.2)["Time"] = dateTime;
            rawData.Rows.Add(3, 0.3, 3.3)["Time"] = dateTime; 
            // 另一个测斜管组
            rawData.Rows.Add(4, 0.4, 4.4)["Time"] = dateTime;
            rawData.Rows.Add(5, 0.5, 5.5)["Time"] = dateTime;
            rawData.Rows.Add(6, 0.6, 6.6)["Time"] = dateTime;
            // 均值
            rawData.Rows.Add(7, 1, 10, 100)["Time"] = dateTime;
            rawData.Rows.Add(8, 2, 20, 200)["Time"] = dateTime;
            // 无需二次计算
            rawData.Rows.Add(9, 100)["Time"] = dateTime;
            rawData.Rows.Add(10, 200, 20, 2)["Time"] = dateTime;
        }

        /// <summary>
        /// 算法配置
        /// </summary>
        private static DataTable arithConfig;
        private static void BuildArithConfig()
        {
            arithConfig = new DataTable();
            // 构建列
            arithConfig.Columns.Add("SensorId").DataType = typeof (int);
            arithConfig.Columns.Add("ArithIden").DataType = typeof (string);

            // 添加算法配置
            arithConfig.Rows.Add(1, "group");
            arithConfig.Rows.Add(2, "group");
            arithConfig.Rows.Add(3, "group");
            arithConfig.Rows.Add(4, "group");
            arithConfig.Rows.Add(5, "group");
            arithConfig.Rows.Add(6, "group");

            arithConfig.Rows.Add(7, "average");
            arithConfig.Rows.Add(8, "average");
        }

        /// <summary>
        /// 分组配置
        /// </summary>
        private static DataTable groupConfig;
        private static void BuildGroupConfig()
        {
            groupConfig = new DataTable();
            // 构建列
            groupConfig.Columns.Add("GroupId").DataType = typeof(int);
            groupConfig.Columns.Add("SensorId").DataType = typeof (int);
            groupConfig.Columns.Add("GroupArg").DataType = typeof (double);

            // 添加分组配置
            groupConfig.Rows.Add(1, 1, -2);
            groupConfig.Rows.Add(1, 2, -4);
            groupConfig.Rows.Add(1, 3, -8);

            groupConfig.Rows.Add(2, 4, -3);
            groupConfig.Rows.Add(2, 5, -8);
            groupConfig.Rows.Add(2, 6, -13);
        }

        /// <summary>
        /// 静态构造器
        /// </summary>
        static FakeDataSource()
        {
            BuildRawData(DateTime.Now);
            BuildArithConfig();
            BuildGroupConfig();
        }
        #endregion

        #region 数据操作方法
        /// <summary>
        /// 查询ET清洗后的数据
        /// </summary>
        /// <param name="structId">结构物编号</param>
        /// <param name="roundNum">采集轮数</param>
        /// <returns>数据</returns>
        public static DataTable GetETData(int structId, DateTime time)
        {
            BuildRawData(DateTime.Now);
            return rawData;
        }

        /// <summary>
        /// 查询算法配置
        /// </summary>
        /// <param name="sensors">传感器数组</param>
        /// <returns>算法配置</returns>
        public static DataTable GetArithmeticConfig(int[] sensors)
        {
            return arithConfig;
        }

        /// <summary>
        /// 查询分组配置
        /// </summary>
        /// <param name="sensors"></param>
        /// <returns></returns>
        public static DataTable GetGroupConfig(int[] sensors)
        {
            return groupConfig;
        }

        /// <summary>
        /// 查询某传感器一段时间的ET清洗后数据
        /// </summary>
        /// <param name="sensorId">传感器编号</param>
        /// <param name="from">开始时间</param>
        /// <param name="to">结束时间</param>
        /// <returns>数据</returns>
        public static DataTable GetETData(int sensorId, DateTime from, DateTime to)
        {
            if (sensorId != 7)
            {
                return null;
            }

            DataTable hisData = new DataTable();
            // 构建列
            hisData.Columns.Add("SensorId").DataType = typeof(int);
            hisData.Columns.Add("Data1").DataType = typeof(double);
            hisData.Columns.Add("Data2").DataType = typeof(double);
            hisData.Columns.Add("Data3").DataType = typeof(double);
            hisData.Columns.Add("Data4").DataType = typeof(double);
            hisData.Columns.Add("Data5").DataType = typeof(double);
            hisData.Columns.Add("Data6").DataType = typeof(double);
            hisData.Columns.Add("Time").DataType = typeof(DateTime);

            DateTime dateTime = DateTime.Now;

            // 添加历史数据
            Random ran = new Random();
            DateTime bfTime = dateTime.AddHours(-1);
            for (int i = 0; i < 24; i++)
            {                
                hisData.Rows.Add(
                    7,
                    ran.Next(0, 5),
                    ran.Next(8, 15),
                    ran.Next(80, 150))["Time"] = bfTime;
                bfTime = bfTime.AddHours(-1);
            }

            return hisData;
        }

        /// <summary>
        /// 查询因异常遗漏未计算的数据
        /// </summary>
        /// <returns>遗漏的数据</returns>
        public static DataTable GetOmitToCalcData()
        {
            return rawData;
        }

        /// <summary>
        /// 保存二次计算后的数据
        /// </summary>
        /// <param name="calcedData">计算后的数据</param>
        public static void SaveCalcedData(DataTable calcedData)
        {

        } 
        #endregion
    }
}