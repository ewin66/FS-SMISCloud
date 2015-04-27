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

var data_table_values = [];//列表

//highcharts变量
var comm_chart = "";
var globalChart;

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
var weldStrainFlag = 0;
$(function () {

    var id = "comm1_error";
    errorTip(id);
    var factorid = document.getElementById('HiddenFactorNo').value;

    if (factorid == 53) {
        $('#sensorList').attr("multiple", false);//焊缝应变取消多选
        weldStrainFlag = 1;
    }
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
    ////选择阈值，重新加载图表
    //$('#threshold').change(function() {
    //    //阈值选择
    //    var threshold = "";

    //    var thresholdvalue = $('#threshold').val();

    //    thresholdValue = [{ "Leval": thresholdvalue }];
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
        //获取焊缝组下所有应变传感器
        if (weldStrainFlag) {
            var url = apiurl + "/combinedSensors/" + sensorList + "/info?token=" + getCookie('token');
            $.ajax({
                url: url,
                type: 'get',
                success: function (data) {
                    if (data.length == 0 || data==null) {
                        alertTips('该设备位置下未配置传感器，请查看配置', 'label-important', 'tip', 5000);
                        display('block');
                    } else {
                        sensorList = [];
                        for (var j = 0; j < data.length; j++) {
                            sensorList[j] = data[j].CorrentSensorId;
                        }
                        getMonitorData(sensorList, sensorLocation, thresholdValue);
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

        } else {
            getMonitorData(sensorList, sensorLocation, thresholdValue);
            document.getElementById("expand_collapse").src = '../resource/img/toggle-collapse.png';
        }
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
            if (defaultSensorID == '' || defaultSensorID == null || defaultSensorID == undefined) {
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

function getMonitorData(sensorList, sensorLocation, thresholdValue) {
    // var interval = (Date.parse(getEnddate().replace(/-/ig, '/')) - Date.parse(getStartdate().replace(/-/ig, '/'))) / 1000 / 60 / 60 / 24;
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
    //var byteLen = 0;
    //for (var i = 0; i < url.length; i++) {
    //    if (url.charCodeAt(i) > 255) {
    //        byteLen += 2;
    //    }
    //    else {
    //        byteLen++;
    //    }
    //}

    //if (byteLen > 2048) {
    //    alert(134);
    //    return;
    //}

    $.ajax({
        url: url,
        type: 'get',

        //timeout: 5000,
        success: function (data) {
            data_table_values = []; //清空列表
            if (data.length == 0 || data==null) {
                display('block');
                return;
            } else {
                display('none');
                //多个传感器，图形右边表格默认展示第一个传感器的信息
                getSensorWarn(data, sensorList, sensorLocation, getStartdate(), getEnddate());
                var mmValue = getMaxMinValue(data);
                comm_chart = createHighchartComm1('comm1', data[0].columns + '趋势图', null, 1);
                var yMax = mmValue.max[0];
                var yMin = mmValue.min[0];

                comm_chart.series = [];
                for (var i = 0; i < data.length; i++) {
                    var location = data[i].location;
                    var unit = data[i].unit;
                    var array = new Array();
                    for (var j = 0; j < data[i].data.length; j++) {
                        if (data[i].data[j].value[0] != null) {
                            var time = data[i].data[j].acquisitiontime.substring(6, 19);
                            array.push([parseInt(time), data[i].data[j].value[0]]);
                            var value_table = [location, data[i].data[j].value[0], time];
                            data_table_values.push(value_table);
                        }
                    }
                    comm_chart.yAxis.title = { text: data[0].columns + '(' + unit + ')' };
                    comm_chart.tooltip.formatter = function () {
                        var tooltipString = '<b>' + this.series.name + '</b>';
                        tooltipString = tooltipString + '<br/><br/>采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                        tooltipString = tooltipString + '<br/>' + '监测数据:' + this.y.toString() + '<b>' + unit + '</b>';
                        return tooltipString;
                    };

                    if (i != 0) {
                        var immValue = getMaxMinValue(data, i);
                        yMax = immValue.max[0] > yMax ? immValue.max[0] : yMax;
                        yMin = immValue.min[0] < yMin ? immValue.min[0] : yMin;
                    }

                    comm_chart.series.push({ name: location, data: array });
                }
                //数据图形拉平
                SetChartRange(comm_chart, yMax, yMin);

                comm_chart.chart.events = {
                    selection: function (event) {
                        if (event.xAxis) {
                            globalChart.yAxis[0].update({
                                max: null,
                                min: null
                            });
                        } else {
                            var diff = yMax - yMin;
                            var total = diff * 4;
                            var half = total / 2;

                            globalChart.yAxis[0].update({
                                max: yMax + half,
                                min: yMin - half
                            });
                        }
                    }
                };

                //雨量的页面要改为柱状图
                var factorId = document.getElementById('HiddenFactorNo').value;
                if (factorId == '6') {
                    comm_chart.chart = {
                        type: 'column',
                        renderTo: 'comm1',
                        zoomType: 'xy'
                    };
                }
                globalChart = new Highcharts.Chart(comm_chart);

                displayThresholdData(sensorList[0], thresholdValue, globalChart, 1);

                tableManager('show_table', data_table_values, ['设备位置', data[0].columns + '(' + unit + ')', '采集时间']);
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
};


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

