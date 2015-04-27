var structid = getCookie("nowStructId");

var thresholdValue = [{ "Level": "first" }, { "Level": "second" }, { "Level": "third" }, { "Level": "fourth" }];

var data_table_values = [];//列表

//highcharts变量
var comm_chart = "";
var chart = "";
var comm_chart_rt = "";
var chart_rt = "";
var globalChartOri;
var globalChart;
var globalRtChartOri;
var globalRtChart;
var rtTable;
var rtTableData = [];
var rtTableDataV = [];//1方向
var rtTableDataH = [];//2方向

var global_Vibration_Rt_Sensor;
var global_Vibration_Rt_Timer;
var global_Vibration_Rt_Interval = 5000;
var global_Vibration_Rt_Running = false;

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
    var factorid = document.getElementById('HiddenFactorNo').value;

    $('#monitorList').addClass('active');

    setTimeout(function () {
        $('#factor_' + factorid).addClass('active');
    }, 2000);

    getSensorList();
    getSensorListRt();

    $('a.box-collapse').click(function () {
        var $target = $(this).parent().parent().next('.box-content');
        if ($target.is(':visible')) {
            $('img', this).attr('src', '../resource/img/toggle-expand.png');
        } else {
            $('img', this).attr('src', '../resource/img/toggle-collapse.png');
        }
        $target.slideToggle();
    });
    //dateValue($('#date').val());//重新判断时间选择

    $('#date').change(function () {
        var sensorId = parseInt($('#sensorList').val());
        vibrationData(sensorId);
        //getTimeList(sensorId);
    });

    $('#btnQuery').click(function () {
        dateValue($('#date').val());//重新获取起始时间
        var sensorId = parseInt($('#sensorList').val());
        vibrationData(sensorId);
        //getTimeList(sensorId);
    });

    $('#chkShowTable').change(function () {
        if (this.checked) {
            $('#rt_table').show();
        } else {
            $('#rt_table').hide();
        }
    });

    $('#tabRt').click(function () {
        $('#sensorListRt').selectpicker('refresh');
        var sensorId = parseInt($('#sensorListRt').val());
        global_Vibration_Rt_Sensor = sensorId;
        //getRtData(true);
        vibrationRtData(true, global_Vibration_Rt_Sensor);

        clearInterval(global_Vibration_Rt_Timer);
        global_Vibration_Rt_Timer = setInterval(function () {
            if (!global_Vibration_Rt_Running) {
                //getRtData(false);
                vibrationRtData(false, global_Vibration_Rt_Sensor);

            }
        }, global_Vibration_Rt_Interval);
    });

    $('#tabHistory').click(function () {
        clearInterval(global_Vibration_Rt_Timer);
    });
});

// 图形为空提示，带传感器参数
function errorTipParam(string_id, str) {
    var graph_id = string_id.split(',');
    var errorTipstring = '<div id=\'error\' class=\'row-fluid dataerror-tip\'>' +
        '<div class="span3">' +
        '<span class=\'label label-important\' style=\'margin-left: 5px;margin-top: 10px;\'>抱歉，' + str + '没有查询到任何有效的数据</span>' +
        '</div>' +
        '</div>';
    for (var i = 0; i < graph_id.length; i++) {
        $('#' + graph_id[i]).append(errorTipstring);
    }
}

// 图形显示控制 /频谱
function display(value, k) {//无数据block
    if (value == "block") {
        if (doubleDirectionFlag == 0) {
            $('#comm2_error').hide();
            $('#comm2').hide();
            $('#comm1').hide();
            $('#comm1_error').hide();

        } else {
            if (k == 0) {
                $('#comm1').hide();
                $('#comm1_error').hide();
            } else {
                $('#comm2').hide();
                $('#comm2_error').hide();
            }
        }
    }
    else {
        if (doubleDirectionFlag == 0) {
            $('#comm2_error').hide();
            $('#comm2').hide();

            $('#comm1_error').hide();
            $('#comm1').show();
            
        } else {
            if (k == 0) {
                $('#comm1_error').hide();
                $('#comm1').show();
            } else {
                $('#comm2_error').hide();
                $('#comm2').show();
            }
        }
    }
}
//时域
function displayOri(value, k) {//无数据block
    if (value == "block") {
        if (doubleDirectionFlag == 0) {
            $('#comm2_ori').hide();
            $('#comm2_ori_error').hide();
            $('#warnBox2').hide();

            $('#comm1_ori').hide();
            $('#comm1_ori_error').show();
            $('#warnBox').hide();
            try {
                $('#data_tablesshow_table').dataTable().fnDestroy();
            } catch (e) {
            }
            $('#show_table').children().remove();
        } else {
            if (k == 0) {
                $('#comm1_ori').hide();
                $('#comm1_ori_error').show();
                $('#warnBox').hide();
                try {
                    $('#data_tablesshow_table').dataTable().fnDestroy();
                } catch (e) {
                }
                $('#show_table').children().remove();

            } else {
                $('#comm2_ori').hide();
                $('#comm2_ori_error').show();
                $('#warnBox2').hide();
            }

        }
    }
    else {
        if (doubleDirectionFlag == 0) {
            $('#comm2_ori').hide();
            $('#comm2_ori_error').hide();
            $('#warnBox2').hide();

            $('#comm1_ori').show();
            $('#comm1_ori_error').hide();
            $('#warnBox').show();
        } else {
            if (k == 0) {
                $('#comm1_ori').show();
                $('#comm1_ori_error').hide();
                $('#warnBox').show();

            } else {
                $('#comm2_ori').show();
                $('#comm2_ori_error').hide();
                $('#warnBox2').show();
            }
        }
    }
}

// 实时数据图形控制 /频谱
function displayRt(value, k) {
    if (value == "block") {
        if (doubleDirectionRtFlag == 0) {
            $('#rt').hide();
            $('#rt_error').hide();

            $('#rt2').hide();
            $('#rt_error2').hide();
        } else {
            if (k == 0) {
                $('#rt').hide();
                $('#rt_error').hide();
            } else {
                $('#rt2').hide();
                $('#rt_error2').hide();
            }
        }
    }
    else {
        if (doubleDirectionRtFlag == 0) {
            $('#rt').show();
            $('#rt_error').hide();
            $('#rt2').hide();
            $('#rt_error2').hide();
        } else {
            if (k == 0) {
                $('#rt').show();
                $('#rt_error').hide();
            } else {
                $('#rt2').show();
                $('#rt_error2').hide();
            }
        }
    }
}
//时域
function displayRtOri(value,k) {
    if (value == "block") {
        if (doubleDirectionRtFlag == 0) {
            $('#rt_ori').hide();
            $('#rt_ori1_error').show();
            if (rtTable != null) {
                rtTable.fnDestroy();
            }
            $('#rt_table').children().remove();
            $('#txtRtBatch').val('');

            $('#rt_ori2').hide();
            $('#rt_ori2_error').hide();

        } else {
            if (k == 0) {
                $('#rt_ori').hide();
                $('#rt_ori1_error').show();
                if (rtTable != null) {
                    rtTable.fnDestroy();
                }
                $('#rt_table').children().remove();
            } else {
                $('#rt_ori2').hide();
                $('#rt_ori2_error').show();
            }
        }
    }
    else {
        if (doubleDirectionRtFlag == 0) {
            $('#rt_ori1_error').hide();
            $('#rt_ori').show();

            $('#rt_ori2').hide();
            $('#rt_ori2_error').hide();
        } else {
            if (k == 0) {
                $('#rt_ori1_error').hide();
                $('#rt_ori').show();
            } else {
                $('#rt_ori2').show();
                $('#rt_ori2_error').hide();
            }
        }
    }
}

/*   历史数据   */
function getSensorList() {
    $('#show_table').children().remove();

    var factorId = document.getElementById('HiddenFactorNo').value;
    var url = apiurl + '/struct/' + structid + '/factor/' + factorId + '/sensors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            if (data.length == 0) {
                $('#sensorList').selectpicker('refresh');
                return;
            }
            var option = '';
            //索力下面有两种sensortype
            for (var i = 0; i < data.length; i++) {
                for (var index = 0; index < data[i].sensors.length; index++) {
                    option += '<option value=\'' + data[i].sensors[index].sensorId + '\'>' + data[i].sensors[index].location + '</option>';
                }
            }
            $('#sensorList').html(option);
            $('#sensorList').selectpicker('refresh');
            var defaultSensor = document.getElementById('HiddenSensorId').value;
            var defaultSensorID = "";
            if (defaultSensor == 0) {
                defaultSensorID = '';
            }
            else {
                defaultSensorID = defaultSensor;
            }
            if (defaultSensorID == '' || defaultSensorID == null || defaultSensorID == undefined) {
                defaultSensorID = data[0].sensors[0].sensorId;
                $('#sensorList').selectpicker('val', defaultSensorID);
            }
            else {
                $('#sensorList').selectpicker('val', parseInt(defaultSensorID));
            }


            vibrationData(defaultSensorID);
            // getTimeList(defaultSensorID);
            //loadChart(thresholdValue);

            $('#sensorList').unbind('change').change(function () {
                $('#sensorList').selectpicker('refresh');
                var sensorId = parseInt($(this).val());
                vibrationData(sensorId);
                //getTimeList(sensorId);
            });


        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else {
                alertTips('传感器列表获取失败, 请尝试刷新页面', 'label-important', 'tip', 5000);
            }
        }
    });
}

function getTimeList(sensorId, k) {
    $('#show_table').children().remove();
    //获取一段时间内的触发振动的时间点
    var url = apiurl + '/vibrationRShell/' + sensorId + '/data-batch/' + getStartdate() + '/' + getEnddate() + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                displayCom();//提示信息
                $('#TimeList').html("");
                $('#TimeList').selectpicker('refresh');
                return;
            } else {
                //加载数据
                var sb = new StringBuffer();
                for (var i = data.length - 1; i >= 0; i--) {
                    sb.append('<option value=\'' + i + '\'>' + JsonToDateTime(data[i].CollectTime) + '</option>');
                }
                $('#TimeList').html(sb.toString());
                $('#TimeList').selectpicker('refresh');

                getMonitorData(sensorId, JsonToDateTime(data[data.length - 1].CollectTime));

                $('#TimeList').unbind('change').change(function () {
                    $('#TimeList').selectpicker('refresh');
                    var index = parseInt($(this).val());
                    getMonitorData(sensorId, JsonToDateTime(data[index].CollectTime));
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                displayOnly();//error提示信息
                alert("登录超时,请重新登录");
                logOut();
            } else {
                $('#TimeList').selectpicker('refresh');
                alertTips('数据获取失败, 请尝试重新查询', 'label-important', 'tip', 5000);
            }
        }
    });
}
var flagCount = 0;
function getMonitorData(sensorList, timeList) {
    $('#show_table').children().remove();
    //由时间点获取触发振动的数据    
    //时域数据
    var url = apiurl + '/vibration-RShell/' + timeList + '/' + sensorList + '/original-data' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        //async: false,
        success: function (data) {
            data_table_values = [];
            if (data == null || data.length == 0) {
                displayCom();//提示信息
                createDataTable('show_table', data_table_values, ['设备位置', '加速度(m/s²)', '采集时间']);
                return;
            } else {
                getSpectrum(timeList, sensorList);///频谱
               
                if (doubleDirectionFlag == 1) {
                    if (data.length > 1) {
                        displayOri('none', 0);//时域
                        displayOri('none', 1);//时域
                    } else {
                        displayOri('block', 1);//时域
                        $("#comm2_ori_error").html("");
                        errorTipParam("comm2_ori_error", "另一方向");
                    }
                } else {
                    if (data.length > 0) {
                        displayOri('none', 0);//时域
                    } else {
                        displayOri('block', 0);//时域
                        $("#comm1_ori_error").html("");
                        errorTipParam("comm1_ori_error", "");
                    }
                }
                for (var i = 0; i < data.length; i++) {
                    if (data[i].data.length > 0) {
                        displayOri('none', i);//时域
                    } else {
                        displayOri('block', i);//时域
                        $("#comm" + (i + 1) + "_ori_error").html("");
                        errorTipParam("comm" + (i + 1) + "_ori_error", data[i].Location + "方向");
                    }
                    var location = data[i].Location;
                    var array = new Array();
                    getSensorWarn(location, data[i].SensorId, timeList, timeList, i);
                    getMaxMinValue(location, data[i].data, i, data[i].Unit);

                    for (var j = 0; j < data[i].data.length; j++) {
                        var time = data[i].data[j].CollectTime;
                        var timeStr = ConvertJsonToDateStr(data[i].data[j].CollectTime);
                        if (data[i].data[j].Speed == null) {
                            array[j] = [time, null];
                            var value_table = [location, '无', timeStr];
                            data_table_values.push(value_table);
                        } else {
                            array[j] = [time, parseFloat(data[i].data[j].Speed.toFixed(4))];
                            var value_table = [location, parseFloat(data[i].data[j].Speed.toFixed(4)), timeStr];
                            data_table_values.push(value_table);
                        }

                    }
                    if (i == 0) {
                        comm_chart = createHighchartTemplate('comm1_ori', location + '振动时域图');
                    } else {
                        comm_chart = createHighchartTemplate('comm2_ori', location + '振动时域图');
                    }
                    comm_chart.xAxis = {
                        title: { text: '采集时间' },
                        type: 'datetime',
                        dateTimeLabelFormats: {
                            second: '%H:%M:%S',
                        },
                        labels: { rotation: -25, align: 'right', style: { font: 'normal 13px Verdana, sans-serif' } }
                    };
                    comm_chart.yAxis.title = { text: '加速度(m/s²)' };
                    comm_chart.tooltip.formatter = function () {
                        var tooltipString = '<b>' + this.series.name + '</b>';

                        tooltipString = tooltipString + '<br/><br/>时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                        tooltipString = tooltipString + '<br/>' + '加速度:' + this.y.toString() + '<b>m/s²</b>';

                        return tooltipString;
                    };
                    comm_chart.series.push({ name: location, data: array });
                    globalChartOri = new Highcharts.Chart(comm_chart);
                    displayThresholdData(data[i].SensorId, thresholdValue, comm_chart, 1, i);
                }
                createDataTable('show_table', data_table_values, ['设备位置', '加速度(m/s²)', '采集时间']);

            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                displayOnly();//error提示信息
                alert("登录超时,请重新登录");
                logOut();
            } else {
                $('#TimeList').selectpicker('refresh');
                alertTips('数据获取失败, 请尝试重新查询', 'label-important', 'tip', 5000);
            }
        }
    });
}

function getSpectrum(timeList, sensorList) {
    var url = apiurl + '/vibration-RShell/' + timeList + '/' + sensorList + '/spectrum-data' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                display('block', 0);//频谱
                display('block', 1);//频谱
                return;
            } else {
                if (data.length > 1) {
                    display('none', 0);
                    display('none', 1);
                } else {
                    display('block', 1);
                }
                var array = new Array();
                for (var i = 0; i < data.length; i++) {
                    array = [];
                    var location = data[i].Location;
                    if (data[i].data.length > 0) {
                        display('none', i);
                    } else {
                        display('block', i);//时域
                    }
                    for (var j = 0; j < data[i].data.length; j++) {
                        var freq = "";
                        var valueP = "";
                        if (data[i].data[j].Frequency == null) {
                            freq = null;
                        } else {
                            freq = parseFloat(data[i].data[j].Frequency.toFixed(2));
                        }
                        if (data[i].data[j].Value == null) {
                            valueP = null;
                        } else {
                            valueP = parseFloat(data[i].data[j].Value.toFixed(4));
                        }
                        array[j] = [freq, valueP];
                    }
                    globalChart = null;
                    if (i == 0) {
                        chart = createHighchartTemplate('comm1',location+ '振动频谱图', null, 1);
                    } else {
                        chart = createHighchartTemplate('comm2', location+'振动频谱图', null, 1);
                    }

                    chart.series = [];
                    var unit = "m/s²";
                    chart.xAxis = {
                        title: { text: '频率 (Hz)' },
                        tickInterval: 50,
                        lineWidth: 2
                    };
                    chart.yAxis.title = { text: '加速度(' + unit + ')' };
                    chart.tooltip.formatter = function () {
                        var tooltipString = '<b>' + this.series.name + '</b>';
                        tooltipString = tooltipString + '<br/><br/>频率:' + this.x.toString() + 'Hz';
                        tooltipString = tooltipString + '<br/>' + '加速度:' + this.y.toString() + '<b>' + unit + '</b>';

                        return tooltipString;
                    };
                    chart.series.push({ name: location, data: array });
                    globalChart = new Highcharts.Chart(chart);
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                display('block',0);
                alert("登录超时,请重新登录");
                logOut();
            } else {
                $('#TimeList').selectpicker('refresh');
                alertTips('无频谱数据', 'label-important', 'comm1_error', 5000);
            }
        }
    });
}

/*   实时数据   */
function getSensorListRt() {
    $('#rt_table').children().remove();

    var factorId = document.getElementById('HiddenFactorNo').value;
    var url = apiurl + '/struct/' + structid + '/factor/' + factorId + '/sensors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            if (data.length == 0) {
                $('#sensorListRt').selectpicker('refresh');
                return;
            }
            var option = '';
            //sensortype
            for (var i = 0; i < data.length; i++) {
                for (var index = 0; index < data[i].sensors.length; index++) {
                    option += '<option value=\'' + data[i].sensors[index].sensorId + '\'>' + data[i].sensors[index].location + '</option>';
                }
            }
            $('#sensorListRt').html(option);
            $('#sensorListRt').selectpicker('refresh');
            var defaultSensor = document.getElementById('HiddenSensorId').value;
            var defaultSensorID = "";
            if (defaultSensor == 0) {
                defaultSensorID = '';
            }
            else {
                defaultSensorID = defaultSensor;
            }
            if (defaultSensorID == '' || defaultSensorID == null || defaultSensorID == undefined) {
                defaultSensorID = data[0].sensors[0].sensorId;
                $('#sensorListRt').selectpicker('val', defaultSensorID);
            }
            else {
                $('#sensorListRt').selectpicker('val', parseInt(defaultSensorID));
            }

            // 设置定时刷新实时数据的SensorId
            global_Vibration_Rt_Sensor = defaultSensorID;
            //getRtData(false);
            vibrationRtData(false, global_Vibration_Rt_Sensor);

            clearInterval(global_Vibration_Rt_Timer);
            global_Vibration_Rt_Timer = setInterval(function () {
                if (!global_Vibration_Rt_Running) {
                    //getRtData(false);
                    vibrationRtData(false, global_Vibration_Rt_Sensor);
                }
            }, global_Vibration_Rt_Interval);

            $('#sensorListRt').unbind('change').change(function () {
                $('#sensorListRt').selectpicker('refresh');
                var sensorId = parseInt($(this).val());
                global_Vibration_Rt_Sensor = sensorId;
                //getRtData(true);
                vibrationRtData(true, global_Vibration_Rt_Sensor);

                clearInterval(global_Vibration_Rt_Timer);
                global_Vibration_Rt_Timer = setInterval(function () {
                    if (!global_Vibration_Rt_Running) {
                        //getRtData(false);
                        vibrationRtData(false, global_Vibration_Rt_Sensor);
                    }
                }, global_Vibration_Rt_Interval);
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else {
                alertTips('传感器列表获取失败, 请尝试刷新页面', 'label-important', 'tipRt', 5000);
            }
        }
    });
}
function getRtData(isInit, sensorId) {
    //var sensorId = global_Vibration_Rt_Sensor;
    global_Vibration_Rt_Running = true;
    var isShowTable = document.getElementById('chkShowTable').checked;
    //由时间点获取触发振动的数据
    //时域数据
    var url = apiurl + '/vibration-RShell/' + sensorId + '/rt-original-data' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        //async: false,
        success: function (data) {
            rtTableData = [];
            if (data == null || data.length == 0) {
                displayRtCom();//实时数据提示信息
                return;
            } else {
                var date = data[0].CollectTime;
                // 同一批次不刷新页面
                if ($('#txtRtBatch').val() == JsonToDateTime(date) && !isInit) {
                    return;
                }
                if (doubleDirectionRtFlag == 1) {
                    if (data.length > 1) {
                        displayRtOri('none', 0);//时域
                        displayRtOri('none', 1);//时域
                    } else {
                        displayRtOri('block', 1);//时域
                        $('#rt_ori2_error').html("");
                        errorTipParam("rt_ori2_error", "有一方向");
                    }
                } else {
                    if (data.length > 0) {
                        displayRtOri('none', 0);//时域
                    } else {
                        displayRtOri('block', 0);//时域
                        $('#rt_ori1_error').html("");
                        errorTipParam("rt_ori1_error", "");
                    }
                }
                getRtSpectrum(sensorId);//频谱图
                $('#txtRtBatch').val(JsonToDateTime(date));

                $('#rt_table').html("");
                var array = new Array();
                for (var i = 0; i < data.length; i++) {
                    array = [];
                    var location = data[i].Location;
                    if (data[i].data.length > 0) {
                        displayRtOri('none', i);//时域
                    } else {
                        displayRtOri('block', i);//时域
                        $('#rt_ori' + (i + 1) + '_error').html("");
                        errorTipParam("rt_ori" + (i + 1) + "_error", data[i].Location + "方向");
                    }
                    $('#txtRtBatch').val(JsonToDateTime(date));
                    for (var j = 0; j < data[i].data.length; j++) {
                        var time = data[i].data[j].CollectTime;
                        var timeStr = ConvertJsonToDateStr(data[i].data[j].CollectTime);

                        if (data[i].data[j].Speed == null) {
                            array.push([time, null]);
                            var value_table = [location, '无', timeStr];
                            rtTableData.push(value_table);
                        } else {
                            array.push([time, parseFloat(data[i].data[j].Speed.toFixed(4))]);
                            var value_table = [location, parseFloat(data[i].data[j].Speed.toFixed(4)), timeStr];
                            rtTableData.push(value_table);
                        }
                        //array.push([time, parseFloat(data[0].data[j].value[0].toFixed(4))]);
                        //var value_table = [location, parseFloat(data[0].data[j].value[0].toFixed(4)), timeStr];
                        //rtTableData.push(value_table);
                    }
                    if (i == 0) {
                        comm_chart_rt = createHighchartTemplate('rt_ori', location + '振动时域图');
                    } else {
                        comm_chart_rt = createHighchartTemplate('rt_ori' + (i + 1), location + '振动时域图');
                    }
                    comm_chart_rt.series = [];
                    comm_chart_rt.xAxis = {
                        title: { text: '采集时间' },
                        type: 'datetime',
                        dateTimeLabelFormats: {
                            second: '%H:%M:%S',
                        },
                        labels: { rotation: -25, align: 'right', style: { font: 'normal 13px Verdana, sans-serif' } }
                    };
                    comm_chart_rt.yAxis.title = { text: '加速度(m/s²)' };
                    var unit = "m/s²";
                    comm_chart_rt.tooltip.formatter = function () {
                        var tooltipString = '<b>' + this.series.name + '</b>';
                        tooltipString = tooltipString + '<br/><br/>时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                        tooltipString = tooltipString + '<br/>' + '加速度:' + this.y.toString() + '<b>' + unit + '</b>';

                        return tooltipString;
                    };
                    comm_chart_rt.series.push({ name: location, data: array });

                    globalRtChartOri = new Highcharts.Chart(comm_chart_rt);
                }
                rtTable = createDataTable('rt_table', rtTableData, ['设备位置', '加速度(m/s²)', '采集时间']);
                if (isShowTable) {
                    $('#rt_table').show();
                } else {
                    $('#rt_table').hide();
                }
            }
            data = null;
            return;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            clearInterval(global_Vibration_Rt_Timer);
            displayRtOnly();//实时数据error提示信息
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else {
                alertTips('数据获取失败, 请尝试重新查询', 'label-important', 'tipRt', 5000);
            }
        }
    });

    global_Vibration_Rt_Running = false;
}
function getRtSpectrum(sensorId) {
    var url = apiurl + '/vibration-RShell/' + sensorId + '/rt-spectrum-data' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        //async: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                displayRt('block', 0);//频谱
                displayRt('block', 1);//频谱
                return;
            } else {
                if (data.length > 1) {
                    displayRt('none', 0);
                    displayRt('none', 1);
                } else {
                    displayRt('block', 1);
                }
                var array = new Array();
                for (var i = 0; i < data.length; i++) {
                    var location = data[i].Location;
                    array = [];
                    if (data[i].data.length > 0) {
                        displayRt('none', i);
                    } else {
                        displayRt('block', i);//时域
                    }
                    for (var j = 0; j < data[i].data.length; j++) {
                        var freq = "";
                        var valueP = "";
                        if (data[i].data[j].Frequency == null) {
                            freq = null;
                        } else {
                            freq = parseFloat(data[i].data[j].Frequency.toFixed(2));
                        }
                        if (data[i].data[j].Value == null) {
                            valueP = null;
                        } else {
                            valueP = parseFloat(data[i].data[j].Value.toFixed(4));
                        }
                        array.push([freq, valueP]);
                        //array.push([parseFloat(data[0].data[j].frequency.toFixed(2)), parseFloat(data[0].data[j].value[0].toFixed(4))]);
                    }
                    globalRtChart = null;
                    if (i == 0) {
                        chart_rt = createHighchartTemplate('rt', location + '振动频谱图', null, 1);
                    } else {
                        chart_rt = createHighchartTemplate('rt2', location + '振动频谱图', null, 1);
                    }

                    chart_rt.series = [];
                    var unit = "m/s²";
                    chart_rt.xAxis = {
                        title: { text: '频率 (Hz)' },
                        tickInterval: 50,
                        lineWidth: 2
                    };
                    chart_rt.yAxis.title = { text: data[i].Columns + '(' + unit + ')' };
                    chart_rt.tooltip.formatter = function () {
                        var tooltipString = '<b>' + this.series.name + '</b>';
                        tooltipString = tooltipString + '<br/><br/>频率:' + this.x.toString() + 'Hz';
                        tooltipString = tooltipString + '<br/>' + '加速度:' + this.y.toString() + '<b>' + unit + '</b>';

                        return tooltipString;
                    };
                    chart_rt.series.push({ name: location, data: array });
                    globalRtChart = new Highcharts.Chart(chart_rt);
                }

            }
            data = null;
            return;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            clearInterval(global_Vibration_Rt_Timer);
            displayRt('block', 0);

            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else {
                alertTips('无频谱数据', 'label-important', 'rt_error', 5000);
            }
        }
    });
}

//把形如2014-07-31 08:50:50转为datetime
function ConvertToDateTime(str) {
    str = str.replace(/-/g, "/");
    var Date1 = new Date(str);
    return Date1;
}

function ConvertJsonToDateTime(json) {
    var date = new Date(json);
    return date;
}

function ConvertJsonToDateStr(json) {
    var dtime = new Date();
    dtime.setTime(json);
    var dec = '';
    if (json.toString().split('.').length > 1) {
        dec = json.toString().split('.')[1].toString();
        for (var k = dec.length; k < 4; k++) {
            dec += '0';
        }
    }
    var normalizedMonth = dtime.getMonth() + 1 < 10 ? "0" + (dtime.getMonth() + 1) : dtime.getMonth() + 1;
    var cudata = dtime.getFullYear() + '-' + normalizedMonth + '-' + normalizeTimeFormat(dtime.getDate()) + ' ' + normalizeTimeFormat(dtime.getHours()) + ':' + normalizeTimeFormat(dtime.getMinutes()) + ':' + normalizeTimeFormat(dtime.getSeconds()) + '.' + dtime.getMilliseconds() + dec;
    return cudata;
}

function createHighchartTemplate(renderTo, title, seriesData, factorIdSend) {
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
            minorGridLineWidth: 0,
            gridLineWidth: 0,
            alternateGridColor: null,
        },
        credits: {
            enabled: false
        },
        plotOptions: {
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
            enabled: true,
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

function createDataTable(domId, data, columns) {
    $('#' + domId).html('<table cellpadding="0" cellspacing="0" border="0" class="display" id="' + domId + '_table"></table>');

    var c = new Array();
    for (var i = 0; i < columns.length; i++) {
        c.push({ "title": columns[i] });
    }

    $('#' + domId + '_table').dataTable({
        "data": data,
        "columns": c,
        "sDom": 'T<"clear">lfrtip',

        "iDisplayLength": 50, //每页显示个数 
        "bScrollCollapse": true,
        "bLengthChange": true,  //每页显示的记录数 
        "bPaginate": true,  //是否显示分页
        "bFilter": true, //搜索栏
        "bSort": true, //是否支持排序功能
        "bInfo": true, //显示表格信息
        "bAutoWidth": false,  //自适应宽度
        "bStateSave": false, //保存状态到cookie *************** 很重要，当搜索的时候页面一刷新会导致搜索的消失。使用这个属性就可避免了

        //"sPaginationType": "bootstrap",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "aaSorting": [[columns.length - 1, "desc"]],
        "bDestroy": true,
        "oTableTools": {
            "sSwfPath": "/resource/library/tableTools/swf/copy_csv_xls_pdf.swf",
            "aButtons": [
                {
                    "sExtends": "xls",
                    "sButtonText": "导出到Excel",
                    "sFileName": "*.xls"
                }
            ]
        }
    });

    var stag = $('.data-table-content');
    if (!stag.is(':visible')) {
        stag.show();
    }
}
 
var doubleDirectionFlag = 0;
function vibrationData(sensorID) {
    if (isNaN(sensorID)) {
        displayOri('block', 0);
        display('block', 0);
        displayOri('block', 1);
        display('block', 1);
        return;
    }
    var url = apiurl + "/combinedSensors/" + sensorID + "/info?token=" + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        success: function (data) {
            if (data.length == 0 || data == null) {
                alertTips('该设备位置下未配置传感器，请查看配置', 'label-important', 'tip', 2000);
                displayCom();//提示信息
                $('#TimeList').html("");
                $('#TimeList').selectpicker('refresh');
            } else {
                flagCount = 0;
                if (data.length == 2) {
                    doubleDirectionFlag = 1;
                } else {
                    doubleDirectionFlag = 0;
                }
                for (var i = 0; i < data.length; i++) {
                    //if (i == 0) {
                    //    data_table_values = [];
                    //}
                    //getTimeList(data[i].CorrentSensorId, i);
                }
                getTimeList(sensorID, 0);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                displayOnly();//error提示信息
                alert("登录超时,请重新登录");
                logOut();
            } else {
                alertTips('数据获取失败, 请尝试重新查询', 'label-important', 'tip', 5000);
            }
        }
    });
}

var doubleDirectionRtFlag = 0;
function vibrationRtData(isInit, sensorID) {
    if (isNaN(sensorID)) {
        displayRtOri('block', 0);
        displayRt('block', 0);
        displayRtOri('block', 1);
        displayRt('block', 1);
        return;
    }
    var url = apiurl + "/combinedSensors/" + sensorID + "/info?token=" + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        success: function (data) {
            if (data.length == 0 || data == null) {
                alertTips('该设备位置下未配置传感器，请查看配置', 'label-important', 'tipRt', 2000);
                $('#txtRtBatch').val("");
                displayRtCom();//实时数据提示信息
            } else {
                if (data.length == 2) {
                    doubleDirectionRtFlag = 1;
                } else {
                    doubleDirectionRtFlag = 0;
                }
                getRtData(isInit, sensorID);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                displayRtOnly();//实时数据error提示信息
                alert("登录超时,请重新登录");
                logOut();
            } else {
                alertTips('数据获取失败, 请尝试重新查询', 'label-important', 'tipRt', 5000);
            }
        }
    });
}
//提示信息
function displayCom() {
    displayOri('block', 0);
    display('block', 0);
    displayOri('block', 1);
    display('block', 1);
    $("#comm1_ori_error").html("");
    $("#comm2_ori_error").html("");
    errorTipParam("comm1_ori_error", "");
}
//error提示信息
function displayOnly() {
    displayOri('block', 0);
    $("#comm1_ori_error").html("");
    $("#comm2_ori_error").html("");
    errorTipParam("comm1_ori_error", "");
}
//实时数据提示信息
function displayRtCom() {
    displayRtOri('block', 0);
    displayRt('block', 0);
    displayRtOri('block', 1);
    displayRt('block', 1);
    $("#rt_ori1_error").html("");
    $("#rt_ori2_error").html("");
    errorTipParam("rt_ori1_error", "");
}

//实时error提示信息
function displayRtOnly() {
    displayRtOri('block', 0);
    $("#rt_ori1_error").html("");
    $("#rt_ori2_error").html("");
    errorTipParam("rt_ori1_error", "");
}
