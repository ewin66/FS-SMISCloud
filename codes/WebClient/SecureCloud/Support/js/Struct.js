/// <reference path="../../resource/img/Topo/cookie.js" />
/// <reference path="../../resource/img/Topo/cookie.js" />
//全局变量
var editFlag = 0; //修改标志 0为新增，1为修改
var fileList = '';
var selectedRow = "";//选中的行
var flag = document.getElementById('HiddenFlag').value;//结构物管理从菜单栏进入
var orgName = document.getElementById('HiddenOrgName').value;
var orgId = document.getElementById('HiddenOrgId').value;

$('#province').change(function () {
    $('#city').children().remove();
    var provinceCode = document.getElementById("province").value;
    getCityList(provinceCode);

});

$('#city').change(function () {
    var cityCode = document.getElementById('city').value;
    getCountryList(cityCode);
});

$(function () {
    $('#SuportSettings').addClass('active');
    $("#structsSupport").addClass('active');

    getStruct();
    ///************************************** 添加结构物 **************************************/
    $('.editor_add').click(function () {
        getProvinceList();  //省市县        
        getStructType();//结构物类型
        editFlag = 0;//新增
        $('#addStructureModalTitle').html('增加结构物');

        //结构物、地址、经纬度、施工单位、项目描述清空
        $('#structName').val('');
        $('#address').val('');
        $('#longitude').val('');
        $('#latitude').val('');
        $('#companyName').val('');
        $('#description').val('');

        setTimeout(function () {
            //结构物类型请选择
            var structTypeOption = document.getElementById('structType');
            structTypeOption.options[0].selected = true;

            //省请选择，市县清空
            var structProvince = document.getElementById('province');
            structProvince.options[0].selected = true;
            //$('#city').children().remove();
            //$('#county').children().remove();
        }, 500)

        //从菜单栏进入，新增时，结构物可选
        if (flag == 1) {
            $("#organizationStructSelect").show();
            $("#organizationStruct").hide();
            getOrganization();
        }
            //从某个组织进入，新增时，结构物不可选
        else {
            $("#organizationStructSelect").hide();
            $("#organizationStruct").show();
            $("#structNameView").val(orgName);
        }
    });

    //保存
    $('#btnSave').click(function () {
        addStructForm();
    });

    //重置表单
    $('#btnReset').click(function () {
        $('#structName').val('');
        $('#address').val('');
        $('#longitude').val('');
        $('#latitude').val('');
        $('#companyName').val('');
        $('#description').val('');

        $('#province').val(1);
        //触发指定事件
        $('#province').trigger("change");
        //结构物类型请选择
        //var structTypeOption = document.getElementById('structType');
        //structTypeOption.options[0].selected = true;
        //省请选择，市县清空
        //var structProvince = document.getElementById('province');
        //structProvince.options[0].selected = true;

        //$('#city').children().remove();
        //$('#county').children().remove();

        $('#btnSave').removeAttr("disabled");
    });
    ///************************************** 查看结构物 **************************************/
    //$('#structTable').on('click', 'a.editor_view', function (e) {
    //    e.preventDefault();
    //    var tr = $(this).parents('tr');

    //    location.assign("SolutionConfig.aspx?structId=" + tr[0].id.substring(14) + "");
    //});

    ///************************************** 修改结构物 **************************************/


    //$('#structTable').on('click', 'a.editor_edit', function (e) {
    //    getStructType();//结构物类型
    //    e.preventDefault();
    //    editFlag = 1;//修改状态
    //    $('#addStructureModalTitle').html('修改结构物');
    //    $("#organizationStructSelect").hide();
    //    $("#organizationStruct").hide();
    //    var tr = $(this).parents('tr');
    //    var selectedRow = tr[0];
    //    editView(selectedRow);

    //});

    /************************************** 删除结构物 **************************************/
    //var struName;
    //var struID;
    //$('#structTable').on('click', 'a.editor_delete', function (e) {
    //    e.preventDefault();
    //    var tr = $(this).parents('tr');
    //    selectedRow = tr[0];
    //    struName = selectedRow.cells[0].innerText;
    //    struID = selectedRow.id.substring(14);
    //    $('#alertMsg').text('确定删除结构物"' + selectedRow.cells[0].innerText + '" ?');
    //});

    //结构物确定删除
    //$('#btnDelete').click(function () {
    //    var url = apiurl + '/struct/remove/' + struID + '?token=' + getCookie('token');
    //    $.ajax({
    //        type: 'post',
    //        url: url,
    //        statusCode: {
    //            202: function () {
    //                getStruct();
    //                alert('删除成功');
    //                $('#deleteStructModal').modal('hide');
    //            },
    //            403: function () {
    //                alert("权限验证出错");
    //                logOut();
    //            },
    //            400: function () {
    //                alert("删除失败,参数错误");
    //            },
    //            500: function () {
    //                alert("内部异常");
    //            },
    //            404: function () {
    //                alert('url错误');
    //            }
    //        }
    //    })
    //})

    /************************************** 方案配置进入 **************************************/
    //$('#structTable').on('click', 'a.editor_structConfig', function (e) {
    //    e.preventDefault();
    //    var tr = $(this).parents('tr');
    //    selectedRow = tr[0];

    //    var structId = "";
    //    var url = "";
    //    if (flag == 1) {
    //        url = apiurl + '/struct/list?token=' + getCookie('token');
    //    }
    //    else {
    //        url = apiurl + '/org/' + orgId + '/structs?token=' + getCookie('token');
    //    }
    //    $.ajax({
    //        url: url,
    //        type: 'get',
    //        success: function (data) {
    //            for (var i = 0; i < data.length; i++) {
    //                if (data[i].structName == selectedRow.cells[0].innerText) {
    //                    structId = data[i].structId;
    //                }
    //            }
    //            location.assign("SolutionConfig.aspx?structId=" + structId);
    //        }
    //    })

    //})

})
function getStruct() {
    //$('#structTable').dataTable().fnDestroy();

    var url = "";
    if (flag == 1) {
        url = apiurl + '/user/' + userId + '/struct/list?token=' + getCookie('token');
    }
    else {
        url = apiurl + '/user/' + userId + '/org/' + orgId + '/structs?token=' + getCookie('token');
    }
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                if (data[i].consCompany == null) {
                    data[i].consCompany = "";
                }
                if (data[i].description == null) {
                    data[i].description = "";
                }
                sb.append("<tr id='StructTbodyTr-" + data[i].structId + "'><td>" + data[i].structName + "</td>");//结构物名称
                sb.append("<td>" + data[i].structType + "</td>");//结构物名称                
                sb.append("<td>" + data[i].projectStatus + "</td>");//项目状态
                sb.append("<td>" + data[i].consCompany + "</td>");//施工单位

                var str = "<td><a href='#addStructModal' class='editor_edit' data-toggle='modal' onclick='Edit_Struct(this," + data[i].structId + ")'>修改</a> | ";
                str += "<a href='#deleteStructModal' class='editor_delete' data-toggle='modal' onclick='Delete_Struct(this," + data[i].structId + ")'>删除</a></td>";
                var strConfig = "<td><a href='#' class='editor_structConfig' onclick='Enter_StructConfig(" + data[i].structId + ")'>进入</a></td>";
                sb.append(str + strConfig);//操作
                if (data[i].structType == "智能张拉" || data[i].structType == "无线采集网站") {
                    sb.append('<td><a href="#" class="editor_uploading" disabled="true">无</a></td>');
                }
                else {
                    sb.append('<td><a href="#uploadingImgModal"  class="editor_uploading" data-toggle="modal">进入</a></td>');
                }

                //sb.append('<td><a class="aSectionConfig" href="SectionConfig.aspx?structId=' + data[i].structId + '&tabIndex=0">进入</a></td>');     
                sb.append('<td><a class="aSectionConfig" href="javascript:;">进入</a></td>');
                sb.append('</tr>');
            }
            $('#StructTbody').html("");
            $('#StructTbody').html(sb.toString());
            Struct_datatable();
        }
    });
}

/**
 * “施工进度/截面配置”点击事件
 */
$('#structTable').on('click', 'a.aSectionConfig', function (e) {
    e.preventDefault();
    var tr = $(this).parents('tr');
    var selectedRow = tr[0];
    var structId = selectedRow.id.split('StructTbodyTr-')[1];
    var structName = selectedRow.cells[0].innerText;
    setCookie('SectionStructName', structName);
    window.location.href = 'SectionConfig.aspx?structId=' + structId + '&tabIndex=0';
});

function Struct_datatable() {

    $('#structTable').dataTable({
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
        "aoColumnDefs": [{
            'bSortable': false,
            'aTargets': [1, 2, 3, 4, 5, 6]
        }]
    });
}

function addStructForm() {
    var organizationName = "";
    var organizationStructNameView = document.getElementById("structNameView").value;//不可选的组织
    if (organizationStructNameView == "" || organizationStructNameView == null) {
        organizationId = document.getElementById("organizationName").value;//可选并选定的组织id号
    }
    //else {
    //    organizationName = organizationStructNameView;
    //}
    var structName = document.getElementById("structName").value;//结构物名称
    var structType = $('#structType').find('option:selected').text();//结构物类型
    var structTypeId = $('#structType').find('option:selected').val();//结构物类型编号
    //省市县
    var provinceCode = $('#province').find('option:selected').val();
    var cityCode = $('#city').find('option:selected').val();
    var countyCode = $('#county').find('option:selected').val();
    var street = document.getElementById("address").value;//地址
    var longitude = document.getElementById("longitude").value;//经度
    var latitude = document.getElementById("latitude").value;//纬度
    var projectStatus = $('#projectStatus').find('option:selected').val();// 项目状态
    var companyName = document.getElementById("companyName").value;//施工单位
    var description = document.getElementById("description").value;//项目描述
    if (structName == null || structName == "" || structName == "结构物名称") {
        //alert('请填写结构物名称');
        $('#structName').focus();
        return false;
    }
    if (structType == null) {
        alert('请选择结构物类型');
        $('#structType').focus();
        return false;
    }
    if (longitude == null || longitude == "" || longitude == "经度") {
        $('#longitude').focus();
        return false;
    }
    if (latitude == null || latitude == "" || latitude == "纬度") {
        $('#latitude').focus();
        return false;
    }
    if (provinceCode == 0) {
        alert('请选择省');
        $('#province').focus();
        return false;
    }
    if (cityCode == 0) {
        alert('请选择市');
        $('#city').focus();
        return false;
    }
    if (countyCode == 0) {
        alert('请选择县区');
        $('#county').focus();
        return false;
    }

    var data = {
        "structName": encodeURIComponent(structName),
        "structTypeId": structTypeId,
        "provinceCode": provinceCode,
        "cityCode": cityCode,
        "countryCode": countyCode,
        "street": street,
        "longitude": longitude,
        "latitude": latitude,
        "projectStatus": projectStatus,
        "consCompany": companyName,
        "description": description,
        "imageName": null

    };
    //添加结构物
    var url = "";
    if (editFlag == 0) {
        if (flag == 1) {
            url = apiurl + '/user/' + userId + '/org/' + organizationId + '/struct/add?token=' + getCookie('token');
        }
        else {
            url = apiurl + '/user/' + userId + '/org/' + orgId + '/struct/add?token=' + getCookie('token');
        }

        $.ajax({
            type: 'post',
            url: url,
            data: data,
            statusCode: {
                202: function () {
                    alert('保存成功,可继续添加');
                    $('#structTable').dataTable().fnDestroy();
                    getStruct();
                },
                403: function () {
                    alert("权限验证出错");
                    logOut();
                },
                400: function () {
                    alert("添加失败,参数错误");
                },
                500: function () {
                    alert("内部异常");
                },
                404: function () {
                    alert('url错误');
                },
                405: function () {
                    alert('抱歉，没有增加结构物的权限');
                }
            }
        });
    }
        //修改组织
    else {
        url = apiurl + '/struct/modify/' + structSelectedId + '?token=' + getCookie('token');
        $.ajax({
            type: 'post',
            url: url,
            data: data,
            statusCode: {
                202: function () {
                    alert('修改成功');
                    $('#addStructModal').modal('hide');
                    $('#structTable').dataTable().fnDestroy();
                    getStruct();

                },
                403: function () {
                    alert("权限验证出错");
                    logOut();
                },
                400: function () {
                    alert("添加失败,参数错误");
                },
                500: function () {
                    alert("内部异常");
                },
                404: function () {
                    alert('url错误');
                },
                405: function () {
                    alert('抱歉，没有修改结构物的权限');
                }
            }
        });
    }
}

//省
function getProvinceList() {
    $('#province').children().remove();
    var urlProvice = apiurl + '/assist/province?token=' + getCookie('token');
    $.ajax({
        url: urlProvice,
        type: 'get',
        success: function (data) {
            var option = '';
            for (var i = 0; i < data.length; i++) {
                option += '<option value=\'' + data[i].id + '\'>' + data[i].value + '</option>';
            }
            //$('#province').html('<option value=\'' + 0 + '\'>请选择</option>' + option);
            $('#province').html(option);
            //$('#city').html('<option value=\'' + data[0].id + '\'>' + data[0].value + '</option>');
            //getCountryList(data[0].id);

            var provinceCode = document.getElementById("province").value;
            getCityList(provinceCode);
        }
    });
}

//市
function getCityList(provinceCode) {
    $('#county').children().remove();
    var urlCity = apiurl + '/assist/city/' + provinceCode + '?token=' + getCookie('token');
    $.ajax({
        url: urlCity,
        type: 'get',
        success: function (data) {
            if (data.length == 0) {
                $('#city').attr("disabled", "disabled");
                return;
            }
            var option = '';
            for (var i = 0; i < data.length; i++) {
                option += '<option value=\'' + data[i].id + '\'>' + data[i].value + '</option>';
            }
            $('#city').html(option);
            var cityLength = $("#city").find("option").length;
            if (cityLength == 1) {
                getCountryList(provinceCode);
            }
            else {
                $('#county').children().remove();
                getCountryList(data[0].id);
            }
        }
    });
}

//县
function getCountryList(cityCode) {
    $('#county').children().remove();
    var urlCountry = apiurl + '/assist/country/' + cityCode + '?token=' + getCookie('token');
    $.ajax({
        url: urlCountry,
        type: 'get',
        success: function (data) {
            var option = '';
            for (var i = 0; i < data.length; i++) {
                option += '<option value=\'' + data[i].id + '\'>' + data[i].value + '</option>';
            }
            $('#county').html(option);
        }
    });
}

function getOrganization() {
    var url = apiurl + '/user/' + userId + '/org/list?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        success: function (data) {
            $('#organizationName').children().remove();
            var option = '';
            for (var i = 0; i < data.length; i++) {
                option += '<option value=\'' + data[i].orgId + '\'>' + data[i].orgName + '</option>';
            }
            $('#organizationName').append(option);
        }
    });
}

//结构物类型
function getStructType() {
    $('#structType').children().remove();
    var url = apiurl + '/struct/type/list?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        async: false,
        success: function (data) {
            var option = '';
            for (var i = 0; i < data.length; i++) {
                option += '<option value=\'' + data[i].structTypeId + '\'>' + data[i].structType + '</option>';
            }
            //$('#structType').html('<option value=\'' + 0 + '\'>请选择</option>' + option);
            $('#structType').html(option);
        }
    });
}

var structSelectedId = "";
function editView(selectedRow, structId) {
    $('#structName').val(selectedRow.cells[0].innerText);//结构物名称
    //结构物类型定位
    var structType = selectedRow.cells[1].innerText;
    setTimeout(function () {
        var structTypeView = document.getElementById('structType');
        for (var i = 0; i < structTypeView.options.length; i++) {
            if (structTypeView.options[i].innerHTML == structType) {
                structTypeView.options[i].selected = true;
                break;
            }
        }
    }, 200)
    //省市县、地址、经纬度
    var url = "";
    if (flag == 1) {
        url = apiurl + '/user/' + userId + '/struct/list?token=' + getCookie('token');
    }
    else {
        url = apiurl + '/user/' + userId + '/org/' + orgId + '/structs?token=' + getCookie('token');
    }
    $.ajax({
        url: url,
        type: 'get',
        async: false,
        cache: false,
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].structId == structId) {
                    structSelectedId = data[i].structId;
                    var provinceName = data[i].province;
                    var cityName = data[i].city;
                    var countryName = data[i].country;
                    var structStreet = data[i].street;
                    var structLongitude = data[i].longitude;
                    var structLatitude = data[i].latitude;
                    var projectStatus = data[i].projectStatus;
                    var consCompany = data[i].consCompany;
                    var description = data[i].description;
                    //省、市、县定位
                    orgProvCityCoun(provinceName, cityName, countryName);


                    $('#address').val(structStreet);//经度
                    $('#longitude').val(structLongitude);//经度
                    $('#latitude').val(structLatitude);//纬度                    
                    var projectStatusPicker = document.getElementById('projectStatus');
                    for (var j = 0; j < projectStatusPicker.options.length; j++) {
                        if (projectStatusPicker.options[j].innerHTML == projectStatus) {
                            projectStatusPicker.options[j].selected = true;
                            break;
                        }
                    }
                    //
                    $('#companyName').val(consCompany);//施工单位
                    $('#description').val(description);//项目描述
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
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
            else {
                alert('url错误');
            }
        }
    })
}

//组织省市县定位
function orgProvCityCoun(provinceName, cityName, countryName) {
    $('#province').children().remove();
    $('#city').children().remove();
    $('#county').children().remove();

    var urlProvice = apiurl + '/assist/province?token=' + getCookie('token');
    $.ajax({
        url: urlProvice,
        type: 'get',
        success: function (data) {
            //省定位
            var option = '';
            for (var i = 0; i < data.length; i++) {
                option += '<option value=\'' + data[i].id + '\'>' + data[i].value + '</option>';
            }
            $('#province').html(option);
            //省定位为空时
            if (provinceName == "" || provinceName == null || provinceName == 0) {
                //$('#province').html('<option value=\'' + 0 + '\'>请选择</option>' + option);
                $('#province').html(option);
                var provinceCodeT = document.getElementById("province").value;
                getCityList(provinceCodeT);
            }
            else {
                var provinceEdit = document.getElementById('province');
                var provinceCode = "";
                for (var i = 0; i < provinceEdit.options.length; i++) {
                    if (provinceEdit.options[i].innerText == provinceName) {
                        provinceEdit.options[i].selected = true;
                        provinceCode = provinceEdit.options[i].value;
                        break;
                    }
                }
                //市定位
                var urlCity = apiurl + '/assist/city/' + provinceCode + '?token=' + getCookie('token');
                $.ajax({
                    url: urlCity,
                    type: 'get',
                    success: function (data) {
                        if (data.length == 0) {
                            $('#city').attr("disabled", "disabled");
                            return;
                        }
                        var option = '';
                        for (var i = 0; i < data.length; i++) {
                            option += '<option value=\'' + data[i].id + '\'>' + data[i].value + '</option>';
                        }
                        $('#city').html(option);

                        var cityEdit = document.getElementById('city');
                        var cityCode = "";
                        for (var i = 0; i < cityEdit.options.length; i++) {
                            if (cityEdit.options[i].innerText == cityName) {
                                cityEdit.options[i].selected = true;
                                cityCode = cityEdit.options[i].value;
                                break;
                            }
                        }

                        //县定位
                        var cityLength = $("#city").find("option").length;
                        if (cityLength == 1) {
                            var urlCountry = apiurl + '/assist/country/' + provinceCode + '?token=' + getCookie('token');
                        }
                        else {
                            $('#county').children().remove();
                            var urlCountry = apiurl + '/assist/country/' + cityCode + '?token=' + getCookie('token');
                        }

                        $.ajax({
                            url: urlCountry,
                            type: 'get',
                            success: function (data) {
                                var option = '';
                                for (var i = 0; i < data.length; i++) {
                                    option += '<option value=\'' + data[i].id + '\'>' + data[i].value + '</option>';
                                }
                                $('#county').html(option);

                                var countryEdit = document.getElementById('county');
                                for (var i = 0; i < countryEdit.options.length; i++) {
                                    if (countryEdit.options[i].innerText == countryName) {
                                        countryEdit.options[i].selected = true;
                                        break;
                                    }
                                }
                            }
                        });
                    }
                });

            }
        }
    });
}

function Edit_Struct(dom, structId) {
    getStructType();//结构物类型

    editFlag = 1;//修改状态
    $('#addStructureModalTitle').html('修改结构物');
    $("#organizationStructSelect").hide();
    $("#organizationStruct").hide();
    var tr = $(dom).parents('tr');
    var selectedRow = tr[0];
    editView(selectedRow, structId);
}

function Delete_Struct(dom, structId) {
    var tr = $(dom).parents('tr');
    selectedRow = tr[0];
    $('#alertMsg').text('确定删除结构物"' + selectedRow.cells[0].innerText + '" ?');

    $('#btnDelete').unbind("click").click(function () {
        var url = apiurl + '/struct/remove/' + structId + '?token=' + getCookie('token');
        $.ajax({
            type: 'post',
            url: url,
            statusCode: {
                202: function () {
                    $('#structTable').dataTable().fnDestroy();
                    getStruct();
                    alert('删除成功');
                    $('#deleteStructModal').modal('hide');
                },
                403: function () {
                    alert("权限验证出错");
                    logOut();
                },
                400: function () {
                    alert("删除失败,参数错误");
                },
                500: function () {
                    alert("内部异常");
                },
                404: function () {
                    alert('url错误');
                },
                405: function () {
                    alert('抱歉，没有删除结构物的权限');
                }
            }
        })
    })
}

function Enter_StructConfig(structId) {

    location.assign("SolutionConfig.aspx?structId=" + structId);

}

/*********************热点配置********************/
var uploadingStruName;
var uploadingStruID;
var uploadingImg;
var uploadingUrl;

$('#structTable').on('click', 'a.editor_uploading', function (e) {
    clearInputFile();

    $('#btnUp').removeAttr("disabled");
    $('#btnUp').addClass("blue");

    e.preventDefault();
    var tr = $(this).parents('tr');
    selectedRow = tr[0];
    uploadingStruName = selectedRow.cells[0].innerText;
    uploadingStruName = uploadingStruName;
    uploadingStruID = selectedRow.id.substring(14);

    setCookie('MainHeapMapStructName', selectedRow.cells[0].innerText);

    var url = apiurl + '/struct/' + uploadingStruID + '/info?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            if (data.length == 0) {
                return;
            } else {
                if (data.imageName != null && data.imageName != "") {
                    document.getElementById("btnUp").value = "重新上传";
                    var bigimg = document.getElementById("imgHotspot");
                    bigimg.src = "/resource/img/Topo/" + data.imageName;
                    var length = data.imageName.split('.').length;
                    var imagename = data.imageName.split('.')[length - 1];

                    if (imagename == 'svg') {
                        uploadingUrl = '/TopoSettingSVG.aspx?structId=' + uploadingStruID + '&imageName=' + data.imageName + '';

                        document.getElementById("btnstationing").disabled = false; //可用 布点
                        $('#btnstationing').addClass('blue');
                    } else if (imagename == 'jpg' || imagename == 'png' || imagename == 'jpeg') {
                        uploadingUrl = '/TopoSetting.aspx?structId=' + uploadingStruID + '&imageName=' + data.imageName + '';

                        document.getElementById("btnstationing").disabled = false; //可用 布点
                        $('#btnstationing').addClass('blue');
                    } else {
                        document.getElementById("btnstationing").disabled = true; //可用 布点
                        $('#btnstationing').removeClass('blue');
                    }
                } else {
                    var bigimg = document.getElementById("imgHotspot");
                    bigimg.src = "";
                    document.getElementById("btnUp").value = "上传";
                    document.getElementById("btnstationing").disabled = true; //不可用 布点
                    $('#btnstationing').removeClass('blue');

                }
            }

        },
        error: function(xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert('获取结构物信息时发生异常.\r\n' + xhr.status + ' : ' + xhr.statusText);
            } 
        }
    });

    $('#fulAchievements').unbind("change").change(function () {
        var file = this.files[0];
        if ((file.size / 1024).toFixed(1) > 1024) {
            alert("请上传小于1M的热点图");
            $('#btnUp').attr("disabled", "disabled");
            $('#btnUp').removeClass("blue");
            return;
        }
        $('#btnUp').removeAttr("disabled");
        $('#btnUp').addClass("blue");
    })

    //上传图片到服务器  保存图片名到数据库
    var uploadingHotName;
    $('#btnUp').unbind('click').click(function () {
        var imgSrc = document.getElementById('fulAchievements').value;
        var imgName = imgSrc.substring(imgSrc.length - 3);
        uploadingHotName = imgName;
        if (imgSrc == "" || (imgName != "jpg" && imgName != "png" && imgName != "svg")) {
            alert("请选择正确图片格式");
            clearInputFile();
        }
        else {
            //执行AJAX上传文件  
            $.ajaxFileUpload({
                url: '/Support/HotspotHandler.ashx?uploadingStruID=' + uploadingStruID + '&uploadingStruName=' + stripscript(uploadingStruName),
                secureuri: false,
                fileElementId: 'fulAchievements',
                dataType: 'json',
                success: function (data, status) {
                    if (data == 405) {
                        alert("抱歉，没有上传权限");
                        return;
                    } else if (data == 403) {
                        alert("权限验证出错");
                        logOut();
                        return;
                    }
                    //alert(data[0]);
                    uploadingHotName = data[0];


                    $('#imgContain').html('');
                    //用动态替换标签和随机数来解决图片缓存的问题
                    var img = '<img id="imgHotspot" style="width: 400px; height:200px;" src="/resource/img/Topo/' + uploadingHotName + '?' + Math.random() + '">';
                    $('#imgContain').html(img);

                    //$('#imgHotspot').attr("src", "");
                    //$('#imgHotspot').attr("src", "/resource/img/Topo/" + data[0]);

                    document.getElementById("btnstationing").disabled = true;//不可用
                    $('#btnstationing').removeClass('blue');

                    var data = {
                        "structName": null,
                        "structTypeId": null,
                        "provinceCode": null,
                        "cityCode": null,
                        "countryCode": null,
                        "street": null,
                        "longitude": null,
                        "latitude": null,
                        "consCompany": null,
                        "decription": null,
                        "imageName": encodeURIComponent(uploadingHotName)
                    };
                    url = apiurl + '/struct/hotspot/modify/' + uploadingStruID + '?token=' + getCookie('token');
                    $.ajax({
                        type: 'post',
                        url: url,
                        data: data,
                        cache: false,
                        statusCode: {
                            202: function () {
                                alert('保存成功');
                                $('#structTable').dataTable().fnDestroy();
                                getStruct();
                                if (uploadingHotName.substring(uploadingHotName.length - 3) == "svg") {
                                    uploadingUrl = '/TopoSettingSVG.aspx?structId=' + uploadingStruID + '&imageName=' + uploadingHotName + '';
                                } else {
                                    uploadingUrl = '/TopoSetting.aspx?structId=' + uploadingStruID + '&imageName=' + uploadingHotName + '';
                                }

                                $('#imgContain').html('');
                                //用动态替换标签和随机数来解决图片缓存的问题
                                var img = '<img id="imgHotspot" style="width: 400px; height:200px;" src="/resource/img/Topo/' + uploadingHotName + '?' + Math.random() + '">';
                                $('#imgContain').html(img);

                                //$('#imgHotspot').attr("src", "");
                                //$('#imgHotspot').attr("src", "/resource/img/Topo/" + uploadingHotName);

                                document.getElementById("btnstationing").disabled = false;//可用 布点
                                $('#btnstationing').addClass('blue');
                            },
                            403: function () {
                                alert("权限验证出错");
                                logOut();
                            },
                            400: function () {
                                alert("添加失败,参数错误");
                            },
                            500: function () {
                                alert("内部异常");
                            },
                            404: function () {
                                alert('url错误');
                            },
                            405: function () {
                                alert('抱歉，没有上传热点图的权限');
                            }
                        }
                    });



                }
            });
        }

    })


    $('#btnstationing').unbind('click').click(function () {
        if (document.getElementById("imgHotspot").src != "" && document.getElementById("imgHotspot").src != null) {
            location.href = uploadingUrl;
        }
        else {
            alert("请上传图片");
        }
    })
})


function clearInputFile() {
    var file = $("#fulAchievements");
    file.after(file.clone().val(""));
    file.remove();
}

//过滤特殊字符和空格
function stripscript(s) {
    var pattern = new RegExp("[+`~!@#$^&*()=|{}':;',\\[\\].<>/?~-！@#￥……&*（）&mdash;—|{}【】‘；：”“'。，、？]");
    var rs = "";
    for (var i = 0; i < s.length; i++) {
        rs = rs + s.substr(i, 1).replace(pattern, '');
    }
    return rs.replace(/(&nbsp;)|\s|\u00a0/g, '');
}