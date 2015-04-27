/**
 * ---------------------------------------------------------------------------------
 * <copyright file="sectionHeapMapConfig.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2014 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述：施工截面非SVG图热点配置js文件
 *
 * 创建标识：PengLing201411
 *
 * 修改标识：PengLing20141227
 * 修改描述：从aspx文件中分离出js文件; 修改删除界面为弹出菜单.
 * </summary>
 * ---------------------------------------------------------------------------------
 */
var locationUrl = decodeURI(location.href);
var urlParams = locationUrl.split('.aspx?')[1].split('&');
var structId = urlParams[0].split('structId=')[1];
var sectionId = urlParams[1].split('sectionId=')[1];
var imageName = urlParams[2].split('imageName=')[1];

var g_hotspots = {};

$(function () {
    // 页面标题
    $('#titleSectionHeapMapSvg').text('截面热点图配置—' + getCookie('SectionStructName') + '—' + getCookie('CurrentSectionName'));
    //ajax请求读出数据
    //这些点分配的id需要存放起来
    $("#img").attr("src", "/resource/img/Topo/" + imageName);

    GetHotspotUnAdded();
    GetHotspotAdded();

    window.onresize = function () {
        for (var key in g_hotspots) {
            var hotspot = g_hotspots[key];
            $('#' + hotspot.hotspotId + 'hotspot').css({
                "left": hotspot.xAxis * $('#img').width() - 8,
                "top": hotspot.yAxis * $('#img').height() - 8
            });
        }
    };
});

/**  
* 获取未被添加为施工截面热点的传感器列表
*/
function GetHotspotUnAdded() {
    var url = apiurl + "/struct/" + structId + "/non-hotspot" + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                sb.append("<tr><td><img id='" + data[i].sensorId + "sensor' src='/resource/img/factorIcon/icon-"
                    + data[i].productTypeId + "-5.png' draggable='true' ondragstart='drag(event)' style='width: 16px; height: 16px;' /></td>");
                sb.append("<td>" + data[i].productName + "</td>");
                sb.append("<td>" + data[i].location + "</td></tr>");
            }
            $('#legend').html(sb.toString());
            extendDatatable();
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取未被添加为施工截面热点的传感器列表时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

/**  
* 获取已被添加为施工截面热点的传感器列表
*/
function GetHotspotAdded() {
    var url = apiurl + '/section/' + sectionId + '/hotspot-config' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                g_hotspots["hotspot_" + data[i].hotspotId] = { hotspotId: data[i].hotspotId, xAxis: data[i].xAxis, yAxis: data[i].yAxis, location: data[i].location };
                $('#imgContain').append("<img id='" + data[i].hotspotId + "hotspot' src='/resource/img/factorIcon/icon-"
                    + data[i].productTypeId + "-5.png' onclick='handlePopupMenu(event, " + data[i].hotspotId + ")' style='left: "
                    + (data[i].xAxis * $('#img').width() - 8) + "px; top: " + (data[i].yAxis * $('#img').height() - 8)
                    + "px; width: 16px; height:16px; position: absolute;' />");
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取已被添加为施工截面热点的传感器列表时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function drag(event) {
    event.dataTransfer.setData("Text", event.target.id);
}

function allowDrop(event) {
    event.preventDefault();
}

function drop(event) {
    event.preventDefault();
    var data = event.dataTransfer.getData("Text");

    var x = event.offsetX || event.layerX;
    var y = event.offsetY || event.layerY;
    var percentX = x / $('#img').width();
    var percentY = y / $('#img').height();

    //ajax把相对坐标、类型写入数据库
    addHotspot(parseInt(data), percentX, percentY);

    //window.location.reload();           
}

/**  
 * 添加施工截面的传感器热点
 */
function addHotspot(sensorId, x, y) {
    var url = apiurl + '/section/' + sectionId + '/hotspot-config/add' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'post',
        data: { sensorId: sensorId, xAxis: x, yAxis: y },
        success: function () {
            // alert("热点添加成功");
            window.location.reload();
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错");
                logOut();
            } else if (xhr.status == 405) { // aborted requests should be just ignored and no error message be displayed
                alert('抱歉，没有布点权限');
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("添加施工截面的传感器热点时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

/**
 * 处理"删除/关闭"弹出菜单
 */
function handlePopupMenu(e, hotspotId) {
    var hotspot = g_hotspots["hotspot_" + hotspotId];

    var x = e.offsetX;
    var y = e.offsetY;

    var pop = $("#theDropDownMenu");
    var pop1 = $(pop.children("li").children("a")[0]);
    var pop2 = $(pop.children("li").children("a")[1]);

    pop1.html("删除热点：" + hotspot.location + " ...").unbind().one("click", function () {
        // hide dropdown menu.
        $(this).parent().parent().hide();
        // popup deletion confirm dialog.
        popupDeletionConfirmDialog(hotspot.hotspotId);
    });
    pop2.html("关闭").unbind().one("click", function () {
        $(this).parent().parent().hide();
    });

    var imgContainer = $("#imgContain");
    var l = x + hotspot.xAxis * $('#img').width() - 8;
    var t = y + hotspot.yAxis * $('#img').height() - 8;
    pop.css("left", l).css("top", t);
    pop.show(200);
}

/**  
 * 弹出删除确认框
 */
function popupDeletionConfirmDialog(hotspotId) {
    $('#modalDeleteHotspot').modal();

    $('#deletionMsg').text('确认删除本热点吗?');

    $('#btnDeleteConfirm').unbind("click").click(function () {
        // delete it
        $('#modalDeleteHotspot').modal('hide');
        deleteHotspot(hotspotId);
    });
}

/**  
 * 删除施工截面的传感器热点
 */
function deleteHotspot(hotspotId) {
    var url = apiurl + '/section/hotspot-config/' + hotspotId + '/remove' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'post',
        success: function () {
            // alert("热点删除成功");
            window.location.reload();
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错");
                logOut();
            } else if (xhr.status == 405) { // aborted requests should be just ignored and no error message be displayed
                alert('抱歉，没有删除点权限');
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("删除施工截面的传感器热点时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

/**  
* 设置“返回”按钮链接
*/
function backSectionConfig() {
    location.href = 'SectionConfig.aspx?structId=' + structId + '&tabIndex=1';
}

/** 
 * 渲染表格样式
 */
function extendDatatable() {
    $('#tableLegend').dataTable({
        "bPaginate": false,
        "sDom": "ft",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        // "bStateSave": true,
        "aoColumnDefs": [{
            'bSortable': false, // 不排序
            'aTargets': [0] // 不排序的列
        }],
        "aaSorting": [[2, "asc"]] // 第1列升序排序
    });
}