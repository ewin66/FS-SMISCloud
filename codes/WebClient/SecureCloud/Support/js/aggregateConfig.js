var AggFactors;
var AggConfigs;
var ConfigTable = null;
var AggType = {
    Day: 1,
    Week: 2,
    Month: 3
};

var ChangeType = { Add: 1, Modify: 2 };

var AggWay = { 1: "最大值",2: "平均值", 3: "最小值" };

var AggTimeDayOfWeek = { 1: "周日", 2: "周一", 3: "周二", 4: "周三", 5: "周四", 6: "周五", 7: "周六" };
var DateRangeDayOfWeek = { 1: "周一", 2: "周二", 3: "周三", 4: "周四", 5: "周五", 6: "周六", 7: "周日" };
var nowOptType;
var nowConfigId = -1;

var defaultDayAggConfig = {
    aggtypeId: AggType.Day,
    aggwayId: 2,
    enable: true,
    beginHour: 0,
    endHour: 24,
    timeMode: '1'
};

var defaultWeekAggConfig = {
    aggtypeId: AggType.Week,
    aggwayId: 2,
    enable: true,
    beginHour: 0,
    endHour: 24,
    beginDate:7,
    endDate:7,
    timeMode: '2,0'
};

var defaultMonthAggConfig = {
    aggtypeId: AggType.Month,
    aggwayId: 2,
    enable: true,
    beginHour: 0,
    endHour: 24,
    beginDate: -1,
    endDate: -1,
    timeMode: '1,0'
};


$('#tabAggConfig').click(function () {
    pageLoad();
});

$('#btnAddAggConfig').click(function () {
    //var aggTypeId = getSelectedItemId('aggType');
    
    initConfig(AggType.Day, defaultDayAggConfig, ChangeType.Add);
    nowOptType = ChangeType.Add;
    nowConfigId = -1;
    $('#editAggConfig').modal('show');
});

function modifyConfig(configId) {
    initConfig(AggConfigs[configId].aggtypeId, AggConfigs[configId], ChangeType.Modify);
    nowOptType = ChangeType.Modify;
    nowConfigId = AggConfigs[configId].id;
    $('#editAggConfig').modal('show');
}

function deleteConfig(configId) {
    if (confirm("确定删除吗？")) {
        var url = apiurl + '/struct/config/aggconfigs/' +AggConfigs[configId].id+ '/delete?token=' + getCookie('token');
        $.ajax({
            type: 'post',
            url: url,
            data: null,
            dataType: 'json',
            statusCode: {
                202: function () {
                    alert('删除成功');
                    getAllAggConfig(structId);
                },
                403: function () {
                    alert("权限验证出错");
                    logOut();
                },
                400: function () {
                    alert("保存失败,参数错误");
                },
                500: function () {
                    alert("内部异常");
                },
                404: function () {
                    alert('url错误');    
                }
            }
        });
    }
}

function pageLoad() {
    
    // $("[name='switch-CheckBox']").not("[data-switch-no-init]").bootstrapSwitch();
   // $("#configEnable").not("[data-switch-no-init]").bootstrapSwitch();
    $('#configEnable div.checker').each(function () {
        $(this).hide();
    });
    
    getfactorList(structId);
    getAllAggConfig(structId);
    //getfactorList(86);
    //getAllAggConfig(86);
   
    var sb = new StringBuffer();
    sb.append('<option id="' + AggType.Day + '">' + "日聚集" + '</option>');
    sb.append('<option id="' + AggType.Week + '">' + "周聚集" + '</option>');
    sb.append('<option id="' + AggType.Month + '">' + "月聚集" + '</option>');
    $('#aggType').html(sb.toString());
    sb = new StringBuffer();
    sb.append('<option id="' + 1 + '">' + AggWay[1] + '</option>');
    sb.append('<option id="' + 2 + '">' + AggWay[2] + '</option>');
    sb.append('<option id="' + 3 + '">' + AggWay[3] + '</option>');
    $('#aggWay').html(sb.toString());
    
    sb = new StringBuffer();
    var i;
    for (i = 0; i < 24; i++) {
        sb.append('<option id="' + i + '">' + i+'时' + '</option>');
    }
    $('#beginDayAggTime').html(sb.toString());
    $('#beginWeekAggTime').html(sb.toString());
    $('#beginWeekAggTime').html(sb.toString());
    $('#beginMonthAggTime1').html(sb.toString());
    $('#beginMonthAggTime2').html(sb.toString());
    
    sb.append('<option id="' + 24 + '">' + 24 + '时' + '</option>');
    $('#beginDataTime').html(sb.toString());
   
    
    $('#endDataTime').html(sb.toString());
    

    
    
    sb = new StringBuffer();
    for (i = 1; i <= 31; i++) {
        sb.append('<option id="' + i + '">' + '第'+i+'号' + '</option>');
    }
    $('#beginMonthDay').html(sb.toString());
    $('#endMonthDay').html(sb.toString());
    sb.append('<option id="' + -1 + '">' + '最后一天' + '</option>');
    $('#monthAggTimeDay').html(sb.toString());
    
    sb = new StringBuffer();
    for (i = 1; i <= 7; i++) {
        sb.append('<option id="' + i + '">' + DateRangeDayOfWeek[i] + '</option>');
    }
    $('#beginWeekDay').html(sb.toString());
    $('#endWeekDay').html(sb.toString());
    
    sb = new StringBuffer();
    for (i = 1; i <= 7; i++) {
        sb.append('<option id="' + i + '">' + AggTimeDayOfWeek[i] + '</option>');
    }
    $('#monthAggTimeDayofWeek').html(sb.toString());
    $('#weekAggTimeDay').html(sb.toString());
    
}

function initConfigTable() {
    try {
        if (ConfigTable != null) {
            $('#aggconfigtable').dataTable().fnDestroy();
        }  
    } catch(e) {
    }
    
    //var tableData = createAggTableData(data);
    //// 生成数据表格(以时间为序)
    //tableManager('datatable', tableData.tableData, tableData.tableHeader, tableData.tableHeader.length - 1);
    
   
    var sb = new StringBuffer();
    for (var i = 0; i < AggConfigs.length; i++) {
        sb.append("<tr id=" + AggConfigs[i].id + "><td >" + AggConfigs[i].factorName + "</td>");//监测因素
        sb.append("<td>" + AggConfigs[i].aggtypeName + '</td>');//聚集类型名称
        if (AggConfigs[i].enable) {
            sb.append("<td>" + "启用" + "</td>");//启用状态
        } else {
            sb.append("<td>" + "禁用" + "</td>");//启用状态
        }
        
        sb.append("<td>" + getAggConfigDescription(AggConfigs[i]) + "</td>");//详细信息
        var str = "<td><a href='javascript:void(0)' onclick='modifyConfig(" + i + ")' class='editor_edit' data-toggle='modal'>修改</a> | ";
        str += "<a href='javascript:void(0)' onclick='deleteConfig(" + i + ")'  class='editor_delete' data-toggle='modal'>删除</a></td></tr>";
        sb.append(str);//操作
    }
    $('#aggconfigtablebody').html("");
    $('#aggconfigtablebody').append(sb.toString());
    
    ConfigTable = $('#aggconfigtable').dataTable({
        //"aLengthMenu": [
        //    [10, 25, 50, -1],
        //    [10, 25, 50, "All"]
        //],
        // set the initial value
        "bLengthChange": false,
        "iDisplayLength": -1,
        "sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
        "sPaginationType": "bootstrap",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "bDestroy": true,
        "bRetrieve": true,
        "bStateSave": true,
        "aoColumnDefs": [{
            'sWidth': '20%',
            'bSortable': false,
            'aTargets': [0]
        }, {
            'sWidth': '10%',
            'bSortable': true,
            'aTargets': [1]
        }, {
            'sWidth': '10%',
            'bSortable': true,
            'aTargets': [2]
        }, {
            'sWidth': '35%',
            'bSortable': false,
            'aTargets': [3]
        }, {
            'sWidth': '15%',
            'bSortable': false,
            'aTargets': [4]
        }]
    });
}

function getAggConfigDescription(config) {
    var configDsp = "";
    var timeModeArrary;
    switch (config.aggtypeId) {
        case 1:
            configDsp = "每天" + config.timeMode + "点，计算" + config.beginHour + "点-" + config.endHour + "点的数据的" + AggWay[config.aggwayId];
            break;
        case 2:
            timeModeArrary = config.timeMode.split(",");
            if (timeModeArrary.length == 2) {
                configDsp = "每周" + AggTimeDayOfWeek[parseInt(timeModeArrary[0])] + timeModeArrary[1] + "点，计算" + DateRangeDayOfWeek[config.beginDate] + "至" + DateRangeDayOfWeek[config.endDate] + config.beginHour + "点-" + config.endHour + "点的数据的" + AggWay[config.aggwayId];
            }   
            break;
        case 3:
            timeModeArrary = config.timeMode.split(",");
            if (timeModeArrary.length == 3) {
                configDsp = "每月第" + timeModeArrary[0] + "个" + AggTimeDayOfWeek[parseInt(timeModeArrary[1])] + timeModeArrary[2] + "点，计算" + config.beginDate + "号至" + config.endDate + "号" + config.beginHour + "点-" + config.endHour + "点的数据的" + AggWay[config.aggwayId];
            } else if (timeModeArrary.length == 2) {
                if (timeModeArrary[0] == -1) {
                    configDsp = "每月最后一天" + timeModeArrary[1] + "点，计算" + config.beginDate + "号至" + config.endDate + "号" + config.beginHour + "点-" + config.endHour + "点的数据的" + AggWay[config.aggwayId];
                } else {
                    configDsp = "每月" + timeModeArrary[0]+"号" + timeModeArrary[1] + "点，计算" + config.beginDate + "号至" + config.endDate + "号" + config.beginHour + "点-" + config.endHour + "点的数据的" + AggWay[config.aggwayId];
                }
                
            }
            break;
        
    default:
    }
    return configDsp;
}


// 显示监测因素列表
function getfactorList(structId) {
    var url = apiurl + '/struct/' + structId + '/factors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        async: false,
        success: function (data) {
            AggFactors = data;
            initFactorList();
        },
        error: function (xmlHttpRequest, textStatus, errorThrown) {
            if (xmlHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            }
            else if (xmlHttpRequest.status == 400) {
                alert("参数错误");
            }
            else if (xmlHttpRequest.status == 500) {
                alert("内部异常");
            }
            else {
                alert('url错误');
            }
        }
    });
}

function initFactorList() {
  
    var sb = new StringBuffer();
    for (var i = 0; i < AggFactors.length; i++) {

        var factor = AggFactors[i].children;

        for (var j = 0; j < factor.length; j++) {

            //sb.append('<option  value="' + factor[j].factorId + '">' + factor[j].factorName + '</option>');
            sb.append('<option id="' + factor[j].factorId + '">' + factor[j].factorName + '</option>');
        }

    }
    $('#aggFactor').html(sb.toString());
}

function getAllAggConfig(structId) {
    
    $('#aggconfigtablebody').children().remove();
    AggConfigs = null;
    var url = apiurl + '/struct/'+structId+'/config/aggconfigs?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        async: false,
        success: function (data) {
            AggConfigs = data;
            initConfigTable();
        },
        error: function (xmlHttpRequest, textStatus, errorThrown) {
            if (xmlHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            }
            else if (xmlHttpRequest.status == 400) {
                alert("参数错误");
            }
            else if (xmlHttpRequest.status == 500) {
                alert("内部异常");
            }
            else {
                alert('url错误');
            }
        }
    });
}


$("[name='aggconfig-switch-CheckBox']").not("[data-switch-no-init]").bootstrapSwitch();

function setSelectSelected(domId, id) {
    var select = document.getElementById(domId);
    for (var i = 0; i < select.options.length; i++) {
        if (select.options[i].id == id) {
            select.options[i].selected = true;
            break;
        }
        
    }
   // find("option[id=" + id + "]").attr("selected", true);
}

function setSwitchState(state) {
    var switchButton = document.getElementById('aggConfigEnable');
    //switchButton.bootstrapSwitch('toggleState');
    if (state != switchButton.checked) {
        $('#aggConfigEnable').bootstrapSwitch('toggleState');
       // $('#aggConfigEnable').bootstrapSwitch('setState', state);
    }


}

function initConfig(configType, configData, callType) {

    if (callType == ChangeType.Add) {
        document.getElementById('aggFormTitle').innerHTML = '新增聚集条件配置';
    } else {
        document.getElementById('aggFormTitle').innerHTML = '修改聚集条件配置';
        setSelectSelected('aggFactor', configData.factorId);
    }
    setSelectSelected('aggWay', configData.aggwayId);
    setSelectSelected('aggType', configData.aggtypeId);
    setSwitchState(configData.enable);
    setSelectSelected('beginDataTime', configData.beginHour);
    setSelectSelected('endDataTime', configData.endHour);
 
    switch (configType) {
        case AggType.Day:
            initDayConfig(configData);
            break;
        case AggType.Week:
            initWeekConfig(configData);
            break;
        case AggType.Month:
            initMonthConfig(configData);
            break;
        default:
            initDayConfig(configData);
            break;
    }
}

$('#aggType').change(function () {
    var aggTypeId = getSelectedItemId('aggType');
    var config;
    switch (aggTypeId) {
        case AggType.Day:
            config = defaultDayAggConfig;
            break;
        case AggType.Week:
            config = defaultWeekAggConfig;
            break;
        case AggType.Month:
            config = defaultMonthAggConfig;
            break;
        default:
            break;
    }
    initConfig(aggTypeId, config, nowOptType);
});

$('#aggFactor').change(function () {
    if (nowOptType == ChangeType.Modify) {
        return;
    }


    var aggTypeId = getSelectedItemId('aggType');
    var config;
    switch (aggTypeId) {
        case AggType.Day:
            config = defaultDayAggConfig;
            break;
        case AggType.Week:
            config = defaultWeekAggConfig;
            break;
        case AggType.Month:
            config = defaultMonthAggConfig;
            break;
        default:
            break;
    }
    initConfig(aggTypeId, config, nowOptType);
});

function getSelectedItemId(arg) {
    var obj = document.getElementById(arg);
    if (obj.selectedIndex < 0) {
       return -1;
    } else {
        return parseInt(obj.options[obj.selectedIndex].id);
    }
}

function initDayConfig(configData) {

    DivHide('monthTiming');
    DivHide('weekTiming');
    DivHide('monthAggDayRang');
    DivHide('weekAggDayRang');
    DivShow('dayTimeing');
    if (configData == null)
        return;
    setSelectSelected('beginDayAggTime', parseInt(configData.timeMode));
}

function initWeekConfig(configData) {

    DivHide('monthTiming');
    DivHide('dayTimeing');
    DivShow('weekTiming');
    DivHide('monthAggDayRang');
    DivShow('weekAggDayRang');
    if (configData == null)
        return;
    setSelectSelected('beginWeekDay', configData.beginDate);
    setSelectSelected('endWeekDay', configData.endDate);
    var timeModeArrary = configData.timeMode.split(",");
    setSelectSelected('weekAggTimeDay', parseInt(timeModeArrary[0]));
    setSelectSelected('beginWeekAggTime', parseInt(timeModeArrary[1]));
}

function initMonthConfig(configData) {

    DivHide('dayTimeing');
    DivHide('weekTiming');
    DivShow('monthTiming');
    DivShow('monthAggDayRang');
    DivHide('weekAggDayRang');
    if (configData == null)
        return;

    setSelectSelected('beginMonthDay', configData.beginDate);
    setSelectSelected('endMonthDay', configData.endDate);
    var timeModeArrary = configData.timeMode.split(",");
    if (timeModeArrary.length == 2) {
        $(':radio[name="optionsRadios"]').eq(0).attr("checked", true);
        setSelectSelected('monthAggTimeDay', parseInt(timeModeArrary[0]));
        setSelectSelected('beginMonthAggTime1', parseInt(timeModeArrary[1]));
    } else if (timeModeArrary.length == 3) {
        $(':radio[name="optionsRadios"]').eq(1).attr("checked", true);
        setSelectSelected('aggWeekIndex', parseInt(timeModeArrary[0]));
        setSelectSelected('monthAggTimeDayofWeek', parseInt(timeModeArrary[1]));
        setSelectSelected('beginMonthAggTime2', parseInt(timeModeArrary[2]));
    }
}

function IsConfigExist(newConfig, optType) {
    var ret = false;

    if (optType == ChangeType.Add) {
        for (var i = 0; i < AggConfigs.length; i++) {
            if (AggConfigs[i].factorId == newConfig.FacotrId && AggConfigs[i].aggtypeId == newConfig.AggTypeId) {
                ret = true;
                break;
            }
        }
    } else if (optType == ChangeType.Modify) {
        for (var i = 0; i < AggConfigs.length; i++) {
            if (AggConfigs[i].factorId == newConfig.FacotrId && AggConfigs[i].aggtypeId == newConfig.AggTypeId && AggConfigs[i].id != nowConfigId) {
                ret = true;
                break;
            }
        }
       
    }

    return ret;
}

function DivShow(arg) {
    document.getElementById(arg).style.display = "block";
}

function DivHide(arg) {
    document.getElementById(arg).style.display = "none";
}

function IsConfigValid(config) {
    if (config.BeginHour >= config.EndHour) {
        alert("每天开始时间需小于结束时间");
        return false;
    }

    if (config.AggTypeId != AggType.Day) {
        if (config.BeginDate > config.EndDate) {
            alert("聚集日期需小于等于结束日期");
            return false;
        }
    }
    return true;
}

$('#btnAggConfigClose').click(function() {
    $("#editAggConfig").modal("hide");
});


$('#btnAggConfigSave').click(function () {
    var aggTypeId = getSelectedItemId('aggType');
    var config;
    switch (aggTypeId) {
        case AggType.Day:
            config = saveDayAggConfig();
            break;
        case AggType.Week:
            config = saveWeekAggConfig();
            break;
        case AggType.Month:
            config = saveMonthAggConfig();
            break;
        default:
            break;
    }


    if (!IsConfigValid(config))
        return;
    

    if (IsConfigExist(config, nowOptType)) {
        if (nowOptType == ChangeType.Add) {
            alert('该监测因素已存在相同聚集周期配置，无法新增');
        } else if (nowOptType == ChangeType.Modify) {
            alert('该监测因素已存在相同聚集周期配置，无法修改');
        }
        return;
    }

    

    var url;
    if (nowOptType == ChangeType.Add) {
        url = apiurl + "/struct/config/aggconfigs/add?token=" + getCookie('token');
    }
    else if (nowOptType == ChangeType.Modify) {
        url = apiurl + "/struct/config/aggconfigs/update/" + nowConfigId + "/?token=" + getCookie('token');
    }

    $.ajax({
        type: 'post',
        url: url,
        data: config,
        dataType: 'json',
        statusCode: {
            202: function () {
                alert('保存成功');
                getAllAggConfig(structId);
                $("#editAggConfig").modal("hide");
            },
            403: function () {
                alert("权限验证出错");
                logOut();
            },
            400: function () {
                alert("保存失败,参数错误");
            },
            500: function () {
                alert("内部异常");
            },
            404: function () {
                alert('url错误');
            }
        }
    });
});

function saveDayAggConfig() {
    var typeId = getSelectedItemId('aggType');
    var factorId = getSelectedItemId('aggFactor');
    var aggWayid = getSelectedItemId('aggWay');
    var enable = document.getElementById('aggConfigEnable').checked;
    var beginHour = getSelectedItemId('beginDataTime');
    var endHour = getSelectedItemId('endDataTime');
    var timeMode = getSelectedItemId('beginDayAggTime');

    var configData = {
        "StructureId": structId,
        "FacotrId": factorId,
        "AggTypeId": typeId,
        "AggWayId": aggWayid,
        "BeginHour": beginHour,
        "EndHour": endHour,
        "BeginDate": "",
        "EndDate": "",
        "TimingMode": timeMode,
        "IsEnable": enable
    };
    return configData;
}

function saveWeekAggConfig() {
    var typeId = getSelectedItemId('aggType');
    var factorId = getSelectedItemId('aggFactor');
    var aggWayid = getSelectedItemId('aggWay');
    var enable = document.getElementById('aggConfigEnable').checked;
    var beginHour = getSelectedItemId('beginDataTime');
    var endHour = getSelectedItemId('endDataTime');
    var beginDate = getSelectedItemId('beginWeekDay');
    var endDate = getSelectedItemId('endWeekDay');
    var timeMode = getSelectedItemId('weekAggTimeDay') + ',' + getSelectedItemId('beginWeekAggTime');

    var configData = {
        "StructureId": structId,
        "FacotrId": factorId,
        "AggTypeId": typeId,
        "AggWayId": aggWayid,
        "BeginHour": beginHour,
        "EndHour": endHour,
        "BeginDate": beginDate,
        "EndDate": endDate,
        "TimingMode": timeMode,
        "IsEnable": enable
    };
    return configData;
}

function saveMonthAggConfig() {
    var typeId = getSelectedItemId('aggType');
    var factorId = getSelectedItemId('aggFactor');
    var aggWayid = getSelectedItemId('aggWay');
    var enable = document.getElementById('aggConfigEnable').checked;
    var beginHour = getSelectedItemId('beginDataTime');
    var endHour = getSelectedItemId('endDataTime');
    var beginDate = getSelectedItemId('beginMonthDay');
    var endDate = getSelectedItemId('endMonthDay');
    var timeMode = "";
    if ($("input[name='optionsRadios']:checked").val() == "option1") {
        timeMode = getSelectedItemId('monthAggTimeDay') + ',' + getSelectedItemId('beginMonthAggTime1');
    } else if ($("input[name='optionsRadios']:checked").val() == "option2") {
        timeMode = getSelectedItemId('aggWeekIndex') + ',' + getSelectedItemId('monthAggTimeDayofWeek')+ ',' + getSelectedItemId('beginMonthAggTime1');
    }

    var configData = {
        "StructureId": structId,
        "FacotrId": factorId,
        "AggTypeId": typeId,
        "AggWayId": aggWayid,
        "BeginHour": beginHour,
        "EndHour": endHour,
        "BeginDate": beginDate,
        "EndDate": endDate,
        "TimingMode": timeMode,
        "IsEnable": enable
    };
    return configData;
}

function aggConfigNextOnclick() {
    $('#tabAggConfig').removeClass('active');
    $('#tab_aggconfig').removeClass('active');
    $('#tabThreshold').addClass('active');
    $('#tab_threshold').addClass('active');
    //ThresholdPageLoad();
}




