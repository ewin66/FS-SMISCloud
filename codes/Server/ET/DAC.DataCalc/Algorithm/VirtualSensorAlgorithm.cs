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

using System;
using System.Collections.Generic;
using System.Linq;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using FS.SMIS_Cloud.DAC.Model;
using log4net;
using SensorGroup = FS.SMIS_Cloud.DAC.DataCalc.Model.SensorGroup;

namespace FS.SMIS_Cloud.DAC.DataCalc.Algorithm
{
    internal class VirtualSensorAlgorithm : BaseAlgorithm
    {
        private readonly SensorGroup _sensorGroup;
        private readonly ILog _logger = LogManager.GetLogger(typeof(VirtualSensorAlgorithm));

        public VirtualSensorAlgorithm(SensorGroup group)
        {
            this._sensorGroup = group;
            this.AlgorithmName = AlgorithmNames.VirtualSensorAlgo;
        }

        public override bool CalcData(IList<SensorAcqResult> sensordatas)
        {
            SensorAcqResult virtualsensor = null;
            switch (_sensorGroup.FormulaId)
            {
                case 13:   // 激光测距 拱顶沉降
                    virtualsensor = CalcLaserSettlement();
                    break;
                case 15:   // 多个传感器数据作平均
                    virtualsensor = CalcVirtualAverage();
                    break;
                case 27:  // TODO 公式ID ? 钢支撑轴力
                    virtualsensor = CalcSteelSupports();
                    break;
                case 31:  // 多个传感器数据取最大值   yfh
                    virtualsensor = CalcVirtualMax();
                    break;
                case 32:  // 虚拟基点计算
                    virtualsensor = CalcVirtualSettlementBase();
                    break;
                default:
                    break;
            }
            if (virtualsensor != null)
            {
                sensordatas.Add(virtualsensor);
                return true;
            }
            else
            {
                return false;
            }
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
                    if (groupItem.Value != null)
                    {
                        var themeval = groupItem.Value.Data.ThemeValues[0];

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
                        case (int)SafetyFactor.ForceAnchor: // 锚杆受力
                            virtualsensor = new SensorAcqResult()
                            {
                                ErrorCode = 0,
                                Sensor = _sensorGroup.VirtualSensor,
                                Data = new VirtualSensor(calcvalue)
                                {
                                    NullPhysicValsCount = 1,
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
            if (_sensorGroup == null || _sensorGroup.VirtualSensor == null)
            {
                return null;
            }
            SensorAcqResult virtualsensor = null;
            DateTime acqTime = DateTime.MinValue;
            int count = 0;
            double sum = 0;
            try
            {
                foreach (var groupItem in _sensorGroup.Items)
                {
                    if (groupItem.Value != null)
                    {
                        if (groupItem.Value.Data.ThemeValues[0] != null)
                        {
                            if (groupItem.Value.Sensor.Parameters.Count > 4 && Math.Abs(groupItem.Value.Sensor.Parameters[4].Value) > 0)
                            {
                                count++;
                                //  phy/aj(单根钢筋的截面积)
                                sum += (groupItem.Value.Data.ThemeValues[0].Value / groupItem.Value.Sensor.Parameters[4].Value);
                            }
                        }
                    }
                }
                if (count > 0)
                {
                    double a = sum / count; // 
                    double ec = _sensorGroup.FormulaParams[0]; // 混凝土弹性模量
                    double ei = _sensorGroup.FormulaParams[1]; // 钢筋弹性模量
                    double ac = _sensorGroup.FormulaParams[2]; // 混凝土净截面面积
                    double ai = _sensorGroup.FormulaParams[3]; // 钢筋总面积
                    if (Math.Abs(ei) > 0)
                    {
                        double nc = a * (ec * ac / ei + ai); // 钢支撑轴力
                        virtualsensor = new SensorAcqResult
                        {
                            ErrorCode = 0,
                            Sensor = _sensorGroup.VirtualSensor,
                            Data = new VirtualSensor(nc)
                            {
                                NullPhysicValsCount = 1,
                                //AcqTime = acqTime,
                                JsonResultData =
                                    string.Format("{0}\"sensorId\":{1},\"data\":\"钢支撑轴力:{2} kN \"{3}", '{',
                                        _sensorGroup.VirtualSensor.SensorID, nc, '}')
                            }
                        };
                    }
                    else
                    {
                        _logger.WarnFormat("二次计算- 钢支撑轴力-{0}-钢筋弹性模量不能为0:", _sensorGroup.VirtualSensor.SensorID);
                    }
                }
                else
                {
                    _logger.WarnFormat("二次计算- 钢支撑轴力-{0}-没有有效的传感器数据:", _sensorGroup.VirtualSensor.SensorID);
                }
            }
            catch (Exception ex)
            {
                _logger.WarnFormat("二次计算- 钢支撑轴力-{0}-计算发生异常 :{1}", _sensorGroup.VirtualSensor.SensorID, ex.Message);
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
                var sensorAcqResult = _sensorGroup.Items[0].Value;

                if (Math.Abs(_sensorGroup.FormulaParams[0]) > 0.00000001 && sensorAcqResult.Data.ThemeValues[0].HasValue)
                {
                    calcvalue = (_sensorGroup.FormulaParams[0] * 1000 -
                                 Math.Abs(sensorAcqResult.Data.ThemeValues[0].Value)) *
                                _sensorGroup.FormulaParams[1] / _sensorGroup.FormulaParams[0];
                }
                // 构造虚拟传感器数据结果实例
                virtualsensor = new SensorAcqResult
                {
                    ErrorCode = 0,
                    Sensor = _sensorGroup.VirtualSensor,
                    Data = new VirtualSensor(calcvalue)
                    {
                        NullPhysicValsCount = 1,

                        JsonResultData =
                            string.Format("{0}\"sensorId\":{1},\"data\":\"拱顶沉降:{2} mm\"{3}", '{',
                                _sensorGroup.VirtualSensor.SensorID, calcvalue, '}')
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.WarnFormat("二次计算 -激光测拱顶沉降 -{0}-计算异常:{1}", _sensorGroup.VirtualSensor.SensorID, ex.Message);
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
                DateTime acqTime = DateTime.MinValue;
                double? calcvalue = 0;
                int count = 0;
                foreach (var groupItem in _sensorGroup.Items)
                {
                    if (groupItem.Value != null)
                    {
                        var themeval = groupItem.Value.Data.ThemeValues[0];
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

                    switch (_sensorGroup.FactorTypeId)
                    {
                        case (int)SafetyFactor.ForceAnchor: // 锚杆受力
                            virtualsensor = new SensorAcqResult()
                            {
                                ErrorCode = 0,
                                Sensor = _sensorGroup.VirtualSensor,
                                Data = new VirtualSensor(calcvalue)
                                {
                                    NullPhysicValsCount = 1,
                                    JsonResultData =
                                        string.Format("{0}\"sensorId\":{1},\"data\":\"平均值:{2} kN\"{3}", '{',
                                            _sensorGroup.VirtualSensor.SensorID, calcvalue, '}')
                                }
                            };
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WarnFormat("二次计算 -传感器数据平均 -{0} -计算异常:{1}", _sensorGroup.VirtualSensor.SensorID, ex.Message);
                virtualsensor = null;
            }
            return virtualsensor;
        }

        /// <summary>
        /// 计算虚拟基点
        /// Result = Base-(RefS-RefB)
        /// Base - 测量系统基点
        /// RefS - 远端参考系统测点
        /// RefB - 远端参考系统基点 
        /// 组中实体传感器数据的顺序与公式参数顺序一致
        /// </summary>
        private SensorAcqResult CalcVirtualSettlementBase()
        {
            SensorAcqResult virtualsensor;
            try
            {
                double? calcvalue = 0;
                if (_sensorGroup.Items.Count < 3
                    || _sensorGroup.Items[0].Value == null
                    || _sensorGroup.Items[1].Value == null
                    || _sensorGroup.Items[2].Value == null)
                {
                    _logger.WarnFormat("二次计算-虚拟基点-{0}-计算失败: 计算所需数据不完整", _sensorGroup.VirtualSensor.SensorID);
                    return null;
                }
                var basePt = _sensorGroup.Items[0].Value;
                var refSitePt = _sensorGroup.Items[1].Value;
                var refBasePt = _sensorGroup.Items[2].Value;
                if (basePt.Data.ThemeValues == null || basePt.Data.ThemeValues.Count == 0)
                {
                    _logger.WarnFormat("二次计算-虚拟基点-{0}-计算失败: 测量基点数据为空", _sensorGroup.VirtualSensor.SensorID);
                    return null;
                }
                if (refSitePt.Data.ThemeValues == null || refSitePt.Data.ThemeValues.Count == 0)
                {
                    _logger.WarnFormat("二次计算-虚拟基点-{0}-计算失败: 远端沉降系统参考测点数据为空", _sensorGroup.VirtualSensor.SensorID);
                    return null;
                }
                if (refBasePt.Data.ThemeValues == null || refBasePt.Data.ThemeValues.Count == 0)
                {
                    _logger.WarnFormat("二次计算-虚拟基点-{0}-计算失败: 远端沉降系统参考基点数据为空", _sensorGroup.VirtualSensor.SensorID);
                    return null;
                }

                calcvalue = basePt.Data.ThemeValues[0] - (refSitePt.Data.ThemeValues[0] - refBasePt.Data.ThemeValues[0]);
                virtualsensor = new SensorAcqResult()
                {
                    ErrorCode = 0,
                    Sensor = _sensorGroup.VirtualSensor,
                    Data = new VirtualSensor(calcvalue)
                    {
                        NullPhysicValsCount = 1,
                        JsonResultData =
                            string.Format("{0}\"sensorId\":{1},\"data\":\"沉降:{2} mm\"{3}", '{',
                                _sensorGroup.VirtualSensor.SensorID, calcvalue, '}')
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.WarnFormat("二次计算-虚拟基点-{0}-计算异常:{1}", _sensorGroup.VirtualSensor.SensorID, ex.Message);
                virtualsensor = null;
            }
            return virtualsensor;
        }
    }
}