$(function () {
    IntConfigUnits(structId, structName);
});
$('#tabFactorUnit').click(function () {
    IntConfigUnits(structId, structName);
});

var allFactorLength = '';

function IntConfigUnits(structId, structName) {
    $('#FactorUnitTable').dataTable().fnDestroy();
    var url = apiurl + '/factor/' + structId + '/unitList/info?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        cache: false,
        success: function(data) {
            if (data != null) {
                var sb = new StringBuffer();
                for (var i = 0; i < data.length; i++) {
                    var selectedUnit = GetConfigedUnit(structId, data[i].factorId, data[i].itemId);
                    allFactorLength = data.length;
                    //调用函数展示已配置的选项
                    var option = '';
                    sb.append("<tr id='factor-" + data[i].factorId + '-' + data[i].itemId + "'><td >" + data[i].factorName + "</td>");
                    sb.append("<td>" + data[i].itemName + '</td>');
                    var a;
                    var unitList = data[i].unit;
                    for (var j = 0; j < unitList.length; j++) {
                        var unit = unitList[j].Unit;
                        var id = unitList[j].id;
                        if (selectedUnit == id) {
                            option += '<option value="' + unit + '" selected="selected">' + unit + '</option>';
                            a = 1;
                        } else {
                            option += '<option value="' + unit + '">' + unit + '</option>';

                        }
                        continue;
                    }
                    sb.append("<td><select id='unitList'>");
                    sb.append(option);
                    sb.append("</select></td>");
                    sb.append('</tr>');
                }
                $('#tbodyFactorUnit').html("");
                $('#tbodyFactorUnit').append(sb.toString());
                FactorUnit_datatable();
                rowsUnitsJudge();
                allFactorLength = data.length;
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

function GetConfigedUnit(structId, factorId, valueIndex) {
    var unitConfig = '';
    var url = apiurl + '/factor/' + structId + '/' + factorId + '/' + valueIndex + '/unit/info?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        cache: false,
        success: function(data) {
            if (data.length == 0 || data == []) {
                unitConfig = '没有';
            } else {
                unitConfig = data[0].id;
            }
        },
    });
    return unitConfig;
}

function saveConfig() {
    var ds = [];
    var table = document.getElementById("FactorUnitTable");
    var rows = table.rows.length; //包括列头一行
    for (var i = 1; i < rows; i++) {
        var tr = table.getElementsByTagName("tr")[i]; //获取对应的行 
        var subSelect = tr.getElementsByTagName("td")[2];
        var unit = subSelect.getElementsByTagName("select")[0].value;
        var id = $(tr).attr('id').split('-');
        var factorId = id[1];
        var itemId = id[2];
        var para = {
            "FactorId": factorId,
            "ItemId": itemId,
            "Unit": unit,
            "StructId": structId
        };
        ds.push(para);
    }

   var obj = {
        date: ds
    };

    var url = apiurl + '/factor/' + structId + '/unit/add?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'post',
            data: obj,
            async: false,
            statusCode: {
                202: function() {
                    alert("保存成功!");

                },
                403: function() {
                    alert("权限验证出错");
                    logOut();
                },
                400: function() {
                    alert("保存失败,参数错误");
                },
                500: function() {
                    alert("保存失败");
                },
                404: function() {
                    alert('保存失败');
                },
                405: function () {
                    alert('抱歉，没有修改因素单位的权限');
                    return;
                }
            }
        });
    
    rowsUnitsJudge();
}


function FactorUnitnextOnclick() {
    $('#tabFactorUnit').removeClass('active');
    $('#tab_FactorUnit').removeClass('active');
    $('#tabDtu').addClass('active');
    $('#tab_DTU').addClass('active');
    GetDtu(structId, structName);
}


function rowsUnitsJudge() {
    var url = apiurl + '/factor/' + structId + '/unit/count?token=' + getCookie("token");
    $.ajax({
        //async: false,//同步
        url: url,
        type: 'get',
        cache: false,
        success: function(data) {
            if (data == null || data.length == 0 || data.length < allFactorLength) {
                $('#tabDtu').hide();
                document.getElementById("FactorUnit").disabled = true; //不可用
                $('#FactorUnit').removeClass('blue');
            } else if (data.length == allFactorLength) {
                JudgeFactorName();
            }
        },
    });
}

function JudgeFactorName() {

    //根据内容来判断是否进入Dtu配置
    var table = document.getElementById("FactorUnitTable");
    var rows = table.rows.length; //包括列头一行
    var count = [];
    var factorList = [];
    for (var i = 1; i < rows; i++) {
        var child = table.getElementsByTagName("tr")[i]; //获取对应的行 
        var factorId = child.getElementsByTagName("td")[0].innerHTML;
        var subSelect = child.getElementsByTagName("td")[2];
        var unit = subSelect.getElementsByTagName("select")[0].value;
        factorList.push(factorId);
        count.push(unit);
    }
    var name1 = "内部位移监测";
    var name2 = "表面位移监测";
    var name3 = "桥墩倾斜监测";
    var name4 = "建筑物倾斜";
    for (var k = 0; k < factorList.length; k++) {
        if ((factorList[k] == factorList[k + 1]) && (factorList[k + 1] == name1)) {
            if (count[k] != count[k + 1]) {
                SetNext();

                break;
            }
        } else if ((factorList[k] == factorList[k + 1]) && (factorList[k + 1] == name2)) {
            if (count[k] != count[k + 1]) {
                SetNext();
                break;
            }
        } else if ((factorList[k] == factorList[k + 1]) && (factorList[k + 1] == name3)) {
            if (count[k] != count[k + 1]) {
                SetNext();
                break;
            }
        } else if ((factorList[k] == factorList[k + 1]) && (factorList[k + 1] == name4)) {
            if (count[k] != count[k + 1]) {
                SetNext();
                break;
            }
        } else {
            $('#tabDtu').show();
            document.getElementById("FactorUnit").disabled = false; //不可用
            $('#FactorUnit').addClass('blue');
            //需要判断
        }
    }
}

function SetNext() {
    $('#tabDtu').hide();
    document.getElementById("FactorUnit").disabled = true; //不可用
    $('#FactorUnit').removeClass('blue');
}

function FactorUnit_datatable() {

    $('#FactorUnitTable').dataTable({
        "aLengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, "All"]
        ],
        // set the initial value
        "iDisplayLength": 50,
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