var structId = getCookie("nowStructId");

var chart_temple = {
    chart: {
        type: 'line',
        renderTo: 'comm_graph2'
    },
    title: {
        text: '浸润线状态示意图'
    },
    subtitle: {
        text: ''
    },
    xAxis: {
        title: {
            text: '子坝高度(m)'
        }
    },
    yAxis: {
        title: {
            text: '浸润线高程(m)'
        }
    },
    credits: {
        enabled: false
    },
    plotOptions: {
        line: {
            dataLabels: {
                enabled: true
            },
            enableMouseTracking: false
        }
    },
    series: []
};

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
    var factorid = document.getElementById('HiddenFactorNo').value;

    $('#monitorList').addClass('active');

    setTimeout(function () {
        $('#factor_' + factorid).addClass('active');
    }, 2000);

    $('#date').selectpicker('refresh');

    var sensorId = getUrlParam('sensorId');

    getGroup(structId, sensorId);
    getInfiltrationLine();
    getInfiltrationGraph();

    $('#btnQuery').click(function () {
        dateValue($('#date').val());//重新获取起始时间
        getInfiltrationLine();
    });

    $('#date').change(function () {
        getInfiltrationLine();
    });

    $('a.box-collapse').click(function () {
        var $target = $(this).parent().parent().next('.box-content');
        if ($target.is(':visible')) {
            $('img', this).attr('src', '../resource/img/toggle-expand.png');
        } else {
            $('img', this).attr('src', '../resource/img/toggle-collapse.png');
        }
        $target.slideToggle();
    });
});

function getGroup(structId, sensorId) {
    var url = apiurl + '/struct/' + structId + '/sensor-group/jinrunxian' + '?token=' + getCookie('token');

    $.ajax({
        url: url,
        type: 'get',
        async: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                return;
            } else {
                // 绑定下拉框
                var option = '';
                for (var i = 0; i < data.length; i++) {
                    option += '<option value="' + data[i].groupId + '">' + data[i].groupName + '</option>';
                }
                $('#sensorList').html(option);
                // 添加图表容器
                var sb = new StringBuffer();
                for (var i = 0; i < data.length; i++) {
                    sb.append('<div class="box" id="comm' + data[i].groupId + '" style="height:200px;display:none;"></div>');
                }
                $('#comm_graph1').html(sb.toString());
                // 判定是否有sensorId传入,显示对应的容器
                if (sensorId == null || sensorId == '' || isNaN(sensorId)) {
                    $('#comm' + data[0].groupId).show();
                    $('#sensorList [value="' + data[0].groupId + '"]').attr('selected', 'selected');
                } else {
                    for (var i = 0; i < data.length; i++) {
                        var sensors = data[i].sensorList;
                        for (var j = 0; j < sensors.length; j++) {
                            if (sensors[j].sensorId == sensorId) {
                                $('#comm' + data[i].groupId).show();
                                $('#sensorList [value="' + data[i].groupId + '"]').attr('selected', 'selected');
                                break;
                            }
                        }                        
                    }
                }
                $('#sensorList').selectpicker('refresh');
            }
        }
    });
}

function getInfiltrationLine() {
    // 分组
    var sensorId_array = $('#sensorList').find('option:selected');
    var sensorArry = [];
    for (var i = 0; i < sensorId_array.length; i++) {
        sensorArry.push(sensorId_array[i].value);
    }

    var groupIds = '';
    for (var i = 0; i < sensorArry.length; i++) {
        groupIds += sensorArry[i];
        if (i != sensorArry.length - 1) {
            groupIds += ',';
        }
    }

    var url = apiurl + '/saturation-line/' + groupIds + '/data/' + getStartdate() + '/' + getEnddate() + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            $('#show_table').children().remove();
            var data_table_values = [];

            var options = $('#sensorList').find('option');
            for (var l = 0; l < options.length; l++) {
                $('#comm' + options[l].value).hide();
            }

            if (data == null || data.length == 0) {
                return;
            } else {                
                for (var i = 0; i < data.length; i++) {
                    $('#comm' + data[i].groupId).show();
                    var chart = createHighchartComm1('comm' + data[i].groupId, data[i].groupName + '趋势图', null);
                    chart.series = [];
                    chart.plotOptions = {};
                    var flag = true;
                    for (var j = 0; j < data[i].items.length; j++) {
                        var location = data[i].items[j].location;
                        var array = new Array();
                        for (var k = 0; k < data[i].items[j].data.length; k++) {
                            var time = data[i].items[j].data[k].acquisitiontime.substring(6, 19);
                            if (data[i].items[j].data[k].height != null) {
                                array.push([parseInt(time), data[i].items[j].data[k].height]);
                                var value_table = [location, data[i].items[j].data[k].height, time];
                                data_table_values.push(value_table);
                            }
                        }
                        chart.series.push({ name: location, data: array });

                        flag = flag & (data[i].items[j].data.length == 0 ? true : false);
                    }
                    if (flag) {
                        $("#comm" + data[i].groupId).html('');
                        errorTip("comm" + data[i].groupId, data[i].groupName);
                    } else {
                        chart.yAxis.title = { text: '浸润线水位(m)' };
                        chart.tooltip.formatter = function () {
                            var tooltipString = '<b>' + this.series.name + '</b>';
                            tooltipString = tooltipString + '<br/><br/>采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                            tooltipString = tooltipString + '<br/>' + '监测数据:' + this.y.toString() + '<b>' + 'm' + '</b>';
                            return tooltipString;
                        };
                        chart.legend = {
                            layout: 'horizontal',
                            align: 'bottom',
                            verticalAlign: 'bottom',
                            borderWidth: 0,
                            x: 30,
                            margin: 0
                        }
                        chart.plotOptions = {
                            series: {
                                marker: {
                                    enabled: false
                                }
                            }
                        };

                        var highcharts = new Highcharts.Chart(chart);
                    }
                }
                tableManager('show_table', data_table_values, ['设备位置', '浸润线水位(m)', '采集时间']);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else {

            }
        }
    });
}

function getInfiltrationGraph() {
    var url = apiurl + '/struct/' + structId + '/saturation-line/height?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        success: function (data) {           
            if (data == null || data.length == 0) {
                return;
            }
            else {
                chart_temple.subtitle = { text: JsonToDateTime(data.collectTime) };
                var colorArray = [];

                for (var i = 0; i < data.data.length; i++) {
                    var array = new Array();
                    for (var j = 0; j < data.data[i].values.length; j++) {
                        if (data.data[i].values[j].height != null) {
                            array.push([data.data[i].values[j].depth, data.data[i].values[j].height]);
                        }
                    }
                    chart_temple.series.push({ name: data.data[i].groupName, data: array });
                    colorArray.push(RandomColor());
                }
                for (var i = 0; i < data.threshold.length; i++) {
                    var array = new Array();
                    for (var j = 0; j < data.threshold[i].values.length; j++) {
                        array[j] = [data.threshold[i].values[j].depth, data.threshold[i].values[j].height];
                    }
                    chart_temple.series.push({ name: data.threshold[i].groupName + '阈值', data: array });
                    colorArray.push("red");
                }
                chart_temple.colors = colorArray;
                var chart = new Highcharts.Chart(chart_temple);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else {

            }
        }
    })
}

//随机生成颜色
function RandomColor() {
    return '#' +
(function (color) {
    return (color += '0123456789abcdef'[Math.floor(Math.random() * 16)])
    && (color.length == 6) ? color : arguments.callee(color);
})('');
}

function errorTip(string_id, stringName) {
    var graph_id = string_id.split(',');
    var errorTipstring = '<div id=\'error\' class=\'row-fluid dataerror-tip\'>' +
        '<div class="span3">' +
        '<span class=\'label label-important\' style=\'margin-left: 5px;margin-top: 10px;\'>抱歉，' + stringName + '趋势图没有查询到任何有效的数据</span>' +
        '</div>' +
        '</div>';
    for (var i = 0; i < graph_id.length; i++) {
        $('#' + graph_id[i]).append(errorTipstring);
    }
}

function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}

//*   判断在数组中是否含有给定的一个变量值
//*   参数：
//*   obj：需要查询的值
//*    a：被查询的数组
//*   在a中查询obj是否存在，如果找到返回true，否则返回false。
//*   此函数只能对字符和数字有效
function contains(a, obj) {
    for (var i = 0; i < a.length; i++) {
        if (a[i] == obj) {
            return true;
        }
    }
    return false;
}