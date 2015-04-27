/**
 * ---------------------------------------------------------------------------------
 * <copyright file="DataWarningTest.js" company="江苏飞尚安全监测咨询有限公司">
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
var g_sensorId = null;
var g_sensorLocation = null;
var g_startTime = null;
var g_endTime = null;

$(function () {
    $('#struct-warning').addClass('active'); // 设置"告警管理"菜单项为活动状态

    g_sensorId = document.getElementById('HiddenSensorId').value;
    g_sensorLocation = document.getElementById('HiddenSensorLocation').value;
    g_startTime = document.getElementById('HiddenStartTime').value;
    g_endTime = document.getElementById('HiddenEndTime').value;

    checkStructOrSensorAlert();
    
    bindBtnClickEvent();
    bindDropdownChangeEvent();
});

/* 
 * 检测待展示告警列表为结构物/传感器告警
 */
function checkStructOrSensorAlert() {
    if (g_sensorId != "" && g_startTime != "" && g_endTime != "") {
        createUserStructList(getCookie("nowStructId"));

        $('#btnUnprocessedAlert').parent().addClass("active");
        $('#btnAllAlert').parent().removeClass("active");
        $('#btnProcessedAlert').parent().removeClass("active");

        $('#btnUnprocessedAlert').attr({ "disabled": "disabled" });
        $("#btnAllAlert").removeAttr("disabled");
        $("#btnProcessedAlert").removeAttr("disabled");

        $("#btnBatchConfirm").removeAttr("disabled");

        $('#WarningPageTitle').html('传感器数据告警(' + g_sensorLocation + ')');
        GetWarning('unprocessed');
    } else {
        var locationUrl = decodeURI(location.href).split('.aspx?');
        var urlStructId;
        if (locationUrl.length == 1) { // the url ends with ".aspx".
            urlStructId = null;
        } else {
            var urlParams = locationUrl[1].split('&');
            urlStructId = urlParams[0].split('structId=')[1];
        }

        if (urlStructId != null && urlStructId != "") {
            setCookie('nowStructId', urlStructId);
        }
        createUserStructList(getCookie("nowStructId"));

        $('#btnUnprocessedAlert').attr({ "disabled": "disabled" });

        $("#btnBatchConfirm").removeAttr("disabled");

        GetWarning('unprocessed');
    }
}

/* 
 * 创建用户的结构物列表
 */
function createUserStructList(nowStructId) {
    var userId = getCookie('userId');
    if (userId === '' || userId === null) {
        alert('获取用户Id失败，请检查浏览器Cookie是否已启用');
        return;
    }
    var url = apiurl + '/user/' + userId + '/structs' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                return;
            }
            var sb = new StringBuffer();
            var flag = true;
            for (var i = 0; i < data.length; i++) {
                if (data[i].structId == parseInt(nowStructId)) {
                    $('.breadcrumb li small a').html(data[i].structName + '<i class="icon-angle-down"></i>');
                    if (i == 0) {
                        flag = false;
                    }
                } else {
                    if (i > 0 && flag) {
                        sb.append('<li class="divider"></li>');
                    }
                    flag = true;
                    sb.append('<li><a href="/DataWarningTest.aspx?structId=' + data[i].structId + '">' + data[i].structName + '</a></li>');
                }
            }
            $('.breadcrumb li small ul').html(sb.toString());
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取用户结构物时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

/* 
 * 将函数绑定到按钮的click事件.
 */
function bindBtnClickEvent() {
    $('#btnAllAlert').click(function () {
        $('#btnAllAlert').parent().addClass("active");
        $('#btnProcessedAlert').parent().removeClass("active");
        $('#btnUnprocessedAlert').parent().removeClass("active");

        $('#btnAllAlert').attr({ "disabled": "disabled" });
        $("#btnProcessedAlert").removeAttr("disabled");
        $("#btnUnprocessedAlert").removeAttr("disabled");

        $("#btnBatchConfirm").attr({ "disabled": "disabled" });

        GetWarning('all');
    });

    $("#btnProcessedAlert").click(function () {
        $('#btnProcessedAlert').parent().addClass("active");
        $('#btnAllAlert').parent().removeClass("active");
        $('#btnUnprocessedAlert').parent().removeClass("active");

        $('#btnProcessedAlert').attr({ "disabled": "disabled" });
        $("#btnAllAlert").removeAttr("disabled");
        $("#btnUnprocessedAlert").removeAttr("disabled");

        $("#btnBatchConfirm").attr({ "disabled": "disabled" });

        GetWarning('processed');
    });

    $('#btnUnprocessedAlert').click(function () {
        $('#btnUnprocessedAlert').parent().addClass("active");
        $('#btnAllAlert').parent().removeClass("active");
        $('#btnProcessedAlert').parent().removeClass("active");

        $('#btnUnprocessedAlert').attr({ "disabled": "disabled" });
        $("#btnAllAlert").removeAttr("disabled");
        $("#btnProcessedAlert").removeAttr("disabled");

        $("#btnBatchConfirm").removeAttr("disabled");

        GetWarning('unprocessed');
    });

    //获取全选的checkbox元素的value属性
    $('#btnBatchConfirm').click(function () {
        var array = new Array();
        $("input.checkboxes:checked").each(function () {
            array.push(this.value);
        });
        if (array.length == 0) {
            alertTip('请先选中告警再批量确认', 'label-important');
        } else {
            $('#warningText').val("");
            $('#myModal').fadeIn();
            $('#close').click(function () {
                $('#myModal').fadeOut();
            });
            $('#btnSubmitAlertConfirmationInfo').unbind('click').click(function () {
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

/*
 * 获取告警信息
 */
function GetWarning(sDealCondition) {
    $('#sample_1').dataTable().fnDestroy();
    
    var url;
    var urlCount;
    var urlStructOrSensor;
    if (g_sensorId != "" && g_startTime != "" && g_endTime != "") { // it is related to dataParse.js
        url = apiurl + '/sensor/' + g_sensorId + '/warnings/' + sDealCondition + '?token=' + getCookie("token");
        urlCount = apiurl + '/sensor/' + g_sensorId + '/warning-count/' + sDealCondition + '?token=' + getCookie("token");
        urlStructOrSensor = "sensor";
    }
    else {
        if (getCookie('nowStructId') == "") {
            alert("获取结构物id失败, 请检查浏览器Cookie是否已启用.");
            return;
        }
        url = apiurl + '/struct/' + getCookie('nowStructId') + '/warnings/' + sDealCondition + '?token=' + getCookie("token");
        urlCount = apiurl + '/struct/' + getCookie("nowStructId") + '/warning-count/' + sDealCondition + '?token=' + getCookie("token");
        urlStructOrSensor = "struct";
    }

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

                if (g_sensorId != "" && g_startTime != "" && g_endTime != "") {
                    $('#btnAllAlert').parent().addClass("active");
                    $('#btnProcessedAlert').parent().removeClass("active");
                    $('#btnUnprocessedAlert').parent().removeClass("active");
                    $('#btnAllAlert').attr({ "disabled": "disabled" });
                    $("#btnProcessedAlert").removeAttr("disabled");
                    $("#btnUnprocessedAlert").removeAttr("disabled");

                    GetWarning('all');
                } else {
                    $('#btnUnprocessedAlert').parent().addClass("active");
                    $('#btnAllAlert').parent().removeClass("active");
                    $('#btnProcessedAlert').parent().removeClass("active");
                    $('#btnUnprocessedAlert').attr({ "disabled": "disabled" });
                    $("#btnAllAlert").removeAttr("disabled");
                    $("#btnProcessedAlert").removeAttr("disabled");

                    GetWarning('unprocessed');
                }
                setTimeout(function () {
                    createWarningBadgeAndContent(getCookie('nowStructId'));
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
