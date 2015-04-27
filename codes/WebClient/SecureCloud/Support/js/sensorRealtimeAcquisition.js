/**
 * ---------------------------------------------------------------------------------
 * <copyright file="sensorRealtimeAcquisition.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2015 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述：技术支持项目仪表盘"传感器即时采集"js文件
 *
 * 创建标识：PengLing20150326
 *
 * 修改标识：
 * 修改描述：
 * </summary>
 * ---------------------------------------------------------------------------------
 */
var g_sensorRealtimeStatus = {};
var g_sensorRealtimeData = {};
var g_sensorRealtimeSetIntervalId = {};

function bindEventsOnBatchRealtimeAcqusition() {
    // 表格标题区 group-checkbox 多选事件
    jQuery('.tableBatchRealtimeAcqusition .group-checkable').change(function () {
        var dtuId = parseInt(this.id.split('groupCheckbox-')[1]);
        var set = jQuery(this).attr("data-set");
        var checked = jQuery(this).prop("checked"); // 不能用attr()，attr()方法只能在“jquery-1.8.3.min.js”才有效！
        jQuery(set).each(function () {
            if (checked) {
                var checkedNum = $('#tbodyBatchRealtimeAcqusition-' + dtuId + ' .checkboxes:checked').length; // up to 5.
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
    
    // "批量即时采集"点击事件
    $('.btnBatchSend').click(function () {
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
            sendSensorRealtimeRequest(dtuId, arrSensorId);
        }
    });

    // "即时采集"点击事件
    $('.tbodyBatchRealtimeAcqusition').on('click', 'a', function (e) {
        var tr = $(this).parents('tr');
        var selectedRow = tr[0];
        var dtuId = parseInt(selectedRow.id.split('-')[1]);
        var $td = $(selectedRow.cells[0]);
        if ($td[0].firstChild.checked) { // if the current checkbox is checked, don't move on.
            return;
        }
        // up to 5.
        var checkedNum = $('#tbodyBatchRealtimeAcqusition-' + dtuId + ' .checkboxes:checked').length;
        if (checkedNum >= 5) {
            e.preventDefault(); // prevent to choose sensor.
            alertTips('一次最多操作5个传感器', 'label-important', 'tip-realtime-' + dtuId, 3000);
            return;
        }
        // set the checkbox checked.
        $td[0].firstChild.checked = true;
    });
}

/**
 * 取消 group-checkbox 选中状态
 */
function undoDomGroupCheckable(dtuId) {
    var input = $('#tableBatchRealtimeAcqusition-' + dtuId + ' .group-checkable');
    if (input.prop('checked')) {
        input.prop('checked', false);
        jQuery.uniform.update(input);
    }
}

/**
 * 获取指定结构物的指定DTU下传感器状态
 */
function getSensorsStatusByStructAndDtu(dtuId) {
    var url = apiurl + '/struct/' + g_currentStructId + '/dtu/' + dtuId + '/sensors/status' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            if (data == null || data.sensors.length == 0) { // 该DTU不存在, 或者DTU下无传感器
                $('#btnBatchSend-' + dtuId).attr('disabled', true).removeClass('blue').addClass('gray'); // 禁用"批量即时采集"
                $('#tableBatchRealtimeAcqusition-' + dtuId).dataTable().fnDestroy();
                $('#tbodyBatchRealtimeAcqusition-' + dtuId).html('');
                extendDatatableOfDtuSensorsRealtime('#tableBatchRealtimeAcqusition-' + dtuId);
            } else {
                showSensorsStatusByStructAndDtu(data, dtuId);
            }
            $('#loading-projectDashboard').hide();
        },
        error: function (xhr) {
            $('#loading-projectDashboard').hide();
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取指定结构物的指定DTU下传感器状态时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

/**
 * 显示当前DTU下的传感器表格
 */
function showSensorsStatusByStructAndDtu(data, dtuId) {
    // 提示: 该DTU为传输型DTU, 因此不能对该DTU下任何传感器进行"即时采集"
    data.dtuIdentification.trim() == '实体型' ? $('#prompt-dtuSensors-dtuType').css('display', 'none') : $('#prompt-dtuSensors-dtuType').css('display', 'block');
    
    var sb = new StringBuffer();
    $.each(data.sensors, function (index, item) {
        sb.append('<tr id="tr-' + dtuId + '-' + item.sensorId + '">');
        sb.append('<td><input type="checkbox" class="checkboxes" value="' + dtuId + '-' + item.sensorId + '" /></td>');
        sb.append('<td>' + item.location + '</td>');
        sb.append('<td>' + item.module + '</td>');
        sb.append('<td>' + item.channel + '</td>');
        sb.append('<td>' + (item.dacStatus == null ? 'N/A' : item.dacStatus) + '</td>');
        // "即时采集"按钮,  当前DTU下传感器操作权限控制
        sb.append('<td id="btnSend-' + dtuId + '-' + item.sensorId + '" class="dtu-sensor">');
        if (data.dtuIdentification.trim() == '实体型' && data.dtuStatus) { // DTU为"实体型"且"在线"时, 可对DTU下传感器进行操作
            $('#btnBatchSend-' + dtuId).attr('disabled', false).removeClass('gray').addClass('blue'); // 使能"批量即时采集"
            sb.append('<a href="javascript: sendSensorRealtimeRequest(' + dtuId + ',' + item.sensorId + ');">' + '即时采集' + '</a></td>');
        } else {
            $('#btnBatchSend-' + dtuId).attr('disabled', true).removeClass('blue').addClass('gray'); // 禁用"批量即时采集"
            sb.append('<span style="color:gray;">即时采集</span></td>'); // 当前DTU下每个传感器"即时采集"不可用
        }
        // 即时采集过程(包括剩余等待时间)及结果.
        sb.append('<td><span id="realtimeData-' + item.sensorId + '"></span>');
        sb.append('<span id="timeContainer-' + item.sensorId + '" style="display: none; margin-left: 10%;">剩余等待时间: <span id="time-' + item.sensorId + '" style="color: #ff0000"></span>秒</span></td>');
        sb.append('</tr>');
    });
    $('#tableBatchRealtimeAcqusition-' + dtuId).dataTable().fnDestroy();
    $('#tbodyBatchRealtimeAcqusition-' + dtuId).html(sb.toString());
    extendDatatableOfDtuSensorsRealtime('#tableBatchRealtimeAcqusition-' + dtuId);

    // 表格内容区 checkbox 单选事件
    $('#tbodyBatchRealtimeAcqusition-' + dtuId + ' .checkboxes').unbind().click(function (e) {
        var checkedNum = $('#tbodyBatchRealtimeAcqusition-' + dtuId + ' .checkboxes:checked').length;
        if (checkedNum >= 6) {
            e.preventDefault(); // prevent to choose sensor.
            alertTips('一次最多操作5个传感器', 'label-important', 'tip-realtime-' + dtuId, 3000);
            // alert(("checkedNum = " + checkedNum)); // can't work!
        }
    });
}

/**
 * 批量/单个下发传感器即时采集请求
 */
function sendSensorRealtimeRequest(dtuId, oSensorId) {
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
                    onsuccessSensorRealtimeRequest(data, dtuId, arrSensorId);
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

function onsuccessSensorRealtimeRequest(data, dtuId, arrSensorId) {
    var msgId = data.msgid;
    $.each(arrSensorId, function (i, item) {
        disableAllButtonsToSendSensorRealtimeRequest(dtuId);
        $('#realtimeData-' + item).html('开始创建传感器即时采集任务，请等待...').css("color", "#008B45");
    });
    showRealtimeWaitingTime(arrSensorId);
    // query the database per 1s.
    var setTimeoutId = null;
    var setIntervalId = setInterval(function () {
        getSensorRealtimeData(msgId, arrSensorId);
        $.each(arrSensorId, function (i, item) {
            if (g_sensorRealtimeStatus["sensor_" + item] == "WAITING") {
                disableAllButtonsToSendSensorRealtimeRequest(dtuId);
                $('#realtimeData-' + item).html('传感器即时采集任务创建中，请等待...').css("color", "#008B45");
            } else if (g_sensorRealtimeStatus["sensor_" + item] == "OK" || g_sensorRealtimeStatus["sensor_" + item] == "FAILED") {
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
                enableAllButtonsToSendSensorRealtimeRequest(dtuId);
            }
        });
    }, 1000);
    // 25s*n timeout limit.
    var timeout = arrSensorId.length * 25000; // 25s needed for one sensor.
    setTimeoutId = setTimeout(function () {
        clearInterval(setIntervalId);
        $.each(arrSensorId, function (i, item) {
            $('#timeContainer-' + item).css("display", "none"); // hide waiting time.

            switch (g_sensorRealtimeStatus["sensor_" + item]) {
                case "OK":
                case "FAILED":
                    handleRealtimeStatusSuccess(item);
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
            
            enableAllButtonsToSendSensorRealtimeRequest(dtuId);
        });
    }, timeout);
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
 * 禁用所有"批量即时采集/即时采集"按钮.
 */
function disableAllButtonsToSendSensorRealtimeRequest(dtuId) {
    var $DtuSensors = $('td.dtu-sensor');
    $.each($DtuSensors, function (index, dom) {
        $(dom).html('<span style="color:gray;">即时采集</span>'); // 当前DTU下每个传感器"即时采集"不可用
    });
    $('#btnBatchSend-' + dtuId).attr('disabled', true).removeClass('blue').addClass('gray'); // 禁用"批量即时采集"
    disableDtuTab(dtuId);

    // 禁用"传感器最新信息"内容中单个传感器"即时采集"
    var $target = $('td.dtu-sensor-single');
    if ($target.length != 0) {
        var sensorId = parseInt($target[0].id.split('-')[2]);
        var tdRealtime = "<span style='color: gray;' class='RealtimeRequest'>即时采集</span> | <a href='javascript: GetSensorChartLastDataBySensorId(" +
            sensorId + ");'>最近24小时异常</a>";
        $target.html(tdRealtime);
    }
}

/**
 * 启用所有"批量即时采集/即时采集"按钮.
 */
function enableAllButtonsToSendSensorRealtimeRequest(dtuId) {
    var $DtuSensors = $('td.dtu-sensor');
    $.each($DtuSensors, function (index, dom) {
        var params = dom.id.split('-');
        var dtuId = parseInt(params[1]);
        var sensorId = parseInt(params[2]);
        $(dom).html('<a href="javascript: sendSensorRealtimeRequest(' + dtuId + ',' + sensorId + ');">' + '即时采集' + '</a>'); // 当前DTU下每个传感器"即时采集"可用
    });
    $('#btnBatchSend-' + dtuId).attr('disabled', false).removeClass('gray').addClass('blue'); // 使能"批量即时采集"
    enableDtuTab(dtuId);
    
    // 启用"传感器最新信息"内容中单个传感器"即时采集"
    var $target = $('td.dtu-sensor-single');
    if ($target.length != 0) {
        var currentSensorId = parseInt($target[0].id.split('-')[2]);
        var dtuSensor = g_currentDtuSensor["sensor-" + currentSensorId];
        var tdRealtime = "<a href='javascript: GetRealtimeRequest(" + dtuSensor.dtuId + ", " + dtuSensor.sensorId + "," +
            dtuSensor.dtuIdentify + "," + dtuSensor.dtuStatus + "," + dtuSensor.enable +
            ")' class='RealtimeRequest'>即时采集</a> | <a href='javascript: GetSensorChartLastDataBySensorId(" +
            dtuSensor.sensorId + ");'>最近24小时异常</a>";
        $target.html(tdRealtime);
    }
}

function disableDtuTab(dtuId) {
    $('#tabDtu-' + dtuId + ' a').css({ "color": "gray", "cursor": "text" });
    $('#tabDtu-' + dtuId + ' a').click(function () { // 禁用"DTU"tab页
        return false;
    });
}

function enableDtuTab(dtuId) {
    $('#tabDtu-' + dtuId + ' a').css({ "color": "#0d638f", "cursor": "pointer" });
    $('#tabDtu-' + dtuId + ' a').unbind('click'); // 使能"DTU"tab页
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
function getSensorRealtimeData(msgId, arrSensorId) {
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
function extendDatatableOfDtuSensorsRealtime(dom) {
    $(dom).dataTable({
        "aLengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, "All"]
        ],
        // set the initial value
        "iDisplayLength": 50,
        "sDom": "<'row-fluid'<'span5'l><'span7'f>r>t<'row-fluid'<'span7'i><'span5'p>>",
        //"sPaginationType": "bootstrap",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        // "bStateSave": true,
        "aoColumnDefs": [{
            'bSortable': false, // 不排序
            'aTargets': [0, 5, 6]      // 不排序的列
        }],
        "aaSorting": [[1, "asc"]] // 第1列升序排序
    });
}
