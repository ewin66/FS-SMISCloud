using System.Diagnostics;
using FreeSunCharts.NET;
using log4net;
using Newtonsoft.Json.Linq;
/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：DrawHighcharts.cs
// 功能描述：
// 
// 创建标识： 2014/10/22 13:03:04
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
// </summary>

//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.Dal;
namespace ReportGeneratorService.TemplateHandle
{
    public class DrawHighcharts
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<StreamChart> Draw(int structId, int factorId, DateTime start, DateTime end,string type)
        {
            var streams = new List<StreamChart>();
            var DeepDisplaceFactorId = ConfigurationManager.AppSettings["DeepDisplaceFactorId"];
            var RainfallFactorId = ConfigurationManager.AppSettings["RainfallFactorId"];
            var SurFaceDisplaceFactorId = ConfigurationManager.AppSettings["SurFaceDisplaceFactorId"];
            var TempHumiFactorId = ConfigurationManager.AppSettings["TempHumiFactorId"];
            var jzwCx = ConfigurationManager.AppSettings["JzwCx"];
            var gfCj = ConfigurationManager.AppSettings["GfCj"];
            var gfSl = ConfigurationManager.AppSettings["GfSl"];
            var FactorId = factorId.ToString();
            var structName = DataAccess.GetStructureInfo(structId).Name;
            var factorName = DataAccess.GetFactorInfoById(factorId).NameCN;
            Stopwatch watch = new Stopwatch();
            if (FactorId == DeepDisplaceFactorId) // 测斜
            {
                watch.Start();
                streams = DrawCxChart(structId, factorId, start, end,type);
                watch.Stop();
                string time = watch.ElapsedMilliseconds.ToString();
                logger.Debug(string.Format("{0}..{1}绘图总耗时..{2} 毫秒\r\n", structName, factorName, time));
            }
            else if (FactorId == RainfallFactorId) // 雨量
            {
                watch.Start();
                streams = DrawRainFallChart(structId, factorId, start, end);
                watch.Stop();
                string time = watch.ElapsedMilliseconds.ToString();
                logger.Debug(string.Format("{0}..{1}绘图总耗时..{2} 毫秒\r\n", structName, factorName, time));
            }
            else if (FactorId == SurFaceDisplaceFactorId) //表面位移
            {
                watch.Start();
                streams = DrawSurfaceFallChart(structId, factorId, start, end);
                watch.Stop();
                string time = watch.ElapsedMilliseconds.ToString();
                logger.Debug(string.Format("{0}..{1}绘图总耗时..{2} 毫秒\r\n", structName, factorName, time));
            }
            else if (FactorId == TempHumiFactorId) //温湿度
            {
                watch.Start();
                streams = DrawTempHumiChart(structId, factorId, start, end);
                watch.Stop();
                string time = watch.ElapsedMilliseconds.ToString();
                logger.Debug(string.Format("{0}..{1}绘图总耗时..{2} 毫秒\r\n", structName, factorName, time));
            }
            else if (FactorId == jzwCx)
            {
                watch.Start();
                streams = DrawGfCxChart(structId, factorId, start, end);
                watch.Stop();
                string time = watch.ElapsedMilliseconds.ToString();
                logger.Debug(string.Format("{0}..{1}绘图总耗时..{2} 毫秒\r\n", structName, factorName, time));
            }
            else if (FactorId == gfCj)
            {
                watch.Start();
                streams = DrawGfCjChart(structId, factorId, start, end);
                watch.Stop();
                string time = watch.ElapsedMilliseconds.ToString();
                logger.Debug(string.Format("{0}..{1}绘图总耗时..{2} 毫秒\r\n", structName, factorName, time));
            }
            else if (FactorId == gfSl)
            {
                watch.Start();
                streams = DrawBorderChart(structId, factorId, start, end);
                watch.Stop();
                string time = watch.ElapsedMilliseconds.ToString();
                logger.Debug(string.Format("{0}..{1}绘图总耗时..{2} 毫秒\r\n", structName, factorName, time));
            }
            else //通用数据
            {

                watch.Start();
                streams = DrawGeneralChart(structId, factorId, start, end);
                watch.Stop();
                string time = watch.ElapsedMilliseconds.ToString();
                logger.Debug(string.Format("{0}..{1}绘图总耗时..{2} 毫秒\r\n", structName, factorName, time));
            }
            return streams;
        }

        public List<StreamChart> DrawGfCxChart(int structId, int factorId, DateTime start, DateTime end)
        {
            var tempStream = new List<StreamChart>();
            Stream tempX = null;
            Stream tempY = null;
            const string template = "template/border.json";
            var factorName = DataAccess.GetFactorInfoById(factorId).NameCN;
            List<SensorList> sensorList = DataAccess.FindSensorsByStructAndFactor(structId, factorId);
            if (sensorList == null || sensorList.Count == 0)
            {
                logger.Warn("传感器列表获取失败..结构物倾斜\r\n");
                return null;
            }
            {
                var sensors = sensorList.SelectMany(list => list.Sensors).ToList();
                var seriesX = new List<Serie>();
                var seriesY = new List<Serie>();
                decimal maxX = decimal.MinValue, minX = decimal.MaxValue;
                decimal maxY = decimal.MinValue, minY = decimal.MaxValue;
                for (int i = 0; i < sensors.Count; i++)
                {
                    var data = DataAccess.GetMonitorData(new[] { sensors[i].SensorId }, start, end);
                    var serieX = new Serie();
                    var serieY = new Serie();
                    serieX.Name = sensors[i].Location;
                    serieY.Name = sensors[i].Location;  
                    if (data == null || data.Count == 0)
                    {
                        logger.Warn("获取数据失败..结构物倾斜\r\n");
                        continue;
                    }
                    if (data.Count > 0)
                    {
                   
                        var pointsX = new double[data[0].Data.Count][];
                        var pointsY = new double[data[0].Data.Count][];
                   
                        for (int j = 0; j < data[0].Data.Count; j++)
                        {
                            maxX = data[0].Data[j].Values[0] > maxX ? data[0].Data[j].Values[0] : maxX;
                            minX = data[0].Data[j].Values[0] < minX ? data[0].Data[j].Values[0] : minX;
                            maxY = data[0].Data[j].Values[1] > maxY ? data[0].Data[j].Values[1] : maxY;
                            minY = data[0].Data[j].Values[1] < minY ? data[0].Data[j].Values[1] : minY;

                            pointsX[j] = new[]
                            {
                                (data[0].Data[j].AcquisitionTime - new DateTime(1970, 1, 1)).TotalMilliseconds,
                                (double) (data[0].Data[j].Values[0])
                            };

                            pointsY[j] = new[]
                            {
                                 (data[0].Data[j].AcquisitionTime - new DateTime(1970, 1, 1)).TotalMilliseconds,
                                (double) (data[0].Data[j].Values[1])
                            };

                        

                            serieX.Data = pointsX;
                            serieY.Data = pointsY;
                          

                        }
                    }

                    // 放入series数组
                    bool flag_serie_data = true;
                    flag_serie_data = CheckSerieDataNullOrNot(serieX);
                    if (flag_serie_data)
                    {
                        seriesX.Add(serieX);
                    }
                    flag_serie_data = CheckSerieDataNullOrNot(serieY);
                    if (flag_serie_data)
                    {
                        seriesY.Add(serieY);
                    }
                
                }
                bool flag = false;
                flag = checkSeries(seriesX);

                if (!flag)
                {
                    var yAxisX = "X方向角度(°)";
                    var diff = maxX - minX;
                    var total = diff * 4;
                    var half = total / 2;
                    maxX = maxX + half;
                    minX = minX - half;
                    tempX = DrawChart(new GeneralLineChart(template, seriesX, factorName + "X方向趋势图", yAxisX,Convert.ToString(maxX),Convert.ToString(minX)));
                    if (tempX!= null)
                    {
                        var stream = new StreamChart();
                        stream.ChartStream = tempX;
                        stream.ChartType = "General";
                        tempStream.Add(stream);
                     
                    }

                }
                else
                {
                    
                    logger.Warn(string.Format("X方向数据列为空..{0}\r\n", factorName));
                }

                flag = checkSeries(seriesY);

                if (!flag)
                {
                    var yAxisY = "Y方向角度(°)";
                    var diff = maxY - minY;
                    var total = diff * 4;
                    var half = total / 2;
                    maxY = maxY + half;
                    minY = minY - half;
                    tempY = DrawChart(new GeneralLineChart(template, seriesY, factorName + "Y方向趋势图", yAxisY,Convert.ToString(maxY),Convert.ToString(minY)));
                    if (tempY != null)
                    {
                        var stream = new StreamChart();
                        stream.ChartStream = tempY;
                        stream.ChartType = "General";
                        tempStream.Add(stream);
                       
                    }

                }
                else
                {
              
                    logger.Warn(string.Format("Y方向数据列为空..{0}\r\n", factorName));
                }

            }
            return tempStream;
        }

        public List<StreamChart> DrawSurfaceFallChart(int structId, int factorId, DateTime start, DateTime end)
        {
            var tempStream = new List<StreamChart>();
            Stream temp_x = null;
            Stream temp_y = null;
            Stream temp_z = null;
            const string template = "template/line.json";

            var factorName = DataAccess.GetFactorInfoById(factorId).NameCN;
            List<SensorList> sensorList = DataAccess.FindSensorsByStructAndFactor(structId, factorId);
            if (sensorList == null || sensorList.Count == 0)
            {
                //Console.WriteLine("传感器列表获取失败..表面位移\r\n");
                logger.Warn("传感器列表获取失败..表面位移\r\n");
                return null;

            }
            if (sensorList.Count > 0)
            {
                var sensors = sensorList.SelectMany(list => list.Sensors).ToList();
                var series_X = new List<Serie>();
                var series_Y = new List<Serie>();
                var series_Z = new List<Serie>();
                for (int i = 0; i < sensors.Count; i++)
                {
                    var data = DataAccess.GetMonitorData(new[] { sensors[i].SensorId }, start, end);
                    var serieX = new Serie();
                    var serieY = new Serie();
                    var serieZ = new Serie();

                    serieX.Name = sensors[i].Location;
                    serieY.Name = sensors[i].Location;
                    serieZ.Name = sensors[i].Location;
                    if (data == null || data.Count == 0)
                    {
                        //Console.WriteLine("获取数据失败...表面位移\r\n");
                        logger.Warn("获取数据失败..表面位移\r\n");
                        continue;
                    }
                    if (data.Count > 0)
                    {
                        //Console.WriteLine("获取数据成功...表面位移\r\n");
                        var points_x = new double[data[0].Data.Count][];
                        var points_y = new double[data[0].Data.Count][];
                        var points_z = new double[data[0].Data.Count][];
                        for (int j = 0; j < data[0].Data.Count; j++)
                        {
                            points_x[j] = new[]
                            {
                                (data[0].Data[j].AcquisitionTime - new DateTime(1970, 1, 1)).TotalMilliseconds,
                                (double) (data[0].Data[j].Values[0])
                            };

                            points_y[j] = new[]
                            {
                                 (data[0].Data[j].AcquisitionTime - new DateTime(1970, 1, 1)).TotalMilliseconds,
                                (double) (data[0].Data[j].Values[1])
                            };

                            points_z[j] = new[]
                            {
                                 (data[0].Data[j].AcquisitionTime - new DateTime(1970, 1, 1)).TotalMilliseconds,
                                (double) (data[0].Data[j].Values[2])
                            };

                            serieX.Data = points_x;
                            serieY.Data = points_y;
                            serieZ.Data = points_z;

                        }
                    }

                    // 放入series数组
                    bool flag_serie_data = true;
                    flag_serie_data = CheckSerieDataNullOrNot(serieX);
                    if (flag_serie_data)
                    {
                        series_X.Add(serieX);
                    }
                    flag_serie_data = CheckSerieDataNullOrNot(serieY);
                    if (flag_serie_data)
                    {
                        series_Y.Add(serieY);
                    }
                    flag_serie_data = CheckSerieDataNullOrNot(serieZ);
                    if (flag_serie_data)
                    {
                        series_Z.Add(serieZ);
                    }
                }
                bool flag = false;
                flag = checkSeries(series_X);

                if (!flag)
                {
                    var yAxisX = "X方向位移(mm)";
                    //Console.WriteLine("开始绘制图表..表面位移X方向\r\n");
                    temp_x = DrawChart(new GeneralLineChart(template, series_X, factorName + "X方向趋势", yAxisX));
                    if (temp_x != null)
                    {
                        var stream = new StreamChart();
                        stream.ChartStream = temp_x;
                        stream.ChartType = "General";
                        tempStream.Add(stream);
                        //Console.WriteLine("表面位移X方向绘制图表完成!\r\n");
                    }

                }
                else
                {
                    //Console.WriteLine("X方向位移数据列为空..{0}\r\n", factorName);
                    logger.Warn(string.Format("X方向位移数据列为空..{0}\r\n", factorName));
                }

                flag = checkSeries(series_Y);

                if (!flag)
                {
                    var yAxisY = "Y方向位移(mm)";
                    //Console.WriteLine("开始绘制图表..表面位移Y方向\r\n");
                    temp_y = DrawChart(new GeneralLineChart(template, series_Y, factorName + "Y方向趋势", yAxisY));
                    if (temp_y != null)
                    {
                        var stream = new StreamChart();
                        stream.ChartStream = temp_y;
                        stream.ChartType = "General";
                        tempStream.Add(stream);
                        //Console.WriteLine("表面位移Y方向绘制图表完成!\r\n");
                    }

                }
                else
                {
                    //Console.WriteLine("Y方向位移数据列为空..{0}\r\n", factorName);
                    logger.Warn(string.Format("Y方向位移数据列为空..{0}\r\n", factorName));
                }

                flag = checkSeries(series_Z);

                if (!flag)
                {
                    var yAxisZ = "Z方向位移(mm)";
                    //Console.WriteLine("开始绘制图表..表面位移Z方向\r\n");
                    temp_z = DrawChart(new GeneralLineChart(template, series_Z, factorName + "Z方向趋势", yAxisZ));
                    if (temp_z != null)
                    {
                        var stream = new StreamChart();
                        stream.ChartStream = temp_z;
                        stream.ChartType = "General";
                        tempStream.Add(stream);
                        //Console.WriteLine("表面位移Z方向绘制图表完成!\r\n");
                    }

                }
                else
                {
                    //Console.WriteLine("Z方向位移数据列为空..{0}\r\n", factorName);
                    logger.Warn(string.Format("Z方向位移数据列为空..{0}\r\n", factorName));
                }
            }
            return tempStream;
        }

        public List<StreamChart> DrawGfCjChart(int structId, int factorId, DateTime start, DateTime end)
        {
            var tempStream = new List<StreamChart>();
            Stream temp = null;
            const string template = "template/border.json";
            var factorName = DataAccess.GetFactorInfoById(factorId).NameCN;
            List<Group> group = DataAccess.FindCjGroupsByStructAndFactor(structId, factorId);
            if (group == null || group.Count == 0)
            {
                logger.Warn("传感器分组获取失败...广佛沉降\r\n");
                return null;
            }
            if (group.Count > 0)
            {
                for (int i = 0; i < group.Count; i++)
                {
                    decimal max = decimal.MinValue, min = decimal.MaxValue;
                    var groupName = group[i].GroupName;
                    var sensorId = DataAccess.FindCjSensorsByGroup(group[i].GroupId);
                    if (sensorId == null || sensorId.Count == 0)
                    {
                        logger.Warn("该分组下没有找到相应传感器信息...广佛沉降\r\n");
                        continue;
                    }
                   var sensors = sensorId.Select(m => m.GroupId).ToArray();
                    var seriesX = new List<Serie>();
                    for (int j = 0; j < sensors.Length; j++)
                    {
                        var data = DataAccess.GetMonitorData(new[] { sensorId[j].GroupId }, start, end);
                        var serieX = new Serie();
                        serieX.Name = sensorId[j].GroupName;
                        if (data == null || data.Count == 0)
                        {
                            continue;
                        }
                        if (data.Count > 0)
                        {

                            var pointsX = new double[data[0].Data.Count][];                        
                            for (int k = 0; k < data[0].Data.Count; k++)
                            {
                                max = data[0].Data[k].Values[0] > max ? data[0].Data[k].Values[0] : max;
                                min = data[0].Data[k].Values[0] < min ? data[0].Data[k].Values[0] : min;
                                pointsX[k] = new[]
                            {
                                (data[0].Data[k].AcquisitionTime - new DateTime(1970, 1, 1)).TotalMilliseconds,
                                (double) (data[0].Data[k].Values[0])
                            };                        
                                serieX.Data = pointsX;
                            }
                        }
                        // 放入series数组
                        bool flag_serie_data = true;
                        flag_serie_data = CheckSerieDataNullOrNot(serieX);
                        if (flag_serie_data)
                        {
                            seriesX.Add(serieX);
                        }
                    }
                    bool flag = false;
                    flag = checkSeries(seriesX);

                    if (!flag)
                    {
                        var yAxisX = "变化值(mm)";
                        var diff = max - min;
                        var total = diff * 4;
                        var half = total / 2;
                        max = max + half;
                        min = min - half;
                        temp = DrawChart(new GeneralLineChart(template, seriesX, groupName + "趋势图", yAxisX,Convert.ToString(max),Convert.ToString(min)));
                        if (temp != null)
                        {
                            var stream = new StreamChart();
                            stream.ChartStream = temp;
                            stream.ChartType = "General";
                            tempStream.Add(stream);

                        }

                    }
                    else
                    {

                        logger.Warn(string.Format("X方向数据列为空..{0}\r\n", factorName));
                    }
                }
            }
            return tempStream;
        }

        public List<StreamChart> DrawCxChart(int structId, int factorId, DateTime start, DateTime end,string type)
        {
            var tempStream = new List<StreamChart>();
            Stream temp_x = null;
            Stream temp_y = null;
            Stream temp_x_leiji = null;
            Stream temp_y_leiji = null;
            const string template = "template/line.json";
            //找到结构物对应的测斜传感器所属的组
            List<Group> group = DataAccess.FindDeepDisplaceSensorsByStruct(structId);
            var data = new List<CxDataByDepth>();
            var data_leiji = new List<CxData>();
            if (group == null || group.Count == 0)
            {
                //Console.WriteLine("传感器分组获取失败...测斜\r\n");
                logger.Warn("传感器分组获取失败...测斜\r\n");
                return null;

            }
            if (group.Count > 0)
            {
                for (int i = 0; i < group.Count; i++)
                {
                    var groupName = group[i].GroupName;
                    //按照深度获取数据画测斜趋势图
                    decimal maxX = decimal.MinValue, minX = decimal.MaxValue;
                    decimal maxY = decimal.MinValue, minY = decimal.MaxValue;
                    #region 按照深度获取数据画趋势图

                    if (type == "month")
                    {
                        data = DataAccess.GetByGroupDirectAndDateGroupByDepth(group[i].GroupId, null, start, end);
                    }
                    else if(type=="week")
                    {
                        data = DataAccess.GetByGroupDirectAndDateGroupByDepthW(group[i].GroupId, null, start, end);
                    }
                    if (data == null || data.Count == 0)
                    {
                        continue;
                    }
                    //画图测斜
                    //Console.WriteLine("获取测斜分组成功!\r\n");
                   var series_X = new List<Serie>();
                   var series_Y = new List<Serie>();
                    // 循环深度
                    for (int j = 0; j < data.Count; j++)
                    {
                        var serieX = new Serie();
                        var serieY = new Serie();
                        serieX.Name = "深度" + data[j].Depth.ToString("f1")+"m";
                        serieY.Name = "深度" + data[j].Depth.ToString("f1")+"m";
                        var points_x = new double[data[j].Values.Count][];
                        var points_y = new double[data[j].Values.Count][];
                        for (int k = 0; k < data[j].Values.Count; k++)
                        {
                            maxX = (decimal)data[j].Values[k].Xvalue > maxX ? (decimal)data[j].Values[k].Xvalue : maxX;
                            minX = (decimal)data[j].Values[k].Xvalue < minX ? (decimal)data[j].Values[k].Xvalue : minX;
                            maxY = (decimal)data[j].Values[k].Yvalue > maxY ? (decimal)data[j].Values[k].Yvalue : maxY;
                            minY = (decimal)data[j].Values[k].Yvalue < minY ? (decimal)data[j].Values[k].Yvalue : minY;

                            // 取出x方向位移数据 [时间，值]
                            points_x[k] = new[]
                            {
                                (data[j].Values[k].Acquisitiontime - new DateTime(1970, 1, 1)).TotalMilliseconds,
                                (double) (data[j].Values[k].Xvalue != null ? (decimal) data[j].Values[k].Xvalue : 0)
                            };

                            points_y[k] = new[]
                            {
                                (data[j].Values[k].Acquisitiontime - new DateTime(1970, 1, 1)).TotalMilliseconds,
                                (double) (data[j].Values[k].Yvalue != null ? (decimal) data[j].Values[k].Yvalue : 0)
                            };

                        }
                        serieX.Data = points_x;
                        serieY.Data = points_y;
                        // 放入series数组
                        bool flag_serie_data = true;
                        flag_serie_data = CheckSerieDataNullOrNot(serieX);
                        if (flag_serie_data)
                        {
                            series_X.Add(serieX);
                        }
                        flag_serie_data = CheckSerieDataNullOrNot(serieY);
                        if (flag_serie_data)
                        {
                            series_Y.Add(serieY);
                        }
                    }
                    bool flag = false;
                    flag = checkSeries(series_X);

                    if (!flag)
                    {
                        var yAxisX = string.Format("X方向位移(mm)");
                      
                        var diff = maxX - minX;
                        var total = diff * 4;
                        var half = total / 2;
                        maxX = maxX + half;
                        minX = minX - half;
                        temp_x = DrawChart(new GeneralLineChart(template, series_X, groupName + "管道X方向趋势", yAxisX));
                        if (temp_x != null)
                        {
                            var stream = new StreamChart();
                            stream.ChartStream = temp_x;
                            stream.ChartType = "General";
                            tempStream.Add(stream);
                        }
                        //Console.WriteLine("测斜X方向绘制图表完成!\r\n");
                    }
                    else
                    {
                        //Console.WriteLine("测斜X方向数据列为空..{0}\r\n", groupName);
                        logger.Warn(string.Format("测斜X方向数据列为空..{0}\r\n", groupName));
                    }

                    flag = checkSeries(series_Y);

                    if (!flag)
                    {
                        var yAxisY = string.Format("Y方向位移(mm)");
                        //Console.WriteLine("开始绘制图表..测斜Y方向\r\n");
                        var diff = maxY - minY;
                        var total = diff * 4;
                        var half = total / 2;
                        maxY = maxY + half;
                        minY = minY - half;
                        temp_y = DrawChart(new GeneralLineChart(template, series_Y, groupName + "管道Y方向趋势", yAxisY));
                        if (temp_y != null)
                        {
                            var stream = new StreamChart();
                            stream.ChartStream = temp_y;
                            stream.ChartType = "General";
                            tempStream.Add(stream);
                        }
                        //Console.WriteLine("测斜Y方向绘制图表完成!\r\n");
                    }
                    else
                    {
                        //Console.WriteLine("测斜Y方向数据列为空..{0}\r\n", groupName);
                        logger.Warn(string.Format("测斜Y方向数据列为空..{0}\r\n", groupName));
                    }
                   
                    #endregion

                    //按照时间获得数据画测斜累计变化量趋势
                    #region 按照时间获得数据画测斜累计变化量趋势
                    data_leiji = DataAccess.GetByGroupDirectAndDateGroupByTime(group[i].GroupId, start, end);
                    if (data_leiji == null || data_leiji.Count == 0)
                    {
                        continue;
                    }
                   var series_X_leiji = new List<Serie>();
                   var series_Y_leiji = new List<Serie>();
                    // 循环时间
                    for (int j = 0; j < data_leiji.Count; j++)
                    {
                        var serieX_leiji = new Serie();
                        var serieY_leiji = new Serie();

                        var dateTime = data_leiji[j].DateTime;
                        if (dateTime != null)
                        {
                            var time = (DateTime)dateTime;
                            var acquisitionTime = time.Year + "-" + time.Month + "-" + time.Day + " " + time.Hour +
                                                  ":" + time.Minute + ":" + time.Second;
                            serieX_leiji.Name = acquisitionTime;
                            serieY_leiji.Name = acquisitionTime;
                        }
                        var points_x_leiji = new double[data_leiji[j].Data.Count][];
                        var points_y_leiji = new double[data_leiji[j].Data.Count][];
                        for (int k = 0; k < data_leiji[j].Data.Count; k++)
                        {
                            // 取出x方向位移数据 [深度，监测数据]
                            points_x_leiji[k] = new[]
                            {
                                (double)
                                    (data_leiji[j].Data[k].Depth != null ? (decimal) data_leiji[j].Data[k].Depth : 0),
                                (double)
                                    (data_leiji[j].Data[k].XValue != null ? (decimal) data_leiji[j].Data[k].XValue : 0)
                            };

                            points_y_leiji[k] = new[]
                            {
                                (double)
                                    (data_leiji[j].Data[k].Depth != null ? (decimal) data_leiji[j].Data[k].Depth : 0),
                                (double)
                                    (data_leiji[j].Data[k].YValue != null ? (decimal) data_leiji[j].Data[k].YValue : 0)
                            };

                        }
                        serieX_leiji.Data = points_x_leiji;
                        serieY_leiji.Data = points_y_leiji;
                        // 放入series数组
                        bool flag_serie_data = true;
                        flag_serie_data = CheckSerieDataNullOrNot(serieX_leiji);
                        if (flag_serie_data)
                        {
                            series_X_leiji.Add(serieX_leiji);
                        }
                        flag_serie_data = CheckSerieDataNullOrNot(serieY_leiji);
                        if (flag_serie_data)
                        {
                            series_Y_leiji.Add(serieY_leiji);
                        }
                    }
                    //测斜累计位移趋势图
                    const string template_leiji = "template/deep_displacement.json";
                    flag = checkSeries(series_X_leiji);
                    if (!flag)
                    {
                        //Console.WriteLine("开始绘制图表..测斜X方向累计位移\r\n");
                        var yAxisX_leiji = string.Format("位移(mm)");
                        temp_x_leiji =
                            DrawChart(new GeneralLineChart(template_leiji, series_X_leiji, groupName + "管道X方向累计位移",
                                yAxisX_leiji));
                        if (temp_x_leiji != null)
                        {
                            var stream = new StreamChart();
                            stream.ChartStream = temp_x_leiji;
                            stream.ChartType = "CexieLeiji";
                            tempStream.Add(stream);
                           
                        }
                        //Console.WriteLine("测斜X方向累计位移绘制图表完成!\r\n");
                    }
                    else
                    {
                        //Console.WriteLine("测斜X方向累计位移数据列为空..{0}\r\n", groupName);
                        logger.Warn(string.Format("测斜X方向累计位移数据列为空..{0}\r\n", groupName));
                    }

                    flag = checkSeries(series_Y_leiji);
                    if (!flag)
                    {
                        //Console.WriteLine("开始绘制图表..测斜Y方向累计位移\r\n");
                        var yAxisY_leiji = string.Format("位移(mm)");
                        temp_y_leiji =
                            DrawChart(new GeneralLineChart(template_leiji, series_Y_leiji, groupName + "管道Y方向累计位移",
                                yAxisY_leiji));
                        //Console.WriteLine("测斜Y方向累计位移绘制图表完成!\r\n");
                        if (temp_y_leiji != null)
                        {
                            var stream = new StreamChart();
                            stream.ChartStream = temp_y_leiji;
                            stream.ChartType = "CexieLeiji";
                            tempStream.Add(stream);
                            
                        }
                    }
                    else
                    {
                        //Console.WriteLine("测斜Y方向累计位移数据列为空..{0}\r\n", groupName);
                        logger.Warn(string.Format("测斜Y方向累计位移数据列为空..{0}\r\n", groupName));
                    }
                    #endregion
                }
            }
            return tempStream;
        }

        public List<StreamChart> DrawRainFallChart(int structId, int factorId, DateTime start, DateTime end)
        {
            var tempStream = new List<StreamChart>();
            const string template = "template/column.json";
            var factorName = DataAccess.GetFactorInfoById(factorId).NameCN;
            List<SensorList> sensorList = DataAccess.FindSensorsByStructAndFactor(structId, factorId);
            Stream temp = null;
            if (sensorList == null || sensorList.Count == 0)
            {
                //Console.WriteLine("传感器列表获取失败..雨量\r\n");
                logger.Warn("传感器列表获取失败..雨量\r\n");
                return null;
            }
            if (sensorList.Count > 0)
            {
                var sensors = sensorList.SelectMany(list => list.Sensors).ToList();
                var series = new List<Serie>();

                for (int i = 0; i < sensors.Count; i++)
                {
                    var data = DataAccess.GetMonitorData(new[] { sensors[i].SensorId }, start, end);
                    Serie serie = new Serie();
                    serie.Name = sensors[i].Location;
                    if (data == null || data.Count == 0)
                    {
                        //Console.WriteLine("获取数据失败...雨量\r\n");
                        logger.Warn("获取数据失败..雨量\r\n");
                        continue;
                    }
                    if (data.Count > 0)
                    {
                        //Console.WriteLine("获取数据成功...雨量\r\n");
                        var points = new double[data[0].Data.Count][];
                        for (int j = 0; j < data[0].Data.Count; j++)
                        {
                            points[j] = new[]
                                {
                                (data[0].Data[j].AcquisitionTime - new DateTime(1970, 1, 1)).TotalMilliseconds,
                                (double) (data[0].Data[j].Values[0])
                                };
                        }
                        serie.Data = points;
                    }
                    //存储数据列
                    bool flag_serie_data = true;
                    flag_serie_data = CheckSerieDataNullOrNot(serie);
                    if (flag_serie_data)
                    {
                        series.Add(serie);
                    }
                }

                bool flag = false;
                flag = checkSeries(series);

                if (!flag)
                {
                    var yAxis = string.Format("雨量(mm)");
                    //Console.WriteLine("开始绘制图表..雨量\r\n");
                    temp = DrawChart(new GeneralLineChart(template, series, factorName + "趋势图", yAxis));
                    if (temp != null)
                    {
                        var stream = new StreamChart();
                        stream.ChartStream = temp;
                        stream.ChartType = "General";
                        tempStream.Add(stream);
                        //Console.WriteLine("雨量绘图完成");
                    }
                }
                else
                {
                    //Console.WriteLine("数据列为空..雨量\r\n");
                    logger.Warn("数据列为空..雨量\r\n");
                    return null;
                }

            }
            return tempStream;
        }

        public List<StreamChart> DrawTempHumiChart(int structId, int factorId, DateTime start, DateTime end)
        {
            var tempStream = new List<StreamChart>();
            const string template = "template/doubleAxis.json";
            var factorName = DataAccess.GetFactorInfoById(factorId).NameCN;
            List<SensorList> sensorList = DataAccess.FindSensorsByStructAndFactor(structId, factorId);
            if (sensorList == null || sensorList.Count == 0)
            {
                //Console.WriteLine("获取传感器列表失败...温湿度\r\n");
                logger.Warn("获取传感器列表失败...温湿度\r\n");
                return null;
            }
            if (sensorList.Count > 0)
            {
                Stream temp = null;
                var sensors = sensorList.SelectMany(list => list.Sensors).ToList();
                for (int i = 0; i < sensors.Count; i++)
                {
                    var seriesData = new List<SerieDoubleAxis>();
                    var twoAxis = new JArray();
                    var data = DataAccess.GetMonitorData(new[] { sensors[i].SensorId }, start, end);
                    if (data == null || data.Count == 0)
                    {
                        //Console.WriteLine("获取数据失败 ...{0}:{1}\r\n", factorName, sensors[i].Location);
                        logger.Warn(string.Format("获取数据失败 ...{0}:{1}\r\n", factorName, sensors[i].Location));
                        continue;
                    }
                    if (data.Count > 0)
                    {
                        //Console.WriteLine("获取数据成功 ...{0}:{1}\r\n", factorName, sensors[i].Location);

                        twoAxis = doubleAxis(data);
                        seriesData = Series(data);

                    }
                    if (seriesData != null && seriesData.Count != 0)
                    {
                        //Console.WriteLine("开始绘制图表..{0}\r\n", factorName);
                        temp = DrawChart(new GeneralLineChart(template, seriesData, factorName + "趋势图", twoAxis));
                        if (temp != null)
                        {
                            var stream = new StreamChart();
                            stream.ChartStream = temp;
                            stream.ChartType = "General";
                            tempStream.Add(stream);
                            //Console.WriteLine("{0}..绘图完成", factorName);
                        }
                    }
                    else
                    {
                        //Console.WriteLine("数据列为空..{0}\r\n", factorName);
                        logger.Warn(string.Format("数据列为空..{0}\r\n", factorName));
                    }
                }
            }

            return tempStream;
        }

        public List<StreamChart> DrawGeneralChart(int structId, int factorId, DateTime start, DateTime end)
        {

            var tempStream = new List<StreamChart>();
            const string template = "template/line.json";
            var factorName = DataAccess.GetFactorInfoById(factorId).NameCN;
            List<SensorList> sensorList = DataAccess.FindSensorsByStructAndFactor(structId, factorId);
            if (sensorList == null || sensorList.Count == 0)
            {
                //Console.WriteLine("获取传感器列表失败...{0}\r\n",factorName);
                logger.Warn(string.Format("获取传感器列表失败..{0}\r\n", factorName));
                return null;
            }
            if (sensorList.Count > 0)
            {
                Stream temp = null;
                var sensors= sensorList.SelectMany(list => list.Sensors).ToList();
                var series = new List<Serie>();
                var yAxis = "";
                for (int i = 0; i < sensors.Count; i++)
                {
                    Serie serie = new Serie();
                    var data = DataAccess.GetMonitorData(new[] { sensors[i].SensorId }, start, end);
                    logger.Debug("获取数据完成" + factorName);
                    if (data == null || data.Count == 0)
                    {
                        //Console.WriteLine("获取数据失败 ...{0}:{1}\r\n", factorName, sensors[i].Location);
                        logger.Warn(string.Format("获取数据失败 ...{0}:{1}\r\n", factorName, sensors[i].Location));
                        continue;
                    }
                    if (data.Count > 0)
                    {
                        //Console.WriteLine("获取数据成功 ...{0}:{1}\r\n", factorName, sensors[i].Location);
                        var yAxis_temp = string.Format("{0}({1})", data[0].Columns[0], data[0].Unit[0]);
                        var points = new double[data[0].Data.Count][];
                        for (int j = 0; j < data[0].Data.Count; j++)
                        {
                            points[j] = new[]
                                {
                                (data[0].Data[j].AcquisitionTime - new DateTime(1970, 1, 1)).TotalMilliseconds,
                                (double) (data[0].Data[j].Values[0])

                                };
                        }
                        serie.Name = sensors[i].Location;
                        serie.Data = points;
                        yAxis = yAxis_temp;
                    }
                   //存储数据列
                    bool flag_serie_data = true;
                    flag_serie_data = CheckSerieDataNullOrNot(serie);
                    if (flag_serie_data)
                    {
                        series.Add(serie);
                    }
                }

                bool flag = false;
                flag = checkSeries(series);

                if (!flag)
                {
                    //Console.WriteLine("开始绘制图表..{0}\r\n", factorName);
                    temp = DrawChart(new GeneralLineChart(template, series, factorName + "趋势图", yAxis));
                    if (temp != null)
                    {
                        var stream = new StreamChart();
                        stream.ChartStream = temp;
                        stream.ChartType = "General";
                        tempStream.Add(stream);
                        //Console.WriteLine("{0}..绘图完成", factorName);
                    }
                }
                else
                {
                    //Console.WriteLine("数据列为空..{0}\r\n", factorName);
                    logger.Warn(string.Format("数据列为空..{0}\r\n", factorName));
                }
            }

            logger.Debug("返回数据完成" + factorName);
            return tempStream;
        }
        public List<StreamChart> DrawBorderChart(int structId, int factorId, DateTime start, DateTime end)
        {

            var tempStream = new List<StreamChart>();
            const string template = "template/border.json";
            var factorName = DataAccess.GetFactorInfoById(factorId).NameCN;
            List<SensorList> sensorList = DataAccess.FindSensorsByStructAndFactor(structId, factorId);
            if (sensorList == null || sensorList.Count == 0)
            {
                logger.Warn(string.Format("获取传感器列表失败..{0}\r\n", factorName));
                return null;
            }
            if (sensorList.Count > 0)
            {
                Stream temp = null;
                var sensors = sensorList.SelectMany(list => list.Sensors).ToList();
                //量程处理
                decimal max = decimal.MinValue;
                decimal min = decimal.MaxValue;
                var series = new List<Serie>();
                var yAxis = "";
                for (int i = 0; i < sensors.Count; i++)
                {
                    Serie serie = new Serie();
                    var data = DataAccess.GetMonitorData(new[] { sensors[i].SensorId }, start, end);
                    logger.Debug("获取数据完成" + factorName);
                    if (data == null || data.Count == 0)
                    {
                        //Console.WriteLine("获取数据失败 ...{0}:{1}\r\n", factorName, sensors[i].Location);
                        logger.Warn(string.Format("获取数据失败 ...{0}:{1}\r\n", factorName, sensors[i].Location));
                        continue;
                    }
                    if (data.Count > 0)
                    {
                        //Console.WriteLine("获取数据成功 ...{0}:{1}\r\n", factorName, sensors[i].Location);
                        var yAxis_temp = string.Format("{0}({1})", data[0].Columns[0], data[0].Unit[0]);
                        var points = new double[data[0].Data.Count][];
                        for (int j = 0; j < data[0].Data.Count; j++)
                        {
                            max = data[0].Data[j].Values[0] > max ? data[0].Data[j].Values[0] : max;
                            min = data[0].Data[j].Values[0] < min ? data[0].Data[j].Values[0] : min;

                            points[j] = new[]
                                {
                                (data[0].Data[j].AcquisitionTime - new DateTime(1970, 1, 1)).TotalMilliseconds,
                                (double) (data[0].Data[j].Values[0])

                                };
                        }
                        serie.Name = sensors[i].Location;
                        serie.Data = points;
                        yAxis = yAxis_temp;
                    }
                    //存储数据列
                    bool flag_serie_data = true;
                    flag_serie_data = CheckSerieDataNullOrNot(serie);
                    if (flag_serie_data)
                    {
                        series.Add(serie);
                    }
                }

                bool flag = false;
                flag = checkSeries(series);

                if (!flag)
                {
                    var diff = max - min;
                    var total = diff * 4;
                    var half = total / 2;
                    max = max + half;
                    min = min - half;
                    //Console.WriteLine("开始绘制图表..{0}\r\n", factorName);
                    temp = DrawChart(new GeneralLineChart(template, series, factorName + "趋势图", yAxis,Convert.ToString(max),Convert.ToString(min)));
                    if (temp != null)
                    {
                        var stream = new StreamChart();
                        stream.ChartStream = temp;
                        stream.ChartType = "General";
                        tempStream.Add(stream);
                        //Console.WriteLine("{0}..绘图完成", factorName);
                    }
                }
                else
                {
                    //Console.WriteLine("数据列为空..{0}\r\n", factorName);
                    logger.Warn(string.Format("数据列为空..{0}\r\n", factorName));
                }
            }

            logger.Debug("返回数据完成" + factorName);
            return tempStream;
        }
        private Stream DrawChart(Chart chart)
        {
            string[] console;
            try
            {
                Stream stream = chart.Draw(out console);
                string info = string.Join("\n", console);
                logger.Debug(info);
                //Console.WriteLine(info);
                return stream;
            }
            catch (Exception e)
            {
                logger.Warn("绘图出现异常：" + e.Message);
            }
            return null;
        }

        //验证传感器下的监测数据是否为空
        public bool CheckSerieDataNullOrNot(Serie serie)
        {
            bool flag = true;
            if (serie == null || serie.Data == null || serie.Data.Length == 0)
            {
                logger.Debug(string.Format("{0}..数据列为空，不绘制曲线", serie.Name));
                flag = false;
            }
            return flag;
        }

        //验证监测因素下的所有传感器监测数据是否都为空
        public bool checkSeries(List<Serie>  series)
        {
            int Num = 0;
            bool flag = false;
            for (int i = 0; i < series.Count(); i++)
            {
                if (series[i] == null || series[i].Data == null || series[i].Data.Length == 0)
                {
                    Num++;
                }
            }
            if (Num == series.Count)
            {
                flag = true;
            }           
            return flag;
        }

        //拼接highchar中的双坐标轴yAxis
        public JArray doubleAxis(List<MonitorData> data)
        {
            var axisData = new JArray();
            var columns = data[0].Columns;
            var unit = data[0].Unit;
            if (columns.Count == 2)
            {
                for (var i = 0; i < columns.Count; i++)
                {


                    var color = "";
                    var a = "";
                    if ((i + 1) % 2 == 0)
                    {
                        color = "#89A54E";
                        a = "right";
                    }
                    else
                    {
                        color = "#4572A7";
                        a = "left";
                    }
                    var labelarry = new JObject(
                        new JProperty("align", a),
                        new JProperty("x", 3),
                        new JProperty("y", 16),
                        new JProperty("style", new JObject(new JProperty("color", color)))
                        );

                    var titlearry = new JObject(
                        new JProperty("text", columns[i] + '(' + unit[i] + ')'),
                        new JProperty("style", new JObject(new JProperty("color", color)))
                        );
                    JObject axisValue = new JObject();
                    //湿度范围在0-100
                    if (unit[i] == "%RH")
                    {
                        if ((i + 1) % 2 == 0)
                        {
                            JObject temp_axisValue = new JObject(
                                new JProperty("min", 0),
                                new JProperty("max", 100),
                                new JProperty("labels", labelarry),
                                new JProperty("title", titlearry),
                                new JProperty("opposite", true),
                                new JProperty("showFirstLabel", false)
                                );
                            axisValue = temp_axisValue;

                        }
                        else
                        {
                            JObject temp_axisValue = new JObject(
                                 new JProperty("min", 0),
                                 new JProperty("max", 100),
                                 new JProperty("labels", labelarry),
                                 new JProperty("title", titlearry),
                                 new JProperty("showFirstLabel", false)
                                 );
                            axisValue = temp_axisValue;
                        }
                    }
                    else
                    {
                        if ((i + 1) % 2 == 0)
                        {
                            JObject temp_axisValue = new JObject(
                                 new JProperty("labels", labelarry),
                                 new JProperty("title", titlearry),
                                 new JProperty("opposite", true),
                                 new JProperty("showFirstLabel", false)
                                 );
                            axisValue = temp_axisValue;
                        }
                        else
                        {
                            JObject temp_axisValue = new JObject(
                                 new JProperty("labels", labelarry),
                                 new JProperty("title", titlearry),
                                 new JProperty("showFirstLabel", false)
                                 );
                            axisValue = temp_axisValue;
                        }
                    }

                    axisData.Add(axisValue);
                }
            }
            return axisData;
        }

        //拼接highchar中的双坐标轴series
        public List<SerieDoubleAxis> Series(List<MonitorData> data)
        {
            var seriesData = new List<SerieDoubleAxis>();

            var columns = data[0].Columns;
            var unit = data[0].Unit;
            var dataValue = data[0].Data;

            for (var i = 0; i < columns.Count; i++)
            {
                var location = data[0].Location;
                var array = new double[data[0].Data.Count][];
                var color = "";
                var yAxis = 0;
                if ((i + 1) % 2 == 0)
                {
                    color = "#89A54E";
                    yAxis = 1;
                }
                else
                {
                    color = "#4572A7";
                    yAxis = 0;
                }
                //拼 data
                for (var j = 0; j < dataValue.Count; j++)
                {
                    array[j] = new[]
                    {
                        (data[0].Data[j].AcquisitionTime - new DateTime(1970, 1, 1)).TotalMilliseconds,
                        (double) (data[0].Data[j].Values[i])
                    };
                }
                SerieDoubleAxis seriesValue = new SerieDoubleAxis();
                seriesValue.Name = columns[i];
                seriesValue.Color = color;
                seriesValue.Type = "spline";
                seriesValue.YAxis = yAxis;
                seriesValue.Data = array;
                if (array == null || array.Length == 0)
                {
                    logger.Debug(string.Format("{0}..数据列为空", location));
                }
                else
                {
                    seriesData.Add(seriesValue);
                }
            }
            return seriesData;
        }

    }

    public enum ChartType
    {
        普通趋势图 = 1,
        测斜累计趋势图 = 2,
    }

}
