/**
 * -----------------------------------------------------------------------------------------------------------------
 * <copyright file="sectionConfig.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2015 飞尚科技
 * 版权所有。
 * </copyright>
 * -----------------------------------------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述：
 *
 * 创建标识：PengLing20141021
 *
 * 修改标识：PengLing20150415
 * 修改描述：增加"快速配置"功能
 *
 * 修改标识：
 * 修改描述：
 * </summary>
 * -----------------------------------------------------------------------------------------------------------------
 */
var g_structId = null;
var g_sectionHeapMapName = null;
var g_sectionsConfigured = {};
var g_currentSection = null;
var g_unconfiguredSensors = {};
var g_expressConfiguredHotspots = [];

$(function () {
    document.getElementById('sectionStructName').innerText = getCookie('SectionStructName');
    document.getElementById('addSectionModalTitle').innerText = getCookie('SectionStructName');

    var locationUrl = decodeURI(location.href);
    var urlParams = locationUrl.split('.aspx?')[1].split('&');
    var urlStructId = urlParams[0].split('structId=')[1];
    var urlTabIndex = urlParams[1].split('tabIndex=')[1];

    g_structId = urlStructId; // assign value
    
    showTab(urlTabIndex);
    showSectionConfig(false);
    
    bindClickEvent();
    bindChangeEvent();
});

function showTab(tabIndex) {
    if (tabIndex == 1) {
        $('#tabSectionConfig a').trigger('click');
    }
}

function showSectionConfig(isUpdated) {
    g_sectionsConfigured = {}; // clear up it
    
    var url = apiurl + '/struct/' + g_structId + '/sections' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false, // 必须加token,该属性才能通过代理服务; 注意:该语句必须,否则dataTable无法刷新.
        success: function (data) {
            try {
                $('#tableSection').dataTable().fnDestroy();
                $('#tableSection').attr('style', 'width:100%;');
                if (data == null) {
                    $('#tbodySection').html("");
                } else {
                    createDataTableOfSections(data);
                    if (isUpdated) {
                        showSourceSections();
                    }
                }
                extendDatatable();
            } catch (err) {
                alert(err);
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取结构物施工截面时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

function createDataTableOfSections(data) {
    var sb = new StringBuffer();
    $.each(data.sections, function (i, item) {
        g_sectionsConfigured["section-" + item.sectionId] = { // assign value
            sectionId: item.sectionId,
            sectionName: item.sectionName,
            sectionStatus: item.sectionStatus,
            heapMapName: item.heapMapName
        };

        sb.append('<tr id="tr-' + item.sectionId + '">');
        sb.append('<td>' + item.sectionName + '</td>');
        switch (item.sectionStatus) {
            case 0:
                sb.append('<td id="td_status_0">' + '未施工' + '</td>');
                break;
            case 1:
                sb.append('<td id="td_status_1">' + '施工中' + '</td>');
                break;
            case 2:
                sb.append('<td id="td_status_2">' + '施工完成' + '</td>');
                break;
            default:
                sb.append('<td id="td_status_3">' + '施工状态未知' + '</td>');
                break;
        }
        sb.append('<td><a href="#getSectionModal" class="aViewSectionModal" data-toggle="modal">' + '查看'
            + '</a> | <a href="#addSectionModal" class="aModifySectionModal" data-toggle="modal">' + '修改'
            + '</a> | <a href="#deleteSectionModal" data-toggle="modal" onclick="confirmDeleteSection(this,' + item.sectionId + ')">' + '删除' + '</a></td>');
        if (item.heapMapName == null) {
            sb.append('<td><span style="color: gray">布点</span></td>');
        } else {
            sb.append('<td><a href="javascript:;" id="aStationingSection-' + item.sectionId + '" class="aAccessHeapMapConfig">布点</a></td>');
        }
        sb.append('</tr>');
    });
    $('#tbodySection').html(sb.toString());
}

function bindClickEvent() {
    // 施工截面配置中"新增截面"点击方法
    $('#btnAddSection').click(onaddSection);
    
    // "新增截面"modal中"快速配置"点击方法
    $('#btnCopySection').click(showCopySections);
    
    // "新增截面"modal中"重置"点击方法
    $('#btnResetAddSection').click(onaddSection);
    
    // "新增截面"modal中"保存"点击方法
    $('#btnSaveAddSection').click(function () {
        if ($('#group-uploadSection')[0].style.display == 'none') {
            saveAddedSectionAndHotspots('Save');
        } else {
            saveAddedSection('Save');
        }
    });

    // "新增截面"modal中"保存并关闭"点击方法
    $('#btnSaveCloseAddSection').click(function () {
        if ($('#group-uploadSection')[0].style.display == 'none') {
            saveAddedSectionAndHotspots('SaveAndClose');
        } else {
            saveAddedSection('SaveAndClose');
        }
    });

    // "查看"单个施工截面信息
    $('#tableSection').on('click', 'a.aViewSectionModal', onviewSection);

    // "修改"单个施工截面信息
    $('#tableSection').on('click', 'a.aModifySectionModal', onmodifySection);
    
    // "修改"施工截面modal中"重置"点击方法
    $('#btnResetModifySection').click(onmodifySection);
    
    // "修改"施工截面modal中"保存"点击方法
    $('#btnSaveModifySection').click(function () {
        saveModifiedSection('Save');
    });

    // "修改"施工截面modal中"保存并关闭"点击方法
    $('#btnSaveCloseModifySection').click(function () {
        saveModifiedSection('SaveAndClose');
    });

    // "上传"施工截面图点击方法
    $('#btnUploadSection').click(onuploadSection);

    // 施工截面"布点"
    $('#tableSection').on('click', 'a.aAccessHeapMapConfig', ondistributeHotspotsOnSection);
}

function bindChangeEvent() {
    $('#fileSection').change(function () {
        var file = this.files[0];
        if ((file.size / 1024).toFixed(1) > 1024) {
            alert("请上传小于1M的热点图");
            return;
        }
    });

    // "新增截面"modal中"源截面"change方法
    $('#selectCopySections').change(copySectionConfig);
}

/**
 * 显示普通配置界面
 */
function showGeneralConfiguration(isModified) {
    $('#group-copySection').hide();
    $('#group-uploadSection').show();
    if (isModified) {
        $('#express-configuration').hide();
    } else {
        $('#express-configuration').show();
    }
}

/**
 * 显示快速配置界面
 */
function showExpressConfiguration() {
    $('#group-uploadSection').hide();
    $('#group-copySection').show();
    $('#express-configuration').show();
}

function onaddSection() {
    $('#textAddOrModifySection').text('新增截面');
    enableSaveButtons();
    
    showOrHideCopySections();
    showGeneralConfiguration(false);
    
    g_sectionHeapMapName = null;
    clearInputFile();
    // 只显示"新增截面"相关保存按钮
    $('#btnResetModifySection').hide();
    $('#btnSaveCloseModifySection').hide();
    $('#btnSaveModifySection').hide();
    $('#btnResetAddSection').show();
    $('#btnSaveCloseAddSection').show();
    $('#btnSaveAddSection').show();

    $('#addSectionName').val('');
    $('#addSectionStatus').val('1'); // 施工中
    // document.getElementById('addSectionStatus').options[1].selected = true;
    $('#uploadImgSection').removeAttr('src');
}

function showOrHideCopySections() {
    $('#labelCopySection').hide();
    $('#controlsCopySection').hide();
    if (jQuery.isEmptyObject(g_sectionsConfigured)) {
        $('#btnCopySection').hide();
    } else {
        $('#btnCopySection').show();
    }
}

function showCopySections() {
    $('#labelCopySection').show();
    $('#controlsCopySection').show();
    $('#btnCopySection').hide();
    
    getUnconfiguredSensors(false); // 获取未配置的传感器, 作"新增截面"modal中"快速配置"用
    
    showSourceSections();
    showExpressConfiguration();
    copySectionConfig();
}

function showSourceSections() {
    var options = '';
    for (var key in g_sectionsConfigured) {
        var section = g_sectionsConfigured[key];
        options += '<option value="' + section.sectionId + '">' + section.sectionName + '</option>';
    }
    // 刷新已配置施工截面选项,下面两行必须！
    $('#selectCopySections').removeClass('chzn-done');
    $('#selectCopySections_chzn').remove();
    $('#selectCopySections').html(options);
    $('#selectCopySections').chosen({
        no_results_text: "没有找到",
        allow_single_de: true
    });
}

function copySectionConfig() {
    var sectionId = parseInt($('#selectCopySections').val());
    var section = g_sectionsConfigured["section-" + sectionId];
    $('#addSectionName').val(section.sectionName);
    $('#addSectionStatus').val(section.sectionStatus);
    
    showSectionImage(sectionId, 'copyContainerImgSection', 'copyImgSection', 'copySectionImgLoader');
}

function onviewSection() {
    $('#getSectionModalTitle').html(getCookie('SectionStructName'));

    var tr = $(this).parents('tr');
    var selectedRow = tr[0];
    var sectionId = parseInt(selectedRow.id.split('tr-')[1]);
    var sectionName = selectedRow.cells[0].innerText;
    var sectionStatus = selectedRow.cells[1].innerText;

    $('#getSectionName').val(sectionName);
    $('#getSectionStatus').val(sectionStatus);

    showSectionImage(sectionId, 'viewContainerImgSection', 'viewImgSection', 'viewSectionImgLoader');
}

function onmodifySection() {
    $('#textAddOrModifySection').text('修改截面');
    showGeneralConfiguration(true);
    
    clearInputFile();
    
    // 只显示"修改"相关保存按钮
    $('#btnResetAddSection').hide();
    $('#btnSaveCloseAddSection').hide();
    $('#btnSaveAddSection').hide();
    $('#btnResetModifySection').show();
    $('#btnSaveCloseModifySection').show();
    $('#btnSaveModifySection').show();

    var sectionId, sectionName, sectionStatusCode;
    if (this.id == 'btnResetModifySection' && g_currentSection != null) {
        sectionId = g_currentSection.sectionId;
        sectionName = g_currentSection.sectionName;
        sectionStatusCode = g_currentSection.sectionStatusCode;
    } else {
        var tr = $(this).parents('tr');
        var selectedRow = tr[0];
        sectionId = parseInt(selectedRow.id.split('tr-')[1]);
        sectionName = selectedRow.cells[0].innerText;
        sectionStatusCode = selectedRow.cells[1].id.split('td_status_')[1];
        g_currentSection = { sectionId: sectionId, sectionName: sectionName, sectionStatusCode: sectionStatusCode }; // assign value
    }

    setCookie('CurrentSectionId', sectionId);

    $('#addSectionName').val(sectionName);
    $('#addSectionStatus').val(sectionStatusCode);
    //$('#addSectionStatus option[text="' + sectionStatusCode + '"]').attr('selected', true); // 设置选中项

    showSectionImage(sectionId, 'uploadContainerImgSection', 'uploadImgSection', 'uploadSectionImgLoader');
}

/**
 * 显示施工截面图
 * @param sectionId 施工截面编号（只能是数字）
 * @param domContainerImgSection 施工截面图容器id（字符串值）
 * @param domImgSection 施工截面图id（字符串值）
 */
function showSectionImage(sectionId, domContainerImgSection, domImgSection, domImgLoader) {
    $('#' + domImgLoader).show();
    var url = apiurl + '/section/' + sectionId + '/info' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            $('#' + domImgLoader).hide();
            if (data == null || data.heapMapName == null) {
                $('#' + domContainerImgSection).html('<img id=' + domImgSection + ' src="/" alt="此截面暂无施工截面图" style="width: 400px; height: 200px; color: red;" />');
                return;
            }
            try {
                document.getElementById(domImgSection).src = "/resource/img/Topo/" + data.heapMapName;
                if (domContainerImgSection == 'copyContainerImgSection') {
                    getHotspotsOfCurrentSection(sectionId, domContainerImgSection, domImgSection, domImgLoader);
                }
                g_sectionHeapMapName = data.heapMapName;
            } catch (err) {
                alert(err);
            }
        },
        error: function (xhr) {
            $('#' + domImgLoader).hide();
            if (xhr.status == 403) {
                logOut();
            } else if (xhr.status !== 0 && xhr.status !== 405) { // aborted requests should be just ignored and no error message be displayed
                alert('获取施工截面信息时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

function getHotspotsOfCurrentSection(sectionId, domContainerImgSection, domImgSection, domImgLoader) {
    $('#' + domImgLoader).show(); // show the loading marker.
    
    clearContext(domContainerImgSection, domImgSection);
    
    var url = apiurl + '/section/' + sectionId + '/hotspot-config' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data.length != 0) {
                g_expressConfiguredHotspots = data; // assign value
                drawHotspots(data, domContainerImgSection, domImgSection);
                createSelectSensorsContainer(data);
            }
            $('#' + domImgLoader).hide(); // hide the loading marker.
        },
        error: function (xhr) {
            $('#' + domImgLoader).hide(); // hide the loading marker.
            if (xhr.status == 403) {
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取已配置的传感器热点信息时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function clearContext(domContainerImgSection, domImgSection) {
    g_expressConfiguredHotspots = []; // clear up it
    
    tryRemoveHotspots(domContainerImgSection, domImgSection); // clear up old hotspots

    $('#group_copySensors_title').hide();
    $('#group_copySensors_content').html('').hide(); // clear up lists of source and destination sensors
    enableSaveButtons();
}

function tryRemoveHotspots(domContainerImgSection, domImgSection) {
    var $hotspots = $('#' + domContainerImgSection + ' img').not('#' + domImgSection);
    if ($hotspots.length > 0) {
        $hotspots.remove();
    }
}

function drawHotspots(data, domContainerImgSection, domImgSection) {
    for (var i = 0; i < data.length; i++) {
        var hotspot = data[i];
        var iconType = "icon-" + hotspot.productTypeId;
        var src = "/resource/img/factorIcon/icon-" + hotspot.productTypeId + "-5.png";
        var $img = $('#' + domImgSection);
        var x = hotspot.xAxis * $img.width() - 8;
        var y = hotspot.yAxis * $img.height() - 8;
        var imgEle = '<img id="img-' + hotspot.sensorId + '" class="' + iconType + ' hotspot"' + ' src="' + src + '" data-toggle="tooltip" title="' + hotspot.location
            + '" style="top: ' + y + 'px; left: ' + x + 'px; position: absolute; cursor: pointer;" width="16" height="16"'
            + ' onclick="editHotspot(event)" />';
        $('#' + domContainerImgSection).append(imgEle);
    }
    $(".spot").tooltip({
        html: true,
        placement: 'bottom'
    });
}

function editHotspot(e) {
    renderHotspotAndSelectBox(e);
}

/**
 * 点击热点图中热点, 渲染对应热点颜色{ "红色"或"绿色" }, 同时切换选择框的选择状态{ "选中" 或 "未选中" }.
 */
function renderHotspotAndSelectBox(e) {
    var obj = e.srcElement;
    var currentHotspotIcon = obj.src;
    var currentSensorId = obj.id.split('img-')[1];
    if (/(-5.png)$/.test(currentHotspotIcon)) {
        obj.src = currentHotspotIcon.slice(0, -6) + '-1.png'; // red icon.
        $('#selectSensors_' + currentSensorId + '_chzn a').attr('style', 'box-shadow: 0 0 5px 1px rgba(116, 178, 227, 0.847059) !important');
    } else if (currentHotspotIcon.indexOf('-1.png') != -1) {
        obj.src = currentHotspotIcon.split('-1.png')[0] + '-5.png'; // green icon of sensor.
        $('#selectSensors_' + currentSensorId + '_chzn a').removeAttr('style');
    }
    // render other hotspots to green icon
    var $otherHotspots = $('.hotspot').not('#' + obj.id);
    $.each($otherHotspots, function (index, item) {
        var hotspotIcon = item.src;
        var sensorId = item.id.split('img-')[1];
        if (!/(-5.png)$/.test(hotspotIcon)) {
            item.src = item.src.split('-1.png')[0] + '-5.png';
            $('#selectSensors_' + sensorId + '_chzn a').removeAttr('style');
        }
    });
}

function createSelectSensorsContainer(data) {
    $('#group_copySensors_title').show();
    
    var doms = '';
    for (var i = 0; i < data.length; i++) {
        var srcHotspot = data[i];
        doms +=
            '<div class="control-group">' +
                '<div class="span6"><input type="text" id="srcSensor_' + srcHotspot.sensorId + '" name="srcSensor_' +
                    srcHotspot.sensorId + '" value="' + srcHotspot.location + '" disabled="disabled" /></div>' +
                '<div class="span6" style="position: relative;">' +
                    '<select id="selectSensors_' + srcHotspot.sensorId + '" name="selectSensors_' + srcHotspot.sensorId +
                        '" class="chzn-select selectSensors" data-placeholder="请选择" style="width: 220px;"></select>' +
                    '<span style="position: absolute; top: 8px; right: 10px; color: red;">*</span></div>' +
            '</div>';
    }
    $('#group_copySensors_content').html(doms).show();
    
    createSelectSensorsOptions();
}

function enableSaveButtons() {
    $('#btnSaveCloseAddSection').attr('disabled', false);
    $('#btnSaveAddSection').attr('disabled', false);
}

function disableSavaButtons() {
    $('#btnSaveCloseAddSection').attr('disabled', true);
    $('#btnSaveAddSection').attr('disabled', true);
}

function createSelectSensorsOptions() {
    var options;
    if (jQuery.isEmptyObject(g_unconfiguredSensors)) {
        options = '<option value="">无可布点传感器<option>';
        disableSavaButtons();
    } else {
        options = '<option value="">请选择<option>';
        for (var key in g_unconfiguredSensors) {
            var destSensor = g_unconfiguredSensors[key];
            options += '<option value="option_' + destSensor.sensorId + '">' + destSensor.location + '</option>';
        }
        enableSaveButtons();
    }
    var $selectSensors = $('.selectSensors');
    $.each($selectSensors, function (index, item) {
        var srcSensorId = parseInt(item.id.split('selectSensors_')[1]);
        showSelectSensors(srcSensorId, options);
    });
    // bind "select" change event
    $selectSensors.unbind('change').change(onchangeSelectSensors);
}

function showSelectSensors(srcSensorId, options) {
    // 刷新结构物下未配置传感器选项,下面两行必须！
    $('#selectSensors_' + srcSensorId).removeClass('chzn-done');
    $('#selectSensors_' + srcSensorId + '_chzn').remove();
    $('#selectSensors_' + srcSensorId).html(options);
    $('#selectSensors_' + srcSensorId).chosen({
        no_results_text: "没有找到",
        allow_single_de: true
    });
    // "请选择" or "无可布点传感器"置灰
    var val = $('#selectSensors_' + srcSensorId).val();
    if (val == "") { // "请选择" or "无可布点传感器"
        $('#selectSensors_' + srcSensorId + '_chzn a span').css('color', '#999');
    }
    // 隐藏选择列表中"请选择"项
    var $li = $('#selectSensors_' + srcSensorId + '_chzn_o_0');
    if ($li.text() == "请选择") {
        $li.hide();
    }

    $('#selectSensors_' + srcSensorId + '_chzn').unbind('click').click(onclickSelectSensors);
}

/**
 * 选择"目标传感器", 渲染对应热点颜色{ "红色"或"绿色" }, 同时切换选择框的选择状态{ "选中" 或 "未选中" }.
 */
function onclickSelectSensors() {
    var currentSensorId = parseInt(this.id.split('selectSensors_')[1].split('_chzn')[0]);
    var objImg = $('#img-' + currentSensorId)[0];
    var currentHotspotIcon = objImg.src;
    var isActive = $('#selectSensors_' + currentSensorId + '_chzn').hasClass('chzn-container-active');
    if (/(-5.png)$/.test(currentHotspotIcon)) {
        objImg.src = currentHotspotIcon.slice(0, -6) + '-1.png'; // red icon.
        $('#selectSensors_' + currentSensorId + '_chzn a').attr('style', 'box-shadow: 0 0 5px 1px rgba(116, 178, 227, 0.847059) !important');
    } else if (currentHotspotIcon.indexOf('-1.png') != -1 && !isActive) {
        objImg.src = currentHotspotIcon.split('-1.png')[0] + '-5.png'; // green icon of sensor.
        $('#selectSensors_' + currentSensorId + '_chzn a').removeAttr('style');
    }
    // render other hotspots to green icon
    var $otherHotspots = $('.hotspot').not('#' + objImg.id);
    $.each($otherHotspots, function (index, item) {
        var hotspotIcon = item.src;
        var sensorId = item.id.split('img-')[1];
        if (!/(-5.png)$/.test(hotspotIcon)) {
            item.src = item.src.split('-1.png')[0] + '-5.png';
            $('#selectSensors_' + sensorId + '_chzn a').removeAttr('style');
        }
    });
}

/**
 * 实时更新可配置热点的传感器列表
 */
function onchangeSelectSensors() {
    var tempUnconfiguredSensors = {};
    var key;
    for (key in g_unconfiguredSensors) {
        var sensorId = g_unconfiguredSensors[key].sensorId;
        tempUnconfiguredSensors["temp_" + sensorId] = g_unconfiguredSensors[key];
    }
    $.each($('.selectSensors'), function (index, item) {
        if (item.value != "") {
            var destSensorId = parseInt(item.value.split('option_')[1]);
            delete tempUnconfiguredSensors["temp_" + destSensorId];
        }
    });
    updateSelectSensors(tempUnconfiguredSensors);
}

function updateSelectSensors(oUnconfiguredSensors) {
    $.each($('.selectSensors'), function (index, item) {
        var options;
        if (item.value == "") {
            if (jQuery.isEmptyObject(oUnconfiguredSensors)) {
                options = '<option value="">无可布点传感器<option>';
                disableSavaButtons();
            } else {
                options = '<option value="">请选择<option>';
                enableSaveButtons();
            }
        } else {
            var selectedSensorId = parseInt(item.value.split('option_')[1]);
            var selectedSensor = g_unconfiguredSensors["destSensor_" + selectedSensorId];
            options = '<option value="option_' + selectedSensor.sensorId + '">' + selectedSensor.location + '</option>'; // this option is selected by default
        }
        for (key in oUnconfiguredSensors) {
            var destSensor = oUnconfiguredSensors[key];
            options += '<option value="option_' + destSensor.sensorId + '">' + destSensor.location + '</option>';
        }
        var srcSensorId = parseInt(item.id.split('selectSensors_')[1]);
        showSelectSensors(srcSensorId, options);
    });
}

/**
 * 获取未配置的传感器
 */
function getUnconfiguredSensors(isUpdated) {
    g_unconfiguredSensors = {}; // empty it
    
    var url = apiurl + "/struct/" + g_structId + "/product/non-hotspot" + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data.length != 0) {
                $.each(data, function (i, productSensors) {
                    $.each(productSensors.sensors, function (j, sensor) {
                        g_unconfiguredSensors["destSensor_" + sensor.sensorId] = { sensorId: sensor.sensorId, location: sensor.location }; // assign value
                    });
                });
            }
            if (isUpdated) {
                createSelectSensorsOptions(); // 刷新"目标传感器"列表.
            }
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

function onuploadSection() {
    // 施工截面名称
    var sectionName = $('#addSectionName').val();
    if (sectionName == "") {
        alert("请填写截面名称");
        return;
    }

    var imgSrc = document.getElementById('fileSection').value;
    if (imgSrc == "") {
        alert("请选择图片");
        return;
    }
    var srcs = imgSrc.split('.');
    var imgExtension = srcs[srcs.length - 1];
    if (imgExtension != "jpg" && imgExtension != "jpeg" && imgExtension != "png" && imgExtension != "svg") {
        alert("请选择正确的图片格式(jpg|jpeg|png|svg)");
        clearInputFile();
        return;
    } else {
        var guid = getGuidGenerator();
        var ashxUrl = '/Support/SectionHeapMapHandler.ashx?uploadingStructId=' + g_structId + '&uploadingGuid=' + guid + '&uploadingSectionName=' + sectionName;
        // 执行AJAX上传文件  
        $.ajaxFileUpload({
            url: ashxUrl,
            secureuri: false,
            fileElementId: 'fileSection',
            dataType: 'json',
            success: function (data, status) {
                if (data == 405) {
                    alert("抱歉，没有上传权限");
                    return;
                } else if (data == 403) {
                    logOut();
                    return;
                }

                g_sectionHeapMapName = data[0];
                // 用动态替换标签和随机数来解决图片缓存的问题
                var img = '<img id="uploadImgSection" style="width: 400px; height:200px;" src="/resource/img/Topo/' + g_sectionHeapMapName + '?' + Math.random() + '">';
                $('#uploadContainerImgSection').html(img);
            }
        });
    }
}

/**
 * 获取配置的施工截面信息
 */
function getSectionConfig() {
    // 施工截面名称
    var sectionName = $('#addSectionName').val();
    if (sectionName == "") {
        alert("请填写截面名称");
        return null;
    }
    // 施工截面状态
    var sectionStatusCode = parseInt($('#addSectionStatus').val());
    var sectionConfig;
    if (g_sectionHeapMapName == null) {
        sectionConfig = {
            "sectionName": encodeURIComponent(sectionName),
            "sectionStatus": sectionStatusCode
        };
    } else {
        sectionConfig = {
            "sectionName": encodeURIComponent(sectionName),
            "sectionStatus": sectionStatusCode,
            "heapMapName": encodeURIComponent(g_sectionHeapMapName)
        };
    }

    return sectionConfig;
}

function getSectionHotspotsConfig() {
    var sectionHotspotsConfig = getSectionConfig();
    var arrHotspot = [];
    var len = g_expressConfiguredHotspots.length;
    if (len > 0) {
        var isAllValid = checkValidityOfExpressConfiguredSensors();
        if (!isAllValid) {
            return null;
        }
        for (var i = 0; i < len; i++) {
            var hotspot = g_expressConfiguredHotspots[i];
            var destSensorId = parseInt($('#selectSensors_' + hotspot.sensorId).val().split('option_')[1]); // 绑定待布点的传感器
            // Sensor
            var hotspotToSave = {};
            hotspotToSave.sensorId = destSensorId;
            hotspotToSave.xAxis = hotspot.xAxis;
            hotspotToSave.yAxis = hotspot.yAxis;
            hotspotToSave.svgPath = hotspot.svgPath;

            arrHotspot.push(hotspotToSave);
        }
    }
    sectionHotspotsConfig.hotspotsConfig = arrHotspot;

    return sectionHotspotsConfig;
}

/**
 * 保存新增的施工截面信息
 */
function saveAddedSection(status) {
    var sectionConfig = getSectionConfig();
    if (sectionConfig == null) {
        return;
    }
    // 新增结构物下的施工截面
    var url = apiurl + '/struct/' + g_structId + '/section/add' + '?token=' + getCookie('token');
    $.ajax({
        type: 'post',
        url: url,
        data: sectionConfig,
        cache: false,
        statusCode: {
            200: function (data) { // 200: "OK"
                showSectionConfig(); // 刷新施工截面列表
                alert('保存成功');
                if (status == 'SaveAndClose') {
                    $('#btnCloseAddSection').click();
                }
            },
            403: function () {
                logOut();
            },
            400: function (result) {
                alert("新增施工截面时发生异常. 可能原因: 施工截面名称已存在或其它.\r\n" + result.status + " : " + result.statusText);
                $('#addSectionName').focus();
                if (status == 'SaveAndClose') {
                    $('#btnCloseAddSection').click();
                }
            },
            500: function () {
                alert("内部异常");
            },
            404: function () {
                alert('url错误');
            },
            405: function () {
                alert('抱歉，没有增加施工截面的权限');
            }
        }
    });
}

/**
 * 保存新增的截面及其"快速配置"的热点
 */
function saveAddedSectionAndHotspots(status) {
    if ($('#selectCopySections').find("option:selected").text() == $('#addSectionName').val()) {
        $('#addSectionName').focus();
        return;
    }
    
    var sectionHotspotsConfig = getSectionHotspotsConfig();
    if (sectionHotspotsConfig == null) {
        return;
    }
    // 新增施工截面及其多个传感器热点
    var url = apiurl + '/struct/' + g_structId + '/section/hotspots-config/add' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'post',
        dataType: 'json',
        data: sectionHotspotsConfig,
        cache: false,
        success: function (data) {
            showSectionConfig(true); // 刷新施工截面列表
            
            getUnconfiguredSensors(true); // 刷新未配置传感器
            
            alert(data.Message);
            if (status == 'SaveAndClose') {
                $('#btnCloseAddSection').click();
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                logOut();
            } else if (xhr.status == 400) {
                var responseJson = $.parseJSON(xhr.responseText); // { Message: "添加施工截面的传感器热点失败, 原因: 传感器热点已存在." }
                alert(responseJson.Message);
            } else if (xhr.status == 405) {
                alert('抱歉，没有增加施工截面的权限');
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('添加施工截面及其多个传感器热点时发生异常.\r\n' + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function checkValidityOfExpressConfiguredSensors() {
    var isAllValid = true;
    $.each($('.selectSensors'), function (index, item) {
        if (item.value == "") {
            $('#' + item.id + '_chzn a').attr('style', 'box-shadow: 0 0 5px 1px rgba(116, 178, 227, 0.847059) !important');
            isAllValid = false;
        } else {
            $('#' + item.id + '_chzn a').removeAttr('style');
        }
    });
    return isAllValid;
}


/**
 * 保存已修改的施工截面信息
 */
function saveModifiedSection(status) {
    // 施工截面名称
    var sectionName = $('#addSectionName').val();
    if (sectionName == "") {
        alert("请填写截面名称");
        return;
    }
    // 施工截面状态
    var sectionStatusCode = parseInt($('#addSectionStatus').val());
    var config;
    if (g_sectionHeapMapName == null) {
        config = {
            "sectionName": encodeURIComponent(sectionName),
            "sectionStatus": sectionStatusCode
        };
    } else {
        config = {
            "sectionName": encodeURIComponent(sectionName),
            "sectionStatus": sectionStatusCode,
            "heapMapName": encodeURIComponent(g_sectionHeapMapName)
        };
    }

    // 修改施工截面信息
    var url = apiurl + '/section/' + getCookie('CurrentSectionId') + '/modify' + '?token=' + getCookie('token');
    $.ajax({
        type: 'post',
        url: url,
        data: config,
        cache: false,
        statusCode: {
            202: function () {
                showSectionConfig(); // 刷新施工截面列表

                alert('保存成功');

                if (status == 'SaveAndClose') {
                    $('#btnCloseAddSection').click();
                }
            },
            403: function () {
                logOut();
            },
            400: function (result) {
                alert(result.responseJSON.Message);
                if (status == 'SaveAndClose') {
                    $('#btnCloseAddSection').click();
                }
            },
            500: function () {
                alert("内部异常");
            },
            404: function () {
                alert('url错误');
            },
            405: function () {
                alert('抱歉，没有修改施工截面权限');
            }
        }
    });
}

/**  
 * 删除结构物的施工截面
 */
function confirmDeleteSection(dom, sectionId) {
    var tr = $(dom).parents('tr');
    var selectedRow = tr[0];
    $('#alertMsgDelete').text('确定删除施工截面"' + selectedRow.cells[0].innerText + '" ?');

    $('#btnDeleteConfirm').unbind("click").click(function () {
        var url = apiurl + '/section/' + sectionId + '/remove' + '?token=' + getCookie('token');
        $.ajax({
            type: 'post',
            url: url,
            success: function (result) {
                showSectionConfig(); // 刷新施工截面列表
                alert(result.Message); 
                $('#deleteSectionModal').modal('hide');
            },
            error: function (xhr) {
                if (xhr.status == 403) {
                    logOut();
                } else if (xhr.status == 400) {
                    alert(xhr.responseJSON.Message);
                } else if (xhr.status == 405) {
                    alert("抱歉，没有删除施工截面的权限");
                } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                    alert("删除施工截面时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
                }
            }
        });
    });
}

/**  
 * 施工截面"布点"
 */
function ondistributeHotspotsOnSection() {
    var tr = $(this).parents('tr');
    var selectedRow = tr[0];
    var sectionId = parseInt(selectedRow.id.split('tr-')[1]);

    var sectionName = selectedRow.cells[0].innerText;
    setCookie('CurrentSectionName', sectionName); // 截面热点图配置页面用

    var url = apiurl + '/section/' + sectionId + '/info' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data == null) {
                alert("获取施工截面信息失败");
                return;
            }
            try {
                if (data.heapMapName == null) {
                    alert("请先上传施工截面图片");
                    return;
                }
                var length = data.heapMapName.split('.').length;
                var imageSuffix = data.heapMapName.split('.')[length - 1];
                var urlSectionStationingPage;
                if (imageSuffix == "svg") {
                    urlSectionStationingPage = 'SectionHeapMapConfigSvg.aspx?structId=' + g_structId + '&sectionId=' + sectionId + '&imageName=' + data.heapMapName;
                } else if (imageSuffix == 'jpg' || imageSuffix == 'png' || imageSuffix == 'jpeg') {
                    urlSectionStationingPage = 'SectionHeapMapConfig.aspx?structId=' + g_structId + '&sectionId=' + sectionId + '&imageName=' + data.heapMapName;
                } else {
                    alert('施工截面的图片格式不在（svg|jpg|png|jpeg）范围内, 请重新选择图片.');
                    return;
                }

                location.href = urlSectionStationingPage; // 进入施工截面布点页面
            } catch (err) {
                alert(err);
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                logOut();
            } else if (xhr.status == 405) { // aborted requests should be just ignored and no error message be displayed
                alert('抱歉，没有布点权限');
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取施工截面信息时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            }
        }
    });
}

/**
 * 函数功能：渲染表格
 */
function extendDatatable() {
    $('#tableSection').dataTable({
        "aLengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, "所有"]
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
            'aTargets': [2, 3] // 不排序的列
        }],
        "aaSorting": [[0, "asc"]] // 第1列升序排序
    });
}

function clearInputFile() {
    var file = $("#fileSection");
    file.val('');
}

/**
 * 生成随机guid数
 */
function getGuidGenerator() {
    var s4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return (s4() + s4() + "-" + s4() + "-" + s4() + "-" + s4() + "-" + s4() + s4() + s4());
}