using System.Data;
using FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry;
// --------------------------------------------------------------------------------------------
// <copyright file="Data.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：数据模型
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
using log4net;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// 数据
    /// </summary>
    public class Data
    {
        private static ILog _logger = LogManager.GetLogger("Data");
        /// <summary>
        /// 传感器编号
        /// </summary>
        public int SensorId { get; set; }

        /// <summary>
        /// 安全监测因素
        /// </summary>
        public SAFE_FACT Safetyfactor { get; set; }

        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime CollectTime { get; set; }

        /// <summary>
        /// 该条数据集合
        /// </summary>
        public IList<double> DataSet { get; set; }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>展示字符串</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(20*this.DataSet.Count);

            for (int i = 0; i < this.DataSet.Count; i++)
            {
                sb.Append("数据").Append(i + 1).Append(":");
                sb.Append(this.DataSet[i]);
                if (i != this.DataSet.Count - 1)
                {
                    sb.Append(";");
                }
            }
            return string.Format("传感器编号：{0},{1},{2}", this.SensorId, sb, this.CollectTime);
        }

        public virtual string GetSQLString() { return default(string); }

        /// <summary>
        /// 插入数据(一张表一次事物提交)
        /// </summary>
        /// <param name="safetype"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool InsertData(SAFE_FACT safetype,IEnumerable<Data> data)
        {
            switch (safetype)
            {
                case SAFE_FACT.rainfall:
                    DataRainfall.InsertData(data);
                    break;
                case SAFE_FACT.deep_displace:
                    DataDeepDisplacement.InsertData(data);
                    break;
                case SAFE_FACT.surf_displace:
                case SAFE_FACT.bearingdisplace:
                    DataSurfaceDisplacement.InsertData(data);
                    break;
                case SAFE_FACT.beach:
                    DataBeachLen.InsertData(data);
                    break;
                case SAFE_FACT.saturation_line:
                    DataSaturationLine.InsertData(data);
                    break;
                case SAFE_FACT.seepage:
                    DataSeepage.InsertData(data);
                    break;
                case SAFE_FACT.humiture:
                    DataTempAndHumi.InsertData(data);
                    break;
                case SAFE_FACT.beamstrain:
                case SAFE_FACT.weldsstrain:
                    DataFbgStrain.InsertData(data);
                    break;
                case SAFE_FACT.beamstress:
                    DataBeamForce.InsertData(data);
                    break;
                case SAFE_FACT.temperature:
                    DataTempAndHumi.InsertData(data);
                    break;
                case SAFE_FACT.wind2:
                case SAFE_FACT.wind3:
                    DataWind.InsertData(data);
                    break;
                default:
                    _logger.Warn("不支持的监测因素类型:" + safetype);
                    break;
            }
            return true;
        }
    }
}