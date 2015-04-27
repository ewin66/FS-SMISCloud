var structSelectedOld = "";
var struct_editSelectedOld = "";

$(function () {
    $('#SuportSettings').addClass('active');
    $('#UserManger').addClass('active');
    $('.selectpicker').selectpicker();

    GetUserList();

    $('#btnadd').click(function () {
        structSelectedOld = "";                              
        $('#orgCheckAll').parent('span').removeClass("checked"); 
        $('#structCheckAll').parent('span').removeClass("checked");
        var array = new Array();
        $("input.checkbox:checked").each(function () {
            array.push(this.value);
        })
        for (var i = 0; i < array.length; i++) {
            if (array[i] == "0") {
                $('#orgCheckAll').click();
            } else if (array[i] == "1") {
                $('#structCheckAll').click();
            }
        }//未刷新，重新设置checkbox未选中状态
        GetAddUserList();
    })

    $('#btnCancel_add').click(function () {
        $('.controls .label-success').hide();
        $('.controls .label-important').hide();
        $('#btnReset_add').click();
    })

    $('#btnReset_add').click(function () {
        $('.controls .label-success').hide();
        $('.controls .label-important').hide();

        $('#addUserForm')[0].reset();
        $('#UserRole').selectpicker('val', 5);
        $('#beOrgNameList').find('option:selected').attr("selected", false);
        $('#beOrgNameList').selectpicker('refresh');
        $('#OrgNameList').find('option:selected').attr("selected", false);
        $('#OrgNameList').selectpicker('refresh');
        $('#StructNameList').html("");
        $('#StructNameList').selectpicker('refresh');
    })

    $('#btnReset_edit').click(function () {
        $('#Password_edit').val('');
        $('#ConfirmPassword_edit').val('');
        $('#UserEmail_edit').val('');
        $('#UserPhone_edit').val('');
        $('#UserSystemName_edit').val('');

        $('#UserRole_edit').selectpicker('val', 5);
        $('#OrgNameList_edit').find('option:selected').attr("selected", false);
        $('#OrgNameList_edit').selectpicker('refresh');
        $('#StructNameList_edit').html("");
        $('#StructNameList_edit').selectpicker('refresh');
    })

    $('#btnSave').click(function () {
        SaveUser();
    })
    $('#OrgNameList').change(function () {
        $('#orgCheckAll').parent('span').removeClass("checked");
        orgChange();
    });
    function orgChange() {
        var orgIdArray = $('#OrgNameList').val();
        if (orgIdArray != null) {
            var sb = new StringBuffer();
            //var defaultStructId;
            var url = apiurl + '/user/' + userId + '/org/' + orgIdArray + '/struct-list' + '?token=' + getCookie("token");
            $.ajax({
                url: url,
                type: 'get',
                dataType: 'json',
                async: false,
                cache: false,
                success: function (data) {
                    if (data == null || data.length == 0) {
                        alert('该组织待配置结构物，请重新选择');
                        return;
                    }
                    for (var j = 0; j < data.length; j++) {
                        sb.append('<option value="' + data[j].structId + '">' + data[j].structName + '</option>');
                    }
                    $('#StructNameList').html(sb.toString());
                    $('#StructNameList').selectpicker('refresh');

                    var structSelectedOldArray = structSelectedOld.split(",");
                    var o = document.getElementById("StructNameList");
                    if (structSelectedOldArray != "") {
                        //for (var m = 0; m < o.length; m++) {
                        //    for (var k = 0; k < structSelectedOldArray.length; k++) {
                        //        if (structSelectedOldArray[k] == o.options[m].value) {
                        //            o.options[m].selected = true;
                        //        }
                        //    }
                        //}
                        $('#StructNameList').selectpicker('val', structSelectedOldArray);
                        $('#StructNameList').selectpicker('refresh');
                    }

                    //defaultStructId=data[0].structId;
                },
                error: function () {
                    alert("获取结构物列表失败");
                }
            });

            //var a = $('#StructNameList').val();
            //$('#StructNameList').selectpicker('val', defaultStructId);
        } else {
            $('#StructNameList').html("");
            $('#StructNameList').selectpicker('refresh');
        }
    }

    $('#OrgNameList_edit').change(function () {
        orgEditChange();
    })
    function orgEditChange() {
        var orgIdArray = $("#OrgNameList_edit").val();
        if (orgIdArray != null) {
            var sb = new StringBuffer();

            var url = apiurl + '/user/' + userId + '/org/' + orgIdArray + '/struct-list' + '?token=' + getCookie("token");
            $.ajax({
                url: url,
                type: 'get',
                dataType: 'json',
                async: false,
                cache: false,
                success: function (data) {
                    if (data == null || data.length == 0) {
                        alert('该组织待配置结构物，请重新选择');
                        return;
                    }
                    for (var j = 0; j < data.length; j++) {
                        sb.append('<option value="' + data[j].structId + '">' + data[j].structName + '</option>');
                    }
                    $('#StructNameList_edit').html(sb.toString());
                    $('#StructNameList_edit').selectpicker('refresh');

                    var struct_editSelectedOldArray = struct_editSelectedOld.split(",");
                    var o = document.getElementById("StructNameList_edit");
                    if (struct_editSelectedOldArray != "") {
                        for (var m = 0; m < o.length; m++) {
                            for (var k = 0; k < struct_editSelectedOldArray.length; k++) {
                                if (struct_editSelectedOldArray[k] == o.options[m].value) {
                                    o.options[m].selected = true;
                                }
                            }
                        }
                        $('#StructNameList_edit').selectpicker('refresh');
                    }

                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    if (XMLHttpRequest.status == 403) {
                        alert("权限验证出错");
                        logOut();
                    } else if (XMLHttpRequest.status == 400) {
                        alert("参数错误");
                    } else if (XMLHttpRequest.status == 500) {
                        alert("内部异常");
                    } else {
                        alert('url错误');
                    }
                }
            });
        }
        else {
            $('#StructNameList_edit').html("");
            $('#StructNameList_edit').selectpicker('refresh');
        }
    }
    $('#StructNameList').change(function () {
        if ($('#StructNameList').val() != null) {
            structSelectedOld = $('#StructNameList').val().toString();
        }
        $('#structCheckAll').parent('span').removeClass("checked");
    });

    $('#StructNameList_edit').change(function () {
        if ($('#StructNameList_edit').val() != null) {
            struct_editSelectedOld = $('#StructNameList_edit').val().toString();
        }
    });

    $('#orgCheckAll').change(function () {
        var array = new Array();
        $("input.checkbox:checked").each(function () {
            array.push(this.value);
        })
        var flag = 0;
        for (var i = 0; i < array.length; i++) {
            if (array[i] == "0") {
                flag = 1;
                var o = document.getElementById("OrgNameList");
                for (var m = 0; m < o.length; m++) {
                    o.options[m].selected = true;
                }
                $('#OrgNameList').selectpicker('refresh');
                orgChange();

                var o = document.getElementById("StructNameList");
                for (var m = 0; m < o.length; m++) {
                    o.options[m].selected = false;
                }
                $('#StructNameList').selectpicker('refresh');
            } 
        }
        if (flag == 0) {
            structSelectedOld = "";//取消结构物列表之前记录的选项
            $('#structCheckAll').parent('span').removeClass("checked");
            var o = document.getElementById("OrgNameList");
            for (var m = 0; m < o.length; m++) {
                o.options[m].selected = false;
            }
            $('#OrgNameList').selectpicker('refresh');
            orgChange();
        }
    });

    $('#structCheckAll').change(function () {
        var array = new Array();
        $("input.checkbox:checked").each(function () {
            array.push(this.value);
        })
        var flag = 0;
        for (var i = 0; i < array.length; i++) {
            if (array[i] == "1") {
                if (document.getElementById("StructNameList").length > 0) {
                    flag = 1;
                    var o = document.getElementById("StructNameList");
                    for (var m = 0; m < o.length; m++) {
                        o.options[m].selected = true;
                    }
                    $('#StructNameList').selectpicker('refresh');
                    if ($('#StructNameList').val() != null) {
                        structSelectedOld = $('#StructNameList').val().toString();
                    }
                } else {
                    $('#structCheckAll').click();
                    alert("无结构物可选");
                }
            } 
        }
        if (flag == 0) {
            var o = document.getElementById("StructNameList");
            for (var m = 0; m < o.length; m++) {
                o.options[m].selected = false;
            }
            $('#StructNameList').selectpicker('refresh');
            structSelectedOld = "";
        }
    });
})

function GetUserList() {
    $('#userTable').dataTable().fnDestroy();
    var url = apiurl + '/user/' + userId + '/list' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                return;
            }
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                sb.append('<tr><td>' + data[i].userName + '</td>');
                //sb.append('<td>' + data[i].password + '</td>');
                sb.append('<td>' + data[i].roleCode + '</td>');               
                data[i].email == null ? sb.append('<td></td>') : sb.append('<td>' + data[i].email + '</td>');
                data[i].phone == null ? sb.append('<td></td>') : sb.append('<td>' + data[i].phone + '</td>');               
                sb.append('<td>');
                if (data[i].orgs != null) {
                    for (var j = 0; j < data[i].orgs.length; j++) {
                        sb.append(data[i].orgs[j].name);
                        if (j != data[i].orgs.length - 1) {
                            sb.append('，');
                        }
                    }
                }
                sb.append('</td>');
                sb.append('<td>');
                if (data[i].structs != null) {
                    for (var k = 0; k < data[i].structs.length; k++) {
                        sb.append(data[i].structs[k].name);
                        if (k != data[i].structs.length - 1) {
                            sb.append('，');
                        }
                    }
                }
                sb.append('</td>');
                //data[i].systemName == null ? sb.append('<td></td>') : sb.append('<td>' + data[i].systemName + '</td>');               
                var str = '<td><a href="#editUser" class="editor_edit" data-toggle="modal" onclick="getEditUserList(this,' + userId + ',' + data[i].userId + ')">修改</a> | ';//修改查找登录用户(userId)能够配置的组织、结构物,配置为列表中对应用户
                str += '<a href="#deleteUser" class="editor_delete" data-toggle="modal" onclick="getDeleteUserList(this,'+data[i].userId+')">删除</a></td></tr>';//删除列表中对应用户
                sb.append(str);
            }
            $('#userTbody').html(sb.toString());
            UserList_To_datatable();
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

function GetAddUserList() {
    $('.controls .label-success').hide();
    $('.controls .label-important').hide();

    var url = apiurl + '/role/list' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                return;
            }
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                sb.append('<option value="' + data[i].roleId + '">' + data[i].roleName + '</option>');
            }
            $('#UserRole').html(sb.toString());
            $('#UserRole').selectpicker('refresh');
            //默认是普通用户
            $('#UserRole').selectpicker('val', 5);
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
    var url2 = apiurl + '/user/' + userId + '/org/list' + '?token=' + getCookie("token");
    //var url2 = apiurl + '/userManage/' + userId + '/org/list' + '?token=' + getCookie("token");
    $.ajax({
        url: url2,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                return;
            }
            var sb2 = new StringBuffer();
            var sbBe = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                sb2.append('<option value="' + data[i].orgId + '">' + data[i].orgName + '</option>');
                sbBe.append('<option value="' + data[i].orgId + '">' + data[i].orgName + '</option>');
            }
            $('#beOrgNameList').html(sbBe.toString());
            $('#beOrgNameList').selectpicker('refresh');
            $('#OrgNameList').html(sb2.toString());
            $('#OrgNameList').selectpicker('refresh');
            //$('#OrgNameList').selectpicker('val', data[0].orgId);

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

//查找登录用户能够配置的组织、结构物, 配置为列表中对应用户
function getEditUserList(dom, loginUserId, userId) {
    var struct_editSelectedOld = "";
    $('#btnReset_edit').click();

    var username_edit = $(dom).parent().siblings().eq(0).text();
    //var password_edit = $(dom).parent().siblings().eq(1).text();   
    var email_edit = $(dom).parent().siblings().eq(2).text();
    var phone_edit = $(dom).parent().siblings().eq(3).text();
    //var systemName = $(dom).parent().siblings().eq(7).text();

    $('#UserName_edit').val(username_edit);
    //$('#Password_edit').val(password_edit);
    //$('#ConfirmPassword_edit').val(password_edit);   
    $('#UserEmail_edit').val(email_edit);
    $('#UserPhone_edit').val(phone_edit);
    //$('#UserSystemName_edit').val(systemName);

    var url = apiurl + '/role/list' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                return;
            }
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                sb.append('<option value="' + data[i].roleId + '">' + data[i].roleName + '</option>');
            }
            $('#UserRole_edit').html(sb.toString());
            $('#UserRole_edit').selectpicker('refresh');

            var url2 = apiurl + '/user/' + loginUserId + '/org/list' + '?token=' + getCookie("token");
            //var url2 = apiurl + '/userManage/' + loginUserId + '/org/list' + '?token=' + getCookie("token");
            $.ajax({
                url: url2,
                type: 'get',
                dataType: 'json',
                cache: false,
                success: function (data) {
                    if (data == null || data.length == 0) {
                        return;
                    }
                    var sb2 = new StringBuffer();
                    for (var i = 0; i < data.length; i++) {
                        sb2.append('<option value="' + data[i].orgId + '">' + data[i].orgName + '</option>');
                    }
                    $('#OrgNameList_edit').html(sb2.toString());
                    $('#OrgNameList_edit').selectpicker('refresh');

                    var url3 = apiurl + '/user/' + userId + '/info' + '?token=' + getCookie("token");
                    $.ajax({
                        url: url3,
                        type: 'get',
                        dataType: 'json',
                        cache: false,
                        success: function (data) {
                            if (data == null || data.length == 0) {
                                return;
                            }
                            $('#Password_edit').val(data.password);
                            $('#ConfirmPassword_edit').val(data.password);
                            $('#UserRole_edit').selectpicker('val', data.roleId);
                            $('#beOrgNameList_edit').val(data.beOrgName);
                            if (data.orgs != null) {
                                var orgArray = [];
                                for (var i = 0; i < data.orgs.length; i++) {
                                    //var a = $('#OrgNameList_edit option[value="' + data.orgs[i].id + '"]');
                                    //$('#OrgNameList_edit option[value="' + data.orgs[i].id + '"]').attr("selected", true);
                                    orgArray.push(data.orgs[i].id);
                                }
                                $('#OrgNameList_edit').selectpicker('val', orgArray);
                                $('#OrgNameList_edit').selectpicker('refresh');
                                $('#StructNameList_edit').selectpicker('refresh');

                            }
                            if (data.structs != null) {
                                var structArray = [];
                                for (var i = 0; i < data.structs.length; i++) {
                                    structArray.push(data.structs[i].id);
                                }
                                $('#StructNameList_edit').selectpicker('val', structArray);
                                $('#StructNameList_edit').selectpicker('refresh');
                            }
                        }
                    })

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

    //防止点击事件冒泡
    $('#btnSave_edit').unbind('click').click(function () {
        SaveEditUser(userId);
    })

}

function getDeleteUserList(dom,userId) {
    var username_delete = $(dom).parent().siblings().eq(0).text();
    $('#alertMsg').html('确定删除用户"' + username_delete + '"?');

    //防止js的事件冒泡
    $('#btnDelete').unbind('click').click(function () {
        DeleteUser(userId);
    })
}

function DeleteUser(userId) {
    var url = apiurl + '/user/remove/' + userId + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'post',
        dataType: 'json',
        cache: false,
        statusCode: {
            202: function () {
                alert("删除成功!");
                $('#btnCancel').click();
                //$('#deleteUser').fadeOut();
                GetUserList();
            },
            403: function () {
                alert("权限验证出错");
                logOut();
            },
            400: function () {
                alert("用户不存在");
            },
            500: function () {
                alert("内部异常");
            },
            404: function () {
                alert('url错误');
            },
            405: function () {
                alert('抱歉，没有删除用户权限');
            }
        }
    })
}

function SaveUser() {
    var userName = $('#UserName').val();
    var password = $('#Password').val();
    var confirm_password=$('#ConfirmPassword').val();
    var email = $('#UserEmail').val();
    var phone = $('#UserPhone').val();
    var roleId = $('#UserRole').val();
    var beOrgName= $('#beOrgNameList').val();
    var orgs;
    $('#OrgNameList').val() == null ? orgs = "" : orgs = $('#OrgNameList').val().toString();
    var structs;
    $('#StructNameList').val() == null ? structs = "" : structs = $('#StructNameList').val().toString();
    //var systemName = $('#UserSystemName').val();
 
    if (!/^[a-zA-Z0-9_-]{6,15}$/.test(userName) || userName == "") {
        $('#UserName').focus();
    }
    else if (!/^[a-zA-Z0-9]{6,15}$/.test(password) || password == "") {
        $('#Password').focus();
    }
    else if (!/^[a-zA-Z0-9]{6,15}$/.test(confirm_password) || confirm_password == "") {
        $('#ConfirmPassword').focus();
    }
    else if(password!=confirm_password){
        alert("两次密码输入不一致");
        $('#ConfirmPassword').focus();
    }
    else if(!/^([a-zA-Z0-9]+[_|\_|\.|\-]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.|\-]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/.test(email)||email==""){
        $('#UserEmail').focus();
    }
    else if(!/(^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$)|(^0{0,1}1[3|4|5|6|7|8|9][0-9]{9}$)/.test(phone)||phone==""){
        $('#UserPhone').focus();
    } else if (beOrgName == null || beOrgName == "" || beOrgName == undefined) {
        $('#beOrgNameList').focus();
        alert("请选择归属组织");
    }
    else if ($('#OrgNameList').val() == null || $('#OrgNameList').val() == "" || $('#OrgNameList').val() == undefined) {
        $('#OrgNameList').focus();
        alert("请选择关注组织");
    } else if ($('#StructNameList').val() == null || $('#StructNameList').val() == "" || $('#StructNameList').val() == undefined) {
        $('#StructNameList').focus();
        alert("请选择关注结构物");
    }
        //else if (systemName.replace(/(^\s*)|(\s*$)/g, '') == "" || systemName == '用户登录后的系统名称') {
        //    alert('系统名称不能为空!')
        //    $('#UserSystemName').focus();
        //}
    else {
        var url = apiurl + '/user/add' + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            data: {
                userName: userName,
                password: password,
                email: email,
                phone: phone,
                roleId: roleId,
                beOrg: beOrgName,
                orgs: orgs,
                structs: structs,
                //systemName: systemName
            },
            statusCode: {
                202: function () {
                    alert("用户添加成功");
                    GetUserList();
                    $('#btnCancel_add').click();
                    $('#btnReset_add').click();
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
                    alert('抱歉，没有添加用户权限');
                }
            }
        })
    }
    
}

function SaveEditUser(userId) {
    var password = $('#Password_edit').val();
    var confirm_password = $('#ConfirmPassword_edit').val();
    var email = $('#UserEmail_edit').val();
    var phone = $('#UserPhone_edit').val();
    var roleId = $('#UserRole_edit').val();
    //var orgs = $('#OrgNameList_edit').val().toString();
    //var structs = $('#StructNameList_edit').val().toString();

    var orgs;
    //"clear"是和后端的约定
    $('#OrgNameList_edit').val() == null ? orgs = "clear" : orgs = $('#OrgNameList_edit').val().toString();
    var structs;
    $('#StructNameList_edit').val() == null ? structs = "clear" : structs = $('#StructNameList_edit').val().toString();

    //var systemName = $('#UserSystemName_edit').val();

    if (!/^[a-zA-Z0-9]{1,15}$/.test(password) || password == "") {
        $('#Password_edit').focus();
    }
    else if (!/^[a-zA-Z0-9]{1,15}$/.test(confirm_password) || confirm_password == "") {
        $('#ConfirmPassword_edit').focus();
    }
    else if (password != confirm_password) {
        alert("两次密码输入不一致");
        $('#ConfirmPassword_edit').focus();
    }
    else if (!/^([a-zA-Z0-9]+[_|\_|\.|\-]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.|\-]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/.test(email) || email == "") {
        $('#UserEmail_edit').focus();
    }
    else if (!/(^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$)|(^0{0,1}1[3|4|5|6|7|8|9][0-9]{9}$)/.test(phone) || phone == "") {
        $('#UserPhone_edit').focus();
    }
    else if ($('#OrgNameList_edit').val() == null || $('#OrgNameList_edit').val() == "" || $('#OrgNameList_edit').val() == undefined) {
        $('#OrgNameList_edit').focus();
        alert("请选择关注组织");
    } else if ($('#StructNameList_edit').val() == null || $('#StructNameList_edit').val() == "" || $('#StructNameList_edit').val() == undefined) {
        $('#StructNameList_edit').focus();
        alert("请选择关注结构物");
    }
    //else if (systemName == "" || systemName == '用户登录后的系统名称') {
    //    alert('系统名称不能为空!')
    //    $('#UserSystemName_edit').focus();
    //}
    else {
        var url = apiurl + '/user/modify/' + userId + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            data: {
                password: password,
                email: email,
                phone: phone,
                roleId: roleId,
                orgs: orgs,
                structs: structs,
                //systemName: systemName
            },
            statusCode: {
                202: function () {
                    alert("用户修改成功");
                    GetUserList();
                    $('#btnCancel_edit').click();
                    $('#btnReset_edit').click();
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
                    alert('抱歉，没有修改用户权限');
                }
            }
        })
    }
   
}

function UserList_To_datatable() {
    $('#userTable').dataTable({
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
            'aTargets': [1, 2, 3, 4, 5, 6]
        }]
    });
}

function checkUserName(dom) {
    var username = $(dom).val();
    if (!/^[a-z0-9_-]{1,15}$/.test(username) || username == "") {
        $('.controls .label-success').hide();
        $('.controls .label-important').hide();
    }
    if (/^[a-z0-9_-]{1,15}$/.test(username)&&username != "") {
        var url = apiurl + '/user/check-exists/' + username + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'get',
            dataType: 'json',
            cache: false,
            success: function (data) {
                if (data == null || data.length == 0) {
                    return;
                }
                if (data) {
                    $('.controls .label-important').show();
                    $('.controls .label-success').hide();
                }
                else {
                    $('.controls .label-success').show();
                    $('.controls .label-important').hide();
                }
            }
        })
    }
}