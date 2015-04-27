// --------------------------------------------------------------------------------------------
// <copyright file="Calculator.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：计算类
// 
// 创建标识：刘歆毅20140217
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Calculation
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using FreeSun.FS_SMISCloud.Server.DataCalc.Arithmetic;
    using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;
    using FreeSun.FS_SMISCloud.Server.DataCalc.Model;

    using log4net;

    /// <summary>
    /// 计算类
    /// </summary>
    public class Calculator
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
        /// <summary>
        /// 结构物编号
        /// </summary>
        public int StructId { get; set; }
        /// <summary>
        /// 数据采集轮数
        /// </summary>
        //public string RoundNum { get; set; }
        /// <summary>
        /// 数据采集时间
        /// </summary>
        public DateTime CollectTime { get; set; }

        /// <summary>
        /// 处理计算请求
        /// </summary>
        /// <param name="source">要计算的数据源</param>
        /// <param name="calcCompletion">计算完成时的回调</param>
        /// <returns></returns>
        public void ProcessRequest(DataTable source, Action<IEnumerable<string>> calcCompletion)
        {
            // 查询算法配置
            DataTable arithConfig
                = DataAccessHelper.GetArithmeticConfig(
                    source.AsEnumerable()
                        .Select(row => row.Field<int>("SensorId"))
                        .ToArray());
            this.logger.Info(string.Format("获取结构物编号：{0},采集时间{1},算法配置-成功", this.StructId, this.CollectTime));

            // 区分需要二次计算和不需要二次计算的数据
            Dictionary<string, IList<Data>> dataToCalc; // 需要计算的字典（键为算法标识）
            IList<Data> dataNonCalc; // 不需要计算的列表
            this.JudgeDataToCalc(arithConfig, source, out dataToCalc, out dataNonCalc);

            // 调用相应的算法
            this.logger.Debug(string.Format("结构物编号：{0},采集时间{1},二次计算开始异步执行", this.StructId, this.CollectTime));
            foreach (KeyValuePair<string, IList<Data>> keyValuePair in dataToCalc)
            {
                string arithIden = keyValuePair.Key;
                IList<Data> rawData = keyValuePair.Value;
                this.logger.Debug(
                    string.Format(
                        "结构物编号：{0},采集时间{1},{2}计算开始，共{3}条数据", this.StructId, this.CollectTime, arithIden, rawData.Count));
                var t = this.CalcViaArithmeticAsyn(arithIden, rawData);                
                t.Start();
                t.ContinueWith(rslt =>
                {
                    if (rslt.Exception != null)
                    {
                        this.logger.Error(
                            string.Format("结构物编号：{0},采集时间{1},{2}计算出错", this.StructId, this.CollectTime, arithIden),
                            rslt.Exception);
                    }
                    else
                    {
                        this.logger.Info(
                            string.Format("结构物编号：{0},采集时间{1},{2}计算完成", this.StructId, this.CollectTime, arithIden));
                        // 完成计算后存入数据和发送通知消息
                        this.CompleteRequest(arithIden, rslt.Result, calcCompletion);
                    }
                });
            }

            // 处理不需要计算的数据
            this.CompleteRequest(null, dataNonCalc, calcCompletion);
        }

        /// <summary>
        /// 利用算法进行异步计算
        /// </summary>        
        /// <param name="arithIden">算法标识</param>
        /// <param name="rawData">原始数据</param>
        /// <returns>带计算结果返回值的异步任务</returns>
        private Task<IList<Data>> CalcViaArithmeticAsyn(string arithIden, IList<Data> rawData)
        {
            Task<IList<Data>> t = new Task<IList<Data>>(() =>
            {
                // 获取算法
                IArithmetic arith = ArithmeticFactory.CreateArithmetic(arithIden);
                // 计算
                IList<Data> calculatedData = null/*arith.Calculate(rawData)*/;

                return calculatedData;
            });
            return t;
        }

        /// <summary>
        /// 完成计算请求
        /// </summary>
        /// <param name="arithIden">完成的算法</param>
        /// <param name="calculatedData">计算后的数据</param>
        /// <param name="calcCompletion">完成时的回调函数</param>
        private void CompleteRequest(string arithIden, IList<Data> calculatedData, Action<IEnumerable<string>> calcCompletion)
        {
            // 生成流水号
            Dictionary<string, Data> dataNumbers = new Dictionary<string, Data>();
            foreach (Data data in calculatedData)
            {
                string iden = Guid.NewGuid().ToString();
                dataNumbers.Add(iden, data);
            }

            // 存入数据库
            //SaveCalculatedData([数据流水号，传感器编号，原始数据1，原始数据2，计算后数据1，计算后数据2...])
            if (arithIden == null)
            {
                this.logger.Info(string.Format(
                    "结构物编号：{0},采集时间{1},二次计算,无需二次计算的结果存入数据库成功", this.StructId, this.CollectTime));
            }
            else
            {
                this.logger.Info(string.Format(
                    "结构物编号：{0},采集时间{1},二次计算,算法：{2}结果存入数据库成功", this.StructId, this.CollectTime, arithIden));
            }

            // 调用回调函数，发送消息
            if (calcCompletion != null)
            {
                calcCompletion(dataNumbers.Keys);
            }
        }

        /// <summary>
        /// 判定是否需要计算
        /// </summary>
        /// <param name="arithConfig">算法配置</param>
        /// <param name="source">数据源</param>
        /// <param name="dataToCalc">需要计算的字典（算法标识为键,数据为值）</param>
        /// <param name="dataNonCalc">不需要计算的数据列表</param>
        private void JudgeDataToCalc(
            DataTable arithConfig, DataTable source, out Dictionary<string, IList<Data>> dataToCalc, out IList<Data> dataNonCalc)
        {
            dataToCalc = new Dictionary<string, IList<Data>>();
            dataNonCalc = new List<Data>();

            IList<int> sensorsToCalc
                = arithConfig.AsEnumerable()
                    .Select(row => row.Field<int>("SensorId"))
                    .ToList();
            // 遍历数据
            foreach (var src in source.AsEnumerable())
            {
                // 对应的传感器
                int sensorId = src.Field<int>("SensorId");
                // 对应数据
                Data data = new Data()
                {
                    SensorId = src.Field<int>("SensorId"),
                    DataSet = new List<double>(),
                    CollectTime = src.Field<DateTime>("Time")
                };

                for (int i = 1; i < src.ItemArray.Length - 1; i++)
                {
                    if (src.ItemArray[i] == DBNull.Value || src.ItemArray[i] == null)
                    {
                        break;
                    }
                    data.DataSet.Add(Convert.ToDouble(src.ItemArray[i]));
                }

                if (sensorsToCalc.Contains(sensorId)) // 是需要计算的传感器
                {
                    // 对应算法
                    string arithIden
                        = arithConfig.AsEnumerable()
                            .First(row => row.Field<int>("SensorId") == sensorId)
                            .Field<string>("ArithIden");                    

                    // 添加到对应算法的数据集合
                    if (dataToCalc.ContainsKey(arithIden)) // 字段中存在该算法
                    {
                        dataToCalc[arithIden].Add(data);
                    }
                    else
                    {
                        dataToCalc[arithIden] = new List<Data> {data};
                    }
                }
                else
                {
                    dataNonCalc.Add(data);
                }
            }
        }
    }
}