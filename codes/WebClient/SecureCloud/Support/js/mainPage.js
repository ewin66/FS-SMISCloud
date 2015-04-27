/**
 * ---------------------------------------------------------------------------------
 * <copyright file="mainPage.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2015 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述：技术支持项目状态仪表盘js文件
 *
 * 创建标识：PengLing20150309
 *
 * 修改标识：
 * 修改描述：
 * </summary>
 * ---------------------------------------------------------------------------------
 */
var g_currentUserId = null;
var g_currentStructId = null;
var g_currentProject = {};
var g_toggledDtus = {}; // 保存DTU状态分类下被点击过的DTU及其"展开/折叠"状态
var g_isCurrentActiveDtuRefreshed = false;
var g_currentActiveDtuId = null; // 保存当前活动的(内容被展开的)DTU
var g_currentActiveDtuSetIntervalId = null;
var g_currentDtuSensor = {};
var g_bufferUserProjectsStatus = {};

$(function () {
    $("#MainPage").addClass('active');

    g_currentUserId = getCookie("userId");
    if (g_currentUserId == "") {
        alert("获取用户id失败, 请检查浏览器Cookie是否已启用");
        return;
    }

    setHighchartsGlobalOptions();

    getProjectsStatusByUser();

    bindClickEvent();
});

function bindClickEvent() {
    // 指定项目下结构物列表中"结构物"名称点击事件
    $('#table-projectStructsStatus').on('click', 'a.aStructName', function () {
        collapseAlarmAndDtuAndSensorStatusByStruct(); // 折叠结构物下"告警/DTU/传感器"状态
        
        var tr = $(this).parents('tr');
        var selectedRow = tr[0];
        g_currentStructId = parseInt(selectedRow.id.split('struct-')[1]); // assign value
        getAlarmAndDtuAndSensorStatusByStruct(); // 获取"结构物下告警/DTU/传感器状态"模块
    });

    // 分类项"存在异常的结构物"点击事件
    $('#category-abnormalStruct').click(function () {
        $('#category-abnormalStruct').removeClass('like-a');
        $('#category-allStruct').addClass('like-a');
        $('#category-normalStruct').addClass('like-a');
        $('#part-structStatus').removeClass('display-block').addClass('display-none');
        getStructsStatusByProject('abnormal');
    });
    // 分类项"全部"结构物点击事件
    $('#category-allStruct').click(function () {
        $('#category-allStruct').removeClass('like-a');
        $('#category-abnormalStruct').addClass('like-a');
        $('#category-normalStruct').addClass('like-a');
        $('#part-structStatus').removeClass('display-block').addClass('display-none');
        getStructsStatusByProject('all');
    });
    // 分类项"正常的结构物"点击事件
    $('#category-normalStruct').click(function () {
        $('#category-normalStruct').removeClass('like-a');
        $('#category-abnormalStruct').addClass('like-a');
        $('#category-allStruct').addClass('like-a');
        $('#part-structStatus').removeClass('display-block').addClass('display-none');
        getStructsStatusByProject('normal');
    });

    // 点击"最近24小时未确认告警/DTU状态/传感器状态", 切换"展开/收缩"图标
    $('a.accordion-toggle-struct').click(function () {
        updateAlarmOrDtuOrSensorStatusByStruct(this); // 每次点击操作均更新已展示内容
        
        var $list = $('a.accordion-toggle-struct');
        var that = this;
        $.each($list, function (index, item) {
            if (item != that) {
                $(item).addClass('collapsed'); // resume the other 'a.accordion-toggle' to initial class
                $('i.accordion-icon-struct', item).removeClass('fa-angle-down').addClass('fa-angle-up');
            }
        });
        $(this).toggleClass('collapsed'); // initial class is "collapsed"
        if ($(this).hasClass('collapsed')) {
            $('i.accordion-icon-struct', this).removeClass('fa-angle-down').addClass('fa-angle-up');
        } else {
            $('i.accordion-icon-struct', this).removeClass('fa-angle-up').addClass('fa-angle-down');
        }
    });
    
    // 分类项"离线DTU"点击事件
    $('#category-offlineDtu').click(function() {
        $('#category-offlineDtu').removeClass('like-a');
        $('#category-neverUplineDtu').addClass('like-a');
        $('#category-allDtu').addClass('like-a');
        $('#category-onlineDtu').addClass('like-a');
        g_currentActiveDtuId = null; // init it
        g_toggledDtus = {}; // init it
        getDtusStatusByStruct('offline');
    });
    // 分类项"从未上线DTU"点击事件
    $('#category-neverUplineDtu').click(function () {
        $('#category-offlineDtu').addClass('like-a');
        $('#category-neverUplineDtu').removeClass('like-a');
        $('#category-allDtu').addClass('like-a');
        $('#category-onlineDtu').addClass('like-a');
        g_currentActiveDtuId = null; // init it
        g_toggledDtus = {}; // init it
        getDtusStatusByStruct('neverUpline');
    });
    // 分类项"全部DTU"点击事件
    $('#category-allDtu').click(function () {
        $('#category-offlineDtu').addClass('like-a');
        $('#category-neverUplineDtu').addClass('like-a');
        $('#category-allDtu').removeClass('like-a');
        $('#category-onlineDtu').addClass('like-a');
        g_currentActiveDtuId = null; // init it
        g_toggledDtus = {}; // init it
        getDtusStatusByStruct('all');
    });
    // 分类项"在线DTU"点击事件
    $('#category-onlineDtu').click(function () {
        $('#category-offlineDtu').addClass('like-a');
        $('#category-neverUplineDtu').addClass('like-a');
        $('#category-allDtu').addClass('like-a');
        $('#category-onlineDtu').removeClass('like-a');
        g_currentActiveDtuId = null; // init it
        getDtusStatusByStruct('online');
    });
}

function updateAlarmOrDtuOrSensorStatusByStruct(dom) {
    if (dom.id == "aStructAlarm") {
        getUnprocessedAlarmStatusByStruct(); // 获取结构物下最近24小时各等级未确认告警数目
    }
    if (dom.id == "aStructDtu") {
        $('#category-offlineDtu').click(); // 获取结构物下不在线DTU状态 -> getDtusStatusByStruct('offline').
    }
    if (dom.id == "aStructSensor") {
        $('#load1').removeClass('fa-angle-up').addClass('fa-angle-down');
        $('#allInfor').show();
        $('#loadChart').removeClass('span6').addClass('span12');
        GetSensorInformationByStruct(); //获取传感器的最新状态
        DisplayIndex(4);
    }
}

/**
 * 折叠"结构物下告警/DTU/传感器"状态
 */
function collapseAlarmAndDtuAndSensorStatusByStruct() {
    var $list = $('a.accordion-toggle-struct');
    $.each($list, function (index, item) {
        $(item).addClass('collapsed'); // resume the other 'a.accordion-toggle' to initial class
        $('i.accordion-icon-struct', item).removeClass('fa-angle-down').addClass('fa-angle-up');
        $(item).parent().next().removeClass('in').css('height', '0'); // collapse the expanded accordion-group
    });
}

/**
 * highcharts本地化时间，汉化下载条目
 */
function setHighchartsGlobalOptions() {
    Highcharts.setOptions({
        global: {
            useUTC: false // 关闭UTC       
        },
        lang: {
            downloadJPEG: '下载JPEG图片',
            downloadPDF: '下载PDF图片',
            downloadPNG: '下载PNG图片',
            downloadSVG: '下载SVG图片',
            printChart: '打印图片'
        }
    });
}

function getProjectsStatusByUser() {
    $('#loading-projectDashboard').show();
    
    var startTime = getDateAndTime(-1);
    var endTime = getDateAndTime(0);
    var url = apiurl + '/statistics/user/' + g_currentUserId + '/projects/status/' + startTime + '/' + endTime + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            if (data.length == 0) {
                $('#loading-projectDashboard').hide();
                $('#projectDashboard').html('<span class="label-red">该用户下无任何项目</span>');
                return;
            }
            onsuccessProjectsStatusByUser(data);
        },
        error: function (xhr) {
            $('#loading-projectDashboard').hide();
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取用户下项目状态统计信息时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function onsuccessProjectsStatusByUser(data) {
    var abnormalProjectCount = 0;
    var projects = {};
    $.each(data, function (i, item) {
        if (item.projectStatus == false) {
            abnormalProjectCount++;
        }
        projects[item.projectId] = { // assign value
            projectId: item.projectId,
            projectAbbreviation: item.projectAbbreviation,
            abnormalStructCount: item.abnormalStructCount,
            normalStructCount: item.normalStructCount
        };
        if (g_bufferUserProjectsStatus[item.projectId] == null) {
            g_bufferUserProjectsStatus[item.projectId] = projects[item.projectId];
            g_bufferUserProjectsStatus[item.projectId].isChanged = false;
        } else {
            if (g_bufferUserProjectsStatus[item.projectId].abnormalStructCount != projects[item.projectId].abnormalStructCount ||
                g_bufferUserProjectsStatus[item.projectId].normalStructCount != projects[item.projectId].normalStructCount) {
                // update the corresponding g_bufferUserProjectsStatus
                g_bufferUserProjectsStatus[item.projectId].abnormalStructCount = projects[item.projectId].abnormalStructCount;
                g_bufferUserProjectsStatus[item.projectId].normalStructCount = projects[item.projectId].normalStructCount;
                g_bufferUserProjectsStatus[item.projectId].isChanged = true;
            } else {
                g_bufferUserProjectsStatus[item.projectId].isChanged = false;
            }
        }
    });

    var title = '&nbsp;(&nbsp;<span style="color: red;">' + abnormalProjectCount + '个项目存在异常</span>&nbsp;/&nbsp;共' + data.length + '个项目&nbsp;)';
    $('#title-ProjectDashboard').html(title);

    showProjectsStatusByUser(projects);
}

function showProjectsStatusByUser(projects) {
    var chart = $('#chart-userProjectsStatus').highcharts();
    if (chart == null) {
        initPointsOnChartOfProjectsStatusByUser(projects);
    } else {
        updatePointsOnChartOfProjectsStatusByUser(chart);
    }
}

/**
 * 初始化用户项目状态highcharts图表
 */
function initPointsOnChartOfProjectsStatusByUser(projects) {
    stackedChartOptions.series = []; // empty it

    stackedChartOptions.title.text = '结构物状态统计';
    stackedChartOptions.yAxis.title.text = '正常/异常结构物个数';

    // Populate data for categories and series
    var arrProjectAbbreviation = [];
    var arrAbnormalStructCount = [];
    var arrNormalStructCount = [];
    var maxLength = 0;
    for (var key in projects) {
        var project = projects[key];

        var totleLength = project.abnormalStructCount + project.normalStructCount;
        if (totleLength > maxLength) {
            maxLength = totleLength; // get the max value of y-axis in highcharts
        }

        arrProjectAbbreviation.push(project.projectAbbreviation);
        project.abnormalStructCount == 0 ?
            arrAbnormalStructCount.push({ id: 'abnormal-' + project.projectId, y: null }) :
            arrAbnormalStructCount.push({ id: 'abnormal-' + project.projectId, y: project.abnormalStructCount });
        project.normalStructCount == 0 ?
            arrNormalStructCount.push({ id: 'normal-' + project.projectId, y: null }) :
            arrNormalStructCount.push({ id: 'normal-' + project.projectId, y: project.normalStructCount });
    }

    stackedChartOptions.yAxis.max = maxLength + 1; // to fix the position of zero label on y-axis
    stackedChartOptions.xAxis.categories = arrProjectAbbreviation;

    //var screenHeight = $(window).height() - 200;
    //$('#chart-userProjectsStatus').parent().attr('data-height', screenHeight).css('height', screenHeight);
    //$('#chart-userProjectsStatus').parent().parent().css('height', screenHeight);

    // highcharts自适应高度
    var len = arrProjectAbbreviation.length;
    if (len > 75) {
        stackedChartOptions.chart.height = 2000;
    } else if (len > 50 && len <= 75) {
        stackedChartOptions.chart.height = 1500;
    } else if (len > 25 && len <= 50) {
        stackedChartOptions.chart.height = 1000;
    } else {
        stackedChartOptions.chart.height = 500;
    }
    // Populate series
    stackedChartOptions.series.push({
        name: '异常结构物个数',
        color: '#FF6A6A',
        data: arrAbnormalStructCount
    });
    stackedChartOptions.series.push({
        name: '正常结构物个数',
        color: '#3CB371',
        data: arrNormalStructCount
    });
    // Generate tooltip
    stackedChartOptions.tooltip.formatter = function () {
        var chartTooltip = '<b>' + this.x + '</b><br/>' +
            this.series.name + ': ' + this.y + '个' + '<br/>' +
            '结构物总数: ' + this.point.stackTotal + '个';
        return chartTooltip;
    };
    // Fires when the chart is finished loading.
    stackedChartOptions.chart.events.load = function () {
        $('#loading-projectDashboard').hide();
    };
    // 每个项目的点击事件
    stackedChartOptions.plotOptions.series.point.events.click = function () {
        $('#loading-projectDashboard').show();

        var projectAbbreviation = this.category;
        $('#title-projectStructsStatus').text(projectAbbreviation);

        for (var key in projects) {
            if (projects[key].projectAbbreviation == projectAbbreviation) {
                g_currentProject = projects[key]; // assign value.
                break;
            }
        }
        if (!isEmptyObject(g_currentProject)) {
            $('#category-abnormalStruct').click(); // getStructsStatusByProject('abnormal')
        }
    };

    $('#chart-userProjectsStatus').highcharts(stackedChartOptions);
    // Set all labels of xAxis for right alignment
    $('.highcharts-container', '#chart-userProjectsStatus').css('text-align', 'right');
}

/**
 * 更新用户项目状态highcharts图表中的变化点
 */
function updatePointsOnChartOfProjectsStatusByUser(chart) {
    for (var key in g_bufferUserProjectsStatus) {
        var projectStatus = g_bufferUserProjectsStatus[key];
        if (projectStatus.isChanged) {
            var abnormalStructCount, normalStructCount;
            abnormalStructCount = projectStatus.abnormalStructCount == 0 ? null : projectStatus.abnormalStructCount;
            normalStructCount = projectStatus.normalStructCount == 0 ? null : projectStatus.normalStructCount;
            chart.get('abnormal-' + projectStatus.projectId).update(abnormalStructCount);
            chart.get('normal-' + projectStatus.projectId).update(normalStructCount);
        }
    }
}

function getStructsStatusByProject(status) {
    $('#loading-projectDashboard').show();
    getProjectsStatusByUser(); // 更新"项目仪表盘"模块 
    
    var startTime = getDateAndTime(-1);
    var endTime = getDateAndTime(0);
    var url = apiurl + '/statistics/user/' + g_currentUserId + '/project/' + g_currentProject.projectId +
        '/structs/status/' + status + '/' + startTime + '/' + endTime + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            showStructsStatusByProject(data);
            $('#loading-projectDashboard').hide();
        },
        error: function (xhr) {
            $('#loading-projectDashboard').hide();
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取项目下结构物状态统计信息时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function showStructsStatusByProject(data) {
    $('#container-userProjectsStatus').removeClass('span12').addClass('span6');
    $('#chart-userProjectsStatus').highcharts().reflow();

    // Create the table for structures' status of the specified project.
    $('#container-projectStructsStatus').attr('style', 'display: block;');
    $('#category-abnormalStruct').html('存在异常的结构物&nbsp;(&nbsp;' + 0 + '&nbsp;)');
    $('#category-allStruct').html('全部&nbsp;(&nbsp;' + 0 + '&nbsp;)');
    $('#category-normalStruct').html('正常的结构物&nbsp;(&nbsp;' + 0 + '&nbsp;)');
    var tableBody = '';
    if (data != null) {
        // Create the categories for structures' status of the specified project.
        $('#category-abnormalStruct').html('存在异常的结构物&nbsp;(&nbsp;' + data.abnormalStructCount + '&nbsp;)');
        var allStructCount = data.abnormalStructCount + data.normalStructCount;
        $('#category-allStruct').html('全部&nbsp;(&nbsp;' + allStructCount + '&nbsp;)');
        $('#category-normalStruct').html('正常的结构物&nbsp;(&nbsp;' + data.normalStructCount + '&nbsp;)');
        
        $.each(data.structs, function (i, struct) {
            tableBody += '<tr id="struct-' + struct.structId + '">'
                + '<td><a class="aStructName" href="javascript:;">' + struct.structName + '</a></td>';
            tableBody += struct.structStatus == false ? '<td style="color: red;">异常</td>' : '<td style="color: green;">正常</td>';
            tableBody += '<td>' + struct.unprocessedAlarmCount + '</td>'
                + '<td>' + struct.notOnlineDtuCount + '</td>';
            tableBody += struct.abnormalSensorCount == null ? '<td>N/A</td>' : '<td>' + struct.abnormalSensorCount + '</td>';
            tableBody += '</tr>';
        });
    }

    $('#table-projectStructsStatus').dataTable().fnDestroy(); // 该语句影响表格自适应显示
    $('#tbody-projectStructsStatus').html(tableBody);
    extendDatatable();
}

function getAlarmAndDtuAndSensorStatusByStruct() {
    $('#loading-projectDashboard').show();
    $('#part-structStatus').removeClass('display-none').addClass('display-block');

    var startTime = getDateAndTime(-1);
    var endTime = getDateAndTime(0);
    var url = apiurl + '/statistics/struct/' + g_currentStructId + '/status/' + startTime + '/' + endTime + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            if (data == null) {
                $('#loading-projectDashboard').hide();
                $('#part-structStatus').html('<span class="label-red">该结构物不存在或已被删除</span>');
                return;
            }
            retrieveStructsStatusByProject(data);
            showAlarmAndDtuAndSensorStatusByStruct(data);
            $('#loading-projectDashboard').hide();
        },
        error: function (xhr) {
            $('#loading-projectDashboard').hide();
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取结构物下告警、DTU、传感器状态统计信息时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

function retrieveStructsStatusByProject(data) {
    var status;
    var target = $('.category-struct').not('.like-a')[0];
    if (target.id == 'category-allStruct') {
        status = 'all';
    } else {
        status = data.structStatus == false ? 'abnormal' : 'normal';
        rerenderStructCategory(status);
    }
    getStructsStatusByProject(status); // 重新获取"项目下结构物状态"
}

function rerenderStructCategory(status) {
    $('#category-' + status + 'Struct').removeClass('like-a');
    var $others = $('.category-struct').not('#category-' + status + 'Struct');
    $.each($others, function (index, item) {
        $(item).addClass('like-a');
    });
}

var sensorTotles = '';
function showAlarmAndDtuAndSensorStatusByStruct(data) {
    $('#title-structStatus-projectName').html($('#title-projectStructsStatus').text());
    $('#title-structStatus-structName').html(data.structName);

    var prefix = '&nbsp;(&nbsp;<span style="color: red;">';
    var suffix = '&nbsp;)';
    $('#title-accordion-alarm').html(prefix + data.unprocessedAlarmCount + '个未确认告警</span>' + suffix);
    var notOnlineDtuCount = data.offlineDtuCount + data.neverUplineCount;
    $('#title-accordion-dtu').html(prefix + notOnlineDtuCount + '个不在线DTU</span>&nbsp;/&nbsp;共' + data.dtuTotles + '个DTU' + suffix);
    var abnormalSensorCount = data.abnormalSensorCount == null ? 'N/A' : data.abnormalSensorCount;
    $('#title-accordion-sensor').html(prefix + '在线DTU下存在' + abnormalSensorCount + '个异常传感器</span>&nbsp;/&nbsp;共' + data.sensorTotles + '个传感器' + suffix);

    // 生成"DTU状态"分类: 离线DTU | 从未上线DTU | 全部 | 在线DTU
    $('#category-offlineDtu').html('离线DTU&nbsp;(&nbsp;' + data.offlineDtuCount + '&nbsp;)');
    $('#category-neverUplineDtu').html('从未上线DTU&nbsp;(&nbsp;' + data.neverUplineCount + '&nbsp;)');
    $('#category-allDtu').html('全部&nbsp;(&nbsp;' + data.dtuTotles + '&nbsp;)');
    var onlineDtuCount = data.dtuTotles - data.offlineDtuCount - data.neverUplineCount;
    $('#category-onlineDtu').html('在线DTU&nbsp;(&nbsp;' + onlineDtuCount + '&nbsp;)');
    sensorTotles = data.sensorTotles;
}

/**
 * 获取指定结构物下最近24小时未确认告警
 */
function getUnprocessedAlarmStatusByStruct() {
    $('#loading-projectDashboard').show();
    getAlarmAndDtuAndSensorStatusByStruct(); // 更新"结构物下告警/DTU/传感器状态"模块
    
    var startTime = getDateAndTime(-1);
    var endTime = getDateAndTime(0);
    var url = apiurl + '/statistics/struct/' + g_currentStructId + '/alarm-count/unprocessed/' + startTime + '/' + endTime + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            if (data == null || data.alarms.length == 0) {
                $('#loading-projectDashboard').hide();
                $('#chart-alarmStatus').html('<span class="label-red">该结构物最近24小时无未确认告警</span>').attr('style', 'height: 40px;');
                return;
            }
            showUnprocessedAlarmStatusByStruct(data);
        },
        error: function (xhr) {
            $('#loading-projectDashboard').hide();
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取结构物下各等级未确认告警数统计信息时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

/**
 * 显示指定结构物下最近24小时未确认告警图表
 */
function showUnprocessedAlarmStatusByStruct(data) {
    $('#chart-alarmStatus').attr('style', 'height: 300px;');
    
    var arrAlarms = [];
    $.each(data.alarms, function (i, item) {
        var alarmName = '';
        var sliceColor = '';
        switch (item.alarmLevel) {
            case 1:
                alarmName = '一级告警';
                sliceColor = '#FF6A6A';
                break;
            case 2:
                alarmName = '二级告警';
                sliceColor = '#EE9A49';
                break;
            case 3:
                alarmName = '三级告警';
                sliceColor = '#8968CD';
                break;
            case 4:
                alarmName = '四级告警';
                sliceColor = '#6495ED';
                break;
        }
        arrAlarms.push({ name: alarmName, y: item.alarmCount, color: sliceColor });
    });
    pieChartOptions.series[0].data = arrAlarms;

    // Fires when the chart is finished loading.
    pieChartOptions.chart.events.load = function () {
        $('#loading-projectDashboard').hide();
    };
    // Fires when a point is clicked. 
    pieChartOptions.plotOptions.pie.point.events.click = function () {
        // link to alarm page.
        top.location.href = '/Support/DataWaringSupport.aspx?orgId=' + g_currentProject.projectId + '&structId=' + g_currentStructId;
    };
    
    $('#chart-alarmStatus').highcharts(pieChartOptions);
}

/** 
 * 获取指定结构物下DTU最新状态
 */
function getDtusStatusByStruct(status) {
    $('#loading-projectDashboard').show();
    getAlarmAndDtuAndSensorStatusByStruct(); // 更新"结构物下告警/DTU/传感器状态"模块
    
    var url = apiurl + '/struct/' + g_currentStructId + '/dtus/' + status + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            if (data == null || data.dtus.length == 0) {
                var tip;
                switch (status) {
                    case "offline":
                        tip = '不存在离线的DTU';
                        break;
                    case "neverUpline":
                        tip = '不存在从未上线的DTU';
                        break;
                    case "online":
                        tip = '不存在在线的DTU';
                        break;
                    default:
                        tip = '不存在任何DTU';
                        break;
                }
                $('#loading-projectDashboard').hide();
                $('#accordion-listDtuStatus').html('<span class="label-red">' + tip +'</span>');
                return;
            }
            showDtusStatusByStruct(data, status);
            $('#loading-projectDashboard').hide();
        },
        error: function (xhr) {
            $('#loading-projectDashboard').hide();
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取结构物下DTU状态时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

function showDtusStatusByStruct(data, status) {
    var domListDtuStatus = '';
    $.each(data.dtus, function (index, item) {
        var domI;
        if (item.status == false) {
            domI = '<i class="fa fa-circle" style="color: gray; font-size: 16px;"></i>';
        } else if (item.status) {
            domI = '<i class="fa fa-circle" style="color: green; font-size: 16px;"></i>';
        } else {
            domI = 'N/A';
        }
        domListDtuStatus +=
            '<div class="accordion-group">' +
            '<div class="accordion-heading">' + 
            '<a id="accordion-toggle-dtu-' + item.dtuId + '" onclick="onclickDtuItem(' + item.dtuId +
                ')" class="accordion-toggle collapsed accordion-toggle-dtu" data-toggle="collapse" data-parent="#accordion-listDtuStatus" href="#collapse-dtu-' + item.dtuId + '">' +
            '<i class="fa fa-thumb-tack marginRight10"></i>' +
            '<span>' + item.dtuNo + '<span class="marginLeft60">当前状态 :&nbsp;&nbsp;' + domI + '</span>' + '</span>' +
            '<i class="fa fa-angle-up position-absolute-right accordion-icon-dtu"></i></a></div>' +
            '<div id="collapse-dtu-' + item.dtuId + '" class="collapse display-none" style="min-height: 100px; height: auto;">' +
            '<div class="tabbable tabbable-custom">' +
            '<ul class="nav nav-tabs">' +
            '<li id="tabDtu-' + item.dtuId + '" class="active"><a href="#tab-dtu-' + item.dtuId + '" data-toggle="tab" class="fontSize13">DTU</a></li>' +
            '<li id="tabSensor-' + item.dtuId + '"><a href="#tab-sensor-' + item.dtuId + '" data-toggle="tab" class="fontSize13">DTU下传感器</a></li>' +
            '</ul>' +
            '<div class="tab-content">' +
            // tab-dtu
            '<div id="tab-dtu-' + item.dtuId + '" class="tab-pane row-fluid active">' +
            '<div class="marginBottom10"><div class="marginBottom5"><i class="fa fa-info-circle fa-gray marginRight10"></i>最新状态</div>' +
                '<div id="prompt-dtu-dtuType" style="display: none;"><span class="label-red">该DTU为传输型DTU, 因此不能对该DTU进行"远程修改"或"重启"</span></div>' +
                // DTU"重启"提示(剩余等待时间)
                '<div style="text-align: left; font-size: 16px; margin-bottom: 10px;">' +
                '<span id="promptDtuRestart" style="font-weight: bold;"></span>' +
                '<span id="timeContainerDtuRestart" style="display: none; margin-left: 10%;">剩余等待时间: <span id="timeDtuRestart" style="color: #ff0000"></span>秒</span>' +
                '</div>' +
                // DTU最新状态表格
                '<div id="container-dtuLatestStatus-' + item.dtuId + '"></div></div>' +
            '<div class="marginBottom10"><div class="marginBottom5"><i class="fa fa-clock-o fa-gray marginRight10"></i>最近24小时状态</div>' +
                '<div id="container-dtuHistoryStatus-' + item.dtuId + '"><div id="chart-dtuStatus-' + item.dtuId + '"></div></div></div></div>' +
            // tab-sensor
            '<div id="tab-sensor-' + item.dtuId + '" class="tab-pane row-fluid">' +
            '<div class="clearfix">' +
            '<div class="row-fluid">' +
            '<div class="form-horizontal">' +
                '<div id="prompt-sensor-' + item.dtuId + '"></div>' +
                // 批量即时采集
                '<div style="margin: 10px 15px; float: left;">' +
                '<input id="btnBatchSend-' + item.dtuId + '" type="button" class="btn blue btnBatchSend" value="批量即时采集" />' +
                '<div class="prompt-orange"><i class="fa fa-exclamation-triangle fa-orange marginRight10"></i>允许一次最多选择5个传感器下发即时采集命令</div>' +
                '<div id="prompt-dtuSensors-dtuType" class="label-red" style="display: none;">该DTU为传输型DTU, 因此不能对该DTU下任何传感器进行"即时采集"</div></div>' +
                // 提示: 一次最多操作5个传感器
                '<div id="tip-realtime-' + item.dtuId + '" class="fixed-prompt"></div>' +
            '</div></div></div>' +
            '<table id="tableBatchRealtimeAcqusition-' + item.dtuId + '" class="table table-striped table-bordered tableBatchRealtimeAcqusition">' +
            '<thead><tr>' +
                '<th style="width: 15px;"><input type="checkbox" id="groupCheckbox-' + item.dtuId +
                    '" class="group-checkable" data-set="#tableBatchRealtimeAcqusition-' + item.dtuId + ' .checkboxes" /></th>' +
                '<th style="width: 15%">传感器</th>' +
                '<th style="width: 7%">模块号</th>' +
                '<th style="width: 7%">通道号</th>' +
                '<th style="width: 16%">上次采集状态</th>' +
                '<th style="width: 10%">操作</th>' +
                '<th>即时采集结果</th></tr></thead>' +
            '<tbody id="tbodyBatchRealtimeAcqusition-' + item.dtuId + '" class="tbodyBatchRealtimeAcqusition"></tbody></table></div></div></div></div></div>';
    });
    $('#accordion-listDtuStatus').html(domListDtuStatus);
    
    // 绑定DTU下传感器"批量即时采集"相关事件.
    bindEventsOnBatchRealtimeAcqusition();

    // 若点击"accordion-toggle-dtu"中某DTU"当前状态"栏, 则获取并展开该DTU的最新状态及历史状态
    if (g_currentActiveDtuId != null && !jQuery.isEmptyObject(g_toggledDtus)) {
        var dtuId = g_toggledDtus["dtu-" + g_currentActiveDtuId].dtuId;
        rerenderDtuCategory(status);
        toggleListDtuStatus(dtuId);
        getDtuLatestStatus(dtuId, true, false); // 显示当前DTU最新状态
        getDtuHistoryStatus(dtuId);
    }
}

function onclickDtuItem(iDtuId) {
    if (g_toggledDtus["dtu-" + iDtuId] == null) {
        g_toggledDtus["dtu-" + iDtuId] = { dtuId: iDtuId, isShown: false }; // assign value. 初始化为"false: 折叠状态"
    }
    g_currentActiveDtuId = iDtuId; // assign value. 设置被点击DTU为当前活动DTU.
    getDtuLatestStatus(iDtuId, false, false); // 获取但不显示当前DTU最新状态

    if (g_currentActiveDtuSetIntervalId != null) {
        stopTimerOfDtuStatus();
    }
    startTimerOfDtuStatus();
}

function startTimerOfDtuStatus() {
    // refresh DTU status per minute.
    g_currentActiveDtuSetIntervalId = setInterval(function () { // assign value
        if (g_currentActiveDtuId == null && g_currentActiveDtuSetIntervalId != null) {
            stopTimerOfDtuStatus();
        } else {
            getDtuLatestStatus(g_currentActiveDtuId, false, true); // 获取但不显示当前DTU最新状态
        }
    }, 60000);
}

function stopTimerOfDtuStatus() {
    clearInterval(g_currentActiveDtuSetIntervalId);
}

function rerenderDtuCategory(status) {
    $('#category-' + status + 'Dtu').removeClass('like-a');
    var $others = $('.category-dtu').not('#category-' + status + 'Dtu');
    $.each($others, function (index, item) {
        $(item).addClass('like-a');
    });
}

/**
 * 点击"DTU状态"列表, 展开被选DTU, 收缩其它DTU, 并切换"展开/收缩"图标
 */
function toggleListDtuStatus(dtuId) {
    var dom = $('#accordion-toggle-dtu-' + dtuId)[0];
    var $list = $('a.accordion-toggle-dtu');
    $.each($list, function (index, item) {
        if (item != dom) {
            $(item).parent().next().addClass('display-none');
            $('i.accordion-icon-dtu', item).removeClass('fa-angle-down').addClass('fa-angle-up');
            var sDtuId = item.id.split('accordion-toggle-dtu-')[1];
            if (g_toggledDtus["dtu-" + sDtuId] != null) {
                g_toggledDtus["dtu-" + sDtuId].isShown = false;
            }
        }
    });
    var $target = $(dom).parent().next();
    if (g_isCurrentActiveDtuRefreshed) { // 若当前DTU栏处于"展开"状态, 则定时刷新DTU状态时继续"展开"该DTU状态栏下的内容
        $target.removeClass('display-none');
        $('i.accordion-icon-dtu', dom).removeClass('fa-angle-up').addClass('fa-angle-down');
        g_toggledDtus["dtu-" + dtuId].isShown = true;
    } else {
        if ($target.is(':visible') || g_toggledDtus["dtu-" + dtuId].isShown) { // "$target.is(':visible')" is always false
            $target.addClass('display-none');
            $('i.accordion-icon-dtu', dom).removeClass('fa-angle-down').addClass('fa-angle-up');
            g_toggledDtus["dtu-" + dtuId].isShown = false;
        } else {
            $target.removeClass('display-none');
            $('i.accordion-icon-dtu', dom).removeClass('fa-angle-up').addClass('fa-angle-down');
            g_toggledDtus["dtu-" + dtuId].isShown = true;
        }
    }
}

function getDtuLatestStatus(iDtuId, isDisplayed, isRefreshed) {
    $('#loading-projectDashboard').show();

    g_isCurrentActiveDtuRefreshed = isRefreshed; // assign value
    
    var dtuId = g_toggledDtus["dtu-" + iDtuId].dtuId;
    var url = apiurl + '/dtu/' + dtuId + '/status' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            if (data == null) {
                $('#loading-projectDashboard').hide();
                $('#container-dtuLatestStatus-' + dtuId).html('<span class="label-red">该DTU不存在或已被删除</span>');
                return;
            }
            if (isDisplayed == false) {
                getDtuLatestStatusToUpdateOthers(data);
            } else {
                showDtuLatestStatus(data, dtuId);
            }
            $('#loading-projectDashboard').hide();
        },
        error: function (xhr) {
            $('#loading-projectDashboard').hide();
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取DTU最新状态时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

function getDtuLatestStatusToUpdateOthers(data) {
    var target = $('.category-dtu').not('.like-a')[0];
    if (target.id == 'category-allDtu') {
        getDtusStatusByStruct('all');
    } else {
        if (data.status == false) {
            getDtusStatusByStruct('offline');
        } else if (data.status) {
            getDtusStatusByStruct('online');
        } else {
            getDtusStatusByStruct('neverUpline');
        }
    }
}

function showDtuLatestStatus(data, dtuId) {
    // 提示: 该DTU为传输型DTU, 因此不能对该DTU进行"远程修改"或"重启"
    data.identification.trim() == '实体型' ? $('#prompt-dtu-dtuType').css('display', 'none') : $('#prompt-dtu-dtuType').css('display', 'block');
    
    // DTU状态
    var domI;
    if (data.status == false) {
        domI = '<i class="fa fa-circle" style="color: gray; font-size: 16px;"></i>';
    } else if (data.status) {
        domI = '<i class="fa fa-circle" style="color: green; font-size: 16px;"></i>';
    } else {
        domI = 'N/A';
    }
    // DTU最近在线时长
    var strDateDiff;
    if (data.lastOnlineTime != null && data.currentOfflineTime != null) {
        strDateDiff = GetDateDiff(GetMilliseconds(data.currentOfflineTime) - GetMilliseconds(data.lastOnlineTime));
    } else if (data.lastOnlineTime != null) {
        strDateDiff = GetDateDiff(GetMilliseconds(data.now) - GetMilliseconds(data.lastOnlineTime));
    } else {
        strDateDiff = "N/A";
    }
    // DTU操作权限控制
    var tdOperation;
    if (data.identification.trim() == '实体型' && data.status) { // DTU为"实体型"且"在线"时, 可对DTU进行操作
        tdOperation = '<a href="#viewDtuModal" data-toggle="modal" class="view">基本信息</a> | ' +
            '<a href="#modifyDtuRemoteConfig" data-toggle="modal" class="modify">远程修改</a> | ' +
            '<span id="tdDtuRestart"><a href="javascript:restartDtu();">重启</a></span>';
    } else {
        tdOperation = '<a href="#viewDtuModal" data-toggle="modal" class="view">基本信息</a> | ' +
            '<span style="color: gray;">远程修改</span> | <span style="color: gray;">重启</span>';
    }

    var dom =
        '<table id="table-dtuLatestStatus" class="table table-striped table-bordered">' +
        '<thead><tr><th style="width: 12%;">DTU编号</th>' +
        '<th style="width: 8%;">状态</th>' +
        '<th style="width: 20%;">最近在线时长</th>' +
        '<th style="width: 20%;">最近上线时间</th>' +
        '<th style="width: 20%;">最近下线时间</th>' +
        '<th>操作</th></tr></thead>' +
        '<tbody><tr id="tr-' + dtuId + '"><td>' + data.dtuNo + '</td>' +
        '<td>' + domI + '</td>' +
        '<td>' + strDateDiff + '</td>' +
        '<td>' + (data.lastOnlineTime != null ? JsonToDateTime(data.lastOnlineTime) : 'N/A') + '</td>' +
        '<td>' + (data.currentOfflineTime != null ? JsonToDateTime(data.currentOfflineTime) : 'N/A') + '</td>' +
        '<td>' + tdOperation + '</td></tr></tbody></table>';
    $('#container-dtuLatestStatus-' + dtuId).html(dom);
    $('#collapse-dtu-' + dtuId).css('height', 'auto');

    bindDtuTableClickEvent(dtuId);
}

function bindDtuTableClickEvent(dtuId) {
    // DTU"基本信息"点击事件
    $('#table-dtuLatestStatus').on('click', 'a.view', function (e) {
        // e.preventDefault();
        var selectedRow = $(this).parents('tr')[0];
        getDtuDetails(selectedRow, true);
    });
    
    // DTU"远程修改"点击事件
    $('#table-dtuLatestStatus').on('click', 'a.modify', function (e) {
        // e.preventDefault();
        emptyDtuRemoteConfigResults(); // handle in dtuRemoteManage.js

        var selectedRow = $(this).parents('tr')[0];
        getDtuDetails(selectedRow, false);
    });

    // "DTU下传感器"点击事件
    $('#tabSensor-' + dtuId).click(function () {
        if (g_currentActiveDtuSetIntervalId != null) {
            stopTimerOfDtuStatus(); // 停止自动刷新DTU状态
        }
        undoDomGroupCheckable(dtuId); // handle in sensorRealtimeAcquisition.js
        getSensorsStatusByStructAndDtu(dtuId); // handle in sensorRealtimeAcquisition.js
    });
}

function getDtuDetails(selectedRow, isDisplayed) {
    $('#loading-projectDashboard').show();
    
    var dtuId = parseInt(selectedRow.id.split('tr-')[1]);
    var dtuNo = selectedRow.cells[0].innerText;

    var url = apiurl + '/dtu/' + dtuId + '/details' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            if (isDisplayed == false) {
                showModifyModeless(data, dtuNo); // handle in "dtuRemoteManage.js"
            } else {
                $('#viewDtuModal-dtuNo').html(dtuNo);
                if (data == null) {
                    var prompt = '<span class="label-red">该DTU无基本信息</span>';
                    $('#loading-projectDashboard').hide();
                    $('#prompt-noData-dtuInfo').show().html(prompt);
                    $('#table-dtuInfo').hide();
                    return;
                }
                showDtuDetails(data);
            }
            $('#loading-projectDashboard').hide();
        },
        error: function (xhr) {
            $('#loading-projectDashboard').hide();
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取DTU基本信息时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

function showDtuDetails(data) {
    $('#prompt-noData-dtuInfo').hide();
    $('#table-dtuInfo').show();

    var factory = (data.factory == null || data.factory.trim() == "") ? "无" : data.factory;
    var model = (data.model == null || data.model.trim() == "") ? "无" : data.model;
    var network = (data.network == null || data.network.trim() == "") ? "无" : data.network;
    var sim = (data.sim == null || data.sim.trim() == "") ? "无" : data.sim;
    var granularity = (data.granularity == null) ? "无" : data.granularity;
    var mode = (data.mode == null || data.mode.trim() == "") ? "无" : data.mode;
    var ip = (data.ip == null || data.ip.trim() == "") ? "无" : data.ip;
    var port = (data.port == null) ? "无" : data.port;
    var ip2 = (data.ip2 == null || data.ip2.trim() == "") ? "无" : data.ip2;
    var port2 = (data.port2 == null) ? "无" : data.port2;
    var packetInterval = (data.packetInterval == null) ? "无" : data.packetInterval;
    var reconnectionCount = (data.reconnectionCount == null) ? "无" : data.reconnectionCount;

    var content1 =
        '<tr><td>' + factory + '</td><td>' + model + '</td><td>' + network + '</td><td>' + sim +
            '</td><td>' + granularity + '</td><td>' + mode + '</td></tr>';
    var content2 =
        '<tr><td>' + ip + '</td><td>' + port + '</td><td>' + ip2 + '</td><td>' + port2 +
            '</td><td>' + packetInterval + '</td><td>' + reconnectionCount + '</td></tr>';

    $('#tbody1-dtuInfo').html(content1);
    $('#tbody2-dtuInfo').html(content2);
}

/**
 * 获取DTU最近24小时历史状态
 */
function getDtuHistoryStatus(dtuId) {
    $('#loading-projectDashboard').show();
    
    var startTime = getDateAndTime(-1);
    var endTime = getDateAndTime(0);
    var url = apiurl + '/dtu/' + dtuId + '/history-status/' + startTime + '/' + endTime + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            if (data.datas.length == 0) {
                var tip = '<span class="label-red">该DTU最近24小时无数据</span>';
                if (data.dtuNo == null) {
                    tip = '<span class="label-red">该DTU不存在或已被删除</span>';
                }
                $('#loading-projectDashboard').hide();
                $('#chart-dtuStatus-' + dtuId).html(tip);
                return;
            }
            showDtuHistoryStatus(data, dtuId);
        },
        error: function (xhr) {
            $('#loading-projectDashboard').hide();
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取DTU最近24小时历史状态时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

function showDtuHistoryStatus(data, dtuId) {
    areaChartOptions.series[0].data = []; // empty it
    
    areaChartOptions.chart.height = 200;
    areaChartOptions.title.text = '最近24小时DTU状态趋势图';
    areaChartOptions.yAxis.title.text = null;
    // customize the labels of yAxis
    areaChartOptions.yAxis.labels.formatter = function () {
        var labelY;
        if (this.value == 1) {
            labelY = '在线';
        } else if (this.value == 0) {
            labelY = '离线';
        } else {
            labelY = '';
        }
        return labelY;
    };
    
    var iNow = parseInt(data.now.substring(6, 19));
    $.each(data.datas, function (index, item) {
        var iTime = parseInt(item.time.substring(6, 19));
        if (item.status) {
            //areaChartOptions.series[0].data.push({ name: '上线', x: iTime, y: 0 });
            areaChartOptions.series[0].data.push({ name: '上线', x: iTime, y: 1 });
        } else {
            areaChartOptions.series[0].data.push({ name: '下线', x: iTime, y: 1 });
            areaChartOptions.series[0].data.push({ name: '下线', x: iTime, y: null });
        }
        // 若DTU最新状态"在线", 则DTU历史状态趋势图中最近在线状态持续呈现到"当前时间".
        if (index == data.datas.length - 1 && item.status) {
            areaChartOptions.series[0].data.push({ name: '在线', x: iNow, y: 1 });
        }
    });
    // Fires when the chart is finished loading.
    areaChartOptions.chart.events.load = function () {
        $('#loading-projectDashboard').hide();
    };
    // customize tooltip
    areaChartOptions.tooltip.formatter = function () {
        return '<b>' + this.point.name + '</b><br/>' +
            '采集时间: ' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
    };
    
    // create the chart
    var chart = $('#chart-dtuStatus-' + dtuId).highcharts();
    if (chart != null) {
        chart.destroy();
    }
    $('#chart-dtuStatus-' + dtuId).highcharts(areaChartOptions);
    $('#collapse-dtu-' + dtuId).css('height', 'auto');
}


/**
 * 获取指定时间(yyyy-MM-dd hh:mm:ss), 时间精确到秒
 */
function getDateAndTime(n) {
    var d = new Date();
    d.setDate(d.getDate() + n);
    d = d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate() + " " + d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds();
    return d.replace(/\b(\w)\b/g, '0$1');
}

/**
 * 将json时间差的毫秒数转换为时间差的小时数
 */
function GetDateDiff(millisecondsDiff) {
    var returnstr = "";
    if (millisecondsDiff >= 0) {
        // 计算出天数
        var dayDiff = Math.floor(millisecondsDiff / (24 * 3600 * 1000));
        returnstr += dayDiff + "天";
        // 计算出小时数 
        var leftMilliSeconds1 = millisecondsDiff % (24 * 3600 * 1000); // 计算天数后剩余的毫秒数 
        var hourDiff = Math.floor(leftMilliSeconds1 / (3600 * 1000));
        returnstr += hourDiff + "时";
        // 计算相差分钟数 
        var leftMilliSeconds2 = leftMilliSeconds1 % (3600 * 1000); // 计算小时数后剩余的毫秒数 
        var minuteDiff = Math.floor(leftMilliSeconds2 / (60 * 1000));
        returnstr += minuteDiff + "分";
        //// 计算相差秒数 
        //var leftMilliSeconds3 = leftMilliSeconds2 % (60 * 1000); // 计算分钟数后剩余的毫秒数 
        //var secondDiff = Math.round(leftMilliSeconds3 / 1000);
        //returnstr += secondDiff + "秒";
    }
    return returnstr;
}

/**
 * 函数功能：渲染表格
 */
function extendDatatable() {
    $('#table-projectStructsStatus').dataTable({
        "iDisplayLength": -1,
        //"sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
        "sDom": 'T<"clear">rt',
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "aaSorting": [[2, "desc"]],
    });
}

/**
 * 判断空对象:{}
 */
function isEmptyObject(obj) {
    for (var name in obj) {
        return false;
    }
    return true;
}


/**传感器状态功能实现代码**/
/**start 传感器功能**/
function GetSensorInformationByStruct() {
    getAlarmAndDtuAndSensorStatusByStruct(); // 更新"结构物下告警/DTU/传感器状态"模块
    
    var url = apiurl + '/statistics/struct/' + g_currentStructId + '/sensors/status' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        //cache: false, // 必须加token，该属性才能通过代理服务
        success: function(data) {
            if (data == null || data.length == 0) {
                $('#allInfor').removeClass('display-none').addClass('display-block');
               // var typeName1 = "传感器采集的最新状态";
                //GetStatusChartByUnable(typeName1);
                GetUnableSensorList(1);
                return;
            }
            $('#allInfor').removeClass('display-none').addClass('display-block');
            GetUnableSensorList(2);
            var typeName = "传感器采集的最新状态";
            GetStatusChartByStructId(data,typeName);
        },
        error: function(xhr) {
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) {
                alert("获取用户下项目状态统计信息时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });

}
//禁用传感器 
var unableSensorList = '';
var enableSensorData = '';
function GetUnableSensorList(indentify) {
    var typeName1 = "传感器采集的最新状态";
    var url = apiurl + '/statistics/struct/unable/' + g_currentStructId + '/sensors' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        //cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            if (data == null || data.length == 0) {
                unableSensorList = 0;
                GetStatusChartByUnable(typeName1);
                return;
            }
            unableSensorList = data.length;
            enableSensorData = data;
            if (indentify == 1) {
                GetChartByEnable(typeName1, data);
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) {
                alert("获取用户下项目状态统计信息时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}


//只有禁用和未知
function GetChartByEnable(typeName, data) {
    //$('#loading-sensorDashboard').hide();
    var colors = Highcharts.getOptions().colors;
    var arrStatusName = [];
    var allColor = [];
    var allKnowSensorId = [];
    for (var k = 0; k < data.length; k++) {
        allKnowSensorId.push(data[k].Id);
    }
    //增加未知状态
    var unableSensorCount = sensorTotles - unableSensorList;
    if (unableSensorCount > 0) {
        arrStatusName.push("未知状态");
        allColor.push({ y: unableSensorCount, color: colors[0] });
    }
    //增加禁用传感器
    var enableSensorCount = unableSensorList;
    if (unableSensorCount > 0) {
        arrStatusName.push("被禁用的传感器");
        allColor.push({ y: enableSensorCount, color: colors[1] });
    }
    var barChartOptions = new CreateHighchartBar('chart-sensorDashboard', typeName);
    barChartOptions.yAxis.title.text = '正常/异常传感器个数';
    barChartOptions.xAxis.categories = arrStatusName;

    for (var i = 0; i < arrStatusName.length; i++) {
        var lengtha = arrStatusName[i].length * 15;
        if (lengtha > 140) {
            barChartOptions.chart.marginLeft = lengtha;
            break;
        }
    }
    barChartOptions.series.push({
        name: '传感器个数',
        data: allColor,
        dataLabels: {
            enabled: true,
            rotation: -0,
            x: 18,
            y: 0,
            align: 'right',
            style: {
                fontWeight: 'bold',
                color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
            }
        }
    });

    // 每个异常情况的点击事件
    barChartOptions.plotOptions.series.point.events.click = function () {
        var statusName = this.category;
        $('#sensorPostionInformation').text(statusName);
        if (statusName != "未知状态") {
            GetSensorListByEnable(data, statusName);
        }
        else {
            GetUnknowSensor(allKnowSensorId, statusName);
        }

        DisplayIndex(2);
    };
    var chart = new Highcharts.Chart(barChartOptions);
}


//获取禁用的列表

function GetSensorListByEnable(data, statusName) {
    $('#loadChart').removeClass('span12').addClass('span6');
    $('#chart-sensorDashboard').highcharts().reflow();
    $('#sensorInformatons').attr('style', 'display: block;');
    $('#sensorPostionInformation').html(statusName + '的传感器列表');
    $('#tableSensorsInformaton').dataTable().fnDestroy();
    $('#tbodySensorsInformaton').html("");
    var sb = '';
    for (var i = 0; i < data.length; i++) {
        sb += "<tr id='sensor-" + data[i].Id + "'><td ><a class='aSensorName' href='javascript:;'>" + data[i].location + "</a></td>";
        sb += "<td>N/A</td>";
        sb += "</tr>";
    }
    $('#tbodySensorsInformaton').html("");
    $('#tbodySensorsInformaton').html(sb);
    var a = $('#tableSensorsInformaton');
    extendDatatableByTime(a);
}

//未知状态 
function GetStatusChartByUnable(typeName) {
    //增加未知状态
    var arrStatusName = [];
    var allColor = [];
    var allKnowSensorId = [0];
    var colors = Highcharts.getOptions().colors;
    var unableSensorCount = sensorTotles;
        arrStatusName.push("未知状态");
        allColor.push({ y: unableSensorCount, color: colors[0] });

    var barChartOptions = new CreateHighchartBar('chart-sensorDashboard', typeName);
    barChartOptions.yAxis.title.text = '正常/异常传感器个数';
    barChartOptions.xAxis.categories = arrStatusName;
    
    barChartOptions.series.push({
        name: '传感器个数',
        data: allColor,
        dataLabels: {
            enabled: true,
            rotation: -0,
            x: 18,
            y: 0,
            align: 'right',
            style: {
                fontWeight: 'bold',
                color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
            }
        }
    });
    // 每个异常情况的点击事件
    barChartOptions.plotOptions.series.point.events.click = function () {
        var statusName = this.category;
        $('#sensorPostionInformation').text(statusName);
            GetUnknowSensor(allKnowSensorId, statusName);
        DisplayIndex(2);
    };
    var chart = new Highcharts.Chart(barChartOptions);
}

/**考虑那些展示**/

function DisplayIndex(index) {
    switch (index) {
    case 1:
        //点击结构物
        $('#collapse-sensor').hide();
        break;
    case 2:
        //点击最新状态
        $('#sensorInformatons').removeClass('display-none').addClass('display-block');
        $('#oneChart').removeClass('display-block').addClass('display-none');
        $('#lastTime').removeClass('display-block').addClass('display-none');
        break;
    case 3:
        //点击传感器列表
        $('#lastTime').removeClass('display-block').addClass('display-none');
        break;
    case 4:
        //点击标题传感器状态
        $('#sensorInformatons').hide();
        $('#oneChart').removeClass('display-block').addClass('display-none');
        $('#lastTime').removeClass('display-block').addClass('display-none');
        break;
    case 5:
        $('#collapse-sensor').show();
        break;
    }
}

function GetStatusChartByStructId(data, typeName) {
    //$('#loading-sensorDashboard').hide();
    var colors = Highcharts.getOptions().colors;
    var arrStatusName = [];
    var allColor = [];
    var projects = {};
    var allCount =0;
    var j = 0;
    var allKnowSensorId = [];
    //et传的
    $.each(data, function(i, item) {
        arrStatusName.push(item.status);
        projects[item.status] = {
            tyoeId: item.status,
            count: item.count
        };
        if (item.status == "成功") {
            allColor.push({ y: item.count, color: colors[2] });

        } else {
            allColor.push({ y: item.count, color: colors[j + 3] });
        }
        j++;
        allCount += item.count;
        var sensors = item.sensors;
        for (var k = 0; k < sensors.length; k++) {
            allKnowSensorId.push(sensors[k].sensorId);

        }

    });
    //禁用的
    for (var j = 0; j < enableSensorData.length; j++) {
        allKnowSensorId.push(enableSensorData[j].Id);

    }
    
    //增加禁用的状态
    var enableSensorCount = unableSensorList;
    if (enableSensorCount > 0) {
        arrStatusName.push("被禁用的传感器");
        allColor.push({ y: enableSensorCount, color: colors[1] });
    }
    //增加未知状态
    var unableSensorCount = sensorTotles - allCount-unableSensorList;
    if (unableSensorCount > 0) {
        arrStatusName.push("未知状态");
        allColor.push({ y: unableSensorCount, color: colors[0] });
    }

    var barChartOptions = new CreateHighchartBar('chart-sensorDashboard', typeName);
    barChartOptions.yAxis.title.text = '正常/异常传感器个数';
    barChartOptions.xAxis.categories = arrStatusName;

    for (var i = 0; i < arrStatusName.length; i++) {
        var lengtha = arrStatusName[i].length * 15;
        if (lengtha > 140) {
            barChartOptions.chart.marginLeft = lengtha;
          break;
       }
    }
    
    // highcharts自适应高度
    var len = arrStatusName.length;
    if (len > 75) {
        barChartOptions.chart.height = 2000;
    } else if (len > 50 && len <= 75) {
        barChartOptions.chart.height = 1500;
    } else if (len > 25 && len <= 50) {
        barChartOptions.chart.height = 1000;
    } else {
        barChartOptions.chart.height = 400;
    }
    barChartOptions.series.push({
        name: '传感器个数',
        data:allColor,
        dataLabels: {
            enabled: true,
            rotation: -0,
            x:18,
            y:0,
            align: 'right',
            style: {
                fontWeight: 'bold',
                color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
            }
        }
    });

    // 每个异常情况的点击事件
    barChartOptions.plotOptions.series.point.events.click = function() {
        var statusName = this.category;
        $('#sensorPostionInformation').text(statusName);
        if (statusName == "未知状态") {
            GetUnknowSensor(allKnowSensorId, statusName);
        } else if (statusName == "被禁用的传感器") {
            GetSensorListByEnable(enableSensorData, statusName);
        }else {
            GetSensorListByTypeId(data, statusName);

        }
        
        DisplayIndex(2);
    };
    var chart = new Highcharts.Chart(barChartOptions);
}

function GetUnknowSensor(allKnowSensorId, tableName) {
    $('#loadChart').removeClass('span12').addClass('span6');
    $('#chart-sensorDashboard').highcharts().reflow();
    $('#sensorInformatons').attr('style', 'display: block;');
    $('#sensorPostionInformation').html(tableName + '的传感器列表');
    $('#tableSensorsInformaton').dataTable().fnDestroy();
    $('#tbodySensorsInformaton').html("");
   
    var url = apiurl + '/statistics/struct/' + g_currentStructId + '/' + allKnowSensorId + '/sensors/' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        //cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            if (data == null || data.length == 0) {
                return;
            }
            var sb = '';
            for (var i = 0; i < data.length; i++) {
               
                sb += "<tr><td >" + data[i].location + "</td>";
                        sb += "<td>N/A</td>";
                        sb += "</tr>";
                    }
            $('#tbodySensorsInformaton').html("");
            $('#tbodySensorsInformaton').html(sb);
            var a = $('#tableSensorsInformaton');
            extendDatatableByTime(a);
        },
        error: function (xhr) {
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) {
                alert("获取结构物下的传感器信息时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });

    
}

/*根据异常类型获得对应的传感器列表*/

function GetSensorListByTypeId(data, tableName) {
    $('#loadChart').removeClass('span12').addClass('span6');
    $('#chart-sensorDashboard').highcharts().reflow();
    $('#sensorInformatons').attr('style', 'display: block;');
    $('#sensorPostionInformation').html(tableName + '的传感器列表');
    $('#tableSensorsInformaton').dataTable().fnDestroy();
    $('#tbodySensorsInformaton').html("");
    var sb = '';
    for (var i = 0; i < data.length; i++) {
        if (data[i].status == tableName) {
            var sensorList = data[i].sensors;
            for (var j = 0; j < sensorList.length; j++) {
                sb += "<tr id='sensor-" + sensorList[j].sensorId + "'><td ><a class='aSensorName' href='javascript:;'>" + sensorList[j].location + "</a></td>";
                sb += "<td>" + TimeFormat(sensorList[j].time) + '</td>';
                sb += "</tr>";
            }
            break;
        }
    }
    $('#tbodySensorsInformaton').html("");
    $('#tbodySensorsInformaton').html(sb);
    var a = $('#tableSensorsInformaton');
    extendDatatableByTime(a);
}

//毫秒级时间

function TimeFormat(jsonDate) {
    jsonDate = jsonDate.replace("/Date(", "").replace(")/", "");
    if (jsonDate.indexOf("+") > 0) {
        jsonDate = jsonDate.substring(0, jsonDate.indexOf("+"));
    } else if (jsonDate.indexOf("-") > 0) {
        jsonDate = jsonDate.substring(0, jsonDate.indexOf("-"));
    }
    var milliseconds = parseInt(jsonDate, 10);
    var date = new Date(milliseconds);
    //转换成标准的“月：MM”
    var normalizedMonth = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
    var dateTime = date.getFullYear() + "-" + normalizedMonth + "-" + normalizeTimeFormat(date.getDate())
        + " " + normalizeTimeFormat(date.getHours()) + ":" + normalizeTimeFormat(date.getMinutes()) + ":" + normalizeTimeFormat(date.getSeconds());
    return dateTime;
}

$('#tableSensorsInformaton').on('click', 'a.aSensorName', function () {
    //局部刷新最新传感器状态、列表、标题、对应的结构物列表、项目图形
    GetSensorInformationByStruct();

    var tr = $(this).parents('tr');
    var selectedRow = tr[0];
    var sensorId = parseInt(selectedRow.id.split('sensor-')[1]); // assign value
    GetSensorInformationBySensorId(sensorId);
    DisplayIndex(3);
});

/**传感器详细信息**/

function GetSensorInformationBySensorId(sensorId) {
    $('#tableOneSensorInformation').dataTable().fnDestroy();
    $('#oneChart').removeClass('display-none').addClass('display-block');
    var url = apiurl + '/sensor/' + sensorId + '/'+g_currentStructId + '/status' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        //cache: false, // 必须加token，该属性才能通过代理服务
        success: function(data) {
            if (data == null) {
                $('#tbodyOneSensorInformation').html("");
                extendDatatableBySensorId();
                return;
            }
            var domI = '';
            if (data.dtuStatus == false) {
                domI = '<i class="fa fa-circle" style="color: gray; font-size: 16px;"></i>';
            } else if (data.dtuStatus) {
                domI = '<i class="fa fa-circle" style="color: green; font-size: 16px;"></i>';
            } else {
                domI = 'N/A';
            }
            var sb = '';
            sb += "<tr id='dtu-" + data.dtuId +'-'+ data.dtuIdentify + "'><td>" + data.location + "</td>";
            sb += "<td>" + data.dtuNo + '</td>';
            sb += "<td>" + domI + '</td>';
            sb += "<td>" + data.module + '</td>';
            sb += "<td>" + data.channel + '</td>';
            sb += "<td>" + data.dacStatus + '</td>';
            if (!data.enable) {
                sb += "<td>使用</td>";
            } else {
                sb += "<td>禁用</td>";
            }
            // "即时采集"按钮
            var str = "<td id='btnSend-" + data.dtuId + "-" + sensorId + "' class='dtu-sensor-single'><a href='javascript: GetRealtimeRequest(" +
                data.dtuId + ", " + sensorId + "," + data.dtuIdentify + "," + data.dtuStatus + "," + data.enable +
                ")' class='RealtimeRequest'>即时采集</a> | ";
            str += "<a href='javascript: GetSensorChartLastDataBySensorId(" + sensorId + ");'>最近24小时异常</a></td>";
            sb += str;
            sb += "<td><span id='realtimeData-" + sensorId + "'></span>" +
                "<span id='timeContainer-" + sensorId + " ' style='display: none; margin-left: 10%;'>剩余等待时间:" +
                " <span id='time-" + sensorId + "' style='color: #ff0000'></span>秒</span></td>";
            sb += "</tr>";
            $('#tbodyOneSensorInformation').html("");
            $('#tbodyOneSensorInformation').html(sb);
            extendDatatableBySensorId();
        },
        error: function(xhr) {
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取传感器详细信息时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

function GetRealtimeRequest(dtuId, oSensorId, dtuIdentify, dtuStatus, enable) {
    if (!dtuStatus) {
        alert("禁止使用即时采集!");
        return;
    }
        //else if (enable) {
        //    alert("禁止使用即时采集!");
        //    return;
        //}
    else if (dtuIdentify != 1) {
        alert("禁止使用即时采集!");
        return;
    } else {
        g_currentDtuSensor["sensor-" + oSensorId] = {
            dtuId: dtuId,
            sensorId: oSensorId,
            dtuIdentify: dtuIdentify,
            dtuStatus: dtuStatus,
            enable: enable
        };
        sendSensorRealtimeRequest(dtuId, oSensorId); // handle in "sensorRealtimeAcquisition.js"
    }
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

/**最近24小时数据**/
function GetSensorChartLastDataBySensorId(sensorId) {
    $('#lastTime').removeClass('display-none').addClass('display-block');
    var url = apiurl + '/statistics/sensor/' + sensorId + '/abnormal/' + showdate(-1) + '/' + showdate(0) + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        //cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            if (data == null || data.length == 0) {
              //  $('#oneSensorChart').hide();
                $('#alertOneInfor').html("抱歉没有查询到任何有效数据！");
                $('#allOneInfor').hide();
                return;
            }
            $('#alertOneInfor').hide();
            $('#lastTime').show();
            $('#sensorListInformation').hide();
            $('#loadSensorChart').removeClass('span6').addClass('span12');
            var typeName = "最近24小时传感器异常次数统计";
            GetLastChart(data, typeName, sensorId);
        },
        error: function (xhr) {
           // $('#oneSensorChart').hide();
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取用户下项目状态统计信息时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

/**最近24小时图形**/


function GetLastChart(data, typeName, sensorId) {
    $('#load3').removeClass('fa-angle-up').addClass('fa-angle-down');
    $('#modfy').show();
    var arrStatusName = [];
    var arrCount = [];
    var projects = {};
    var colors = Highcharts.getOptions().colors;
    var j = 0;
    $.each(data, function (i, item) {
        var splitNameCount = item.data.split(":");
        arrStatusName.push(splitNameCount[0]);
        var a = parseInt(splitNameCount[1]);
        arrCount.push({ y: a ,color:colors[j]});
        projects[item.alarmTypeId] = {
            typeName: item.data.split(':')[0],
            typeId: item.alarmTypeId,
        };
        j++;

    });
    var barChartOptions = new CreateHighchartBar('sensor-Chart', typeName);
    barChartOptions.yAxis.title.text = '异常情况的次数';
    barChartOptions.xAxis.categories = arrStatusName;
    
    for (var i = 0; i < arrStatusName.length; i++) {
        var lengtha = arrStatusName[i].length * 15;
        if (lengtha > 140) {
            barChartOptions.chart.marginLeft = lengtha;
            break;
        }
    }
    // highcharts自适应高度
    var len = arrStatusName.length;
    if (len > 75) {
        barChartOptions.chart.height = 2000;
    } else if (len > 50 && len <= 75) {
        barChartOptions.chart.height = 1500;
    } else if (len > 25 && len <= 50) {
        barChartOptions.chart.height = 1000;
    } else {
        barChartOptions.chart.height = 400;
    }
    barChartOptions.series.push({
        name: '异常情况的次数',
       // color: '#3CB371',
        data: arrCount,
        dataLabels: {
            enabled: true,
            rotation: -0,
            x:18,
            align: 'right',
            style: {
                fontWeight: 'bold',
                color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
            }
        }
    });
    // 每个异常情况的点击事件
    barChartOptions.plotOptions.series.point.events.click = function() {
        var statusName = this.category;
        var nowName = {};
        for (var key in projects) {
            if (projects[key].typeName == statusName) {
                nowName = projects[key]; // assign value.
                break;
            }
        }
        
        setSensor(nowName, sensorId);
    };
    var chart = new Highcharts.Chart(barChartOptions);
}


/**最近24小时列表**/

function setSensor(typeName, sensorId) {
    $('#loadSensorChart').removeClass('span12').addClass('span6');
    $('#sensor-Chart').highcharts().reflow();
    $('#sensorListInformation').attr('style', 'display: block;');
    $('#sensor-ListHeader').html(typeName.typeName + '的列表');
    $('#tableSensorListInformation').dataTable().fnDestroy();
    var a = $('#tableSensorListInformation');
    var url = apiurl + '/sensor/' + sensorId + '/alarm-type/' + typeName.typeId + '/history-status/' + showdate(-1) + '/' + showdate(0) + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        //cache: false, // 必须加token，该属性才能通过代理服务
        success: function(data) {
            if (data == null) {
                $('#tbodySensorListInformation').html("");
                extendDatatableByTime(a);
                return;
            }
            var timeList = data.datas;
            var sb = '';
            for (var i = 0; i < timeList.length; i++) {
                sb += "<tr><td>" + data.location + "</td>";
                sb += "<td>" + TimeFormat(timeList[i].time) + "</td>";
                sb += "</tr>";
            }
            $('#tbodySensorListInformation').html("");
            $('#tbodySensorListInformation').html(sb);
            
            extendDatatableByTime(a);
        },
        error: function(xhr) {
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取传感器详细信息时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}


// 点击"传感器最新状态统计/传感器状态/最近24小时", 切换"展开/收缩"图标
$('#load1').click(function () {
    var a = $(this);
    var b = $('#allInfor');
    RemoveAndAdd(a, b);
});

$('#load2').click(function () {
    var a = $(this);
    var b = $('#alertInformation');
    RemoveAndAdd(a, b);
});
$('#load3').click(function () {
    var a = $(this);
    var b = $('#modfy');
    RemoveAndAdd(a, b);
});

function RemoveAndAdd(clickPostion,modfyPosition) {
    if (clickPostion.hasClass('fa-angle-up')) {
        clickPostion.removeClass('fa-angle-up').addClass('fa-angle-down');
        modfyPosition.show();
    } else {
        clickPostion.removeClass('fa-angle-down').addClass('fa-angle-up');
        modfyPosition.hide();
    }
}

function extendDatatableBySensorId() {
    $('#tableOneSensorInformation').dataTable({
        "iDisplayLength": -1,
        "sDom": 'T<"clear">rt',
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "aoColumnDefs": [{
            'bSortable': false,// 不排序
            'aTargets': [0, 1, 2, 3, 4, 5, 6, 7] // 不排序的列
        }]
    });
}

function extendDatatableByTime(a) {
    a.dataTable({
        "iDisplayLength": -1,
        "sDom": 'T<"clear">rt',
        "sPaginationType": "bootstrap",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },

        "aaSorting": [[1, "desc"]]
    });
}
/**end 传感器功能**/
