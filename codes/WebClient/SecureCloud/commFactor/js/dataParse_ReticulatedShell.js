function displayThresholdData(sensorId, thresholdValue, chart, itemId,paramK) {
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
                                if (paramK == 1) {
                                    $('#' + key + thresholdValue[j].Level + 'Threshold2').html("无");
                                } else {
                                    $('#' + key + thresholdValue[j].Level + 'Threshold').html("无");
                                }
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

                                var thresholdStart = v[0].substring(1);
                                if (thresholdStart == '-') {
                                    thresholdStart = min - (max - min) * 0.1;
                                } else {
                                    thresholdStart = parseFloat(thresholdStart);
                                }
                                var thresholdEnd = v[1].substring(0, v[1].length - 1);
                                if (thresholdEnd == '+') {
                                    thresholdEnd = max + (max - min) * 0.1;
                                } else {
                                    thresholdEnd = parseFloat(thresholdEnd);
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
    var unit = data[0].unit
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
function getMaxMinValue(location, data, k, unit) {

    var min = "";
    var max = "";
    var Xmin = "";
    var Xmax = "";
    var Ymin = "";
    var Ymax = "";
    var Zmin = "";
    var Zmax = "";

    if (data.length == 0) {
        if (k == 0) {
            $('#XsensorLocation').html(location);
        } else {
            $('#XsensorLocation2').html(location);
        }
        return;
    }

    min = data[0].Speed;
    max = data[0].Speed;
    for (var j = 0; j < data.length; j++) {

        if (min == null) {
            min = data[j].Speed;
        }
        if (max == null) {
            max = data[j].Speed;
        }

        if (data[j].Speed != null) {
            if (data[j].Speed < min) {
                min = data[j].Speed;
            }
            if (data[j].Speed > max) {
                max = data[j].Speed;
            }
        }
    }
    if (k == 0) {
        $('#XmaxValue').html("");
        $('#XminValue').html("");
        $('#XsensorLocation').html("");
        if (max == null) {
            $('#XmaxValue').html("");
        } else {
            $('#XmaxValue').html(max.toFixed(4) + unit);
        }
        if (min == null) {
            $('#XminValue').html("");
        } else {
            $('#XminValue').html(min.toFixed(4) + unit);
        }
        $('#XsensorLocation').html(location);
    } else {
        $('#XmaxValue2').html("");
        $('#XminValue2').html("");
        $('#XsensorLocation2').html("");
        if (max == null) {
            $('#XmaxValue2').html("");
        } else {
            $('#XmaxValue2').html(max.toFixed(4) + unit);
        }
        if (min == null) {
            $('#XminValue2').html("");
        } else {
            $('#XminValue2').html(min.toFixed(4) + unit);
        }
        $('#XsensorLocation2').html(location);
    }
    if (max == null || min == null) {
        return { "max": [""], "min": [""] };

    } else if (max == null && min != null) {
        return { "max": [""], "min": [min.toFixed(4)] };

    } else if (max != null && min != null) {
        return { "max": [max.toFixed(4)], "min": [min.toFixed(4)] };

    } else {
        return { "max": [max.toFixed(4)], "min": [""] };
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
function getSensorWarn(location, sensorList, Startdate, Enddate, k) {
    $('#sensorLink').show();
    //告警等级越小，告警越严重
    var href = "";
    href = "../DataWarningTest.aspx?sensorId=" + sensorList + "&sensorLocation=" + location + "&startTime=" + Startdate + "&endTime=" + Enddate;

    if (k == 0) {
        $('#XsensorLink').click(function () {
            window.parent.location.href = href;
        });
    } else {
        $('#XsensorLink2').click(function () {
            window.parent.location.href = href;
        });
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



