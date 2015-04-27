/**
 * ---------------------------------------------------------------------------------
 * <copyright file="dtuRemoteManage.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2015 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述：技术支持项目仪表盘"修改DTU远程配置"js文件
 *
 * 创建标识：PengLing20150326
 *
 * 修改标识：
 * 修改描述：
 * </summary>
 * ---------------------------------------------------------------------------------
 */
var g_dtuRestartStatus;
var g_dtuRemoteConfigStatus;
var g_dtuRemoteConfigResults = [];
var g_dtuRemoteConfigSetIntervalId;
var g_dtuRestartSetIntervalId;

/**
 * DTU配置"远程修改"界面
 */
function showModifyModeless(data, dtuNo) {
    clearRemoteConfigurationTips();

    var ip1 = (data.ip == null || data.ip.trim() == "") ? "无" : data.ip.trim();
    var port1 = (data.port == null) ? "无" : data.port;
    var ip2 = (data.ip2 == null || data.ip2.trim() == "") ? "无" : data.ip2.trim();
    var port2 = (data.port2 == null) ? "无" : data.port2;
    var dtuMode = (data.mode == null || data.mode.trim() == "") ? "无" : data.mode.trim();
    var packetInterval = (data.packetInterval == null) ? "无" : data.packetInterval;
    var reconnectionCount = (data.reconnectionCount == null) ? "无" : data.reconnectionCount;
    
    $('#modifyDtu').val(dtuNo);
    
    $('#modifyIp1').val(ip1);
    
    $('#modifyPort1').val(port1);
    
    if (ip2 != "无") {
        $('#modifyIp2').val(ip2);
    } else {
        $('#modifyIp2').val(ip1); // set the value of "ip1" to "ip2".
    }
    
    if (port2 != "无") {
        $('#modifyPort2').val(port2);
    } else {
        $('#modifyPort2').val(port1); // set the value of "port1" to "port2".
    }
    
    if (dtuMode != "无") {
        $("#modifyDtuMode").val(dtuMode);
    } else {
        $("#modifyDtuMode").val("TCP");
    }
    
    if (packetInterval != "无") {
        $('#modifyPacketInterval').val(packetInterval);
    } else {
        $('#modifyPacketInterval').val("200"); // set the default value of "200" ms.
    }
    
    if (reconnectionCount != "无") {
        $('#modifyReconnectionCount').val(reconnectionCount);
    } else {
        $('#modifyReconnectionCount').val("6"); // set the default value of "6" times.
    }
    
    validateRemoteConfiguration();
}

function emptyDtuRemoteConfigResults() {
    //$('#timeContainerDtuRemoteConfig').css("display", "none"); // hide waiting time.
    $('#promptDtuRemoteConfig').html('');

    $('#resultOfModifyIp1').html('');
    $('#resultOfModifyPort1').html('');
    $('#resultOfModifyIp2').html('');
    $('#resultOfModifyPort2').html('');
    $('#resultOfModifyDtuMode').html('');
    $('#resultOfModifyPacketInterval').html('');
    $('#resultOfModifyReconnectionCount').html('');
}

/**
 * DTU配置"远程修改"界面中"重置"点击方法
 */
$('#btnResetModifyDtuRemoteConfig').click(function () {
    clearRemoteConfigurationTips();
    
    $('#modifyIp1').val('');
    $('#modifyPort1').val('');
    $('#modifyIp2').val('');
    $('#modifyPort2').val('');
    $('#modifyDtuMode').val('TCP');
    $('#modifyPacketInterval').val('');
    $('#modifyReconnectionCount').val('');

    emptyDtuRemoteConfigResults();
});

/**
 * DTU配置"远程修改"界面中"下发"点击方法
 */
$('#btnSendModifyDtuRemoteConfig').click(function () {
    $('#modalConfirmDtuRemoteConfig').modal();
    $('#btnConfirmDtuRemoteConfigSend').unbind("click").click(function () {
        $('#modalConfirmDtuRemoteConfig').modal('hide');
        var configHaveNull = promptConfig();
        if (configHaveNull) {
            return;
        }
        var allConfigsIsValid = true;
        $('.modifyRange').each(function () {
            if ($(this).css("display") != "none") {
                allConfigsIsValid = false;
                return false;
            }
        });
        if (!allConfigsIsValid) {
            return;
        }
        var ip1 = $('#modifyIp1').val().trim();
        var port1 = $('#modifyPort1').val().trim();
        var ip2 = $('#modifyIp2').val().trim();
        var port2 = $('#modifyPort2').val().trim();
        var mode = $('#modifyDtuMode').val().trim().toUpperCase();
        var packetInterval = $('#modifyPacketInterval').val().trim();
        var reconnectionCount = $('#modifyReconnectionCount').val().trim();
        var dtuConfig = {
            "ip": ip1,
            "port": parseInt(port1),
            "ip2": ip2,
            "port2": parseInt(port2),
            "dtuMode": mode,
            "packetInterval": parseInt(packetInterval),
            "reconnectionCount": parseInt(reconnectionCount)
        };
        postModifyDtuRemoteConfigRequest(dtuConfig);
    });
});

/**
 * 清理待远程配置DTU的上下文.
 */
function clearDtuContext() {
    g_dtuRemoteConfigStatus = null; // clear it
    g_dtuRemoteConfigResults = []; // empty it
}

/**
 * 请求修改DTU远程配置
 * @param data DTU远程配置
 */
function postModifyDtuRemoteConfigRequest(dtuConfig) {
    clearDtuContext(); // 清理DTU远程配置上下文
    
    var url = apiurl + '/dtu/' + g_currentActiveDtuId + '/remote-config/modify-request' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'post',
        dataType: 'json',
        async: true,
        cache: false,
        data: dtuConfig,
        success: function (data) {
            try {
                onsuccessDtuModifyRemoteConfigRequest(data, dtuConfig);
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
                alert('请求修改DTU远程配置时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

function onsuccessDtuModifyRemoteConfigRequest(data, dtuConfig) {
    $('#btnClose').attr('disabled', true).removeClass('dark-gray').addClass('gray'); // "关闭"不可用
    $('#btnResetModifyDtuRemoteConfig').attr('disabled', true).removeClass('blue').addClass('gray'); // "重置"不可用
    $('#btnSendModifyDtuRemoteConfig').attr('disabled', true).removeClass('red').addClass('gray'); // "下发"不可用
    $('#promptDtuRemoteConfig').html('开始下发DTU远程配置修改任务，请等待...').css("color", "#008B45");
    showDtuRemoteConfigWaitingTime();
    // query the database per 1s.
    var setTimeoutId = null;
    var setIntervalId = setInterval(function () {
        if (g_dtuRemoteConfigStatus != "OK" && g_dtuRemoteConfigStatus != "FAILED") {
            updateDtuRemoteConfig(data.msgid, dtuConfig); // 更新DTU远程配置
        }
        
        if (g_dtuRemoteConfigStatus == "WAITING") {
            // console.log("WAITING >>>>>> #timeContainerDtuRemoteConfig >>>> display = " + $('#timeContainerDtuRemoteConfig').css("display"));
            $('#btnClose').attr('disabled', true).removeClass('dark-gray').addClass('gray'); // "关闭"不可用
            $('#btnResetModifyDtuRemoteConfig').attr('disabled', true).removeClass('blue').addClass('gray'); // "重置"不可用
            $('#btnSendModifyDtuRemoteConfig').attr('disabled', true).removeClass('red').addClass('gray'); // "下发"不可用
            $('#promptDtuRemoteConfig').html('DTU远程配置修改任务下发中，请等待...').css("color", "#008B45");
        } else if (g_dtuRemoteConfigStatus == "OK" || g_dtuRemoteConfigStatus == "FAILED") {
            clearInterval(setIntervalId);
            $('#timeContainerDtuRemoteConfig').css("display", "none"); // hide waiting time.
            // console.log("OK >>>>>> #timeContainerDtuRemoteConfig >>>> display = " + $('#timeContainerDtuRemoteConfig').css("display"));
            if (g_dtuRemoteConfigSetIntervalId != null) {
                clearInterval(g_dtuRemoteConfigSetIntervalId);
            }
            if (setTimeoutId != null) {
                clearTimeout(setTimeoutId);
            }
            
            showDtuRemoteConfigResults(); // show DTU remote config result.
            
            $('#btnClose').attr('disabled', false).removeClass('gray').addClass('dark-gray'); // "关闭"可用
            $('#btnResetModifyDtuRemoteConfig').attr('disabled', false).removeClass('gray').addClass('blue'); // "重置"可用
            $('#btnSendModifyDtuRemoteConfig').attr('disabled', false).removeClass('gray').addClass('red'); // "下发"可用
        }
    }, 1000);
    // 120s timeout limit.
    setTimeoutId = setTimeout(function () {
        clearInterval(setIntervalId);
        $('#timeContainerDtuRemoteConfig').css("display", "none"); // hide waiting time.
        // console.log("setTimeout >>>>>> #timeContainerDtuRemoteConfig >>>> display = " + $('#timeContainerDtuRemoteConfig').css("display"));
        
        showDtuRemoteConfigResults();
        
        $('#btnClose').attr('disabled', false).removeClass('gray').addClass('dark-gray'); // "关闭"可用
        $('#btnResetModifyDtuRemoteConfig').attr('disabled', false).removeClass('gray').addClass('blue'); // "重置"可用
        $('#btnSendModifyDtuRemoteConfig').attr('disabled', false).removeClass('gray').addClass('red'); // "下发"可用
    }, 120000); // 120000ms
}

function showDtuRemoteConfigResults() {
    $('#promptDtuRemoteConfig').html('');
    
    var len = g_dtuRemoteConfigResults.length;
    if (len == 0) {
        $('#resultOfModifyIp1').html('下发失败!').css("color", "#ff0000");
        $('#resultOfModifyPort1').html('下发失败!').css("color", "#ff0000");
        $('#resultOfModifyIp2').html('下发失败!').css("color", "#ff0000");
        $('#resultOfModifyPort2').html('下发失败!').css("color", "#ff0000");
        $('#resultOfModifyDtuMode').html('下发失败!').css("color", "#ff0000");
        $('#resultOfModifyPacketInterval').html('下发失败!').css("color", "#ff0000");
        $('#resultOfModifyReconnectionCount').html('下发失败!').css("color", "#ff0000");
        
        return;
    }
    for (var i = 0; i < len; i++) {
        var result = g_dtuRemoteConfigResults[i].result;
        switch (g_dtuRemoteConfigResults[i].cmd) {
            case "setIP1":
                if (result == "OK") {
                    $('#resultOfModifyIp1').html('下发成功.').css("color", "#008B45");
                } else {
                    $('#resultOfModifyIp1').html('下发失败!').css("color", "#ff0000");
                }
                break;
            case "setPort1":
                if (result == "OK") {
                    $('#resultOfModifyPort1').html('下发成功.').css("color", "#008B45");
                } else {
                    $('#resultOfModifyPort1').html('下发失败!').css("color", "#ff0000");
                }
                break;
            case "setIP2":
                if (result == "OK") {
                    $('#resultOfModifyIp2').html('下发成功.').css("color", "#008B45");
                } else {
                    $('#resultOfModifyIp2').html('下发失败!').css("color", "#ff0000");
                }
                break;
            case "setPort2":
                if (result == "OK") {
                    $('#resultOfModifyPort2').html('下发成功.').css("color", "#008B45");
                } else {
                    $('#resultOfModifyPort2').html('下发失败!').css("color", "#ff0000");
                }
                break;
            case "setMode":
                if (result == "OK") {
                    $('#resultOfModifyDtuMode').html('下发成功.').css("color", "#008B45");
                } else {
                    $('#resultOfModifyDtuMode').html('下发失败!').css("color", "#ff0000");
                }
                break;
            case "setByteInterval":
                if (result == "OK") {
                    $('#resultOfModifyPacketInterval').html('下发成功.').css("color", "#008B45");
                } else {
                    $('#resultOfModifyPacketInterval').html('下发失败!').css("color", "#ff0000");
                }
                break;
            case "setRetry":
                if (result == "OK") {
                    $('#resultOfModifyReconnectionCount').html('下发成功.').css("color", "#008B45");
                } else {
                    $('#resultOfModifyReconnectionCount').html('下发失败!').css("color", "#ff0000");
                }
                break;
            default:
                break;
        }
    }
}

/**
 * 修改DTU远程配置剩余等待时间
 */
function showDtuRemoteConfigWaitingTime() {
    if (g_dtuRemoteConfigSetIntervalId != null) {
        clearInterval(g_dtuRemoteConfigSetIntervalId);
        g_dtuRemoteConfigSetIntervalId = null;
    }
    $('#timeContainerDtuRemoteConfig').css("display", "inline");
    // console.log("showDtuRemoteConfigWaitingTime >>>>>> #timeContainerDtuRemoteConfig >>>> display = " + $('#timeContainerDtuRemoteConfig').css("display"));
    $('#timeDtuRemoteConfig').html("120"); // init waiting time.
    var timeleft = 119; // waiting time for 120s.
    g_dtuRemoteConfigSetIntervalId = setInterval(function () {
        $('#timeDtuRemoteConfig').html(timeleft);
        if (timeleft-- == 0) {
            clearInterval(g_dtuRemoteConfigSetIntervalId);
            // console.log("showDtuRemoteConfigWaitingTime >>>> timeleft == 0, so clearInterval");
        }
    }, 1000);
}

/**
 * 更新DTU远程配置
 * @param msgId 消息id
 * @param data DTU远程配置
 */
function updateDtuRemoteConfig(msgId, dtuConfig) {
    var url = apiurl + '/messageId/' + msgId + '/dtu/' + g_currentActiveDtuId + '/remote-config/modify' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'post',
        dataType: 'json',
        async: true,
        cache: false,
        data: dtuConfig,
        success: function (data) {
            if (data == null) {
                g_dtuRemoteConfigStatus = "NULL";
                return;
            }
            try {
                if (data.status == null) {
                    g_dtuRemoteConfigStatus = "WAITING";
                } else {
                    g_dtuRemoteConfigStatus = data.status; // "OK" 或者 "FAILED"
                    
                    if (data.result == null) {
                        alert('修改DTU远程配置时发生异常.\r\n可能原因: "[dbo].[T_TASK_INSTANT]"表中"[RESULT_JSON]"数据格式错误!');
                    } else {
                        g_dtuRemoteConfigResults = data.result.cmds; // assign value
                    }
                }
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
                alert('修改DTU远程配置时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

/**
 * 提示DTU配置
 */
function promptConfig() {
    if ($('#modifyIp1').val() == '' || $('#modifyPort1').val() == '' || $('#modifyIp2').val() == '' || $('#modifyPort2').val() == ''
        || $('#modifyPacketInterval').val() == '' || $('#modifyReconnectionCount').val() == '') {
        alert('下发失败，请检查DTU配置是否完整');
        return true;
    }
    return false;
}

/**
 * 校验远程配置的有效性
 */
function validateRemoteConfiguration() {
    $('#modifyIp1').focus(function () {
        document.getElementById('modifyIp1Range').style.display = 'none';
    });
    $('#modifyIp1').blur(function () {
        var ip1 = document.getElementById("modifyIp1").value;
        var regexp = new RegExp(/^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$/);
        if (!regexp.test(ip1) || ip1 == "") {
            document.getElementById('modifyIp1Range').style.display = 'block';
        }
    });
    $('#modifyIp2').focus(function () {
        document.getElementById('modifyIp2Range').style.display = 'none';
    }); 
    $('#modifyIp2').blur(function () {
        var ip2 = document.getElementById("modifyIp2").value;
        var regexp = RegExp(/^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$/);
        if (!regexp.test(ip2) || ip2 == "") {
            document.getElementById('modifyIp2Range').style.display = 'block';
        }
    });
    
    $('#modifyPort1').focus(function () {
        document.getElementById('modifyPort1Range').style.display = 'none';
    });
    $('#modifyPort1').blur(function () {
        var port1 = $('#modifyPort1').val();
        if (!/^[1-9]\d{3}|[1-5]\d{4}|6[0-5]{2}[0-3][0-5]$/.test(port1) || port1 == "") {
            document.getElementById('modifyPort1Range').style.display = 'block';
        }
    });
    
    $('#modifyPort2').focus(function () {
        document.getElementById('modifyPort2Range').style.display = 'none';
    });
    $('#modifyPort2').blur(function () {
        var port2 = $('#modifyPort2').val();
        if (!/^[1-9]\d{3}|[1-5]\d{4}|6[0-5]{2}[0-3][0-5]$/.test(port2) || port2 == "") {
            document.getElementById('modifyPort2Range').style.display = 'block';
        }
    });

    $('#modifyPacketInterval').focus(function () {
        document.getElementById('modifyPacketIntervalRange').style.display = 'none';
    });
    $('#modifyPacketInterval').blur(function () {
        var packetInterval = $('#modifyPacketInterval').val();
        if (!/^(\d)+$/.test(packetInterval) || packetInterval == "") {
            document.getElementById('modifyPacketIntervalRange').style.display = 'block';
        }
    });
    
    $('#modifyReconnectionCount').focus(function () {
        document.getElementById('modifyReconnectionCountRange').style.display = 'none';
    });
    $('#modifyReconnectionCount').blur(function () {
        var reconnectionCount = $('#modifyReconnectionCount').val();
        if (!/^(\d)+$/.test(reconnectionCount) || reconnectionCount == "") {
            document.getElementById('modifyReconnectionCountRange').style.display = 'block';
        }
    });
}

/**
 * 清除远程配置提示
 */
function clearRemoteConfigurationTips() {
    document.getElementById('modifyIp1Range').style.display = 'none';
    document.getElementById('modifyIp2Range').style.display = 'none';
    document.getElementById('modifyPort1Range').style.display = 'none';
    document.getElementById('modifyPort2Range').style.display = 'none';
    document.getElementById('modifyPacketIntervalRange').style.display = 'none';
    document.getElementById('modifyReconnectionCountRange').style.display = 'none';
}

/**
 * 请求重启DTU
 */
function restartDtu() {
    $('#modalConfirmDtuRestart').modal();
    $('#btnConfirmDtuRestart').unbind("click").click(function () {
        $('#modalConfirmDtuRestart').modal('hide');
        g_dtuRestartStatus = null; // clear context.
        var url = apiurl + '/dtu/' + g_currentActiveDtuId + '/restart-request' + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'get',
            dataType: 'json',
            async: true,
            cache: false,
            success: function (data) {
                try {
                    onsuccessDtuRestartRequest(data);
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
                    alert('获取DTU重启请求时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
                }
            }
        });
    });
}

function onsuccessDtuRestartRequest(data) {
    var msgId = data.msgid;
    $('#tdDtuRestart').html('<span style="color:gray;">重启</span>'); // "重启"不可用
    $('#promptDtuRestart').html("开始创建DTU重启任务，请等待...").css("color", "#008B45");
    showDtuRestartWaitingTime();
    // query the database per 1s.
    var setTimeoutId = null;
    var setIntervalId = setInterval(function () {
        getDtuRestartResult(msgId);
        if (g_dtuRestartStatus == "WAITING") {
            $('#tdDtuRestart').html('<span style="color:gray">重启</span>'); // "重启"不可用
            $('#promptDtuRestart').html("DTU重启任务创建中，请等待...").css("color", "#008B45");
        } else if (g_dtuRestartStatus == "OK" || g_dtuRestartStatus == "FAILED") {
            clearInterval(setIntervalId);
            $('#timeContainerDtuRestart').css("display", "none"); // hide waiting time.
            
            if (g_dtuRestartSetIntervalId != null) {
                clearInterval(g_dtuRestartSetIntervalId);
            }
            if (setTimeoutId != null) {
                clearTimeout(setTimeoutId);
            }
            // show DTU restart result.
            if (g_dtuRestartStatus == "OK") {
                $('#promptDtuRestart').html("DTU重启任务下发成功！").css("color", "#008B45");
            } else { // result is "FAILED".
                $('#promptDtuRestart').html('DTU重启任务下发失败！请再次重启').css("color", "#ff0000");
            }
            $('#tdDtuRestart').html('<a href="javascript:restartDtu();">重启</a>');
        }
    }, 1000);
    // 30s timeout limit.
    setTimeoutId = setTimeout(function () {
        clearInterval(setIntervalId);
        $('#timeContainerDtuRestart').css("display", "none"); // hide waiting time.
        switch (g_dtuRestartStatus) {
            case "NULL":
                $('#promptDtuRestart').html('DTU重启任务创建失败！请再次重启').css("color", "#ff0000");
                break;
            case "FAILED":
            case "WAITING":
                $('#promptDtuRestart').html('DTU重启任务下发失败！请再次重启').css("color", "#ff0000");
                break;
            default:
                break;
        }
        $('#tdDtuRestart').html('<a href="javascript:restartDtu();">重启</a>');
    }, 30000);
}

/**
 * DTU重启剩余等待时间
 */
function showDtuRestartWaitingTime() {
    if (g_dtuRestartSetIntervalId != null) {
        clearInterval(g_dtuRestartSetIntervalId);
        g_dtuRestartSetIntervalId = null;
    }
    $('#timeContainerDtuRestart').css("display", "inline");
    $('#timeDtuRestart').html("30"); // init waiting time.
    var timeleft = 29; // waiting time for 30s.
    g_dtuRestartSetIntervalId = setInterval(function () {
        $('#timeDtuRestart').html(timeleft);
        if (timeleft-- == 0) {
            clearInterval(g_dtuRestartSetIntervalId);
        }
    }, 1000);
}

/**
 * 获取DTU重启结果
 */
function getDtuRestartResult(msgId) {
    var url = apiurl + '/messageId/' + msgId + '/dtu/restart-result' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false,
        success: function (data) {
            if (data == null) {
                g_dtuRestartStatus = "NULL";
                return;
            }
            try {
                if (data.status == null) {
                    g_dtuRestartStatus = "WAITING";
                } else {
                    g_dtuRestartStatus = data.status; // "OK" 或者 "FAILED"
                }
            } catch (err) {
                alert(err);
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取DTU重启结果时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}
