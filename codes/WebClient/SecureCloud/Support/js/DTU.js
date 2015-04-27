
var dtuArray = [];

$(function () {
    GetDtu(structId, structName);
    bindDtuProducts();
});
$('#tabDtu').click(function () {
    GetDtu(structId, structName);
});
function GetDtu(structId, structName) {
    dtuArray = [];
    $('#dtuTable').dataTable().fnDestroy();
    var url = apiurl + '/struct/' + structId + '/dtu?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                sb.append("<tr id='dtu-" + data[i].dtuId + "'><td >" + data[i].dtuNo + "</td>");//DTU编号
                sb.append("<td>" + data[i].networkType + '</td>');//连接类型
                sb.append("<td>" + data[i].granularity + "分钟</td>");//采集粒度
                sb.append("<td>" + data[i].sim + "</td>");//SIM卡号
                sb.append("<td>" + data[i].ip + "</td>");//IP
                sb.append("<td>" + data[i].port + "</td>");//端口
                sb.append('<td>' + data[i].p1 + '</td>');//文件路径
                var str = "<td><a href='#modifyDTUModal' class='editor_edit' data-toggle='modal'>修改</a> | ";
                str += "<a href='#deleteDTUModal' class='editor_delete' data-toggle='modal'>删除</a></td></tr>";
                sb.append(str);//操作
                var dtuNoId = ["", 0];
                dtuNoId[0] = data[i].dtuNo;
                dtuNoId[1] = data[i].dtuId;
                dtuArray.push(dtuNoId);
            }

            $('#dtuTbody').html("");
            $('#dtuTbody').append(sb.toString());
            DTU_datatable();
            rowsDTUJudge();//判断DTU是否有已配置
            InitDTUBox();

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
function rowsDTUJudge() {
    var table = document.getElementById("dtuTable");
    var rows = table.rows.length;//包括列头一行
    if (rows > 1) {
        var url = apiurl + '/struct/' + structId + '/info?token=' + getCookie('token');
        $.ajax({
            //async: false,//同步
            url: url,
            type: 'get',
            cache: false,
            success: function (data) {
                if (data != null) {
                    if (data.structType == "无线采集网站" || data.structType == "智能张拉") {
                        $('#tabSensor').hide();
                        $('#tabThreshold').hide();
                        document.getElementById("dtuNext").disabled = true;//不可用
                        $('#dtuNext').removeClass('blue');
                    } else {
                        $('#tabSensor').show();
                        document.getElementById("dtuNext").disabled = false;//可用
                        $('#dtuNext').addClass('blue');
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
    else {
        $('#tabSensor').hide();
        $('#tabThreshold').hide();
        document.getElementById("dtuNext").disabled = true;//不可用
        $('#dtuNext').removeClass('blue');
    }
}
// 绑定dtu产品到下拉列表
var dtuProducts;// dtu产品列表-全局变量
function bindDtuProducts() {
    $('#addDTUFactory option').remove();
    $('#addDTUModel option').remove();
    $('#modifyDTUFactory option').remove();
    $('#modifyDTUModel option').remove();
    var url = apiurl + '/dtu/product?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.length == 0) {
                return;
            }
            dtuProducts = data;
            // 绑定DTU厂商下拉框
            for (var i = 0; i < data.length; i++) {
                document.getElementById('addDTUFactory').options.add(new Option(data[i].dtuFactory, data[i].dtuFactory));
                document.getElementById('modifyDTUFactory').options.add(new Option(data[i].dtuFactory, data[i].dtuFactory));
            }
            $('#addDTUFactory').chosen({
                no_results_text: "没有找到",
                allow_single_de: true
            });
            $('#modifyDTUFactory').chosen({
                no_results_text: "没有找到",
                allow_single_de: true
            });
            // 绑定Dtu型号
            for (var j = 0; j < data[0].models.length; j++) {
                if (data[0].models[j].dtuModel != null && data[0].models[j].dtuModel != "null" && data[0].models[j].dtuModel != "") {
                    document.getElementById('addDTUModel').options.add(new Option(data[0].models[j].dtuModel, data[0].models[j].productId));
                    document.getElementById('modifyDTUModel').options.add(new Option(data[0].models[j].dtuModel, data[0].models[j].productId));
                } else {
                    document.getElementById('addDTUModel').options.add(new Option("", data[0].models[j].productId));
                    document.getElementById('modifyDTUModel').options.add(new Option("", data[0].models[j].productId));
                }
            }
            $('#addDTUFactory').change();
        },
        error: function () {

        }
    });
}
// 添加dtu界面-产品下拉列表切换
$('#addDTUFactory').change(function () {
    $('#addDTUModel option').remove();
    var name = $('#addDTUFactory').find('option:selected').text();
    for (var i = 0; i < dtuProducts.length; i++) {
        if (name == dtuProducts[i].dtuFactory) {
            for (var j = 0; j < dtuProducts[i].models.length; j++) {
                if (dtuProducts[i].models[j].dtuModel != null && dtuProducts[i].models[j].dtuModel != "null" && dtuProducts[i].models[j].dtuModel != "") {
                    document.getElementById('addDTUModel').options.add(new Option(dtuProducts[i].models[j].dtuModel, dtuProducts[i].models[j].productId));
                } else {
                    document.getElementById('addDTUModel').options.add(new Option("", dtuProducts[i].models[j].productId));
                }
            }
        }
    }
    $('#addDTUModel').change();
});
$('#addDTUModel').change(function () {
    var name = $('#addDTUFactory').find('option:selected').text();
    var model = $('#addDTUModel').find('option:selected').text();
    var type = '';
    for (var i = 0; i < dtuProducts.length; i++) {
        if (name == dtuProducts[i].dtuFactory) {
            for (var j = 0; j < dtuProducts[i].models.length; j++) {
                if (dtuProducts[i].models[j].dtuModel == model) {
                    type = dtuProducts[i].models[j].networkType;
                    $('#addConnectType').val(type);
                }
            }
        }
    }
    // 修改配置界面
    if (type.toLowerCase() == 'gprs') {
        $('.gprs-control').show();
        $('.local-control').hide();
    } else if (/(local)/m.test(type.toLowerCase())) {
        $('.local-control').show();
        $('.gprs-control').hide();
    }
});
// 修改dtu界面-产品下拉列表切换
$('#modifyDTUFactory').change(function () {
    $('#modifyDTUModel option').remove();
    var name = $('#modifyDTUFactory').find('option:selected').text();
    for (var i = 0; i < dtuProducts.length; i++) {
        if (name == dtuProducts[i].dtuFactory) {
            for (var j = 0; j < dtuProducts[i].models.length; j++) {
                if (dtuProducts[i].models[j].dtuModel != null && dtuProducts[i].models[j].dtuModel != "null" && dtuProducts[i].models[j].dtuModel != "") {
                    document.getElementById('modifyDTUModel').options.add(new Option(dtuProducts[i].models[j].dtuModel, dtuProducts[i].models[j].productId));
                } else {
                    document.getElementById('modifyDTUModel').options.add(new Option("", dtuProducts[i].models[j].productId));
                }
            }
        }
    }
    $('#modifyDTUModel').change();
});
$('#modifyDTUModel').change(function () {
    var name = $('#modifyDTUFactory').find('option:selected').text();
    var model = $('#modifyDTUModel').find('option:selected').text();
    var type = '';
    for (var i = 0; i < dtuProducts.length; i++) {
        if (name == dtuProducts[i].dtuFactory) {
            for (var j = 0; j < dtuProducts[i].models.length; j++) {
                if (dtuProducts[i].models[j].dtuModel == model) {
                    type = dtuProducts[i].models[j].networkType;
                    $('#modifyConnectType').val(type);
                }
            }
        }
    }
    // 修改配置界面
    if (type.toLowerCase() == 'gprs') {
        $('.gprs-control').show();
        $('.local-control').hide();
    } else if (/(local)/m.test(type.toLowerCase())) {
        $('.local-control').show();
        $('.gprs-control').hide();
    }
});

function DTU_datatable() {

    $('#dtuTable').dataTable({
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
        "bDestroy": true,
        "bRetrieve": true,
        "bStateSave": true,
        "aoColumnDefs": [{
            'bSortable': false,
            'aTargets': [0]
        }]
    });
}

/************************************** 增加DTU **************************************/
$('#btnAddDTU').on('click', function () {
    $('#addDTUstr').val(structName);
    //$('#addDTUip').val('223.2.212.14');
    $('#addDTUip').val('218.3.150.107');
    $('#addDTUnumber').val('');
    $('#addDTUsim').val('');
    $('#addDTUport').val('');
    document.getElementById('addDtuRepet').style.display = 'none';
    document.getElementById('addDTUportRange').style.display = 'none';
});

$("#addDTUnumber").focus(function () {
    document.getElementById('addDtuRepet').style.display = 'none';
});
$('#addDTUport').focus(function () {
    document.getElementById('addDTUportRange').style.display = 'none';
});

//DTU 增加重置
$('#btnResetDTU').on('click', function () {
    $('#addDTUnumber').val('');
    $('#addDTUsim').val('');
    $('#addDTUport').val('');
    //$('#addDTUip').val('');
});

//DTU增加
$('#btnSaveDTU').click(function () {
    var addDTUstr = document.getElementById("addDTUstr").value; //结构物
    var addDTUnumber = document.getElementById("addDTUnumber").value; //DTU编号
    var addDTUsim = document.getElementById("addDTUsim").value; //卡号
    var addDTUcollectGranularity = $('#addDTUcollectGranularity').find('option:selected').val(); //采集粒度
    var addDTUport = document.getElementById("addDTUport").value; //端口
    var addDTUip = document.getElementById("addDTUip").value; //ip
    var addDTUProduct = $('#addDTUModel').find('option:selected').val(); //产品id
    for (var i = 0; i < dtuArray.length; i++) {
        if (dtuArray[i][0] == addDTUnumber) {
            document.getElementById('addDtuRepet').style.display = 'block';
            return false;
        }
    }
    var type = $('#addConnectType').val();
    var file = $('#addDTUFile').val();
    if (!/^\d{8}$/.test(addDTUnumber) || addDTUnumber == "") {
        $('#addDTUnumber').focus();
    } else if (addDTUcollectGranularity == null) {
        alert('请选择采集粒度');
        $('#addDTUcollectGranularity').focus();
        return false;
    } else if (type.toLowerCase() == 'gprs') {
        if (!/^((\(\d{3}\))|(\d{3}\-))?18\d{9}|13\d{9}|15\d{9}$/.test(addDTUsim) || addDTUsim == "") {
            $('#addDTUsim').focus();
        } else if (!(/^(\d)+$/.test(addDTUport) && parseInt(addDTUport) <= 65535 && parseInt(addDTUport) >= 0) || addDTUport == "") {
            $('#addDTUport').focus();
            document.getElementById('addDTUportRange').style.display = 'block';
        } else if (!/^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$/.test(addDTUip) || addDTUip == "") {
            $('#addDTUip').focus();
        } else {
            var data = {
                "structId": structId,
                "dtuNo": addDTUnumber,
                "sim": addDTUsim,
                "granularity": addDTUcollectGranularity.substring(0, 2),
                "ip": addDTUip,
                "port": addDTUport,
                "productId": addDTUProduct
            };
            var url = apiurl + '/dtu/add?token=' + getCookie('token');
            $.ajax({
                //async: false,//同步
                type: 'post',
                url: url,
                data: data,
                statusCode: {
                    202: function () {
                        $('#dtuTable').dataTable().fnDestroy();
                        GetDtu(structId, structName);
                        alert('保存成功');
                        $("#addDtuModal").modal("hide");
                    },
                    403: function () {
                        alert("权限验证出错");
                        logOut();
                    },
                    400: function () {
                        alert("添加失败,DTU编号已存在");
                    },
                    500: function () {
                        alert("添加失败,DTU编号已存在");
                    },
                    404: function () {
                        alert('url错误');
                    },
                    405: function () {
                        alert('抱歉，没有增加DTU的权限');
                    }
                }
            });
        }
    } else if (/(local)/.test(type.toLowerCase())) {
        if (file == '') {
            $('#addDTUFile').focus();
        } else {
            var data = {
                "structId": structId,
                "dtuNo": addDTUnumber,
                "granularity": addDTUcollectGranularity,
                "productId": addDTUProduct,
                "p1": file
            };
            var url = apiurl + '/dtu/add?token=' + getCookie('token');
            $.ajax({
                //async: false,//同步
                type: 'post',
                url: url,
                data: data,
                statusCode: {
                    202: function () {
                        $('#dtuTable').dataTable().fnDestroy();
                        GetDtu(structId, structName);
                        alert('保存成功');
                        $("#addDtuModal").modal("hide");
                    },
                    403: function () {
                        alert("权限验证出错");
                        logOut();
                    },
                    400: function () {
                        alert("添加失败,DTU编号已存在");
                    },
                    500: function () {
                        alert("添加失败,DTU编号已存在");
                    },
                    404: function () {
                        alert('url错误');
                    },
                    405: function () {
                        alert('抱歉，没有增加DTU的权限');
                    }
                }
            });
        }
    }
});

/************************************** end 增加DTU **************************************/

/************************************** 修改 DTU **************************************/

var dtuIdModfiy;
var oldDtuNumber;
$('#dtuTable').on('click', 'a.editor_edit', function (e) {
    e.preventDefault();
    //$('#modifyDTUorg').val('公路局');
    $('#modifyDTUstr').val(structName);

    var tr = $(this).parents('tr');
    var selectedRow = tr[0];
    dtuIdModfiy = selectedRow.id.substring(4);

    // 获取dtu信息
    var url = apiurl + '/dtu/' + dtuIdModfiy + '/info?token=' + getCookie('token');
    $.ajax({
        async: false,//同步
        type: 'get',
        url: url,
        dataType: 'json',
        success: function (data) {
            if (data == null) {
                alert('dtu编号无效');
                $('#addDtuModal').hide();
                return;
            }
            $('#modifyDTUnumber').val(data.dtuNo);
            oldDtuNumber = data.dtuNo;
            var collectGranularityEdit = document.getElementById('modifyDTUcollectGranularity');
            for (var i = 0; i < collectGranularityEdit.options.length; i++) {
                if (collectGranularityEdit.options[i].value == data.granularity) {
                    collectGranularityEdit.options[i].selected = true;
                    break;
                }
            }

            // 重新绑定厂商
            $("#modifyDTUFactory").parent().children().remove('div');
            $("#modifyDTUFactory").removeClass();
            $("#modifyDTUFactory").html("");
            for (var i = 0; i < dtuProducts.length ; i++) {
                document.getElementById('modifyDTUFactory').options.add(new Option(dtuProducts[i].dtuFactory, dtuProducts[i].dtuFactory));
            }
            $("#modifyDTUFactory").addClass("chzn-select");

            $('#modifyDTUFactory').val(data.dtuFactory);
            $('#modifyDTUFactory').chosen({
                no_results_text: "没有找到",
                allow_single_de: true
            });

            // 重新绑定型号下拉列表
            $('#modifyDTUModel option').remove();
            var name = $('#modifyDTUFactory').find('option:selected').text();
            for (var i = 0; i < dtuProducts.length; i++) {
                if (name == dtuProducts[i].dtuFactory) {
                    for (var j = 0; j < dtuProducts[i].models.length; j++) {
                        if (dtuProducts[i].models[j].dtuModel != null && dtuProducts[i].models[j].dtuModel != "null") {
                            document.getElementById('modifyDTUModel').options.add(new Option(dtuProducts[i].models[j].dtuModel, dtuProducts[i].models[j].productId));
                        }
                        else {
                            document.getElementById('modifyDTUModel').options.add(new Option("", dtuProducts[i].models[j].productId));
                        }
                    }
                }
            }
            // 赋值型号
            var modifyDTUModel = document.getElementById('modifyDTUModel');
            for (var i = 0; i < modifyDTUModel.options.length; i++) {
                if (modifyDTUModel.options[i].innerHTML == data.dtuModel) {
                    modifyDTUModel.options[i].selected = true;
                    break;
                }
            }
            $('#modifyConnectType').val(data.networkType);

            // 修改配置界面
            if (data.networkType.toLowerCase() == 'gprs') {
                $('.gprs-control').show();
                $('.local-control').hide();
                $('#modifyDTUsim').val(data.sim);
                $('#modifyDTUport').val(data.port);
                $('#modifyDTUip').val(data.ip);
            } else if (/(local)/.test(data.networkType.toLowerCase())) {
                $('.local-control').show();
                $('.gprs-control').hide();
                $('#modifyDTUFile').val(data.p1);
            }

            document.getElementById('modifyDtuRepet').style.display = 'none';
            document.getElementById('modifyDTUportRange').style.display = 'none';
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("权限验证出错");
                logOut();
            } else if (XMLHttpRequest.status == 405) {
                alert('抱歉，没有修改DTU权限');
                $('#addDtuModal').hide();
            }
            else {
                alert('获取数据失败');
                $('#addDtuModal').hide();
            }
        }
    });
});
$("#modifyDTUnumber").focus(function () {
    document.getElementById('modifyDtuRepet').style.display = 'none';
});
$('#modifyDTUport').focus(function () {
    document.getElementById('modifyDTUportRange').style.display = 'none';
});

//DTU 修改重置
$('#btnResetModifyDTU').on('click', function () {
    $('#modifyDTUnumber').val('');
    $('#modifyDTUsim').val('');
    $('#modifyDTUport').val('');
})

//DTU保存修改
$('#btnSaveModifyDTU').click(function () {
    var modifyDTUnumber = document.getElementById("modifyDTUnumber").value;//DTU编号
    var modifyDTUsim = document.getElementById("modifyDTUsim").value;//卡号
    var modifyDTUcollectGranularity = $('#modifyDTUcollectGranularity').find('option:selected').val();//采集粒度
    var modifyDTUport = document.getElementById("modifyDTUport").value;//端口
    var modifyDTUip = document.getElementById("modifyDTUip").value;//ip
    var modifyDTUProduct = $('#modifyDTUModel').find('option:selected').val();// dtu产品id
    if (modifyDTUnumber != oldDtuNumber) {
        for (var i = 0; i < dtuArray.length; i++) {
            if (dtuArray[i][0] == modifyDTUnumber) {
                document.getElementById('modifyDtuRepet').style.display = 'block';
                return;
            }
        }
    }
    var type = $('#modifyConnectType').val();
    var file = $('#modifyDTUFile').val();
    if (!/^\d{8}$/.test(modifyDTUnumber) || modifyDTUnumber == "") {
        $('#modifyDTUnumber').focus();
    }
    else if (modifyDTUcollectGranularity == null) {
        alert('请选择采集粒度');
        $('#modifyDTUcollectGranularity').focus();
        return false;
    }
    else if (type.toLowerCase() == 'gprs') {
        if (!/^((\(\d{3}\))|(\d{3}\-))?18\d{9}|13\d{9}|15\d{9}$/.test(modifyDTUsim) || modifyDTUsim == "") {
            $('#modifyDTUsim').focus();
        } else if (!(/^(\d)+$/.test(modifyDTUport) && parseInt(modifyDTUport) <= 65535 && parseInt(modifyDTUport) >= 0) || modifyDTUport == "") {
            $('#modifyDTUport').focus();
            document.getElementById('modifyDTUportRange').style.display = 'block';
        } else if (!(/^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$/.test(modifyDTUip)) || modifyDTUip == "") {
            $('#modifyDTUip').focus();
        } else {
            var data = {
                "dtuNo": modifyDTUnumber,
                "sim": modifyDTUsim,
                "granularity": modifyDTUcollectGranularity,
                "ip": modifyDTUip,
                "port": modifyDTUport,
                "productId": modifyDTUProduct
            };
            var url = apiurl + '/dtu/modify/' + parseInt(dtuIdModfiy) + '?token=' + getCookie('token');
            //var url = apiurl + '/dtu/modify/' + parseInt(dtuIdModfiy) + '';
            $.ajax({
                //async: false,//同步
                type: 'post',
                url: url,
                data: data,
                statusCode: {
                    202: function () {
                        $('#dtuTable').dataTable().fnDestroy();
                        GetDtu(structId, structName);
                        alert("修改成功");
                        $("#modifyDTUModal").modal("hide");
                    },
                    403: function () {
                        alert("权限验证出错");
                        logOut();
                    },
                    400: function () {
                        alert("修改失败");
                    },
                    500: function () {
                        alert("添加失败");
                    },
                    404: function () {
                        alert('url错误');
                    },
                    405: function () {
                        alert('抱歉，没有修改DTU权限');
                    }
                }
            });
        }
    } else if (/local/.test(type.toLowerCase())) {
        if (file == '') {
            $('#modifyDTUFile').focus();
        } else {
            var data = {
                "dtuNo": modifyDTUnumber,
                "granularity": modifyDTUcollectGranularity.substring(0, 2),
                "productId": modifyDTUProduct,
                "p1": file
            };
            var url = apiurl + '/dtu/modify/' + parseInt(dtuIdModfiy) + '?token=' + getCookie('token');
            //var url = apiurl + '/dtu/modify/' + parseInt(dtuIdModfiy) + '';
            $.ajax({
                //async: false,//同步
                type: 'post',
                url: url,
                data: data,
                statusCode: {
                    202: function () {
                        $('#dtuTable').dataTable().fnDestroy();
                        GetDtu(structId, structName);
                        alert("修改成功");
                        $("#modifyDTUModal").modal("hide");
                    },
                    403: function () {
                        alert("权限验证出错");
                        logOut();
                    },
                    400: function () {
                        alert("修改失败");
                    },
                    500: function () {
                        alert("添加失败");
                    },
                    404: function () {
                        alert('url错误');
                    },
                    405: function () {
                        alert('抱歉，没有修改DTU权限');
                    }
                }
            });
        }
    }
});

/************************************** end 修改 DTU **************************************/

/************************************** 删除 DTU **************************************/
var dtuIdDelete;
$('#dtuTable').on('click', 'a.editor_delete', function (e) {
    e.preventDefault();
    var tr = $(this).parents('tr');
    var selectedRow = tr[0];
    dtuIdDelete = selectedRow.id.substring(4);
    $('#pDeleteDTU').text("确定删除DTU编号 “" + selectedRow.cells[0].innerText + "”？");

});

//DTU确定删除
$('#btnDeleteDTU').click(function () {
    var url = apiurl + '/dtu/remove/' + dtuIdDelete + '/' + structId + '?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        type: 'post',
        url: url,
        statusCode: {
            202: function () {
                $('#dtuTable').dataTable().fnDestroy();
                GetDtu(structId, structName);
                alert('删除成功');
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
                alert('抱歉，没有删除DTU权限');
            },
            409:function() {
                alert('不能删除当前DTU,请先删除该DTU下所包含的传感器');
            }
        }
    });
});
/************************************** end 删除 DTU **************************************/

//DTU下一步
function DTUnextOnclick() {
    $('#tabDtu').removeClass('active');
    $('#tab_DTU').removeClass('active');
    $('#tabSensor').addClass('active');
    $('#tab_sensor').addClass('active');
    GetSensor();
    InitFactorBox();
    //location.assign("DAI.aspx");
}


$('#btnAddDTU_exit').click(function () {
    $('#addDTUstr_exit').val(structName);

    var url_DTU_exit = apiurl + '/struct/' + structId + '/org-dtu' + '?token=' + getCookie('token');
    $.ajax({
        url: url_DTU_exit,
        type: 'get',
        cache: false,
        success: function (data) {
            var option_DTU_exit = '';
            for (var i = 0; i < data.length; i++) {
                option_DTU_exit += '<option value="' + data[i].dtuId + '">' + data[i].dtuNo + '</option>';
            }
            $('#DTU_List_exit').html(option_DTU_exit);
            $('#DTU_List_exit').selectpicker('refresh');
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
                alert("抱歉，没有增加DTU映射权限");
            }
            else {
                alert('url错误');
            }
        }
    })
})

$('#btnSaveDTU_exit').click(function () {
    var dtuId = $('#DTU_List_exit')[0].value;
    var url_DTU_add = apiurl + '/dtu-map/add/' + dtuId + '/' + structId + '?token=' + getCookie('token');
    $.ajax({
        type: 'post',
        url: url_DTU_add,
        statusCode: {
            202: function () {
                $('#dtuTable').dataTable().fnDestroy();
                GetDtu(structId, structName);
                alert('添加映射成功');
            },
            403: function () {
                alert("权限验证出错");
                logOut();
            },
            400: function () {
                alert("不可添加当前结构物下DTU");
            },
            500: function () {
                alert("内部异常");
            },
            404: function () {
                alert('url错误，请选择DTU');
            },
            405: function () {
                alert('抱歉，没有增加DTU映射的权限');
            }
        }
    })
})