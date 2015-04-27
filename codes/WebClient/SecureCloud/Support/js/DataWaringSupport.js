var g_listOrgStructs = {}; // 保存组织及组织下结构物列表

var FilteredDeviceType = "all";
var FilteredStatus = "unprocessed";
var FilteredStartTime = null;
var FilteredEndTime = null;
var FilteredLevel = [];
var OrderedDevice = "none";
var OrderedLevel = "up";
var OrderedTime = "none";
var g_param = {};

$(function () {
    $('#projectMonitoring').addClass('active'); // active"项目监控"菜单项.
    $('#DataWaringSupport').addClass('active'); // active"告警管理"子菜单项.

    alarmFilterClickEvent();
    initFliter();
    initPage();
    alarmSortClickEvent();
    bindChangeEvent();
    bindClickEvent();
});

/* 
 * 初始化告警页面，默认展示第一个结构物未确认告警.
 */
function initPage() {
    var currentUserId = getCookie("userId");
    if (currentUserId == "") {
        alert("获取用户编号失败, 请检查浏览器Cookie是否已启用");
        return;
    }
    initFilteredAndOrderedCondition();
    var linkedOrgStruct = getLinkedOrgAndStruct();
    getOrgStructsListByUser(currentUserId, linkedOrgStruct);
}

function initFilteredAndOrderedCondition() {
    FilteredStartTime = showDate(-1).toString();
    FilteredEndTime = showDate(0).toString();
    g_param = {
        "filteredDeviceType": FilteredDeviceType, // 过滤条件: 设备类型, 默认值: "all"
        "filteredStatus": FilteredStatus, // 过滤条件: 告警状态, 默认值: "unprocessed"
        "filteredLevel": "1,2,3,4", // 过滤条件: 告警等级数组, 支持同时查询多个告警等级, 默认值: 所有告警等级[1,2,3,4]
        "filteredStartTime": FilteredStartTime, // 过滤条件: 查询起始时间, 默认值: DateTime.Now.AddYears(-10)
        "filteredEndTime": FilteredEndTime, // 过滤条件: 查询结束时间, 默认值: DateTime.Now.AddYears(10)
        "orderedDevice": OrderedDevice, // 按照设置位置排序, 默认值: "none"
        "orderedLevel": OrderedLevel, // 按照告警等级排序, 默认值: "up"
        "orderedTime": OrderedTime // 按照告警产生时间排序, 默认值: "none"
    };
}

function getLinkedOrgAndStruct() {
    var linkedOrgStruct = {};
    if (location.href.split('=')[1] != null && location.href.split('=')[1] != "") {
        var paramOrgId = location.href.split('=')[1].split('&')[0];
        var paramStructId = location.href.split('=')[2];
        linkedOrgStruct = { orgId: paramOrgId, structId: paramStructId };

        setCookie('nowOrgId', paramOrgId);
        setCookie('nowStructId', paramStructId);
    }
    else { /*fix refresh start*/
        if (getCookie('nowOrgId') != "NaN" && getCookie('nowOrgId') != '' && getCookie('nowStructId') != "NaN" && getCookie('nowStructId') != '') {
            linkedOrgStruct = { orgId: getCookie('nowOrgId'), structId: getCookie('nowStructId') };
        } else {
            if (getCookie('nowOrgId') != "NaN" && getCookie('nowOrgId') != '') {
                linkedOrgStruct = { orgId: getCookie('nowOrgId'), structId: '' };
            } else if (getCookie('nowStructId') != "NaN" && getCookie('nowStructId') != '') {
                linkedOrgStruct = { orgId: '', structId: getCookie('nowStructId') };
            }
        }
    }  /*fix refresh end*/
    return linkedOrgStruct;
}

/**
 * 获取用户下组织结构物列表
 */
function getOrgStructsListByUser(userId, linkedOrgStruct) {
    $('#loading-alarm').show();
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
                showOrgStructsListByUser(data, linkedOrgStruct);
            }
            $('#loading-alarm').hide();
        },
        error: function (xhr) {
            $('#loading-alarm').hide();
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取用户下组织结构物列表时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function showOrgStructsListByUser(data, linkedOrgStruct) {
    var orgOptions = '';
    $.each(data.orgs, function (i, org) {
        orgOptions += '<option id="optionOrg-' + org.orgId + '" value="valOrg-' + org.orgId + '">' + org.orgName + '</option>';
        var arrStruct = [];
        $.each(org.structs, function (j, struct) {
            arrStruct.push({ structId: struct.structId, structName: struct.structName });
        });
        g_listOrgStructs["org-" + org.orgId] = arrStruct; // assign value
    });
    $('#listOrg').html(orgOptions);
    if (!jQuery.isEmptyObject(linkedOrgStruct) && linkedOrgStruct.orgId != "NaN" && linkedOrgStruct.orgId != '') { // 若通过链接进入告警页面, 则显示对应组织
        $('#listOrg').val("valOrg-" + linkedOrgStruct.orgId); // 设置当前选项为链接的组织
    }
    else { /*fix refresh start*/
        if (!jQuery.isEmptyObject(linkedOrgStruct) && getCookie('nowOrgId') != "NaN" && getCookie('nowOrgId') != '') {
            $('#listOrg').val("valOrg-" + getCookie('nowOrgId'));
        } else {
            var org = $('#listOrg').find('option:selected')[0];
            var orgId = parseInt(org.id.split('optionOrg-')[1]);
            $('#listOrg').val("valOrg-" + orgId);
            setCookie('nowOrgId', orgId);
        }
    } /*fix refresh end*/
    // 筛选框
    $('#listOrg').chosen({
        no_results_text: "没有找到",
        allow_single_de: true
    });

    showOrgStructs(linkedOrgStruct);
}

/**
 * 显示当前组织下的结构物列表
 */
function showOrgStructs(linkedOrgStruct) {
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
        structOptions += '<option id="optionStruct-' + struct.structId + '" value="valStruct-' + struct.structId + '">' + struct.structName + '</option>';
    });
    // 刷新结构物列表,下面两行必须！
    $('#listStruct').removeClass('chzn-done');
    $('#listStruct_chzn').remove();
    $('#listStruct').html(structOptions);
    /*fix refresh start*/
    if (!jQuery.isEmptyObject(linkedOrgStruct) && getCookie('nowOrgId') == orgId && linkedOrgStruct.structId != "NaN" && linkedOrgStruct.structId != '') { // 若通过链接进入告警页面, 则显示对应结构物
        $('#listStruct').val("valStruct-" + linkedOrgStruct.structId); // 设置当前选项为链接的结构物
    } else {
        if (getCookie('nowOrgId') == orgId && getCookie('nowStructId') != "NaN" && getCookie('nowStructId') != '') {
            $('#listStruct').val("valStruct-" + getCookie('nowStructId'));
        } else {
            setCookie('nowOrgId', orgId);
            var struct = $('#listStruct').find('option:selected')[0];
            var structId = parseInt(struct.id.split('optionStruct-')[1]);
            setCookie('nowStructId', structId);
        }
    }  /*fix refresh end*/
    // 筛选框,必须！
    $('#listStruct').chosen({
        no_results_text: "没有找到",
        allow_single_de: true
    });

    getUnprocessedWarnCount();
    getWarning();
}

/* 
 * 将函数绑定到DOM的change事件.
 */
function bindChangeEvent() {
    $('#listOrg').change(function () {
        initFliter();
        initSortMenuTitle();
        /*fix refresh start*/
        var org = $('#listOrg').find('option:selected')[0];
        var orgId = parseInt(org.id.split('optionOrg-')[1]);
        $('#listOrg').val("valOrg-" + orgId);
        setCookie('nowOrgId', orgId);
        setCookie('nowStructId', '');
        var linkedOrgStruct = { orgId: orgId, structId: '' };
        /*fix refresh end*/
        showOrgStructs(linkedOrgStruct);
    });

    $('#listStruct').change(function () {
        initFliter();
        initSortMenuTitle();
        /*fix refresh start*/
        var struct = $('#listStruct').find('option:selected')[0];
        var structId = parseInt(struct.id.split('optionStruct-')[1]);
        setCookie('nowStructId', structId);
        /*fix refresh end*/
        getUnprocessedWarnCount();
        getWarning();
    });
}

/* 
 * 将函数绑定到DOM的click事件.
 */
function bindClickEvent() {
    $('#btnDownload').click(function () {
        var struct = $('#listStruct').find('option:selected')[0];
        var structId = parseInt(struct.id.split('optionStruct-')[1]);
        var urlDownload = apiurl + '/struct/' + structId + '/filtered-ordered/alarms' + '?token=' + getCookie("token");
        var href = '/Support/WarnExcelDownload.ashx?Url=' + urlDownload + "&Url_params=" + JSON.stringify(g_param);
        window.open(href);
    });

    $('#filter_summit').click(function () {
        getFilteTime();
        getUnprocessedWarnCount();
        getWarning();
    });
}

function getFilteTime() {
    if (getTime("#dpfrom") != "" && getTime("#dpend") != "") {
        var flag = checkEndTime();
        if (!flag) {
            alert("告警起止时间点设置不合法, 终止时间点不宜小于起始时间点!");
            $("#dpend").focus();
            initFilteTime();
            return;
        }
    }
    if (getTime("#dpfrom") == "") {
        alert("请选择告警产生的起始时间!");
        $("#dpfrom").focus();
        initFilteTime();
        return;
    }
    if (getTime("#dpend") == "") {
        alert("请选择告警产生的终止时间!");
        $("#dpend").focus();
        initFilteTime();
        return;
    }
    FilteredStartTime = getTime('#dpfrom');
    FilteredEndTime = getTime('#dpend');
    setFilteTime(FilteredStartTime, FilteredEndTime);
}

function initFilteTime() {
    FilteredStartTime = showDate(-1).toString();
    FilteredEndTime = showDate(0).toString();
    setFilteTime(FilteredStartTime, FilteredEndTime);
}

function setFilteTime(start, end) {
    if ($("#selectTime").length > 0) {
        $("#selectTime a").html("告警产生时间：&nbsp;" + start + "至" + end);
    } else {
        var str = '<dd id="selectTime" class="selected"><a href="javascript:;">告警产生时间：&nbsp;' + start + '至' + end + '</a></dd>';
        $(".select-result dl").append(str);
    }
    $(".select-no").hide();
    g_param.filteredStartTime = start;
    g_param.filteredEndTime = end;
}

function initSortMenuTitle() {
    $("#sortDevice").text("告警源");
    $("#sortLevel").text("等级从高到低");
    $("#sortTime").text("告警产生时间");
}

function getUnprocessedWarnCount() {
    $('#loading-alarm').show();
    var struct = $('#listStruct').find('option:selected')[0];
    var structId = parseInt(struct.id.split('optionStruct-')[1]);
    var url = apiurl + '/struct/' + structId + '/warn-number/unprocessed/' + g_param.filteredStartTime + '/' + g_param.filteredEndTime + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        dataType: 'json',
        success: function (data, textStatus) {
            $('#loading-alarm').hide();
            initUnprocessedWarnCount();
            if (data == null) {
                return;
            }
            var warns = data[0].stats;
            for (var i = 0; i < warns.length; i++) {
                var level = parseInt(warns[i].level);
                if (level == 1) {
                    $('#warnlevel_1').html("一级(" + warns[i].number.toString() + ")");
                    break;
                }
            }
            for (var i = 0; i < warns.length; i++) {
                var level = parseInt(warns[i].level);
                if (level == 2) {
                    $('#warnlevel_2').html("二级(" + warns[i].number.toString() + ")");
                    break;
                }
            }
            for (var i = 0; i < warns.length; i++) {
                var level = parseInt(warns[i].level);
                if (level == 3) {
                    $('#warnlevel_3').html("三级(" + warns[i].number.toString() + ")");
                    break;
                }
            }
            for (var i = 0; i < warns.length; i++) {
                var level = parseInt(warns[i].level);
                if (level == 4) {
                    $('#warnlevel_4').html("四级(" + warns[i].number.toString() + ")");
                    break;
                }
            }
        },
        error: function (XMLHttpRequest) {
            $('#loading-alarm').hide();
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else if (XMLHttpRequest.status !== 0) {
                alert("获取结构物下未处理的告警各等级分布情况时发生异常.\r\n" + XMLHttpRequest.status + " : " + XMLHttpRequest.statusText);
            }
        }
    });
}

function initUnprocessedWarnCount() {
    $('#warnlevel_1').html("一级(0)");
    $('#warnlevel_2').html("二级(0)");
    $('#warnlevel_3').html("三级(0)");
    $('#warnlevel_4').html("四级(0)");
}

/*
 * 获取告警信息
 */
function getWarning() {
    $('#warnTable').dataTable().fnDestroy();
    var struct = $('#listStruct').find('option:selected')[0];
    var structId = parseInt(struct.id.split('optionStruct-')[1]);
    var url = apiurl + '/struct/' + structId + '/filtered-ordered/alarms' + '?token=' + getCookie("token");
    var count = apiurl + '/struct/' + structId + '/filtered-ordered/alarms-count' + '?token=' + getCookie("token");
    if (g_param.filteredLevel == "") {
        g_param.filteredLevel = "1,2,3,4";
    }
    setTableOption('warnTable', url, count, g_param);
}

function setTableOption(tableId, url, count, params) {
    $('#' + tableId).dataTable({
        "bAutoWidth": false,
        "aLengthMenu": [
             [10, 25, 50],
             [10, 25, 50]
        ],
        "iDisplayLength": 50,
        "bStateSave": false,
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "aoColumns": [//这个属性下的设置会应用到所有列，按顺序没有是空
            { "mData": 'warning_source' },
            { "mData": 'warning_level' },
            { "mData": 'warning_time' },
            { "mData": 'warning_reason' },
            { "mData": 'warning_information' },
            { "mData": 'warning_dealFlag' },
            { "mData": 'warning_confirmInfo' },
            { "mData": 'warning_send' }
        ],
        "bSort": false,
        "sPaginationType": "full_numbers",
        //"bLengthChange": false,
        "bFilter": false,
        "bProcessing": true,
        "bServerSide": true,
        "sAjaxSource": "/Support/AlarmSupport.ashx?now=" + Math.random() + "&role=support&ss=struct" + "&Url=" + url + "&Url_count=" + count + "&Url_params=" + JSON.stringify(params)
    });
}

/*
 * 确认告警
 */
function confirmAlert(warningId) {
    $('#warningText').val("");
    $('#myModal').fadeIn();
    $('#close').click(function () {
        $("#myModal").fadeOut();
    });
    $('#btnSubmitAlertConfirmationInfo').unbind('click').click(function () {
        var t = $('#warningText');
        var v = t.val();
        if ($.trim(v) == "") {
            alert('告警确认信息不能为空!');
            t.focus();
        } else {
            //根据id去更新数据库告警，更新成功后再将填写告警处理信息和确认告警的用户写回
            $('#myModal').fadeOut();
            onconfirmAlert(warningId, getCookie("userId"), $('#warningText').val());
        }
    });
}

function onconfirmAlert(warningArray, userId, msg) {
    var url = apiurl + '/warnings/confirm/' + warningArray + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'post',
        data: {
            "confirmor": userId,
            "suggestion": msg
        },
        statusCode: {
            202: function () {
                $('#warnTable').dataTable().fnDestroy();
                getUnprocessedWarnCount();
                getWarning();
                alert('确认成功');
            },
            400: function () {
                alert('下发失败');
            },
            500: function () {
                alert('处理出现异常');
            },
            403: function () {
                alert('权限验证出错');
                logOut();
            },
            404: function () {
                alert('url错误');
            },
            405: function () {
                alert('抱歉, 没有权限');
            }
        }
    });
}

/*
 * 下发告警
 */
function issueAlert(warningArray) {
    var isrun = confirm('是否下发?');
    if (isrun) {
        var url = apiurl + '/warnings/distribute/' + warningArray + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'post',
            statusCode: {
                202: function () {
                    $('#warnTable').dataTable().fnDestroy();
                    getUnprocessedWarnCount();
                    getWarning();
                    alert('下发成功');
                },
                400: function () {
                    alert('未找到告警');
                },
                500: function () {
                    alert('处理出现异常');
                },
                403: function () {
                    alert('权限验证出错');
                    logOut();
                },
                404: function () {
                    alert('url错误');
                },
                405: function () {
                    alert('抱歉, 没有权限');
                },
            }
        });
    }
}

/********************************************alarm filter start*************************************************/
function showDate(n) {
    var uom = new Date();
    uom.setDate(uom.getDate() + n);
    uom = uom.getFullYear() + "-" + (uom.getMonth() + 1) + "-" + uom.getDate() + " " + uom.getHours() + ":" + uom.getMinutes() + ":" + uom.getSeconds();
    return uom.replace(/\b(\w)\b/g, '0$1');
}

function initFliter() {
    $('#filter_level').click();
    $('#filter_device').click();
    $('#filter_unprocess').click();
    //init warn time 
    $("#dpfrom").val(showDate(-1).toString());
    $("#dpend").val(showDate(0).toString());
    getFilteTime();
}

function levelClickEvent(param1, param2, param3) {
    $("#filter_level").removeClass("selected");
    if ($("#selectA").length > 0) {
        $("#selectA").remove();
        FilteredLevel = [];
    }
    $("#" + param1).addClass("selected");
    var copyThisA = $("#" + param1).clone();
    if ($("#" + param2).length > 0) {
        $(param3).html("告警等级：&nbsp;" + $("#" + param1).text());
    } else {//不存在追加
        $(".select-result dl").append(copyThisA.attr("id", param2));
        $(param3).html("告警等级：&nbsp;" + $("#" + param1).text());
    }
    if (($("#selectA_1").length > 0) && ($("#selectA_2").length > 0) && ($("#selectA_3").length > 0) && ($("#selectA_4").length > 0)) {
        levelAll();
    }
}

function levelRemove(param1, param2) {
    $(param1).remove();
    $(param2).removeClass("selected");
    if (($("#selectA_1").length <= 0) && ($("#selectA_2").length <= 0) && ($("#selectA_3").length <= 0) && ($("#selectA_4").length <= 0)) {
        $("#filter_level").addClass("selected");
        var copyThisA = $("#filter_level").clone();
        if ($("#selectA").length > 0) {
            $("#selectA a").html("告警等级：&nbsp;" + $("#filter_level").text());
        } else {
            $(".select-result dl").append(copyThisA.attr("id", "selectA"));
            $("#selectA a").html("告警等级：&nbsp;" + $("#filter_level").text());
        }
        FilteredLevel = [];
        g_param.filteredLevel = "";
    }
}

function levelAll() {
    $("#filter_level").addClass("selected").siblings().removeClass("selected");
    for (var i = 1; i < 5; i++) {
        if ($("#selectA_" + i.toString()).length > 0) {
            $("#selectA_" + i.toString()).remove();
        }
    }
    var copyThisA = $("#filter_level").clone();
    if ($("#selectA").length > 0) {
        $("#selectA a").html("告警等级：&nbsp;" + $("#filter_level").text());
    } else {
        $(".select-result dl").append(copyThisA.attr("id", "selectA"));
        $("#selectA a").html("告警等级：&nbsp;" + $("#filter_level").text());
    }
    FilteredLevel = ["1", "2", "3", "4"];
    g_param.filteredLevel = "1,2,3,4";
}

function uniqueArrayItem(param) {
    FilteredLevel.push(param);
    FilteredLevel = FilteredLevel.filter(function (item, i, a) {
        return i == FilteredLevel.indexOf(item);
    });
    g_param.filteredLevel = FilteredLevel.join(',');
}

function alarmFilterClickEvent() {
    // warn level
    $("#filter_level").click(function () {
        levelAll();
    });

    $("#level_1").click(function () {
        levelClickEvent("level_1", "selectA_1", "#selectA_1 a");
        uniqueArrayItem("1");
    });
    $("#level_2").click(function () {
        levelClickEvent("level_2", "selectA_2", "#selectA_2 a");
        uniqueArrayItem("2");
    });
    $("#level_3").click(function () {
        levelClickEvent("level_3", "selectA_3", "#selectA_3 a");
        uniqueArrayItem("3");
    });
    $("#level_4").click(function () {
        levelClickEvent("level_4", "selectA_4", "#selectA_4 a");
        uniqueArrayItem("4");
    });
    // device type
    $("#select2 dd").click(function () {
        $(this).addClass("selected").siblings().removeClass("selected");
        if ($(this).hasClass("select-all")) {
            $("#selectB").remove();
        } else {
            var copyThisB = $(this).clone();
            if ($("#selectB").length > 0) {
                $("#selectB a").html("设备类型：&nbsp;" + $(this).text());
            } else {
                $(".select-result dl").append(copyThisB.attr("id", "selectB"));
                $("#selectB a").html("设备类型：&nbsp;" + $(this).text());
            }
        }
        if ($(this).text() == "传感器") {
            FilteredDeviceType = "sensor";
        } else if ($(this).text() == "DTU") {
            FilteredDeviceType = "dtu";
        } else {
            FilteredDeviceType = "all";
        }
        g_param.filteredDeviceType = FilteredDeviceType;
    });
    // status
    $("#select3 dd").click(function () {
        $(this).addClass("selected").siblings().removeClass("selected");
        if ($(this).hasClass("select-all")) {
            $("#selectC").remove();
        } else {
            var copyThisC = $(this).clone();
            if ($("#selectC").length > 0) {
                $("#selectC a").html("状态：&nbsp;" + $(this).text());
            } else {
                $(".select-result dl").append(copyThisC.attr("id", "selectC"));
                $("#selectC a").html("状态：&nbsp;" + $(this).text());
            }
        }
        if ($(this).text() == "全部") {
            FilteredStatus = "all";
        } else if ($(this).text() == "已确认") {
            FilteredStatus = "processed";
        } else if ($(this).text() == "已下发") {
            FilteredStatus = "issued";
        } else {
            FilteredStatus = "unprocessed";
        }
        g_param.filteredStatus = FilteredStatus;
    });

    $("#selectA").live("click", function () {
        $(this).remove();
        $("#filter_level").addClass("selected").siblings().removeClass("selected");
        FilteredLevel = [];
        g_param.filteredLevel = "";
    });
    $("#selectA_1").live("click", function () {
        levelRemove("#selectA_1", "#level_1");
        FilteredLevel.splice($.inArray("1", FilteredLevel), 1);
        g_param.filteredLevel = FilteredLevel.join(',');
    });
    $("#selectA_2").live("click", function () {
        levelRemove("#selectA_2", "#level_2");
        FilteredLevel.splice($.inArray("2", FilteredLevel), 1);
        g_param.filteredLevel = FilteredLevel.join(',');
    });
    $("#selectA_3").live("click", function () {
        levelRemove("#selectA_3", "#level_3");
        FilteredLevel.splice($.inArray("3", FilteredLevel), 1);
        g_param.filteredLevel = FilteredLevel.join(',');
    });
    $("#selectA_4").live("click", function () {
        levelRemove("#selectA_4", "#level_4");
        FilteredLevel.splice($.inArray("4", FilteredLevel), 1);
        g_param.filteredLevel = FilteredLevel.join(',');
    });

    $("#selectB").live("click", function () {
        $(this).remove();
        $("#filter_device").addClass("selected").siblings().removeClass("selected");
        FilteredDeviceType = "all";
        g_param.filteredDeviceType = FilteredDeviceType;
    });

    $("#selectC").live("click", function () {
        $(this).remove();
        $("#filter_unprocess").addClass("selected").siblings().removeClass("selected");
        FilteredStatus = "unprocessed";
        g_param.filteredStatus = FilteredStatus;
    });

    $(".select dd").live("click", function () {
        if ($(".select-result dd").length > 1) {
            $(".select-no").hide();
            $("#filter_summit").show();
        } else {
            $(".select-no").show();
            $("#filter_summit").hide();
        }
    });

    // warn time
    $('.date').datetimepicker({
        format: 'yyyy-MM-dd hh:mm:ss',
        language: 'pt-BR'
    });

    $("#selectTime").live("click", function () {
        $(this).remove();
        $("#dpfrom").val("");
        $("#dpend").val("");
        if ($(".select-result dd").length > 1) {
            $(".select-no").hide();
            $("#filter_summit").show();
        } else {
            $(".select-no").show();
            $("#filter_summit").hide();
        }
        FilteredStartTime = showDate(-1).toString();
        FilteredEndTime = showDate(0).toString();
        g_param.filteredStartTime = FilteredStartTime;
        g_param.filteredEndTime = FilteredEndTime;
    });
}

function getDateCompatibleWithIE(dateStr) {
    var str1 = dateStr.split('-');
    var year = str1[0];
    var month = str1[1];
    var str2 = str1[2].split(' ');// dd hh:mm:ss
    var day = str2[0];
    var str3 = str2[1].split(':');
    var hour = str3[0];
    var minute = str3[1];
    var second = str3[2];
    var date = new Date();
    date.setUTCFullYear(year, month - 1, day);
    date.setUTCHours(hour, minute, second);
    return date;
}


function checkEndTime() {
    var start = getTime('#dpfrom');
    var startTime = getDateCompatibleWithIE(start);
    var end = getTime('#dpend');
    var endTime = getDateCompatibleWithIE(end);
    if (endTime < startTime) {
        return false;
    }
    return true;
}
function getTime(param) {
    var date = $(param).val();
    date = date.replace(new RegExp("/", "g"), "-");
    return date;
}
/********************************************alarm filter end*************************************************/

/********************************************alarm sort start*************************************************/

function alarmSortClickEvent() {
    sortByDeviceClickEvent();
    sortByLevelClickEvent();
    sortByTimeClickEvent();
}

// sort by device
function sortByDeviceClickEvent() {
    $("#deviceDesc").live("click", function () {
        $("#sortDevice").text("告警源位置降序");
        deviceDesc();
    });

    $("#deviceAsc").live("click", function () {
        $("#sortDevice").text("告警源位置升序");
        deviceAsc();
    });
}

function sortByDevice(param) {
    $("#sortLevel").text("告警等级");
    $("#sortTime").text("告警产生时间");
    g_param.orderedDevice = param;
    g_param.orderedLevel = "none";
    g_param.orderedTime = "none";
    getWarning();
}

function deviceDesc() {
    sortByDevice("down");
}

function deviceAsc() {
    sortByDevice("up");
}

// sort by warn level
function sortByLevelClickEvent() {
    $("#levelDesc").live("click", function () {
        $("#sortLevel").text("等级从高到低");
        levelDesc();
    });
    $("#levelAsc").live("click", function () {
        $("#sortLevel").text("等级从低到高");
        levelAsc();
    });
}

function sortByLevel(param) {
    $("#sortTime").text("告警产生时间");
    $("#sortDevice").text("告警源");
    g_param.orderedLevel = param;
    g_param.orderedDevice = "none";
    g_param.orderedTime = "none";
    getWarning();
}

function levelDesc() {
    sortByLevel("up");
}

function levelAsc() {
    sortByLevel("down");
}

//sort by time
function sortByTimeClickEvent() {
    $("#timeDesc").live("click", function () {
        $("#sortTime").text("告警时间降序");
        timeDesc();
    });
    $("#timeAsc").live("click", function () {
        $("#sortTime").text("告警时间升序");
        timeAsc();
    });
}

function sortByTime(param) {
    $("#sortDevice").text("告警源");
    $("#sortLevel").text("告警等级");
    g_param.orderedTime = param;
    g_param.orderedLevel = "none";
    g_param.orderedDevice = "none";
    getWarning();
}
function timeDesc() {
    sortByTime("down");
}

function timeAsc() {
    sortByTime("up");
}
/********************************************alarm sort end*************************************************/