var data_depth_time_x = [];
var data_depth_time_y = [];
var data_values_x = [];
var data_values_y = [];
var data_table_values = [];
var old_groupName = "";
var groupName = "";
var interval;
var indexTime;

var dataTod = [];
var dataYes = [];
var dataBefYes = [];
var data_table_three = [];

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

function display_leiji(leiji) {
    if (leiji == "block") {
        if (direct == 'x') {
            $('#X_leiji_error').show();
            $('#Y_leiji_error').hide();
        }
        else if (direct == 'y') {
            $('#X_leiji_error').hide();
            $('#Y_leiji_error').show();
        }
        else {
            $('#X_leiji_error').show();
            $('#Y_leiji_error').show();
        }
        $('#X_leiji').hide();
        $('#Y_leiji').hide();
    } else {
        $('#X_leiji_error').hide();
        $('#Y_leiji_error').hide();
    }
}

function display_qushi(qushi) {
    if (qushi == "block") {
        if (direct == 'x') {
            $('#X_dot_error').show();
            $('#Y_dot_error').hide();
        }
        else if (direct == 'y') {
            $('#X_dot_error').hide();
            $('#Y_dot_error').show();
        } else {
            $('#X_dot_error').show();
            $('#Y_dot_error').show();
        }
        $('#X_dot').hide();
        $('#Y_dot').hide();
    } else {
        $('#X_dot_error').hide();
        $('#Y_dot_error').hide();
    }
}

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
    }, 1000)

    var id = "X_leiji_error,Y_leiji_error,X_dot_error,Y_dot_error";
    errorTip(id);

    direct = 'y';
    getSensorList();

})

$('a.box-collapse').click(function () {
    var $target = $(this).parent().parent().next('.box-content');
    if ($target.is(':visible')) {
        $('img', this).attr('src', '../resource/img/toggle-expand.png');
    } else {
        $('img', this).attr('src', '../resource/img/toggle-collapse.png');
    }
    $target.slideToggle();
});

$('#XY_direction').change(function () {
    direct = $('#XY_direction :selected').attr('value');
});

$('.btndataquery').click(function () {
    dateValue($('#date').val());//重新获取起始时间
    loadChart();
});
$("#date").change(function () {

    loadChart();
});
$("#sensorList").change(function () {

    loadChart();
});
$("#XY_direction").change(function () {

    loadChart();
});



function loadChart() {
    var gm = $('#sensorList :selected').attr('value').split(",");
    var groupId = gm[0];
    var maxDepth = gm[1];
    groupName = $('#sensorList :selected').text();
    if (groupId == null)
        return;

    getMonitorData(groupId, maxDepth, groupName);
    document.getElementById("expand_collapse").src = '../resource/img/toggle-collapse.png';

    contrastData(groupId, groupName);
}
/*************start 对比数据**************/
function contrastData(groupId, groupName) {
    var myDate = new Date();

    var todayCEnd = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + ' 23:59:59';
    var todayCStart = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + ' 00:00:00';

    myDate.setDate(myDate.getDate() - 1);
    var yesterdayCEnd = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + ' 23:59:59';
    var yesterdayCStart = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + ' 00:00:00';

    myDate.setDate(myDate.getDate() - 2);
    var befYesterdayCEnd = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + ' 23:59:59';
    var befYesterdayCStart = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + ' 00:00:00';

    //今天
    var url_daily_time_Tod = apiurl + '/deep-displace/' + groupId + '/daily-data-by-time/y/' + todayCStart + '/' + todayCEnd + '' + '?token=' + getCookie('token');//日表 累计
    $.ajax({
        url: url_daily_time_Tod,
        type: 'get',
        success: function (data) {
            dataTod = data;
            contrastDataY(groupId, groupName, yesterdayCStart, yesterdayCEnd, befYesterdayCStart, befYesterdayCEnd);
        },//success
        error: function () {
            dataTod = null;
            contrastDataY(groupId, groupName, yesterdayCStart, yesterdayCEnd, befYesterdayCStart, befYesterdayCEnd);
        }
    });//累计

}

function contrastDataY(groupId, groupName, yesterdayCStart, yesterdayCEnd, befYesterdayCStart, befYesterdayCEnd) {
    //昨天
    var url_daily_time_Yes = apiurl + '/deep-displace/' + groupId + '/daily-data-by-time/y/' + yesterdayCStart + '/' + yesterdayCEnd + '' + '?token=' + getCookie('token');//日表 累计
    $.ajax({
        url: url_daily_time_Yes,
        type: 'get',
        success: function (data) {
            dataYes = data;
            contrastDataBY(groupId, groupName, befYesterdayCStart, befYesterdayCEnd);
        },//success
        error: function () {
            dataYes = null;
            contrastDataBY(groupId, groupName, befYesterdayCStart, befYesterdayCEnd);
        }
    });//累计
}

function contrastDataBY(groupId, groupName, befYesterdayCStart, befYesterdayCEnd) {
    //前天
    var url_daily_time_BeYes = apiurl + '/deep-displace/' + groupId + '/daily-data-by-time/y/' + befYesterdayCStart + '/' + befYesterdayCEnd + '' + '?token=' + getCookie('token');//日表 累计
    $.ajax({
        url: url_daily_time_BeYes,
        type: 'get',
        success: function (data) {
            dataBefYes = data;
            contrastDataTable(groupId, groupName);
        },//success
        error: function () {
            dataBefYes = null;
            contrastDataTable(groupName);
        }
    });//累计
}

function contrastDataTable(groupId, groupName) {
    data_table_three = [];
    var data_depth = [];
    var url = apiurl + '/group/' + groupId + '/sensors?token=' + getCookie('token');//对应管道下，所有深度
    $.ajax({
        url: url,
        type: 'get',
        success: function (data) {
            if (data.sensors.length == 0) {
                tableManager('show_table_comp', data_table_three, ['管组名称', '深度(m)', '今天累计值(mm)', '昨天累计值(mm)', '前天累计值(mm)', '今天变化量(mm)', '昨天变化量(mm)'], 10);
            } else {
                for (var i = 0; i < data.sensors.length; i++) {
                    data_depth.push(data.sensors[i].depth);
                    data_table_three.push([groupName, data.sensors[i].depth, "无", "无", "无", "无", "无"]);
                }
            }
            if (dataTod.length > 0) {
                if (dataTod[0].values.length > 0) {
                    for (var i = 0; i < dataTod[0].values.length; i++) {
                        for (var j = 0; j < data_table_three.length; j++) {
                            if (data_table_three[j][1] == dataTod[0].values[i].depth) {
                                if (dataTod[0].values[i].yvalue == null) {
                                    data_table_three[j][2] = "无";
                                }
                                else {
                                    data_table_three[j][2] = dataTod[0].values[i].yvalue;
                                }
                            }
                        }
                    }
                } else {
                    for (var i = 0; i < data_table_three.length; i++) {
                        data_table_three[i][2] = "无";
                    }
                }
            } else {
                for (var i = 0; i < data_table_three.length; i++) {
                    data_table_three[i][2] = "无";
                }
            }

            if (dataYes.length > 0) {
                if (dataYes[0].values.length > 0) {
                    for (var i = 0; i < dataYes[0].values.length; i++) {
                        for (var j = 0; j < data_table_three.length; j++) {
                            if (data_table_three[j][1] == dataYes[0].values[i].depth) {
                                if (dataYes[0].values[i].yvalue == null) {
                                    data_table_three[j][3] = "无";
                                } else {
                                    data_table_three[j][3] = dataYes[0].values[i].yvalue;
                                }
                            }
                        }
                    }
                } else {
                    for (var i = 0; i < data_table_three.length; i++) {
                        data_table_three[i][3] = "无";
                    }
                }
            } else {
                for (var i = 0; i < data_table_three.length; i++) {
                    data_table_three[i][3] = "无";
                }
            }

            if (dataBefYes.length > 0) {
                if (dataBefYes[0].values.length > 0) {
                    for (var i = 0; i < dataBefYes[0].values.length; i++) {
                        for (var j = 0; j < data_table_three.length; j++) {
                            if (data_table_three[j][1] == dataBefYes[0].values[i].depth) {
                                if (dataBefYes[0].values[i].yvalue == null) {
                                    data_table_three[j][4] = "无";
                                } else {
                                    data_table_three[j][4] = dataBefYes[0].values[i].yvalue;
                                }
                            }
                        }
                    }
                } else {
                    for (var i = 0; i < data_table_three.length; i++) {
                        data_table_three[i][4] = "无";
                    }
                }
            } else {
                for (var i = 0; i < data_table_three.length; i++) {
                    data_table_three[i][4] = "无";
                }
            }

            for (var k = 0; k < data_table_three.length; k++) {
                if (data_table_three[k][2] == "无" || data_table_three[k][3] == "无") {
                    data_table_three[k][5] = "无";
                } else {
                    data_table_three[k][5] = (data_table_three[k][2] - data_table_three[k][3]).toFixed(6);
                }

                if (data_table_three[k][3] == "无" || data_table_three[k][4] == "无") {
                    data_table_three[k][6] = "无";
                } else {
                    data_table_three[k][6] = (data_table_three[k][3] - data_table_three[k][4]).toFixed(6);
                }
            }

            tableManager('show_table_comp', data_table_three, ['管组名称', '深度(m)', '今天累计值(mm)', '昨天累计值(mm)', '前天累计值(mm)', '今天变化量(mm)', '昨天变化量(mm)'], 10);

        },//success
        error: function () {
            tableManager('show_table_comp', data_table_three, ['管组名称', '深度(m)', '今天累计值(mm)', '昨天累计值(mm)', '前天累计值(mm)', '今天变化量(mm)', '昨天变化量(mm)'], 10);
        }
    });//累计
}
/***********end 对比数据**********/

function graph_display_leiji() {
    switch (direct) {
        case "x":
            var x = document.getElementById("X_leiji");
            x.style.width = "20%";
            $('#X_leiji').show();
            $('#Y_leiji').hide();
            $('#Y_leiji_error').hide();
            $('#X_leiji_error').hide();
            break;
        case "y":
            var y = document.getElementById("X_leiji");
            y.style.width = "20%";
            $('#X_leiji').hide();
            $('#Y_leiji').show();
            $('#Y_leiji_error').hide();
            $('#X_leiji_error').hide();
            break;
        case "xy":
            var x = document.getElementById("X_leiji");
            x.style.width = "20%";
            var y = document.getElementById("Y_leiji");
            y.style.width = "20%";
            $('#X_leiji').show();
            $('#Y_leiji').show();
            $('#Y_leiji_error').hide();
            $('#X_leiji_error').hide();
            break;
    }
}

function graph_display_qushi() {
    switch (direct) {
        case "x":
            var x = document.getElementById("X_dot");
            x.style.width = "73%";
            $('#X_dot').show();
            $('#Y_dot').hide();
            $('#Y_dot_error').hide();
            $('#X_dot_error').hide();
            break;
        case "y":
            var y = document.getElementById("Y_dot");
            y.style.width = "73%";
            $('#X_dot').hide();
            $('#Y_dot').show();
            $('#Y_dot_error').hide();
            $('#X_dot_error').hide();
            break;
        case "xy":
            var x = document.getElementById("X_dot");
            var y = document.getElementById("Y_dot");
            x.style.width = "53%";
            y.style.width = "53%";
            $('#X_dot').show();
            $('#Y_dot').show();
            $('#Y_dot_error').hide();
            $('#X_dot_error').hide();
            break;
    }
}

function getSensorList() {
    var s_id = document.getElementById('HiddenSensorId').value;
    $('#sensorList').children().remove();
    var structid = getCookie("nowStructId");
    var url = apiurl + '/struct/' + structid + '/factor/deep-displace/groups?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',

        success: function (data) {
            if (data.length == 0) {
                $('.selectpicker').selectpicker('refresh');
                return;
            }
            var value_id = [];
            for (var index = 0; index < data.length; index++) {
                var value = [data[index].groupId, data[index].maxDepth];
                $('#sensorList').append('<option value=\'' + value + '\'>' + data[index].groupName + '</option>');
                if (s_id != undefined || s_id != null || s_id != "" || s_id != 0) {
                    if (s_id == value[0]) {
                        value_id = data[index].groupName;
                    }
                }
            }
            if (value_id.length != 0) {
                var title = document.getElementById('sensorList');
                for (var i = 0; i < title.options.length; i++) {
                    if (title.options[i].innerHTML == value_id) {
                        title.options[i].selected = true;
                        break;
                    }
                }
            }
            $('#sensorList').selectpicker('refresh');
            loadChart();
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

function getMonitorData(groupId, maxDepth, groupName) {
    //interval = (Date.parse(getEnddate().replace(/-/ig, '/')) - Date.parse(getStartdate().replace(/-/ig, '/'))) / 1000 / 60 / 60 / 24;

    interval = getInteval();
    if (direct == undefined)
        direct = "y";
    var url_date_depth = apiurl + '/deep-displace/' + groupId + '/data-by-depth/' + direct + '/' + getStartdate() + '/' + getEnddate() + '' + '?token=' + getCookie('token');//趋势
    var url_date_time = apiurl + '/deep-displace/' + groupId + '/data-by-time/' + direct + '/' + getStartdate() + '/' + getEnddate() + '' + '?token=' + getCookie('token');//累计

    var url_daily_depth = apiurl + '/deep-displace/' + groupId + '/daily-data-by-depth/' + direct + '/' + getStartdate() + '/' + getEnddate() + '' + '?token=' + getCookie('token');//日表 趋势
    var url_daily_time = apiurl + '/deep-displace/' + groupId + '/daily-data-by-time/' + direct + '/' + getStartdate() + '/' + getEnddate() + '' + '?token=' + getCookie('token');//日表 累计
    if (interval <= 6) {
        if (interval != 1) {
            if (direct == "xy") {
                XY_direct(url_date_depth, url_daily_time, maxDepth, groupName);
            }
            else if (direct == "x") {
                X_direct(url_date_depth, url_daily_time, maxDepth, groupName);
            }
            else {
                Y_direct(url_date_depth, url_daily_time, maxDepth, groupName);
            }
        }
        else {
            if (direct == "xy") {
                XY_direct(url_date_depth, url_date_time, maxDepth, groupName);
            }
            else if (direct == "x") {
                X_direct(url_date_depth, url_date_time, maxDepth, groupName);
            }
            else {
                Y_direct(url_date_depth, url_date_time, maxDepth, groupName);
            }
        }//
    }//interval <= 6
    else {
        if (direct == "xy") {
            XY_direct(url_daily_depth, url_daily_time, maxDepth, groupName);
        }
        else if (direct == "x") {
            X_direct(url_daily_depth, url_daily_time, maxDepth, groupName);
        }
        else {
            Y_direct(url_daily_depth, url_daily_time, maxDepth, groupName);
        }
    }
}

function XY_direct(url_depth, url_time, maxDepth, groupName) {

    $.ajax({
        url: url_depth,
        type: 'get',

        success: function (data) {
            data_depth_time_x = [];
            data_depth_time_y = [];
            data_table_values = [];//下次查询前清空
            if (data.length == 0) {
                display_qushi('block');
                tableManager('show_table', data_table_values, ['管组名称', '深度(m)', 'X方向位移(mm)', 'Y方向位移(mm)', '采集时间']);
                return;
            }
            else {
                graph_display_qushi();
                display_qushi('none');
            }
            for (var i = 0; i < data.length; i++) {
                data_values_x = [];
                data_values_y = [];
                for (var j = 0; j < data[i].values.length; j++) {
                    var depth = data[i].depth;
                    var time = data[i].values[j].acquistiontime.substring(6, 19);
                    if (data[i].values[j].xvalue != null) {
                        var value_x = [parseInt(time), data[i].values[j].xvalue];
                        data_values_x.push(value_x);
                    }
                    if (data[i].values[j].yvalue != null) {
                        var value_y = [parseInt(time), data[i].values[j].yvalue];
                        data_values_y.push(value_y);
                    }
                    if (data[i].values[j].xvalue != null || data[i].values[j].yvalue != null) {
                        var value_table_xy = [groupName, depth, data[i].values[j].xvalue, data[i].values[j].yvalue, time];
                        data_table_values.push(value_table_xy);
                    }
                }
                data_values_x = [depth, data_values_x];
                data_values_y = [depth, data_values_y];

                data_depth_time_x.push(data_values_x);
                data_depth_time_y.push(data_values_y);
            }
            createLine({
                renderTo: 'X_dot',
                titleText: groupName + '管道' + 'x方向趋势',
                subtitleText: '趋势',
                yAxisTitleText: '位移(mm)',
                seriesList: [{
                    dataCollect: data_depth_time_x
                }]
            });
            createLine({
                renderTo: 'Y_dot',
                titleText: groupName + '管道' + 'y方向趋势',
                subtitleText: '趋势',
                yAxisTitleText: '位移(mm)',
                seriesList: [{
                    dataCollect: data_depth_time_y
                }]
            });
            tableManager('show_table', data_table_values, ['管组名称', '深度(m)', 'X方向位移(mm)', 'Y方向位移(mm)', '采集时间']);
        },//success
        error: function () {
            data_table_values = [];
            display_qushi('block');
            tableManager('show_table', data_table_values, ['管组名称', '深度(m)', 'X方向位移(mm)', 'Y方向位移(mm)', '采集时间']);
            //error_alert();
        }
    });//趋势

    $.ajax({
        url: url_time,
        type: 'get',

        success: function (data) {
            data_depth_time_y = [];
            data_depth_time_x = [];
            data_table_values = [];//下次查询前清空
            if (data.length == 0) {
                display_leiji('block');
                return;
            }
            else {
                graph_display_leiji();
                display_qushi('none');
            }
            var i = 0;
            if (interval == 1) {
                i = data.length - 1;
            }
            for (i ; i < data.length; i++) {
                data_values_y = [];
                data_values_x = [];
                for (var j = 0; j < data[i].values.length; j++) {
                    var time = new Date(parseInt(data[i].acquistiontime.substring(6, 19)));
                    time = time.getFullYear() + "-" + (time.getMonth() + 1) + "-" + time.getDate() + " " + time.getHours() + ":" + time.getMinutes() + ":" + time.getSeconds();
                    if (j == 0) {
                        var value = [parseFloat(maxDepth), 0];
                        //data_values_y.push(value);
                        //data_values_x.push(value);
                    }
                    if (data[i].values[j].yvalue != null) {
                        var value_y = [data[i].values[j].depth, data[i].values[j].yvalue];
                        data_values_y.push(value_y);
                    }
                    if (data[i].values[j].xvalue != null) {
                        var value_x = [data[i].values[j].depth, data[i].values[j].xvalue];
                        data_values_x.push(value_x);
                    }
                }
                data_values_y = [time, data_values_y];
                data_values_x = [time, data_values_x];
                data_depth_time_y.push(data_values_y);
                data_depth_time_x.push(data_values_x);
            }
            createLine_leiji({
                renderTo: 'X_leiji',
                titleText: groupName + '管道' + 'x方向累计位移',
                subtitleText: '累计',
                yAxisTitleText: '深度(m)',
                seriesList: [{
                    dataCollect: data_depth_time_x
                }]
            });
            createLine_leiji({
                renderTo: 'Y_leiji',
                titleText: groupName + '管道' + 'y方向累计位移',
                subtitleText: '累计',
                yAxisTitleText: '深度(m)',
                seriesList: [{
                    dataCollect: data_depth_time_y
                }]
            });
        },//success
        error: function () {
            display_leiji('block');
            //error_alert();
        }
    });//累计

}//XY_direct

function X_direct(url_depth, url_time, maxDepth, groupName) {
    $.ajax({
        url: url_depth,
        type: 'get',

        success: function (data) {
            data_depth_time_x = [];
            data_table_values = [];//下次查询前清空
            if (data.length == 0) {
                display_qushi('block');
                tableManager('show_table', data_table_values, ['管组名称', '深度(m)', 'X方向位移(mm)', '采集时间']);
                return;
            }
            else {
                graph_display_qushi();
                display_qushi('none');
            }
            for (var i = 0; i < data.length; i++) {
                data_values_x = [];
                for (var j = 0; j < data[i].values.length; j++) {
                    var depth = data[i].depth;
                    var time = data[i].values[j].acquistiontime.substring(6, 19);
                    if (data[i].values[j].xvalue != null) {
                        var value = [parseInt(time), data[i].values[j].xvalue]
                        var value_table = [groupName, depth, data[i].values[j].xvalue, time];
                        data_values_x.push(value);
                        data_table_values.push(value_table);
                    }

                }
                data_values_x = [depth, data_values_x];
                data_depth_time_x.push(data_values_x);
            }
            createLine({
                renderTo: 'X_dot',
                titleText: groupName + '管道' + direct + '方向趋势',
                subtitleText: '趋势',
                yAxisTitleText: '位移(mm)',
                seriesList: [{
                    dataCollect: data_depth_time_x
                }]
            });
            tableManager('show_table', data_table_values, ['管组名称', '深度(m)', 'X方向位移(mm)', '采集时间']);
        },//success
        error: function () {
            data_table_values = [];
            display_qushi('block');
            tableManager('show_table', data_table_values, ['管组名称', '深度(m)', 'X方向位移(mm)', '采集时间']);
            //error_alert();
        }
    });//趋势

    $.ajax({
        url: url_time,
        type: 'get',

        success: function (data) {
            if (data.length == 0) {
                display_leiji('block');
                return;
            }
            else {
                graph_display_leiji();
                display_qushi('none');
            }
            data_depth_time_x = [];
            var i = 0;
            if (interval == 1) {
                i = data.length - 1;
            }
            for (i ; i < data.length; i++) {
                data_values_x = [];
                for (var j = 0; j < data[i].values.length; j++) {
                    var time = new Date(parseInt(data[i].acquistiontime.substring(6, 19)));
                    time = time.getFullYear() + "-" + (time.getMonth() + 1) + "-" + time.getDate() + " " + time.getHours() + ":" + time.getMinutes() + ":" + time.getSeconds();
                    if (j == 0) {
                        var value = [parseFloat(maxDepth), 0];
                        //data_values_x.push(value);
                    }
                    if (data[i].values[j].xvalue != null) {
                        var value_x = [data[i].values[j].depth, data[i].values[j].xvalue];
                        data_values_x.push(value_x);
                    }
                }
                data_values_x = [time, data_values_x];
                data_depth_time_x.push(data_values_x);
            }
            createLine_leiji({
                renderTo: 'X_leiji',
                titleText: groupName + '管道' + direct + '方向累计位移',
                subtitleText: '累计',
                yAxisTitleText: '深度(m)',
                seriesList: [{
                    dataCollect: data_depth_time_x
                }]
            });
        },//success
        error: function () {
            display_leiji('block');
            //error_alert();
        }
    });//累计

}//X_direct

function Y_direct(url_depth, url_time, maxDepth, groupName) {
    $.ajax({
        url: url_depth,
        type: 'get',

        success: function (data) {
            //alert("aa");
            data_depth_time_y = [];
            data_table_values = [];//下次查询前清空
            if (data.length == 0) {
                display_qushi('block');
                tableManager('show_table', data_table_values, ['管组名称', '深度(m)', 'Y方向位移(mm)', '采集时间']);
                return;
            }
            else {
                graph_display_qushi();
                display_qushi('none');
            }
            for (var i = 0; i < data.length; i++) {
                data_values_y = [];
                for (var j = 0; j < data[i].values.length; j++) {
                    var depth = data[i].depth;
                    var time = data[i].values[j].acquistiontime.substring(6, 19);
                    if (data[i].values[j].yvalue != null) {
                        var value = [parseInt(time), data[i].values[j].yvalue];
                        var value_table = [groupName, depth, data[i].values[j].yvalue, time];
                        data_values_y.push(value);
                        data_table_values.push(value_table);
                    }
                }
                data_values_y = [depth, data_values_y];
                data_depth_time_y.push(data_values_y);
            }
            createLine({
                renderTo: 'Y_dot',
                titleText: groupName + '管道' + direct + '方向趋势',
                subtitleText: '趋势',
                yAxisTitleText: '位移(mm)',
                seriesList: [{
                    dataCollect: data_depth_time_y
                }]
            });
            tableManager('show_table', data_table_values, ['管组名称', '深度(m)', 'Y方向位移(mm)', '采集时间']);
        },//success
        error: function () {
            data_table_values = [];
            display_qushi('block');
            tableManager('show_table', data_table_values, ['管组名称', '深度(m)', 'Y方向位移(mm)', '采集时间']);
            //error_alert();
        }
    });//趋势

    $.ajax({
        url: url_time,
        type: 'get',

        success: function (data) {
            if (data.length == 0) {
                display_leiji('block');
                return;
            }
            else {
                graph_display_leiji();
                display_qushi('none');
            }
            data_depth_time_y = [];
            data_table_values = [];//下次查询前清空
            var i = 0;
            if (interval == 1) {
                i = data.length - 1;
            }
            for (i ; i < data.length; i++) {
                data_values_y = [];
                for (var j = 0; j < data[i].values.length; j++) {
                    var time = new Date(parseInt(data[i].acquistiontime.substring(6, 19)));
                    time = time.getFullYear() + "-" + (time.getMonth() + 1) + "-" + time.getDate() + " " + time.getHours() + ":" + time.getMinutes() + ":" + time.getSeconds();
                    if (j == 0) {
                        var value = [parseFloat(maxDepth), 0];
                        //data_values_y.push(value);
                    }
                    if (data[i].values[j].yvalue) {
                        var value_y = [data[i].values[j].depth, data[i].values[j].yvalue];
                        data_values_y.push(value_y);
                    }

                }
                data_values_y = [time, data_values_y];
                data_depth_time_y.push(data_values_y);
            }
            createLine_leiji({
                renderTo: 'Y_leiji',
                titleText: groupName + '管道' + direct + '方向累计位移',
                subtitleText: '累计',
                yAxisTitleText: '深度(m)',
                seriesList: [{
                    dataCollect: data_depth_time_y
                }]
            });
        },//success
        error: function () {
            display_leiji('block');
            //error_alert();
        }
    });//累计
}//Y_direct



function createLine(parameters) {

    var chartObj = {
        chart: {
            renderTo: parameters.renderTo,
            type: 'spline',
            zoomType: 'x'
        },
        credits: {
            enabled: false,
            //    href: 'http://www.free-sun.com.cn',
            //    text: '江西飞尚科技有限公司'
        },
        title: {
            text: parameters.titleText,
            style: {
                color: '#339900',
                fontWeight: 'bold'
            }
        },
        subtitle: {
            style: {
                color: '#339900',
                fontWeight: 'bold'
            }
        },
        xAxis: {
            type: 'datetime',
            dateTimeLabelFormats: {
                second: '%H:%M:%S',
                minute: '%H:%M:%S',
                hour: '%H:%M:%S',
                day: '%Y-%b-%e',
                week: '%Y-%b',
                month: '%Y-%b',
                year: '%Y'
            },
            labels: { rotation: -25, align: 'right', style: { font: 'normal 13px Verdana, sans-serif' } }
        },
        yAxis: {
            title: {
                text: parameters.yAxisTitleText
            }
        },
        tooltip: {
            formatter: function () {
                var tooltipString = '<b>' + this.series.name + '</b>';
                tooltipString = tooltipString + '<br/><br/>采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                tooltipString = tooltipString + '<br/>' + '监测数据:' + this.y.toString() + "mm";
                return tooltipString;
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle',
            borderWidth: 0
        }
    };

    chartObj.series = [];
    var datalist = parameters.seriesList[0].dataCollect;
    for (var index = 0; index < datalist.length; index++) {
        var d = datalist[index][0];
        chartObj.series.push({
            name: "深度 " + datalist[index][0] + "m",
            data: datalist[index][1],
            marker: {
                enabled: false
            }
        });
    }
    var highLine = new Highcharts.Chart(chartObj);
}

function createLine_leiji(parameters) {

    var chartObj = {
        chart: {
            renderTo: parameters.renderTo,
            type: 'spline',
            inverted: true//可选，控制显示方式，默认上下正向显示  
        },
        credits: {
            enabled: false,
            //    href: 'http://www.free-sun.com.cn',
            //    text: '江西飞尚科技有限公司'
        },
        title: {
            text: parameters.titleText,
            style: {
                color: '#339900',
                fontWeight: 'bold'
            }
        },
        subtitle: {
            style: {
                color: '#339900',
                fontWeight: 'bold'
            }
        },
        yAxis: {
            gridLineWidth: 0,
            title: {
                text: '位移(mm)'
            }
        },
        xAxis: {
            reversed: false,
            gridLineWidth: 1,
            title: {
                text: '深度(m)'
            }
        },
        tooltip: {
            formatter: function () {
                var tooltipString = '<b>' + this.series.name + '</b>';
                tooltipString = tooltipString + '<br/><br/>位移:' + this.y + ' mm';
                tooltipString = tooltipString + '<br/>' + '深度:' + this.x + ' m';
                return tooltipString;
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle',
            borderWidth: 0,
            floating: true,
            x: -10,
            y: 200
        }
    };

    chartObj.series = [];
    var datalist = parameters.seriesList[0].dataCollect;
    for (var index = 0; index < datalist.length; index++) {
        chartObj.series.push({
            name: datalist[index][0],
            data: datalist[index][1],
            marker: {
                enabled: true
            }
        });
    }
    var highLine = new Highcharts.Chart(chartObj);
}









