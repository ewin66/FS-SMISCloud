var g_data = [];
var g_listOrgStructs = {}; // 保存组织及组织下结构物列表
var structId = -1;
var count = 0;
var MAX = 5;

var rowindex = 0;
var rowindexarray = new Array();

var multiFsctorId = -100;
var multiFsctorName = "多项监测因素";
var rename_rptId = "";

var successCount = 0;
var errorCount = 0;
var sumCount = 0;

$(function () {
    $('#dataServices').addClass('active');
    $('#ReportManager').addClass('active');
    initPage();
    getReportList();
    manualUploadBindClickEvent();
    dateTimePickerEvent();
    manualUploadChangeEvent();
    rowindexarray.push(0);
});

/******************************************人工上传报表start************************************/
// 人工上传报表   
function InitManualUploadModal() {
    for (var i = 0; i < rowindexarray.length; i++) {
        var index = rowindexarray[i];
        $('#DateType_' + index).find('option:selected').attr("selected", false);
        $('#DateType_' + index).trigger("liszt:updated");
        $('#RptDate_' + index).val('');
        clearInputFile('#RptFile_' + index);
    }
}

function manualUploadBindClickEvent() {
    $('#btnManualupload').click(function () {
        $('#ManualUploadModal').modal('show');
        // 初始化
        var currentUserId = getCookie("userId");
        if (currentUserId == "") {
            alert("获取用户id失败, 请检查浏览器Cookie是否已启用");
            return;
        }
        getOrgStructsListByUser(currentUserId);
        getDateType("");
        InitManualUploadModal();
    });
    
    $('#btn_manual_upload').unbind('click').click(function () {
        ProcessManualUpload();
    });

    $('#manual_upload_cancel').click(function () {
        $('.close').click();
    });

    $('#manual_upload_reset').click(function () {
        //组织
        $('#OrgList').find('option:selected').attr("selected", false);
        $('#OrgList').trigger("liszt:updated");
        showOrgStructs();
        InitManualUploadModal();
    });

    $('#rename_reset').click(function () {
        $('#rename_new').val("");
        $('#rename_new').focus();
    });

    $('#rename_cancel').click(function () {
        $('#rename_new').val("");
        $('.close').click();
    });

    $('#rename_sumit').click(function () {
        RenameSumit();
    });
}

function dateTimePickerEvent() {
    //日期控件参数设置
    $('.date').datetimepicker({
        format: 'yyyy-MM-dd',
        language: 'pt-BR',
        pickTime: false
    });
}

function manualUploadChangeEvent() {
    $('#OrgList').change(function () {
        showOrgStructs();
    });

    $('#StructList').change(function () {
        structId = parseInt($('#StructList').val());
        getFactorList(structId, "");
    });
}

function getOrgStructsListByUser(userId) {
    var url = apiurl + '/user/' + userId + '/org-structs/list' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        async: true,
        cache: false, // 必须加token，该属性才能通过代理服务
        success: function (data) {
            g_listOrgStructs = {}; // empty it
            if (data == null) {
                alert('该用户没有组织');
            } else {
                showOrgStructsListByUser(data);
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) { // 权限验证出错, 禁止访问
                logOut();
            } else if (xhr.status == 405) {
                alert("抱歉,没有访问权限!");
            }
            else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取用户下组织结构物列表时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function showOrgStructsListByUser(data) {
    var orgOptions = '';
    $.each(data.orgs, function (i, org) {
        orgOptions += '<option id="optionOrg-' + org.orgId + '" value="' + org.orgId + '">' + org.orgName + '</option>';
        var arrStruct = [];
        $.each(org.structs, function (j, struct) {
            arrStruct.push({ structId: struct.structId, structName: struct.structName });
        });
        g_listOrgStructs[org.orgId] = arrStruct; // assign value
    });
    $('#OrgList').html(orgOptions);
    // 筛选框
    $('#OrgList').chosen({
        no_results_text: "没有找到",
        allow_single_de: true
    });
    if (jQuery.isEmptyObject(g_listOrgStructs)) {
        alert('该用户可能没有组织');
        return;
    }
    showOrgStructs();
}

function showOrgStructs() {
    var orgId = parseInt($('#OrgList').val());
    var orgStructs = g_listOrgStructs[orgId];
    // 创建当前组织下的结构物列表
    var structOptions = '';
    $.each(orgStructs, function (j, struct) {
        structOptions += '<option id="optionStruct-' + struct.structId + '" value="' + struct.structId + '">' + struct.structName + '</option>';
    });
    // 刷新结构物列表,下面两行必须！
    $('#StructList').removeClass('chzn-done');
    $('#StructList_chzn').remove();
    $('#StructList').html(structOptions);
    // 筛选框,必须！
    $('#StructList').chosen({
        no_results_text: "没有找到",
        allow_single_de: true
    });
    structId = parseInt($('#StructList').val());
    getFactorList(structId, "");
}

function getFactorList(childCode, param) {
    var url = apiurl + '/struct/' + childCode + '/factors?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        dataType: 'json',
        success: function (data, textStatus) {
            if (data == null || data.length == 0) {
                if (param == "") {
                    for (i = 0; i < rowindexarray.length; i++) {
                        var index = rowindexarray[i];
                        $('#FactorList_' + index).children().remove();
                        $('#FactorList_' + index).trigger("liszt:updated");
                    }
                } else {
                    $('#' + param).children().remove();
                    $('#' + param).trigger("liszt:updated");
                }
                alert("该结构物下没有监测因素！");
                return;
            }
            var sb = new StringBuffer();
            sb.append('<option value="' + multiFsctorId + '">' + multiFsctorName + '</option>');
            for (var i = 0; i < data.length; i++) {
                var theme = data[i].children;
                for (var j = 0; j < theme.length; j++) {
                    sb.append('<option value="' + theme[j].factorId + '">' + theme[j].factorName + '</option>');
                }
            }
            if (param == "") {
                for (i = 0; i < rowindexarray.length; i++) {
                    var index = rowindexarray[i];
                    $('#FactorList_' + index).children().remove();
                    $('#FactorList_' + index).trigger("liszt:updated");
                    $('#FactorList_' + index).html(sb.toString());
                    $('#FactorList_' + index).trigger("liszt:updated");
                    $('#FactorList_' + index).chosen({
                        max_selected_options: 1,
                        no_results_text: "没有找到",
                        allow_single_de: true
                    });
                }
            } else {
                $('#' + param).children().remove();
                $('#' + param).trigger("liszt:updated");
                $('#' + param).html(sb.toString());
                $('#' + param).trigger("liszt:updated");
                $('#' + param).chosen({
                    max_selected_options: 1,
                    no_results_text: "没有找到",
                    allow_single_de: true
                });
            }
        },
        error: function (XMLHttpRequest) {
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else if (XMLHttpRequest.status !== 0) {
                alert("获取监测因素列表时发生异常.\r\n" + XMLHttpRequest.status + " : " + XMLHttpRequest.statusText);
            }
        }
    });
}

function getDateType(param) {
    var sb = new StringBuffer();
    sb.append('<option value="1">日报表</option>');
    sb.append('<option value="2">周报表</option>');
    sb.append('<option value="3">月报表</option>');
    sb.append('<option value="4">年报表</option>');
    if (param == "") {
        for (var i = 0; i < rowindexarray.length; i++) {
            var index = rowindexarray[i];
            $('#DateType_' + index).children().remove();
            $('#DateType_' + index).trigger("liszt:updated");

            $('#DateType_' + index).html(sb.toString());
            $('#DateType_' + index).trigger("liszt:updated");

            $('#DateType_' + index).chosen({
                max_selected_options: 1,
                no_results_text: "没有找到",
                allow_single_de: true
            });
        }
    } else {
        $('#' + param).children().remove();
        $('#' + param).trigger("liszt:updated");
        $('#' + param).html(sb.toString());
        $('#' + param).trigger("liszt:updated");
        $('#' + param).chosen({
            max_selected_options: 1,
            no_results_text: "没有找到",
            allow_single_de: true
        });
    }
}

function AddRpt() {
    count++;// 下标
    if (count + 1 > MAX) {
        count = MAX - 1;
        delFlag = false;
        alert("一次最多上传5个报表！");
        return;
    }
    rowindex++;
    rowindexarray.push(rowindex);
    var sb = '';
    //sb += '<div class="clearfix"></div>';
    sb += '<div id="StartRow_' + rowindex.toString() + '"  class="row-fluid">';
    sb += '<div class="form-horizontal">';
    sb += '<div style="margin: 10px 10px 10px 10px; float: left">';
    sb += '<select id="FactorList_' + rowindex.toString() + '" name="FactorList" class="width100  chzn-select" style="width: 150px;" data-size="10" data-placeholder="请选择监测因素"></select>';
    sb += '</div>';
    sb += '<div style="margin: 10px 10px 10px 10px; float: left">';
    sb += '<select id="DateType_' + rowindex.toString() + '" name="DateType" class="width100 chzn-select " style="width: 150px;" data-size="10" data-placeholder="请选择报表类型"></select>';
    sb += '</div>';
    sb += '<div style="margin: 10px 10px 10px 10px; float: left" class=" input-append date">';
    sb += '<input id="RptDate_' + rowindex.toString() + '" class="ui_timepicker " style="width: 120px;" type="text" placeholder="报表生成日期" /><span class="add-on" style="height: 20px;"> <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i> </span>';
    sb += '</div>';
    sb += '<div style="margin: 10px 10px 10px 10px; float: left; width: 230px;">';
    sb += ' <input  style="width: 100%" type="file" id="RptFile_' + rowindex.toString() + '" name="file" size="40" value="浏览" /></div>';
    sb += ' <div style="margin: 10px 10px 10px 10px; float: right;">';
    sb += '<img id="expand_collapse_' + rowindex.toString() + '"  alt="" src="/resource/img/toggle-collapse.png" style="width: 30px; height: 30px;" onclick="DelRpt( ' + rowindex.toString() + ')" />';
    sb += '</div>';
    sb += '</div>';
    sb += '</div>';
    //$('#StartRow_0').append(sb);
    $('#clearfix').append(sb);
    $('.date').datetimepicker({
        format: 'yyyy-MM-dd',
        language: 'pt-BR',
        pickTime: false
    });
    getFactorList(structId, 'FactorList_' + rowindex.toString());
    getDateType('DateType_' + rowindex.toString());
}

function DelRpt(delIndex) {
    // AddRpt 在count++之后的count 是下标
    if (count > 0) {
        count--;// 下标向前移动一个   
        $('#StartRow_' + delIndex).remove();
        rowindexarray.splice(rowindexarray.indexOf(delIndex), 1);
    } else {
        alert("待上传的报表个数异常!");
    }
}

function GetDate(param) {
    var date = $(param).val();
    date = date.replace(new RegExp("/", "g"), "-");
    return date;
}

function CheckUploadResult(count) {
    sumCount = successCount + errorCount;
    if (sumCount == count) {
        $('#ManualUploadModal').modal('hide');
        $('#ReportTable').dataTable().fnDestroy();
        getReportList();
        var sb = new StringBuffer();
        sb.append("上传结果:\r\n 失败个数: " + errorCount + "\t 成功个数: " + successCount);
        alert(sb.toString());
    }
}

function ProcessManualUploadInit() {
    errorCount = 0;
    successCount = 0;
    $('#btn_manual_upload').attr("disabled", "disabled");
    $('#manual_upload_reset').attr("disabled", "disabled");
}

function ProcessManualUpload() {
    var flagFile = true;
    var flagFactor = true;
    var flagType = true;
    var flagDate = true;
    var fileSrc = '';
    var fileExt = '';
    var factorName = '';
    var dateType = null;
    var date = null;
    var rptParam = new Array();
    var fileSet = new Array();
    // 验证待上传报表记录参数的合法性
    for (var i = 0; i < rowindexarray.length; i++) {
        var index = rowindexarray[i];
        var orgId = parseInt($('#OrgList').val());
        var structName = $('#StructList').find("option:selected").text();
        var structId = parseInt($('#StructList').val());
        factorName = $('#FactorList_' + index).find("option:selected").text();
        var factorId = parseInt($('#FactorList_' + index).val());
        dateType = parseInt($('#DateType_' + index).val());
        date = GetDate('#RptDate_' + index);
        fileSrc = $('#RptFile_' + index).val();
        fileExt = fileSrc.substring(fileSrc.lastIndexOf('.'));

        if (factorName == "" || factorId == NaN) {
            flagFactor = false;
            break;
        } else {
            flagFactor = true;
        }

        if (dateType == NaN) {
            flagType = false;
            break;
        } else {
            flagType = true;
        }

        if (date == '' || date == new Date().toString().replace(new RegExp("/", "g"), "-") || date == null) {
            flagDate = false;
            break;
        } else {
            flagDate = true;
        }

        if (fileSrc == "") {
            flagFile = false;
            break;
        } else {
            flagFile = true;
        }
        if (flagFactor && flagDate && flagType && flagFile) {
            fileSet.push($('#RptFile_' + index));
            var param = GetRptParam(orgId, structId, structName, factorId, factorName, dateType, date, fileExt);
            rptParam.push(param);
        }
    }
    if (!flagFactor) {
        alert("请选择监测因素!");
        return;
    } else if (!flagType) {
        alert("请选择报表生成日期类型!");
        return;
    } else if (!flagDate) {
        alert("请选择报表生成日期!");
        $('#RptDate_' + index).focus();
        return;
    } else if (!flagFile) {
        alert("请选择待上传的报表文件!");
        return;
    }
    var loginCount = 0;
    ProcessManualUploadInit();
    for (var j = 0; j < rptParam.length; j++) {
        $.ajaxFileUpload({
            url: '/Support/ManualUploadHandler.ashx?RptParam=' + rptParam[j] + '&MultiFactorId=' + multiFsctorId,
            secureuri: false,
            fileElementId: fileSet[j],
            dataType: 'json',
            ansyc: false,
            success: function (data, textStatus) {
                if (data == 405) {
                    alert("抱歉，没有人工上传权限");
                    return;
                } else if (data == 403) {
                    alert("权限验证出错");
                    logOut();
                    return;
                }
                successCount++;
                CheckUploadResult(rptParam.length);
            },
            error: function (XMLHttpRequest) {
                if (XMLHttpRequest.status == 403) {
                    loginCount++;
                    if (loginCount == 1) {
                        alert("权限验证出错");
                    }
                    logOut();
                } else if (XMLHttpRequest.status !== 0) {
                    errorCount++;
                    CheckUploadResult(rptParam.length);
                }
            }
        });
    }
    $('#btn_manual_upload').removeAttr("disabled");
    $('#manual_upload_reset').removeAttr("disabled");
}

function GetRptParam(orgId, structId, structName, factorId, factorName, dateType, createDate, fileExt) {
    var param = {
        "OrgId": orgId,
        "StructId": structId,
        "StructName": encodeURIComponent(structName),
        "FactorId": factorId,
        "FactorName": encodeURIComponent(factorName),
        "RptType": dateType,
        "Date": createDate,
        "ExtName": fileExt
    };
    return JSON.stringify(param);
}

function RenameReport(rptId) {
    if (g_data == null) {
        alert("获取待管理的报表信息为空!");
        return;
    }
    rename_rptId = rptId;
    var data = {};
    for (var i = 0; i < g_data.length; i++) {
        if (g_data[i].reportId == rptId) {
            data = g_data[i];
            break;
        }
    }
    //清空
    $("#rename_org").val("");
    $("#rename_struct").val("");
    $("#rename_rptType").val("");
    $("#rename_date").val("");
    $("#rename_old").val("");
    $("#rename_new").val("");
    //填原始值
    $("#rename_org").val(data.OrgName);
    $("#rename_struct").val(data.StructName);
    $("#rename_rptType").val(data.DateType);
    $("#rename_date").val(data.ConfirmedDate);
    $("#rename_old").val(data.reportName);    
}

function RenameSumit() {
    var newName = $("#rename_new").val();
    if (newName == "" || newName == "重命名的报表名称") {
        alert("请填写新的报表名称!");
        $("#rename_new").focus();
        return;
    }
    var url = apiurl + '/report/rename/' + rename_rptId + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'post',
        dataType: 'json',
        cache: false,
        data: {
            Name: encodeURIComponent(newName)
        },
        success: function (data) {
            $('#renameRptModal').modal('hide');
            alert("报表重命名成功!");
            $('#ReportTable').dataTable().fnDestroy();
            getReportList();
        },
        error: function (XMLHttpRequest) {
            if (XMLHttpRequest.status == 403) {
                alert("权限验证出错");
                logOut();
            } else if (XMLHttpRequest.status == 400) {
                alert("参数错误");
            } else if (XMLHttpRequest.status == 500) {
                alert("内部异常");
            } else if (XMLHttpRequest.status == 404) {
                alert('url错误');
            } else if (XMLHttpRequest.status == 405) {
                alert('抱歉，没有报表重命名的权限');
            } else if (XMLHttpRequest.status !== 0) {
                alert("报表重命名时发生异常.\r\n" + XMLHttpRequest.status + " : " + XMLHttpRequest.statusText);
            } else {
                $('#renameRptModal').modal('hide');
                alert("报表重命名失败!");
                $('#ReportTable').dataTable().fnDestroy();
                getReportList();
            }
        }
    });
}

/******************************************人工上传报表end************************************/
function checkboxClicked() {
    var total = $('.checkboxes').length;
    var checkedTotal = $('.checkboxes:checked').length;
    if (total === checkedTotal) {
        jQuery.uniform.update(jQuery('.group-checkable').parent('span').attr('class','checked'));
    } else {
        jQuery.uniform.update(jQuery('.group-checkable').prop("checked", false));
    }
}

function initPage() {
    MultiOption("#btnMultiDel");
    MultiOption("#btnMultidown");
    MultiOption("#btnMultiupload");
    //全选与反选
    jQuery('.group-checkable').change(function () {
        var set = jQuery(this).attr("data-set");
        var checked = jQuery(this).prop("checked");
        jQuery(set).each(function () {
            if (checked) {
                $(this).prop("checked", true);
            } else {
                $(this).prop("checked", false);
            }
       });
        jQuery.uniform.update(set);
    });
    $('.close').click(function () {
        var child = $('.close');
        child.parent().parent().fadeOut();
    });
}

function MultiOption(btnMulti) {
    $(btnMulti).click(function () {
        var array = new Array();
        $("input.checkboxes:checked").each(function () {
            array.push(this.value);
        });
        if (array == null || array.length == 0) {
            switch (btnMulti) {
                case "#btnMultiDel":
                    alertTips('请先选中报表再批量删除', 'label-important', "alertTip", 5000);
                    break;
                case "#btnMultidown":
                    alertTips('请先选中报表再批量下载', 'label-important', "alertTip", 5000);
                    break;
                case "#btnMultiupload":
                    alertTips('请先选中报表再批量上传', 'label-important', "alertTip", 5000);
                    break;
            }
        }
        else {
            switch (btnMulti) {
                case "#btnMultiDel":
                    $('#MultideleteModel').modal('show');
                    MultiDel(array);
                    break;
                case "#btnMultidown":
                    MultiDownload(array);
                    break;
                case "#btnMultiupload":
                    $('#MultiuploadModel').modal('show');
                    MultiUpload(array);
                    break;
            }

        }
    });
}

function checkUrlNullOrNot(array) {
    var checkUrlNull = new Array();
    var arrayNew = new Array();
    for (var i = 0; i < g_data.length; i++) {
        for (var j = 0; j < array.length; j++) {
            if (g_data[i].reportId == array[j]) {
                if (((g_data[i].status == "0") && (g_data[i].UnconfirmedUrl == null)) || ((g_data[i].status == "1") && (g_data[i].ConfirmedUrl == null))) {
                    checkUrlNull.push(g_data[i].reportName);
                } else {
                    arrayNew.push(array[j]);
                }
            }
        }
    }
    var data = {
        "check_url": checkUrlNull,
        "array_new": arrayNew
    };
    return data;
}

//批量删除
function MultiDel(array) {
    var checkUrlNull = checkUrlNullOrNot(array).check_url;
    var arrayNew = checkUrlNullOrNot(array).array_new;

    if (checkUrlNull.length > 0) {
        if (checkUrlNull.length == array.length) {
            alert("找不到待删除的文件资源，文件可能已删除或移动!");
            return;
        }
        var st = checkUrlNull.join('  、  ');
        alert("待删除的文件列表中，如下文件可能已删除或移动：" + st);
    }
    if (arrayNew.length > 0) {
        var multiDelRptParams = arrayNew.join('@');
        $('#btn_delete_multi').removeAttr("disabled");
        $('#btn_delete_multi').addClass("red");
        $('#alertMsg_MultiDel').text('确定删除选中报表 ?');
        $('#btn_delete_multi').unbind("click").click(function () {
            $.ajax({
                url: '/Support/MultiDelReport.ashx?MultiDelRptParams=' + encodeURIComponent(multiDelRptParams),
                type: 'post',
                dataType: 'json',
                cache: false,
                success: function (data, textStatus) {
                    if (data == 405) {
                        alert("抱歉，没有批量删除权限");
                        return;
                    } else if (data == 403) {
                        alert("权限验证出错");
                        logOut();
                        return;
                    }
                    $('#MultideleteModel').modal('hide');
                    alert("选中报表删除成功！");
                    $('#ReportTable').dataTable().fnDestroy();
                    getReportList();
                },
                error: function (XMLHttpRequest) {
                    if (XMLHttpRequest.status == 403) {
                        alert("权限验证出错");
                        logOut();
                    } else if (XMLHttpRequest.status !== 0) {
                        $('#MultideleteModel').modal('hide');
                        alert("选中报表批量删除失败，可能删除了部分文件！");
                        $('#ReportTable').dataTable().fnDestroy();
                        getReportList();
                    }
                }
            });
        });
    }
}

//批量下载
function MultiDownload(array) {
    var checkUrlNull = checkUrlNullOrNot(array).check_url;
    var arrayNew = checkUrlNullOrNot(array).array_new;

    if (checkUrlNull.length > 0) {
        if (checkUrlNull.length == array.length) {
            alert("找不到待下载的文件资源，文件可能已删除或移动!");
            return;
        }
        var st = checkUrlNull.join('  、  ');
        alert("待下载的文件列表中，如下文件可能已删除或移动：" + st);
    }
    if (arrayNew.length > 0) {
        var multiDownRptParams = arrayNew.join('@');
        //批量下载处理程序
        var url = '/Support/MultiDownLoadReport.ashx?multiDownRptParams=' + encodeURIComponent(multiDownRptParams);
        window.open(url);
    }
}

//批量上传
function MultiUpload(array) {
    var checkUrlNull = new Array();
    var arrayNew = new Array();
    var url_arrayNew = new Array();
    for (var i = 0; i < g_data.length; i++) {
        for (var j = 0; j < array.length; j++) {
            if (g_data[i].reportId == array[j]) {
                if (((g_data[i].status == "0") && (g_data[i].UnconfirmedUrl == null)) || ((g_data[i].status == "1") && (g_data[i].ConfirmedUrl == null))) {
                    checkUrlNull.push(g_data[i].reportName);
                } else {
                    arrayNew.push(g_data[i].reportId);
                    if (g_data[i].status == "0") {
                        url_arrayNew.push(g_data[i].UnconfirmedUrl);
                    } else  {
                        url_arrayNew.push(g_data[i].ConfirmedUrl);
                    }
                }
            }
        }
    }
    if (checkUrlNull.length > 0) {
        if (checkUrlNull.length == array.length) {
            alert("请先检查原始报表文件是否被移动或删除，暂不允许上传！");
            return;
        }
        var st = checkUrlNull.join('  、  ');
        alert("待确认的报表列表中，如下报表的原始文件不存在：" + st);
    }
    if (arrayNew.length > 0) {
        $('#btn_upload_multi').attr("disabled");
        $('#btn_upload_multi').addClass("grey");
        var multiUploadRptParams = arrayNew.join('@');
        var sb = '';
        for (var m = 0; m < arrayNew.length; m++) {
            var filename = "";
            for (var k = 0; k < g_data.length; k++) {
                if (arrayNew[m] == g_data[k].reportId) {
                    filename = g_data[k].reportName;
                    break;
                }
            }
            sb += '<div class="row-fluid">';
            sb += filename;
            sb += '</div>';
            sb += '<div class="row-fluid"  style=" width: 480px;">';
            sb += ' <input  style="width: 100%" type="file" id="' + arrayNew[m] + '" name="file" size="40" value="浏览" /></div>';
            sb += '</div>';
        }
        $('#MutiInputfile').html("");
        $('#MutiInputfile').html(sb);
        $('#btn_upload_multi').unbind('click').click(function () {
            var count1 = 0;
            var flag1 = false;
            var count2 = 0;
            var flag2 = false;
            var fileSet = new Array();
            for (var n = 0; n < arrayNew.length; n++) {
                var fileSrc = $('#' + arrayNew[n]).val();
                var fileFix = fileSrc.substring(fileSrc.lastIndexOf('.'));
                var originFileFix = url_arrayNew[n].substring(url_arrayNew[n].lastIndexOf('.'));
                if (fileSrc == "") {
                    count1++;
                    flag1 = true;
                } else {
                    if (originFileFix == ".docx" || originFileFix == ".doc") {
                        if ((fileFix != ".docx") && (fileFix != ".doc")) {
                            count2++;
                            flag2 = true;
                            clearInputFile('#' + arrayNew[n]);
                        }
                    } else {
                        if ((fileFix != originFileFix) && (url_arrayNew[n].lastIndexOf('.') != -1)) {
                            count2++;
                            flag2 = true;
                            clearInputFile('#' + arrayNew[n]);
                        }
                    }
                }
                fileSet.push($("#" + arrayNew[n]));
            }
            if (count1 && !flag2) {
                alert("待上传的文件列表中出现没有选择文件的情况!");
            }
            if (count2 && !flag1) {
                alert("上传的文件列表中出现格式错误的情况，请选择.docx、.doc、.xls等格式文件，尽量保证报表文件格式与原文件格式一致！");
            }
            if (flag1 && flag2) {
                alert("上传的文件列表中出现格式错误或没有选择待上传文件的情况！");
            }
            if (!count1 && !count2) {
                $('#btn_upload_multi').removeAttr("disabled");
                $('#btn_upload_multi').addClass("blue");
                $("#multi_loading")
                    .ajaxStart(function () {
                        $(this).show();
                    })
                    .ajaxComplete(function () {
                        $(this).hide();
                    });
                $.ajaxFileUpload({
                    url: '/Support/MultiUploadHandler.ashx?MultiUploadRptParams=' + encodeURIComponent(multiUploadRptParams),
                    secureuri: false,
                    fileElementId: fileSet,
                    dataType: 'json',
                    success: function (result, status) {
                        if (result == 405) {
                            alert("抱歉，没有批量上传报表权限");
                            return;
                        } else if (result == 403) {
                            alert("权限验证出错");
                            logOut();
                            return;
                        }
                        $('#MultiuploadModel').modal('hide');
                        alert("报表批量上传成功！");
                        $('#ReportTable').dataTable().fnDestroy();
                        getReportList();
                    },
                    error: function (XMLHttpRequest) {
                        if (XMLHttpRequest.status == 403) {
                            alert("权限验证出错");
                            logOut();
                        } else if (XMLHttpRequest.status !== 0) {
                            alert("报表批量上传失败！");
                        }
                    }
                });
            }
        });
    }
}

function DownloadReport(rpturl) {
    var fileFullName = decodeURIComponent(rpturl);
    if ((fileFullName == null)) {
        alert("找不到待下载的文件资源，文件可能被移动或删除！");
        return;
    }
    var url = '/Support/DownLoadReport.ashx?fileFullName=' + rpturl;
    window.open(url);
}

function UploadReport(id, rpturl, state) {
    var fileFullName = decodeURIComponent(rpturl);
    if (fileFullName == null) {
        alert("请先检查原始报表文件是否被移动或删除，暂不允许上传！");
        return;
    }
    //初始化按钮的颜色
    $('#btn_upload').attr("disabled");
    $('#btn_upload').addClass("grey");
    //设置模态框标题的名称
    if (state == "0") {
        $('#UploadReportModalTitle').html('上传完整报表');
    } else if (state == "1") {
        $('#UploadReportModalTitle').html('重新上传完整报表');
    } else if (state == "2") {
        $('#UploadReportModalTitle').html('重新上传人工报表');
    }
    clearInputFile('#ReportFile');
    $('#btn_upload').unbind('click').click(function () {
        var str = "ReportFile";
        var fileSrc = $('#' + str).val();
        var fileFix = fileSrc.substring(fileSrc.lastIndexOf('.'));
        var originFileFix = rpturl.substring(rpturl.lastIndexOf('.'));
        var flag = false;
        if (fileSrc == "") {
            flag = false;
            alert("请选择需要上传的报表文件!");
        } else {
            if (originFileFix == ".docx" || originFileFix == ".doc") {
                if ((fileFix != ".docx") && (fileFix != ".doc")) {
                    flag = false;
                    alert("报表文件格式为.docx或.doc,请选择正确的报表文件!");
                    clearInputFile('#ReportFile');
                } else {
                    flag = true;
                }
            } else {
                if ((fileFix != originFileFix) && (rpturl.lastIndexOf('.') != -1)) {
                    flag = false;
                    alert("报表文件格式为" + decodeURIComponent(originFileFix) + ",请选择正确的报表文件!");
                    clearInputFile('#ReportFile');
                } else {
                    flag = true;
                }
            }
        }
        if (!flag) {
            $('#btn_upload').attr("disabled");
            $('#btn_upload').addClass("grey");
        }
        if (flag) {
            $('#btn_upload').removeAttr("disabled");
            $('#btn_upload').addClass("blue");
            var fileSet = new Array();
            fileSet.push($('#ReportFile'));
            $("#loading")
         .ajaxStart(function () {
             $(this).show();
         })
         .ajaxComplete(function () {
             $(this).hide();
         });
            //上传文件到服务器
            $.ajaxFileUpload({
                url: '/Support/UploadHandler.ashx?RptId=' + encodeURIComponent(id) + '&RptUrl=' + encodeURIComponent(fileFullName) + '&RptStatus=' + state,
                secureuri: false,
                fileElementId: fileSet,
                dataType: 'json',
                success: function (result, status) {
                    if (result == 405) {
                        alert("抱歉，没有重新上传权限");
                        return;
                    } else if (result == 403) {
                        alert("权限验证出错");
                        logOut();
                        return;
                    }
                    $('#UploadReportModal').modal('hide');
                    alert("报表上传成功！");
                    $('#ReportTable').dataTable().fnDestroy();
                    getReportList();
                },
                error: function (XMLHttpRequest) {
                    if (XMLHttpRequest.status == 403) {
                        alert("权限验证出错");
                        logOut();
                    } else if (XMLHttpRequest.status !== 0) {
                        alert("报表上传失败！");
                        $('#UploadReportModal').modal('hide');
                    }
                }
            });
        }

    });
}

function DeleteReport(id, name) {
    var fileName = decodeURIComponent(name);
    if (fileName == null) {
        alert("找不到待删除的文件，文件可能被移动或删除！");
        return;
    }
    $('#btn_delete').removeAttr("disabled");
    $('#btn_delete').addClass("red");

    $('#alertMsg_delete').text('确定删除报表"' + decodeURIComponent(name) + '" ?');
    $('#btn_delete').unbind("click").click(function () {
        var url = apiurl + '/report/remove/' + id + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            cache: false,
            statusCode: {
                202: function () {
                    $('#ReportTable').dataTable().fnDestroy();
                    getReportList();
                    $('#deleteRptModal').modal('hide');
                    alert('删除成功');
                },
                403: function () {
                    alert("权限验证出错");
                    logOut();
                },
                400: function () {
                    alert("删除失败，参数错误");
                },
                500: function () {
                    alert("内部异常");
                },
                404: function () {
                    alert('url错误');
                },
                405: function () {
                    alert('抱歉，没有删除报表的权限');
                }
            }
        });
    });
}

function GetManagedReportInfo() {
    var url = apiurl + '/user/' + userId + '/report/managedRpt-list' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data == null) {
                g_data = null;
                return;
            }
            else {
                g_data = data;
            }
        },
        error: function (XMLHttpRequest) {
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else if (XMLHttpRequest.status !== 0) {
                alertTips('获取待管理的报表信息列表失败, 请尝试重新查询', 'label-important', 'alertTip', 5000);
            }
        }
    });
}

function getReportList() {
    jQuery.uniform.update(jQuery('.group-checkable').prop("checked", false));
    GetManagedReportInfo();
    // var url = apiurl + '/user/' + userId + '/report/managedRpt-list' + '?token=' + getCookie("token");
    var url = apiurl + '/user/' + userId + '/report/orderManagedRpt-list' + '?token=' + getCookie("token");
    var count = apiurl + '/user/' + userId + '/report/manualRpt-count' + '?token=' + getCookie("token");
    setTableOption('ReportTable', url, count);
}

function setTableOption(tableId, url, count) {
    $('#' + tableId).dataTable({
        "bAutoWidth": false,
        "aLengthMenu": [
                [10, 25, 50, -1],
                [10, 25, 50, "All"]
        ],

        "iDisplayLength": 10,//每页显示个数
        "bStateSave": true,
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "aoColumns": [
               { "mData": 'checkbox' },
               { "mData": 'reportName' },
               { "mData": 'orgName' },
               { "mData": 'structName' },
               { "mData": 'reportType' },
               { "mData": 'date' },
               { "mData": 'status' },
               { "mData": 'option' }
        ],
        "bSort": false,//是否启动各个字段的排序功能
        "sPaginationType": "full_numbers",//默认翻页样式设置
        //"bFilter": false,//禁用搜索框
        "bProcessing": true,//table数据载入时，是否显示进度提示
        "bServerSide": true,//是否启动服务端数据导入，即要和AjaxSource结合使用
        "sAjaxSource": "/Support/ManageReport.ashx?now=" + Math.random() + "&Url=" + url + "&Url_count=" + count
    });
}

function alertTips(parameters, tipcolor, containerId, timeout) {
    $('#' + containerId).html('<div class="row-fluid" id="alert-tip' + containerId + '" style="display: none;"><span class="label" id="alert-tip-text' + containerId + '" style="margin-top: 5px;">01</span></div>');
    var alerttext = $('#alert-tip-text' + containerId);
    if (alerttext.text() == parameters)
        return;
    $('#alert-tip' + containerId).slideToggle();
    $('#alert-tip-text' + containerId).html(parameters);
    $('#alert-tip-text' + containerId).remove('label-success').remove('label-important');
    $('#alert-tip-text' + containerId).addClass(tipcolor);
    if (timeout != undefined) {
        setTimeout(function () {
            $('#alert-tip' + containerId).slideToggle();
            $('#alert-tip-text' + containerId).html(' ');
        }, timeout);
    }
}

function clearInputFile(inputId) {
    var file = $(inputId);
    file.after(file.clone().val(""));
    file.remove();

}

//字符串缓冲
function StringBuffer() {
    this.data = [];
}

StringBuffer.prototype.append = function () {
    this.data.push(arguments[0]);
    return this;
};

StringBuffer.prototype.toString = function () {
    return this.data.join("");
};


