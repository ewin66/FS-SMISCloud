
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

function display(comm1) {
    if (comm1 == "block") {
        $('#comm1_error').show();
        $('#comm1').hide();
    }
    else {
        $('#comm1_error').hide();
        $('#comm1').show();
    }
}

$(function () {
    Highcharts.setOptions({
        lang: {
            printChart: "打印",
            downloadJPEG: "下载JPEG 图片",
            downloadPDF: "下载PDF文档",
            downloadPNG: "下载PNG 图片",
            downloadSVG: "下载SVG 矢量图",
            exportButtonTitle: "导出图片"
        }
    });
    var id = "comm1_error";
    errorTip(id);

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
    

    $('.btndataquery').click(function () {
        dateValue($('#date').val());//重新获取起始时间
        loadChart();
    });
    $("#date").change(function () {

        loadChart();
    });
    $("#sensorList").change(function () {

        loadChart();
        loadDailyReportChart();

    });
    $('#btn_Query').click(function () {
        loadDailyReportChart();
    });

});

function loadChart() {
    $('#show_table').children().remove();
    var sensorList = [];
    var sensorId = $('#sensorList').find('option:selected');
    for (var i = 0; i < sensorId.length; i++) {
        sensorList[i] = sensorId[i].value;
    }
    if (sensorList.length == 0) {
        display('block');
    }
    else {
        //display('none');
        getMonitorData(sensorList);
        document.getElementById("expand_collapse").src = '../resource/img/toggle-collapse.png';
    }

};

function getSensorList() {
    $('#sensorList').children().remove();
    var structid = getCookie("nowStructId");

    var url = apiurl + '/struct/' + structid + '/sensor-group/chenjiang?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',

        success: function (data) {
            if (data.length == 0) {
                return;
            }
            var option = '';
            for (var i = 0; i < data.length; i++) {
                option += '<option value="' + data[i].groupId + '">' + data[i].groupName + '</option>';
            }
            $('#sensorList').html(option);
            $('#sensorList').selectpicker();
            loadChart();
            loadDailyReportChart();
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

function getMonitorData(groupId) {
    var url = apiurl + "/settle/" + groupId + "/daily-data/" + getStartdate() + '/' + getEnddate() + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        success: function (data) {
            data_table_values = []; //清空列表
            if (data.data.length == 0) {
                display('block');
                return;
            } else {
                display('none');
                var tableValue = createHighchartComm1(data);
                tableManager('show_table', tableValue, ['设备位置', '位置距离(m)', '变化值(mm)', '采样时间'], -1);
            }
        },
        error: function () {
            display('block');
        }
    });
}

function createHighchartComm1(data) {
    var chartObj = {
        chart: {
            renderTo: 'comm1',
            type: 'line',
            zoomType: 'x'
        },
        credits: {
            enabled: false
        },
        title: {
            text: '组对比趋势图',
            style: {
                color: '#339900',
                fontWeight: 'bold'
            }
        },
        subtitle: {
            text: '',
            style: {
                color: '#339900',
                fontWeight: 'bold'
            }
        },
        xAxis: {
            title: {
                text: '位置距离(m)'
            },
            labels: {
                rotation: -25,
                align: 'right',
                style: { font: 'normal 13px Verdana, sans-serif' }
            }
        },
        yAxis: {
            title: {
                text: '变化值(mm)'
            },
        },
        tooltip: {
            useHTML: true,
            formatter: function () {
                var s = '<div style="max-height:250px;overflow-y:scroll"><b>位置:' + this.points[0].point.name + '-距离' + this.x + 'm</b>';

                $.each(this.points, function () {
                    s += '<br/>' + this.series.name + '-变化值:' +
                        this.y + 'mm';
                });

                return s += '</div>';
            },
            hideDelay: 30000,
            shared: true,
            crosshairs: true
        }

    };

    chartObj.series = [];
    var data_table_values = [];
    for (var i = 0; i < data.data.length; i++) {
        var settleValues = data.data[i].values;
        var sensorSettle = new Array();
        var time = data.data[i].acquistiontime.substring(6, 19);
        for (var j = 0; j < settleValues.length; j++) {
            if (settleValues[j].value != null) {
                sensorSettle.push({
                    name: settleValues[j].location,
                    x: settleValues[j].len,
                    y: settleValues[j].value
                });
                var value_table = [settleValues[j].location, settleValues[j].len, settleValues[j].value, toDate(time)];
                data_table_values.push(value_table);
            }
        }
        chartObj.series.push({
            name: toDate(time),
            data: sensorSettle
        });
    }

    var highLine = new Highcharts.Chart(chartObj);
    return data_table_values;
}

function toDate(json) {
    var dateTime = new Date(parseInt(json));
    var year = dateTime.getFullYear();
    var month = dateTime.getMonth() + 1;
    var date = dateTime.getDate();

    return year + "-" + month + "-" + date;
}


function loadDailyReportChart() {
    var sensorList = [];
    var sensorId = $('#sensorList').find('option:selected');
    for (var i = 0; i < sensorId.length; i++) {
        sensorList[i] = sensorId[i].value;
    }
    if (sensorList.length > 0) {
        $('#settleDaliyReport').show();
    }
    getSettleDaliyReportTodInfo(sensorList);
    document.getElementById("Img1").src = '../resource/img/toggle-collapse.png';
};

var dataTod = [];
var dataYes = [];
var dataBefYes = [];
//通过改变algorithm的值，选择累计变量生成的方法：这里algorithm=1（选择第一个不为空的值）
//这里algorithm=2（选择零点到早上8点的均值）
var algorithm = 1;

//获取表格信息
function getSettleDaliyReportTodInfo(groupId) {
    var myDate = new Date($('#dpform3').val());
    myDate.setDate(myDate.getDate() - 1);//昨天
    var yesterdayCEnd = getyesterdayEnd(myDate);
    var yesterdayCStart = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + ' 00:00:00';
    myDate.setDate(myDate.getDate() - 1);//前天
    var beYesterdayCEnd = getBefYesterdayEnd(myDate);
    var beYesterdayCStart = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + ' 00:00:00';

    var url = apiurl + '/settlement/' + groupId + '/daily-report/' + getReportStartdate() + '/' + getReportEnddate() + '/' + algorithm + '/info?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            dataTod = data;
            getSettleDaliyReportYesInfo(groupId, yesterdayCStart, yesterdayCEnd, beYesterdayCStart, beYesterdayCEnd);
        },
        error: function () {
            dataTod = null;
            getSettleDaliyReportYesInfo(groupId, yesterdayCStart, yesterdayCEnd, beYesterdayCStart, beYesterdayCEnd);
        }
    });
}


function getSettleDaliyReportYesInfo(groupId, yesterdayCStart, yesterdayCEnd, beYesterdayCStart, beYesterdayCEnd) {
    var url = apiurl + '/settlement/' + groupId + '/daily-report/' + yesterdayCStart + '/' + yesterdayCEnd + '/' + algorithm + '/info?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            dataYes = data;
            getSettleDaliyReportBefYesInfo(groupId, beYesterdayCStart, beYesterdayCEnd);
        },
        error: function () {
            dataYes = null;
            getSettleDaliyReportBefYesInfo(groupId, beYesterdayCStart, beYesterdayCEnd);
        }
    });
}


function getSettleDaliyReportBefYesInfo(groupId, beYesterdayCStart, beYesterdayCEnd) {
    var url = apiurl + '/settlement/' + groupId + '/daily-report/' + beYesterdayCStart + '/' + beYesterdayCEnd + '/' + algorithm + '/info?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            dataBefYes = data;
            fillDataTable();
        },
        error: function () {
            dataBefYes = null;
            fillDataTable();
        }
    });
}

//计算变化值
function getValitorData(sensorId, data, value) {
    var valitorData = "无";
    for (var i = 0; i < data.length; i++) {
        var sId = data[i].SensorId;
        if (sId == sensorId) {
            valitorData = getValue(value, data[i].Value);
        }
    }
    return valitorData;
}

//数据格式
function getValue(a, b) {
    var value = parseFloat(a - b);
    return $.number(value, 1);
}

function fillDataTable() {

    var time = $('#dpform3').val();
    var sb = new StringBuffer();
    $('#tableSettleDaliyReport').dataTable().fnDestroy();
    if (dataTod.length > 0) {
        for (var i = 0; i < dataTod.length; i++) {
            var sensorId = dataTod[i].SensorId;
            var location = dataTod[i].Location;
            var nowValue = dataTod[i].Value;
           // var ab = i + 1;
            //sb.append("<tr id='sensorId_" + sensorId + "'><td style='display: none;'>" + ab + "</td>");
           
            sb.append("<tr><td>" + location + "</td>");
            sb.append("<td>" + $.number(nowValue,1) + "</td>");
            var currentValitorValue = getValitorData(sensorId, dataYes, nowValue);
            sb.append("<td>" + currentValitorValue + "</td>");
            if (currentValitorValue != "无") {
                var yesValue = nowValue - currentValitorValue;
                var lastValitorValue = getValitorData(sensorId, dataBefYes, yesValue);

                sb.append("<td>" + lastValitorValue + "</td>");

            } else {
                sb.append("<td>" + "无" + "</td>");
            }
            sb.append("<td>" + time + "</td>");
            sb.append("</tr>");
        }
    } else {
        if (dataYes.length > 0 && dataBefYes.length > 0) {
            for (var l = 0; l < dataYes.length; l++) {
                var sensor = dataYes[l].SensorId;
                for (var m = 0; m < dataBefYes.length; m++) {
                    var sZd = dataBefYes[m].SensorId;
                    if (sensor == sZd) {
                        var value = $.number(dataYes[l].Value - dataBefYes[m].Value, 1);
                       // var a = m + 1;
                        //sb.append("<tr id='sensorId_" + sensor + "'><td style='display: none;'>" + a + "</td>");
                        sb.append("<tr><td>" + dataBefYes[m].Location + "</td>");
                        sb.append("<td>" + "无" + "</td>");
                        sb.append("<td>" + "无" + "</td>");
                        sb.append("<td>" + value + "</td>");
                        sb.append("<td>" + time + "</td>");
                        sb.append("</tr>");
                    }
                }
            }
        }
    }

    $('#tbodySettleDaliyReport').html("");
    $('#tbodySettleDaliyReport').html(sb.toString());
    SettleDaliyReport_Datatable();

}
function SettleDaliyReport_Datatable() {
    $('#tableSettleDaliyReport').dataTable({
        "sDom": 'T<"clear">lfrtip',
        "iDisplayLength": 50, //每页显示个数 
        "bScrollCollapse": true,
        "bLengthChange": true,  //每页显示的记录数 
        "bPaginate": true,  //是否显示分页
        "bFilter": true, //搜索栏
        "bSort": true, //是否支持排序功能
        "bInfo": true, //显示表格信息
        //"bAutoWidth": false,  //自适应宽度
        "bStateSave": false, //保存状态到cookie *************** 很重要，当搜索的时候页面一刷新会导致搜索的消失。使用这个属性就可避免了

        //"sPaginationType": "bootstrap",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "aaSorting": [[4, "desc"]],
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
}

function showdate1(n) {
    var uom = new Date();
    uom.setDate(uom.getDate() + n);
    uom = uom.getFullYear() + "-" + (uom.getMonth() + 1) + "-" + uom.getDate();
    return uom.replace(/\b(\w)\b/g, '0$1');
}
$('#dpform3').val(showdate1(0));
$('#dpform4').datetimepicker({
    format: 'yyyy-MM-dd',
    language: 'pt-BR',
    pickTime: false
});

//今天
function getReportStartdate() {
    return $('#dpform3').val() + ' 00:00:00';
}

function getReportEnddate() {
    if (algorithm == 1) {
        return $('#dpform3').val() + ' 23:59:59';
    } else {
        return $('#dpform3').val() + ' 08:00:00';
    }
}

function getyesterdayEnd(myDate) {
    var yesterdayCEnd;
    if (algorithm == 1) {
        yesterdayCEnd = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + ' 23:59:59';
    } else {
        yesterdayCEnd = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + ' 08:00:00';
    }
    return yesterdayCEnd;
}

function getBefYesterdayEnd(myDate) {
    var befYesterdayCEnd;
    if (algorithm == 1) {
        befYesterdayCEnd = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + ' 23:59:59';
    } else {
        befYesterdayCEnd = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + ' 08:00:00';
    }
    return befYesterdayCEnd;
}
