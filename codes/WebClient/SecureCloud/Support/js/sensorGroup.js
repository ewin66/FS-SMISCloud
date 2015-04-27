$(function () {
    $('#tabSensorGroup').unbind();
    $('#tabSensorGroup').click(function () {
        initGroupTab();
    });
});

function initGroupTab() {
    var url = apiurl + '/struct/' + structId + '/sensor-group/type?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            var exists = false;
            for (var i = 0; i < data.length; i++) {
                var g = data[i];

                if (g.groupTypeId == 1) {
                    $('#tabCeXie').show();
                    $('#tabCeXie').parent().show();
                    initGroupCeXieTable();
                    initAddGroupCeXie();

                    $('#btnCeXieSave').unbind();
                    $('#btnCeXieSave').click(function () {
                        var tbodyCeXie = $('#TbodyCexie tr');
                        var array = [];
                        var depth = [];
                        for (var i = 0; i < tbodyCeXie.length; i++) {
                            var d = tbodyCeXie.find('input')[i].value;
                            if (d == null || d == '') {
                                alert('深度不能为空');
                                return;
                            }
                            if (contains(depth, d)) {
                                alert('重复的深度:' + d);
                                return;
                            }
                            depth.push(d);
                            array.push({ "sensorId": parseInt(tbodyCeXie.find('span')[i].textContent), "depth": d });
                        }
                        SaveCeXieTable($('#SensorGroupCeXieLocation').val(), array);
                    });

                    exists = true;
                }
                if (g.groupTypeId == 2) {
                    $('#tabChenJiang').show();
                    $('#tabChenJiang').parent().show();
                    $('.chenjiang-name').html(g.groupTypeName);
                    $('#btnAddSensorGroupChenJiang').val('增加' + g.groupTypeName);
                    var f = g.groupTypeName == '挠度分组' ? 31 : 42;
                    initGroupChenJiangTable(f);
                    initAddGroupChenJiang(f);
                    $('#btnChenJiangSave').unbind();
                    $('#btnChenJiangSave').click(function () {
                        var tbodyChenJiang = $('#TbodyChenJiang tr');
                        var array = [];
                        for (var i = 0; i < tbodyChenJiang.length; i++) {
                            array.push({ "sensorId": parseInt(tbodyChenJiang.find('span')[i].textContent), "isDatum": tbodyChenJiang.find('.is')[i].checked, "len": parseInt(tbodyChenJiang.find('.len')[i].value) });
                        }
                        SaveChenJiangTable($('#SensorGroupChenJiangLocation').val(), array, f);
                    });

                    exists = true;
                }
                if (g.groupTypeId == 3) {
                    $('#tabJinRunXian').show();
                    $('#tabJinRunXian').parent().show();
                    initGroupJinRunXianTable();
                    initAddGroupJinRunXian();
                    $('#btnJinRunXianSave').unbind();
                    $('#btnJinRunXianSave').click(function () {
                        var tbodyCeXie = $('#TbodyJinRunXian tr');
                        var array = [];
                        var depth = [];
                        for (var i = 0; i < tbodyCeXie.length; i++) {
                            var h = tbodyCeXie.find('input')[i].value;
                            if (h == null || h == '') {
                                alert('高度不能为空');
                                return;
                            }
                            if (contains(depth, h)) {
                                alert("重复的高度:" + h);
                                return;
                            }
                            depth.push(h);
                            array.push({ "sensorId": parseInt(tbodyCeXie.find('span')[i].textContent), "height": h });
                        }
                        SaveJinRunXianTable($('#SensorGroupJinRunXianLocation').val(), array);
                    });

                    exists = true;
                }
            }

            $('#tabCeXie').removeClass('active');
            $('#tab_CeXie').removeClass('active');
            $('#tabChenJiang').removeClass('active');
            $('#tab_ChenJiang').removeClass('active');
            $('#tabJinRunXian').removeClass('active');
            $('#tab_JinRunXian').removeClass('active');

            if ($('#tabCeXie').css('display') != 'none') {
                $('#tabCeXie').addClass('active');
                $('#tab_CeXie').addClass('active');
            } else if ($('#tabChenJiang').css('display') != 'none') {
                $('#tabChenJiang').addClass('active');
                $('#tab_ChenJiang').addClass('active');
            } else if ($('#tabJinRunXian').css('display') != 'none') {
                $('#tabJinRunXian').addClass('active');
                $('#tab_JinRunXian').addClass('active');
            }

            if (!exists) {
                $('#info').show();
                $('#groupTabContainer').hide();
            } else {
                $('#info').hide();
                $('#groupTabContainer').show();
            }
        },
        error: function () {

        }
    });
}

/*-------------------测斜 begin-----------------*/
/*  初始化测斜分组界面   */
function initGroupCeXieTable() {
    $('#SensorGroupCeXieTable').dataTable().fnDestroy();
    var url = apiurl + '/struct/' + structId + '/sensor-group/cexie?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                sb.append('<tr><td>测斜传感器</td>');
                sb.append('<td>' + data[i].groupName + '</td>');
                sb.append('<td><table>');
                for (var j = 0; j < data[i].sensorList.length; j++) {
                    sb.append('<tr><td><span style="display:none;">' + data[i].sensorList[j].sensorId + '</span>' + data[i].sensorList[j].sensorLocation + '</td><td>' + data[i].sensorList[j].depth + '</td></tr>');
                }
                sb.append('</table></td>');
                sb.append('<td><a href="#" onclick="DeleteGroupCeXie(' + data[i].groupId + ')" >删除</a>|<a href="#editSensorGroup" data-toggle="modal" onclick="EditGroupCeXie(this,' + data[i].groupId + ')">编辑</a></td></tr>');
            }
            $('#SensorGroupCeXieTbody').html(sb.toString());
            Table_To_datatable('#SensorGroupCeXieTable');
        },
        error: function () {

        }
    });
}
/*  新增测斜分组  */
function SaveCeXieTable(groupLocation, sensorList) {
    var url = apiurl + '/sensor-group/cexie/add' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'post',
        data: {
            "groupName": groupLocation,
            "sensorList": sensorList
        },
        statusCode: {
            202: function () {
                alert("新增成功!");
                initGroupCeXieTable();
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
                alert('抱歉，没有增加传感器分组权限');
            }
        }
    });
    initAddGroupCeXie();
}
/*  修改测斜分组  */
function EditCeXieTable(groupId, groupLocation, sensorList) {
    var url = apiurl + '/sensor-group/cexie/modify/' + groupId + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'post',
        data: {
            "groupName": groupLocation,
            "sensorList": sensorList
        },
        statusCode: {
            202: function () {
                alert("修改成功!");
                initGroupCeXieTable();
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
                alert('抱歉，没有编辑传感器分组权限');
            }
        }
    });
}
/*  删除测斜分组    */
function DeleteGroupCeXie(groupId) {
    var isDel = confirm('是否删除?');
    if (isDel) {
        var url = apiurl + '/sensor-group/remove/' + groupId + '?token=' + getCookie('token');
        $.ajax({
            url: url,
            type: 'post',
            statusCode: {
                202: function () {
                    alert("删除成功!");
                    initGroupCeXieTable();
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
                    alert('抱歉，没有删除传感器分组权限');
                }
            }
        });
    }
}
/*  初始化新增测斜分组界面   */
function initAddGroupCeXie() {
    var url = apiurl + '/struct/' + structId + '/factor/10/sensors' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.length != 0 && data != []) {
                var sb2 = new StringBuffer();
                for (var j = 0; j < data.length; j++) {
                    if (data[j].sensors != [] && data[j].sensors != null) {
                        for (var i = 0; i < data[j].sensors.length; i++) {
                            sb2.append('<option value="' + data[j].sensors[i].sensorId + '">' + data[j].sensors[i].location + '</option>');
                        }
                    }
                }

                $('#sensorGroupListCeXie').html(sb2.toString());
                $("#sensorGroupListCeXie").chosen({
                    no_results_text: "没有找到",
                    allow_single_de: true
                });

                $('#SensorGroupCeXieLocation').val('');
                $('#TbodyCexie').html('');

                $('#sensorGroupListCeXie').change(function () {
                    var sensorList = $('#sensorGroupListCeXie option:selected');
                    var str = '';
                    for (var i = 0; i < sensorList.length; i++) {
                        str += '<tr><td><span style="display:none;">' + sensorList[i].value + '</span>' + sensorList[i].text + '</td><td><input type="text" /></td></tr>';
                    }
                    $('#TbodyCexie').html(str);
                });

                $('#sensorGroupListCeXie').trigger("liszt:updated");
            }
        }
    });
}
/*  初始化修改测斜分组界面    */
function EditGroupCeXie(dom, groupId) {
    var sensorLocation = $(dom).parent().siblings().eq(1).text();
    var sensorListDom = $(dom).parent().siblings().eq(2).find('span');
    var sensorListArray = [];
    for (var i = 0; i < sensorListDom.length; i++) {
        sensorListArray.push(sensorListDom[i].innerText);
    }
    var sensorListInputDom = $(dom).parent().siblings().eq(2).find('tr');
    var sensorListInput = [];
    for (var i = 0; i < sensorListInputDom.length; i++) {
        sensorListInput.push(sensorListInputDom[i].cells[1].innerHTML);
    }
    $('#editSensorGroupCeXieLocation').val(sensorLocation);
    var url = apiurl + '/struct/' + structId + '/factor/10/sensors' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.length != 0 && data != []) {
                var sb2 = new StringBuffer();
                for (var j = 0; j < data.length; j++) {
                    if (data[j].sensors != [] && data[j].sensors != null) {
                        for (var i = 0; i < data[j].sensors.length; i++) {
                            sb2.append('<option value="' + data[j].sensors[i].sensorId + '">' + data[j].sensors[i].location + '</option>');
                        }
                    }
                }

                $('#editsensorGroupListCeXie').html(sb2.toString());
                for (var i = 0; i < sensorListArray.length; i++) {
                    $('#editsensorGroupListCeXie [value="' + sensorListArray[i] + '"]').attr('selected', 'selected');
                }
                $('#editsensorGroupListCeXie').trigger("liszt:updated");
                $("#editsensorGroupListCeXie").chosen({
                    no_results_text: "没有找到",
                    allow_single_de: true
                });
                var sensorList = $('#editsensorGroupListCeXie option:selected');
                var str = '';
                for (var i = 0; i < sensorList.length; i++) {
                    str += '<tr><td><span style="display:none;">' + sensorList[i].value + '</span>' + sensorList[i].text + '</td><td><input type="text" value="' + sensorListInput[i] + '" /></td></tr>';
                }
                $('#editTbodyCexie').html(str);

                $('#editsensorGroupListCeXie').change(function () {
                    var sensorList = $('#editsensorGroupListCeXie option:selected');
                    var str = '';
                    for (var i = 0; i < sensorList.length; i++) {
                        str += '<tr><td><span style="display:none;">' + sensorList[i].value + '</span>' + sensorList[i].text + '</td><td><input type="text" /></td></tr>';
                    }
                    $('#editTbodyCexie').html(str);

                });

                $('#btnCeXieEdit').unbind();
                $('#btnCeXieEdit').click(function () {
                    var tbodyCeXie = $('#editTbodyCexie tr');
                    var array = [];
                    var depth = [];
                    for (var i = 0; i < tbodyCeXie.length; i++) {                        
                        var d = tbodyCeXie.find('input')[i].value;
                        if (d == null || d == '') {
                            alert('深度不能为空');
                            return;
                        }
                        if (contains(depth, d)) {
                            alert('重复的深度:' + d);
                            return;
                        }
                        depth.push(d);
                        array.push({ "sensorId": parseInt(tbodyCeXie.find('span')[i].textContent), "depth": d });
                    }
                    EditCeXieTable(groupId, $('#editSensorGroupCeXieLocation').val(), array);
                });
            }
        }
    });
}
/*-------------------测斜 end  -----------------*/

/*-------------------沉降 begin-----------------*/
/*  初始化沉降分组界面   */
function initGroupChenJiangTable(factor) {
    $('#SensorGroupChenJiangTable').dataTable().fnDestroy();
    var url = apiurl + '/struct/' + structId + '/sensor-group/chenjiang?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                sb.append('<tr><td>沉降传感器</td>');
                sb.append('<td>' + data[i].groupName + '</td>');
                sb.append('<td><table>');
                for (var j = 0; j < data[i].sensorList.length; j++) {
                    sb.append('<tr><td><span style="display:none;">' + data[i].sensorList[j].sensorId + '</span>' + data[i].sensorList[j].sensorLocation + '</td>' +
                        '<td>' + data[i].sensorList[j].len + '</td>' +
                        '<td>' + (data[i].sensorList[j].isDatum ? '是' : '') + '</td></tr>');
                }
                sb.append('</table></td>');
                sb.append('<td><a href="#" onclick="DeleteGroupChenJiang(' + data[i].groupId + ',' + factor + ')" >删除</a>|<a href="#editSensorGroupChenJiang" data-toggle="modal" onclick="EditGroupChenJiang(this,' + data[i].groupId + ', ' + factor +')">编辑</a></td></tr>');
            }
            $('#SensorGroupChenJiangTbody').html(sb.toString());
            Table_To_datatable('#SensorGroupChenJiangTable');
        },
        error: function () {

        }
    });
}
/*  新增沉降分组  */
function SaveChenJiangTable(groupLocation, sensorList, factor) {
    var url = apiurl + '/sensor-group/settle/add' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'post',
        data: {
            "groupName": groupLocation,
            "sensorList": sensorList
        },
        statusCode: {
            202: function () {
                alert("新增成功!");
                initGroupChenJiangTable(factor);
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
            }
        }
    });
    initAddGroupChenJiang(factor);
}
/*  修改沉降分组  */
function EditChenJiangTable(groupId, groupLocation, sensorList, factor) {
    var url = apiurl + '/sensor-group/settle/modify/' + groupId + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'post',
        data: {
            "groupName": groupLocation,
            "sensorList": sensorList
        },
        statusCode: {
            202: function () {
                alert("修改成功!");
                initGroupChenJiangTable(factor);
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
            }
        }
    });
}
/*  删除沉降分组    */
function DeleteGroupChenJiang(groupId, factor) {
    var isDel = confirm('是否删除?');
    if (isDel) {
        var url = apiurl + '/sensor-group/remove/' + groupId + '?token=' + getCookie('token');
        $.ajax({
            url: url,
            type: 'post',
            statusCode: {
                202: function () {
                    alert("删除成功!");
                    initGroupChenJiangTable(factor);
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
                }
            }
        });
    }
}
/*  初始化新增沉降分组界面   */
function initAddGroupChenJiang(factor) {
    var url = apiurl + '/struct/' + structId + '/factor/' + factor + '/sensors' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.length != 0 && data != []) {
                var sb2 = new StringBuffer();
                for (var j = 0; j < data.length; j++) {
                    if (data[j].sensors != [] && data[j].sensors != null) {
                        for (var i = 0; i < data[j].sensors.length; i++) {
                            sb2.append('<option value="' + data[j].sensors[i].sensorId + '">' + data[j].sensors[i].location + '</option>');
                        }
                    }
                }

                $('#sensorGroupListChenJiang').html(sb2.toString());
                $("#sensorGroupListChenJiang").chosen({
                    no_results_text: "没有找到",
                    allow_single_de: true
                });

                $('#SensorGroupChenJiangLocation').val('');
                $('#TbodyChenJiang').html('');

                $('#sensorGroupListChenJiang').change(function () {
                    var sensorList = $('#sensorGroupListChenJiang option:selected');
                    var str = '';
                    for (var i = 0; i < sensorList.length; i++) {
                        str += '<tr><td><span style="display:none;">' + sensorList[i].value + '</span>' + sensorList[i].text + '</td>' +
                            '<td><input class="len" type="text" /></td>' +
                            '<td><input class="is" type="checkbox" /></td></tr>';
                    }
                    $('#TbodyChenJiang').html(str);
                });
                $('#sensorGroupListChenJiang').trigger("liszt:updated");
            }
        }
    });
}
/*  初始化修改沉降分组界面    */
function EditGroupChenJiang(dom, groupId, factor) {
    var sensorLocation = $(dom).parent().siblings().eq(1).text();
    var sensorListDom = $(dom).parent().siblings().eq(2).find('span');
    var sensorListArray = [];
    for (var i = 0; i < sensorListDom.length; i++) {
        sensorListArray.push(sensorListDom[i].innerText);
    }
    var sensorListInputDom = $(dom).parent().siblings().eq(2).find('tr');
    var sensorListInput = [];
    for (var i = 0; i < sensorListInputDom.length; i++) {
        sensorListInput.push({
            "len": sensorListInputDom[i].cells[1].innerHTML,
            "isDatum": sensorListInputDom[i].cells[2].innerHTML == '是' ? true : false
        });
    }
    $('#editSensorGroupChenJiangLocation').val(sensorLocation);
    var url = apiurl + '/struct/' + structId + '/factor/' + factor + '/sensors' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.length != 0 && data != []) {
                var sb2 = new StringBuffer();
                for (var j = 0; j < data.length; j++) {
                    if (data[j].sensors != [] && data[j].sensors != null) {
                        for (var i = 0; i < data[j].sensors.length; i++) {
                            sb2.append('<option value="' + data[j].sensors[i].sensorId + '">' + data[j].sensors[i].location + '</option>');
                        }
                    }
                }

                $('#editsensorGroupListChenJiang').html(sb2.toString());
                for (var i = 0; i < sensorListArray.length; i++) {
                    $('#editsensorGroupListChenJiang [value="' + sensorListArray[i] + '"]').attr('selected', 'selected');
                }
                $('#editsensorGroupListChenJiang').trigger("liszt:updated");
                $("#editsensorGroupListChenJiang").chosen({
                    no_results_text: "没有找到",
                    allow_single_de: true
                });
                var sensorList = $('#editsensorGroupListChenJiang option:selected');
                var str = '';
                for (var i = 0; i < sensorList.length; i++) {
                    str += '<tr><td><span style="display:none;">' + sensorList[i].value + '</span>' + sensorList[i].text + '</td>' +
                        '<td><input class="len" type="text" value="' + sensorListInput[i].len + '" /></td>' +
                        '<td><input class="is" type="checkbox" ' + (sensorListInput[i].isDatum ? 'checked="checked"' : '') + '" /></td></tr>';
                }
                $('#editTbodyChenJiang').html(str);

                $('#editsensorGroupListChenJiang').change(function () {
                    var sensorList = $('#editsensorGroupListChenJiang option:selected');
                    var str = '';
                    for (var i = 0; i < sensorList.length; i++) {
                        str += '<tr><td><span style="display:none;">' + sensorList[i].value + '</span>' + sensorList[i].text + '</td>' +
                            '<td><input class="len" type="text" /></td>' +
                            '<td><input class="is" type="checkbox" /></td></tr>';
                    }
                    $('#editTbodyChenJiang').html(str);

                });

                $('#btnChenJiangEdit').unbind();
                $('#btnChenJiangEdit').click(function () {
                    var tbodyChenJiang = $('#editTbodyChenJiang tr');
                    var array = [];
                    for (var i = 0; i < tbodyChenJiang.length; i++) {
                        array.push({ "sensorId": parseInt(tbodyChenJiang.find('span')[i].textContent), "isDatum": tbodyChenJiang.find('.is')[i].checked, "len": tbodyChenJiang.find('.len')[i].value });
                    }
                    EditChenJiangTable(groupId, $('#editSensorGroupChenJiangLocation').val(), array, factor);
                });
            }
        }
    });
}
/*-------------------沉降 end  -----------------*/

/*------------------浸润线 begin-----------------*/
/*  初始化浸润线分组界面   */
function initGroupJinRunXianTable() {
    $('#SensorGroupJinRunXianTable').dataTable().fnDestroy();
    var url = apiurl + '/struct/' + structId + '/sensor-group/jinrunxian?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                sb.append('<tr><td>浸润线传感器</td>');
                sb.append('<td>' + data[i].groupName + '</td>');
                sb.append('<td><table>');
                for (var j = 0; j < data[i].sensorList.length; j++) {
                    sb.append('<tr><td><span style="display:none;">' + data[i].sensorList[j].sensorId + '</span>' + data[i].sensorList[j].sensorLocation + '</td><td>' + data[i].sensorList[j].height + '</td></tr>');
                }
                sb.append('</table></td>');
                sb.append('<td><a href="#" onclick="DeleteGroupJinRunXian(' + data[i].groupId + ')" >删除</a>|<a href="#editSensorGroupJinRunXian" data-toggle="modal" onclick="EditGroupJinRunXian(this,' + data[i].groupId + ')">编辑</a></td></tr>');
            }
            $('#SensorGroupJinRunXianTbody').html(sb.toString());
            Table_To_datatable('#SensorGroupJinRunXianTable');
        },
        error: function () {

        }
    });
}
/*  新增浸润线分组  */
function SaveJinRunXianTable(groupLocation, sensorList) {
    var url = apiurl + '/sensor-group/saturation-line/add' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'post',
        data: {
            "groupName": groupLocation,
            "sensorList": sensorList
        },
        statusCode: {
            202: function () {
                alert("新增成功!");
                initGroupJinRunXianTable();
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
            }
        }
    });
    initAddGroupJinRunXian();
}
/*  修改浸润线分组  */
function EditJinRunXianTable(groupId, groupLocation, sensorList) {
    var url = apiurl + '/sensor-group/saturation-line/modify/' + groupId + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'post',
        data: {
            "groupName": groupLocation,
            "sensorList": sensorList
        },
        statusCode: {
            202: function () {
                alert("修改成功!");
                initGroupJinRunXianTable();
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
            }
        }
    });
}
/*  删除浸润线分组    */
function DeleteGroupJinRunXian(groupId) {
    var isDel = confirm('是否删除?');
    if (isDel) {
        var url = apiurl + '/sensor-group/remove/' + groupId + '?token=' + getCookie('token');
        $.ajax({
            url: url,
            type: 'post',
            statusCode: {
                202: function () {
                    alert("删除成功!");
                    initGroupJinRunXianTable();
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
                }
            }
        });
    }
}
/*  初始化新增浸润线分组界面   */
function initAddGroupJinRunXian() {
    var url = apiurl + '/struct/' + structId + '/factor/34/sensors' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.length != 0 && data != []) {
                var sb2 = new StringBuffer();
                for (var j = 0; j < data.length; j++) {
                    if (data[j].sensors != [] && data[j].sensors != null) {
                        for (var i = 0; i < data[j].sensors.length; i++) {
                            sb2.append('<option value="' + data[j].sensors[i].sensorId + '">' + data[j].sensors[i].location + '</option>');
                        }
                    }
                }

                $('#sensorGroupListJinRunXian').html(sb2.toString());
                $("#sensorGroupListJinRunXian").chosen({
                    no_results_text: "没有找到",
                    allow_single_de: true
                });

                $('#SensorGroupJinRunXianLocation').val('');
                $('#TbodyJinRunXian').html('');

                $('#sensorGroupListJinRunXian').change(function () {
                    var sensorList = $('#sensorGroupListJinRunXian option:selected');
                    var str = '';
                    for (var i = 0; i < sensorList.length; i++) {
                        str += '<tr><td><span style="display:none;">' + sensorList[i].value + '</span>' + sensorList[i].text + '</td><td><input type="text" /></td></tr>';
                    }
                    $('#TbodyJinRunXian').html(str);
                });
                $('#sensorGroupListJinRunXian').trigger("liszt:updated");
            }
        }
    });
}
/*  初始化修改浸润线分组界面    */
function EditGroupJinRunXian(dom, groupId) {
    var sensorLocation = $(dom).parent().siblings().eq(1).text();
    var sensorListDom = $(dom).parent().siblings().eq(2).find('span');
    var sensorListArray = [];
    for (var i = 0; i < sensorListDom.length; i++) {
        sensorListArray.push(sensorListDom[i].innerText);
    }
    var sensorListInputDom = $(dom).parent().siblings().eq(2).find('tr');
    var sensorListInput = [];
    for (var i = 0; i < sensorListInputDom.length; i++) {
        sensorListInput.push(sensorListInputDom[i].cells[1].innerHTML);
    }
    $('#editSensorGroupJinRunXianLocation').val(sensorLocation);
    var url = apiurl + '/struct/' + structId + '/factor/34/sensors' + '?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.length != 0 && data != []) {
                var sb2 = new StringBuffer();
                for (var j = 0; j < data.length; j++) {
                    if (data[j].sensors != [] && data[j].sensors != null) {
                        for (var i = 0; i < data[j].sensors.length; i++) {
                            sb2.append('<option value="' + data[j].sensors[i].sensorId + '">' + data[j].sensors[i].location + '</option>');
                        }
                    }
                }

                $('#editsensorGroupListJinRunXian').html(sb2.toString());
                for (var i = 0; i < sensorListArray.length; i++) {
                    $('#editsensorGroupListJinRunXian [value="' + sensorListArray[i] + '"]').attr('selected', 'selected');
                }
                $('#editsensorGroupListJinRunXian').trigger("liszt:updated");
                $("#editsensorGroupListJinRunXian").chosen({
                    no_results_text: "没有找到",
                    allow_single_de: true
                });
                var sensorList = $('#editsensorGroupListJinRunXian option:selected');
                var str = '';
                for (var i = 0; i < sensorList.length; i++) {
                    str += '<tr><td><span style="display:none;">' + sensorList[i].value + '</span>' + sensorList[i].text + '</td><td><input type="text" value="' + sensorListInput[i] + '" /></td></tr>';
                }
                $('#editTbodyJinRunXian').html(str);

                $('#editsensorGroupListJinRunXian').change(function () {
                    var sensorList = $('#editsensorGroupListJinRunXian option:selected');
                    var str = '';
                    for (var i = 0; i < sensorList.length; i++) {
                        str += '<tr><td><span style="display:none;">' + sensorList[i].value + '</span>' + sensorList[i].text + '</td><td><input type="text" /></td></tr>';
                    }
                    $('#editTbodyJinRunXian').html(str);

                });

                $('#btnJinRunXianEdit').unbind();
                $('#btnJinRunXianEdit').click(function () {
                    var tbodyJinRunXian = $('#editTbodyJinRunXian tr');
                    var array = [];
                    var height = [];
                    for (var i = 0; i < tbodyJinRunXian.length; i++) {
                        var h = tbodyJinRunXian.find('input')[i].value;
                        if (h == null || h == '') {
                            alert('高度不能为空');
                            return;
                        }
                        if (contains(height, h)) {
                            alert('重复的高度:' + h);
                            return;
                        }
                        height.push(h);
                        array.push({ "sensorId": parseInt(tbodyJinRunXian.find('span')[i].textContent), "height": h });
                    }
                    EditJinRunXianTable(groupId, $('#editSensorGroupJinRunXianLocation').val(), array);
                });
            }
        }
    });
}
/*------------------浸润线 end  -----------------*/

/*  下一步    */
function groupConfigNext() {
    $('#tabSensorGroup').removeClass('active');
    $('#tab_sensorGroup').removeClass('active');
   
    $('#tabAggConfig').addClass('active');
    $('#tab_aggconfig').addClass('active');
   // ThresholdPageLoad();
}

function Table_To_datatable(id) {
    //$(id).dataTable().fnDestroy();
    $(id).dataTable({
        "aLengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, "All"]
        ],
        // set the initial value
        "iDisplayLength": 10,
        //"sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
        //"sPaginationType": "bootstrap",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "bDestroy": true,
        "bRetrieve": true
        //"sPaginationType": "full_numbers",
        //"bFilter": false,//禁用搜索框
        //"bStateSave": true,        
    });
}

function contains(array, value) {
    for (var i = 0; i < array.length; i++) {
        if (array[i] == value) {
            return true;
        }
    }
    return false;
}