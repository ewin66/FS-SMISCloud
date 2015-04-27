/**
 * ---------------------------------------------------------------------------------
 * <copyright file="sectionHeapMapConfigSvg.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2014 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述：施工截面SVG图热点配置js文件
 *
 * 创建标识：PengLing20141103
 *
 * 修改标识：PengLing20150209
 * 修改描述：增加"批量删除"功能.
 * </summary>
 * ---------------------------------------------------------------------------------
 */
var g_structId = null;
var g_sectionId = null;
var g_svgIsLoaded = false;
var g_svgDoc;
var g_hotspots = {};
var g_sinfo = {}; // { type-sensor/section-id => { pid:productTypeId, pname:productName, l:location } }
var HOTSPOT_TYPE_SENSOR = 1;
var g_offsetWidth, g_offsetHeight;
var g_productSensors = {};
var g_hotspotsBatchSelected = {}; // it is used to save the hotspots which are red.

$(function () {
    var locationUrl = decodeURI(location.href);
    var urlParams = locationUrl.split('.aspx?')[1].split('&');
    g_structId = urlParams[0].split('structId=')[1];
    g_sectionId = urlParams[1].split('sectionId=')[1];
    var imageName = urlParams[2].split('imageName=')[1];
    // 页面标题
    $('#sectionHeapMapStructName').text(getCookie('SectionStructName') + '—' + getCookie('CurrentSectionName'));
    // dropdown菜单
    $("#theDropDownMenu").dropdown();
    var svgContainer = $("#svgContainer");
    $("#theDropDownMenu").detach().appendTo(svgContainer);
    // SVG图
    $('<embed src="/resource/img/Topo/' + imageName + '" id="svgEle" onload="onloadSvg();" type="image/svg+xml" />').appendTo('#svgContainer');
    // 获取未配置热点
    getUnconfiguredSensors();
    
    bindChangeEvent();
    bindClickEvent();

    addTooltipOfPath();
});

function addTooltipOfPath() {
    var tooltip = '<div>';
    tooltip += '<p>"path"为SVG图中用以描述曲线路径的节点, 常见命令有:</p>';
    tooltip += '<table border="1" width="100%">';
    tooltip += '<tr><th style="width: 20%">命令</th><th style="width: 40%;">含义</th><th style="width: 40%;">语法</th></tr>';
    tooltip += '<tr><td>M</td><td>moveto</td><td>M X,Y</td></tr>';
    tooltip += '<tr><td>L</td><td>lineto</td><td>L X,Y</td></tr>';
    tooltip += '<tr><td>H</td><td>horizontal lineto</td><td>H X</td></tr>';
    tooltip += '<tr><td>V</td><td>vertical lineto</td><td>V Y</td></tr>';
    tooltip += '<tr><td>C</td><td>curveto</td><td>C X1,Y1,X2,Y2,ENDX,ENDY</td></tr>';
    //tooltip += '<tr><td>S</td><td>smooth curveto</td><td>S X2,Y2,ENDX,ENDY</td></tr>';
    //tooltip += '<tr><td>Q</td><td>quadratic Belzier curve</td><td>Q X,Y,ENDX,ENDY</td></tr>';
    //tooltip += '<tr><td>T</td><td>smooth quadratic Belzier curveto</td><td>T ENDX,ENDY</td></tr>';
    //tooltip += '<tr><td>A</td><td>elliptical Arc</td><td>A RX,RY,XROTATION,FLAG1,FLAG2,X,Y</td></tr>';
    tooltip += '<tr><td>Z</td><td>closepath</td><td>Z</td></tr></table>';
    tooltip += '<p style="margin-bottom: 0;">注意: 上面所有的命令也可以表示成小写形式. 大写字母表示绝对位置, 小写字母表示相对位置.<br />' +
        '<span style="color: green;">例子: "M 0,0 100,100" 表示 "SVG图中起点(0,0)至终点(100,100)的一条线段".</span></p></div>';
    $('#labelPath').attr('title', tooltip);
    
    $('#labelPath').tooltip({
        html: true,
        placement: 'bottom'
    });
}

function bindChangeEvent() {
    // product type change event handler.
    $('#configProductType').change(onchangeProductType);
}

function bindClickEvent() {
    // 新建/编辑modal中"保存"点击事件.
    $('#btnConfigModalSave').click(onsaveHotspot);
    // handle the Close menu item.
    var pop = $("#theDropDownMenu");
    var pop3 = $(pop.children("li").children("a")[2]);
    pop3.text("关闭").bind("click", function () {
        $(this).parent().parent().hide();
    });
    // "选择全部" "取消选择" "删除" click event.
    $('#btnSelectAll').click(onselectAllHotspots);
    $('#btnCancelSelect').click(oncancelSelect);
    $('#btnBatchDelete').click(ondeleteHotspots);
}

/**
 * "选择全部"按钮点击方法.
 */
function onselectAllHotspots() {
    renderHotspotIcons('red');
    $('#btnCancelSelect').removeClass('gray').addClass('blue').removeAttr('disabled');
    $('#btnBatchDelete').removeClass('gray').addClass('red').removeAttr('disabled');
}

function renderHotspotIcons(color) {
    for (var key in g_hotspots) {
        var hotspot = g_hotspots[key];
        if (hotspot == null) {
            continue;
        }
        if (color == 'red') {
            hotspot.svg.href.baseVal = "/resource/img/factorIcon/icon-" + hotspot.productTypeId + "-1.png";
            g_hotspotsBatchSelected[key] = hotspot;
        } else {
            hotspot.svg.href.baseVal = "/resource/img/factorIcon/icon-" + hotspot.productTypeId + "-5.png";
        }
    }
}

/**
 * "取消选择"按钮点击方法.
 */
function oncancelSelect() {
    for (var key in g_hotspotsBatchSelected) {
        var hotspot = g_hotspotsBatchSelected[key];
        if (hotspot == null) {
            continue;
        }
        if (hotspot.sectionId != null) {
            hotspot.svg.href.baseVal = "/resource/img/icon-section-2.png";
        } else {
            hotspot.svg.href.baseVal = "/resource/img/factorIcon/icon-" + hotspot.productTypeId + "-5.png";
        }
        delete g_hotspotsBatchSelected[key];
    }
    $('#btnCancelSelect').removeClass('blue').addClass('gray').attr('disabled', 'disabled');
    $('#btnBatchDelete').removeClass('red').addClass('gray').attr('disabled', 'disabled');
}

function ondeleteHotspots() {
    popupDeletionConfirmDialog('batch');
}

/**
 * SVG加载方法
 */
function onloadSvg() {
    g_svgIsLoaded = true; // SVG已加载完成
}

window.onload = function () {
    if (g_svgIsLoaded) {
        showSvg();
    } else {
        setTimeout(function () {
            showSvg();
        }, 2000);
    }
};

function showSvg() {
    var svgEle = document.getElementById("svgEle");
    g_svgDoc = svgEle.getSVGDocument(); // 获得svg的document对象
    g_svgDoc.addEventListener('click', clickSvg, false); // 监听SVG点击事件
    g_svgDoc.addEventListener('contextmenu', popupContextMenuOnSvg, false); // 监听SVG弹出上下文菜单

    g_offsetWidth = svgEle.offsetWidth;
    g_offsetHeight = svgEle.offsetHeight;

    // 获取已配置热点
    getConfiguredSensors();
}

function clickSvg(event) {
    $("#theDropDownMenu").hide(); // close the popup menu.
    handleBatchSelect(event); // batch select.
    handleCancelSelect();
}

function popupContextMenuOnSvg(event) {
    event.preventDefault();
    handlePopupMenu(event);
}

/**
 * 获取未配置的传感器
 */
function getUnconfiguredSensors() {
    var url = apiurl + "/struct/" + g_structId + "/product/non-hotspot" + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            g_productSensors = {}; // 清空
            if (data.length == 0) {
                return;
            }
            $.each(data, function (i, productSensors) {
                g_productSensors["product_" + productSensors.productTypeId] = productSensors;
                $.each(productSensors.sensors, function (j, sensor) {
                    g_sinfo[HOTSPOT_TYPE_SENSOR + "_" + sensor.sensorId] = { pid: productSensors.productTypeId, pname: productSensors.productName, l: sensor.location };
                });
            });
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取未配置的传感器时发生异常.\r\n' + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

/**
 * 获取已配置的传感器热点信息
 */
function getConfiguredSensors() {
    $('#page-loader').show(); // show the loading marker.
    
    var url = apiurl + '/section/' + g_sectionId + '/hotspot-config' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data.length == 0) {
                $('#btnSelectAll').removeClass('blue').addClass('gray').attr('disabled', true); // disabled "选择全部"按钮.
            } else {
                drawHotspots(data);
                $('#btnSelectAll').removeClass('gray').addClass('blue').removeAttr('disabled'); // enable "选择全部"按钮.
            }
            $('#page-loader').hide(); // hide the loading marker.
        },
        error: function (xhr) {
            $('#page-loader').hide(); // hide the loading marker.
            if (xhr.status == 403) {
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取已配置的传感器热点信息时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function drawHotspots(data) {
    // var isIE = checkBrowser();
    for (var i = 0; i < data.length; i++) {
        createHotspotItem(data[i]);
        g_sinfo[HOTSPOT_TYPE_SENSOR + "_" + data[i].sensorId] = { pid: data[i].productTypeId, pname: data[i].productName, l: data[i].location };
    }
}

/**
 * SVG图上创建热点
 */
function createHotspotItem(hotspot, check) {
    var hotspotEleId = "hotspot_" + HOTSPOT_TYPE_SENSOR + "_" + hotspot.hotspotId;
    
    var checkExisted = check == null ? false : check;
    if (checkExisted) removeHotspotItem(hotspotEleId); // remove from SVG if existed.

    var svgImg = g_svgDoc.createElementNS('http://www.w3.org/2000/svg', 'image');
    svgImg.setAttribute('id', hotspotEleId);
    svgImg.setAttribute('width', '16');
    svgImg.setAttribute('height', '16');
    svgImg.setAttribute("hotspotId", hotspot.hotspotId);
    svgImg.href.baseVal = "/resource/img/factorIcon/icon-" + hotspot.productTypeId + "-5.png";
    svgImg.setAttribute('x', hotspot.xAxis * g_offsetWidth - 8);
    svgImg.setAttribute('y', hotspot.yAxis * g_offsetHeight - 8);
    g_svgDoc.documentElement.appendChild(svgImg);
    
    hotspot.svg = svgImg;
    g_hotspots[hotspotEleId] = hotspot; // 保存SVG图中热点信息
}

/**
 * SVG图上移除热点
 */
function removeHotspotItem(hotspotElmId) {
    var imageEle = g_svgDoc.getElementById(hotspotElmId);
    $(imageEle).remove();
}

function handleBatchSelect(e) {
    var source = e.srcElement;
    var key = source.id;
    if (source.localName == "image") { // 渲染红色图标.
        var hotspotIcon = source.href.baseVal;
        if (/(-5.png)$/.test(hotspotIcon)) {
            source.href.baseVal = hotspotIcon.slice(0, -6) + '-1.png'; // red icon.
            g_hotspotsBatchSelected[key] = g_hotspots[key];
        } else if (hotspotIcon.indexOf('-1.png') != -1) {
            source.href.baseVal = hotspotIcon.split('-1.png')[0] + '-5.png'; // green icon of sensor.
            if (g_hotspotsBatchSelected[key] != null) {
                delete g_hotspotsBatchSelected[key];
            }
        }
    }
}

/**
 * 点击SVG时, 启用/禁用"取消选择"按钮.
 */
function handleCancelSelect() {
    var flag = false;
    for (var key in g_hotspotsBatchSelected) {
        var hotspot = g_hotspotsBatchSelected[key];
        if (hotspot == null) {
            continue;
        }
        flag = true;
        break;
    }
    if (flag) {
        $('#btnCancelSelect').removeClass('gray').addClass('blue').removeAttr('disabled');
        $('#btnBatchDelete').removeClass('gray').addClass('red').removeAttr('disabled');
        
    } else {
        $('#btnCancelSelect').removeClass('blue').addClass('gray').attr('disabled', 'disabled');
        $('#btnBatchDelete').removeClass('red').addClass('gray').attr('disabled', 'disabled');
    }
}

function handlePopupMenu(e) {
    var x = e.offsetX;
    var y = e.offsetY;

    var pop = $("#theDropDownMenu");
    var pop1 = $(pop.children("li").children("a")[0]);
    var pop2 = $(pop.children("li").children("a")[1]);

    var source = e.srcElement;
    if (source.localName == "image") { // 编辑或删除
        var hotspotEleId = source.id;
        var hotspot = g_hotspots[hotspotEleId];
        var text = hotspot.location;
        $(pop.children("li").children("a")[0]).removeAttr("style"); // restore the popup.
        pop1.text("编辑热点：" + text + " ...").unbind().one("click", function () {
            $(this).parent().parent().hide();
            $('#titleConfigHotspot').text('编辑热点');
            showHotspotModal(x, y, hotspot);
        });
        pop2.show().text("删除本热点").unbind().one("click", function () {
            // hide dropdown menu.
            $(this).parent().parent().hide();
            // popup deletion confirm dialog.
            popupDeletionConfirmDialog('single', hotspotEleId);
        });
    } else { // 新建
        if (isEmptyObject(g_productSensors)) { // 禁用新建
            $(pop.children("li").children("a")[0]).html("此处新建热点...").attr("style", "color: gray; cursor: text;");
        } else {
            $(pop.children("li").children("a")[0]).removeAttr("style"); // restore the popup.
            $(pop.children("li").children("a")[0]).html("此处新建热点...").unbind().one("click", function () {
                $(this).parent().parent().hide();
                $('#titleConfigHotspot').text('新建热点');
                $("#configPath").val(''); // clear up path
                showHotspotModal(x, y, null);
            });
        }
        pop2.hide();
    }

    var svgContainer = $("#svgContainer");
    var l = x + svgContainer.offset().left;
    var t = y + svgContainer.offset().top;
    pop.css("left", l).css("top", t);
    pop.show(200);
}

/**
 * 显示“新建/编辑热点”对话框
 */
function showHotspotModal(x, y, oldHotspot) {
    var dlg = $("#modalConfigHotspot");
    dlg.data("oldData", oldHotspot);

    var path = "M " + x + "," + y; // initial path for new modal
    var sidToModify = null;
    // show Old value
    if (oldHotspot != null) {
        // 显示要编辑的热点信息.
        x = Math.round(oldHotspot.xAxis * g_offsetWidth);
        y = Math.round(oldHotspot.yAxis * g_offsetHeight);
        path = oldHotspot.svgPath == null ? ("M " + x + "," + y) : oldHotspot.svgPath;
        sidToModify = oldHotspot.sensorId;
    }
    $('#configAxis').val('X=' + x + '      Y=' + y);
    $("#configHotspotX").val(x);
    $("#configHotspotY").val(y);
    $("#configPath").val(path);

    showProductType(sidToModify);
    
    dlg.modal(); // show modal. this statement is vital!
}

/**  
 * 弹出删除确认框
 */
function popupDeletionConfirmDialog(type, hotspotEleId) {
    $('#modalDeleteHotspot').modal();
    var confirmContent = '确认删除本热点吗?';
    if (type == 'batch') {
        confirmContent = '确认删除这些热点吗?';
    }
    $('#deletionMsg').text(confirmContent);
    $('#btnDeleteConfirm').unbind("click").click(function () {
        // delete it
        $('#modalDeleteHotspot').modal('hide');
        if (type == 'single') {
            onremoveHotspot('single', hotspotEleId);
        } else {
            // delete multiple/all hotspots.
            onremoveHotspot('batch');
        }
    });
}

/**
 * 改变"产品类型"时，展示对应的"传感器位置"
 */
function onchangeProductType(event, targetObj, sensorIdToModify) {
    var productTypeId = $('#configProductType').val();
    var sb = new StringBuffer();
    // 编辑modal
    if (sensorIdToModify != null) {
        var ss = g_sinfo[HOTSPOT_TYPE_SENSOR + '_' + sensorIdToModify].l;
        sb.append('<option value="' + sensorIdToModify + '">' + ss + '</option>'); // 编辑modal中显示待编辑的传感器位置
    } else if (productTypeId == 'N/A') {
        sb.append('<option value="N/A">无可布点传感器<option>');
    }
    if (g_productSensors["product_" + productTypeId] != null) {
        var sensors = g_productSensors["product_" + productTypeId].sensors;
        $.each(sensors, function (j, sensor) {
            sb.append('<option value="' + sensor.sensorId + '">' + sensor.location + '</option>');
        });
    }
    // 刷新传感器位置列表,下面两行必须！
    $('#configSensor').removeClass('chzn-done');
    $('#configSensor_chzn').remove();
    $('#configSensor').html(sb.toString());
    // 筛选框,必须！
    $('#configSensor').chosen({
        no_results_text: "没有找到",
        allow_single_de: true
    });
}

/**
 * 显示产品类型列表
 */
function showProductType(sensorId) {
    var sbProductType = new StringBuffer();
    // 编辑modal
    if (sensorId != null) {
        var sensor = g_sinfo[HOTSPOT_TYPE_SENSOR + '_' + sensorId];
        sbProductType.append('<option value="' + sensor.pid + '">' + sensor.pname + '</option>');
    } else {
        if (isEmptyObject(g_productSensors)) {
            sbProductType.append('<option value="N/A">无可布点产品类型<option>');
        }
        // create the struct productType.
        $.each(g_productSensors, function (key, productSensors) {
            sbProductType.append('<option value="' + productSensors.productTypeId + '">' + productSensors.productName + '</option>');
        });
    }
    // show the struct productType.
    $('#configProductType').removeClass('chzn-done'); // these statements are vital.
    $('#configProductType_chzn').remove();
    $('#configProductType').html(sbProductType.toString()); // 产品类型
    $('#configProductType').chosen({
        no_results_text: "没有找到",
        allow_single_de: true
    });
    // show the product sensors.
    onchangeProductType(null, null, sensorId);
}

/**
 * 保存热点
 */
function onsaveHotspot() {
    var dlg = $("#modalConfigHotspot");
    var path = $('#configPath').val();
    //if (path == "") {
    //    alert('请填写"path"');
    //    return;
    //}
    var oldData = dlg.data("oldData");
    var isModifyHotspot = oldData != null;
    // 编辑?  新建?   
    var url = null;
    var dataToSave = {};
    var posx = parseInt($("#configHotspotX").val()) / g_offsetWidth;
    var posy = parseInt($("#configHotspotY").val()) / g_offsetHeight;
    // var hotspotType = $("#configHotspotType").val();
    var token = getCookie("token");
    var ssid = $("#configSensor").val(); // sensor's ID
    // Sensor
    dataToSave.sensorId = ssid;
    dataToSave.xAxis = posx;
    dataToSave.yAxis = posy;
    dataToSave.svgPath = path;
    if (isModifyHotspot) {
        if (oldData == null) {
            return;
        }
        var hotspotId = oldData.hotspotId;
        url = apiurl + '/hotspot-config/' + hotspotId + '/modify' + '?token=' + token; // sensor
    } else {
        url = apiurl + '/section/' + g_sectionId + '/hotspot-config/add' + '?token=' + token; // sensor
    }
    // 保存
    $.ajax({
        url: url,
        type: "post",
        dataType: "json",
        data: dataToSave,
        success: function (data) {
            handleHotspotSaved(data, dataToSave, isModifyHotspot);
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("保存数据时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
            $('#modalConfigHotspot').modal('hide');
        }
    });
}

function handleHotspotSaved(data, dataToSave, isModify) {
    // 保存成功, 将已保存对象传递至调用者.
    var jsonData = $.parseJSON(data);
    $("#btnConfigModalCancel").click(); // 保存成功后关闭热点配置对话框
    if (dataToSave != null) {
        var sensorInfo = null;
        dataToSave.hotspotId = jsonData.hotspotId;
        sensorInfo = g_sinfo[HOTSPOT_TYPE_SENSOR + "_" + dataToSave.sensorId];
        getUnconfiguredSensors();
        dataToSave.productTypeId = sensorInfo.pid; // vital, use to show the icon of sensor product type in SVG image
        dataToSave.location = sensorInfo.l; // vital, use to show sensor localtion in popup menu
        createHotspotItem(dataToSave, isModify);
        if (!isModify) { // enable "选择全部"按钮.
            $('#btnSelectAll').removeClass('gray').addClass('blue').removeAttr('disabled');
        }
    }
}

/**
 * 删除热点
 */
function onremoveHotspot(typeSingleOrBatch, hotspotElmId) {
    if (typeSingleOrBatch == 'batch') {
        removeBatchHotspot();
    } else {
        removeSingleHotspot(hotspotElmId);
    }
}

function removeBatchHotspot() {
    var arrHotspotSensor = [];
    for (var key in g_hotspotsBatchSelected) {
        var hotspot = g_hotspotsBatchSelected[key];
        if (hotspot == null) {
            continue;
        }
        arrHotspotSensor.push(hotspot.hotspotId);
    }
    var url;
    if (arrHotspotSensor.length > 0) {
        url = apiurl + '/hotspot-config/remove/' + arrHotspotSensor + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            success: function () {
                var arrImageEle = [];
                for (var i = 0; i < arrHotspotSensor.length; i++) {
                    var hotspotEleId = "hotspot_" + HOTSPOT_TYPE_SENSOR + "_" + arrHotspotSensor[i];
                    var imageEle = g_svgDoc.getElementById(hotspotEleId);
                    if (imageEle != null) {
                        arrImageEle.push($(imageEle));
                    }
                }
                if (arrImageEle.length > 0) {
                    handleHotspotRemoved(arrImageEle);
                } else {
                    alert('不存在待移除的热点(传感器).');
                }
            },
            error: function (xhr) {
                if (xhr.status == 403) {
                    logOut();
                } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                    alert('批量删除传感器热点时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
                }
            }
        });
    }
}

function removeSingleHotspot(hotspotElmId) {
    var imageEle = g_svgDoc.getElementById(hotspotElmId);
    if (imageEle == null) {
        return;
    }
    // remove it.
    var hotspotId = imageEle.getAttribute("hotspotId");
    var url = apiurl + '/section/hotspot-config/' + hotspotId + '/remove' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'post',
        dataType: 'json',
        success: function () {
            handleHotspotRemoved(new Array($(imageEle)));
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('删除传感器热点时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

/**
 * handle on the hotspots which have been removed successfully.
 */
function handleHotspotRemoved(hotspotObj) {
    getUnconfiguredSensors();
    for (var i = 0; i < hotspotObj.length; i++) {
        hotspotObj[i].remove();

        var key = hotspotObj[i][0].id;
        delete g_hotspots[key];
        delete g_hotspotsBatchSelected[key];
    }
    if (isEmptyObject(g_hotspotsBatchSelected)) {
        $('#btnBatchDelete').removeClass('red').addClass('gray').attr('disabled', 'disabled');
        $('#btnCancelSelect').removeClass('blue').addClass('gray').attr('disabled', 'disabled');
    }
    if (isEmptyObject(g_hotspots)) {
        $('#btnSelectAll').removeClass('blue').addClass('gray').attr('disabled', 'disabled');
    }
}

/**  
 * 设置“返回”按钮链接
 */
function backSectionConfig() {
    location.href = 'SectionConfig.aspx?structId=' + g_structId + '&tabIndex=1';
}

/**
 * 检查当前浏览器是否为IE浏览器
 */
function checkBrowser() {
    return navigator.appName == "Microsoft Internet Explorer";
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
