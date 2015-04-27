/*********** 初始化 归属DTU ****************/
function InitDTUBox() {
    $('#addSensorDTU option').remove();
    $('#modifySensorDTU option').remove();
    for (var i = 0; i < dtuArray.length; i++) {
        document.getElementById('addSensorDTU').options.add(new Option(dtuArray[i][0], 'dtuId-' + dtuArray[i][1]));
        document.getElementById('modifySensorDTU').options.add(new Option(dtuArray[i][0], 'dtuIdmodify-' + dtuArray[i][1]));
    }
}
var oldFormulaA = null;
var oldFormulaM = null;
$(function () {
    GetSensor();
   // InitSensorTypeBox();
});
$('#tabSensor').click(function () {
    GetSensor();
    InitFactorBox();
    //getLocation();

});
//表格初始化
var moduleArray = [];
var sensorInitData;
function GetSensor(old) {
    $('#sensorTable').dataTable().fnDestroy();
    var url = apiurl + '/struct/' + structId + '/sensors?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            sensorInitData = data;
            var sb = new StringBuffer();
            for (var k = 0; k < dtuArray.length; k++) {
                moduleArray.push([dtuArray[k][0], []]);
            }
            for (var i = 0; i < data.length; i++) {
                if (data[i].identify == 0 || data[i].identify == 1) {

                    sb.append("<tr id='sensor-" + data[i].sensorId + "'><td >" + data[i].dtuNo + "</td>"); //归属DTU编号
                    sb.append("<td>" + data[i].moduleNo + "</td>"); //模块号
                    sb.append("<td>" + data[i].channel + "</td>"); //通道号
                    sb.append("<td>" + data[i].sensorType + "</td>"); //产品类型

                    if (data[i].sensorModel != null && data[i].sensorModel != "null") {
                        sb.append("<td>" + data[i].sensorModel + "</td>"); //传感器型号
                    } else {
                        sb.append("<td></td>"); //传感器型号
                    }
                    sb.append("<td>" + data[i].location + "</td>"); //传感器位置
                    if (data[i].identify == 0) {
                        sb.append("<td>" + "实体" + "</td>"); //传感器类型
                    } else {
                        sb.append("<td>" + "数据" + "</td>"); //传感器类型
                    }
                    var tmp;
                    if (data[i].enable == false)
                        tmp = '是';
                    else {
                        tmp = '否';
                    }
                    sb.append("<td>" + tmp+ "</td>"); //是否启用(2-26)
                    var str = "<td><a href='#viewSensorModal' class='editor_view' data-toggle='modal'>查看</a> |";
                    str += "<a href='#modifySensorModal' class='editor_edit' data-toggle='modal'>修改</a> | ";
                    str += "<a href='#deleteSensorModal' class='editor_delete' data-toggle='modal'>删除</a></td></tr>";
                    sb.append(str); //操作
                    for (var j = 0; j < moduleArray.length; j++) {
                        if (moduleArray[j][0].toString() == data[i].dtuNo.toString()) {
                            moduleArray[j][1].push(data[i].moduleNo + "-" + data[i].channel);
                        }
                    }
                } else {
                    sb.append("<tr id='sensor-" + data[i].sensorId + "'><td >" + data[i].dtuNo + "</td>"); //归属DTU编号
                    sb.append("<td>无</td>"); //模块号
                    sb.append("<td>无</td>"); //通道号
                    sb.append("<td>" + data[i].sensorType + "</td>"); //传感器类型
                    if (data[i].sensorModel != null && data[i].sensorModel != "null") {
                        sb.append("<td>" + data[i].sensorModel + "</td>"); //传感器型号
                    } else {
                        sb.append("<td></td>"); //传感器型号
                    }
                    sb.append("<td>" + data[i].location + "</td>"); //传感器位置
                    sb.append("<td>" + "组合" + "</td>"); //传感器类型
                    var tpm;
                    if (data[i].enable == false)
                        tpm = '是';
                    else {
                        tpm = '否';
                    }
                    sb.append("<td>" + tpm + "</td>"); //是否启用(2-26)
                    var str = "<td><a href='#viewSensorModal' class='editor_view' data-toggle='modal'>查看</a> |";
                    str += "<a href='#modifySensorModal' class='editor_edit' data-toggle='modal'>修改</a> | ";
                    str += "<a href='#deleteSensorModal' class='editor_delete' data-toggle='modal'>删除</a></td></tr>";
                    sb.append(str); //操作
                }

            }
            $('#sensorTbody').html("");
            $('#sensorTbody').append(sb.toString());
            Sensor_datatable();
            rowsSensorJudge();//判断传感器是否有已配置
            if (old) {
                InitDTUBox();
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
function Sensor_datatable() {
    $('#sensorTable').dataTable({
        "aLengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, "All"]
        ],
        // set the initial value
        "bDestroy": true,
        "bRetrieve": true,
        "iDisplayLength": 10,
        "sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
        "sPaginationType": "bootstrap",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "bStateSave": true,
        "aoColumnDefs": [{
            'bSortable': false,
            'aTargets': [0]
        }]
    });
}

//判断传感器是否有已配置
function rowsSensorJudge() {
    var table = document.getElementById("sensorTable");
    var rows = table.rows.length;//包括列头一行
    if (rows > 1) {
        $('#tabSensorGroup').show();
        initGroupTab();
        $('#tabThreshold').show();
        $('#tabValidate').show();
        document.getElementById("sensorNext").disabled = false;//可用
        $('#sensorNext').addClass('blue');
    }
    else {
        $('#tabSensorGroup').hide();
        $('#tabThreshold').hide();
        $('#tabThreshold').hide();
        document.getElementById("sensorNext").disabled = true;//不可用
        $('#sensorNext').removeClass('blue');
    }
}
//初始化传感器位置

function getLocation() {
    $("#addCorrentPosition").children().remove();
    var productTypeId = document.getElementById("addSensorType").value.toString();
    //var url = apiurl + '/struct/' + structId + '/sensors?token=' + getCookie('token');
    if (productTypeId != '') {
        var url = apiurl + '/combined-sensor/' + structId + '/' + productTypeId + '/sensorList/info?token=' + getCookie('token');
        $.ajax({
            //async: false,//同步
            url: url,
            type: 'get',
            cache: false,
            success: function(data) {
                if (data.length == 0 || data == []) {
                    return;
                }
                var cs = '';
                for (var i = 0; i < data.length; i++) {

                    cs += '<option  value=" ' + data[i].sensorId + ' ">' + data[i].location + '</option>';
                }
                //关联传感器
                $('#addCorrentPosition').html(cs);
                $('#addCorrentPosition').trigger("liszt:updated");
                $('#addCorrentPosition').chosen({
                    no_results_text: "没有找到",
                    allow_single_de: true
                });

            },
            error: function(XMLHttpRequest, textStatus, errorThrown) {
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
}


//初始化监测因素
function InitFactorBox() {
    $('#addSensorFactor option').remove();
    $('#modifySensorFactor option').remove();

    var url = apiurl + '/struct/' + structId + '/factor-config' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                return;
            }
            for (var i = 0; i < data.length; i++) {
                for (var j = 0; j < data[i].children.length; j++) {
                    if (data[i].children[j].choose) {
                        if (data[i].children[j].description != null) {
                            document.getElementById('addSensorFactor').options.add(new Option(data[i].children[j].factorName + '(' + data[i].children[j].description + ')', 'factorId-' + data[i].children[j].factorId));
                            document.getElementById('modifySensorFactor').options.add(new Option(data[i].children[j].factorName + '(' + data[i].children[j].description + ')', 'factorIdmodify-' + data[i].children[j].factorId));
                        } else {
                            document.getElementById('addSensorFactor').options.add(new Option(data[i].children[j].factorName, 'factorId-' + data[i].children[j].factorId));
                            document.getElementById('modifySensorFactor').options.add(new Option(data[i].children[j].factorName, 'factorIdmodify-' + data[i].children[j].factorId));
                        }
                    }
                }
            }
        },
        error: function() {
            alert("加载监测因素列表出错(传感器)");
        }
    });


    //var url = apiurl + '/struct/' + structId + '/factors?token=' + getCookie('token');
    //$.ajax({
    //    //async: false,//同步
    //    url: url,
    //    type: 'get',
    //    dataType: 'json',
    //    success: function (data) {
    //        if (data.length < 1) {
    //            return;
    //        }
    //        for (var i = 0; i < data.length; i++) {
    //            for (var j = 0; j < data[i].children.length; j++) {
    //                document.getElementById('addSensorFactor').options.add(new Option(data[i].children[j].factorName, 'factorId-' + data[i].children[j].factorId));
    //                document.getElementById('modifySensorFactor').options.add(new Option(data[i].children[j].factorName, 'factorIdmodify-' + data[i].children[j].factorId));
    //            }
    //        }
    //    },
    //    error: function () {

    //    }
    //})

}

$('#addSensorFactor').change(function () {
    var addSensorFactorId = $('#addSensorFactor').find('option:selected').val().substring(9);
    InitSensorTypeBox(addSensorFactorId, 'addSensorType');
    getLocation();

});

$('#modifySensorFactor').change(function () {
    var modifySensorFactor = $('#modifySensorFactor').find('option:selected').val().substring(15);
    InitSensorTypeBox(modifySensorFactor, 'modifySensorType');
    corrIdArray = [];
    getLocationWhenEdit(aModify, sensorIdModify);
});

//初始化传感器类型-型号
function InitSensorTypeBox(factorId,productType) {
    $('#addSensorType option').remove();
    $('#addSensorTypeNumber option').remove();
    $('#modifySensorType option').remove();
    $('#modifySensorTypeNumber option').remove();
    //var url = apiurl + '/sensor/product/list?token=' + getCookie('token');
    
    var url = apiurl + '/factor/' + factorId + '/correlate-product-type/info?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        dataType: 'json',
        cache:false,
        success: function (data) {
            if (data.length < 1) {
                return;
            }

            productData = data;
            for (var i = 0; i < data.length; i++) {
                document.getElementById(productType).options.add(new Option(data[i].productName, data[i].productTypeId[0].product_TypeId));
                //document.getElementById('modifySensorType').options.add(new Option(data[i].productName, data[i].productName));
            }
            //$('#addSensorType').chosen({
            //    no_results_text: "没有找到",
            //    allow_single_de: true
            //});
            //$('#modifySensorType').chosen({
            //    no_results_text: "没有找到",
            //    allow_single_de: true
            //});
            var indexFormula;
            for (var j = 0; j < data[0].models.length; j++) {
                if (j == 0) {
                    indexFormula = data[0].models[j].productId;
                }
                if (data[0].models[j].productCode != null && data[0].models[j].productCode != "null" && data[0].models[j].productCode != "") {
                    document.getElementById('addSensorTypeNumber').options.add(new Option(data[0].models[j].productCode, 'productId-' + data[0].models[j].productId));
                    document.getElementById('modifySensorTypeNumber').options.add(new Option(data[0].models[j].productCode, 'productIdmodify-' + data[0].models[j].productId));
                }
                else {
                    document.getElementById('addSensorTypeNumber').options.add(new Option("", 'productId-' + data[0].models[j].productId));
                    document.getElementById('modifySensorTypeNumber').options.add(new Option("", 'productIdmodify-' + data[0].models[j].productId));
                }               
            }
            InitSensorFormulaBoxAdd(indexFormula);
            InitSensorFormulaBoxModify(indexFormula);
        },
        error: function () {

        }
    });
}

$('#addSensorType').change(function() {
    $('#addSensorTypeNumber option').remove();
    var name = $('#addSensorType').find('option:selected').text();
    var indexFormula;
    for (var i = 0; i < productData.length; i++) {
        if (name == productData[i].productName) {
            for (var j = 0; j < productData[i].models.length; j++) {
                if (j == 0) {
                    indexFormula = productData[i].models[j].productId;
                }
                if (productData[i].models[j].productCode != null && productData[i].models[j].productCode != "null" && productData[i].models[j].productCode != "") {
                    document.getElementById('addSensorTypeNumber').options.add(new Option(productData[i].models[j].productCode, 'productId-' + productData[i].models[j].productId));
                } else {
                    document.getElementById('addSensorTypeNumber').options.add(new Option("", 'productId-' + productData[i].models[j].productId));
                }
            }
        }
    }
    InitSensorFormulaBoxAdd(indexFormula);
});

$('#modifySensorType').change(function() {
    $('#modifySensorTypeNumber option').remove();
    var name = $('#modifySensorType').find('option:selected').text();
    var indexFormula;
    for (var i = 0; i < productData.length; i++) {
        if (name == productData[i].productName) {
            for (var j = 0; j < productData[i].models.length; j++) {
                if (j == 0) {
                    indexFormula = productData[i].models[j].productId;
                }
                if (productData[i].models[j].productCode != null && productData[i].models[j].productCode != "null" && productData[i].models[j].productCode != "") {
                    document.getElementById('modifySensorTypeNumber').options.add(new Option(productData[i].models[j].productCode, 'productIdModify-' + productData[i].models[j].productId));
                } else {
                    document.getElementById('modifySensorTypeNumber').options.add(new Option("", 'productIdModify-' + productData[i].models[j].productId));
                }
            }
        }
    }
    InitSensorFormulaBoxModify(indexFormula);
});

//初始化计算公式
function InitSensorFormulaBoxAdd(indexFormula) {
    $('#addSensorTable').dataTable().fnDestroy();
    //var url = apiurl + '/struct/' + structId + '/sensors?token=' + getCookie('token');
    var url = apiurl + '/sensor/product/' + indexFormula + '?token=' + getCookie('token');;
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            if (data.formula != null) {
                if (data.formula == oldFormulaA) {
                    return;
                }

                $('#addSensorFormula').val(data.formula);
                var sb = new StringBuffer();
                for (var i = 0; i < data.params.length; i++) {
                    sb.append("<tr id='ID-" + data.params[i].id + "'><td >" + data.params[i].key + "</td><td><input class='tdAformula' type='text'/></td>");//计算公式参数
                }

                $('#addSensorTbody').html("");
                $('#addSensorTbody').append(sb.toString());
            }
            else {
                $('#addSensorFormula').val("无");
                $('#addSensorTbody').html("");
                var sb = new StringBuffer();
                sb.append("");
                $('#addSensorTbody').append(sb.toString());
            }
            oldFormulaA = data.formula;
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
function InitSensorFormulaBoxModify(indexFormula) {
    $('#modifySensorTable').dataTable().fnDestroy();
    //var url = apiurl + '/struct/' + structId + '/sensors?token=' + getCookie('token');
    var url = apiurl + '/sensor/product/' + indexFormula + '?token=' + getCookie('token');;
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            if (data.formula != null) {
                if (data.formula == oldFormulaM) {
                    return;
                }

                $('#modifySensorFormula').val(data.formula);
                var sb = new StringBuffer();
                for (var i = 0; i < data.params.length; i++) {
                    sb.append("<tr id='IDModify-" + data.params[i].id + "'><td >" + data.params[i].key + "</td><td><input class='tdMformula' type='text'/></td>");//计算公式参数
                }

                $('#modifySensorTbody').html("");
                $('#modifySensorTbody').append(sb.toString());
            }
            else {
                $('#modifySensorFormula').val("无");
                $('#modifySensorTbody').html("");
                var sb = new StringBuffer();
                sb.append("");
                $('#modifySensorTbody').append(sb.toString());
            }
            oldFormulaM = data.formula;
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


$('#addSensorTypeNumber').change(function() {
    var id = $('#addSensorTypeNumber').find('option:selected').val().substring(10); //
    InitSensorFormulaBoxAdd(id);
});

$('#modifySensorTypeNumber').change(function() {
    var id = $('#modifySensorTypeNumber').find('option:selected').val().substring(16); //
    InitSensorFormulaBoxModify(id);
});


/****************** 增加传感器 **************************************/
$('#btnAddSensor').on('click', function () {
    var addSensorFactorId = $('#addSensorFactor').find('option:selected').val().substring(9);
    InitSensorTypeBox(addSensorFactorId, 'addSensorType');
    getLocation();
    $('#addSensorModule').val('');
    $('#addSensorChannelNumber').val('');
    $('#addSensorPosition').val('');
    document.getElementById('addSensorRepet').style.display = 'none';
    document.getElementById('addSensorChannelRange').style.display = 'none';
    $('#addSensorIdentification option[value="0"]').attr("Selected", true);
    $('#addControl option[value="0"]').attr("Selected", true);//2-26
    $('#addSensorIdentification').change(function() {
        var identifiction = document.getElementById("addSensorIdentification").value;
        if (identifiction == 2) {
            $('#dtuNo').show();
            document.getElementById('module').style.display = 'none';
            document.getElementById('channel').style.display = 'none';
            $('#corrent').show();
            var a = document.getElementById('addCorrentPosition');
            RemoveSelectedItem(a);
            $('#addCorrentPosition').trigger("liszt:updated");
            getLocation();
        } else {
            $('#dtuNo').show();
            $('#module').show();
            $('#channel').show();
            $('#corrent').hide();

        }
    });
    $('#addSensorIdentification').change();
    
    oldFormulaA = $('#addSensorFormula').val();
});

$("#addSensorModule").focus(function () {
    document.getElementById('addSensorRepet').style.display = 'none';
});
$('#addSensorChannelNumber').focus(function () {
    document.getElementById('addSensorChannelRange').style.display = 'none';
    document.getElementById('addSensorRepet').style.display = 'none';
});

//传感器增加重置
$('#btnResetSensor').on('click', function() {
    $('#addSensorModule').val('');
    $('#addSensorChannelNumber').val('');
    $('#addSensorPosition').val('');
    $('.tdAformula').val('');
    $('#addSensorIdentification option[value="0"]').attr("Selected", true);

    $('#addSensorIdentification').change(function() {
        var identifiction = document.getElementById("addSensorIdentification").value;
        if (identifiction == 2) {
            $('#dtuNo').show();
            document.getElementById('module').style.display = 'none';
            document.getElementById('channel').style.display = 'none';
            $('#corrent').show();
            var a = document.getElementById('addCorrentPosition');
            RemoveSelectedItem(a);
            $('#addCorrentPosition').trigger("liszt:updated");
            getLocation();
        } else {
            $('#dtuNo').show();
            $('#module').show();
            $('#channel').show();
            $('#corrent').hide();

        }
    });
    $('#addSensorIdentification').change();

    var a = document.getElementById('addCorrentPosition');
    RemoveSelectedItem(a);
    $('#addCorrentPosition').trigger("liszt:updated");
});

//传感器增加并关闭
$('#btnSaveCloseSensor').click(function() {
    sensorSave(0);
});

function sensorSave(param) {
    if ($('#addSensorFactor').val() == null) {
        alert("还没有配置监测因素!");
        return false;
    }
    var IdentifyNo = document.getElementById('addSensorIdentification').value; //传感器标识
    var addSensorFactor = $('#addSensorFactor').find('option:selected').text(); //监测因素
    var addSensorFactorId = $('#addSensorFactor').find('option:selected').val().substring(9);
    var addSensorDTU = $('#addSensorDTU').find('option:selected').text(); //归属DTU编号
    var addSensorDTUId = $('#addSensorDTU').find('option:selected').val().substring(6);
    var addSensorModule = document.getElementById("addSensorModule").value; //模块号
    var addSensorChannelNumber = document.getElementById("addSensorChannelNumber").value; //通道号
    var addSensorTypeNumberId = $('#addSensorTypeNumber').find('option:selected').val().substring(10);
    var virtualSensorNumber = $('#addSensorTypeNumber').find('option:selected').text().split("-")[1];
   
    var addSensorPosition = document.getElementById("addSensorPosition").value; //传感器位置
    var addSensorFormula = document.getElementById("addSensorFormula").value; //计算公式
    var addSensorControl = $('#addControl').find('option:selected').text();//是否启用 2-26
    var tmp=false;
    if (addSensorControl == '是')
        tmp = false;
    else
        tmp = true;
    var addCorrentID = '';
    //实体和数据
    if (IdentifyNo == 0 || IdentifyNo == 1) {
        if (addSensorFactor == "") {
            alert('请选择监测因素');
            $('#addSensorFactor').focus();
            return false;
        } else if (addSensorDTU == "") {
            alert('请选择DTU编号');
            $('#addSensorDTU').focus();
            return false;
        }
        if (!/^(\d)+$/.test(parseInt(addSensorModule)) || addSensorModule == "") {
            $('#addSensorModule').focus();
        } else if (!(/^(\d)+$/.test(parseInt(addSensorChannelNumber)) && parseInt(addSensorChannelNumber) <= 32 && parseInt(addSensorChannelNumber) > 0) || addSensorChannelNumber == "" || addSensorChannelNumber == "1~32的通道号") {
            $('#addSensorChannelNumber').focus();
            document.getElementById('addSensorChannelRange').style.display = 'block';
        } else if ($('#addSensorType').val() == null) {
            alert('请选择传感器类型');
            $('#addSensorType').focus();
            return false;
        } else if ($('#addSensorTypeNumber').val() == null) {
            alert('请选择传感器型号');
            $('#addSensorTypeNumber').focus();
            return false;
        } else if (virtualSensorNumber == "V") {
            alert('请重新选择产品型号，该型号是组合传感器的产品型号！');
            $('#addSensorTypeNumber').focus();
            return false;
        }
        else if (addSensorPosition.replace(/(^\s*)|(\s*$)/g, '') == "") {
            alert('请填写传感器位置');
            $('#addSensorPosition').focus();
            return false;
        } else {
            for (var i = 0; i < moduleArray.length; i++) {
                if (moduleArray[i][0].toString() == addSensorDTU.toString()) {
                    for (var j = 0; j < moduleArray[i][1].length; j++) {
                        if (moduleArray[i][1][j].toString() == addSensorModule + "-" + addSensorChannelNumber) {
                            document.getElementById('addSensorRepet').style.display = 'block';
                            return;
                        }
                    }
                }
            }
            var fparam = [];
            for (var z = 1; z < document.getElementById("addSensorTable").rows.length; z++) {
                var id;
                var value;
                var a = { "id": id, "value": value };
                a.id = document.getElementById("addSensorTable").rows[z].id.substring(3);
                var b = document.getElementById("addSensorTable").rows[z].cells[1];
                a.value = document.getElementById("addSensorTable").rows[z].cells[1].children[0].value;
                if (a.value.length > 0) {
                    if (isNaN(a.value)) {
                        alert("参数值只能是数字");
                        return;
                    }
                }
                fparam.push(a);
            }
            var data = {
                "structId": structId,
                "factorId": parseInt(addSensorFactorId),
                "dtuId": parseInt(addSensorDTUId),
                "moduleNo": parseInt(addSensorModule),
                "channel": parseInt(addSensorChannelNumber),
                "productId": parseInt(addSensorTypeNumberId),
                "location": encodeURIComponent(addSensorPosition),
                "params": fparam,
                "identify": parseInt(IdentifyNo),
                //2-26
                "enable":tmp, 
                "correntId": addCorrentID,
            };
           
            var url = apiurl + '/sensor/add?token=' + getCookie('token');
            $.ajax({
                //async: false,//同步
                type: 'post',
                url: url,
                data: data,
                statusCode: {
                    202: function() {
                        $('#sensorTable').dataTable().fnDestroy();
                        GetSensor(true);
                        alert('保存成功');
                        if (param == 0) {
                            $('#btnCloseSensor').click();
                        }
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
                    405: function () {
                        alert('抱歉，没有增加传感器权限');
                    }
                }
            });
        }
    } else {
        var Corrent_Id = '';
        var corrtags = document.getElementById('addCorrentPosition_chzn').getElementsByTagName('a');
        for (i = 0; i < corrtags.length; i++) {
            var optsidx = corrtags[i].getAttribute('rel');
            Corrent_Id += document.getElementById('addCorrentPosition').options[optsidx].value + ',';
        }

//        var CorrentID = $('#addCorrentPosition').find('option:selected');
//        var Corrent_Id = '';
//        for (var i = 0; i < CorrentID.length; i++) {
//            Corrent_Id += CorrentID[i].value + ',';
//        }
        Corrent_Id = Corrent_Id.substring(0, Corrent_Id.length - 1); //去掉结尾的逗号
        if (addSensorFactor == "") {
            alert('请选择监测因素');
            $('#addSensorFactor').focus();
            return false;
        }
        else if (addSensorDTU == "") {
            alert('请选择DTU编号');
            $('#addSensorDTU').focus();
            return false;
        } else if ($('#addSensorType').val() == null) {
            alert('请选择传感器类型');
            $('#addSensorType').focus();
            return false;
        } else if ($('#addSensorTypeNumber').val() == null) {
            alert('请选择传感器型号');
            $('#addSensorTypeNumber').focus();
            return false;
        } else if (virtualSensorNumber != "V") {
            alert('请重新选择产品型号，组合传感器的产品型号中间只含有"V"!');
            $('#addSensorTypeNumber').focus();
            return false;
        }
        else if ($('#addCorrentPosition').val() == null) {
            alert('请选择关联传感器');
            $('#addCorrentPosition').focus();
            return false;
        } else if (addSensorPosition.replace(/(^\s*)|(\s*$)/g, '') == "") {
            alert('请填写传感器位置');
            $('#addSensorPosition').focus();
            return false;
        } else {
            var fparam = [];
            for (var z = 1; z < document.getElementById("addSensorTable").rows.length; z++) {
                var id;
                var value;
                var a = { "id": id, "value": value };
                a.id = document.getElementById("addSensorTable").rows[z].id.substring(3);
                var b = document.getElementById("addSensorTable").rows[z].cells[1];
                a.value = document.getElementById("addSensorTable").rows[z].cells[1].children[0].value;
                if (a.value.length > 0) {
                    if (isNaN(a.value)) {
                        alert("参数值只能是数字");
                        return;
                    }
                }
                fparam.push(a);
            }
            var data = {
                "structId": structId,
                "factorId": parseInt(addSensorFactorId),
                "dtuId": parseInt(addSensorDTUId),
                "moduleNo": null,
                "channel": null,
                "productId": parseInt(addSensorTypeNumberId),
                "location": encodeURIComponent(addSensorPosition),
                "params": fparam,
                "identify": parseInt(IdentifyNo),
                //2-26
                "enable": tmp,
                "correntId": Corrent_Id,
            };
            console.log(data);
            var url = apiurl + '/sensor/add?token=' + getCookie('token');
            $.ajax({
                //async: false,//同步
                type: 'post',
                url: url,
                data: data,
                statusCode: {
                    202: function() {
                        $('#sensorTable').dataTable().fnDestroy();
                        GetSensor(true);
                        alert('保存成功');
                        if (param == 0) {
                            $('#btnCloseSensor').click();
                        }
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
                    405: function () {
                        alert('抱歉，没有增加传感器权限');
                    }
                }
            });
        }
    }
}

//传感器增加
$('#btnSaveSensor').click(function () {
    sensorSave(1);
    getLocation();

});

/************************************** end 增加传感器**************************************/


/******************************修改 传感器*****************************/
var oldSensorModuleNo;
var oldSensorChannel;
var sensorIdModify;
$('#sensorTable').on('click', 'a.editor_edit', function(e) {
    e.preventDefault();
    var tr = $(this).parents('tr');
    var selectedRow = tr[0];
    sensorIdModify = selectedRow.id.substring(7);
    modifySensorInfo(sensorIdModify);
    document.getElementById('modifySensorRepet').style.display = 'none';
    document.getElementById('modifySensorChannelRange').style.display = 'none';
});

var aModify = '';
function modifySensorInfo(sensorId) {
    var url = apiurl + '/sensor/' + sensorId + '/info?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        cache: false,
        success: function(data) {
            var a = data.identify;
            aModify = a;
            getCorrentSensorId(sensorId);
           // getLocationWhenEdit(a, sensorId);

            var modifySensorFactor = document.getElementById('modifySensorFactor');
            for (var i = 0; i < modifySensorFactor.options.length; i++) {
                if (modifySensorFactor.options[i].innerHTML == data.factorName) {
                    modifySensorFactor.options[i].selected = true;
                    break;
                }
            }
	    
	    var modifySensorFactorId = $('#modifySensorFactor').find('option:selected').val().substring(15);
            InitSensorTypeBox(modifySensorFactorId, 'modifySensorType');
	    
            //2-26 
            var tpm="";
            if (data.enable == false)
                tpm = '是';
            else
                tpm = '否';
            var modifySensorControl = document.getElementById("modifyControl");
            for (var i = 0; i < modifySensorControl.options.length; i++) {
                if (modifySensorControl.options[i].innerHTML == tpm) {
                    modifySensorControl.options[i].selected = true;
                    break;
                }
            }
            var modifySensorDTU = document.getElementById('modifySensorDTU');
            for (var i = 0; i < modifySensorDTU.options.length; i++) {
                if (modifySensorDTU.options[i].innerHTML == data.dtuNo) {
                    modifySensorDTU.options[i].selected = true;
                    break;
                }
            }
            $('#modifySensorModule').val(data.moduleNo);
            oldSensorModuleNo = data.moduleNo;
            $('#modifySensorChannelNumber').val(data.channel);
            oldSensorChannel = data.channel;
            var modifySensorType = document.getElementById('modifySensorType');
            for (var i = 0; i < modifySensorType.options.length; i++) {
                if (modifySensorType.options[i].innerHTML == data.sensorType) {
                    //modifySensorType.options[i].selected = true;
                    $("#modifySensorType").parent().children().remove('div');
                    $("#modifySensorType").removeClass();
                    $("#modifySensorType").html("");
                    for (var i = 0; i < productData.length; i++) {
                        document.getElementById('modifySensorType').options.add(new Option(productData[i].productName, productData[i].productTypeId[0].product_TypeId));
                    }
                    $("#modifySensorType").addClass("chzn-select");

                    $('#modifySensorType').val(data.sensorType);
                    getLocationWhenEdit(a, sensorId);
                    //$('#modifySensorType').chosen({
                    //    no_results_text: "没有找到",
                    //    allow_single_de: true
                    //});
                    break;
                }
            }
            for (var i = 0; i < modifySensorType.options.length; i++) {
                if (modifySensorType.options[i].innerHTML == data.sensorType) {
                    modifySensorType.options[i].selected = true;
                    break;
                }
            }

            $('#modifySensorTypeNumber option').remove();
            var name = $('#modifySensorType').find('option:selected').text();
            var indexFormula;
            for (var i = 0; i < productData.length; i++) {
                if (name == productData[i].productName) {
                    for (var j = 0; j < productData[i].models.length; j++) {
                        if (j == 0) {
                            indexFormula = productData[i].models[j].productId;
                        }
                        if (productData[i].models[j].productCode != null && productData[i].models[j].productCode != "null") {
                            document.getElementById('modifySensorTypeNumber').options.add(new Option(productData[i].models[j].productCode, 'productIdModify-' + productData[i].models[j].productId));
                        } else {
                            document.getElementById('modifySensorTypeNumber').options.add(new Option("", 'productIdModify-' + productData[i].models[j].productId));
                        }
                    }
                }
            }
            var modifySensorTypeNumber = document.getElementById('modifySensorTypeNumber');
            for (var i = 0; i < modifySensorTypeNumber.options.length; i++) {
                if (modifySensorTypeNumber.options[i].innerHTML == data.sensorModel) {
                    modifySensorTypeNumber.options[i].selected = true;
                    break;
                }
            }
            $('#modifySensorPosition').val(data.location);
            if (data.identify == 0) {
                $('#modifyIdentity').val('实体');
            } else if (data.identify == 1) {
                $('#modifyIdentity').val('数据');
            } else {
                $('#modifyIdentity').val('组合');
            }
            if (data.formula == null) {
                $('#modifySensorFormula').val("无");
                $('#modifySensorTbody').html("");
                var sb = new StringBuffer();
                sb.append("");
                $('#modifySensorTbody').append(sb.toString());
            } else {
                $('#modifySensorTable').dataTable().fnDestroy();
                $('#modifySensorFormula').val(data.formula);
                oldFormulaM = data.formula;
                var sb = new StringBuffer();
                for (var i = 0; i < data.params.length; i++) {
                    sb.append("<tr id='IDModify-" + data.params[i].id + "'><td >" + data.params[i].key + "</td><td><input class='tdMformula' type='text'value='" + data.params[i].value + "'/></td>"); //计算公式参数
                }
                $('#modifySensorTbody').html("");
                $('#modifySensorTbody').append(sb.toString());
            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
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

//获取选择的关联的id
var corrIdArray = new Array();

function getCorrentSensorId(sensorId) {
    $('#modifySensor').children().remove();
    corrIdArray = [];

    var url = apiurl + '/correntSensor/' + sensorId + '/info?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        cache: false,
        success: function(data) {
            for (var i = 0; i < data.length; i++) {
                corrIdArray.push(data[i].correntId);
            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
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

//绑定关联的
function getLocationWhenEdit(a, sensorId) {
    var modfiyProductTypeId = document.getElementById("modifySensorType").value.toString();

    //var url = apiurl + '/struct/' + structId + '/sensors?token=' + getCookie('token');
    var url = apiurl + '/combined-sensor/' + structId + '/' + modfiyProductTypeId + '/sensorList/info?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        cache: false,
        success: function(data) {
            if (a == 0 || a == 1) {
                $('#modifyCorrent').hide();
                $('#modifyDTU').show();
                $('#modifyModule').show();
                $('#modifyChannel').show();
            } else {
                $('#modifyCorrent').show();
                $('#modifyDTU').show();
                $('#modifyModule').hide();
                $('#modifyChannel').hide();
                var cs = '';
                var i = '';
                var csc = new Array();
                for (i = 0; i < data.length; i++) {
                    for (var j = 0; j < corrIdArray.length; j++) {
                        if (data[i].sensorId == corrIdArray[j]) {
                            csc[j] = i;
                            data[i].selected = true; //缓存状态
                            break;
                        }
                    }
                    if (data[i].sensorId == sensorId) { //保证不和自己关联
                        cs += '<option  value=" ' + data[i].sensorId + ' " style="display:none;">' + data[i].location + '</option>';

                    } else {
                        if (!(data[i].selected)) {
                            cs += '<option  value=" ' + data[i].sensorId + ' ">' + data[i].location + '</option>';
                        }
                    }
                }

                for (i = 0; i < corrIdArray.length; i++) {
                    cs += '<option selected="selected" value=" ' + data[csc[i]].sensorId + ' ">' + data[csc[i]].location + '</option>';
                }
                $('#modifySensor').html(cs);
                $('#modifySensor').trigger("liszt:updated");
                $('#modifySensor').chosen({
                    no_results_text: "没有找到",
                    allow_single_de: true
                });

            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
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

$("#modifySensorModule").focus(function () {
    document.getElementById('modifySensorRepet').style.display = 'none';
});
$('#modifySensorChannelNumber').focus(function () {
    document.getElementById('modifySensorChannelRange').style.display = 'none';
    document.getElementById('modifySensorRepet').style.display = 'none';
});

//传感器修改重置
$('#btnResetModifySensor').on('click', function() {
    $('#modifySensorModule').val('');
    $('#modifySensorChannelNumber').val('');
    $('#modifySensorPosition').val('');
    $('.tdMformula').val('');
    var a = document.getElementById('modifySensor');
    RemoveSelectedItem(a);
    $('#modifySensor').trigger("liszt:updated");
});
//传感器修改
$('#btnSaveCloseModifySensor').click(function() {
    var modifySensorFactor = $('#modifySensorFactor').find('option:selected').text(); //监测因素
    var modifySensorFactorId = $('#modifySensorFactor').find('option:selected').val().substring(15);

    var modifySensorDTU = $('#modifySensorDTU').find('option:selected').text(); //归属DTU编号
    var modifySensorDTUId = $('#modifySensorDTU').find('option:selected').val().substring(12);

    var modifySensorModule = document.getElementById("modifySensorModule").value; //模块号
    var modifySensorChannelNumber = document.getElementById("modifySensorChannelNumber").value; //通道号

    var modifySensorTypeNumber = $('#modifySensorTypeNumber').find('option:selected').val().substring(16);
    var virtualSensorNumber = $('#modifySensorTypeNumber').find('option:selected').text().split("-")[1];

    var modifySensorPosition = document.getElementById("modifySensorPosition").value; //传感器位置
    var modifySensorFormula = document.getElementById("modifySensorFormula").value; //计算公式 
    var modifyIdentify = document.getElementById('modifyIdentity').value;
    var modifySensorControl = $('#modifyControl').find('option:selected').text();//2-26
    var tmp = false;
    if (modifySensorControl == '是')
        tmp = false;
    else {
        tmp = true;
    }
    var modifyCorrentID = '';
    var Identify = '';
    if (modifyIdentify == "实体") {
        Identify = 0;
    } else if (modifyIdentify == "数据") {
        Identify = 1;
    } else {
        Identify = 2;
    }
    if (Identify == 0 || Identify == 1) {

        if (modifySensorFactor == "") {
            alert('请选择监测因素');
            $('#modifySensorFactor').focus();
            return false;
        } else if (modifySensorDTU == "") {
            alert('请选择DTU编号');
            $('#modifySensorDTU').focus();
            return false;
        } else if (virtualSensorNumber == "V") {
            alert('请重新选择产品型号，该型号是组合传感器的产品型号!');
            $('#modifySensorTypeNumber').focus();
            return false;
        }
        if (!/^(\d)+$/.test(parseInt(modifySensorModule)) || modifySensorModule == "") {
            $('#modifySensorModule').focus();
        } else if (!(/^(\d)+$/.test(parseInt(modifySensorChannelNumber)) && parseInt(modifySensorChannelNumber) <= 32 && parseInt(modifySensorChannelNumber) > 0) || modifySensorChannelNumber == "" || modifySensorChannelNumber == "1~32的通道号") {
            $('#modifySensorChannelNumber').focus();
            document.getElementById('modifySensorChannelRange').style.display = 'block';
        } else if (modifySensorTypeNumber == "") {
            alert('请选择传感器型号');
            $('#modifySensorTypeNumber').focus();
            return false;
        } else if (modifySensorPosition == "") {
            alert('请填写传感器位置');
            $('#modifySensorPosition').focus();
            return false;
        } else {
            if (modifySensorModule + "-" + modifySensorChannelNumber != oldSensorModuleNo + "-" + oldSensorChannel) {
                for (var i = 0; i < moduleArray.length; i++) {
                    if (moduleArray[i][0].toString() == modifySensorDTU.toString()) {
                        for (var j = 0; j < moduleArray[i][1].length; j++) {
                            if (moduleArray[i][1][j].toString() == modifySensorModule + "-" + modifySensorChannelNumber) {
                                document.getElementById('modifySensorRepet').style.display = 'block';
                                return;
                            }
                        }
                    }
                }
            }
            var fparam = [];
            for (var z = 1; z < document.getElementById("modifySensorTable").rows.length; z++) {
                var id;
                var value;
                var a = { "id": id, "value": value };
                a.id = document.getElementById("modifySensorTable").rows[z].id.substring(9);
                var b = document.getElementById("modifySensorTable").rows[z].cells[1];
                a.value = document.getElementById("modifySensorTable").rows[z].cells[1].children[0].value;
                if (a.value.length > 0) {
                    if (isNaN(a.value)) {
                        alert("参数值只能是数字");
                        return;
                    }
                }
                fparam.push(a);
            }

            var data = {
                "structId": structId,
                "factorId": parseInt(modifySensorFactorId),
                "dtuId": parseInt(modifySensorDTUId),
                "moduleNo": parseInt(modifySensorModule),
                "channel": parseInt(modifySensorChannelNumber),
                "productId": parseInt(modifySensorTypeNumber),
                "location": encodeURIComponent(modifySensorPosition),
                "params": fparam,
                "identify": parseInt(Identify),
                "Enable":tmp,
                "correntId": modifyCorrentID,
            };
            var url = apiurl + '/sensor/modify/' + sensorIdModify + '?token=' + getCookie('token');
            $.ajax({
                //async: false,//同步
                type: 'post',
                url: url,
                data: data,
                statusCode: {
                    202: function() {
                        $('#sensorTable').dataTable().fnDestroy();
                        GetSensor();
                        alert('保存成功');
                        $("#modifySensorModal").modal("hide");
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
                    405: function () {
                        alert('抱歉，没有修改传感器权限');
                    }
                }
            });


        }
    } else {
        var Corrent_Id = '';
        var corrtags = document.getElementById('modifySensor_chzn').getElementsByTagName('a');
        for (i = 0; i < corrtags.length; i++) {
            var optsidx = corrtags[i].getAttribute('rel');
            Corrent_Id += document.getElementById('modifySensor').options[optsidx].value + ',';
        }
//        var correntId = $('#modifySensor').find('option:selected');
//        var Corrent_Id = '';
//        for (var i = 0; i < correntId.length; i++) {
//            Corrent_Id += correntId[i].value + ',';
//        }
        Corrent_Id = Corrent_Id.substring(0, Corrent_Id.length - 1); //去掉结尾的逗号
        if (modifySensorFactor == "") {
            alert('请选择监测因素');
            $('#modifySensorFactor').focus();
            return false;
        } else if (modifySensorDTU == "") {
            alert('请选择DTU编号');
            $('#modifySensorDTU').focus();
            return false;
        }
        else if (modifySensorTypeNumber == "") {
            alert('请选择传感器型号');
            $('#modifySensorTypeNumber').focus();
            return false;
        } else if (virtualSensorNumber != "V") {
            alert('请重新选择产品型号，组合传感器的产品型号中间只含有"V"!');
            $('#modifySensorTypeNumber').focus();
            return false;
        }
        else if ($('#modifySensor').val() == null) {
            alert('请选择关联传感器');
            $('#modifySensor').focus();
            return false;
        } else if (modifySensorPosition == "") {
            alert('请填写传感器位置');
            $('#modifySensorPosition').focus();
            return false;
        } else {
            var fparam = [];
            for (var z = 1; z < document.getElementById("modifySensorTable").rows.length; z++) {
                var id;
                var value;
                var a = { "id": id, "value": value };
                a.id = document.getElementById("modifySensorTable").rows[z].id.substring(9);
                var b = document.getElementById("modifySensorTable").rows[z].cells[1];
                a.value = document.getElementById("modifySensorTable").rows[z].cells[1].children[0].value;
                if (a.value.length > 0) {
                    if (isNaN(a.value)) {
                        alert("参数值只能是数字");
                        return;
                    }
                }
                fparam.push(a);
            }

            var data = {
                "structId": structId,
                "factorId": parseInt(modifySensorFactorId),
                "dtuId": parseInt(modifySensorDTUId),
                "moduleNo": null,
                "channel": null,
                "productId": parseInt(modifySensorTypeNumber),
                "location": encodeURIComponent(modifySensorPosition),
                "params": fparam,
                "identify": parseInt(Identify),
                //2-16
                "Enable": tmp,
                "correntId": Corrent_Id,
            };
            var url = apiurl + '/sensor/modify/' + sensorIdModify + '?token=' + getCookie('token');
            $.ajax({
                //async: false,//同步
                type: 'post',
                url: url,
                data: data,
                statusCode: {
                    202: function() {
                        $('#sensorTable').dataTable().fnDestroy();
                        GetSensor();
                        alert('保存成功');
                        $("#modifySensorModal").modal("hide");
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
                    405: function () {
                        alert('抱歉，没有修改传感器权限');
                    }
                }
            });
        }
    }
});
/****************************** end 修改 传感器*****************************/


/*************************查看传感器************************************/
$('#sensorTable').on('click', 'a.editor_view', function (e) {
    e.preventDefault();
    var tr = $(this).parents('tr');
    var selectedRow = tr[0];
    var sensorIdView = selectedRow.id.substring(7);
    var factorName;
    var productId;
    var viewSensorFactor = document.getElementById('viewSensorFactor');
    for (var j = 0; j < sensorInitData.length; j++) {
        if (sensorIdView == sensorInitData[j].sensorId.toString()) {
            factorName = sensorInitData[j].factorName;
            productId = sensorInitData[j].productId;
        }
    }
    $('#viewSensorFactor').text(factorName);
    $('#viewSensorDTU').text(selectedRow.cells[0].innerText);
    $('#viewSensorModule').text(selectedRow.cells[1].innerText);
    $('#viewSensorChannel').text(selectedRow.cells[2].innerText);
    $('#viewSensorType').text(selectedRow.cells[3].innerText);
    $('#viewSensorTypeNumber').text(selectedRow.cells[4].innerText);
    $('#viewSensorPosition').text(selectedRow.cells[5].innerText);
    $("#viewControl").text(selectedRow.cells[7].innerText);//2-26

   viewSensorInfo(sensorIdView);
    
});

function viewSensorInfo(sensorId) {
    var url = apiurl + '/sensor/' + sensorId + '/info?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        cache: false,
        success: function(data) {

            if (data.identify == 0) {
                $('#viewIdentification').text('实体');
            } else if (data.identify == 1) {
                $('#viewIdentification').text('数据');
            } else {
                $('#viewIdentification').text('组合');
            }
            if (data.formula == null) {
                $('#viewSensorFormula').text("无");
                $('#viewSensorParam').text("无");
            } else {
                $('#viewSensorFormula').text(data.formula);
                var sbInfoContent = new StringBuffer();
                for (var i = 0; i < data.params.length; i++) {
                    if (i == data.params.length - 1) {
                        sbInfoContent.append(sbInfoContent.append("" + data.params[i].key + ":" + data.params[i].value + " "));
                    } else {
                        sbInfoContent.append("" + data.params[i].key + ":" + data.params[i].value + "；");
                    }
                }
                $('#viewSensorParam').html('');
                $('#viewSensorParam').html(sbInfoContent.toString());
            }
            var a = data.identify;
            getCorrentSensorInfo(a, sensorId);
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
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

//获取关联传感器的位置
function getCorrentSensorInfo(a, sensorId) {

    var url = apiurl + '/correntSensor/' + sensorId + '/info?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        cache: false,
        success: function(data) {
            if (a == 0 || a == 1) {
                $('#correntSensor').hide();
            } else {

                $('#correntSensor').show();
                var correntlocation = '';

                for (var i = 0; i < data.length; i++) {
                    correntlocation += data[i].location + '；';
                }
                correntlocation = correntlocation.substring(0, correntlocation.length - 1); //去掉结尾的逗号

                $('#viewCorrent').html('');
                $('#viewCorrent').text(correntlocation);
            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
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

/************************* end 查看传感器************************************/



/************************************** 删除 传感器 **************************************/
var sensorIdDelete;
$('#sensorTable').on('click', 'a.editor_delete', function (e) {
    e.preventDefault();
    var tr = $(this).parents('tr');
    var selectedRow = tr[0];
    sensorIdDelete = selectedRow.id.substring(7);
    $('#pDeleteSensor').text("确定删除安装在“" + selectedRow.cells[5].innerText + "”的“" + selectedRow.cells[4].innerText + "”型号“" + selectedRow.cells[3].innerText + "”？");
});

//传感器确定删除
$('#btnDeleteSensor').click(function () {
    var url = apiurl + '/sensor/remove/' + sensorIdDelete + '?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        type: 'post',
        url: url,
        statusCode: {
            202: function () {
                $('#sensorTable').dataTable().fnDestroy();
                GetSensor();
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
                alert('抱歉，没有删除传感器权限');
            }
        }
    });
});
/************************************** end 删除 传感器 **************************************/


//传感器下一步
function SensornextOnclick() {
    $('#tabSensor').removeClass('active');
    $('#tab_sensor').removeClass('active');
    $('#tabSensorGroup').addClass('active');
    $('#tab_sensorGroup').addClass('active');    
}

//清除所选项
function RemoveSelectedItem(objSelect) {
    var length = objSelect.options.length - 1;
    for (var i = length; i >= 0; i--) {
        if (objSelect[i].selected == true) {
            objSelect.options[i].selected = false;
        }
    }
}