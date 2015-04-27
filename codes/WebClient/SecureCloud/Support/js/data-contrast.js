/**
 * ---------------------------------------------------------------------------------
 * <copyright file="data-contrast.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2015 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述："数据对比"子菜单页面js文件
 *
 * 创建标识：
 *
 * 修改标识：PengLing20150403
 * 修改描述：增加"组织结构物"列表.
 * </summary>
 * ---------------------------------------------------------------------------------
 */
function errorTip(stringId) { //没有数据给出提示
    var graphId = stringId.split(',');
    var errorTipstring = '<div id=\'error\' class=\'row-fluid dataerror-tip\'>' +
        '<div class="span3">' +
        '<span class=\'label label-important\' style=\'margin-left: 5px;margin-top: 10px;\'>抱歉，没有查询到任何有效的数据</span>' +
        '</div>' +
        '</div>';
    for (var i = 0; i < graphId.length; i++) {
        $('#' + graphId[i]).append(errorTipstring);
    }
}

function display(comm1) { //隐藏和展示跳转
    if (comm1 == "block") {
        $('#comm1_error').show();
        $('#comm1').hide();
    } else if (comm1 == "all") {
        $('#comm1_error').hide();
        $('#comm1').hide();
    } else {
        $('#comm1_error').hide();
        $('#comm1').show();
    }
}

$(function () {
    var id = "comm1_error";
    errorTip(id);
    $('#dataServices').addClass('active');
    $('#dataContact2').addClass('active');

    //页面已加载，初始化
    var currentUserId = getCookie("userId");
    if (currentUserId == "") {
        alert("获取用户id失败, 请检查浏览器Cookie是否已启用");
        return;
    }
    getOrgStructsListByUser(currentUserId);

    //点击查询按钮
    $('#btnQuery').click(function () {
        myData = [];
        myTime = [];
        var p = document.getElementById("factorList").value;
        var valueNumber = new Array();
        valueNumber = p.split("/");
        loadChart_Contrast(valueNumber[1]);
    });
    // 切换组织事件
    $('#listOrg').change(function () {
        myData = [];
        myTime = [];
        showOrgStructs();
    });
    $('#structList').change(function () {//根据监测因素变换测点
       myData = [];
       myTime = [];
        getFactor_contract();
        timeTableChange();
    });
    $('#factorList').change(function () {//根据监测因素变换测点
        timeTableChange();
        myData = [];
        myTime = [];
        var p = document.getElementById("factorList").value;
        var valueNumber = new Array();
        valueNumber = p.split("/");
        getSensorList_contrast(valueNumber[0], valueNumber[1]);

    });
});

$('a.box-collapse').click(function () {
    var $target = $(this).parent().parent().next('#dataAllList');
    if ($target.is(':visible')) {
        $('img', this).attr('src', '../resource/img/toggle-expand.png');
    } else {
        $('img', this).attr('src', '../resource/img/toggle-collapse.png');
    }
    $target.slideToggle();
});
//highChart全局变量
var comm_chart;
var comm_chart1;

//先给出框架
function loadChart_Contrast(valuenumber) {
    string = [];
    a = GetDateDiff();
    if (a == 0) {
        alert("请重新选择时间，不同时间段的时间跨度要相同!");
        display('all');
        return;
    } else if (a == 1) {
        getDateAll(valuenumber);
    }
}

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
                alert('该用户没有组织');
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

    getFactor_contract();
}

//获得监测因素因子
function getFactor_contract() {
    $('#factorList').children().remove();
    var struct = $('#structList').find('option:selected')[0];
    var structId = parseInt(struct.id.split('optionStruct-')[1]);
    var url = apiurl + '/struct/' + structId + '/factors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function(data) {
            if (data==null||data.length == 0) {
                $('#factorList').selectpicker('refresh');
                return;
            }
            var monitorFactor = '';
            for (var i = 0; i < data.length; i++) {
                var factor = data[i].children;
                for (var j = 0; j < factor.length; j++) {
                    if (factor[j].factorId == 51 || factor[j].factorId == 54 || factor[j].factorId == 56) {
                        monitorFactor += "";
                    } else {
                        monitorFactor += '<option  value="' + factor[j].factorId + '/' + factor[j].valueNumber + '/' + factor[j].factorName + '">' + factor[j].factorName + '</option>';
                    }
                }
            }
            $('#factorList').html(monitorFactor);
            $('#factorList').selectpicker('refresh');
            var p = document.getElementById("factorList").value;
            var valueNumber = new Array();
            valueNumber = p.split("/");
            getSensorList_contrast(valueNumber[0], valueNumber[1]);
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取结构物列表失败.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

//获取监测因素下的测点
function getSensorList_contrast(factorId, valuenumber) {
    $('#sensorList').children().remove();
    var struct = $('#structList').find('option:selected')[0];
    var structId = parseInt(struct.id.split('optionStruct-')[1]);
    var url = apiurl + '/struct/' + structId + '/factor/' + factorId + '/sensors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        success: function (data) {
            if (data == null || data.length == 0) {
                $('#sensorList').trigger("liszt:updated");
                $('.selectpicker').selectpicker('refresh');
                return;
            }
            var option = '';
            for (var i = 0; i < data.length; i++) {
                for (var index = 0; index < data[i].sensors.length; index++) {
                    if (valuenumber == 1) {
                        option += '<option value="' + data[i].sensors[index].sensorId + '">' + data[i].sensors[index].location + '</option>';
                    } else if (valuenumber == 2 || valuenumber == 4) {
                        if (factorId == 5) {
                            option += '<option value="' + data[i].sensors[index].sensorId + '">' + data[i].sensors[index].location + '-温度</option>';
                            option += '<option value="' + data[i].sensors[index].sensorId + '/1' + '">' + data[i].sensors[index].location + '-湿度</option>';
                        } else if (factorId == 18) {
                            option += '<option value="' + data[i].sensors[index].sensorId + '">' + data[i].sensors[index].location + '-风速</option>';
                            option += '<option value="' + data[i].sensors[index].sensorId + '/1' + '">' + data[i].sensors[index].location + '-风向</option>';
                        } else {
                            option += '<option value="' + data[i].sensors[index].sensorId + '">' + data[i].sensors[index].location + '-x</option>';
                            option += '<option value="' + data[i].sensors[index].sensorId + '/1' + '">' + data[i].sensors[index].location + '-y</option>';
                        }

                    } else {
                        if (factorId == 30) {
                            option += '<option value="' + data[i].sensors[index].sensorId + '">' + data[i].sensors[index].location + '-风速</option>';
                            option += '<option value="' + data[i].sensors[index].sensorId + '/1' + '">' + data[i].sensors[index].location + '-风向</option>';
                            option += '<option value="' + data[i].sensors[index].sensorId + '/2' + '">' + data[i].sensors[index].location + '-风仰角</option>';
                        } else {
                            option += '<option value="' + data[i].sensors[index].sensorId + '">' + data[i].sensors[index].location + '-x</option>';
                            option += '<option value="' + data[i].sensors[index].sensorId + '/1' + '">' + data[i].sensors[index].location + '-y</option>';
                            option += '<option value="' + data[i].sensors[index].sensorId + '/2' + '">' + data[i].sensors[index].location + '-z</option>';
                        }
                        
                    }
                }
            }
            $('#sensorList').html(option);
            $('#sensorList').val(data[0].sensors[0].sensorId);//可以模糊选择,第一默认     
            $('#sensorList').trigger("liszt:updated");
            $('#sensorList').chosen({
                //max_selected_options: 5,
                no_results_text: "没有找到",
                allow_single_de: true
            });
            loadChart_Contrast(valuenumber);
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else if (XMLHttpRequest.status !=0) {
                alert('传感器列表获取失败!');
            }
        }
    });
}

var myData = new Array();
var myTime = new Array();
//循环发送请求
function getDateAll(valuenumber) {
    var start1 = new Array();
    var end1 = new Array();
    start1 = getTime(".ui_timepicker");
    end1 = getTime(".ui_time");
    var k = 0;
    var gS = document.getElementsByClassName('mySelfCss').length;
    while (k < gS) {
        getData(start1[k], end1[k], valuenumber);
        k++;
    }
    if (string.length > 0 && string.length<gS) { //多时间段的提示
        alert(string + "时间段没有数据");
    }
    if (string.length == gS) {
        //确保时间段和数据对应
        display('block');
    }
}


//获取数据并绘图
var string = new Array();
function getUrl(a,start, end) {
    var interval = getInteval();
    var url;
    if (interval <= 6) {
        url = apiurl + "/sensor/" + a + "/data/" + start + '/' + end + '?token=' + getCookie('token');
    } else if (interval <= 30) {
        url = apiurl + "/sensor/" + a + "/data/" + start + '/' + end + '/' + '1/minute?token=' + getCookie('token');
    } else if (interval <= 90) {
        url = apiurl + "/sensor/" + a + "/data/" + start + '/' + end + '/' + '1/hour?token=' + getCookie('token');
    } else if (interval <= 365) {
        url = apiurl + "/sensor/" + a + "/data/" + start + '/' + end + '/' + '12/hour?token=' + getCookie('token');
    } else {
        url = apiurl + "/sensor/" + a + "/data/" + start + '/' + end + '/' + '1/day?token=' + getCookie('token');
    }
    return url;
}

//获取时间

function GetDatetimeByIE(str) {
    var str1 = str.split(' ')[0].split('-');
    var str2 = str.split(' ')[1].split(':');
    var date = new Date();
    date.setUTCFullYear(str1[0], str1[1] - 1, str1[2]);
    date.setUTCHours(str2[0], str2[1], str2[2],0);
    return date; //考虑Ie的兼容性 
}

function getData(start, end, valuenumber) {
    var p = document.getElementById("factorList").value;
    var valueNumber = new Array();
    valueNumber = p.split("/");
    var a;
    var b;
    var url;
    if (parseInt(chose_get_text('#sensorList')) >= 2) {
        if (valueNumber[0] == 5 || valueNumber[0] == 18 || valueNumber[0] == 30) {
            alert("不可以同时选择");
            var a1 = document.getElementById("sensorList");
            jsRemoveSelectedItemFromSelect(a1);
            $('#sensorList').trigger("liszt:updated");
            display("block"); //同时选择时把图形清除
        } else {
            a = chose_get_value("#sensorList"); //按数组循环               
            b = get_direction("#sensorList");
            url = getUrl(a, start, end);
            $.ajax({
                url: url,
                type: 'get',
                async: false,
                success: function(data) {
                    if (data==null||data.length == 0) {
                        display('block');
                        return;
                    } else {
                        display('none');
                        comm_chart = createHighchartComm2('comm1', '同一时间段不同监测点的数据对比', null, 1);
                        comm_chart.series = [];
                        for (var i = 0; i < data.length; i++) {
                            var unit = data[i].unit;
                            var array = new Array();
                            for (var j = 0; j < data[i].data.length; j++) {
                                var time = data[i].data[j].acquisitiontime.substring(6, 19); //时间                           
                                if (b[i] == 0 && data[i].data[j].value[0] != null) { //单方向和x轴方向
                                    array.push([parseInt(time), data[i].data[j].value[0]]); //需要变动                                      
                                } else if (b[i] == 1 && data[i].data[j].value[1] != null) {
                                    array.push([parseInt(time), data[i].data[j].value[1]]);
                                } else if (b[i] == 2 && data[i].data[j].value[2] != null) {
                                    array.push([parseInt(time), data[i].data[j].value[2]]);
                                }
                            }
                            if (b[i] == 0) { //单方向和x轴方向
                                if (valuenumber == 1) {
                                    comm_chart.series.push({ name: data[i].location, data: array });
                                } else {
                                    comm_chart.series.push({ name: data[i].location + '-x', data: array });
                                }
                            } else if (b[i] == 1) {
                                comm_chart.series.push({ name: data[i].location + '-y', data: array });
                            } else if (b[i] == 2) {
                                comm_chart.series.push({ name: data[i].location + '-z', data: array });
                            }
                        }
                        var p = document.getElementById("factorList").value;
                        var valueNumber1 = new Array();
                        valueNumber1 = p.split("/");
                        if (valueNumber1[0] == '6') {
                            comm_chart.chart = {
                                type: 'column',
                                renderTo: 'comm1'
                            };
                        }
                        comm_chart.yAxis.title = { text: valueNumber1[2] + '(' + data[0].unit[0] + ')' }; //需要变动
                        comm_chart.tooltip.formatter = function() {
                            var tooltipString = '<b>' + this.series.name + '</b>';
                            tooltipString = tooltipString + '<br/><br/>采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                            tooltipString = tooltipString + '<br/>' + '监测数据:' + this.y.toString() + '<b>' + unit[0] + '</b>';
                            return tooltipString;
                        };
                        var chart = new Highcharts.Chart(comm_chart);
                    }
                },
                error: function (xhr) {
                    if (xhr.status == 403) {
                        alert("权限验证出错，禁止访问");
                        logOut();
                    } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                        alert("数据获取失败, 请尝试重新查询.\r\n" + xhr.status + " : " + xhr.statusText);
                    }
                }
            });
        }
    } else if (parseInt(chose_get_text('#sensorList')) == 1) {
        display('none');
        a = chose_get_value("#sensorList");
        b = get_direction("#sensorList");
        url = getUrl(a, start, end);
        $.ajax({
            url: url,
            type: 'get',
            async: false,
            success: function(data) {
                if ( data == null||data.length == 0) {
                    string.push(start + '--' + end);
                    myData.push([]);
                    myTime.push([]);
                    return;
                } else {
                    comm_chart1 = createHighchartComm1('comm1', '不同时间段同一监测点的数据对比', null, 1);
                    comm_chart1.series = [];
                    var scaleRange = parseInt(100);
                    var interval = getInteval() * 24 * 3600 * 1000;
                    var startIe = GetDatetimeByIE(start);
                    var timeBegain = new Date(startIe).getTime();//2007-1-11 00:00:00变为14137890
                    for (var i = 0; i < data.length; i++) {
                        var unit ;
                        var array1 = new Array();
                        var timeList = {};
                        for (var j = 0; j < data[i].data.length; j++) {
                            var time = data[i].data[j].acquisitiontime.substring(6, 19); //时间
                            var dtime = ((time - timeBegain) / interval) * scaleRange; //时间进行转换
                            for (var k = 0; k < $('#sensorList' + " option:selected").length; k++) {
                                if (b[k] == 0 && data[i].data[j].value[0] != null) {
                                    array1.push([dtime, data[i].data[j].value[0]]); //需要变动
                                    timeList[dtime] = time;
                                } else if (b[k] == 1 && data[i].data[j].value[1] != null) {
                                    array1.push([dtime, data[i].data[j].value[1]]);
                                    timeList[dtime] = time;
                                } else if (b[k] == 2 && data[i].data[j].value[2] != null) {
                                    array1.push([dtime, data[i].data[j].value[2]]);
                                    timeList[dtime] = time;
                                }
                            }
                        }
                    }
                }
                //获取factorId和factorName
                var p = document.getElementById("factorList").value;
                var valueNumber1 = new Array();
                valueNumber1 = p.split("/");
                if (valueNumber1[0] == '6') {
                    comm_chart1.chart = {
                        type: 'column',
                        renderTo: 'comm1'
                    };
                }
                if (valueNumber1[0] == '5' || valueNumber1[0] == '18'||valueNumber1[0] == '30') {
                    if (b[0] == '1') {
                        comm_chart1.yAxis.title = { text: valueNumber1[2] + '(' + data[0].unit[1] + ')' };
                        unit = data[0].unit[1];
                    } else if (b[0] == '2') {
                        comm_chart1.yAxis.title = { text: valueNumber1[2] + '(' + data[0].unit[2] + ')' };
                        unit = data[0].unit[2];
                    }
                    else {
                        comm_chart1.yAxis.title = { text: valueNumber1[2] + '(' + data[0].unit[0] + ')' };
                        unit = data[0].unit[0];
                    }
                } else {
                    comm_chart1.yAxis.title = { text: valueNumber1[2] + '(' + data[0].unit[0] + ')' };
                    unit = data[0].unit[0];
                }
                myData.push(array1);
                myTime.push(timeList);
                var start1 = new Array();
                var end1 = new Array();
                start1 = getTime(".ui_timepicker");
                end1 = getTime(".ui_time");
                
                for (var num = 0; num < myData.length; num++) {

                        comm_chart1.series.push({ name: start1[num] + '--' + end1[num], data: myData[num], id: num.toString() });
                    }
                    comm_chart1.tooltip.formatter = function (e) {
                        var tooltipString = '<b>' + this.series.name + '</b>';
                        tooltipString = tooltipString + '<br/></br>采集时间：' + toDate(myTime[this.series.index][this.x]);
                        tooltipString = tooltipString + '<br/><br/>监测数据:' + this.y.toString() + '<b>' + unit + '</b>';
                        return tooltipString;
                    }
                    var chart = new Highcharts.Chart(comm_chart1);
            },

            error: function (xhr) {
                if (xhr.status == 403) {
                    alert("权限验证出错，禁止访问");
                    logOut();
                } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                    alert("数据获取失败, 请尝试重新查询.\r\n" + xhr.status + " : " + xhr.statusText);
                }
            }
        });
    }
}

//时间转换
function toDate(json) {
    var dateTime = new Date(parseInt(json));
    var year = dateTime.getFullYear();
    var month = dateTime.getMonth() + 1;
    var date = dateTime.getDate();
    var hour = dateTime.getHours();
    var minutes = dateTime.getMinutes();
    var second = dateTime.getSeconds();

    return year + "-" + month + "-" + date + " " + hour + ":" + minutes + ":" + second;
}

//根据监测点的个数，选择时间段
$('#sensorList').change(function() {
    var sb;
    if (parseInt(chose_get_text('#sensorList')) >= 2) {
        sb = '<div class="control-group"><table class="mySelfCss"><tr><td><b>对比时段:'
            + '</b></td><td><div class="controls" style="margin-left:0;"><div class="input-append date"><input type="text" id="dpform1" class="ui_timepicker" />' +
            '<span class="add-on" style="height:20px;"><i data-time-icon="icon-time" data-date-icon="icon-calendar"></i></span></div></td>' +
            '<td><b>至</b></td>' +
            '<td><div class="input-append date"><input type="text" id="dpdend1" class="ui_time"/><span class="add-on" style="height:20px;"><i data-time-icon="icon-time" data-date-icon="icon-calendar"></i></span></td></tr></table></div>';

        $('#timeTable').html(sb);
        $('.input-append ').datetimepicker({
            format: 'yyyy-MM-dd hh:mm:ss',
            language: 'pt-BR'
        });
        $("#dpform1").val(showdate(-1));
        $("#dpdend1").val(showdate(0));
    } else if (parseInt(chose_get_text('#sensorList')) == 1) {
        timeTableChange();
    }
});

function timeTableChange() {
    var sb;
    sb = '<div class="control-group"><table class="mySelfCss"><tr><td><b>对比时段1:' 
       + '</b></td><td><div class="controls" style="margin-left:0;"><div class="input-append date"><input type="text" id="dpform1" class="ui_timepicker"/>' +
        '<span class="add-on" style="height:20px;"><i data-time-icon="icon-time" data-date-icon="icon-calendar"></i></span></div></td>' +
        '<td><b>至</b></td>' +
        '<td><div class="input-append date"><input type="text" id="dpdend1" class="ui_time"/><span class="add-on" style="height:20px;"><i data-time-icon="icon-time" data-date-icon="icon-calendar"></i></span></td></tr></table></div>';

    sb += '<div class="control-group"><table class="mySelfCss"><tr><td><b>对比时段2:' 
       + '</b></td><td><div class="controls" style="margin-left:0;"><div class="input-append date"><input type="text" id="dpform2" class="ui_timepicker"/>' +
        '<span class="add-on" style="height:20px;"><i data-time-icon="icon-time" data-date-icon="icon-calendar"></i></span></div></td>' +
        '<td><b>至</b></td>' +
        '<td><div class="input-append date"><input type="text" id="dpdend2" class="ui_time"/><span class="add-on" style="height:20px;"><i data-time-icon="icon-time" data-date-icon="icon-calendar"></i></span></div>';
    sb += '<td><img id="expand_collapse" alt="" src="/resource/img/toggle-expand.png" style="width: 40px; height: 40px;" onclick="addTimeTable()" /></td></tr></table></div>';

    $('#timeTable').html(sb);
    $('.input-append ').datetimepicker({
        format: 'yyyy-MM-dd hh:mm:ss',
        language: 'pt-BR'
    });
    $("#dpform1").val(showdate(-1));
    $("#dpdend1").val(showdate(0));
    $("#dpform2").val(showdate(-2));
    $("#dpdend2").val(showdate(-1));
}


//select text获取，多选时请注意
function chose_get_text(select) {
    return $(select + " option:selected").length;
}
//获取多选列表
function chose_get_value(select) {
    var b = $(select + " option:selected").length;
    var cId = new Array(); //获得设备Id    
    var p = new Array(); //设备的Id及数据方向
    var i = 0;
    while (i < b) {
        var a = $(select + " option:selected")[i].value;
        p = a.split("/");
        cId.push(p[0]);
        i++;
    }
    return cId;
}

//清除所选项
function jsRemoveSelectedItemFromSelect(objSelect) {
    // var b = $(select + " option:selected").length;
    var length = objSelect.options.length - 1;
    for (var i = length; i >= 0; i--) {
        if (objSelect[i].selected == true) {
            objSelect.options[i].selected = false;
        }
    }
}

function get_direction(select) {
    var b = $(select + " option:selected").length;
    var dz = new Array();//数据的方向
    var p = new Array();//设备的Id及数据方向
    var idFz = new Array();
    var i = 0;
    while (i < b) {
        var a = $(select + " option:selected")[i].value;
        p = a.split("/");
        if (!p[1]) {
            p[1] = 0;
        }
        dz.push(p[1]);
        i++;
    }
    return dz;
}

//获取时间
function getTime(d1) {
    var b = document.getElementsByClassName('mySelfCss');
    var a = new Array();
    var i = 0;
    while (i < b.length) {
        var c = $(d1)[i].value;
        a.push(c);
        i++;
    }
    return a;
}

//比较各个时间段的步长是否一致
function GetDateDiff() {
    //步长以小时计算   
    var divNum = 1000 * 3600;
    var durationArray = new Array();
    var b = document.getElementsByClassName('mySelfCss');
    var startTimeArray = $('.ui_timepicker');
    var endTimeArray = $('.ui_time');
    for (var i = 0; i < b.length; i++) {
        var startTime = startTimeArray[i].value;
        var endTime = endTimeArray[i].value;
        startTime = startTime.replace(/-/g, "/");
        endTime = endTime.replace(/-/g, "/");
        var sTime = new Date(startTime);
        var eTime = new Date(endTime);
        var a = parseInt((eTime.getTime() - sTime.getTime()) / parseInt(divNum));
        durationArray.push(a);
    }
    for (var i = 1; i < durationArray.length; i++) {
        if (durationArray[0] != durationArray[i]) {
            return 0;
        } else {
            continue;
        }
    }
    return 1;
}


//获得步长
function getInteval() {
    var divNum = 1000 * 3600 * 24;
    var startTimeArray = $('.ui_timepicker');
    var endTimeArray = $('.ui_time');
    var startTime = startTimeArray[0].value;
    var endTime = endTimeArray[0].value;
    startTime = startTime.replace(/-/g, "/");
    endTime = endTime.replace(/-/g, "/");
    var sTime = new Date(startTime);
    var eTime = new Date(endTime);
    var a = parseInt((eTime.getTime() - sTime.getTime()) / parseInt(divNum));
    return a;
}


//获取今天，昨天，前天的时间
function showdate(n, string) {
    var uom = new Date();
    uom.setDate(uom.getDate() + n);
    if (string == "other")
        uom = uom.getFullYear() + "-" + (uom.getMonth() + 1) + "-" + uom.getDate();
    else {
        uom = uom.getFullYear() + "-" + (uom.getMonth() + 1) + "-" + uom.getDate() + " " + uom.getHours() + ":" + uom.getMinutes() + ":" + uom.getSeconds();
    }
    return uom.replace(/\b(\w)\b/g, '0$1'); //时间的格式
}

$("#dform1").val(showdate(-1));
$("#ddend1").val(showdate(0));
$("#dform2").val(showdate(-2));
$("#ddend2").val(showdate(-1));




