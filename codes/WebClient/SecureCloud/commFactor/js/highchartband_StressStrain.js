
$(function () {
    Highcharts.setOptions({
        global: {
            useUTC: false //关闭UTC       
        },
        lang: {
            printChart: "打印",
            downloadJPEG: "下载JPEG 图片",
            downloadPDF: "下载PDF文档",
            downloadPNG: "下载PNG 图片",
            downloadSVG: "下载SVG 矢量图",
            exportButtonTitle: "导出图片"
        }
    });
});

function createHighchartComm1(renderTo, title, itemId, renderTOIndex) {
    var template = {
        chart: {
            type: 'spline',
            renderTo: renderTo,
            zoomType: 'x',
            animation: false
        },
        title: {
            text: title,
            x: -20 //center
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
            //categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            labels: { rotation: -25, align: 'right', style: { font: 'normal 13px Verdana, sans-serif' } }
        },
        yAxis: {
            title: {},
            plotLines: [{
                value: 0,
                width: 2,
                dashStyle: 'Dash',
                zIndex: 5
            }],
            //min: 0,
            minorGridLineWidth: 0,
            gridLineWidth: 0,
            alternateGridColor: null,

            // plotBands: plotbands
        },
        credits: {
            enabled: false
            //href: 'http://www.f-song.com',
            //text: '江西飞尚科技有限公司'
        },
        plotOptions: {
            series: {
                events: {
                    //鼠标移上去表格值变化
                    mouseOver: function () {
                        //由传感器位置查找传感器id号
                        var thisSensorId = "";
                        var thisLocation = this.name;
                        var factorId = document.getElementById('HiddenFactorNo').value;
                        var sensorUrl = apiurl + '/struct/' + structid + '/factor/' + factorId + '/sensors?token=' + getCookie('token');
                        var currentChart = this.chart;

                        var sensorId = $('#sensorList').find('option:selected');
                        var sensorList = [];
                        for (var i = 0; i < sensorId.length; i++) {
                            sensorList[i] = sensorId[i].value;
                        }
                        if (renderTo == 'chart_x') {
                            getSensorData(sensorList[0], thisLocation, currentChart, renderTOIndex, 1);//1判断应力图形

                        } else {
                            sensorUrl = apiurl + "/combinedSensors/" + sensorList + "/info?token=" + getCookie('token');
                            $.ajax({
                                url: sensorUrl,
                                type: 'get',

                                success: function (data) {
                                    if (data.length == 0) {
                                        return;
                                    }
                                    else {

                                        for (var j = 0; j < data.length; j++) {
                                            if (thisLocation == data[j].SENSOR_LOCATION_DESCRIPTION) {
                                                thisSensorId = data[j].CorrentSensorId;
                                                break;
                                            }
                                        }
                                        //传感器采集告警信息、最大值、最小值
                                        getSensorData(thisSensorId, thisLocation, currentChart, renderTOIndex, 2);//2判断应变图形

                                    }
                                }
                            });
                        }

                    }
                }
            },
            spline: {
                lineWidth: 1.5,
                states: {
                    hover: {
                        lineWidth: 2.5,
                    }
                },
                marker: {
                    enabled: false
                }
            }
        },
        tooltip: {
            //shared: true,
            //crosshairs: true,
            formatter: {}
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle',
            borderWidth: 0
        },
        series: []
    };

    return template;
}
function getInteval() {

    var divNum = 1000 * 3600 * 24;
    var startTimeArray = $("#dpform");
    var endTimeArray = $("#dpdend");

    var startTime = startTimeArray[0].value;
    var endTime = endTimeArray[0].value;

    startTime = startTime.replace(/-/g, "/");
    endTime = endTime.replace(/-/g, "/");

    var sTime = new Date(startTime);
    var eTime = new Date(endTime);
    var a = parseInt((eTime.getTime() - sTime.getTime()) / parseInt(divNum));

    return a;
}
function getSensorData(thisSensorId, thisLocation, chart, itemId, k) {
    //var interval = (Date.parse(getEnddate().replace(/-/ig, '/')) - Date.parse(getStartdate().replace(/-/ig, '/'))) / 1000 / 60 / 60 / 24;
    var interval = getInteval();

    if (interval <= 6) {
        var url = apiurl + "/sensor/" + thisSensorId + "/data/" + getStartdate() + '/' + getEnddate() + '?token=' + getCookie('token');
    }
    else if (interval <= 30) {
        var url = apiurl + "/sensor/" + thisSensorId + "/data/" + getStartdate() + '/' + getEnddate() + '/' + '1/minute?token=' + getCookie('token');
    }
    else if (interval <= 90) {
        var url = apiurl + "/sensor/" + thisSensorId + "/data/" + getStartdate() + '/' + getEnddate() + '/' + '1/hour?token=' + getCookie('token');
    }
    else if (interval <= 365) {
        var url = apiurl + "/sensor/" + thisSensorId + "/data/" + getStartdate() + '/' + getEnddate() + '/' + '12/hour?token=' + getCookie('token');
    }
    else {
        var url = apiurl + "/sensor/" + thisSensorId + "/data/" + getStartdate() + '/' + getEnddate() + '/' + '1/day?token=' + getCookie('token');
    }
    $.ajax({
        url: url,
        type: 'get',

        //timeout: 5000,
        success: function (data) {
            if (data.length == 0) {
                return;
            }
            else {
                //获取告警信息（告警）
                getSensorWarn(data, thisSensorId, thisLocation, getStartdate(), getEnddate());
                //加载告警图表(最大值、最小值)
                getMaxMinValue(data, k);//k判断哪个图形

                var thresholdValue = [{ "Level": "first" }, { "Level": "second" }, { "Level": "third" }, { "Level": "fourth" }];
                displayThresholdData(thisSensorId, thresholdValue, chart, itemId);
            }
        }

    });
}