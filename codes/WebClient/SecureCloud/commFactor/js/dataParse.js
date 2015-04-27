function displayThresholdData(sensorId, thresholdValue, chart, itemId) {
    var data_values_threshold = [];
    var key;
    var item;
    if (itemId == null) {
        key = "";
        item = 1;
    } else if (itemId == 1) {
        item = itemId;
        key = "X";
    } else if (itemId == 2) {
        item = itemId;
        key = "Y";
    } else if (itemId == 3) {
        item = itemId;
        key = "Z";
    }

    var url = apiurl + '/sensor/' + sensorId + '/threshold?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        async : false,
        success: function (data) {
            

            var thresholdMap = { "first": 1, "second": 2, "third": 3, "fourth": 4 };
            var thresholdData;
            var i;
            for (i = 0; i < data.length; i++) {
                if (data[i].itemId == item) {
                    thresholdData = data[i].threshold;
                    break;
                }
            }

            for (var j = 0; j < thresholdValue.length; j++) {
                for (var k = 0; k < thresholdData.length; k++) {
                    if (thresholdMap[thresholdValue[j].Level] == thresholdData[k].level) {
                        try {
                            var value = thresholdData[k].value.split(';');
                            if (value == "") {
                                $('#'+ key + thresholdValue[j].Level + 'Threshold').html("无");
                                continue;
                            }
                            var color = "rgba(0, 165, 0, 0.1)"; //绿色;
                            switch (thresholdData[k].level) {
                            case 1:
                                color = "rgba(255, 0, 0, 0.1)";
                                break; //红色
                            case 2:
                                color = "rgba(255, 165, 0, 0.1)";
                                break; //橙色
                            case 3:
                                color = "#E6E6FA";
                                break; //紫色
                            case 4:
                                color = "#F0FFFF";
                                break; //蓝色
                            }
                            $('#' + key + thresholdValue[j].Level + 'Threshold').html('<div>' + value + '</div>');
                            $('#' + key + thresholdValue[j].Level + 'Threshold > div').css('background-color', color);
                            var min = parseFloat($('#'+key+'minValue').html()); // 最小
                            var max = parseFloat($('#'+key+'maxValue').html()); // 最大
                            for (i = 0; i < value.length; i++) {
                                var v = value[i].split(',');
                                var tStart = v[0].substring(1), tEnd = v[1].substring(0, v[1].length - 1);
                                var thresholdStart, thresholdEnd;
                                if (tStart == '+' || tEnd == '-') {
                                    continue;
                                }
                                if (tStart == '-') { // 下限为负无穷
                                    var yMin = chart.yAxis[0].min;
                                    if (yMin == null) {
                                        yMin = min - (max - min) * 0.1;
                                        if (yMin > tEnd) {
                                            yMin = tEnd - (max - min) * 0.1;
                                        }
                                    }
                                    thresholdStart = yMin;
                                } else {
                                    thresholdStart = parseFloat(tStart);
                                }

                                if (tEnd == '+') { // 上限为正无穷
                                    var yMax = chart.yAxis[0].max;
                                    if (yMax == null) {
                                        yMax = max + (max - min) * 0.1;
                                        if (yMax < tStart) {
                                            yMax = tStart + (max - min) * 0.1;
                                        }
                                    }
                                    thresholdEnd = yMax;
                                } else {
                                    thresholdEnd = parseFloat(tEnd);
                                }

                                
                                var valuesThreshold = [thresholdValue[j].Level, thresholdStart, thresholdEnd];
                                data_values_threshold.push(valuesThreshold);
                            }
                        } catch (e) {
                        }
                    }
                }
            }
            // 显示阈值带
            var bands = thresholdBands(data_values_threshold);
            // chart.yAxis.plotBands = bands;
            try {
                chart.yAxis[0].removePlotBand('first');
                chart.yAxis[0].removePlotBand('second');
                chart.yAxis[0].removePlotBand('third');
                chart.yAxis[0].removePlotBand('fourth');

                for (var i = 0; i < bands.length; i++) {
                    chart.yAxis[0].addPlotBand(bands[i]);
                }

            } catch (e) {
            }
        },
        error: function () {
            for (var i = 0; i < 4; i++) {
                $('#' + thresholdValue[i].Level + 'Threshold').html("无");
            }
            alertTips('阈值获取失败, 请尝试重新查询', 'label-warning', 'tip', 5000);
        }
    });
}

//拼接highchar中的plotBands
function thresholdBands(cdata) {

    var thresholdPlotBands = [];

    for (var i = 0; i < cdata.length; i++) {
        var colorband = "";
        switch (cdata[i][0]) {
            case "first": colorband = "rgba(255, 0, 0, 0.1)"; break;//红色
            case "second": colorband = "rgba(255, 165, 0, 0.1)"; break;//橙色
            case "third": colorband = "#E6E6FA"; break;//紫色
            case "fourth": colorband = "#F0FFFF"; break;//蓝色
        }
        var labelarry = [];

        labelarry = {
            "text": cdata[i][0],
            "style": { "color": "#606060" }
        };

        var thresholdBands = {
            "from": cdata[i][1],
            "to": cdata[i][2],
            "color": colorband,
            "id": cdata[i][0],
        };
        thresholdPlotBands.push(thresholdBands);
    }
    return thresholdPlotBands;
}

//拼接highchar中的双坐标轴yAxis
function twoAxis(data) {
    var axisData = [];
    var columns = data[0].columns;
    var unit = data[0].unit;
    if (columns.length == 2) {
        for (var i = 0; i < columns.length; i++) {
            var labelarry = [];
            var titlearry = [];
            var color = '';
            var axisValue = [];
            var a = '';
            if ((i + 1) % 2 == 0) {
                color = "#89A54E";
                a = "left";
            }
            else {
                color = "#4572A7";
                a = "right";
            }

            labelarry = {
                "align": a,
                "x": 3,
                "y": 16,
                //"formatter": function () { return Highcharts.numberFormat(this.value, 0); },
                "formatter": function () {
                    return this.value;
                },
                //"format":this.value+unit[i],
                "style": { "color": color },
            };

            titlearry = {
                "text": columns[i] + '(' + unit[i] + ')',
                "style": { "color": color }
            };
            //湿度范围在0-100
            if (unit[i] == "%RH") {
                if ((i + 1) % 2 == 0) {
                    axisValue = {
                        "min": 0,
                        "max": 100,
                        "labels": labelarry,
                        "title": titlearry,
                        "opposite": true,
                        "showFirstLabel": false
                    };
                }
                else {
                    axisValue = {
                        "min": 0,
                        "max": 100,
                        "labels": labelarry,
                        "title": titlearry,
                        "showFirstLabel": false
                    };
                }
            }
            else {
                if ((i + 1) % 2 == 0) {
                    axisValue = {                     
                        "labels": labelarry,
                        "title": titlearry,
                        "opposite": true,
                        "showFirstLabel": false
                    };
                }
                else {
                    axisValue = {                      
                        "labels": labelarry,
                        "title": titlearry,
                        "showFirstLabel": false
                    };
                }
            }
            
            axisData.push(axisValue);
        }
        return axisData;
    }
    else { }

}

//拼接highchar中的双坐标轴series
function series(data) {
    var seriesData = [];
    var seriesValue = [];
    var columns = data[0].columns;
    var unit = data[0].unit;
    var dataValue = data[0].data;
    var data_table_values = [];
    var tableValues = [];
    var dataSeries = [];

    for (var i = 0; i < columns.length; i++) {
        var location = data[0].location;
        var unitComm = unit[i];
        var array = new Array();
        var color = '';
        var axisValue = [];
        var yAxis = "";
        if ((i + 1) % 2 == 0) {
            color = "#89A54E";
            yAxis = 1;
        }
        else {
            color = "#4572A7";
            yAxis = 0;
        }
        //拼 data
        for (var j = 0; j < dataValue.length; j++) {
            var time = dataValue[j].acquisitiontime.substring(6, 19);
            if (dataValue[j].value[i] != null) {
                array.push([parseInt(time), dataValue[j].value[i]]);
            }
        }
        seriesValue = {
            "name": columns[i],
            "color": color,
            "type": "spline",
            "yAxis": yAxis,
            "data": array,
            "tooltip": { "valueSuffix": unit[i] }
        };
        seriesData.push(seriesValue);

    }
    for (var j = 0; j < dataValue.length; j++) {
        var time = dataValue[j].acquisitiontime.substring(6, 19);
        if (dataValue[j].value[0] != null || dataValue[j].value[1] != null) {
            var value_table = [location, dataValue[j].value[0], dataValue[j].value[1], time];
        }
        data_table_values.push(value_table);
    }
    return {
        tableValues: data_table_values,
        dataSeries: seriesData
    }

}

//获取采集最大值、最小值、传感器位置
function getMaxMinValue(data, index) {
    if (index == null) {
        index = 0;
    }
    var min = "";
    var max = "";
    var Xmin = "";
    var Xmax = "";
    var Ymin = "";
    var Ymax = "";
    var Zmin = "";
    var Zmax = "";
    var columnsLength = data[index].columns.length;
    var sensorLocation = data[index].location;

    if (columnsLength == 1) {
        if (data[index].data.length == 0) {
            $('#XsensorLocation').html(sensorLocation);
            return;
        }

        var unit = data[index].unit[0];

        min = data[index].data[0].value[0];
        max = data[index].data[0].value[0];
        for (var j = 0; j < data[index].data.length; j++) {

            if (min == null) {
                min = data[index].data[j].value[0];
            }
            if (max == null) {
                max = data[index].data[j].value[0];
            }

            if (data[index].data[j].value[0] != null) {
                if (data[index].data[j].value[0] < min) {
                    min = data[index].data[j].value[0];
                }
                if (data[index].data[j].value[0] > max) {
                    max = data[index].data[j].value[0];
                }
            }
        }

        if (index == 0) {
            $('#XmaxValue').html("");
            $('#XminValue').html("");
            $('#XsensorLocation').html("");
            $('#XmaxValue').html(max + unit);
            $('#XminValue').html(min + unit);
            $('#XsensorLocation').html(sensorLocation);
        }

        return { "max": [max], "min": [min] };
    } else {
        var Xunit = data[0].unit[0];
        var Yunit = data[0].unit[1];
        var Zunit = data[0].unit[2];
        Xmin = data[index].data[0].value[0];
        Xmax = data[index].data[0].value[0];
        Ymin = data[index].data[0].value[1];
        Ymax = data[index].data[0].value[1];
        Zmin = data[index].data[0].value[2];
        Zmax = data[index].data[0].value[2];
        for (var j = 0; j < data[index].data.length; j++) {

            if (min == null) {
                Xmin = data[index].data[j].value[0];
                Ymin = data[index].data[j].value[1];
                Zmin = data[index].data[j].value[2];
            }
            if (max == null) {
                Xmax = data[index].data[j].value[0];
                Ymax = data[index].data[j].value[1];
                Zmax = data[index].data[j].value[2];
            }

            if (data[index].data[j].value[0] != null) {
                if (data[index].data[j].value[0] < Xmin) {
                    Xmin = data[index].data[j].value[0];
                }
                if (data[index].data[j].value[0] > Xmax) {
                    Xmax = data[index].data[j].value[0];
                }
            }

            if (data[index].data[j].value[1] != null) {
                if (data[index].data[j].value[1] < Ymin) {
                    Ymin = data[index].data[j].value[1];
                }
                if (data[index].data[j].value[1] > Ymax) {
                    Ymax = data[index].data[j].value[1];
                }
            }

            if (data[index].data[j].value[2]) {
                if (data[index].data[j].value[2] < Zmin) {
                    Zmin = data[index].data[j].value[2];
                }
                if (data[index].data[j].value[2] > Zmax) {
                    Zmax = data[index].data[j].value[2];
                }
            }
        }
        if (index == 0) {
            $('#XmaxValue').html("");
            $('#XminValue').html("");
            $('#YmaxValue').html("");
            $('#YminValue').html("");

            $('#XsensorLocation').html("");
            $('#YsensorLocation').html("");

            $('#XmaxValue').html(Xmax + Xunit);
            $('#XminValue').html(Xmin + Xunit);
            $('#YmaxValue').html(Ymax + Yunit);
            $('#YminValue').html(Ymin + Yunit);
            $('#XsensorLocation').html(sensorLocation);
            $('#YsensorLocation').html(sensorLocation);
        }
        if (columnsLength == 3) {
            if (index == 0) {
                $('#ZmaxValue').html("");
                $('#ZminValue').html("");
                $('#ZsensorLocation').html("");

                $('#ZmaxValue').html(Zmax + Zunit);
                $('#ZminValue').html(Zmin + Zunit);

                $('#ZsensorLocation').html(sensorLocation);
            }
            return { "max": [Xmax, Ymax, Zmax], "min": [Xmin, Ymin, Zmin] };
        }

        return { "max": [Xmax, Ymax], "min": [Xmin, Ymin] };
    }
}

function SetChartRange(chart, max, min) {
    var diff = max - min;
    var total = diff * 4;
    var half = total / 2;

    chart.yAxis.max = max + half;
    chart.yAxis.min = min - half;
}

//获得传感器告警
function getSensorWarn(sensorData, sensorList,sensorLocation, Startdate, Enddate) {
    $('#sensorLink').show();
    //告警等级越小，告警越严重
    var href = "../DataWarningTest.aspx?sensorId=" + sensorList + "&sensorLocation=" + encodeURIComponent(sensorLocation) + "&startTime=" + Startdate + "&endTime=" + Enddate;
    var columnsLength = sensorData[0].columns.length;
    if (columnsLength == 1) {
        $('#XsensorLink').click(function() {
            top.location.href = href;
        });
    } else {
        $('#XsensorLink').click(function() {
            top.location.href = href;
        });
        $('#YsensorLink').click(function() {
            top.location.href = href;
        });
        if (columnsLength == 3) {            
            $('#ZsensorLink').click(function() {
                top.location.href = href;
            });
        }
    }
}

//获得传感器告警
function SensorWarn(sensorData, sensorList,sensorLocation, Startdate, Enddate, isDoubleAxis) {
    var src = "";
    var href = "";
    href = "../DataWarningTest.aspx?sensorId=" + sensorList +"&sensorLocation="+sensorLocation+ "&startTime=" + Startdate + "&endTime=" + Enddate;
    var columnsLength = sensorData[0].columns.length;
    if (columnsLength == 1 || isDoubleAxis) {
        $('#XsensorLink').click(function () {
            window.parent.location.href = href;
        });
    } else {
        $('#XsensorLink').click(function () {
            window.parent.location.href = href;
        });
        $('#YsensorLink').click(function () {
            window.parent.location.href = href;
        });
        if (columnsLength == 3) {
            $('#ZsensorLink').click(function () {
                window.parent.location.href = href;
            });
        }
    }

}


//获取采集最大值、最小值、传感器位置
function MaxMinValue(data) {
    var Xmin = "";
    var Xmax = "";
    var Ymin = "";
    var Ymax = "";
    var sensorLocation = data[0].location;

    var Xunit = data[0].unit[0];
    var Yunit = data[0].unit[1];
    for (var i = 0; i < data.length; i++) {
        Xmin = data[i].data[0].value[0];
        Xmax = data[i].data[0].value[0];
        Ymin = data[i].data[0].value[1];
        Ymax = data[i].data[0].value[1];
        for (var j = 0; j < data[i].data.length; j++) {
            if (Xmin == null) {
                Xmin = data[i].data[j].value[0];
            }
            if (Ymin == null) {
                Ymin = data[i].data[j].value[1];
            }
            if (Xmax == null) {
                Xmax = data[i].data[j].value[0];
            }
            if (Ymax == null) {
                Ymax = data[i].data[j].value[1];
            }
            if (data[i].data[j].value[0] != null) {
                if (data[i].data[j].value[0] < Xmin) {
                    Xmin = data[i].data[j].value[0];
                }
                if (data[i].data[j].value[0] > Xmax) {
                    Xmax = data[i].data[j].value[0];
                }
            }
            if (data[i].data[j].value[1] != null) {
                if (data[i].data[j].value[1] < Ymin) {
                    Ymin = data[i].data[j].value[1];
                }
                if (data[i].data[j].value[1] > Ymax) {
                    Ymax = data[i].data[j].value[1];
                }
            }
        }
    }
    $('#maxValue1').html("");
    $('#minValue1').html("");
    $('#maxValue2').html("");
    $('#minValue2').html("");
    $('#sensorLocation').html("");

    $('#maxValue1').html(Xmax + Xunit);
    $('#minValue1').html(Xmin + Xunit);
    $('#maxValue2').html(Ymax + Yunit);
    $('#minValue2').html(Ymin + Yunit);
    $('#sensorLocation').html(sensorLocation);

}



