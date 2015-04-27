
//验证用户访问权限
//verifyUserAccess();

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

var thresholdValue = [];

var chart1;
var chart2;

function display(comm2) {
    if (comm2 == "block") {
        $('#x_error').show();
        $('#chart_x').hide();
        $('#chart_y').hide();
        $('#XwarnBox').hide();
        $('#YwarnBox').hide();


    } else {
        $('#x_error').hide();
        $('#y_error').hide();
        $('#chart_x').show();
        $('#chart_y').show();
        $('#XwarnBox').show();
        $('#YwarnBox').show();
    }
}

var structid = getCookie("nowStructId");
var stressFlag = 0;
$(function () {
    var id = "x_error,y_error";
    errorTip(id);
    var factorid = document.getElementById('HiddenFactorNo').value;

    //$('#sensorList').attr("multiple", false);//杆件应力取消多选
    //$('#xDirect').parent().css('display', 'none');//杆件应力 方向的隐藏
    //$('#Th1').parent().css('display', 'none');//杆件应力 方向的隐藏

    $('#monitorList').addClass('active');

    setTimeout(function () {
        $('#factor_' + factorid).addClass('active');
    }, 1000);

    getSensorList();

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

});

function loadChart(thresholdValue) {
    columnName = [];
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
        //获取杆件组下所有应变传感器
        getStressStrainData(sensorList, 1, sensorLocation, thresholdValue)
    }
}


function getSensorList() {
    $('#sensorList').children().remove();
    var factorId = document.getElementById('HiddenFactorNo').value;

    var url = apiurl + '/struct/' + structid + '/factor/' + factorId + '/sensors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',

        success: function (data) {
            if (data.length == 0) {
                $('.selectpicker').selectpicker('refresh');
                //errorTip();
                return;
            }
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
            if (defaultSensorID == '') {
                defaultSensorID = data[0].sensors[0].sensorId;
                $('#sensorList').selectpicker('val', defaultSensorID);
            }
            else {
                $('#sensorList').selectpicker('val', parseInt(defaultSensorID));
            }
            loadChart(thresholdValue);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            }
            else {
                alertTips('传感器列表获取失败, 请尝试刷新页面', 'label-important', 'tip', 5000);
            }
        }
    });
}

//获得时间步长
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
//拼接杆件应力表格数据列头
var columnNameStress = [];
var columnNameStrain = [];
var columnName = [];
var dataStress = [];//杆件应力数据存储
var dataStrain = [];//杆件应变数据存储
function getStressStrainData(sensorList, k, sensorLocation, thresholdValue) {
    if (k == 1) {
        $('#show_table').children().remove();
    }
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
        success: function (data) {
            if (k == 1) {
            } else { }
           //var  data_table_values = []; //清空列表
            if (data.length == 0) {
                display('block');
                return;
            } else {
                display('none');
                //多个传感器，图形右边表格默认展示第一个传感器的信息
                getSensorWarn(data, sensorList, getStartdate(), getEnddate(), k);
                var mmValue = getMaxMinValue(data,k); 
                var yMax1 = mmValue.max[0];
                var yMin1 = mmValue.min[0];
                var comm_chart1 = "";
                if (k == 1) {
                    comm_chart1 = createHighchartComm1('chart_x', (data[0].columns)[0] + '趋势图', null, 1);
                } else {
                    comm_chart1 = createHighchartComm1('chart_y', (data[0].columns)[0] + '趋势图', null, 1);
                }
                if (k == 1) {
                    dataStress = data;
                } else {
                    dataStrain = data;
                }
                comm_chart1.series = [];
                columnNameStrain = [];
                for (var i = 0; i < data.length; i++) {
                    if (k == 1) {
                        columnNameStress = [];
                        columnNameStress.push(data[i].location + '(' + data[i].unit[0] + ')');//拼接杆件应力表格数据列头
                    }
                    if (k == 2) {
                        columnNameStrain.push(data[i].location + '(' + data[i].unit[0] + ')');//拼接杆件应力表格数据列头
                        if (i == data.length - 1) {
                            columnNameStrain.push("采集时间");
                            columnName = columnNameStress.concat(columnNameStrain);
                            tableStressStrain();
                        }
                    }
                    var location = data[i].location;
                    var columns = data[i].columns;
                    var unit = data[i].unit;
                    var array1 = new Array();
                    for (var j = 0; j < data[i].data.length; j++) {
                        var time = data[i].data[j].acquisitiontime.substring(6, 19);
                        if (data[i].data[j].value[0] != null) {
                            array1.push([parseInt(time), data[i].data[j].value[0]]);
                        }
                    }

                    comm_chart1.yAxis.title = { text: columns[0] + '(' + unit[0] + ')' };
                    comm_chart1.tooltip.formatter = function () {
                        var tooltipString = '<b>' + this.series.name + '</b>';
                        tooltipString = tooltipString + '<br/><br/>采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                        tooltipString = tooltipString + '<br/>' + '监测数据:' + this.y.toString() + '<b>' + unit[0] + '</b>';
                        return tooltipString;
                    }
                    comm_chart1.series.push({ name: location, data: array1 });
                    if (i != 0) {
                        var immValue = getMaxMinValue(data, k, i);
                        yMax1 = immValue.max[0] > yMax1 ? immValue.max[0] : yMax1;
                        yMin1 = immValue.min[0] < yMin1 ? immValue.min[0] : yMin1;
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
                chart1 = new Highcharts.Chart(comm_chart1);
               
                displayThresholdData(sensorList[0], thresholdValue, chart1, k);

            }
            if (k == 1) {
                var url = apiurl + "/combinedSensors/" + sensorList + "/info?token=" + getCookie('token');
                $.ajax({
                    url: url,
                    type: 'get',
                    success: function (data) {
                        if (data.length == 0 || data == null) {
                            alertTips('该设备位置下未配置传感器，请查看配置', 'label-important', 'tip', 5000);
                            display('block');
                        } else {
                            var sensorStrainList = [];
                            for (var j = 0; j < data.length; j++) {
                                sensorStrainList[j] = data[j].CorrentSensorId;
                            }
                            getStressStrainData(sensorStrainList, 2, sensorLocation, thresholdValue)
                            document.getElementById("expand_collapse").src = '../resource/img/toggle-collapse.png';
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

function tableStressStrain() {
    var data_table_values = [];
    if (dataStress.length > 0) {
        for (var i = 0; i < dataStress[0].data.length; i++) {
            var value_table = [];
            value_table.push(dataStress[0].data[i].value);
            var time = dataStress[0].data[i].acquisitiontime.substring(6, 19);
            if (dataStrain.length > 0) {
                for (var j = 0; j < dataStrain.length; j++) {
                    for (var k = 0; k < dataStrain[j].data.length; k++) {
                        var timeStrain = dataStrain[j].data[k].acquisitiontime.substring(6, 19);
                        if (time == timeStrain) {
                            value_table.push(dataStrain[j].data[k].value);
                            break;
                        } else if (k == dataStrain[j].data.length - 1) {//判断某些应变没有值
                            value_table.push("无");
                            break;
                        }
                    }
                    //var timeStrain = dataStrain[j].data[i].acquisitiontime.substring(6, 19);
                    //if (time == timeStrain) {
                    //    value_table.push(dataStrain[j].data[i].value);
                    //} else {
                    //    dataStress.push("无");
                    //}

                }
            }
            value_table.push(time);
           data_table_values.push(value_table);
        }
    }
    tableManager('show_table', data_table_values, columnName);
    data_table_values = [];
    columnName = [];
}
