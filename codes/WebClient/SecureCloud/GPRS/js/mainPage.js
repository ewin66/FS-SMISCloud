/**
 * ---------------------------------------------------------------------------------
 * <copyright file="mainPage.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2014 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述：无线采集传感器数据查询js文件
 *
 * 创建标识：PengLing20140909
 *
 * 修改标识：
 * 修改描述：
 * </summary>
 * ---------------------------------------------------------------------------------
 */

var sensorList = [];
var currentSensorType;

var chartOptions =
{
    chart: {
        type: 'spline'
    },
    title: {
        text: '',
        x: -20 //center
    },
    subtitle: {
        text: '',
        x: -20
    },
    xAxis: {
        type: 'datetime',
        dateTimeLabelFormats: {
            //second: '%H:%M:%S',
            minute: '%H:%M:%S',
            //hour: '%H:%M:%S',
            day: '%Y-%m-%d',
            month: '%b %y',
            //year: '%Y-%m-%d'
        },
        labels: {
            rotation: -25,
            align: 'right',
            style: {
                font: 'normal 13px Verdana, sans-serif'
            }
        }
    },
    yAxis: {
        labels: {
            align: 'left',
            x: 3,
            y: 16,
            formatter: function () {
                return Highcharts.numberFormat(this.value, 0);
            }
        },
        showFirstLabel: false
    },
    credits: {
        enabled: false
        //href: 'http://www.f-song.com',
        //text: '江西飞尚科技有限公司'
    },
    plotOptions: {
        spline: {
            lineWidth: 1.5,
            states: {
                hover: {
                    lineWidth: 1.5
                }
            },
            marker: {
                enabled: false
            }
            //pointInterval: 3600000, // one hour
            //pointStart: Date.UTC(2013, 9, 27, 0, 0, 0)
        }
    },

    tooltip: {
        //shared: true,
        //crosshairs: true
    },
    legend: {
        layout: 'vertical',
        align: 'right',
        verticalAlign: 'middle',
        borderWidth: 0
    },
    series: []
};


$(function () {
    $("#main-page").addClass('active');
    // highcharts本地化时间，汉化下载条目
    Highcharts.setOptions({
        global: {
            useUTC: false //关闭UTC       
        },
        lang: {
            downloadJPEG: '下载JPEG图片',
            downloadPDF: '下载PDF图片',
            downloadPNG: '下载PNG图片',
            downloadSVG: '下载SVG图片',
            printChart: '打印图片'
        }
    });
    // 收缩/展开列表数据
    $('a.box-collapse').click(function () {
        var $target = $(this).parent().parent().next('.box-content');
        if ($target.is(':visible')) {
            $('img', this).attr('src', '../resource/img/toggle-expand.png');
        } else {
            $('img', this).attr('src', '../resource/img/toggle-collapse.png');
        }
        $target.slideToggle();
    });

    showSensorType();
});

function showSensorType() {   
    var structId = getCookie("gprsStructId");
    // 获取传感器类型
    var url = apiurl + '/struct/' + structId + '/sensorType' + '?token=' + getCookie("token");   
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            if (data.length == 0) {
                alert("无传感器类型");
                return;
            }
            try {
                var sb = new StringBuffer();
                $.each(data, function (index, item) {
                    sb.append('<li id="sensorType-' + index + '" class="sensorType"><a href="javascript:showSensor(' + index + ');">' + item.sensorType + '</a></li>');

                    var tempSensorList = [];
                    $.each(item.sensors, function (indexSensors, itemSensors) {
                        tempSensorList.push({
                            sensorId: itemSensors.sensorId,
                            sensorLocation: itemSensors.location
                        });
                    });
                    sensorList.push({
                        sensorType: item.sensorType,
                        sensors: tempSensorList
                    });
                });               
                
                $('#sensor-type').html(sb.toString());

                $('#sensor-list').selectpicker('refresh');
                
                showSensor(0); // 默认显示第一个传感器类型的第一个option
            } catch (err) {
                alert(err);
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取传感器类型时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function showSensor(index) {
    // $('.sensorType').attr("style", "");
    // $("#sensorType-" + index).attr("style", "background: gray;"); // 设置菜单栏选择状态
    // $('#main-page ul li:first-child').addClass('active');
    $('.sensorType').removeClass('active');
    $("#sensorType-" + index).addClass('active');
    
    $('#expand_collapse').attr('src', '../resource/img/toggle-collapse.png'); // 默认显示“收缩”图片

    currentSensorType = sensorList[index].sensorType; // 保存当前传感器类型
    // 以下两种传感器类型只有一项原始值
    if (currentSensorType == "压差式变形测量传感器" || currentSensorType == "LVDT裂缝计") {
        $('#chart-original-1').hide();
    } else {
        $('#chart-original-1').show();
    }
        
    var sb = new StringBuffer();
    $.each(sensorList, function (i, item) {
        if (i == index) {
            $.each(item.sensors, function (j, itemSensors) {
                sb.append('<option id="' + itemSensors.sensorId + '" value="option-' + j + '">' + itemSensors.sensorLocation + '</option>');
            });
        }
    });

    $('#sensor-list').html(sb.toString());    
      
    $('#sensor-list').selectpicker('refresh');
    $('#sensor-list').selectpicker('val', 'option-0'); // 默认显示第一个option 
    $('#date').selectpicker('refresh'); // 默认显示近24小时
    $('#date').selectpicker('val', 'day');
    
    // “传感器数据”主页显示当前传感器类型下第一个传感器在近24小时的数据
    var sensor = $('#sensor-list').find('option:selected');
    var currentSensorId = parseInt(sensor[0].id);
    var startTime = convertMillisecondsToDateTime_milliseconds(new Date() - 24 * 60 * 60 * 1000);
    var endTime = convertMillisecondsToDateTime_milliseconds(new Date().valueOf());
    loadChart(currentSensorId, startTime, endTime);
}

$("#btnQuery").click(function () {
    var arrSensorId = [];
    var arrSensorLocation = [];
    var sensor = $('#sensor-list').find('option:selected');
    for (var i = 0; i < sensor.length; i++) {
        arrSensorId[i] = parseInt(sensor[i].id); // 传感器id数组
        arrSensorLocation[i] = sensor[i].innerText;
    }

    var arrTime = $('#date').find('option:selected');
    var timeInterval = arrTime[0].value;

    if (arrSensorId.length == 0) {
        alert("请选择传感器");
    } else {
        constructUrlParam(arrSensorId, timeInterval);
    }
});

function constructUrlParam(arrSensorId, timeInterval) {
    var startTime, endTime;
    switch (timeInterval) {
        case "day":
            startTime = convertMillisecondsToDateTime_milliseconds(new Date() - 24 * 60 * 60 * 1000);
            endTime = convertMillisecondsToDateTime_milliseconds(new Date().valueOf());
            break;
        case "week":
            startTime = convertMillisecondsToDateTime_milliseconds(new Date() - 7 * 24 * 60 * 60 * 1000);
            endTime = convertMillisecondsToDateTime_milliseconds(new Date().valueOf());
            break;
        case "month":
            startTime = convertMillisecondsToDateTime_milliseconds(new Date() - 30 * 24 * 60 * 60 * 1000);
            endTime = convertMillisecondsToDateTime_milliseconds(new Date().valueOf());
            break;
        case "other":
            startTime = getStartdate();
            endTime = getEnddate();
            break;
        default:
            startTime = convertMillisecondsToDateTime_milliseconds(new Date() - 24 * 60 * 60 * 1000);
            endTime = convertMillisecondsToDateTime_milliseconds(new Date().valueOf());
            break;
    }

    var sensorIds = "";
    if (arrSensorId.length == 1) {
        sensorIds = arrSensorId[0];
    } else {
        for (var i = 0; i < arrSensorId.length; i++) {
            if (i == 0) {
                sensorIds = arrSensorId[i];
            } else {
                sensorIds += ',' + arrSensorId[i];
            }
        }
    }

    loadChart(sensorIds, startTime, endTime);
}

function loadChart(sensorIds, startTime, endTime) {
    var url = apiurl + '/gprs/sensor/' + sensorIds + '/data/' + startTime + '/' + endTime + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data.length == 0) {
                // alert("传感器无数据");
                // var chart = $('#chart-original-0').highcharts();
                // chart.destroy();
                $('#chart-original').hide();
                $('#box-table').hide();
                $('#tipNoData-sensor').show();
                return;
            }
            try {
                if (data[0].columns.originalColumn.length == 1) { // 一项原始值
                    $('#tipNoData-sensor').hide();
                    $('#chart-original').show();
                    $('#chart-original-0').show();
                    $('#chart-original-1').hide();
                    $('#box-table').show();

                    var chartOptionsOriginal = chartOptions; // 存放图形选项
                    var arrayTableData = []; // 存放表格数据

                    // 仅一个趋势图
                    chartOptionsOriginal.title.text = currentSensorType + '趋势图'; // highcharts标题
                    chartOptionsOriginal.yAxis.title = { text: data[0].columns.originalColumn[0] + '(' + data[0].units.originalUnit[0] + ')' };
                    chartOptionsOriginal.series = []; // 清空数据
                    $.each(data, function (index, item) {
                        var array = [];
                        for (var j = 0; j < item.data.length; j++) {
                            var time = item.data[j].acquisitiontime.substring(6, 19);
                            array[j] = [parseInt(time), item.data[j].originalValue[0]];

                            // 填充table数据
                            var arrayCurrentData = [];
                            arrayCurrentData.push(item.location);
                            arrayCurrentData.push(item.data[j].originalValue[0]);
                            // 物理量值
                            arrayCurrentData.push(item.data[j].calculatedValue[0]);
                            arrayCurrentData.push(time);
                            arrayTableData.push(arrayCurrentData);
                        }
                        chartOptionsOriginal.series.push({ name: item.location, data: array });
                        // tooltip
                        chartOptionsOriginal.tooltip.formatter = function () {
                            var chartTooltip = '<b>' + this.series.name + '</b>';
                            chartTooltip = chartTooltip + '<br />采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                            chartTooltip = chartTooltip + '<br />' + '监测数据:' + this.y.toString() + '<b>' + item.units.originalUnit[0] + '</b>';
                            return chartTooltip;
                        };
                    });
                    $('#chart-original-0').highcharts(chartOptionsOriginal);

                    // 表头
                    var arrayTableHead = [];
                    arrayTableHead.push('设备位置');
                    arrayTableHead.push(data[0].columns.originalColumn[0] + "(" + data[0].units.originalUnit[0] + ")");
                    arrayTableHead.push('物理量值(' + data[0].units.calculatedUnit[0] + ')');
                    arrayTableHead.push('采集时间');
                    var timeIndex = arrayTableHead.length - 1; // 采集时间序列号
                    // 生成数据表格
                    tableManager('table-sensor', arrayTableData, arrayTableHead, timeIndex);
                } else if (data[0].columns.originalColumn.length == 2) { // 两项原始值
                    $('#tipNoData-sensor').hide();
                    $('#chart-original').show();
                    $('#chart-original-0').show();
                    $('#chart-original-1').show();
                    $('#box-table').show();

                    var chartOptionsOriginal = chartOptions; // 存放图形选项
                    var arrayTableData = []; // 存放表格数据

                    // 第一个趋势图
                    chartOptionsOriginal.title.text = currentSensorType + '趋势图'; // highcharts标题
                    chartOptionsOriginal.yAxis.title = { text: data[0].columns.originalColumn[0] + '(' + data[0].units.originalUnit[0] + ')' };
                    chartOptionsOriginal.series = []; // 清空数据
                    $.each(data, function (index, item) {
                        var array = [];
                        for (var j = 0; j < item.data.length; j++) {
                            var time = item.data[j].acquisitiontime.substring(6, 19);
                            array[j] = [parseInt(time), item.data[j].originalValue[0]];
                        }
                        chartOptionsOriginal.series.push({ name: item.location, data: array });
                        //// shared tooltip
                        //chartOptionsOriginal.tooltip.formatter = function () {
                        //    var chartTooltip = '<span>' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x) + '</span>';
                        //    for (var i = 0; i < this.points.length; i++) {
                        //        chartTooltip += '<br /><b>' + this.points[i].series.name + '：' + '</b><span>' + this.points[i].y + item.units.originalUnit[0] + '</span>';
                        //    }
                        //    return chartTooltip;
                        //};
                        chartOptionsOriginal.tooltip.formatter = function () {
                            var chartTooltip = '<b>' + this.series.name + '</b>';
                            chartTooltip = chartTooltip + '<br />采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                            chartTooltip = chartTooltip + '<br />' + '监测数据:' + this.y.toString() + '<b>' + item.units.originalUnit[0] + '</b>';
                            return chartTooltip;
                        };                     
                    });
                    $('#chart-original-0').highcharts(chartOptionsOriginal);
                    
                    // 第二个趋势图
                    chartOptionsOriginal.title.text = currentSensorType + '趋势图'; // highcharts标题
                    chartOptionsOriginal.yAxis.title = { text: data[0].columns.originalColumn[1] + '(' + data[0].units.originalUnit[1] + ')' }; 
                    chartOptionsOriginal.series = []; // 清空数据
                    $.each(data, function (index, item) {
                        var array = [];
                        for (var j = 0; j < item.data.length; j++) {
                            var time = item.data[j].acquisitiontime.substring(6, 19);
                            array[j] = [parseInt(time), item.data[j].originalValue[1]];

                            // 填充table数据
                            var arrayCurrentData = [];
                            arrayCurrentData.push(item.location);
                            for (var k = 0; k < item.data[j].originalValue.length; k++) {
                                arrayCurrentData.push(item.data[j].originalValue[k]);
                            }
                            // 物理量值
                            for (var k = 0; k < item.data[j].calculatedValue.length; k++) {
                                if (item.columns.calculatedColumn[k] == "X方向累积位移") { // 不显示“X方向累积位移，Y方向累积位移”
                                    break;
                                }
                                arrayCurrentData.push(item.data[j].calculatedValue[k]);
                            }
                            arrayCurrentData.push(time);
                            arrayTableData.push(arrayCurrentData);
                        }
                        chartOptionsOriginal.series.push({ name: item.location, data: array });
                        //// shared tooltip
                        //chartOptionsOriginal.tooltip.formatter = function () {
                        //    var chartTooltip = '<span>' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x) + '</span>';
                        //    for (var i = 0; i < this.points.length; i++) {
                        //        chartTooltip += '<br /><b>' + this.points[i].series.name + '：' + '</b><span>' + this.points[i].y + item.units.originalUnit[1] + '</span>';
                        //    }
                        //    return chartTooltip;
                        //};
                        chartOptionsOriginal.tooltip.formatter = function () {
                            var chartTooltip = '<b>' + this.series.name + '</b>';
                            chartTooltip = chartTooltip + '<br />采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                            chartTooltip = chartTooltip + '<br />' + '监测数据:' + this.y.toString() + '<b>' + item.units.originalUnit[1] + '</b>';
                            return chartTooltip;
                        };
                    });
                    $('#chart-original-1').highcharts(chartOptionsOriginal);
                    
                    // 表头
                    var arrayTableHead = [];
                    arrayTableHead.push('设备位置');
                    // 原始值对应的列名
                    for (var i = 0; i < data[0].columns.originalColumn.length; i++) {
                        arrayTableHead.push(data[0].columns.originalColumn[i] + "(" + data[0].units.originalUnit[i] + ")");
                    }
                    // 计算后值对应的列名
                    if (data[0].columns.calculatedColumn.length == 1) {
                        arrayTableHead.push('物理量值(' + data[0].units.calculatedUnit[0] + ')');
                    } else {
                        for (var i = 0; i < data[0].columns.calculatedColumn.length; i++) {
                            if (data[0].columns.calculatedColumn[i] == "X方向累积位移") { // 不显示“X方向累积位移，Y方向累积位移”
                                break;
                            }
                            arrayTableHead.push(data[0].columns.calculatedColumn[i] + "(" + data[0].units.calculatedUnit[i] + ")");
                        }
                    }
                    // 时间列
                    arrayTableHead.push('采集时间');
                    var timeIndex = arrayTableHead.length - 1; // 采集时间序列号
                    // 生成数据表格
                    tableManager('table-sensor', arrayTableData, arrayTableHead, timeIndex);
                }                
            } catch (err) {
                alert(err);
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取传感器数据时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}