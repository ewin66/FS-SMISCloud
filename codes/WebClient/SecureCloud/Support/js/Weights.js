var orgId = null;
var structId = null;
var location_sensor_subFactor = "";

$(function () {
    $('#SuportSettings').addClass('active');
    $('#weights').addClass('active');

    if (location.href.split('=')[1] == undefined) {
        OrgStructShow(null, null);
    } else {
        orgId = location.href.split('=')[1].split('&')[0];
        structId = location.href.split('=')[2];
        OrgStructShow(orgId, structId);
    }

    $("#ThemeList-Sub-factors").change(function () {
        GetSubfactorsList(orgId, structId, parseInt($(this).val()));
    })

    $("#ThemeList-Sensors").change(function () {
        var factorIdNow = parseInt($(this).val());
        var url = apiurl + '/struct/' + structId + '/factors' + '?token=' + getCookie("token");
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
                    if (data[i].factorId == factorIdNow) {
                        for (var j = 0; j < data[i].children.length; j++) {
                            sb.append('<option value="' + data[i].children[j].factorId + '">' + data[i].children[j].factorName + '</option>');
                        }
                        $("#Sub-factorsList-Sensors").html(sb.toString());
                        //必须加上refresh
                        $("#Sub-factorsList-Sensors").selectpicker('refresh');
                        $("#Sub-factorsList-Sensors").selectpicker('val', data[i].children[0].factorId);
                        //GetSensorsList(orgId, structId, data[i].children[0].factorId);
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
    })

    $("#Sub-factorsList-Sensors").change(function () {
        if ($(this).val() == null) {
            GetSensorsList(orgId, structId, parseInt(location_sensor_subFactor));
        } else {
            GetSensorsList(orgId, structId, parseInt($(this).val()));
        }
    })
       
})



function OrgStructShow(orgId, structId) {
    //if (orgId == null) {
    //加载组织列表
    var url = apiurl + '/user/' + userId + '/org/list' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                alert("不存在组织，无法配置权重");
                return;
            }
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                if (orgId == null) {
                    if (i == 0) {
                        $("#spanOrg").html('<i class="icon-angle-down"></i>' + data[i].orgName);
                    }
                    else {
                        if (i > 0) { sb.append('<li class="divider"></li>'); }
                        sb.append('<li><a href="/Support/Weights.aspx?orgId=' + data[i].orgId + '&structId=' + '">' + data[i].orgName + '</a></li>');
                    }
                }
                else {
                    if (data[i].orgId == parseInt(orgId)) {
                        $("#spanOrg").html('<i class="icon-angle-down"></i>' + data[i].orgName);
                    } else {
                        if (i > 0) { sb.append('<li class="divider"></li>'); }
                        sb.append('<li><a href="/Support/Weights.aspx?orgId=' + data[i].orgId + '&structId=' + '">' + data[i].orgName + '</a></li>');
                    }
                }
            }
            $("#OrgDropdownList").html(sb.toString());

            if (orgId == null) {
                orgId = data[0].orgId;
            } 
            getOrgStruct(orgId, structId);
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

//获取组织下结构物
function getOrgStruct(orgNowId, structNowId) {
    //加载结构物列表
    var url2 = apiurl + '/user/' + userId + '/org/' + orgNowId + '/structs' + '?token=' + getCookie("token");
    $.ajax({
        url: url2,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                alert("该组织下不存在结构物，无法配置");
                return;
            }
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                if (structNowId == null || structNowId == "") {
                    if (i == 0) {
                        $("#spanStruct").html('<i class="icon-angle-down"></i>' + data[i].structName);
                    }
                    else {
                        if (i > 0) { sb.append('<li class="divider"></li>'); }
                        sb.append('<li><a href="/Support/Weights.aspx?orgId=' + orgNowId + '&structId=' + data[i].structId + '">' + data[i].structName + '</a></li>');
                    }
                } else {
                    if (data[i].structId == parseInt(structNowId)) {
                        $("#spanStruct").html('<i class="icon-angle-down"></i>' + data[i].structName);
                    }
                    else {
                        if (i > 0) { sb.append('<li class="divider"></li>'); }
                        sb.append('<li><a href="/Support/Weights.aspx?orgId=' + orgNowId + '&structId=' + data[i].structId + '">' + data[i].structName + '</a></li>');
                    }
                }
            }
            $("#StructDropdownList").html(sb.toString());
            //}
            orgId = orgNowId;
            if (structNowId == null || structNowId=="") {
                structId = data[0].structId;
            } else {
                structId = structNowId;
            }
            initPage(orgId, structId);
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

function initPage(orgId, structId) {
    //$('.selectpicker').selectpicker();

    //OrgStructShow(orgId, structId);
    //计算配置进度
    ProgressRate(orgId, structId);
    
    GetThemeList(orgId, structId);

    var url = apiurl + '/struct/' + structId + '/factors' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {               
                return;
            }
            //加载子因素权重配置的主题列表
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                sb.append('<option value="' + data[i].factorId + '">' + data[i].factorName + '</option>');
            }
            $("#ThemeList-Sub-factors").html(sb.toString());
            $('#ThemeList-Sub-factors').selectpicker('refresh');
            $('#ThemeList-Sub-factors').selectpicker('val',data[0].factorId);
            GetSubfactorsList(orgId, structId, data[0].factorId);

            
            //加载传感器权重配置的主题列表
            $("#ThemeList-Sensors").html(sb.toString());
            $("#ThemeList-Sensors").selectpicker('refresh');
            $("#ThemeList-Sensors").selectpicker('val', data[0].factorId);
            var sb2 = new StringBuffer();
            for (var i = 0; i < data[0].children.length; i++) {
                sb2.append('<option value="' + data[0].children[i].factorId + '">' + data[0].children[i].factorName + '</option>');
            }
            $("#Sub-factorsList-Sensors").html(sb2.toString());
            $("#Sub-factorsList-Sensors").selectpicker('refresh');
            $("#Sub-factorsList-Sensors").selectpicker('val', data[0].children[0].factorId);
            GetSensorsList(orgId, structId, data[0].children[0].factorId);
                      
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

    //定位到当前配置项
    PageLocation(orgId, structId);

    //GetSubfactorsList(orgId, structId, 2);
    //GetSensorsList(orgId, structId, 17);
}

//判断权重输入框输入是否合理
function isDigit(str) {
    var reg = /^\d*$/;
    return reg.test(str);
}

//计算当前配置进度
function ProgressRate(orgId,structId) {
    var url = apiurl + '/org/' + orgId + '/struct/' + structId + '/weight/progress' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache:false,
        success: function (data) {
            if (data.factor.length === 0 || data === null) {
                return;
            }
            var sumTotal = 1 + data.factor.length;
            var sumAdded = 0;
            if (data.facTotal == 100) {
                sumAdded++;
            }
            for (var i = 0; i < data.factor.length; i++) {
                sumTotal += data.factor[i].sub.length;
                if (data.factor[i].subTotal == 100) {
                    sumAdded++;
                }
                for (var j = 0; j < data.factor[i].sub.length; j++) {
                    if (data.factor[i].sub[j].sensorTotal == 100) {
                        sumAdded++;
                    }                  
                }
            }
            var rate = sumAdded/sumTotal;
            rate = parseInt(rate * 100);

            if (rate == 100) {
                $("#barAll").parent().addClass("progress-success");
            }
            else {
                $("#barAll").parent().removeClass("progress-success");
            }

            $("#barAll").attr("style", "width:" + rate + "%;");
            $(".barAllText").html(rate);
            //if (rate == 100) {
            //    alert("当前结构物配置完成!");
            //}
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

//定位到最近未配置的项
function PageLocation(orgId, structId) {
    var url = apiurl + '/org/' + orgId + '/struct/' + structId + '/weight/progress' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.factor.length === 0 || data === null) {
                return;
            }
            if (data.facTotal < 100) {
                //定位到主题配置页面(默认无需操作)
            }
            else {
                for (var i = 0; i < data.factor.length; i++) {
                    if (data.factor[i].subTotal < 100) {
                        //定位到子因素配置页面
                        $("#ThemeLi").removeClass("active");
                        $("#tabTheme").removeClass("active");
                        $("#Sub-factorsLi").addClass("active");
                        $("#tabSub-factors").addClass("active");

                        $("#ThemeList-Sub-factors").selectpicker('val', data.factor[i].factorId);

                        return;//返回，相当于直接跳出两层循环
                    }
                }
                //var flag = false;//标志位，用于跳出两层循环
                for (var i = 0; i < data.factor.length; i++) {
                    //if (flag) { break;}
                    var sb = new StringBuffer();
                    var flag = 0;
                    for (var j = 0; j < data.factor[i].sub.length; j++) {
                        flag = 0;
                        if (data.factor[i].sub[j].sensorTotal < 100) {
                            flag = 1;
                            //定位到传感器配置页面
                            $("#ThemeLi").removeClass("active");
                            $("#tabTheme").removeClass("active");
                            $("#SensorsLi").addClass("active");
                            $("#tabSensors").addClass("active");

                            $("#ThemeList-Sensors").selectpicker('val', data.factor[i].factorId);
                            location_sensor_subFactor = data.factor[i].sub[j].subId;
                        }
                        sb.append('<option value="' + data.factor[i].sub[j].subId + '">' + data.factor[i].sub[j].subName + '</option>');
                    }
                    if (flag) {
                        $("#Sub-factorsList-Sensors").html(sb.toString());
                        //必须加上refresh
                        $("#Sub-factorsList-Sensors").selectpicker('refresh');
                        setTimeout(function () {
                            $("#Sub-factorsList-Sensors").selectpicker('val', location_sensor_subFactor);
                        }, 500)

                        return;//返回，相当于直接跳出循环
                    }
                }
            }
            //if (data.facWeiNum < data.facNum) {
            //    //定位到主题配置页面(默认无需操作)
            //}
            //else if (data.subWeiNum < data.subNum) {
            //    //定位到子因素配置页面
            //    $("#ThemeLi").removeClass("active");
            //    $("#tabTheme").removeClass("active");
            //    $("#Sub-factorsLi").addClass("active");
            //    $("#tabSub-factors").addClass("active");
            //}
            //else if (data.senWeiNum < data.senNum) {
            //    //定位到传感器配置页面
            //    $("#ThemeLi").removeClass("active");
            //    $("#tabTheme").removeClass("active");
            //    $("#SensorsLi").addClass("active");
            //    $("#tabSensors").addClass("active");
            //}
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

//加载主题权重配置Tab页内容
function GetThemeList(orgId,structId) {
    var url = apiurl + '/org/' + orgId + '/struct/' + structId + '/factor/weight' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                $("#ThemeContent").html("");
                return;
            }
            var sb = new StringBuffer();
            //显示的已经配好的权重百分比
            var weightsPercent = 0;
            for (var i = 0; i < data.length; i++) {
                sb.append('<div class="control-group">');
                sb.append('<label for="amountTheme'+data[i].factorId+'">' + data[i].factorName + '权重：</label>');
                sb.append('<div class="span2"><input type="text" id="amountTheme' + data[i].factorId + '" style="width: 50px;" /></div>');
                sb.append('<div class="span6"><div id="sliderTheme' + data[i].factorId + '" style="height: 12px"></div></div></div>');

                weightsPercent += data[i].weight;
            }
            $("#ThemeContent").html(sb.toString());

            if (weightsPercent == 100) {
                $("#barTheme").parent().removeClass("progress-danger");
                $("#barTheme").parent().addClass("progress-success");
            }
            //加载百分比条
            $("#barTheme").attr("style", "width:" + weightsPercent + "%;");
            $(".barThemeText").html(weightsPercent);

            for (var i = 0; i < data.length; i++) {              
                $("#sliderTheme"+data[i].factorId).slider({
                    orientation: "horizontal",
                    range: "min",
                    min: 0,
                    max: 100,
                    value: data[i].weight,
                    //step:0.1,
                    slide: function (event, ui) {
                        //获取当前元素的id
                        var a = $(this).attr("id");
                        var id = a.replace(/[^\d]/g, "");                       
                        $("#amountTheme" + id).val(ui.value);
                        
                        //求拉动时当前的所有权重之和
                        var weightsAdded = ui.value;                       
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].factorId != parseInt(id)) {
                                weightsAdded +=parseInt($("#amountTheme" + data[j].factorId).val());
                            }                                                       
                        }                       
                        if (weightsAdded <100) {
                            $("#barTheme").parent().removeClass("progress-danger");
                            $("#barTheme").parent().removeClass("progress-success");
                            $("#barTheme").attr("style", "width:" + weightsAdded + "%;");
                            $(".barThemeText").html(weightsAdded);
                        }
                        else if (weightsAdded == 100) {
                            $("#barTheme").parent().removeClass("progress-danger");
                            $("#barTheme").parent().addClass("progress-success");
                            $("#barTheme").attr("style", "width:" + weightsAdded + "%;");
                            $(".barThemeText").html(100);
                        }
                        else {
                            $("#barTheme").parent().removeClass("progress-success");
                            $("#barTheme").parent().addClass("progress-danger");
                            $("#barTheme").attr("style", "width:" + 100 + "%;");
                            $(".barThemeText").html(100);
                        }
                    }
                    //stop: function (event, ui) {
                    //    var a = $(this).attr("id");
                    //    var id = a.replace(/[^\d]/g, "");
                    //    $("#amountTheme" + id).val(ui.value);

                    //    //求拉动停止时的所有权重之和
                    //    var weightsAdded = ui.value;
                    //    for (var j = 0; j < data.length; j++) {
                    //        if (data[j].factorId != parseInt(id)) {
                    //            weightsAdded += parseInt($("#amountTheme" + data[j].factorId).val());
                    //        }
                    //    }
                    //    if (weightsAdded > 100) {
                    //        alert("权重配置超出范围!");
                    //        var weightsBefore = weightsAdded - ui.value;
                    //        $("#barTheme").attr("style", "width:" + weightsBefore + "%;");
                    //        $(".barThemeText").html(weightsBefore);

                    //        $("#sliderTheme" + id).slider("value", 0);
                    //        $("#amountTheme" + id).val('0');
                            
                    //    }
                    //}
                });
                $("#amountTheme" + data[i].factorId).val($("#sliderTheme" + data[i].factorId).slider("value"));
                $("#amountTheme" + data[i].factorId).change(function () {                   
                    var a = $(this).attr("id");
                    var id = a.replace(/[^\d]/g, "");
                    //var c = $(this).val();
                    var weightsAdded = parseInt($(this).val());
                    for (var j = 0; j < data.length; j++) {
                        if (data[j].factorId != parseInt(id)) {
                            weightsAdded += parseInt($("#amountTheme" + data[j].factorId).val());
                        }                       
                    }
                    if (!isDigit($(this).val()) || parseInt($(this).val()) > 100) {
                        //$("#barTheme").parent().removeClass("progress-danger");
                        alert('输入有误!');
                        var weightsBefore = 0;
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].factorId != parseInt(id)) {
                                weightsBefore += parseInt($("#amountTheme" + data[j].factorId).val());
                            }
                        }
                        if (weightsBefore < 100) {
                            $("#barTheme").parent().removeClass("progress-success");
                            $("#barTheme").parent().removeClass("progress-danger");
                            $("#barTheme").attr("style", "width:" + weightsBefore + "%;");
                            $(".barThemeText").html(weightsBefore);
                        }
                        else if (weightsBefore==100) {
                            $("#barTheme").parent().removeClass("progress-danger");
                            $("#barTheme").parent().addClass("progress-success");
                            $("#barTheme").attr("style", "width:" + 100 + "%;");
                            $(".barThemeText").html(100);
                        }
                        else {
                            $("#barTheme").parent().removeClass("progress-success");
                            $("#barTheme").parent().addClass("progress-danger");
                            $("#barTheme").attr("style", "width:" + 100 + "%;");
                            $(".barThemeText").html(100);
                        }

                        $("#sliderTheme" + id).slider("value", 0);
                        $("#amountTheme" + id).val('0');                       
                        
                    }
                    else if (weightsAdded > 100) {
                        $("#barTheme").parent().removeClass("progress-success");
                        $("#barTheme").parent().addClass("progress-danger");
                        $("#sliderTheme" + id).slider("value", $("#amountTheme" + id).val());
                        $("#barTheme").attr("style", "width:" + 100 + "%;");
                        $(".barThemeText").html(100);
                    }
                    else if (weightsAdded==100) {
                        $("#barTheme").parent().removeClass("progress-danger");
                        $("#barTheme").parent().addClass("progress-success");
                        $("#sliderTheme" + id).slider("value", $("#amountTheme" + id).val());
                        $("#barTheme").attr("style", "width:" + 100 + "%;");
                        $(".barThemeText").html(100);
                    }
                    else {
                        $("#barTheme").parent().removeClass("progress-success");
                        $("#barTheme").parent().removeClass("progress-danger");
                        $("#sliderTheme" + id).slider("value", $("#amountTheme" + id).val());
                        $("#barTheme").attr("style", "width:" + weightsAdded + "%;");
                        $(".barThemeText").html(weightsAdded);
                    }
                })
            }
          
            $("#btnSaveTheme").unbind('click').click(function () {
                var sum = 0;
                var arrayWeights = [];
                for (var i = 0; i < data.length; i++) {
                    sum += parseInt($("#amountTheme" + data[i].factorId).val());
                    var obj = { "orgId": parseInt(orgId), "structId": parseInt(structId), "factorId": data[i].factorId, "weight": parseInt($("#amountTheme" + data[i].factorId).val()) };
                    arrayWeights.push(obj);
                }
                if (sum != 100 && sum != 0) {
                    alert("权重配置不正确！");
                }
                else {
                    //alert(JSON.stringify(arrayWeights));
                    var url = apiurl + '/factor/weight/add' + '?token=' + getCookie("token");
                    $.ajax({
                        url: url,
                        type: 'post',
                        data: {"":arrayWeights},
                        statusCode: {
                            202: function () {
                                alert("保存配置成功!");
                                ProgressRate(orgId,structId);
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
                                alert('抱歉，没有配置主题权重的权限');
                            }
                        }
                    })
                }
            })

            $("#btnSaveNextTheme").unbind('click').click(function () {
                var sum = 0;
                var arrayWeights = [];
                for (var i = 0; i < data.length; i++) {
                    sum += parseInt($("#amountTheme" + data[i].factorId).val());
                    var obj = { "orgId": parseInt(orgId), "structId": parseInt(structId), "factorId": data[i].factorId, "weight": parseInt($("#amountTheme" + data[i].factorId).val()) };
                    arrayWeights.push(obj);
                }
                if (sum != 100 && sum != 0) {
                    alert("权重配置不正确！");
                }
                else {
                    //alert(JSON.stringify(arrayWeights));
                    var url = apiurl + '/factor/weight/add' + '?token=' + getCookie("token");
                    $.ajax({
                        url: url,
                        type: 'post',
                        data: { "": arrayWeights },
                        statusCode: {
                            202: function () {
                                alert("保存配置成功!");
                                // 跳转到子因素权重配置
                                $("#ThemeLi").removeClass("active");
                                $("#tabTheme").removeClass("active");
                                $("#Sub-factorsLi").addClass("active");
                                $("#tabSub-factors").addClass("active");

                                ProgressRate(orgId, structId);
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
                                alert('抱歉，没有配置主题权重的权限');
                            }
                        }
                    })
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
}

function GetSubfactorsList(orgId, structId, factorId) {
    var url = apiurl + '/org/' + orgId + '/struct/' + structId + '/factor/' + factorId + '/sub-factor/weight' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                $("#Sub-factorsContent").html("");
                return;
            }
            var sb = new StringBuffer();
            //显示的已经配好的权重百分比
            var weightsPercent = 0;
            for (var i = 0; i < data.length; i++) {
                sb.append('<div class="control-group">');
                sb.append('<label for="amountSub-factors' + data[i].subFactorId + '">' + data[i].subFactorName + '权重：</label>');
                sb.append('<div class="span2"><input type="text" id="amountSub-factors' + data[i].subFactorId + '" style="width: 50px;" /></div>');
                sb.append('<div class="span6"><div id="sliderSub-factors' + data[i].subFactorId + '" style="height: 12px"></div></div></div>');

                weightsPercent += data[i].weight;             
            }           
            $("#Sub-factorsContent").html(sb.toString());

            if (weightsPercent == 100) {
                $("#barSub-factors").parent().removeClass("progress-danger");
                $("#barSub-factors").parent().addClass("progress-success");
            }
            //加载百分比条
            $("#barSub-factors").attr("style", "width:" + weightsPercent + "%;");
            $(".barSub-factorsText").html(weightsPercent);

            for (var i = 0; i < data.length; i++) {
                $("#sliderSub-factors" + data[i].subFactorId).slider({
                    orientation: "horizontal",
                    range: "min",
                    min: 0,
                    max: 100,
                    value: data[i].weight,
                    //step:0.1,
                    slide: function (event, ui) {
                        //获取当前元素的id
                        var a = $(this).attr("id");
                        var id = a.replace(/[^\d]/g, "");
                        $("#amountSub-factors" + id).val(ui.value);

                        //求拉动时当前的所有权重之和
                        var weightsAdded = ui.value;
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].subFactorId != parseInt(id)) {
                                weightsAdded += parseInt($("#amountSub-factors" + data[j].subFactorId).val());
                            }
                        }
                        if (weightsAdded < 100) {
                            $("#barSub-factors").parent().removeClass("progress-success");
                            $("#barSub-factors").parent().removeClass("progress-danger");
                            $("#barSub-factors").attr("style", "width:" + weightsAdded + "%;");
                            $(".barSub-factorsText").html(weightsAdded);
                        }
                        else if (weightsAdded==100) {
                            $("#barSub-factors").parent().removeClass("progress-danger");
                            $("#barSub-factors").parent().addClass("progress-success");
                            $("#barSub-factors").attr("style", "width:" + 100 + "%;");
                            $(".barSub-factorsText").html(100);
                        }
                        else {
                            $("#barSub-factors").parent().removeClass("progress-success");
                            $("#barSub-factors").parent().addClass("progress-danger");
                            $("#barSub-factors").attr("style", "width:" + 100 + "%;");
                            $(".barSub-factorsText").html(100);
                        }
                    },
                    //stop: function (event, ui) {
                    //    var a = $(this).attr("id");
                    //    var id = a.replace(/[^\d]/g, "");
                    //    $("#amountSub-factors" + id).val(ui.value);

                    //    //求拉动停止时的所有权重之和
                    //    var weightsAdded = ui.value;
                    //    for (var j = 0; j < data.length; j++) {
                    //        if (data[j].subFactorId != parseInt(id)) {
                    //            weightsAdded += parseInt($("#amountSub-factors" + data[j].subFactorId).val());
                    //        }
                    //    }
                    //    if (weightsAdded > 100) {
                    //        alert("权重配置超出范围!");
                    //        var weightsBefore = weightsAdded - ui.value;
                    //        $("#barSub-factors").attr("style", "width:" + weightsBefore + "%;");
                    //        $(".barSub-factorsText").html(weightsBefore);

                    //        $("#sliderSub-factors" + id).slider("value", 0);
                    //        $("#amountSub-factors" + id).val('0');

                    //    }
                    //}
                });
                $("#amountSub-factors" + data[i].subFactorId).val($("#sliderSub-factors" + data[i].subFactorId).slider("value"));
                $("#amountSub-factors" + data[i].subFactorId).change(function () {
                    var a = $(this).attr("id");
                    var id = a.replace(/[^\d]/g, "");

                    var weightsAdded = parseInt($(this).val());
                    for (var j = 0; j < data.length; j++) {
                        if (data[j].subFactorId != parseInt(id)) {
                            weightsAdded += parseInt($("#amountSub-factors" + data[j].subFactorId).val());
                        }
                    }
                    if (!isDigit($(this).val()) || parseInt($(this).val()) > 100) {
                        //$("#barSub-factors").parent().removeClass("progress-danger");
                        alert('输入有误!');
                        var weightsBefore = 0;
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].subFactorId != parseInt(id)) {
                                weightsBefore += parseInt($("#amountSub-factors" + data[j].subFactorId).val());
                            }
                        }
                        if (weightsBefore < 100) {
                            $("#barSub-factors").parent().removeClass("progress-success");
                            $("#barSub-factors").parent().removeClass("progress-danger");
                            $("#barSub-factors").attr("style", "width:" + weightsBefore + "%;");
                            $(".barSub-factorsText").html(weightsBefore);
                        }
                        else if (weightsBefore==100) {
                            $("#barSub-factors").parent().removeClass("progress-danger");
                            $("#barSub-factors").parent().addClass("progress-success");
                            $("#barSub-factors").attr("style", "width:" + 100 + "%;");
                            $(".barSub-factorsText").html(100);
                        }
                        else {
                            $("#barSub-factors").parent().removeClass("progress-success");
                            $("#barSub-factors").parent().addClass("progress-danger");
                            $("#barSub-factors").attr("style", "width:" + 100 + "%;");
                            $(".barSub-factorsText").html(100);
                        }

                        $("#sliderSub-factors" + id).slider("value", 0);
                        $("#amountSub-factors" + id).val('0');

                    }
                    else if (weightsAdded > 100) {
                        $("#barSub-factors").parent().removeClass("progress-success");
                        $("#barSub-factors").parent().addClass("progress-danger");
                        $("#sliderSub-factors" + id).slider("value", $("#amountSub-factors" + id).val());
                        $("#barSub-factors").attr("style", "width:" + 100 + "%;");
                        $(".barSub-factorsText").html(100);
                    }
                    else if (weightsAdded==100) {
                        $("#barSub-factors").parent().removeClass("progress-danger");
                        $("#barSub-factors").parent().addClass("progress-success");
                        $("#sliderSub-factors" + id).slider("value", $("#amountSub-factors" + id).val());
                        $("#barSub-factors").attr("style", "width:" + 100 + "%;");
                        $(".barSub-factorsText").html(100);
                    }
                    else {
                        $("#barSub-factors").parent().removeClass("progress-success");
                        $("#barSub-factors").parent().removeClass("progress-danger");
                        $("#sliderSub-factors" + id).slider("value", $("#amountSub-factors" + id).val());
                        $("#barSub-factors").attr("style", "width:" + weightsAdded + "%;");
                        $(".barSub-factorsText").html(weightsAdded);
                    }
                })
            }

            $("#btnSaveSub-factors").unbind('click').click(function () {
                var sum = 0;
                var arrayWeights = [];
                for (var i = 0; i < data.length; i++) {
                    sum += parseInt($("#amountSub-factors" + data[i].subFactorId).val());
                    var obj = { "orgId": parseInt(orgId), "structId": parseInt(structId), "subFactorId": data[i].subFactorId, "weight": parseInt($("#amountSub-factors" + data[i].subFactorId).val()) };
                    arrayWeights.push(obj);
                }
                if (sum != 100 && sum != 0) {
                    alert("权重配置不正确！");
                }
                else {
                    //alert(JSON.stringify(arrayWeights));
                    var url = apiurl + '/sub-factor/weight/add' + '?token=' + getCookie("token");
                    $.ajax({
                        url: url,
                        type: 'post',
                        data: { "": arrayWeights },
                        statusCode: {
                            202: function () {
                                alert("保存配置成功!");
                                ProgressRate(orgId, structId);
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
                                alert('抱歉，没有配置子因素权重的权限');
                            }
                        }
                    })
                }
            })

            $("#btnSaveNextSub-factors").unbind('click').click(function () {
                var sum = 0;
                var arrayWeights = [];
                for (var i = 0; i < data.length; i++) {
                    sum += parseInt($("#amountSub-factors" + data[i].subFactorId).val());
                    var obj = { "orgId": parseInt(orgId), "structId": parseInt(structId), "subFactorId": data[i].subFactorId, "weight": parseInt($("#amountSub-factors" + data[i].subFactorId).val()) };
                    arrayWeights.push(obj);
                }
                if (sum != 100 && sum != 0) {
                    alert("权重配置不正确！");
                }
                else {
                    //alert(JSON.stringify(arrayWeights));
                    var url = apiurl + '/sub-factor/weight/add' + '?token=' + getCookie("token");
                    $.ajax({
                        url: url,
                        type: 'post',
                        data: { "": arrayWeights },
                        statusCode: {
                            202: function () {
                                alert("保存配置成功!");

                                var url2 = apiurl + '/struct/' + structId + '/factors' + '?token=' + getCookie("token");
                                $.ajax({
                                    url: url2,
                                    type: 'get',
                                    dataType: 'json',
                                    success: function (result) {
                                        if (result[result.length - 1].factorId == parseInt($("#ThemeList-Sub-factors").val())) {
                                            //列表最后一个,跳到传感器权重配置
                                            $("#Sub-factorsLi").removeClass("active");
                                            $("#tabSub-factors").removeClass("active");
                                            $("#SensorsLi").addClass("active");
                                            $("#tabSensors").addClass("active");

                                            $("#ThemeList-Sensors").selectpicker('val',result[0].factorId);
                                        }
                                        else {
                                            //切换到主题列表下一个
                                            for (var i = 0; i < result.length; i++) {
                                                if (result[i].factorId == parseInt($("#ThemeList-Sub-factors").val())) {
                                                    $("#ThemeList-Sub-factors").selectpicker('val', result[i + 1].factorId);
                                                    break;
                                                }
                                            }
                                        }
                                    },
                                    error: function () {
                                        alert("获取结构物监测因素失败!");
                                    }
                                })
                              
                                ProgressRate(orgId, structId);

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
                                alert('抱歉，没有配置子因素权重的权限');
                            }
                        }
                    })
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

}

function GetSensorsList(orgId, structId, subFactorId) {
    var url = apiurl + '/org/' + orgId + '/struct/' + structId + '/sub-factor/' + subFactorId + '/sensor/weight' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                $("#SensorsContent").html("");
                return;
            }
            var sb = new StringBuffer();
            //显示的已经配好的权重百分比
            var weightsPercent = 0;
            for (var i = 0; i < data.length; i++) {
                sb.append('<div class="control-group">');
                sb.append('<label for="amountSensors' + data[i].sensorId + '">' + data[i].location + '权重：</label>');
                sb.append('<div class="span2"><input type="text" id="amountSensors' + data[i].sensorId + '" style="width: 50px;" /></div>');
                sb.append('<div class="span6"><div id="sliderSensors' + data[i].sensorId + '" style="height: 12px"></div></div></div>');

                weightsPercent += data[i].weight;
            }
            $("#SensorsContent").html(sb.toString());

            if (weightsPercent == 100) {
                $("#barSensors").parent().removeClass("progress-danger");
                $("#barSensors").parent().addClass("progress-success");
            }
            //加载百分比条
            $("#barSensors").attr("style", "width:" + weightsPercent + "%;");
            $(".barSensorsText").html(weightsPercent);

            for (var i = 0; i < data.length; i++) {
                $("#sliderSensors" + data[i].sensorId).slider({
                    orientation: "horizontal",
                    range: "min",
                    min: 0,
                    max: 100,
                    value: data[i].weight,
                    //step:0.1,
                    slide: function (event, ui) {
                        //获取当前元素的id
                        var a = $(this).attr("id");
                        var id = a.replace(/[^\d]/g, "");
                        $("#amountSensors" + id).val(ui.value);

                        //求拉动时当前的所有权重之和
                        var weightsAdded = ui.value;
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].sensorId != parseInt(id)) {
                                weightsAdded += parseInt($("#amountSensors" + data[j].sensorId).val());
                            }
                        }
                        if (weightsAdded < 100) {
                            $("#barSensors").parent().removeClass("progress-success");
                            $("#barSensors").parent().removeClass("progress-danger");
                            $("#barSensors").attr("style", "width:" + weightsAdded + "%;");
                            $(".barSensorsText").html(weightsAdded);
                        }
                        else if (weightsAdded==100) {
                            $("#barSensors").parent().removeClass("progress-danger");
                            $("#barSensors").parent().addClass("progress-success");
                            $("#barSensors").attr("style", "width:" + 100 + "%;");
                            $(".barSensorsText").html(100);
                        }
                        else {
                            $("#barSensors").parent().removeClass("progress-success");
                            $("#barSensors").parent().addClass("progress-danger");
                            $("#barSensors").attr("style", "width:" + 100 + "%;");
                            $(".barSensorsText").html(100);
                        }
                    },
                    //stop: function (event, ui) {
                    //    var a = $(this).attr("id");
                    //    var id = a.replace(/[^\d]/g, "");
                    //    $("#amountSensors" + id).val(ui.value);

                    //    //求拉动停止时的所有权重之和
                    //    var weightsAdded = ui.value;
                    //    for (var j = 0; j < data.length; j++) {
                    //        if (data[j].sensorId != parseInt(id)) {
                    //            weightsAdded += parseInt($("#amountSensors" + data[j].sensorId).val());
                    //        }
                    //    }
                    //    if (weightsAdded > 100) {
                    //        alert("权重配置超出范围!");
                    //        var weightsBefore = weightsAdded - ui.value;
                    //        $("#barSensors").attr("style", "width:" + weightsBefore + "%;");
                    //        $(".barSensorsText").html(weightsBefore);

                    //        $("#sliderSensors" + id).slider("value", 0);
                    //        $("#amountSensors" + id).val('0');

                    //    }
                    //}
                });
                $("#amountSensors" + data[i].sensorId).val($("#sliderSensors" + data[i].sensorId).slider("value"));
                $("#amountSensors" + data[i].sensorId).change(function () {
                    var a = $(this).attr("id");
                    var id = a.replace(/[^\d]/g, "");

                    var weightsAdded = parseInt($(this).val());
                    for (var j = 0; j < data.length; j++) {
                        if (data[j].sensorId != parseInt(id)) {
                            weightsAdded += parseInt($("#amountSensors" + data[j].sensorId).val());
                        }
                    }
                    if (!isDigit($(this).val()) || parseInt($(this).val()) > 100) {
                        //$("#barSensors").parent().removeClass("progress-danger");
                        alert('输入有误!');
                        var weightsBefore = 0;
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].sensorId != parseInt(id)) {
                                weightsBefore += parseInt($("#amountSensors" + data[j].sensorId).val());
                            }
                        }
                        if (weightsBefore < 100) {
                            $("#barSensors").parent().removeClass("progress-success");
                            $("#barSensors").parent().removeClass("progress-danger");
                            $("#barSensors").attr("style", "width:" + weightsBefore + "%;");
                            $(".barSensorsText").html(weightsBefore);                            
                        }
                        else if (weightsBefore==100) {
                            $("#barSensors").parent().removeClass("progress-danger");
                            $("#barSensors").parent().addClass("progress-success");
                            $("#barSensors").attr("style", "width:" + 100 + "%;");
                            $(".barSensorsText").html(100);
                        }
                        else {
                            $("#barSensors").parent().removeClass("progress-success");
                            $("#barSensors").parent().addClass("progress-danger");
                            $("#barSensors").attr("style", "width:" + 100 + "%;");
                            $(".barSensorsText").html(100);
                        }
                        
                        $("#sliderSensors" + id).slider("value", 0);
                        $("#amountSensors" + id).val('0');

                    }
                    else if (weightsAdded > 100) {
                        $("#barSensors").parent().removeClass("progress-success");
                        $("#barSensors").parent().addClass("progress-danger");
                        $("#sliderSensors" + id).slider("value", $("#amountSensors" + id).val());
                        $("#barSensors").attr("style", "width:" + 100 + "%;");
                        $(".barSensorsText").html(100);
                    }
                    else if (weightsAdded==100) {
                        $("#barSensors").parent().removeClass("progress-danger");
                        $("#barSensors").parent().addClass("progress-success");
                        $("#sliderSensors" + id).slider("value", $("#amountSensors" + id).val());
                        $("#barSensors").attr("style", "width:" + 100 + "%;");
                        $(".barSensorsText").html(100);
                    }
                    else {
                        $("#barSensors").parent().removeClass("progress-success");
                        $("#barSensors").parent().removeClass("progress-danger");
                        $("#sliderSensors" + id).slider("value", $("#amountSensors" + id).val());
                        $("#barSensors").attr("style", "width:" + weightsAdded + "%;");
                        $(".barSensorsText").html(weightsAdded);
                    }
                })
            }

            $("#btnSaveSensors").unbind('click').click(function () {
                var sum = 0;
                var arrayWeights = [];
                for (var i = 0; i < data.length; i++) {
                    sum += parseInt($("#amountSensors" + data[i].sensorId).val());
                    var obj = { "orgId": parseInt(orgId), "structId": parseInt(structId), "sensorId": data[i].sensorId, "weight": parseInt($("#amountSensors" + data[i].sensorId).val()) };
                    arrayWeights.push(obj);
                }
                if (sum != 100 && sum != 0) {
                    alert("权重配置不正确！");
                }
                else {
                    //alert(JSON.stringify(arrayWeights));
                    var url = apiurl + '/sensor/weight/add' + '?token=' + getCookie("token");
                    $.ajax({
                        url: url,
                        type: 'post',
                        data: { "": arrayWeights },
                        statusCode: {
                            202: function () {
                                alert("保存配置成功!");
                                ProgressRate(orgId, structId);
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
                                alert('抱歉，没有配置传感器权重的权限');
                            }
                        }
                    })
                }
            })

            $("#btnSaveNextSensors").unbind('click').click(function () {
                var sum = 0;
                var arrayWeights = [];
                for (var i = 0; i < data.length; i++) {
                    sum += parseInt($("#amountSensors" + data[i].sensorId).val());
                    var obj = { "orgId": parseInt(orgId), "structId": parseInt(structId), "sensorId": data[i].sensorId, "weight": parseInt($("#amountSensors" + data[i].sensorId).val()) };
                    arrayWeights.push(obj);
                }
                if (sum != 100 && sum != 0) {
                    alert("权重配置不正确！");
                }
                else {
                    //alert(JSON.stringify(arrayWeights));
                    var url = apiurl + '/sensor/weight/add' + '?token=' + getCookie("token");
                    $.ajax({
                        url: url,
                        type: 'post',
                        data: { "": arrayWeights },
                        statusCode: {
                            202: function () {
                                alert("保存配置成功!");

                                var url2 = apiurl + '/struct/' + structId + '/factors' + '?token=' + getCookie("token");
                                $.ajax({
                                    url: url2,
                                    type: 'get',
                                    dataType: 'json',
                                    success: function (result) {
                                        //最后一个主题的最后一个子因素
                                        var maxLength=result[result.length-1].children.length;
                                        if (result[result.length - 1].children[maxLength-1].factorId == parseInt($("#Sub-factorsList-Sensors").val())) {
                                            alert("已经配置到当前结构物最后一项");
                                        }
                                        else {
                                            //切换到下一个子因素
                                            for (var i = 0; i < result.length; i++) {
                                                var maxChildrenLength = result[i].children.length;
                                                if (result[i].children[maxChildrenLength - 1].factorId == parseInt($("#Sub-factorsList-Sensors").val())) {
                                                    $("#ThemeList-Sensors").selectpicker('val', result[i + 1].factorId);
                                                    break;
                                                }
                                                else {
                                                    for (var j = 0; j < result[i].children.length; j++) {
                                                        if (result[i].children[j].factorId == parseInt($("#Sub-factorsList-Sensors").val())) {
                                                            $("#Sub-factorsList-Sensors").selectpicker('val', result[i].children[j + 1].factorId);
                                                            break;
                                                        }
                                                    }
                                                }                                               
                                            }
                                        }
                                    },
                                    error: function () {
                                        alert("获取结构物监测因素失败!");
                                    }
                                })

                                ProgressRate(orgId, structId);
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
                                alert('抱歉，没有配置传感器权重的权限');
                            }
                        }
                    })
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
                alert('url错误，请重新选择主题或子因素');
            }
        }
    })
}


