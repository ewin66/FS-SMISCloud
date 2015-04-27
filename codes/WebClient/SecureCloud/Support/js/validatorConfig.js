var currentValidateConfigFactorId;
$(function () {
    validatePageLoad();

    $('#tabValidate').click(function() {
        validatePageLoad();
    });
});

/**
* init factor tab
*/
function validatePageLoad() {
    var url = apiurl + '/struct/' + structId + '/factor-config' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        async: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                $('#table-validate').dataTable().fnDestroy();
                $("#tbody-validate").html("");
                return;
            }
            var sb = new StringBuffer();
            var flag = true;
            var firstFactorId;
            for (var i = 0; i < data.length; i++) {
                for (var j = 0; j < data[i].children.length; j++) {
                    if (data[i].children[j].choose) {
                        sb.append('<li ');
                        if (flag) {
                            firstFactorId = data[i].children[j].factorId;
                            flag = false;
                            sb.append('class="active" ');
                        }
                        sb.append('onclick="factorTabSeleted(this)">');
                        sb.append('<a href="#">' + data[i].children[j].factorName);
                        if (data[i].children[j].description != null) {
                            sb.append('(' + data[i].children[j].description + ')');
                        }
                        sb.append('</a>');
                        sb.append('<input value="' + data[i].children[j].factorId + '" style="display:none;" />');
                        sb.append('</li>');
                    }
                }
            }
            $("#ul_validate").html(sb.toString());
            if (firstFactorId != null) {
                currentValidateConfigFactorId = firstFactorId;
                tableValidateShow(firstFactorId);
            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("权限验证出错");
                logOut();
            }
        }
    });
}

/**
* switch factor tab
*/
function factorTabSeleted(sender) {
    $(sender).siblings().removeClass("active");
    $(sender).addClass("active");
    var factorId = $(sender).find('input')[0].value;

    currentValidateConfigFactorId = factorId;
    tableValidateShow(factorId);
}

/**
* init validate table
*/
function tableValidateShow(factorId) {
    $('#tbody-validate').html('');

    var url = apiurl + '/struct/' + structId + '/factor/' + factorId + '/data-validate' + '?token=' + getCookie("token");

    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                $("#tbody-validate").html("");
            }
            if (data.length >= 1) {
                var sb = new StringBuffer();

                for (var i = 0; i < data.length; i++) {
                    sb.append('<tr id="validateItem-' + data[i].sensorId + '-' + data[i].itemId + '">');
                    if (i == 0 || data[i].sensorId != data[i - 1].sensorId) {
                        var count = 1;
                        for (var j = i + 1; j < data.length; j++) {
                            if (data[j].sensorId == data[i].sensorId) {
                                count++;
                            } else {
                                break;
                            }
                        }
                        sb.append('<td rowspan="' + count + '" style="vertical-align:middle" >' + data[i].location + '</td>');
                    }

                    sb.append('<td>' + data[i].itemName + '</td>');
                    if (data[i].rvEnabled) {
                        sb.append('<td><input type="button" class="btn btn-rv green" value="已启用" onclick="enabledBtnClicked(this, \'rv\')" /></td>');
                        sb.append('<td><input type="text" class="m-wrap span8 text-rv" placeholder="数字" value="' + (data[i].rvLower != null ? data[i].rvLower : "") + '" /></td>');
                        sb.append('<td><input type="text" class="m-wrap span8 text-rv" placeholder="数字" value="' + (data[i].rvUpper != null ? data[i].rvUpper : "") + '" /></td>');
                    } else {
                        sb.append('<td><input type="button" class="btn btn-rv" value="已禁用" onclick="enabledBtnClicked(this, \'rv\')" /></td>');
                        sb.append('<td><input type="text" class="m-wrap span8 text-rv" placeholder="数字" value="' + (data[i].rvLower != null ? data[i].rvLower : "") + '" disabled /></td>');
                        sb.append('<td><input type="text" class="m-wrap span8 text-rv" placeholder="数字" value="' + (data[i].rvUpper != null ? data[i].rvUpper : "") + '" disabled /></td>');
                    }

                    if (data[i].svEnabled) {
                        sb.append('<td><input type="button" class="btn btn-sv green" value="已启用" onclick="enabledBtnClicked(this, \'sv\')" /></td>');
                        sb.append('<td><input type="text" class="m-wrap span8 text-sv" placeholder="正整数" value="' + (data[i].svWindowSize != null ? data[i].svWindowSize : "") + '" /></td>');
                        sb.append('<td><input type="text" class="m-wrap span8 text-sv" placeholder="正数" value="' + (data[i].svKt != null ? data[i].svKt : "") + '" /></td>');
                        sb.append('<td><input type="text" class="m-wrap span8 text-sv" placeholder="正整数" value="' + (data[i].svDt != null ? data[i].svDt : "") + '" /></td>');
                        sb.append('<td><input type="text" class="m-wrap span8 text-sv" placeholder="正整数" value="' + (data[i].svRt != null ? data[i].svRt : "") + '" /></td>');
                    } else {
                        sb.append('<td><input type="button" class="btn btn-sv" value="已禁用" onclick="enabledBtnClicked(this, \'sv\')" /></td>');
                        sb.append('<td><input type="text" class="m-wrap span8 text-sv" placeholder="正整数" value="' + (data[i].svWindowSize != null ? data[i].svWindowSize : "") + '" disabled /></td>');
                        sb.append('<td><input type="text" class="m-wrap span8 text-sv" placeholder="正数" value="' + (data[i].svKt != null ? data[i].svKt : "") + '" disabled /></td>');
                        sb.append('<td><input type="text" class="m-wrap span8 text-sv" placeholder="正整数" value="' + (data[i].svDt != null ? data[i].svDt : "") + '" disabled /></td>');
                        sb.append('<td><input type="text" class="m-wrap span8 text-sv" placeholder="正整数" value="' + (data[i].svRt != null ? data[i].svRt : "") + '" disabled /></td>');
                    }

                    sb.append('<td><input type="button" class="btn blue" value="保存修改" onclick="saveValidateConfig(this)" /></td>');
                    sb.append('</tr>');
                }
                $('#tbody-validate').html(sb.toString());
            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("权限验证出错");
                logOut();
            }
        }
    });
}

/**
* switch enabled button
*/
function enabledBtnClicked(sender, name) {
    var btn = $(sender).parent().parent().find('.btn-' + name)[0];
    var txt = $(sender).parent().parent().find('.text-' + name);
    if (btn.value == '已启用') {
        btn.value = '已禁用';
        $(btn).removeClass('green');
        for (var i = 0; i < txt.length; i++) {
            txt[i].setAttribute('disabled', 'disabled');
        }
    } else {
        btn.value = '已启用';
        $(btn).addClass('green');
        for (var j = 0; j < txt.length; j++) {
            txt[j].removeAttribute('disabled');
        }
    }
}

/**
* save config
*/
function saveValidateConfig(sender) {
    var tr = $(sender).parent().parent();
    var id = tr.attr('id').split('-');
    var sensorId = id[1];
    var itemId = id[2];
    var rvEnabled = tr.find('.btn-rv')[0].value == '已启用' ? true : false;
    var rvValue = tr.find('.text-rv');
    var rvLower = rvValue[0].value;
    var rvUpper = rvValue[1].value;
    if (rvEnabled) {
        if (rvLower === '' || !textValidate(rvLower, '数字')) {
            rvValue[0].focus();
            return;
        }
        if (rvUpper === '' || !textValidate(rvUpper, '数字')) {
            rvValue[1].focus();
            return;
        }
    } else {
        rvLower = rvLower === '' ? null : rvLower;
        if (rvLower != null && !textValidate(rvLower, '数字')) {
            rvValue[0].focus();
            return;
        }
        rvUpper = rvUpper === '' ? null : rvUpper;
        if (rvUpper != null && !textValidate(rvUpper, '数字')) {
            rvValue[0].focus();
            return;
        }
    }
    var svEnabled = tr.find('.btn-sv')[0].value == '已启用' ? true : false;
    var svValue = tr.find('.text-sv');
    var svW = svValue[0].value;
    var svK = svValue[1].value;
    var svD = svValue[2].value;
    var svR = svValue[3].value;
    if (svEnabled) {
        if (svW === '' || !textValidate(svW, '正整数')) {
            svValue[0].focus();
            return;
        }
        if (svK === '' || !textValidate(svK, '正数')) {
            svValue[1].focus();
            return;
        }
        if (svD === '' || !textValidate(svD, '正整数')) {
            svValue[2].focus();
            return;
        }
        if (svR === '' || !textValidate(svR, '正整数')) {
            svValue[3].focus();
            return;
        }
    } else {
        svW = svW === '' ? null : svW;
        svK = svK === '' ? null : svK;
        svD = svD === '' ? null : svD;
        svR = svR === '' ? null : svR;
        if (svW != null && !textValidate(svW, '正整数')) {
            svValue[0].focus();
            return;
        }
        if (svK != null && !textValidate(svK, '正数')) {
            svValue[1].focus();
            return;
        }
        if (svD != null && !textValidate(svD, '正整数')) {
            svValue[2].focus();
            return;
        }
        if (svR != null && !textValidate(svR, '正整数')) {
            svValue[3].focus();
            return;
        }
    }

    var para = {
        "sensorId": sensorId,
        "itemId": itemId,
        "rvEnabled": rvEnabled,
        "rvLower": rvLower,
        "rvUpper": rvUpper,
        "svEnabled": svEnabled,
        "svWindowSize": svW,
        "svKt": svK,
        "svDt": svD,
        "svRt": svR
    };

    var url = apiurl + '/sensor/data-validate/config' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'post',
        data: para,
        async: false,
        statusCode: {
            202: function() {
                alert("保存成功!");
                tableThresholdShow(factorId);
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
                alert('抱歉，没有配置权限');
            }
        }
    });

    tableValidateShow(currentValidateConfigFactorId);
}

/**
* validate
*/
function textValidate(value, type) {
    var reg = '';

    if (type == '数字') {
        reg = /^-?\d+(\.\d+)?$/;
    } else if (type == '正整数') {
        reg = /^[1-9]\d*$/;
    }else if (type == '正数') {
        reg = /^\d+(\.\d+)?$/;
    } else {
        return true;
    }

    return reg.test(value);
}