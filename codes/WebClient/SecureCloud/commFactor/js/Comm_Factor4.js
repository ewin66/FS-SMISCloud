
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
//验证用户访问权限
//verifyUserAccess();

var thresholdValue = [];

var data_table_values = [];//列表

//highcharts变量
var comm_chart = "";
var structid = getCookie("nowStructId");

function display(comm1) {
    if (comm1 == "block") {
        $('#comm1_error').show();
        $('#comm1').hide();
        $('#warnBox').hide();
    }
    else {
        $('#comm1_error').hide();
        $('#comm1').show();
        $('#warnBox').show();
    }
}

$(function () {

    var id = "comm1_error";
    errorTip(id);

    var factorid = document.getElementById('HiddenFactorNo').value;

    $('#monitorList').addClass('active');

    setTimeout(function () {
        $('#factor_' + factorid).addClass('active');
    }, 1000);

    //初始化时，当天的数据
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

    $("#date").change(function () {

        loadChart(thresholdValue);
    });
    $('.btndataquery').click(function () {
        dateValue($('#date').val());//重新获取起始时间
        loadChart(thresholdValue);
    });
    //$("#sensorList").change(function () {

    //    loadChart(thresholdValue);
    //});
});

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
            //索力下面有两种sensortype
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
    var interval = getInteval();
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
        cache: false,
        //timeout: 5000,
        success: function (data) {
            if (data.length == 0) {
                display('block');
                return;
            } else {
                display('none');
                //填充页面图形右边表格双坐标轴监测因素
                $("#sensorFactor").children().remove();
                var factor = "";
                for (var i = 0; i < data.length; i++) {
                    for (var j = 0; j < data[i].columns.length; j++) {
                        factor += '<th  class="headTh">' + data[i].columns[j] + '</th> ';
                    }
                }
                $("#sensorFactor").append('<th  class="headTh"></th>' + factor);

                //多个传感器，图形右边表格默认展示第一个传感器的信息
                if (sensorList.length > 1) {
                    //获取告警信息（告警）
                    SensorWarn(data, sensorList[0], sensorLocation[0], getStartdate(), getEnddate(), true);
                    //加载告警图表(最大值、最小值、传感器位置)
                    MaxMinValue([data[0]]);
                } else {
                    SensorWarn(data, sensorList, sensorLocation, getStartdate(), getEnddate(), true);
                    MaxMinValue(data);
                }

                var doubleAxis = twoAxis(data);
                var seriesData = series(data);
                var tableValus = seriesData.tableValues;
                var columns = data[0].columns;
                var unit = data[0].unit;
                comm_chart = createHighchartComm1('comm1', '趋势图', doubleAxis, seriesData.dataSeries, 1);

                comm_chart.tooltip.formatter = function () {
                    var tooltipString = '<b>' + this.series.name + '</b>';
                    tooltipString = tooltipString + '<br/><br/>采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                    tooltipString = tooltipString + '<br/>' + '监测数据:' + this.y.toString() + '<b>' + unit[this.series.index] + '</b>';

                    return tooltipString;
                }

                display('none');
                var chart = new Highcharts.Chart(comm_chart);

                //chart.yAxis[1].options.startOnTick = false;
                //chart.yAxis[1].options.endOnTick = false;
                //chart.yAxis[1].setExtremes(0, 100);

                tableManager('show_table', tableValus, ['设备位置', columns[0] + '(' + unit[0] + ')', columns[1] + '(' + unit[1] + ')', '采集时间']);
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

