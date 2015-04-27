/**
 * ---------------------------------------------------------------------------------
 * <copyright file="batchExport.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2014 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述：无线采集批量导出单个模块号下所有传感器数据js文件
 *
 * 创建标识：PengLing20140911
 *
 * 修改标识：
 * 修改描述：
 * </summary>
 * ---------------------------------------------------------------------------------
 */

var listDtuModule = [];

$(function () {
    $('#batch-export').addClass('active');

    getStructSensors();
});

function getStructSensors() {
    var structId = getCookie("gprsStructId");
    var url = apiurl + '/struct/' + structId + '/non-virtual/sensors' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data.length == 0) {
                alertTips('该结构物下无传感器', 'label-important', 'tipNoSensor', 3000);
                return;
            }
            try {
                refactorModuleSensors(data);
                var sb = new StringBuffer();
                $.each(listDtuModule, function (index, item) {
                    sb.append('<option id="itemDtuModule-' + index + '" value="option-' + index + '">' + item.dtuNo + '-' + item.moduleNo + '</option>');
                });
                $('#dtu-module').html(sb.toString()); 
                parseSensorIdAndTimeInterval(); // 默认显示第一个DTU模块号的table数据              
            } catch (err) {
                alert(err);
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取结构物下的传感器列表时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function refactorModuleSensors(data) {
    var listSensor = [];
    $.each(data, function (index, item) {
        listSensor.push({
            dtuNo: item.dtuNo,
            moduleNo: item.moduleNo,
            sensorType: item.sensorType,
            sensorId: item.sensorId,
            location: item.location,
            flag: false // 比较标志
        });
    });
    for (var i = 0; i < listSensor.length; i++) {
        var tempSensors = [];
        if (listSensor[i].flag == false) {
            tempSensors.push({
                sensorId: listSensor[i].sensorId,
                location: listSensor[i].location
            });
        } else {
            continue;
        }
        // 当前对象和后面的对象做比较
        for (var j = i + 1; j < listSensor.length; j++) {
            if (listSensor[i].dtuNo == listSensor[j].dtuNo && listSensor[i].moduleNo == listSensor[j].moduleNo) {
                tempSensors.push({
                    sensorId: listSensor[j].sensorId,
                    location: listSensor[j].location
                });
                listSensor[j].flag = true;
            }
        }
        listDtuModule.push({
            dtuNo: listSensor[i].dtuNo,
            moduleNo: listSensor[i].moduleNo,
            sensors: tempSensors
        });
    }
}

$("#btnQuery").click(function () {
    parseSensorIdAndTimeInterval();
});

/**
 * 函数功能：根据已选的DTU模块号和时间解析接口访问需要的参数
 */
function parseSensorIdAndTimeInterval() {
    var arrSensorId = [];
    var arrSensorLocation = [];
    var dtuModule = $('#dtu-module').find('option:selected');
    for (var i = 0; i < dtuModule.length; i++) {
        var index = parseInt(dtuModule[i].id.split('-')[1]);
        $.each(listDtuModule[index].sensors, function (indexSensors, itemSensors) {
            arrSensorId.push(itemSensors.sensorId);
            arrSensorLocation.push(itemSensors.location);
        });
    }

    var arrTime = $('#date').find('option:selected');
    var timeInterval = arrTime[0].value;

    if (arrSensorId.length == 0) {
        alert("请选择传感器");
    } else {
        constructUrlParam(arrSensorId, timeInterval); 
    }
}

/**
 * 函数功能：构造接口调用参数
 */
function constructUrlParam(arrSensorId, timeInterval) {
    var startTime, endTime;
    switch (timeInterval) {
        case "day":
            startTime = convertMillisecondsToDateTime_milliseconds(new Date() - 24 * 60 * 60 * 1000);
            endTime = convertMillisecondsToDateTime_milliseconds(new Date().valueOf());
            break;
        case "week":
            startTime = convertMillisecondsToDateTime_milliseconds(new Date() - 7 * 24 * 60 * 60 * 1000);
            endTime = convertMillisecondsToDateTime_milliseconds(new Date().valueOf());
            break;
        case "month":
            startTime = convertMillisecondsToDateTime_milliseconds(new Date() - 30 * 24 * 60 * 60 * 1000);
            endTime = convertMillisecondsToDateTime_milliseconds(new Date().valueOf());
            break;
        case "other":
            startTime = getStartdate();
            endTime = getEnddate();
            break;
        default:
            break;
    }

    var sensorIds = "";
    if (arrSensorId.length == 1) {
        sensorIds = arrSensorId[0];
    } else {
        for (var i = 0; i < arrSensorId.length; i++) {
            if (i == 0) {
                sensorIds = arrSensorId[i];
            } else {
                sensorIds += ',' + arrSensorId[i];
            }
        }
    }

    getSensorData(sensorIds, startTime, endTime);
}

function getSensorData(sensorIds, startTime, endTime) {
    var url = apiurl + '/gprs/sensor/' + sensorIds + '/data/' + startTime + '/' + endTime + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data.length == 0) {
                $('#table-sensor').hide();
                $('#tipNoData-sensor').show();
                return;
            }
            try {
                $('#tipNoData-sensor').hide();
                $('#table-sensor').show();
                    
                var arrayTableData = []; // 存放表格数据

                $.each(data, function (index, item) {
                    var array = [];
                    for (var j = 0; j < item.data.length; j++) {
                        var time = item.data[j].acquisitiontime.substring(6, 19);
                        // 填充table数据
                        var arrayCurrentData = [];
                        arrayCurrentData.push(item.location);
                        for (var k = 0; k < item.data[j].originalValue.length; k++) {
                            arrayCurrentData.push(item.data[j].originalValue[k]);
                        }
                        // 物理量值
                        for (var k = 0; k < item.data[j].calculatedValue.length; k++) {
                            if (item.columns.calculatedColumn[k] == "X方向累积位移") { // 不显示“X方向累积位移，Y方向累积位移”
                                break;
                            }
                            arrayCurrentData.push(item.data[j].calculatedValue[k]);
                        }
                        arrayCurrentData.push(time);
                        arrayTableData.push(arrayCurrentData);
                    }
                });

                // 表头
                var arrayTableHead = [];
                arrayTableHead.push('设备位置');
                // 原始值对应的列名
                for (var i = 0; i < data[0].columns.originalColumn.length; i++) {
                    arrayTableHead.push(data[0].columns.originalColumn[i] + "(" + data[0].units.originalUnit[i] + ")");
                }
                // 计算后值对应的列名
                if (data[0].columns.calculatedColumn.length == 1) {
                    arrayTableHead.push('物理量值(' + data[0].units.calculatedUnit[0] + ')');
                } else {
                    for (var i = 0; i < data[0].columns.calculatedColumn.length; i++) {
                        if (data[0].columns.calculatedColumn[i] == "X方向累积位移") { // 不显示“X方向累积位移，Y方向累积位移”
                            break;
                        }
                        arrayTableHead.push(data[0].columns.calculatedColumn[i] + "(" + data[0].units.calculatedUnit[i] + ")");
                    }
                }
                // 时间列
                arrayTableHead.push('采集时间');
                var timeIndex = arrayTableHead.length - 1; // 采集时间序列号
                // 生成数据表格
                tableManager('table-sensor', arrayTableData, arrayTableHead, timeIndex);
            } catch (err) {
                alert(err);
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取传感器数据时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}