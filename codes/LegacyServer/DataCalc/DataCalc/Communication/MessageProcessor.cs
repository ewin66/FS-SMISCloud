// --------------------------------------------------------------------------------------------
// <copyright file="MessageProcessor.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：消息处理类
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

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Threading.Tasks;

    using FreeSun.FS_SMISCloud.Server.Common.Messages;
    using FreeSun.FS_SMISCloud.Server.DataCalc.Calculation;
    using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;

    using log4net;

    /// <summary>
    /// 消息处理类
    /// </summary>
    public class MessageProcessor
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());

        /// <summary>
        /// 异步处理请求
        /// </summary>
        /// <param name="request">请求消息</param>
        /// <param name="processCompletion">处理完成时的回调</param>
        /// <returns>处理任务</returns>
        public Task ProcessRequestAsyn(RequestDataCalcMessage request, Action<IEnumerable<RequestScoreMessage>> processCompletion)
        {
            int structId = request.StructId;
            DateTime collectTime = request.DateTime;
            this.logger.Debug(string.Format("开始处理计算请求,结构物编号：{0},采集时间：{1}", structId, collectTime));

            if(string.IsNullOrEmpty(request.FilePath))
            {
                // 查询原始数据
                DataTable source = DataAccessHelper.GetOriginalData(collectTime);

                // 计算任务
                Task task = new Task(() =>
                {
                    DataProcessor dataProcessor = new DataProcessor();
                    dataProcessor.ProcessRequest(structId, source, null);
                });

                return task;
            }
            else // 处理振动数据
            {
                // 计算任务
                Task task = new Task(() =>
                {
                    DataProcessor dataProcessor = new DataProcessor();
                    dataProcessor.ProcessRequest(request.FilePath, request.SensorID, request.DateTime, null);
                });

                return task;
            }
        }

    }
}