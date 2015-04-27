#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="MakeMsgToDataCul.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140808 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace R.WirelessCustomTransmission
{
    using System;
    using System.Threading.Tasks;

    using FreeSun.FS_SMISCloud.Server.Common.Messages;

    using log4net;

    using MakeWarningInfo;

    public class MakeMsgToDataCalc
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MakeMsgToDataCalc));
        public static RequestDataCalcMessage MakeMsgToRequestDataCalc(int project, DateTime time)
        {
            var msg = new RequestDataCalcMessage
                          {
                              Id = Guid.NewGuid(),
                              StructId = project,
                              DateTime = time,
                              RoundNum = string.Empty
                          };
                Task task=new Task(
                    () =>
                        {
                            WarningThread.GetWarningThread().SendWarningProcessMessage(msg);
                        });
            task.Start();
            task.ContinueWith(
                        t =>
                        {
                            if (t.Exception != null)
                            {
                                Log.Error("发送数据计算请求失败:" + t.Exception.Message, t.Exception);
                            }
                            else
                            {
                                Log.Info("发送数据计算请求成功");
                            }
                        });
            return msg;
        }

        internal static void MakeMsgToRequestDataCalc(int sensorid, string filePath, DateTime acqTime)
        {
            var msg = new RequestDataCalcMessage
            {
                Id = Guid.NewGuid(),
                SensorID = sensorid,
                DateTime = acqTime,
                FilePath = filePath,
                RoundNum = string.Empty
            };
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(
                () =>
                {
                    WarningThread.GetWarningThread().SendWarningProcessMessage(msg);
                });
            task.Start();
            task.ContinueWith(
                        t =>
                        {
                            if (t.Exception != null)
                            {
                                Log.Error("发送振动计算请求失败:" + t.Exception.Message, t.Exception);
                            }
                            else
                            {
                                Log.Info("发送振动计算请求成功");
                            }
                        });
        }

    }
}