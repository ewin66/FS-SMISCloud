//全局变量
var editFlag = 0; //修改标志 0为新增，1为修改
var g_datas = {};
var g_listOrgStructs = {}; // 保存组织及组织下结构物列表

$(".chzn-select").chosen();

$('#DateType_add').change(function () {
    dateTypeChangEvent();
});

function dateTypeChangEvent() {
    var childCode = parseInt($('#DateType_add').val());
    getTemplateListByType(childCode);
    checkGetDataTimeShowHide(childCode);
}

$(function () {

    $('#dataServices').addClass('active');
    $('#ReportConfig').addClass('active');
    getRptConfigList();
    init();
});

function init() {
    var currentUserId = getCookie("userId");
    if (currentUserId == "") {
        alert("获取用户id失败, 请检查浏览器Cookie是否已启用");
        return;
    }
    //增加报表配置
    $('#btnadd').click(function () {
        editFlag = 0;//新增
        $('#btnReset_add').click();
        $('#addRptConfigModalTitle').html('增加报表');
        $('#edit_fieldset').hide();
        $('#IsEnabled_edit_Div').hide();
        $('#add_fieldset').show();
        getOrgStructsListByUser(currentUserId);
        $('#Org').change(function () {
            showOrgStructs();
        });
        dateTypeChangEvent();
    });

    //模态框
    $('.close').click(function () {
        var child = $('.close');
        child.parent().parent().fadeOut();
    });

    $('#btnCancel_add').click(function () {//关闭
        $('#btnReset_add').click();
        $('.close').click();
    });

    $('#btnReset_add').click(function () {//重置

        if (!editFlag) {//增加报表配置
            //组织报表
            $('#Org').find('option:selected').attr("selected", false);
            $('#Org').trigger("liszt:updated");
            //结构物报表
            $('#Struct').find('option:selected').attr("selected", false);
            $('#Struct').trigger("liszt:updated");
        }

        //日期类型
        $('#DateType_add').find('option:selected').attr("selected", false);
        $('#DateType_add').trigger("liszt:updated");
        dateTypeChangEvent();
        //需要确认
        $('#confirm_add').find('option:selected').attr("selected", false);
        $('#confirm_add').trigger("liszt:updated");

        $('#ReportName_add').val('');
        $('#createInterval_add').val('');

        $('#Org').trigger("change");
        $('#btnSave_add').removeAttr("disabled");


    });

    $('#btnSave_add').click(function () {
        SaveAddConfig();
    });
}

function checkGetDataTimeShowHide(childCode) {
    if (childCode == 1) {
        $('#dataDate_add_Div').show();
        initGetDataDate();
    } else {
        $('#dataDate_add_Div').hide();
        $('#dataDate_add').children().remove();
    }
}

function getTemplateListByType(childCode) {
    $('#Template_add').children().remove();
    $('#Template_add').trigger("liszt:updated");
    switch (childCode) {
        case 1:
            getTemplateList("day");
            break;
        case 2:
            getTemplateList("week");
            break;
        case 3:
            getTemplateList("month");
            break;
        case 4:
            getTemplateList("year");
            break;
        default:
            getTemplateList("all");
            break;
    }
}

function initGetDataDate() {
    var sb = new StringBuffer();
    for (var i = 1; i < 25; i++) {
        sb.append('<option value="' + i + '">' + i + '点</option>');
    }
    $('#dataDate_add').html(sb.toString());
    $('#dataDate_add').trigger("liszt:updated");
    // 筛选框
    $('#dataDate_add').chosen({
        max_selected_options: 1,
        no_results_text: "没有找到",
        allow_single_de: true
    });
}

function getTemplateList(option) {
    $('#Template_add').children().remove();
    var url = apiurl + '/template/list/' + option + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        dataType: 'json',
        success: function (data, textStatus) {
            if (data == null || data.length == 0) {
                //alertTips('系统没有提供用来配置的模板', 'label-important', "Template_add_Div", 5000);
                alert('系统没有检索到有效模板,请先检查模板是否已配置正确');
                return;
            }
                var sb = new StringBuffer();
                for (var i = 0; i < data.length; i++) {
                    sb.append('<option value="' + data[i].Id + '">' + data[i].Name + '</option>');
                }

                $('#Template_add').html(sb.toString());
                $('#Template_add').trigger("liszt:updated");
                // 筛选框
                $('#Template_add').chosen({
                    max_selected_options: data.length,
                    no_results_text: "没有找到",
                    allow_single_de: true
                });
        },
        error: function (XMLHttpRequest) {
            //alertTips('报表模板获取失败, 请尝试刷新页面', 'label-important', "Template_add_Div", 5000);
            if (XMLHttpRequest.status == 403) {
                alert("权限验证出错");
                logOut();
            } else if (XMLHttpRequest.status !== 0) {
                alert("获取报表模板列表时发生异常.\r\n" + XMLHttpRequest.status + " : " + XMLHttpRequest.statusText);
            }
        }
    });
}

function SaveAddConfig() {
    var orgId = null;
    var structId = null;
    var status = null;
    var reportName = null;
    if ($('#edit_fieldset').is(":hidden") && $('#IsEnabled_edit_Div').is(":hidden")) {
        orgId = parseInt(chose_get_value('#Org'));
        structId = parseInt(chose_get_value('#Struct'));
        reportName = $('#ReportName_add').val();
    } else {
        status = chose_get_value("#IsEnabled_edit");
        reportName = $('#ReportName_edit').val();
    }
    var isEnabled = true;
    if (status == "1") {
        isEnabled = true;
    } else {
        isEnabled = false;
    }
   
    var createInterval = $('#createInterval_add').val();
    var dateType = parseInt(chose_get_value('#DateType_add'));
    var dataDate = null;
    if ($('#dataDate_add_Div').is(":visible")) {
        dataDate = chose_get_value('#dataDate_add');
    }

    var template = multi_chose_get_value('#Template_add');//多选，以，分割
    var needConfirm = chose_get_value('#confirm_add');
    var confirm = false;
    if (needConfirm == "1") {
        confirm = true;
    } else {
        confirm = false;
    }

    if (reportName == null || reportName == "" || reportName == "报表名称最多50字") {
        $("#ReportName_add").focus();
        return false;
    }
  
    if (createInterval == null || createInterval == "") {
        alert("请设置报表生成周期");
        $('#createInterval_add').focus();
        return false;
    }
    if ($('#Template_add').find('option:selected').text() == "" || $('#Template_add').find('option:selected').text() == null || $('#Template_add').find('option:selected').text() == "请选择") {
        alert("请选择报表模板");
        $('#Template_add').focus();
        return false;
    }
    var flag = check_multi_template('#Template_add');
    if (flag) {
        alert("模板组合类型不一致，请选择文件类型一致的模板组合");
        $('#Template_add').focus();
        return false;
    }

    var checkWord = check_word_template('#Template_add');
    if (checkWord) {
        alert("word模板至多选择一个，不能进行组合");
        $('#Template_add').focus();
        return false;
    }
    if ((dateType == 1) && ($('#dataDate_add').find('option:selected').text() == "" || $('#dataDate_add').find('option:selected').text() == null || $('#dataDate_add').find('option:selected').text() == "请选择")) {
        alert("请选择日数据采样时间点");
        $('#dataDate_add').focus();
        return false;
    }
    var url = null;
    if (!editFlag) {
        url = apiurl + '/reportConfig/add?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            cache: false,
            data: {
                OrgId: orgId,
                StructId: structId,
                ReportName: encodeURIComponent(reportName),
                DateType: dateType,
                CreateInterval: createInterval,
                Templates: template,
                GetDataTime: dataDate,
                NeedConfirmed: confirm,
                IsEnabled: true
            },
            success: function (data) {
                if (data == null ) {
                    return;
                } else {
                    if (!data.interval) {                       
                        alert("请检查报表生成周期格式是否正确！");
                        $('#createInterval_add').focus();
                    }
                    if (data.exist) {                  
                        alert("待配置项已存在，请重新添加！");
                        $('#addRptConfigModal').modal('hide');
                    }
                    if (!data.template) {
                        alert("请选择报表模板！");
                        $('#Template_add').focus();
                    }
                    
                    if (!data.name) {
                        alert("报表名称重复，请重新填写报表名称！");
                        $("#ReportName_add").focus();
                    }
                    
                    if (data.interval && !data.exist && data.template && data.name) {
                        $('#addRptConfigModal').modal('hide');
                        alert("增加报表配置成功！");
                        $('#RptConfigTable').dataTable().fnDestroy();
                        getRptConfigList();
                    }
                }
            },
            error: function (XMLHttpRequest) {
                if (XMLHttpRequest.status == 403) {
                    alert("权限验证出错");
                    logOut();
                }else if (XMLHttpRequest.status == 400) {
                    alert("报表配置添加失败,参数错误");
                } else if (XMLHttpRequest.status == 500) {
                    alert("内部异常");
                }else if(XMLHttpRequest.status == 404){
                    alert('url错误');
                } else if (XMLHttpRequest.status == 405) {
                    alert('抱歉，没有添加报表配置权限');
                } else if (XMLHttpRequest.status !== 0) {
                    alert("新增报表配置时发生异常.\r\n" + XMLHttpRequest.status + " : " + XMLHttpRequest.statusText);
                }
            }
        });
       
    } else {
        var temp = template;
        var urlGetInfo = apiurl + '/reportConfig/info/' + rptConfigId_edit + '?token=' + getCookie("token");
        $.ajax({
            url: urlGetInfo,
            type: 'get',
            dataType: 'json',
            success: function (data, textStatus) {
                if (data == null || data.length == 0) {
                    return;
                }
                else {
                    data.Org[0].id == null ? orgId = null : orgId = data.Org[0].id;
                    data.Struct[0].id == null ? structId = null : structId = data.Struct[0].id;
                    url = apiurl + '/reportConfig/modify-info/' + rptConfigId_edit + '?token=' + getCookie("token");
                    $.ajax({
                        url: url,
                        type: 'post',
                        dataType: 'json',
                        cache: false,
                        data: {
                            OrgId: orgId,
                            StructId: structId,
                            ReportName: encodeURIComponent(reportName),
                            DateType: dateType,
                            CreateInterval: createInterval,
                            Templates: temp,
                            GetDataTime: dataDate,
                            NeedConfirmed: confirm,
                            IsEnabled: isEnabled
                        },
                        success: function (data) {
                            if (data == null) {
                                return;
                            } else {
                                if (!data.interval) {
                                    alert("请检查报表生成周期格式是否正确！");
                                    $('#createInterval_add').focus();
                                }                                
                                if (!data.template) {
                                    alert("请选择报表模板！");
                                    $('#Template_add').focus();
                                }
                                if (!data.exist) {
                                    alert("待修改的配置项找不到");
                                    $('#addRptConfigModal').modal('hide');
                                }
                               
                                if (data.interval && data.template && data.exist) {
                                    $('#addRptConfigModal').modal('hide');
                                    alert("报表配置修改成功");
                                    $('#RptConfigTable').dataTable().fnDestroy();
                                    getRptConfigList();
                                }
                                
                            }
                        },
                        error: function (XMLHttpRequest) {
                            if (XMLHttpRequest.status == 403) {
                                alert("权限验证出错");
                                logOut();                            
                            } else if (XMLHttpRequest.status == 400) {
                                alert("修改失败,参数错误");
                            } else if (XMLHttpRequest.status == 500) {
                                alert("内部异常");
                            } else if (XMLHttpRequest.status == 404) {
                                alert('url错误');
                            } else if (XMLHttpRequest.status == 405) {
                                alert('抱歉，没有修改报表配置权限');
                            } else if (XMLHttpRequest.status !== 0) {
                                alert("修改报表配置时发生异常.\r\n" + XMLHttpRequest.status + " : " + XMLHttpRequest.statusText);
                            }
                        }                       
                    });
                }
            },
            error: function (XMLHttpRequest) {
                if (XMLHttpRequest.status == 403) {
                    alert("登录超时,请重新登录");
                    logOut();
                } else if (XMLHttpRequest.status !== 0) {
                    alert("获取报表配置原始信息时发生异常.\r\n" + XMLHttpRequest.status + " : " + XMLHttpRequest.statusText);
                }
            }
        });

    }

}

function getRptConfigList() {
    var url = apiurl + '/user/' + userId + '/reportConfig/list' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            if (data == null) {
                $('#RptConfigTbody').html("");
                RptConfig_datatable('#RptConfigTable');
                return;
            }
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                g_datas[data[i].Id] = data[i];
                sb.append("<tr id='RptConfigTbodyTr-" + data[i].Id + "'>");
                data[i].Org == null ? sb.append('<td></td>') : sb.append('<td>' + data[i].Org[0].name + '</td>');
                data[i].Struct == null ? sb.append('<td></td>') : sb.append('<td>' + data[i].Struct[0].name + '</td>');

                sb.append("<td>" + data[i].ReportName + "</td>");
                sb.append("<td>" + data[i].DateTypeLabel + "</td>");

                sb.append("<td>" + data[i].CreateInterval + "</td>");

                sb.append('<td>');
                if (data[i].Template != null) {
                    for (var k = 0; k < data[i].Template.length; k++) {
                        sb.append(data[i].Template[k].name);
                        if (k != data[i].Template.length - 1) {
                            sb.append('，');
                        }
                    }
                }
                sb.append('</td>');
                data[i].GetDataTime == null ? sb.append("<td>&nbsp;</td>") : sb.append("<td>" + data[i].GetDataTime + "</td>");;
                data[i].NeedConfirmed == true ? sb.append('<td>是</td>') : sb.append('<td>否</td>');
                data[i].IsEnabled == true ? sb.append('<td>启用</td>') : sb.append('<td>禁用</td>');
                var str = "<td><a href='#addRptConfigModal' class='editor_edit' data-toggle='modal' onclick='EditRptConfig(this," + data[i].Id + ")'>修改</a> | ";
                str += "<a href='#deleteRptConfigModal' class='editor_delete' data-toggle='modal' onclick='DeleteRptConfig(this," + data[i].Id + ")'>删除</a></td>";
                sb.append(str);
            }
            $('#RptConfigTbody').html("");
            $('#RptConfigTbody').html(sb.toString());
            RptConfig_datatable('#RptConfigTable');
        },
        error: function (XMLHttpRequest) {
            if (XMLHttpRequest.status == 403) {
                alert("权限验证出错");
                logOut();
            }
            else if (XMLHttpRequest.status == 400) {
                alert("参数错误");
            }
            else if (XMLHttpRequest.status == 500) {
                alert("内部异常");
            }
            else if (XMLHttpRequest.status == 404) {
                alert('url错误');
            } else if (XMLHttpRequest.status !== 0) {
                alert("获取报表配置列表时发生异常.\r\n" + XMLHttpRequest.status + " : " + XMLHttpRequest.statusText);
            }
        }
    });
}

function DeleteRptConfig(dom, rptConfigId) {
    $('#alertMsg_delete').text('确定删除选中的配置?');
    $('#btnDelete').unbind("click").click(function () {
        var url = apiurl + '/reportConfig/remove/' + rptConfigId + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            cache: false,
            statusCode: {
                202: function () {
                    $('#deleteRptConfigModal').modal('hide');
                    alert('删除成功');
                    $('#RptConfigTable').dataTable().fnDestroy();
                    getRptConfigList();
                },
                403: function () {
                    alert("权限验证出错");
                    logOut();
                },
                400: function () {
                    alert("配置不存在");
                },
                500: function () {
                    alert("内部异常");
                },
                404: function () {
                    alert('url错误');
                },
                405: function () {
                    alert('抱歉，没有删除报表配置的权限');
                }
            }
        });
    });
}

function RptConfig_datatable(param) {
    $(param).dataTable({
        "aLengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, "All"]
        ],
        // set the initial value
        "iDisplayLength": 10,
        "sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
        "sPaginationType": "bootstrap",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        //状态保存，使用了翻页或者改变了每页显示数据数量，会保存在cookie中，下回访问时会显示上一次关闭页面时的内容。
        "bStateSave": true,
        "bDestroy": true,
        "bAutoWidth": false,  //自适应宽度
        "bSort": true //是否支持排序功能
    });
}

function InitTemplateWhenEdit(option, template) {
    $('#Template_add').children().remove();
    var url = apiurl + '/template/list/' + option + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        dataType: 'json',
        success: function (data, textStatus) {
            if (data == null || data.length == 0) {
                alertTips('系统没有提供用来配置的模板', 'label-important', "Template_add_Div", 5000);
                return;
            }
                var sb = new StringBuffer();
                for (var i = 0; i < data.length; i++) {
                    if (template != null) {
                        for (var j = 0; j < template.length; j++) {
                            if (data[i].Id == template[j].id) {
                                sb.append('<option selected="selected" value="' + data[i].Id + '">' + data[i].Name + '</option>');
                            }
                        }
                    }
                    sb.append('<option value="' + data[i].Id + '">' + data[i].Name + '</option>');
                }

                $('#Template_add').html(sb.toString());
                $('#Template_add').trigger("liszt:updated");
                // 筛选框
                $('#Template_add').chosen({
                    max_selected_options: data.length,
                    no_results_text: "没有找到",
                    allow_single_de: true
                });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            }else if (XMLHttpRequest.status !== 0) {
                alert("获取报表模板时发生异常.\r\n" + XMLHttpRequest.status + " : " + XMLHttpRequest.statusText);
            }
        }
    });
}

var rptConfigId_edit = null;//待修改的报表配置编号
function EditRptConfig(dom, rptConfigId) {
    rptConfigId_edit = rptConfigId;
    editFlag = 1;//修改状态
    $('#addRptConfigModalTitle').html('修改报表配置');
    $('#edit_fieldset').show();
    $('#IsEnabled_edit_Div').show();
    $('#add_fieldset').hide();
    //先清空
    $('#ReportName_edit').val('');
    $('#createInterval_add').val('');
    //获取待需改的原始值
    var di = g_datas[rptConfigId];
    var orgEdit = di.Org[0].name;
    var structEdit = di.Struct[0].name;
    var reportName = di.ReportName;
    var createInterval = di.CreateInterval;
    var dateType = di.DateType;
    var needConfirmed = di.NeedConfirmed;
    var isEnabled = di.IsEnabled;
    var getDataTime = di.GetDataTime;
    var template = di.Template;

    //填原始值
    $("#org_edit").val(orgEdit);
    $("#struct_edit").val(structEdit);
    $("#ReportName_edit").val(reportName);
    $("#createInterval_add").val(createInterval);

    //下拉框的原始值定位
    $("#DateType_add [value='" + dateType + "']").attr('selected', 'selected');
    $("#DateType_add").trigger("liszt:updated");
    var childCode = parseInt(dateType);
    checkGetDataTimeShowHide(childCode);
    if (getDataTime != null) {
        $("#dataDate_add [value='" + getDataTime + "']").attr('selected', 'selected');
        $("#dataDate_add").trigger("liszt:updated");
    }
    var option = "";
    switch (childCode) {
        case 1:
            option = "day";
            break;
        case 2:
            option = "week";
            break;
        case 3:
            option = "month";
            break;
        case 4:
            option = "year";
            break;
    }
    InitTemplateWhenEdit(option, template);

    if (needConfirmed) {
        $("#confirm_add [value='1']").attr('selected', 'selected');
        $("#confirm_add").trigger("liszt:updated");
    } else {
        $("#confirm_add [value='0']").attr('selected', 'selected');
        $("#confirm_add").trigger("liszt:updated");
    }

    if (isEnabled) {
        $("#IsEnabled_edit [value='1']").attr('selected', 'selected');
        $("#IsEnabled_edit").trigger("liszt:updated");
    } else {
        $("#IsEnabled_edit [value='0']").attr('selected', 'selected');
        $("#IsEnabled_edit").trigger("liszt:updated");
    }
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
            if (data.length == 0) {
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
    $('#Org').html(orgOptions);
    // 筛选框
    $('#Org').chosen({
        no_results_text: "没有找到",
        allow_single_de: true
    });

    showOrgStructs();
}

function showOrgStructs() {
    if (jQuery.isEmptyObject(g_listOrgStructs)) {
        alert('该用户可能没有组织');
        return;
    }

    var org = $('#Org').find('option:selected')[0];
    var orgId = parseInt(org.id.split('optionOrg-')[1]);
    var orgStructs = g_listOrgStructs[orgId];
    // 创建当前组织下的结构物列表
    var structOptions = '';
    $.each(orgStructs, function (j, struct) {
        structOptions += '<option id="optionStruct-' + struct.structId + '" value="' + struct.structId + '">' + struct.structName + '</option>';
    });
    // 刷新结构物列表,下面两行必须！
    $('#Struct').removeClass('chzn-done');
    $('#Struct_chzn').remove();
    $('#Struct').html(structOptions);
    // 筛选框,必须！
    $('#Struct').chosen({
        no_results_text: "没有找到",
        allow_single_de: true
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

//字符串缓冲
function StringBuffer() {
    this.data = [];
}

StringBuffer.prototype.append = function() {
    this.data.push(arguments[0]);
    return this;
};

StringBuffer.prototype.toString = function() {
    return this.data.join("");
};

//select 数据同步
function chose_get_ini(select) {
    $(select).chosen().change(function () { $(select).trigger("liszt:updated"); });
}

//单选select 数据初始化
function chose_set_ini(select, value) {
    $(select).attr('value', value);
    $(select).trigger("liszt:updated");
}

//多选select 数据初始化
function chose_mult_set_ini(select, values) {
    var arr = values.split(',');
    var length = arr.length;
    var value = '';
    for (var i = 0; i < length; i++) {
        value = arr[i];
        $(select + " [value='" + value + "']").attr('selected', 'selected');
    }
    $(select).trigger("liszt:updated");
}

//单选select value获取
function chose_get_value(select) {
    return $(select).val();
}

//获取多选列表的value
function multi_chose_get_value(select) {
    var selectValues = new Array();
    var length = $(select + " option:selected").length;
    for (var i = 0; i < length; i++) {
        var value = $(select + " option:selected")[i].value;
        selectValues.push(value);
    }
    return selectValues.join(',');
}

//验证多选模板的合法性
function check_multi_template(select) {
    var selectTexts = new Array();
    var length = $(select + " option:selected").length;
    for (var i = 0; i < length; i++) {
        var text = $(select + " option:selected")[i].text;
        selectTexts.push(text);
    }
    var temp = selectTexts[0].substring(selectTexts[0].indexOf('.'));
    var flag = false;
    for (var j = 0; j < selectTexts.length; j++) {
        var fix = selectTexts[j].substring(selectTexts[j].indexOf('.'));
        if (fix == temp) {
            flag = false;
            continue;
        } else {
            flag = true;
            break;
        }
    }
    return flag;
}
//验证word模板只能选一个，不能组合
function check_word_template(select) {
    var selectTexts = new Array();
    var length = $(select + " option:selected").length;
    for (var i = 0; i < length; i++) {
        var text = $(select + " option:selected")[i].text;
        selectTexts.push(text);
    }
    var fix = selectTexts[0].substring(selectTexts[0].indexOf('.'));
    var flag = false;
    if (length > 1 && (fix == ".docx" || fix == ".doc")) {
        flag = true;
    }
    return flag;
}

