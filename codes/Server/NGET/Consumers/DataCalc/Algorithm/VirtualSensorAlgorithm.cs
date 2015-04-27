#region Clone
//  --------------------------------------------------------------------------------------------
//  <copyright file="VirtualSensorAlgorithm.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141112 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGET.DataCalc.Algorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FS.SMIS_Cloud.NGET.DataCalc.Model;
    using FS.SMIS_Cloud.NGET.Model;

    using log4net;

    internal class VirtualSensorAlgorithm : BaseAlgorithm
    {
        private readonly SensorGroup _sensorGroup;
        private readonly ILog _logger = LogManager.GetLogger(typeof(VirtualSensorAlgorithm));

        public VirtualSensorAlgorithm(SensorGroup group)
        {
            this._sensorGroup = group;
            this.AlgorithmName = AlgorithmNames.VirtualSensorAlgo;
        }

        public override void CalcData(IList<SensorAcqResult> sensordatas)
        {
            SensorAcqResult virtualsensor = null;
            switch (this._sensorGroup.FormulaId)
            {
                case 13:    // 激光测距 拱顶沉降
                    virtualsensor = this.CalcLaserSettlement();
                    break;
                case 15:    // 多个传感器数据作平均
                    virtualsensor = this.CalcVirtualAverage();
                    break;
                case 27: // TODO 公式ID ? 钢支撑轴力
                    virtualsensor = this.CalcSteelSupports();
                    break;
                case 31:  // 多个传感器数据取最大值   yfh
                    virtualsensor = CalcVirtualMax();
                    break;
                default:
                    break;
            }
            if (virtualsensor != null)
                sensordatas.Add(virtualsensor);
        }

        /// <summary>
        ///  多个传感器数据取最大值
        /// </summary>
        private SensorAcqResult CalcVirtualMax()
        {
            SensorAcqResult virtualsensor = null;
            try
            {
                IList<double?> calcValueList = new List<double?>();

                foreach (var groupItem in _sensorGroup.Items)
                {
                    if (groupItem.Paramters.ContainsKey("value"))
                    {
                        var acqRes = (SensorAcqResult)groupItem.Paramters["value"];
                        var themeval = acqRes.Data.ThemeValues[0];

                        if (themeval != null)
                        {
                            calcValueList.Add(themeval);
                        }
                    }
                }

                if (calcValueList.Count > 0)
                {
                    double? calcvalue = calcValueList.Max();

                    switch (_sensorGroup.FactorTypeId)
                    {
                        case 16: // 锚杆受力
                            virtualsensor = new SensorAcqResult()
                            {
                                ErrorCode = 0,
                                Sensor = _sensorGroup.VirtualSensor,
                                Data = new VirtualSensor(1, new []{calcvalue})
                                {
                                    JsonResultData =
                                        string.Format("{0}\"sensorId\":{1},\"data\":\"最大值:{2} kN\"{3}", '{',
                                            _sensorGroup.VirtualSensor.SensorID, calcvalue, '}')
                                }
                            };
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("二次计算-多个传感器数据取最大值 计算异常:" + ex.Message);
                virtualsensor = null;
            }
            return virtualsensor;
        }

        private SensorAcqResult CalcSteelSupports()
        {
            if (this._sensorGroup == null || this._sensorGroup.VirtualSensor == null)
            {
                return null;
            }
            SensorAcqResult virtualsensor = null;
            DateTime acqTime = DateTime.MinValue;
            int count = 0;
            double sum = 0;
            try
            {
                foreach (var groupItem in this._sensorGroup.Items)
                {
                    if (groupItem.Paramters.ContainsKey("value"))
                    {
                        var acqRes = (SensorAcqResult)groupItem.Paramters["value"];
                        //acqTime = acqRes.Data.AcqTime;
                        if (acqRes.Data != null && acqRes.Data.ThemeValues[0] != null)
                        {
                            if (acqRes.Sensor.Parameters.Count > 4 && Math.Abs(acqRes.Sensor.Parameters[4].Value) > 0)
                            {
                                count++;
                                //  phy/aj(单根钢筋的截面积)
                                sum += (acqRes.Data.ThemeValues[0].Value / acqRes.Sensor.Parameters[4].Value);
                            }
                        }
                    }
                }
                if (count > 0)
                {
                    double a = sum / count; // 
                    double ec = this._sensorGroup.FormulaParams[0]; // 混凝土弹性模量
                    double ei = this._sensorGroup.FormulaParams[1]; // 钢筋弹性模量
                    double ac = this._sensorGroup.FormulaParams[2]; // 混凝土净截面面积
                    double ai = this._sensorGroup.FormulaParams[3]; // 钢筋总面积
                    if (Math.Abs(ei) > 0)
                    {
                        double? nc = a * (ec * ac / ei + ai); // 钢支撑轴力
                        virtualsensor = new SensorAcqResult
                        {
                            ErrorCode = 0,
                            Sensor = this._sensorGroup.VirtualSensor,
                            Data = new VirtualSensor(1, new[] { nc })
                            {
                                //AcqTime = acqTime,
                                JsonResultData =
                                    string.Format("{0}\"sensorId\":{1},\"data\":\"钢支撑轴力:{2} kN \"{3}", '{',
                                        this._sensorGroup.VirtualSensor.SensorID, nc, '}')
                            }
                        };
                    }
                    else
                    {
                        this._logger.WarnFormat("二次计算- 钢支撑轴力-{0}-钢筋弹性模量不能为0:", this._sensorGroup.VirtualSensor.SensorID);
                    }
                }
                else
                {
                    this._logger.WarnFormat("二次计算- 钢支撑轴力-{0}-没有有效的传感器数据:", this._sensorGroup.VirtualSensor.SensorID);
                }
            }
            catch (Exception ex)
            {
                this._logger.WarnFormat("二次计算- 钢支撑轴力-{0}-计算发生异常 :{1}", this._sensorGroup.VirtualSensor.SensorID, ex.Message);
                virtualsensor = null;
            }
            return virtualsensor;
        }

        /// <summary>
        /// 激光测拱顶沉降
        /// </summary>
        private SensorAcqResult CalcLaserSettlement()
        {
            SensorAcqResult virtualsensor = null;
            double? calcvalue = null;
            try
            {
                var sensorAcqResult = (SensorAcqResult)this._sensorGroup.Items[0].Paramters["value"];

                if (Math.Abs(this._sensorGroup.FormulaParams[0]) > 0.00000001 && sensorAcqResult.Data.ThemeValues[0].HasValue)
                {
                    calcvalue = (this._sensorGroup.FormulaParams[0] * 1000 -
                                 Math.Abs(sensorAcqResult.Data.ThemeValues[0].Value)) *
                                this._sensorGroup.FormulaParams[1] / this._sensorGroup.FormulaParams[0];
                }
                // 构造虚拟传感器数据结果实例
                virtualsensor = new SensorAcqResult
                {
                    ErrorCode = 0,
                    Sensor = this._sensorGroup.VirtualSensor,
                    Data = new VirtualSensor(0, new[] { calcvalue })    //@TODO 0 or 1
                    {
                        JsonResultData =
                            string.Format("{0}\"sensorId\":{1},\"data\":\"拱顶沉降:{2} mm\"{3}", '{',
                                this._sensorGroup.VirtualSensor.SensorID, calcvalue, '}')
                    }
                };
            }
            catch (Exception ex)
            {
                this._logger.WarnFormat("二次计算- 激光测拱顶沉降- {0}- 计算异常:{1}", this._sensorGroup.VirtualSensor.SensorID, ex.Message);
                virtualsensor = null;
            }
            return virtualsensor;
        }

        /// <summary>
        /// 多个传感器数据作平均
        /// </summary>
        private SensorAcqResult CalcVirtualAverage()
        {
            SensorAcqResult virtualsensor = null;
            try
            {
                double? calcvalue = 0;
                int count = 0;
                foreach (var groupItem in this._sensorGroup.Items)
                {
                    if (groupItem.Paramters.ContainsKey("value"))
                    {
                        var acqRes = (SensorAcqResult)groupItem.Paramters["value"];
                        var themeval = acqRes.Data.ThemeValues[0];
                        if (themeval != null)
                        {
                            calcvalue += themeval;
                            count++;
                        }
                    }
                }

                if (count > 0)
                {
                    calcvalue = calcvalue / count;

                    switch (this._sensorGroup.FactorTypeId)
                    {
                        case 16: // 锚杆受力
                            virtualsensor = new SensorAcqResult()
                            {
                                ErrorCode = 0,
                                Sensor = this._sensorGroup.VirtualSensor,
                                Data = new VirtualSensor(0, new[] { calcvalue })    //@TODO 0 or 1
                                {
                                    JsonResultData =
                                        string.Format("{0}\"sensorId\":{1},\"data\":\"平均值:{2} kN\"{3}", '{',
                                            this._sensorGroup.VirtualSensor.SensorID, calcvalue, '}')
                                }
                            };
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.WarnFormat("二次计算- 锚杆受力平均- {0}- 计算异常:{1}", this._sensorGroup.VirtualSensor.SensorID, ex.Message);
                virtualsensor = null;
            }
            return virtualsensor;
        }
    }
}