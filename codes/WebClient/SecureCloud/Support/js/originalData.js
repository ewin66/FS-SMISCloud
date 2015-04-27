/**
 * ---------------------------------------------------------------------------------
 * <copyright file="originalData.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2015 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述："原始数据查看"子菜单页面js文件
 *
 * 创建标识：
 *
 * 修改标识：PengLing20150403
 * 修改描述：增加"组织结构物"列表.
 * </summary>
 * ---------------------------------------------------------------------------------
 */

//全局变量
var g_listOrgStructs = {}; // 保存组织及组织下结构物列表
var factorId;
var sensorId;

// 页面初始
$(function () {
    $('#dataServices').addClass('active');
    $('#originalDataView').addClass('active');

    var currentUserId = getCookie("userId");
    if (currentUserId == "") {
        alert("获取用户id失败, 请检查浏览器Cookie是否已启用");
        return;
    }
    getOrgStructsListByUser(currentUserId);

    //loadChart(159, "2014-12-06 22:44:59", "2014-12-07 22:44:59");
    //loadChart(147, "2014-12-06 22:44:59", "2014-12-07 22:44:59");
});

/**
 * 获取用户下组织结构物列表
 */
function getOrgStructsListByUser(userId) {
    var url = apiurl + '/user/' + userId + '/org-structs/list' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            g_listOrgStructs = {}; // empty it

            if (data.length == 0) {
                alertTips('该用户没有组织', 'label-important', 'tip', 3000);
            } else {
                showOrgStructsListByUser(data);
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取用户下组织结构物列表时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function showOrgStructsListByUser(data) {
    var orgOptions = '';
    $.each(data.orgs, function (i, org) {
        orgOptions += '<option id="optionOrg-' + org.orgId + '">' + org.orgName + '</option>';
        var arrStruct = [];
        $.each(org.structs, function (j, struct) {
            arrStruct.push({ structId: struct.structId, structName: struct.structName });
        });
        g_listOrgStructs["org-" + org.orgId] = arrStruct; // assign value
    });
    $('#listOrg').html(orgOptions);
    // 筛选框
    $('#listOrg').chosen({
        no_results_text: "没有找到",
        allow_single_de: true
    });

    showOrgStructs();
}

/**
 * 显示当前组织下的结构物列表
 */
function showOrgStructs() {
    if (jQuery.isEmptyObject(g_listOrgStructs)) {
        alert('该用户可能没有组织');
        return;
    }

    var org = $('#listOrg').find('option:selected')[0];
    var orgId = parseInt(org.id.split('optionOrg-')[1]);
    var orgStructs = g_listOrgStructs["org-" + orgId];
    // 创建当前组织下的结构物列表
    var structOptions = '';
    $.each(orgStructs, function (j, struct) {
        structOptions += '<option id="optionStruct-' + struct.structId + '">' + struct.structName + '</option>';
    });
    // 刷新结构物列表,下面两行必须！
    $('#structList').removeClass('chzn-done');
    $('#structList_chzn').remove();
    $('#structList').html(structOptions);
    // 筛选框,必须！
    $('#structList').chosen({
        no_results_text: "没有找到",
        allow_single_de: true
    });

    refreshFactorList();
}

//刷新监测因素列表
function refreshFactorList() {
    var struct = $('#structList').find('option:selected')[0];
    var structId = parseInt(struct.id.split('optionStruct-')[1]);
    factorListView(structId);
    obj = document.getElementById("factorList");
    if (obj.selectedIndex < 0) {
        factorId = obj.selectedIndex;
    } else {
        factorId = parseInt(obj.options[obj.selectedIndex].id);
    }
    
    refreshSensorList();
}

function refreshSensorList() {
    var struct = $('#structList').find('option:selected')[0];
    var structId = parseInt(struct.id.split('optionStruct-')[1]);
    sensorListView(structId, factorId);
    $('#btnQuery').click();
}

// 显示监测因素列表
function factorListView(structId) {
    var url = apiurl + '/struct/' + structId + '/factors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        async: false,
        success: function (data) {
            var obj = $('#factorList');
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {

                var factor = data[i].children;

                for (var j = 0; j < factor.length; j++) {

                    //sb.append('<option  value="' + factor[j].factorId + '">' + factor[j].factorName + '</option>');
                    sb.append('<option id="' + factor[j].factorId + '">' + factor[j].factorName + '</option>');
                }

            }
            obj.html(sb.toString());
            //obj.chosen({
            //    //max_selected_options: 1,
            //    no_results_text: "没有找到",
            //    allow_single_de: true
            //});
            //obj.selectpicker('refresh');
        },
        error: function (xmlHttpRequest, textStatus, errorThrown) {
            if (xmlHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            }
            else if (xmlHttpRequest.status == 400) {
                alert("参数错误");
            }
            else if (xmlHttpRequest.status == 500) {
                alert("内部异常");
            }
            else {
                alert('url错误');  
            }
        }
    });
}

//点击查询按钮
$('#btnQuery').click(function () {
    dateValue($('#date').val());//重新获取起始时间
    var startTime = getStartdate();
    var endTime = getEnddate();
    if (startTime >= endTime) {
        alert("结束时间需大于开始时间，请重新设置");
        return;
    }
    var sensorIds = getSensors();
    loadChart(sensorIds, startTime, endTime);
});

//获取多选列表
function getSensors() {
    var len = $("#sensorList option:selected").length;
    var sensorIds = [];
    for (var i = 0; i < len; i++) {
        var a = $("#sensorList option:selected")[i].value;
        sensorIds.push(a);
    }
    return sensorIds;
}

function loadChart(sensorIds, startTime, endTime) {
    // var url = apiurl + '/gprs/sensor/' + sensorIds + '/data/' + startTime + '/' + endTime;
    var factorName = document.getElementById("factorList").value;
    //var factorName = obj.options[obj.selectedIndex].textStr;

    //var url = apiurl + "/sensor/" + sensorIds + "/data/" + getStartdate() + '/' + getEnddate() + '?token=' + getCookie('token');
    var url = apiurl + '/sensor/' + sensorIds + '/originaldata/' + startTime + '/' + endTime + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        async: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                showDataResult(false);
                return;
            } else {
                showDataResult(true);
            }

            var graphChart = createChart("originalGraph", factorName + "原始数据趋势");
            graphChart.yAxis = createYAxis(data[0].columns, data[0].unit);
            graphChart.series = createSeries(data);

            graphChart.tooltip.formatter = function() {
                var tooltipString = '<b>' + this.series.name + '</b>';
                tooltipString = tooltipString + '<br/><br/>采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                tooltipString = tooltipString + '<br/>' + '监测数据:' + this.y.toString() + '<b>' + data[0].unit[this.series.index % data[0].unit.length] + '</b>';

                return tooltipString;
            };

           // display('none');
            $('#datagraph').highcharts(graphChart);
            var tableData = createTableData(data);
            // 生成数据表格(以时间为序)
            tableManager('datatable', tableData.tableData, tableData.tableHeader, tableData.tableHeader.length - 1);

        },
        error: function (xmlHttpRequest, textStatus, errorThrown) {
            if (xmlHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else {
                alertTips('传感器数据获取失败！', 'label-important', 'tip', 5000);
            }
            showDataResult(false);
        }
    });
}

function createChart(renderTo, title) {
    var template = {
        chart: {
            type: 'spline',
            renderTo: renderTo,
            zoomType: 'x'
        },
        title: {
            text: title,
            x: -20 //center
        },

        xAxis: {
            type: 'datetime',
            //tickInterval: 24 * 3600 * 1000,
            dateTimeLabelFormats: {
                //second: '%H:%M:%S',
                //minute: '%H:%M:%S',
                ////hour: '%H:%M:%S',
                //day: '%Y-%m-%d',
                //month: '%b %y',
                //year: '%Y-%m-%d'
            },
            labels: {
                rotation: -25, align: 'right', style: { font: 'normal 13px Verdana, sans-serif' }, formatter: function () {

                    return Highcharts.dateFormat('%Y-%m-%d %H:%M', this.value);

                }
            }
        },
        yAxis: null,
        credits: {
            enabled: false
            //href: 'http://www.f-song.com',
            //text: '江西飞尚科技有限公司'
        },
        plotOptions: {
            spline: {
                lineWidth: 1.5,
                states: {
                    hover: {
                        lineWidth: 2.5
                    }
                },
                marker: {
                    enabled: false
                }
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle',
            borderWidth: 0
        },
        tooltip: {
            shared: false
        },
        series: null
    };

    return template;
}

function createYAxis(columns, units) {
    //var colors = Highcharts.getOptions().colors;
    var axisData = [];
    var labelarry = [];
    var titlearry = [];
    var color = '';
    var axisValue = [];
    var a = '';
    for (var i = 0; i < columns.length; i++) {
        var labelarry = [];
        var titlearry = [];
        var color = '';
        var axisValue = [];
        var a = '';
        var opposite;
        if ((i + 1) % 2 == 0) {
            color = "#89A54E";
            a = "right";
            opposite = true;
        }
        else {
            color = "#4572A7";
            a = "left";
            opposite = false;
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
            "text": columns[i] + '(' + units[i] + ')',
            "style": { "color": color }
        };

        axisValue = {
           // "min": 0,
           // "max": 100,
            "labels": labelarry,
            "title": titlearry,
            "opposite": opposite,
            "showFirstLabel": false
        };
        axisData.push(axisValue);
    }
    
    return axisData;
}

//创建HightCharts图形数据
function createSeries(data) {
    var seriesData = [];
    var seriesValue = [];
   // var columns = data[0].columns;
  //  var unit = data[0].unit;
   // var dataValue = data[0].data;
    //var data_table_values = [];
    for (var itemcnt = 0; itemcnt < data.length; itemcnt++) {
        for (var i = 0; i < data[itemcnt].columns.length; i++) {
            // var location = data[0].location;
            var array = new Array();
            var color = '';
            var yAxis = "";
            if ((i + 1) % 2 == 0) {
                color = "#89A54E";
            }
            else {
                color = "#4572A7";
            }
            yAxis = i;
            //拼 data
            for (var j = 0; j < data[itemcnt].data.length; j++) {
                var time = data[itemcnt].data[j].acquisitiontime.substring(6, 19);
                array[j] = [parseInt(time), data[itemcnt].data[j].value[i]];
            }
            seriesValue = {
                "name": data[itemcnt].location + "-"+ data[itemcnt].columns[i],
                "color": color,
                "type": "spline",
                "yAxis": yAxis,
                "data": array,
                "tooltip": { "valueSuffix": data[itemcnt].unit[i] }
            };
            seriesData.push(seriesValue);
        }
    }
    return seriesData;
}

//创建表格数据
function createTableData(data) {
    var tableData = [];
    var header = [];
    var colmn = "";
   // var location = data[0].location;
    var dataValue = data[0].data;
    header.push("设备位置");
    for (var i = 0; i < data[0].columns.length; i++) {
        header.push(data[0].columns[i] + '(' + data[0].unit[i] + ')');
    }
    header.push("采集时间");

    for (var k = 0; k < data.length; k++) {
        for (i = 0; i < data[k].data.length; i++) {
            var value = [];
            var time = data[k].data[i].acquisitiontime.substring(6, 19);
            value.push(data[k].location);
            for (var j = 0; j < data[k].columns.length; j++) {
                value.push(data[k].data[i].value[j]);
            }
            value.push(time);
            tableData.push(value);
        }
    }
    
    
    return {
        tableHeader: header,
        tableData: tableData
    };
}

//获取监测因素下的测点
function sensorListView(structId, factorId) {

    $('#sensorList').children().remove();
    

    var url = apiurl + '/struct/' + structId + '/factor/' + factorId + '/non-virtual/sensors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        async: false,
        success: function (data) {
            var obj = $('#sensorList');
            if (data == null || data.length === 0) {
                //obj.selectpicker('refresh');
                return;
            }
            var option = '';
            var firstItem = true;
            for (var i = 0; i < data.length; i++) {
                for (var index = 0; index < data[i].sensors.length; index++) {
                    if (firstItem) {
                        option += '<option value="' + data[i].sensors[index].sensorId + '"' + ' selected ="selected">' + data[i].sensors[index].location + '</option>';
                        firstItem = false;
                    } else {
                        option += '<option value="' + data[i].sensors[index].sensorId + '">' + data[i].sensors[index].location + '</option>';
                    }
                    
                }

            }

            obj.html(option);
            //chosen({
            //    no_results_text: "没有找到",
            //    allow_single_de: true
            //});
            //obj.selectpicker('refresh');
        },
        error: function (xmlHttpRequest, textStatus, errorThrown) {
            if (xmlHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else {
                alertTips('传感器列表获取失败！', 'label-important', 'tip', 5000);
            }
        }
    });
    $('#sensorList').selectpicker('refresh');
}

function showDataResult(bshow) {
    if (bshow) {
        $('#datagraph').show();
        $('#box-table').show();
        $('#tipNoData').hide();
    } else {
        $('#datagraph').hide();
        $('#box-table').hide();
        $('#tipNoData').show();
    }
}

// 切换组织事件
$('#listOrg').change(function () {
    showOrgStructs();
});

$('#structList').change(function () {
    refreshFactorList();   
});

$('#factorList').change(function () {
    var obj = document.getElementById("factorList");
    factorId = parseInt(obj.options[obj.selectedIndex].id);
    refreshSensorList();
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