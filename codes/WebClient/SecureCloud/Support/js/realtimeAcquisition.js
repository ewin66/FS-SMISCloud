/**
 * ---------------------------------------------------------------------------------
 * <copyright file="realtimeAcquisition.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2015 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述："即时采集"子菜单页面js文件
 *
 * 创建标识：PengLing20140912
 *
 * 修改标识：PengLing20150401
 * 修改描述：1. 传感器即时采集任务下发失败细化, 界面显示采集异常对应描述.
 *           2. 增加"组织结构物"列表.
 * </summary>
 * ---------------------------------------------------------------------------------
 */
var g_listOrgStructs = {}; // 保存组织及组织下结构物列表
var g_listDtuSensors = {}; // 保存单个结构物下dtu和传感器列表
var g_sensorRealtimeStatus = {};
var g_sensorRealtimeData = {};
var g_sensorRealtimeSetIntervalId = {};

$(function () {
    $('#dataServices').addClass('active');
    $('#realtime-acquisition').addClass('active');

    var currentUserId = getCookie("userId");
    if (currentUserId == "") {
        alert("获取用户id失败, 请检查浏览器Cookie是否已启用");
        return;
    }
    getOrgStructsListByUser(currentUserId);

    bindChangeEvent();
    bindClickEvent();
});

/**
 * 获取用户下组织结构物列表
 */
function getOrgStructsListByUser(userId) {
    $('#loading-realtime').show();
    
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
                alertTips('该用户没有组织', 'label-important', 'tip-realtime', 3000);
            } else {
                showOrgStructsListByUser(data);
            }
            $('#loading-realtime').hide();
        },
        error: function (xhr) {
            $('#loading-realtime').hide();
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
    $('#listStruct').removeClass('chzn-done');
    $('#listStruct_chzn').remove();
    $('#listStruct').html(structOptions);
    // 筛选框,必须！
    $('#listStruct').chosen({
        no_results_text: "没有找到",
        allow_single_de: true
    });
    
    getDtuSensorsListByStruct(); // 获取单个结构物下DTU和传感器列表
}

/* 
 * 将函数绑定到DOM的change事件.
 */
function bindChangeEvent() {
    // 切换组织事件
    $('#listOrg').change(function () {
        undoDomGroupCheckable();
        // 显示当前组织下结构物列表
        showOrgStructs();
    });
    
    // 切换结构物事件
    $('#listStruct').change(function () {
        undoDomGroupCheckable();
        // 获取单个结构物下DTU和传感器列表
        getDtuSensorsListByStruct();
    });

    // 切换DTU事件
    $('#dtu-realtime').change(function () {
        undoDomGroupCheckable();
        // 显示当前dtu下的传感器表格
        showTableOfDtuSensors();
    });

    // 表格标题区 group-checkbox 多选事件
    jQuery('#realtimeAcqusition-table .group-checkable').change(function () {
        var set = jQuery(this).attr("data-set");
        var checked = jQuery(this).prop("checked"); // 不能用attr()，attr()方法只能在“jquery-1.8.3.min.js”才有效！
        jQuery(set).each(function () {
            if (checked) {
                var checkedNum = $('#realtimeAcqusition-tbody .checkboxes:checked').length; // up to 5.
                if (checkedNum >= 5) {
                    return;
                }
                $(this).prop("checked", true);
            } else {
                $(this).prop("checked", false);
            }
        });
        jQuery.uniform.update(set);
    });
}

/**
 * 取消 group-checkbox 选中状态
 */
function undoDomGroupCheckable() {
    var input = $('#realtimeAcqusition-table .group-checkable');
    if (input.prop('checked')) {
        input.prop('checked', false);
        jQuery.uniform.update(input);
    }
}

/* 
 * 将函数绑定到DOM的click事件.
 */
function bindClickEvent() {
    // "批量下发"点击事件
    $('#btnBatchSend').click(function () {
        var dtuId;
        var arrSensorId = [];
        $("input.checkboxes:checked").each(function () {
            var dtuSensor = this.value.split('-');
            dtuId = parseInt(dtuSensor[0]);
            var sensorId = parseInt(dtuSensor[1]);
            arrSensorId.push(sensorId);
        });
        if ($("input.checkboxes:checked").length == 0) {
            alert("请先选中传感器，再批量下发");
        } else {
            sendRealtimeRequest(dtuId, arrSensorId);
        }
    });

    // "下发"点击事件
    $('#realtimeAcqusition-tbody').on('click', 'a', function (e) {
        var tr = $(this).parents('tr');
        var selectedRow = tr[0];
        var $td = $(selectedRow.cells[0]);
        if ($td[0].firstChild.checked) { // if the current checkbox is checked, don't move on.
            return;
        }
        // up to 5.
        var checkedNum = $('#realtimeAcqusition-tbody .checkboxes:checked').length;
        if (checkedNum >= 5) {
            e.preventDefault(); // prevent to choose sensor.
            alertTips('一次最多操作5个传感器', 'label-important', 'tip-realtime', 2000);
            return;
        }
        // set the checkbox checked.
        $td[0].firstChild.checked = true;
    });
}

/**
 * 获取单个结构物下DTU和传感器列表
 */
function getDtuSensorsListByStruct() {
    $('#loading-realtime').show();
    
    var struct = $('#listStruct').find('option:selected')[0];
    var structId = parseInt(struct.id.split('optionStruct-')[1]);
    var url = apiurl + '/struct/' + structId + '/dtu-sensors/list/non-virtual' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false,
        success: function (data) {
            g_listDtuSensors = {}; // empty it

            if (data.length == 0) {
                alertTips('该结构物下无DTU', 'label-important', 'tip-realtime', 3000);
                $('#realtimeAcqusition-table').dataTable().fnDestroy(); // 必须！！！
                $('#realtimeAcqusition-tbody').html('');
                extendDatatable();
            } else {
                var sbDtu = new StringBuffer();
                $.each(data, function (index, item) {
                    sbDtu.append('<option id="optionDtu-' + item.dtuId + '">' + item.dtuNo + '</option>');
                    g_listDtuSensors["dtu-" + item.dtuId] = item; // assign value
                });
                $('#dtu-realtime').html(sbDtu.toString());
                // 显示当前dtu下的传感器表格
                showTableOfDtuSensors();
            }
            
            $('#loading-realtime').hide();
        },
        error: function (xhr) {
            $('#loading-realtime').hide();
            if (xhr.status == 403) {
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取结构物下的传感器列表时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

/**
 * 显示当前dtu下的传感器表格
 */
function showTableOfDtuSensors() {
    if (jQuery.isEmptyObject(g_listDtuSensors)) {
        alert('该结构物下可能没有DTU');
        return;
    }
    
    var dtu = $('#dtu-realtime').find('option:selected')[0];
    var dtuId = parseInt(dtu.id.split('-')[1]);
    var dtuSensors = g_listDtuSensors["dtu-" + dtuId];
    
    var dtuIdentification = dtuSensors.dtuIdentification;
    // 提示: 该DTU为传输型DTU, 因此不能对该DTU下任何传感器进行"即时采集"
    dtuIdentification.trim() == '实体型' ? $('#prompt-dtuTransfered').css('display', 'none') : $('#prompt-dtuTransfered').css('display', 'inline');

    if (dtuSensors.sensors.length == 0) {
        $('#btnBatchSend').attr('disabled', true).removeClass('blue').addClass('gray'); // 禁用"批量下发"按钮
    } else {
        $('#btnBatchSend').attr('disabled', false).removeClass('gray').addClass('blue'); // 启用"批量下发"按钮
    }

    var sb = new StringBuffer();
    $.each(dtuSensors.sensors, function (indexSensors, itemSensors) {
        sb.append('<tr>');
        sb.append('<td><input type="checkbox" class="checkboxes" value="' + dtuSensors.dtuId + '-' + itemSensors.sensorId + '" /></td>');
        sb.append('<td>' + itemSensors.location + '</td>');
        sb.append('<td>' + itemSensors.moduleNo + '</td>');
        sb.append('<td>' + itemSensors.channel + '</td>');
        if (dtuIdentification.trim() == "实体型") {
            sb.append('<td id="btnSend-' + itemSensors.sensorId + '"><a href="javascript: sendRealtimeRequest(' + dtuSensors.dtuId + ',' + itemSensors.sensorId + ');">' + '下发' + '</a></td>');
        } else {
            // 禁用所有"批量下发/下发"按钮
            $('#btnBatchSend').attr('disabled', true).removeClass('blue').addClass('gray');
            sb.append('<td id="btnSend-' + itemSensors.sensorId + '"><span style="color: gray;">下发</span></td>');
        }
        // 即时采集过程(包括剩余等待时间)及结果.
        sb.append('<td><span id="realtimeData-' + itemSensors.sensorId + '"></span>');
        sb.append('<span id="timeContainer-' + itemSensors.sensorId + '" style="display: none; margin-left: 10%;">剩余等待时间: <span id="time-' + itemSensors.sensorId + '" style="color: #ff0000"></span>秒</span></td>');
        sb.append('</tr>');
    });
    $('#realtimeAcqusition-table').dataTable().fnDestroy(); // 必须！！！
    $('#realtimeAcqusition-tbody').html(sb.toString());
    extendDatatable();
    
    // 表格内容区 checkbox 单选事件
    $('#realtimeAcqusition-tbody .checkboxes').unbind().click(function (e) {
        var checkedNum = $('#realtimeAcqusition-tbody .checkboxes:checked').length;
        if (checkedNum >= 6) {
            e.preventDefault(); // prevent to choose sensor.
            alertTips('一次最多操作5个传感器', 'label-important', 'tip-realtime', 2000);
            // alert(("checkedNum = " + checkedNum)); // can't work!
        }
    });
}

/**
 * 批量/单个下发传感器即时采集请求
 */
function sendRealtimeRequest(dtuId, oSensorId) {
    $('#modalConfirmRealtime').modal();
    $('#btnConfirmRealtimeSend').unbind("click").click(function () {
        $('#modalConfirmRealtime').modal('hide');
        var arrSensorId = [];
        var checkArray = isArray(oSensorId);
        if (!checkArray) {
            arrSensorId.push(oSensorId);
        } else {
            arrSensorId = oSensorId;
        }
        clearContext(arrSensorId); // 清理数据
        var sensorIds = arrSensorId.join(',');
        var url = apiurl + '/dtu/' + dtuId + '/sensor/' + sensorIds + '/realtime-request' + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'get',
            dataType: 'json',
            async: true,
            cache: false,
            success: function (data) {
                try {
                    onsuccessRealtimeRequest(data, dtuId, arrSensorId);
                } catch (err) {
                    alert(err);
                }
            },
            error: function (xhr) {
                if (xhr.status == 403) {
                    logOut();
                } else if (xhr.status == 405) { // 操作权限控制
                    alert('抱歉，没有下发权限');
                } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                    alert('获取传感器即时采集任务结果时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
                }
            }
        });
    });
}

/**
 * 判断数组类型
 */
function isArray(o) {
    return Object.prototype.toString.call(o) === '[object Array]';
}

/**
 * 清理待即时采集传感器的上下文.
 */
function clearContext(arrSensorId) {
    $.each(arrSensorId, function (i, sensorId) {
        g_sensorRealtimeStatus["sensor_" + sensorId] = null;
        g_sensorRealtimeData["sensor_" + sensorId] = null;
    });
}

/**
 * 启用所有"批量下发/下发"按钮.
 */
function enableAllSendButton() {
    var dtu = $('#dtu-realtime').find('option:selected')[0];
    var dtuId = parseInt(dtu.id.split('-')[1]);
    $.each(g_listDtuSensors["dtu-" + dtuId].sensors, function (indexSensors, itemSensors) {
        $('#btnSend-' + itemSensors.sensorId).html('<a href="javascript: sendRealtimeRequest(' + g_listDtuSensors["dtu-" + dtuId].dtuId + ',' + itemSensors.sensorId + ');">' + '下发' + '</a>'); // "下发"可用
    });
    $('#btnBatchSend').attr('disabled', false).removeClass('gray').addClass('blue');
}

/**
 * 禁用所有"批量下发/下发"按钮.
 */
function disableAllSendButton() {
    var dtu = $('#dtu-realtime').find('option:selected')[0];    
    var dtuId = parseInt(dtu.id.split('-')[1]);
    $.each(g_listDtuSensors["dtu-" + dtuId].sensors, function (indexSensors, itemSensors) {
        $('#btnSend-' + itemSensors.sensorId).html('<span style="color:gray;">下发</span>'); // "下发"不可用
    });
    $('#btnBatchSend').attr('disabled', true).removeClass('blue').addClass('gray');
}

function onsuccessRealtimeRequest(data, dtuId, arrSensorId) {
    var msgId = data.msgid;
    $.each(arrSensorId, function (i, item) {
        // $('#btnSend-' + item).html('<span style="color:gray;">下发</span>'); // "下发"不可用
        disableAllSendButton();
        $('#realtimeData-' + item).html('开始创建传感器即时采集任务，请等待...').css("color", "#008B45");
    });
    showRealtimeWaitingTime(arrSensorId);
    // query the database per 1s.
    var setTimeoutId = null;
    var setIntervalId = setInterval(function () {
        getRealtimeData(msgId, arrSensorId);
        $.each(arrSensorId, function (i, item) {
            // console.log('setInterval >>>>>>> g_sensorRealtimeStatus["sensor_' + item + '"] -> status = ' + g_sensorRealtimeStatus["sensor_" + item]);
            if (g_sensorRealtimeStatus["sensor_" + item] == "WAITING") {
                // console.log('WAITING >>>>>>> #timeContainer-' + item + ' -> display = ' + $('#timeContainer-' + item).css("display"));
                disableAllSendButton();
                $('#realtimeData-' + item).html('传感器即时采集任务创建中，请等待...').css("color", "#008B45");
            } else if (g_sensorRealtimeStatus["sensor_" + item] == "OK" || g_sensorRealtimeStatus["sensor_" + item] == "FAILED") {
                // console.log('OK >>>>>>> #timeContainer-' + item + ' -> display = ' + $('#timeContainer-' + item).css("display"));
                clearInterval(setIntervalId);
                $('#timeContainer-' + item).css("display", "none"); // hide waiting time.

                if (g_sensorRealtimeSetIntervalId["setInterval_" + item] != null) {
                    clearInterval(g_sensorRealtimeSetIntervalId["setInterval_" + item]);
                }
                if (setTimeoutId != null) {
                    clearTimeout(setTimeoutId);
                }
                // show sensor realtime result.
                handleRealtimeStatusSuccess(item);
                enableAllSendButton();
            }
        });
    }, 1000);
    // 25s*n timeout limit.
    var timeout = arrSensorId.length * 25000; // 25s needed for one sensor.
    setTimeoutId = setTimeout(function () {
        clearInterval(setIntervalId);
        $.each(arrSensorId, function (i, item) {
            $('#timeContainer-' + item).css("display", "none"); // hide waiting time.
            // console.log('setTimeout >>>>>>> #timeContainer-' + item + ' -> display = ' + $('#timeContainer-' + item).css("display"));
            // console.log('setTimeout >>>>>>> g_sensorRealtimeStatus["sensor_' + item + '"] -> status = ' + g_sensorRealtimeStatus["sensor_" + item]);
            switch (g_sensorRealtimeStatus["sensor_" + item]) {
                case "OK":
                case "FAILED":
                    handleRealtimeStatusSuccess(item);
                    // console.log('setTimeout >>>>>>> "OK" >>>> g_sensorRealtimeStatus["sensor_' + item + '"] -> status = ' + g_sensorRealtimeStatus["sensor_" + item]);
                    break;
                case "NULL":
                    $('#realtimeData-' + item).html('传感器即时采集任务创建失败！请重新下发').css("color", "#ff0000");
                    break;
                case "WAITING":
                    $('#realtimeData-' + item).html('传感器即时采集任务下发超时！请重新下发').css("color", "#ff0000");
                    break;
                default:
                    break;
            }
            // $('#btnSend-' + item).html('<a href="javascript: sendRealtimeRequest(' + dtuId + ',' + item + ');">' + '下发' + '</a>');
            enableAllSendButton();
        });
    }, timeout);
}

/**
 * 批量/单个传感器即时采集剩余等待时间
 */
function showRealtimeWaitingTime(arrSensorId) {
    var timeout = arrSensorId.length * 25; // 25s needed for one sensor.
    $.each(arrSensorId, function (i, item) {
        if (g_sensorRealtimeSetIntervalId["setInterval_" + item] != null) {
            clearInterval(g_sensorRealtimeSetIntervalId["setInterval_" + item]);
            g_sensorRealtimeSetIntervalId["setInterval_" + item] = null;
        }
        $('#timeContainer-' + item).css("display", "inline");
        // console.log('showRealtimeWaitingTime >>>>>>> #timeContainer-' + item + ' -> display = ' + $('#timeContainer-' + item).css("display"));
        $('#time-' + item).html(timeout); // init waiting time.
        // setInterval to each sensor.
        var timeleft = timeout - 1;
        g_sensorRealtimeSetIntervalId["setInterval_" + item] = setInterval(function () {
            $('#time-' + item).html(timeleft);
            if (timeleft-- == 0) {
                clearInterval(g_sensorRealtimeSetIntervalId["setInterval_" + item]);
            }
        }, 1000);
    });
}

function handleRealtimeStatusSuccess(sensorId) {
    var sensor = g_sensorRealtimeData["sensor_" + sensorId];
    if (sensor == null) {
        alert("异常: 当前传感器上下文已清空");
        return;
    }
    if (!sensor.check) {
        $('#realtimeData-' + sensor.sid).html('此传感器即时采集数据获取失败').css("color", "#ff0000");
        return;
    }
    if (sensor.time == null) {
        $('#realtimeData-' + sensor.sid).html(sensor.data + "采集时间：无");
    } else {
        $('#realtimeData-' + sensor.sid).html(sensor.data + ';&nbsp;&nbsp;&nbsp;&nbsp;采集时间: ' + JsonToDateTime(sensor.time));
    }
}

/**
 * 获取传感器即时采集数据
 * @param guid 系统生成的GUID值
 * @param sensorId 多个传感器id组成的字符串，各传感器id之间用“,”连接
 */
function getRealtimeData(msgId, arrSensorId) {
    var url = apiurl + '/messageId/' + msgId + '/sensor/realtime-data' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false,
        success: function (data) {
            if (data == null) {
                $.each(arrSensorId, function (i, itemSensorId) {
                    g_sensorRealtimeStatus["sensor_" + itemSensorId] = "NULL"; // 不存在数据
                });
                return;
            }
            try {
                $.each(arrSensorId, function (i, itemSensorId) {
                    if (data.status == null) {
                        g_sensorRealtimeStatus["sensor_" + itemSensorId] = "WAITING";
                    } else {
                        g_sensorRealtimeStatus["sensor_" + itemSensorId] = data.status; // "OK" 或者 "FAILED"
                        var inArray = false;
                        $.each(data.result, function (j, itemResult) {
                            if (itemResult.sensorId == itemSensorId) {
                                g_sensorRealtimeData["sensor_" + itemSensorId] = { sid: itemSensorId, data: itemResult.data, time: data.time, check: true };
                                inArray = true;
                                return false;
                            }
                        });
                        if (!inArray) {
                            g_sensorRealtimeData["sensor_" + itemSensorId] = { sid: itemSensorId, data: "N/A", time: "N/A", check: false };
                        }
                    }
                });
            } catch (err) {
                alert(err);
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取传感器即时采集数据时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

/**
 * 函数功能：渲染表格
 */
function extendDatatable() {
    $('#realtimeAcqusition-table').dataTable({
        "aLengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, "All"]
        ],
        // set the initial value
        "iDisplayLength": 50,
        "sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
        //"sPaginationType": "bootstrap",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        // "bStateSave": true,
        "aoColumnDefs": [{
            'bSortable': false, // 不排序
            'aTargets': [0, 4, 5]      // 不排序的列
        }],
        "aaSorting": [[1, "asc"]] // 第1列升序排序
    });
}
