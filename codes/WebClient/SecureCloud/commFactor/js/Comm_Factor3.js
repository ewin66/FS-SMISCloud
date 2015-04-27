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
};

var thresholdValue = [];
var data_table_values = [];//列表      
var comm_chart1 = '';
var comm_chart2 = '';
var comm_chart3 = '';

var chart1, chart2, chart3;

var structid = getCookie("nowStructId");

$(function () {

    var id = "x_error,y_error,z_error";
    errorTip(id);

    var factorid = document.getElementById('HiddenFactorNo').value;
    $('#monitorList').addClass('active');

    setTimeout(function () {
        $('#factor_' + factorid).addClass('active');
    }, 1000)

    getMonitorSensorList();

    $('a.box-collapse').click(function () {
        var $target = $(this).parent().parent().next('.box-content');
        if ($target.is(':visible')) {
            $('img', this).attr('src', '../resource/img/toggle-expand.png');
        } else {
            $('img', this).attr('src', '../resource/img/toggle-collapse.png');
        }
        $target.slideToggle();
    });



    thresholdValue = [{ "Level": "first" }, { "Level": "second" }, { "Level": "third" }, { "Level": "fourth" }];
    $('.btndataquery').click(function () {
        dateValue($('#date').val());//重新获取起始时间
        loadChart(thresholdValue);
    });
    $("#date").change(function () {

        loadChart(thresholdValue);
    });
    //$("#sensorList").change(function () {

    //    loadChart(thresholdValue);
    //});
})

function loadChart(thresholdValue) {
    $('#show_table').children().remove();
    var sensorList = [];
    var sensorLocation = [];
    var sensorId = $('#sensorList').find('option:selected');
    for (var i = 0; i < sensorId.length; i++) {
        sensorList[i] = sensorId[i].value;
        sensorLocation[i] = sensorId[i].innerText;
    }
    if (sensorList.length == 0) {
        display('block');
    }
    else {
        //display('none');
        getMonitorData(sensorList, sensorLocation, thresholdValue);
        document.getElementById("expand_collapse").src = '../resource/img/toggle-collapse.png';
    }
}


function getMonitorSensorList() {
    $('#monitor').empty();
    $('#monitor').children().remove();
    $('#sensorList').children().remove();

    var factorId = document.getElementById('HiddenFactorNo').value;

    var url = apiurl + '/struct/' + structid + '/factor/' + factorId + '/sensors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',

        success: function (data) {
            if (data.length == 0) {
                $('.selectpicker').selectpicker('refresh');
                return;
            }
            if (data.length == 1) {
                var option = '';

                for (var i = 0; i < data.length; i++) {
                    for (var index = 0; index < data[i].sensors.length; index++) {
                        option += '<option value=\'' + data[i].sensors[index].sensorId + '\'>' + data[i].sensors[index].location + '</option>';
                    }
                }
                $('#sensorList').html(option);
                $('#sensorList').selectpicker();
                var defaultSensor = document.getElementById('HiddenSensorId').value;
                var defaultSensorID = "";
                if (defaultSensor == 0) {
                    defaultSensorID = '';
                }
                else {
                    defaultSensorID = defaultSensor;
                }
                //判断是否从热点点击进入
                if (defaultSensorID == '') {
                    defaultSensorID = data[0].sensors[0].sensorId;
                    $('#sensorList').selectpicker('val', defaultSensorID);
                }
                else {
                    $('#sensorList').selectpicker('val', parseInt(defaultSensorID));
                }
            }
            if (data.length > 1) {
                $('#monitorDisplay').show();

                var sensor_location = "";
                var sensor_type = "";
                var option_monitor = "";

                //加载设备类型列表            
                for (var i = 0; i < data.length; i++) {
                    option_monitor += "<option value='" + data[i].sensortype + "'>" + data[i].sensortype + "</option>";
                }
                $('#monitor').html(option_monitor);
                monitor.selectpicker();
                monitor.selectpicker('refresh');

                var defaultSensorID = '26';
                //判断是否从热点点击进入
                if (defaultSensorID == '') {
                    $('#monitor').selectpicker('val', data[0].sensor_type);
                    var option_sensor = '';
                    for (var i = 0; i < data[0].sensors.length; i++) {
                        option_sensor += '<option value=' + data[0].sensors[i].sensorId + '>' + data[0].sensors[i].location + '</option>';
                    }
                    $('#sensorList').html(option_sensor);
                    $('#sensorList').selectpicker();
                    var se = data[0].sensors[0].sensorid;
                    $('#sensorList').selectpicker('val', se);
                }
                else {
                    for (var i = 0; i < data.length; i++) {
                        //根据defaultSensorID查找传感器位置和监测类型                                
                        var a = data[i].sensors
                        for (var j = 0; j < a.length; j++) {
                            if (parseInt(defaultSensorID) == a[j].sensorId) {
                                sensor_location = a[j].location;
                                sensor_type = data[i].sensortype;
                                break;
                            }
                        }
                        //根据监测类型找所有的传感器
                        if (sensor_type == data[i].sensortype) {
                            var option = '';
                            var sensors = data[i].sensors;
                            for (var index = 0; index < sensors.length; index++) {
                                option += "<option value=" + sensors[index].sensorId + ">" + sensors[index].location + "</option>";
                            }
                            $('#sensorList').html(option);
                            $('#sensorList').selectpicker();
                            $('#sensorList').selectpicker('val', parseInt(defaultSensorID));
                            break;
                        }
                        break;
                    }
                    if (sensor_type.length != 0) {
                        for (var i = 0; i < $('#monitor').length; i++) {
                            if ($('#monitor')[i].innerHTML == sensor_type) {
                                $('#monitor')[i].selected = true;
                                break;
                            }
                        }
                    }
                }
                $('#monitor').change(function () {
                    var monitortype = $('#monitor').val();
                    $('#sensorList').empty();

                    var sensors;
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].sensortype == monitortype) {
                            sensors = data[i].sensors;
                        }
                    }
                    var option = '';
                    for (var index = 0; index < sensors.length; index++) {
                        option += "<option value=" + sensors[index].sensorid + ">" + sensors[index].location + "</option>";
                    }
                    $('#sensorList').html(option);
                    $('#sensorList').selectpicker('val', sensors[0].sensorid);
                    $('#sensorList').selectpicker('refresh');
                })

            }
            loadChart(thresholdValue);
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

function getMonitorData(sensorList, sensorLocation, thresholdValue) {
    var interval = getInteval()
    if (interval <= 6) {
        var url = apiurl + "/sensor/" + sensorList + "/data/" + getStartdate() + '/' + getEnddate() + '?token=' + getCookie('token');
    }
    else if (interval <= 30) {
        var url = apiurl + "/sensor/" + sensorList + "/data/" + getStartdate() + '/' + getEnddate() + '/' + '1/minute?token=' + getCookie('token');
    }
    else if (interval <= 90) {
        var url = apiurl + "/sensor/" + sensorList + "/data/" + getStartdate() + '/' + getEnddate() + '/' + '1/hour?token=' + getCookie('token');
    }
    else if (interval <= 365) {
        var url = apiurl + "/sensor/" + sensorList + "/data/" + getStartdate() + '/' + getEnddate() + '/' + '12/hour?token=' + getCookie('token');
    }
    else {
        var url = apiurl + "/sensor/" + sensorList + "/data/" + getStartdate() + '/' + getEnddate() + '/' + '1/day?token=' + getCookie('token');
    }

    $.ajax({
        url: url,
        type: 'get',

        //timeout: 5000,
        success: function(data) {
            data_table_values = []; //清空列表
            if (data.length == 0) {
                display('block');
                return;
            } else {
                display('none');
                //多个传感器，图形右边表格默认展示第一个传感器的信息
                getSensorWarn(data, sensorList, sensorLocation, getStartdate(), getEnddate());
                var mmValue = getMaxMinValue(data);
                var yMax1 = mmValue.max[0];
                var yMin1 = mmValue.min[0];
                var yMax2 = mmValue.max[1];
                var yMin2 = mmValue.min[1];
                var yMax3 = mmValue.max[2];
                var yMin3 = mmValue.min[2];

                //comm_chart1 = createHighchartComm1('chart_x', 'X方向趋势图', null, 1);
                //comm_chart2 = createHighchartComm1('chart_y', 'Y方向趋势图', null, 2);
                //comm_chart3 = createHighchartComm1('chart_z', 'Z方向趋势图', null, 3); 
                comm_chart1 = createHighchartComm1('chart_x', data[0].columns[0] + '趋势图', null, 1);
                comm_chart2 = createHighchartComm1('chart_y', data[0].columns[1] + '趋势图', null, 2);
                comm_chart3 = createHighchartComm1('chart_z', data[0].columns[2] + '趋势图', null, 3);

                comm_chart1.series = [];
                comm_chart2.series = [];
                comm_chart3.series = [];
                for (var i = 0; i < data.length; i++) {
                    var location = data[i].location;
                    var columns = data[i].columns;
                    var unit = data[i].unit;
                    var array1 = new Array();
                    var array2 = new Array();
                    var array3 = new Array();
                    for (var j = 0; j < data[i].data.length; j++) {
                        var time = data[i].data[j].acquisitiontime.substring(6, 19);
                        if (data[i].data[j].value[0] != null) {
                            array1.push([parseInt(time), data[i].data[j].value[0]]);
}
                        if (data[i].data[j].value[1] != null) {
                            array2.push([parseInt(time), data[i].data[j].value[1]]);
                        }
                        if (data[i].data[j].value[2] != null) {
                            array3.push([parseInt(time), data[i].data[j].value[2]]);
                        }
                        if (data[i].data[j].value[0] != null || data[i].data[j].value[1] != null || data[i].data[j].value[2] != null) {
                            var value_table = [location, data[i].data[j].value[0], data[i].data[j].value[1], data[i].data[j].value[2], time];
                            data_table_values.push(value_table);
                        }
                    }
                    comm_chart1.yAxis.title = { text: columns[0] + '(' + unit[0] + ')' };
                    comm_chart1.tooltip.formatter = function () {
                        var tooltipString = '<b>' + this.series.name + '</b>';
                        tooltipString = tooltipString + '<br/><br/>采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                        tooltipString = tooltipString + '<br/>' + '监测数据:' + this.y.toString() + '<b>' + unit[0] + '</b>';
                        return tooltipString;
                    }
                    comm_chart2.yAxis.title = { text: columns[1] + '(' + unit[1] + ')' };
                    comm_chart2.tooltip.formatter = function () {
                        var tooltipString = '<b>' + this.series.name + '</b>';
                        tooltipString = tooltipString + '<br/><br/>采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                        tooltipString = tooltipString + '<br/>' + '监测数据:' + this.y.toString() + '<b>' + unit[1] + '</b>';
                        return tooltipString;
                    }
                    comm_chart3.yAxis.title = { text: columns[2] + '(' + unit[2] + ')' };
                    comm_chart3.tooltip.formatter = function () {
                        var tooltipString = '<b>' + this.series.name + '</b>';
                        tooltipString = tooltipString + '<br/><br/>采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                        tooltipString = tooltipString + '<br/>' + '监测数据:' + this.y.toString() + '<b>' + unit[2] + '</b>';
                        return tooltipString;
                    }
                    comm_chart1.series.push({ name: location, data: array1 });
                    comm_chart2.series.push({ name: location, data: array2 });
                    comm_chart3.series.push({ name: location, data: array3 });
                    if (i != 0) {
                        var immValue = getMaxMinValue(data, i);
                        yMax1 = immValue.max[0] > yMax1 ? immValue.max[0] : yMax1;
                        yMin1 = immValue.min[0] < yMin1 ? immValue.min[0] : yMin1;
                        yMax2 = immValue.max[1] > yMax2 ? immValue.max[1] : yMax2;
                        yMin2 = immValue.min[1] < yMin2 ? immValue.min[1] : yMin2;
                        yMax3 = immValue.max[2] > yMax3 ? immValue.max[2] : yMax3;
                        yMin3 = immValue.min[2] < yMin3 ? immValue.min[2] : yMin3;
                    }
                }
                //数据图形拉平
                SetChartRange(comm_chart1, yMax1, yMin1);
                comm_chart1.chart.events = {
                    selection: function (event) {
                        if (event.xAxis) {
                            chart1.yAxis[0].update({
                                max: null,
                                min: null
                            });
                        } else {
                            var diff = yMax1 - yMin1;
                            var total = diff * 4;
                            var half = total / 2;

                            chart1.yAxis[0].update({
                                max: yMax1 + half,
                                min: yMin1 - half
                            });
                        }
                    }
                };
                SetChartRange(comm_chart2, yMax2, yMin2);
                comm_chart2.chart.events = {
                    selection: function (event) {
                        if (event.xAxis) {
                            chart2.yAxis[0].update({
                                max: null,
                                min: null
                            });
                        } else {
                            var diff = yMax2 - yMin2;
                            var total = diff * 4;
                            var half = total / 2;

                            chart2.yAxis[0].update({
                                max: yMax2 + half,
                                min: yMin2 - half
                            });
                        }
                    }
                };

                SetChartRange(comm_chart3, yMax3, yMin3);
                comm_chart3.chart.events = {
                    selection: function (event) {
                        if (event.xAxis) {
                            chart3.yAxis[0].update({
                                max: null,
                                min: null
                            });
                        } else {
                            var diff = yMax3 - yMin3;
                            var total = diff * 4;
                            var half = total / 2;

                            chart3.yAxis[0].update({
                                max: yMax3 + half,
                                min: yMin3 - half
                            });
                        }
                    }
                };

                chart1 = new Highcharts.Chart(comm_chart1);
                chart2 = new Highcharts.Chart(comm_chart2);
                chart3 = new Highcharts.Chart(comm_chart3);

                displayThresholdData(sensorList[0], thresholdValue, chart1, 1);
                displayThresholdData(sensorList[0], thresholdValue, chart2, 2);
                displayThresholdData(sensorList[0], thresholdValue, chart3, 3);

                tableManager('show_table', data_table_values, ['设备位置', columns[0] + '(' + unit[0] + ')', columns[1] + '(' + unit[1] + ')', columns[2] + '(' + unit[2] + ')', '采集时间']);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                display('block');
                alert("登录超时,请重新登录");
                logOut();
            } else {
                alertTips('数据获取失败, 请尝试重新查询', 'label-important', 'tip', 5000);
            }
        }
    });
}

function display(comm3) {
    if (comm3 == "block") {
        $('#x_error').show();
        $('#chart_x').hide();
        $('#chart_y').hide();
        $('#chart_z').hide();
        $('#XwarnBox').hide();
        $('#YwarnBox').hide();
        $('#ZwarnBox').hide();

    } else {
        $('#x_error').hide();
        $('#y_error').hide();
        $('#z_error').hide();
        $('#chart_x').show();
        $('#chart_y').show();
        $('#chart_z').show();
        $('#XwarnBox').show();
        $('#YwarnBox').show();
        $('#ZwarnBox').show();
    }
}