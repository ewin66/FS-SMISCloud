/**
 * ---------------------------------------------------------------------------------
 * <copyright file="userWarning.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2014 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述：告警查看js文件
 *
 * 创建标识：
 *
 * 修改标识：PengLing20141204
 * 修改描述：以服务器端"分页"的方式展示告警列表.
 * </summary>
 * ---------------------------------------------------------------------------------
 */
$(function() {
    $('#struct-warning').addClass('active'); // 设置"告警管理"菜单项为活动状态
    
    createWarningBadgeAndContentByUser(); // it is used for "orgin.Master".
    GetWarning('unprocessed');    
    
    bindBtnClickEvent();
    bindDropdownChangeEvent();
});

/* 
 * 将函数绑定到按钮的click事件.
 */
function bindBtnClickEvent() {
    $('#btnAllAlarm').click(function() {
        $('#btnAllAlarm').parent().addClass("active");
        $('#btnProcessedAlarm').parent().removeClass("active");
        $('#btnUnprocessedAlarm').parent().removeClass("active");

        $('#btnAllAlarm').attr({ "disabled": "disabled" });
        $("#btnProcessedAlarm").removeAttr("disabled");
        $("#btnUnprocessedAlarm").removeAttr("disabled");

        $("#btnBatchConfirm").attr({ "disabled": "disabled" });

        GetWarning('all');
    });

    $("#btnProcessedAlarm").click(function() {
        $('#btnProcessedAlarm').parent().addClass("active");
        $('#btnAllAlarm').parent().removeClass("active");
        $('#btnUnprocessedAlarm').parent().removeClass("active");

        $('#btnProcessedAlarm').attr({ "disabled": "disabled" });
        $("#btnAllAlarm").removeAttr("disabled");
        $("#btnUnprocessedAlarm").removeAttr("disabled");

        $("#btnBatchConfirm").attr({ "disabled": "disabled" });

        GetWarning('processed');
    });

    $('#btnUnprocessedAlarm').click(function() {
        $('#btnUnprocessedAlarm').parent().addClass("active");
        $('#btnAllAlarm').parent().removeClass("active");
        $('#btnProcessedAlarm').parent().removeClass("active");

        $('#btnUnprocessedAlarm').attr({ "disabled": "disabled" });
        $("#btnAllAlarm").removeAttr("disabled");
        $("#btnProcessedAlarm").removeAttr("disabled");

        $("#btnBatchConfirm").removeAttr("disabled");

        GetWarning('unprocessed');
    });

    //获取全选的checkbox元素的value属性
    $('#btnBatchConfirm').click(function() {
        var array = new Array();
        $("input.checkboxes:checked").each(function() {
            array.push(this.value);
        });
        if (array.length == 0) {
            alertTip('请先选中告警再批量确认', 'label-important');
        } else {
            $('#warningText').val("");
            $('#myModal').fadeIn();
            $('#close').click(function() {
                $('#myModal').fadeOut();
            });
            $('#btnSubmitAlertConfirmationInfo').unbind('click').click(function() {
                var t = $('#warningText');
                if ($.trim(t.val()) == "") {
                    alert("告警处理信息不能为空！");
                    t.focus();
                } else {
                    $('#myModal').fadeOut();
                    onconfirmAlert(array, getCookie("userId"), $('#warningText').val());
                }
            });
        }
    });
}

/* 
 * 将函数绑定到按钮的change事件.
 */
function bindDropdownChangeEvent() {
    jQuery('#sample_1 .group-checkable').change(function () {
        var set = jQuery(this).attr("data-set");
        var checked = jQuery(this).is(":checked");
        jQuery(set).each(function () {
            if (checked) {
                $(this).attr("checked", true);
            } else {
                $(this).attr("checked", false);
            }
        });
        jQuery.uniform.update(set);
    });
}

/**
 * 创建所有结构物告警徽章及告警信息
 */
function createWarningBadgeAndContentByUser() {
    var userId = getCookie('userId');
    if (userId === '' || userId === null) {
        alert('获取用户Id失败，请检查浏览器Cookie是否已启用');
        return;
    }
    var url = apiurl + '/user/' + userId + '/warning-count/unprocessed' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data == null || data.count == 0) {
                $('#accordion1').html('<span class="label label-important label-mini">无告警数据</span>');
                return;
            }
            var warningCount = data.count;
            $('.badge').html(warningCount);

            $('.notification').html('');
            $('.notification').append('<li><p>存在' + warningCount + '个未确认告警</p></li>');

            createWarningContentByUser(userId);
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取用户未确认告警数目时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

// 创建所有结构物告警信息
function createWarningContentByUser(userId) {
    var url = apiurl + '/user/' + userId + '/warnings/unprocessed' + '?token=' + getCookie("token") + '&startRow=1&endRow=15';
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data == null || data.length == 0) {
                return;
            }

            var len = data.length;
            var count = 0;
            var badgeSB = new StringBuffer();
            //告警列表
            var sb = new StringBuffer();
            for (var i = 0; i < len; i++) {
                sb.append('<div class="accordion-group"><div class="accordion-heading">');
                sb.append('<a class="accordion-toggle collapsed" data-toggle="collapse" data-parent="#accordion1" href="#collapse_' + i + '">');
                sb.append('<i class="icon-angle-left"></i><span>' + data[i].structName + '</span>');
                sb.append('<span style="position:absolute; right:6%;" onclick="showMoreWarnings(' + data[i].structId + ')">更多...</span></a></div>');

                if (i === 0) {
                    sb.append('<div id="collapse_' + i + '" class="accordion-body collapse in">');
                    sb.append('<div class="accordion-inner"><ul class="feeds">');
                } else {
                    sb.append('<div id="collapse_' + i + '" class="accordion-body collapse">');
                    sb.append('<div class="accordion-inner"><ul class="feeds">');
                }

                var warnings = data[i].warnings;
                if (warnings.length == 0) {
                    sb.append('<span class="label label-important label-mini">该结构物无告警数据</span>');
                }
                else {
                    for (var j = 0; j < warnings.length; j++) {
                        if (j >= 10) { // warning count is limited up to 10.
                            break;
                        }
                        sb.append('<li><a href="#"><div class="col1"><div class="cont"><div class="cont-col1"><div class="label label-important">');
                        sb.append('<i class="icon-bell"></i></div></div><div class="cont-col2"><div class="desc">' + warnings[j].source + warnings[j].content);
                        sb.append('<a href="/DataWarningTest.aspx?structId=' + data[i].structId + '"><span class="label label-important label-mini">处理<i class="icon-share-alt"></i></span></a></div></div></div>');
                        sb.append('</div><div class="col2"><div class="date">' + nowDateInterval(GetMilliseconds(warnings[j].time)) + '</div></div></a></li>');

                        if (count <= 5) {
                            var content = warnings[j].source + warnings[j].content;
                            if (content.length > 22) {
                                content = content.substring(0, 21);
                                content = content + '…';
                            }
                            badgeSB.append('<li><a href="/DataWarningTest.aspx" onclick="setnowStructId(' + data[i].structId + ')" ><span class="label label-info"><i class="icon-bell"></i></span>' + content + '&nbsp;&nbsp;&nbsp;&nbsp;<span class="time">' + nowDateInterval(GetMilliseconds(warnings[j].time)) + '</span></a></li>');
                        }
                        count++;
                    }
                }
                sb.append('</ul></div></div></div>');
            }
            $('#accordion1').html(sb.toString());

            $('.notification').append(badgeSB.toString());
            $('.notification').append('<li class="external"><a href="/DataWarningTest.aspx">更多<i class="m-icon-swapright"></i></a></li>');
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取用户未确认告警内容时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

/*
 * 获取告警信息
 */
function GetWarning(sDealCondition) {
    $('#sample_1').dataTable().fnDestroy();
    
    var userId = getCookie("userId");
    if (userId == "") {
        alert("获取用户id失败, 请检查浏览器Cookie是否已启用.");
        return;
    }
    var url = apiurl + '/user/' + userId + '/warnings/' + sDealCondition + '?token=' + getCookie("token");
    var urlCount = apiurl + '/user/' + userId + '/warning-count/' + sDealCondition + '?token=' + getCookie("token");
    var urlStructOrSensor = "struct";
    $('#sample_1').dataTable({
        "aLengthMenu": [
            [10, 25, 50],
            [10, 25, 50]
        ],
        // set the initial value
        "iDisplayLength": 25,
        "sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
        // "sPaginationType": "bootstrap",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        // "bStateSave": true,
        "aoColumns": [ //这个属性下的设置会应用到所有列，按顺序没有是空
            { "mData": 'warning_checkbox', "sWidth": "15px" },
            { "mData": 'warning_source', "sWidth": "12%" },
            { "mData": 'warning_level', "sWidth": "6%" },
            { "mData": 'warning_time', "sWidth": "12%" },
            { "mData": 'warning_typeID', "sWidth": "10%" },
            { "mData": 'warning_reason', "sWidth": "10%" },
            { "mData": 'warning_information', "sWidth": "10%" },
            { "mData": 'warning_dealFlag', "sWidth": "9%" },
            { "mData": 'warning_confirmInfo', "visible": false }
        ],
        "bSort": false,
        "bFilter": false, // 禁用搜索框
        //"aoColumnDefs": [{
        //    'bSortable': false,
        //    'aTargets': [0, 1, 4, 5, 6, 7]
        //}],
        //"aaSorting": [[2, "asc"]], // 第3列升序排序
        // 加上这个属性表格样式有问题
        "sPaginationType": "full_numbers",
        // "bLengthChange": false, // 禁用每页选择个数
        "bProcessing": true,
        "bServerSide": true,
        "sAjaxSource": "/datable.ashx?now=" + Math.random() + "&role=client&ss=" + urlStructOrSensor + "&Url=" + url + "&Url_count=" + urlCount
    });

    var table = $('#sample_1').DataTable();
    if (sDealCondition == "unprocessed") {
        table.column(8).visible(false);
    } else {
        table.column(8).visible(true);
    }

    $('#th_checkbox').removeClass('sorting_asc'); // remove the sorting_asc icon of checkbox.
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
    if (warningArray.length > 25) {
        alert("一次处理告警数不能超过25条，请重新选择");
        return;
    }
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
                $('#sample_1').dataTable().fnDestroy();

                $('#btnUnprocessedAlarm').parent().addClass("active");
                $('#btnAllAlarm').parent().removeClass("active");
                $('#btnProcessedAlarm').parent().removeClass("active");

                $('#btnUnprocessedAlarm').attr({ "disabled": "disabled" });
                $("#btnAllAlarm").removeAttr("disabled");
                $("#btnProcessedAlarm").removeAttr("disabled");

                GetWarning('unprocessed');

                setTimeout(function () {
                    createWarningBadgeAndContentByUser();
                }, 200);

                alertTip('确认成功', 'label-success');
            },
            400: function () {
                alertTip('确认失败', 'label-important');
            },
            500: function () {
                alertTip('处理出现异常', 'label-important');
            },
            403: function () {
                alert('权限验证出错');
                logOut();
            },
            404: function () {
                alertTip('确认失败', 'label-important');
            }
        }
    });
}

function alertTip(parameters, tipcolor) {
    var alerttext = $('.alert-tip-text');

    if (alerttext.text() == parameters)
        return;

    $('.alert-tip').slideToggle();
    $('.alert-tip-text').html(parameters);
    $('.alert-tip-text').remove('label-success').remove('label-important');
    $('.alert-tip-text').addClass(tipcolor);
    setTimeout(function () {
        $('.alert-tip').slideToggle();
        $('.alert-tip-text').html(' ');
    }, 3000);
}

function TimeFormat(jsonDate) {
    jsonDate = jsonDate.replace("/Date(", "").replace(")/", "");
    if (jsonDate.indexOf("+") > 0) {
        jsonDate = jsonDate.substring(0, jsonDate.indexOf("+"));
    }
    else if (jsonDate.indexOf("-") > 0) {
        jsonDate = jsonDate.substring(0, jsonDate.indexOf("-"));
    }
    var milliseconds = parseInt(jsonDate, 10);
    var date = new Date(milliseconds);
    //转换成标准的“月：MM”
    var normalizedMonth = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;

    var date_time = date.getFullYear() + "-" + normalizedMonth + "-" + normalizeTimeFormat(date.getDate())
                    + " " + normalizeTimeFormat(date.getHours()) + ":" + normalizeTimeFormat(date.getMinutes()) + ":" + normalizeTimeFormat(date.getSeconds());
    return date_time;
}

//标准化时间格式
function normalizeTimeFormat(time) {
    if (time < 10) {
        time = "0" + time;
    }
    return time;
}

