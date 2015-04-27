/**
 * ---------------------------------------------------------------------------------
 * <copyright file="Organization.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2015 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述：组织配置js文件
 *
 * 创建标识：
 *
 * 修改标识：PengLing20150313
 * 修改描述：注册组织时增加"系统简称"项, 并提供对该简称的修改功能
 * </summary>
 * ---------------------------------------------------------------------------------
 */

//全局变量
var editFlag = 0; //修改标志 0为新增，1为修改
var selectedRow = "";
var flag = 0;//
var orgnationId = ""
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
    $("#organizationSupport").addClass('active');
    getOrganization();
    ///************************************** 添加组织 **************************************/
    $('.editor_add').click(function () {
        editFlag = 0;
        $('#addOrganizationModalTitle').html('增加组织');
        $('#addOrganizationForm')[0].reset();
        $('#Organization').show();
        $('#viewOrganization').hide();
        $('#organizationfax').show();
        getProvinceList();
        //setTimeout(function () {
        //    //省请选择，市县清空
        //    var structProvince = document.getElementById('province');
        //    structProvince.options[0].selected = true;
        //    $('#city').children().remove();
        //    $('#county').children().remove();
        //}, 500)

    });

    //$('#btadd').click(function () {
    //    setTimeout(function () {
    //        alert($('#province').val() + ',' + $('#city').val()+ ',' + $('#county').val());
    //    },1000)
        
    //})
  
    $('#btnSave').click(function () {
        addOrganizationForm();
    });

    //重置表单
    $('#btnReset').click(function () {
        $('#organizationName').val('');
        $('#address').val('');
        $('#zipcode').val('');
        $('#phone').val('');
        $('#faxnum').val('');
        $('#website').val('');
        $('#OrgSystemName').val('');
        $('#systemAbbreviation').val('');

        $('#province').val(1);
        //触发指定事件
        $('#province').trigger("change");
        //$('#city').val(1);
        //$('#county').val(2);
        //省请选择，市县清空
        //var structProvince = document.getElementById('province');
        //structProvince.options[0].selected = true;
        //$('#city').children().remove();
        //$('#county').children().remove();

        $('#btnSave').removeAttr("disabled");
    });
    ///************************************** 查看组织 **************************************/
    //$('#organizationTable').on('click', 'a.editor_view', function (e) {
    //    e.preventDefault();
    //    var tr = $(this).parents('tr');
    //    selectedRow = tr[0];
    //    var orgName = selectedRow.cells[0].innerText;
    //    getStruct(selectedRow);

    //});

    ///************************************** 修改组织 **************************************/
    //$('#organizationTable').on('click', 'a.editor_edit', function (e) {
    //    e.preventDefault();
    //    editFlag = 1;//修改状态
    //    $('#addOrganizationModalTitle').html('修改组织');
    //    $('#Organization').hide();
    //    $('#viewOrganization').show();
    //    $('#organizationfax').hide();
    //    var tr = $(this).parents('tr');
    //    selectedRow = tr[0];
    //    editView(selectedRow);
    //});

    /************************************** 删除组织 **************************************/
    //var organizationDeleteName;
    //$('#organizationTable').on('click', 'a.editor_delete', function (e) {
    //    e.preventDefault();
    //    var tr = $(this).parents('tr');
    //    selectedRow = tr[0];
    //    organizationDeleteName = selectedRow.cells[0].innerText;
    //    $('#alertMsg').text('确定删除组织"' + selectedRow.cells[0].innerText + '" ?');
    //});

    

    /************************************** 配置结构物组织 **************************************/
    //$('#organizationTable').on('click', 'a.editor_structConfig', function (e) {
    //    e.preventDefault();

    //    var tr = $(this).parents('tr');
    //    selectedRow = tr[0];
    //    //获取组织Id号
    //    var urlOrganization = apiurl + '/org/list?token=' + getCookie('token');
    //    var orgId = "";
    //    $.ajax({
    //        url: urlOrganization,
    //        type: 'get',
    //        success: function (data) {
    //            for (var i = 0; i < data.length; i++) {
    //                if (data[i].orgName == selectedRow.cells[0].innerText) {
    //                    orgId = data[i].orgId;
    //                    break;
    //                }
    //            }
    //            location.assign("Struct.aspx?orgName=" + selectedRow.cells[0].innerText+"&orgId="+orgId);
    //        }
    //    });       
    //});
});

function getOrganization() {
    //dataTable.fnDestroy()后会改变表格的样式，比如自动适应宽度
    //$('#organizationTable').dataTable().fnDestroy();

    var url = apiurl + '/user/' + userId + '/org/list?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache:false,
        success: function (data) {
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                var orgName = data[i].orgName;
                var country = data[i].country;
                var city = data[i].city;
                var province = data[i].province;
                var street = data[i].street;
                var zipCode = data[i].zipCode;
                var phone = data[i].phone;
                var fax = data[i].fax;
                var website = data[i].website;
                var systemName = data[i].systemName;
                var systemAbbreviation = data[i].systemAbbreviation;
                
                if (orgName == null) {
                    orgName = "";
                }
                if (country == null) {
                    country = "";
                }
                if (city == null) {
                    city = "";
                }
                if (province == null) {
                    province = "";
                }
                if (street == null) {
                    street = "";
                }
                if (zipCode == null) {
                    zipCode = "";
                }
                if (phone == null) {
                    phone = "";
                }
                if (fax == null) {
                    fax = "";
                }
                if (website == null) {
                    website = "";
                }
                if (systemName == null) {
                    systemName == null;
                }
                sb.append("<tr id='org_" + data[i].orgId + "'><td>" + orgName + "</td>");//组织名称
                sb.append("<td>" + province + city + country + "</td>");//省、市、县
                sb.append("<td>" + street + "</td>");//街道地址
                sb.append("<td>" + zipCode + "</td>");//邮政编码
                sb.append("<td>" + phone + "</td>");//联系电话
                sb.append("<td>"+fax+"</td>");
                sb.append("<td>" + website + "</td>");//网址
                sb.append("<td>" + systemName + "</td>");
                sb.append("<td>" + systemAbbreviation + "</td>");
                sb.append("<td><a href='#uploadOrgLogo' data-toggle='modal' onclick='UpLoadOrgLogo("+data[i].orgId+",\""+data[i].orgName+"\")'>进入</a></td>");
                var str = "<td><a href='#viewOrganizationModal' class='editor_view' data-toggle='modal' onclick='View_Organization("+data[i].orgId+",\""+data[i].orgName+"\")' >查看</a> | ";
                str += "<a href='#addOrganiztionModal' class='editor_edit' data-toggle='modal' onclick='Edit_Organization(this,"+data[i].orgId+")'>修改</a> | ";
                str += "<a href='#deleteOrganizationModal' class='editor_delete' data-toggle='modal' onclick='Delete_Organization("+data[i].orgId+",\""+data[i].orgName+"\")'>删除</a></td>";
                str += "<td><a href='/Support/Struct.aspx?orgName=" + data[i].orgName + "&orgId=" + data[i].orgId + "' class='editor_structConfig'>进入</a></td></tr>";
                sb.append(str);//操作               
            }
            $('#organizationTbody').html("");
            $('#organizationTbody').html(sb.toString());
            Organization_datatable();
        }
    });
}

function Organization_datatable() {

    $('#organizationTable').dataTable({
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
        "bStateSave": true,
        "aoColumnDefs": [{
            'bSortable': false,
            'aTargets': [1,2,3,4,5,6,7,8,9,10]
        }]
    });
}

function addOrganizationForm() {
    var organizationName = "";
    var organizationView = document.getElementById("organizationNameview").value;
    if (organizationView == "" || organizationView == null) {
        organizationName = document.getElementById("organizationName").value;
    }
    else {
        organizationName = organizationView;
    }
    //省市县
    var province = $('#province').find('option:selected').val();
    var city = $('#city').find('option:selected').val();
    var county = $('#county').find('option:selected').val();

    var address = document.getElementById("address").value;//地址
    var zipcode = document.getElementById("zipcode").value;//邮政编码
    var phone = document.getElementById("phone").value;//联系电话
    var faxnum = document.getElementById("faxnum").value;//传真
    var website = document.getElementById("website").value;//网站
    var orgSystemName = $('#OrgSystemName').val();
    var systemAbbreviation = $('#systemAbbreviation').val();

    if (organizationName == "" || organizationName == "组织名称" || !/^[a-zA-Z0-9\u4E00-\u9FA5\-]+$/.test(organizationName)) {
        $('#organizationName').focus();
    }
    else if (zipcode != "" && !/^\d{6}$/.test(zipcode)) {
        $('#zipcode').focus();
    }
    else if (phone != "" && !/(^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$)|(^0{0,1}1[3|4|5|6|7|8|9][0-9]{9}$)/.test(phone)) {
        $('#phone').focus();
    }
    else if (faxnum != "" && !/^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/.test(faxnum)) {
        $('#faxnum').focus();
    }
    else if (orgSystemName.replace(/(^\s*)|(\s*$)/g, '') == "" || orgSystemName == '登录后的系统名称') {
        alert('系统名称不能为空!');
        $('#OrgSystemName').focus();
    }
    else if (systemAbbreviation.replace(/(^\s*)|(\s*$)/g, '') == "" || systemAbbreviation == '项目仪表盘中的系统简称') {
        alert('系统简称不能为空!');
        $('#systemAbbreviation').focus();
    }
    else {
        //添加组织
        if (editFlag == 0) {
            var data = {
                "orgName": organizationName,
                "provinceCode": province,
                "cityCode": city,
                "countryCode": county,
                "street": address,
                "zipCode": zipcode,
                "phone": phone,
                "fax": faxnum,
                "website": website,
                "systemName": orgSystemName,
                "systemAbbreviation": systemAbbreviation,
                "logo": ""
            };

            var url = apiurl + '/user/' + userId + '/org/add?token=' + getCookie('token');
            $.ajax({
                type: 'post',
                url: url,
                data: data,
                statusCode: {
                    202: function() {
                        alert('保存成功,可继续添加');
                        $('#organizationTable').dataTable().fnDestroy();
                        getOrganization();

                    },
                    403: function() {
                        alert("权限验证出错");
                        logOut();
                    },
                    400: function() {
                        alert("组织名称重复");
                    },
                    500: function() {
                        alert("内部异常");
                    },
                    404: function() {
                        alert('url错误');
                    },
                    405: function () {
                        alert('抱歉，没有添加组织权限');
                    }
                }
            });
        }
            //修改组织
        else {
            var data = {
                "provinceCode": province,
                "cityCode": city,
                "countryCode": county,
                "street": address,
                "zipCode": zipcode,
                "phone": phone,
                "fax": faxnum,
                "website": website,
                "systemName": orgSystemName,
                "systemAbbreviation": systemAbbreviation,
                "logo": ""
            };

            var url = apiurl + '/org/modify/' + parseInt(orgnationId) + '?token=' + getCookie('token');
            $.ajax({
                type: 'post',
                url: url,
                data: data,
                statusCode: {
                    202: function() {
                        alert('修改成功');
                        $('#organizationTable').dataTable().fnDestroy();
                        getOrganization();
                        $('#addOrganiztionModal').modal('hide');

                    },
                    403: function() {
                        alert("权限验证出错");
                        logOut();
                    },
                    400: function() {
                        alert("组织名称重复");
                    },
                    500: function() {
                        alert("内部异常");
                    },
                    404: function() {
                        alert('url错误');
                    },
                    405: function () {
                        alert('抱歉，没有修改组织权限');
                    }
                }
            });
        }
    }
}

//var orgnationId = "";
//function editView(selectedRow) {
//    var organizationName = $('#organizationNameview').val(selectedRow.cells[0].innerText);
//    var urlOrganization = apiurl + '/org/list?token=' + getCookie('token');
//    $.ajax({
//        url: urlOrganization,
//        type: 'get',
//        success: function (data) {
//            for (var i = 0; i < data.length; i++) {

//                if (data[i].orgName == selectedRow.cells[0].innerText) {
//                    orgnationId = data[i].orgId;
//                    var provinceCode = data[i].provinceCode;
//                    var cityCode = data[i].cityCode;
//                    var countryCode = data[i].countryCode;
//                    //省、市、县定位
//                    orgProvCityCoun(provinceCode, cityCode, countryCode);

//                    $('#organizationNameview').val(selectedRow.cells[0].innerText);
//                    $("#address").val(selectedRow.cells[2].innerText);//地址
//                    $("#zipcode").val(selectedRow.cells[3].innerText);//邮政编码
//                    $("#phone").val(selectedRow.cells[4].innerText);//联系电话
//                    $("#website").val(selectedRow.cells[5].innerText);//网站

//                }
//            }
//        }
//    });        
//}

function getStruct(orgId,orgName) {
   
    var url = apiurl + '/user/' + userId + '/org/' + orgId + '/structs' + '?token=' + getCookie('token');

    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache:false,
        success: function (data) {
            var sb = new StringBuffer();
            if (data.length == 0) {
                $('#viewOrganiztionTbody').children().remove();
                sb.append("<tr><td>" + orgName + "</td>");//组织名称
                sb.append("<td></td>");//结构物名称
                sb.append("<td></td>");//结构物类型
                sb.append("<td></td>");//施工单位
                sb.append("<td></td></tr>");//项目描述
                $('#viewOrganiztionTbody').append(sb.toString());
                return;
            }
            else {
                for (var i = 0; i < data.length; i++) {
                    var consCompany = data[i].consCompany;
                    var description = data[i].description;
                    if (consCompany == null) {
                        consCompany = "";
                    }
                    if (description == null) {
                        description = "";
                    }
                    sb.append("<tr><td>" + orgName + "</td>");//组织名称
                    sb.append("<td>" + data[i].structName + "</td>");//结构物名称
                    sb.append("<td>" + data[i].structType + "</td>");//结构物类型
                    sb.append("<td>" + consCompany + "</td>");//施工单位
                    sb.append("<td>" + description + "</td></tr>");//项目描述
                    $('#viewOrganiztionTbody').children().remove();
                    $('#viewOrganiztionTbody').append(sb.toString());
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
    });
    
    


}
//省
function getProvinceList() {
    $('#province').children().remove();
    $('#city').children().remove();
    $('#county').children().remove();

    var urlProvice = apiurl + '/assist/province?token=' + getCookie('token');
    $.ajax({
        url: urlProvice,
        type: 'get',
        success: function (data) {
            var option = '';
            for (var i = 0; i < data.length; i++) {
                option += '<option value=\'' + data[i].id + '\'>' + data[i].value + '</option>';
            }
           // $('#province').html('<option value=\'' + 0 + '\'>请选择</option>' + option);
           
            //$('#city').html('<option value=\'' + data[0].id + '\'>' + data[0].value + '</option>');
            //getCountryList(data[0].id);

            $('#province').html(option);
            var provinceCode = document.getElementById("province").value;
            getCityList(provinceCode);
        }
    });
}
//市
function getCityList(provinceCode) {
    $('#county').children().remove();
    var urlCity = apiurl + '/assist/city/' + provinceCode+ '?token=' + getCookie('token');
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
    var urlCountry = apiurl + '/assist/country/' + cityCode+ '?token=' + getCookie('token');
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

//组织省市县定位
function orgProvCityCoun(provinceCode, cityCode, countryCode) {
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
            if (provinceCode == "" || provinceCode == null || provinceCode == 0) {
                //$('#province').html('<option value=\'' + 0 + '\'>请选择</option>' + option);
                $('#province').html(option);
                var provinceCodeT = document.getElementById("province").value;
                getCityList(provinceCodeT);
            }
            else {
                var provinceEdit = document.getElementById('province');
                for (var i = 0; i < provinceEdit.options.length; i++) {
                    if (provinceEdit.options[i].value == provinceCode) {
                        provinceEdit.options[i].selected = true;
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
                        for (var i = 0; i < cityEdit.options.length; i++) {
                            if (cityEdit.options[i].value == cityCode) {
                                cityEdit.options[i].selected = true;
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
                                    if (countryEdit.options[i].value == countryCode) {
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


function Edit_Organization(dom,orgId) {
    orgnationId = orgId;

    editFlag = 1;//修改状态
    $('#addOrganizationModalTitle').html('修改组织');
    $('#Organization').hide();
    $('#viewOrganization').show();
    //$('#organizationfax').hide();

    $('#organizationNameview').val($(dom).parent().siblings().eq(0).text());
    $("#address").val($(dom).parent().siblings().eq(2).text());//地址
    $("#zipcode").val($(dom).parent().siblings().eq(3).text());//邮政编码
    $("#phone").val($(dom).parent().siblings().eq(4).text());//联系电话
    $('#faxnum').val($(dom).parent().siblings().eq(5).text());//传真
    $("#website").val($(dom).parent().siblings().eq(6).text());//网站
    $('#OrgSystemName').val($(dom).parent().siblings().eq(7).text());//系统名称
    $('#systemAbbreviation').val($(dom).parent().siblings().eq(8).text()); // 系统简称
    
    var url = apiurl + '/org/' + orgId + '/info' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            if (data == null) {
                return;
            }
            orgProvCityCoun(data.provinceCode,data.cityCode,data.countryCode);
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
            } else if (XMLHttpRequest.status == 405) {
                alert("抱歉，没有修改组织权限");
                //$('#addOrganiztionModal').modal('hide');
                
                $('#btnCloseLogo').click();
            }
            else {
                alert('url错误');
            }
        }
    })
}


function View_Organization(orgId,orgName) {      
    getStruct(orgId,orgName);
}

function Delete_Organization(orgId,orgName) {  
    
    $('#alertMsg').text('确定删除组织"' + orgName + '" ?');
 
    $('#btnDelete').unbind('click').click(function () {
        var url = apiurl + '/org/remove/' + orgId + '?token=' + getCookie('token');
        $.ajax({
            type: 'post',
            url: url,
            statusCode: {
                202: function () {
                    $('#organizationTable').dataTable().fnDestroy();
                    getOrganization();
                    alert('删除成功');
                    $('#deleteOrganizationModal').modal('hide');
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
                    alert('抱歉，没有删除组织的权限');
                }
            }
        });      
    });
}

function UpLoadOrgLogo(orgId, orgName) {
    clearInputFile();//清除input file标签
    $('#btnUp').removeAttr("disabled");
    $('#btnUp').addClass("blue");

    var url = apiurl + '/org/' + orgId + '/info' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            if (data.length == 0) {
                return;
            }
            if (data.logo != null && data.logo != "") {               
                $('#btnUp').val("重新上传");
                var img = '<img src="/resource/img/OrgLogo/' + data.logo + '?'+Math.random()+'" style="width: 160px; height: 40px;" />'
                $('#imgContain').html(img);
                //$('#imgOrgLogo').attr("src", "/resource/img/OrgLogo/" + data.logo+ '?' + Math.random());
            }
            else {
                $('#btnUp').val("上传");
                $('#imgContain').html('');
                //$('#imgOrgLogo').attr("src","");
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
            else if (XMLHttpRequest.status == 405) {
                $('#btnUp').attr("disabled", "disabled");
                $('#btnUp').removeClass("blue");
                $('#fulAchievements').attr("disabled", "disabled");
                alert("抱歉，没有上传Logo的权限");
            }
            else {
                alert('url错误');
            }
        }
    })

    $('#btnUp').unbind('click').click(function () {       
        var imageType = $('#fulAchievements').val();
        imageType = imageType.substring(imageType.length - 3);
        if (imageType == "" || (imageType != "jpg" && imageType != "png")) {
            alert("请选择正确的图片格式");
            clearInputFile();
        }
        else {
            //上传图片
            $.ajaxFileUpload({
                url: '/Support/HotspotHandler.ashx?orgId=' + orgId + '&orgName=' + stripscript(orgName),
                secureuri: false,
                fileElementId: 'fulAchievements',
                dataType: 'json',
                success: function (result, status) {
                    if (result.length == 0) { return; }
                    var logoName = result[0];
                    
                    var url = apiurl + '/org/modify/' + orgId + '?token=' + getCookie('token');
                    $.ajax({
                        type: 'post',
                        url: url,                      
                        cache: false,
                        data: {
                            "provinceCode": null,
                            "cityCode": null,
                            "countryCode": null,
                            "street": null,
                            "zipCode": null,
                            "phone": null,
                            "fax": null,
                            "website": null,
                            "systemName": null,
                            "logo": logoName
                        },
                        statusCode: {
                            202: function () {
                                alert("保存成功");
                                $('#btnUp').val("重新上传");
                                $('#imgContain').html('');
                                //用动态替换标签和随机数来解决图片缓存的问题
                                var img = '<img src="/resource/img/OrgLogo/' + logoName + '?'+Math.random()+'" style="width: 160px; height: 40px;" />'
                                $('#imgContain').html(img);

                                clearInputFile();
                                //$('#imgOrgLogo').attr("src", "");
                                //$('#imgOrgLogo').attr("src", "/resource/img/Topo/" + logoName + '?' + Math.random());                               
                                $('#organizationTable').dataTable().fnDestroy();
                                getOrganization();
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
                                alert('抱歉，没有权限上传Logo');
                            }
                        }
                    })
                }
            })
        }
    })

    $('#fulAchievements').unbind('change').change(function () {
        var file = this.files[0];
        if ((file.size / 1024).toFixed(1) > 1024) {
            alert("请上传小于1M的图片");
            $('#imgContain').html("");
            $('#btnUp').attr("disabled", "disabled");
            $('#btnUp').removeClass("blue");
            return;
        }
        $('#btnUp').removeAttr("disabled");
        $('#btnUp').addClass("blue");
        if (window.FileReader) {
            var reader = new FileReader();
            reader.readAsDataURL(file);
            //监听文件读取结束后事件  
            reader.onloadend = function (e) {               
                showImg(e.target.result, file.fileName);
            };
        }
    })
  
}

function showImg(source) {
    $('#imgContain').html('');   
    var img = '<img src="' + source + '" style="width: 160px; height: 40px;" />'
    $('#imgContain').html(img);     
}

function clearInputFile() {
    var file = $("#fulAchievements");
    file.after(file.clone().val(""));
    file.remove();    
}

//过滤特殊字符和空格
function stripscript(s) {
    var pattern = new RegExp("[+`~!@#$^&*()=|{}':;',\\[\\].<>/?~！@#￥……&*（）&mdash;—|{}【】‘；：”“'。，、？]")
    var rs = "";
    for (var i = 0; i < s.length; i++) {
        rs = rs + s.substr(i, 1).replace(pattern, '');
    }
    return rs.replace(/(&nbsp;)|\s|\u00a0/g, '');
}