//全局变量，页面里的结构物Id和Name
var structId = location.href.split('=')[1].split('#')[0];
var structName;

$(function () {
    getStructName(structId);
    getFactorListAdded(structId);
    

    //修改监测因素
    $(".editor_Factort").click(function () {
        getFactorListChoosen(structId);
    })
    //下一步
    $("#factorNext").click(function() {
        $('#tabFactor').removeClass('active');
        $('#tab_Factor').removeClass('active');
        $('#tabFactorUnit').addClass('active');
        $('#tab_FactorUnit').addClass('active');
        IntConfigUnits(structId, structName);
    });

    //重置表单
    $('#btnResetFactor').click(function () {
        getFactorListChoosen(structId);
    });

    $("#btnSaveFactor").click(function() {
        var array = new Array();
        $("input.checkboxes:checked").each(function() {
            array.push(this.value);

        })
        var str = array.toString();
        if (str == "") {
            alert("请选择监测因素再保存");
        } else {
            var url = apiurl + '/struct/' + structId + '/factor/modify' + '?token=' + getCookie("token");
            $.ajax({
                url: url,
                type: 'post',
                data: { "": str },
                statusCode: {
                    202: function() {
                        alert("保存成功！");
                        getFactorListAdded(structId);
                        IntConfigUnits(structId, structName);//保存的时候判断监测因素
                        alert("请查看监测因素单位配置界面是否已经保存！确保DTU配置界面和普通用户界面展示正常");
                    },
                    403: function() {
                        alert("权限验证出错");
                        logOut();
                    },
                    400: function() {
                        alert("添加失败,参数错误");
                    },
                    500: function() {
                        alert("内部异常");
                    },
                    404: function() {
                        alert('url错误');
                    },
                    405: function() {
                        alert('抱歉，没有修改因素配置的权限');
                    },
                    409: function () {
                        alert('部分监测因素不能被移除，请先删除待移除监测因素下的传感器配置信息');
                    }
                }
            });
            
        }

    });

});


function getStructName(structId) {
    var url = apiurl + '/struct/' + structId + '/info' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {           
            if (data!=null) {
                structName = data.structName;
                $("#spanStructName").html(structName);
                $("#organizationNameview").val(structName);
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

function getFactorListAdded(structId) {
    var url = apiurl + '/struct/' + structId + '/factor-config' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache:false,
        success: function (data) {
            if (data.length >= 1) {
                var sb = new StringBuffer();
                for (var i = 0; i < data.length; i++) {                  
                    sb.append('<table><tr><th><label class="control-label"><b>' + data[i].factorName + ':</b></label></th>');
                    for (var j = 0; j < data[i].children.length; j++) {
                        if (data[i].children[j].choose) {
                            sb.append('<th style="width: 100px"><font style="font-weight: normal;">' + data[i].children[j].factorName);
                            if (data[i].children[j].description != null) {
                                sb.append('(' + data[i].children[j].description + ')');
                            }
                            sb.append('</font></th>');
                        }                      
                    }
                    sb.append('</tr></table>');
                    if (i != data.length - 1) {
                        sb.append('<hr style="border: 1px #cccccc dashed;" />');
                    }
                }
                $("#factorListAdded").html(sb.toString());
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

function getFactorListChoosen(structId) {
    var url = apiurl + '/struct/' + structId + '/factor-config' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache:false,
        success: function (data) {
            if (data.length >= 1) {
                var sb = new StringBuffer();
                for (var i = 0; i < data.length; i++) {
                    sb.append('<table><tr><th><b>' + data[i].factorName + ':</b></th>');
                    for (var j = 0; j < data[i].children.length; j++) {
                        if (data[i].children[j].choose) {
                            sb.append('<th style="width: 100px;"><input type="checkbox" class="checkboxes" checked="checked" value="' + data[i].children[j].factorId + '" /><font style="font-weight: normal;">' + data[i].children[j].factorName);
                        }
                        else {
                            sb.append('<th style="width: 100px;"><input type="checkbox" class="checkboxes" value="' + data[i].children[j].factorId + '" /><font style="font-weight: normal;">' + data[i].children[j].factorName);
                        }
                        if (data[i].children[j].description != null) {
                            sb.append('(' + data[i].children[j].description + ')');
                        }
                        sb.append('</font></th>');
                    }
                    sb.append('</tr></table>');
                    if (i != data.length - 1) {
                        sb.append('<hr style="border: 1px #cccccc dashed;" />');
                    }
                }
                $("#factorListChoosen").html(sb.toString());
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





