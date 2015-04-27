
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

function createHighchartComm1(renderTo, title, doubleAxis, seriesData, factorIdSend) {
    var template = {
        chart: {
            type: 'spline',
            renderTo: renderTo,
            zoomType: 'x'
        },
        title: {
            text: title,
            x: -20 //center
        },
        //subtitle: {
        //    text: '来源：江西飞尚科技有限公司',
        //    x: -20
        //},
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
        yAxis: doubleAxis,
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
                        if (factorIdSend == undefined || factorIdSend == null || factorId == "") {
                            var factorId = document.getElementById('HiddenFactorNo').value;
                        }
                        else {
                            var factorId = factorIdSend;
                        }
                        if (factorId != "5") {
                            var sensorUrl = apiurl + '/struct/' + getCookie("nowStructId") + '/factor/' + factorId + '/sensors?token=' + getCookie('token');
                            $.ajax({
                                url: sensorUrl,
                                type: 'get',

                                success: function (data) {
                                    if (data.length == 0) {
                                        return;
                                    }
                                    else {
                                        for (var i = 0; i < data.length; i++) {
                                            var sensors = data[i].sensors;
                                            for (var j = 0; j < sensors.length; j++) {
                                                if (thisLocation == sensors[j].location) {
                                                    thisSensorId = sensors[j].sensorId;
                                                    break;
                                                }
                                            }
                                        }
                                        //传感器采集告警信息、最大值、最小值
                                        getSensorData(thisSensorId, thislocation);
                                    }
                                }
                            })
                        }
                    }
                }
            },
            spline: {
                lineWidth: 1.5,
                states: {
                    hover: {
                        lineWidth: 2.5
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
        series: seriesData
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
function getSensorData(thisSensorId, thislocation) {
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
                SensorWarn(data, thisSensorId, thislocation, getStartdate(), getEnddate(), true);
                //加载告警图表(最大值、最小值)
                MaxMinValue(data);
            }
        }

    });
}