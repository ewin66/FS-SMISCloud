var structid = getCookie("nowStructId");

var thresholdValue = [{ "Level": "first" }, { "Level": "second" }, { "Level": "third" }, { "Level": "fourth" }];

var data_table_values = [];//列表

//highcharts变量
var comm_chart = "";
var globalChartOri;
var globalChart;
var globalRtChartOri;
var globalRtChart;
var rtTable;
var rtTableData = [];

var global_Vibration_Rt_Sensor;
var global_Vibration_Rt_Timer;
var global_Vibration_Rt_Interval = 5000;
var global_Vibration_Rt_Running = false;

$(function () {
    var id = "comm1_error";
    errorTip(id);
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

    $('#date').change(function () {
        var sensorId = parseInt($('#sensorList').val());
        getTimeList(sensorId);
    });

    $('#btnQuery').click(function () {
        dateValue($('#date').val());//重新获取起始时间
        var sensorId = parseInt($('#sensorList').val());
        getTimeList(sensorId);
    });

    $('#chkShowTable').change(function () {
        if (this.checked) {
            createDataTable('rt_table', rtTableData, ['设备位置', '速度(cm/s)', '采集时间']);
        } else {
            if (rtTable != null) {
                rtTable.fnDestroy();
            }
            $('#rt_table').children().remove();
        }
    });

    $('#tabRt').click(function () {
        $('#sensorListRt').selectpicker('refresh');
        var sensorId = parseInt($('#sensorListRt').val());
        global_Vibration_Rt_Sensor = sensorId;
        getRtData(true);
        clearInterval(global_Vibration_Rt_Timer);
        global_Vibration_Rt_Timer = setInterval(function () {
            if (!global_Vibration_Rt_Running) {
                getRtData();
            }
        }, global_Vibration_Rt_Interval);
    });

    $('#tabHistory').click(function () {
        clearInterval(global_Vibration_Rt_Timer);
    });
});

// 图形为空提示
function errorTip(string_id) {
    var graph_id = string_id.split(',');
    var errorTipstring = '<div id=\'error\' class=\'row-fluid dataerror-tip\'>' +
        '<div class="span3">' +
        '<span class=\'label label-important\' style=\'margin-left: 5px;margin-top: 10px;\'>抱歉，没有查询到任何有效的数据</span>' +
        '</div>' +
        '</div>';
    for (var i = 0; i < graph_id.length; i++) {
        $('#' + graph_id[i]).append(errorTipstring);
    }
}

// 图形显示控制
function display(value) {
    if (value == "block") {
        $('#comm1_error').show();
        $('#comm1').hide();
    }
    else {
        $('#comm1_error').hide();
        $('#comm1').show();
    }
}

function displayOri(value) {
    if (value == "block") {
        $('#comm1_ori').hide();
        try {
            $('#data_tablesshow_table').dataTable().fnDestroy();
        } catch (e) {
        }
        $('#show_table').children().remove();
        $('#warnBox').hide();
    }
    else {
        $('#comm1_ori').show();
        $('#warnBox').show();
    }
}

// 实时数据图形控制
function displayRt(value) {
    if (value == "block") {
        $('#rt').hide();
    }
    else {
        $('#rt').show();
    }
}

function displayRtOri(value) {
    if (value == "block") {
        $('#rt_ori_error').show();
        if (rtTable != null) {
            rtTable.fnDestroy();
        }
        $('#rt_table').children().remove();
        $('#txtRtBatch').val('');
        $('#rt_ori').hide();
    }
    else {
        $('#rt_ori_error').hide();
        $('#rt_ori').show();
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


            getTimeList(defaultSensorID);
            //loadChart(thresholdValue);

            $('#sensorList').unbind('change').change(function () {
                $('#sensorList').selectpicker('refresh');
                var sensorId = parseInt($(this).val());
                getTimeList(sensorId);
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

function getTimeList(sensorId) {
    $('#show_table').children().remove();
    //获取一段时间内的触发振动的时间点
    var url = apiurl + '/vibration/' + sensorId + '/data-batch/' + getStartdate() + '/' + getEnddate() + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                displayOri('block');
                display('block');
                $('#TimeList').html("");
                $('#TimeList').selectpicker('refresh');
                return;
            } else {
                displayOri('none');
                //加载数据
                var sb = new StringBuffer();
                for (var i = data.length - 1; i >= 0; i--) {
                    sb.append('<option value=\'' + i + '\'>' + JsonToDateTime(data[i].collectTime) + '</option>');
                }
                $('#TimeList').html(sb.toString());
                $('#TimeList').selectpicker('refresh');

                var sensorLocation = $('#sensorList').find("option:selected").text();
                getMonitorData(sensorId, sensorLocation, data[data.length - 1].batchId, data[data.length - 1].maxFrequency, JsonToDateTime(data[data.length - 1].collectTime));

                $('#TimeList').unbind('change').change(function() {
                    $('#TimeList').selectpicker('refresh');
                    var index = parseInt($(this).val());
                    getMonitorData(sensorId, sensorLocation, data[index].batchId, data[index].maxFrequency, JsonToDateTime(data[index].collectTime));
                });
            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                displayOri('block');
                display('block');
                alert("登录超时,请重新登录");
                logOut();
            } else {
                $('#TimeList').selectpicker('refresh');
                alertTips('数据获取失败, 请尝试重新查询', 'label-important', 'tip', 5000);
            }
        }
    });
}

function getMonitorData(sensorList, sensorLocation, batchId, maxFrequency, timeList) {
    $('#show_table').children().remove();
    //由时间点获取触发振动的数据    
    //时域数据
    var url = apiurl + '/vibration/' + batchId + '/original-data' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        //async: false,
        success: function (data) {
            data_table_values = [];
            if (data == null || data.length == 0) {
                displayOri('block');
                return;
            } else if (data[0].data.length == 0 || data[0].data.length == null) {
                displayOri('block');
                return;
            } else {
                displayOri('none');
                getSpectrum(batchId);

                getSensorWarn(data, sensorList, sensorLocation, timeList, timeList);
                getMaxMinValue(data);

                var location = data[0].location;
                var array = new Array();
                for (var j = 0; j < data[0].data.length; j++) {
                    var time = data[0].data[j].collectTime;
                    var timeStr = ConvertJsonToDateStr(data[0].data[j].collectTime);
                    array[j] = [time, data[0].data[j].value[0]];
                    var value_table = [location, data[0].data[j].value[0], timeStr];
                    data_table_values.push(value_table);
                }

                var chart = createChartOption('振动时域图', location);
                chart.series = [];
                chart.xAxis[0].type = 'time';
                chart.legend.data[0] = location;
                chart.series.push({ name: location, type: 'line', data: array });

                globalChartOri = echarts.init(document.getElementById('comm1_ori'));
                globalChartOri.setTheme(theme);
                globalChartOri.setOption(chart);

                createDataTable('show_table', data_table_values, ['设备位置', '速度(cm/s)', '采集时间']);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                display('block');
                alert("登录超时,请重新登录");
                logOut();
            } else {
                $('#TimeList').selectpicker('refresh');
                alertTips('数据获取失败, 请尝试重新查询', 'label-important', 'tip', 5000);
            }
        }
    });
}

function getSpectrum(batchId) {
    var url = apiurl + '/vibration/' + batchId + '/spectrum-data' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                display('block');
                return;
            } else if (data[0].data.length == 0 || data[0].data.length == null) {
                display('block');
                return;
            } else {
                display('none');

                var location = data[0].location;
                var array = new Array();
                for (var j = 0; j < data[0].data.length; j++) {
                    array[j] = [data[0].data[j].frequency, data[0].data[j].value[0]];
                }

                var chart = createChartOption('振动频谱图', location);
                chart.series = [];
                chart.series.push({ name: location, type: 'line', data: array });
                globalChart = echarts.init(document.getElementById('comm1'));
                globalChart.setTheme(theme);
                globalChart.setOption(chart);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                display('block');
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
            getRtData();
            clearInterval(global_Vibration_Rt_Timer);
            global_Vibration_Rt_Timer = setInterval(function () {
                if (!global_Vibration_Rt_Running) {
                    getRtData();
                }
            }, global_Vibration_Rt_Interval);

            $('#sensorListRt').unbind('change').change(function () {
                $('#sensorListRt').selectpicker('refresh');
                var sensorId = parseInt($(this).val());
                global_Vibration_Rt_Sensor = sensorId;
                getRtData(true);
                clearInterval(global_Vibration_Rt_Timer);
                global_Vibration_Rt_Timer = setInterval(function () {
                    if (!global_Vibration_Rt_Running) {
                        getRtData();
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

function getRtData(isInit) {
    var sensorId = global_Vibration_Rt_Sensor;
    if (sensorId == null) return;

    global_Vibration_Rt_Running = true;
    var isShowTable = document.getElementById('chkShowTable').checked;
    //由时间点获取触发振动的数据
    //时域数据
    var hasOriValue = false;
    var url = apiurl + '/vibration/' + sensorId + '/rt-original-data' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        //async: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                displayRtOri('block');
                return;
            } else if (data[0].data == null || data[0].data.length == 0) {
                displayRtOri('block');
                return;
            } else {
                if (data[0].sensorId != global_Vibration_Rt_Sensor) {
                    displayRtOri('block');
                    return;
                }
                displayRtOri('none');
                var date = data[0].collectTime;
                hasOriValue = true;
                // 同一批次不刷新页面
                if ($('#txtRtBatch').val() == JsonToDateTime(date) && !isInit) {
                    return;
                }
                getRtSpectrum(sensorId);
                if (!hasOriValue) {
                    displayRt('block');
                }

                $('#txtRtBatch').val(JsonToDateTime(date));
                $('#rt_table').html("");
                var location = data[0].location;
                var array = new Array();
                rtTableData = [];
                for (var j = 0; j < data[0].data.length; j++) {
                    var time = ConvertJsonToDateTime(data[0].data[j].collectTime);
                    var timeStr = ConvertJsonToDateStr(data[0].data[j].collectTime);
                    array.push([time, data[0].data[j].value[0]]);
                    var value_table = [location, data[0].data[j].value[0], timeStr];
                    rtTableData.push(value_table);
                }

                var chart = createChartOption('振动时域图', location);
                chart.series = [];
                chart.xAxis[0].type = 'time';
                chart.legend.data[0] = location;
                chart.series.push({ name: location, type: 'line', data: array });
                chart.tooltip.formatter = function (params) {
                    return params.seriesName + ' : [ '
                           + params.value[0] + ', '
                           + params.value[1] + ' ]';
                }
                
                globalRtChartOri = echarts.init(document.getElementById('rt_ori'));
                globalRtChartOri.setTheme(theme);
                globalRtChartOri.setOption(chart);

                if (isShowTable) {
                    rtTable = createDataTable('rt_table', rtTableData, ['设备位置', '速度(cm/s)', '采集时间']);
                }
                data_table_values = null;
            }
            data = null;
            return;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            clearInterval(global_Vibration_Rt_Timer);
            displayRt('block');

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
    var url = apiurl + '/vibration/' + sensorId + '/rt-spectrum-data' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        //async: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                displayRt('block');
                return;
            } else if (data[0].data == null || data[0].data.length == 0) {
                displayRt('block');
                return;
            } else {
                displayRt('none');

                var location = data[0].location;
                var array = new Array();
                for (var j = 0; j < data[0].data.length; j++) {
                    array.push([data[0].data[j].frequency, data[0].data[j].value[0]]);
                }

                var chart = createChartOption('振动频谱图', location);
                chart.series = [];
                chart.series.push({ name: location, type: 'line', data: array });
                globalRtChart = echarts.init(document.getElementById('rt'));
                globalRtChart.setTheme(theme);
                globalRtChart.setOption(chart);
            }
            data = null;
            return;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            clearInterval(global_Vibration_Rt_Timer);
            displayRt('block');

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

function createChartOption(title, location) {
    var template = {
        title: {
            text: title,
            x: 'center'
        },
        legend: {
            orient: 'vertical',
            x: 'right',
            y: 'center',
            data: [location]
        },
        toolbox: {
            show: true,
            feature: {
                mark: { show: false },
                dataZoom: { show: true },
                dataView: { show: false, readOnly: true },
                magicType: { show: false, type: ['line', 'bar'] },
                restore: { show: true },
                saveAsImage: { show: true }
            }
        },
        tooltip: {
            show: true,
            trigger: 'item',
            formatter: function (params) {
                return params.seriesName + ' : [ '
                       + params.value[0] + ', '
                       + params.value[1] + ' ]';
            }
        },
        dataZoom: {
            show: true,
            realtime: true
        },
        xAxis: [
            {
                type: 'value',
                axisLabel: {
                    rotate: -25,
                    textStyle: {
                        align: 'right',
                        fontFamily: 'Verdana, sans-serif',
                        fontSize: 13
                    },
                }
            }
        ],
        yAxis: [
            {
                type: 'value',
                scale: true,
                axisLabel: {
                    formatter: '{value} cm/s'
                }
            }
        ]
    };
    return template;
}

function createDataTable(domId, data, columns) {
    $('#' + domId).html('<table cellpadding="0" cellspacing="0" border="0" class="display" id="' + domId + '_table"></table>');

    var c = new Array();
    for (var i = 0; i < columns.length; i++) {
        c.push({ "title": columns[i] });
    }

    $('#'+domId + '_table').dataTable({
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