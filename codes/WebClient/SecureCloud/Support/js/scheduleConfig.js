var structId = location.href.split('=')[1].split('&')[0];

$(function () {

    getLine();

});


function getLine() {
    $('#tableSchedule').dataTable().fnDestroy();//消除图表

    if (structId != null) {//保证有structId时才发送请求
        var url = apiurl + '/struct/' + structId + '/constructInfo/list?token=' + getCookie('token');
        $.ajax({
            url: url,
            type: 'get',
            cache: false,
            success: function (data) {
                if (data == null||data.length==0)
                {
                    $('#tbodySchedule').html("");//表格内容清空
                    Schedule_Datatable();

                }
                else {
                    var sb = new StringBuffer();
                    for (var i = 0; i < data.length; i++) {
                        var LineId = data[i].LineId;
                        var LineName = data[i].LineName;
                        var LineLength = data[i].LineLength;
                        var StartId = data[i].StartId;
                        var EndId = data[i].EndId;
                        var Color = data[i].Color;
                        var Unit = data[i].Unit1;

                        if (Color == null) {
                            Color = "";
                        }

                        var lineNumber = i + 1;
                        sb.append("<tr id='Line_" + data[i].LineId + "'><td style='display:none;'>" + LineId + "</td>");//线路编号
                        sb.append("<td>" + LineName + "</td>");//线路名称
                        sb.append("<td>" + LineLength + '(' + Unit + ')' + "</td>");//线路长度
                        sb.append("<td>" + StartId + "</td>");//开始位置id
                        sb.append("<td>" + EndId + "</td>");//结束位置id                               
                        sb.append("<td><label style='background-color:" + Color + "; width:60px;padding:2px;'>" + Color + "</label></td>");//颜色
                        var str = "<td><a href='#modifyScheduleModal' class='editor_edit' data-toggle='modal' >修改</a> | ";
                        str += "<a href='#deleteLineModal' class='editor_delete' data-toggle='modal'>删除</a></td></tr>";
                        sb.append(str);//操作               
                    }
                    $('#tbodySchedule').html("");
                    $('#tbodySchedule').html(sb.toString());
                    Schedule_Datatable();
                }
            }
        });

    }

}

function Schedule_Datatable() {
    $('#tableSchedule').dataTable({
        "aLengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, "All"]
        ],
        "iDisplayLength": 50,
        "sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "aoColumnDefs": [{
            'bSortable': false, // 不排序
            'aTargets': [1,2, 6, 5] // 不排序的列
        }],
        "aaSorting": [[3,"asc"]] // 第1列升序排序
    });
}

/************************************** 增加施工线路 **************************************/
$('#btnAddLine').on('click', function () {
    //$('#addScheduleStruct').val(structName);
    $('#addScheduleLine').val('');
    $('#addLineLength').val('');
    $('#addScheduleInit_ID').val('');
    $('#addScheduleEnd_ID').val('');
    $('#addLineName').val('');
    $('#addScheduleColor').val('');
    document.getElementById('addLineRepet').style.display = 'none';
});


//线路 增加重置
$('#btnResetSchedule').on('click', function () {
    $('#addScheduleLine').val('');
    $('#addLineName ').val('');
    $('#addLineLength').val('');
    $('#addScheduleInit_ID').val('');
    $('#addScheduleEnd_ID').val('');
    $('#addScheduleColor').val('');
    $('#addLineUnit option[value="米"]').attr("Selected", true);

});

//线路增加
$('#btnSaveSchedule').click(function () {
    // var addLinestr = document.getElementById("addScheduleStruct").value;//结构物
    var addScheduleLine = document.getElementById("addScheduleLine").value;//线路编号
    var addLineName = document.getElementById("addLineName").value;
    var addLineLength = document.getElementById("addLineLength").value;//线路长度
    var addScheduleInit_ID = document.getElementById("addScheduleInit_ID").value;//开始id
    var addScheduleEnd_ID = document.getElementById("addScheduleEnd_ID").value;//结束id
    var addLineUnit = document.getElementById("addLineUnit").value;//长度单位
    var addProgressColor = document.getElementById("addScheduleColor").value;//颜色

    var flag = 0;
    if (addLineName == "") {
        $('#addLineName').focus();
        return flag = 5;
    }
    if (!/^\d{1,8}$/.test(addLineLength) || addLineLength == "") {
        if (!/^\d{1,8}\.\d{1,4}$/.test(addLineLength)) {
            alert("请输入正确的格式！");
            $('#addLineLength').focus();
            return flag = 1;
        }
    }

    if (!/^[0-9]*$/.test(addScheduleInit_ID) || addScheduleInit_ID == "") {
        alert("请输入非负数！");
        $('#addScheduleInit_ID').focus();
        return flag = 2;
    }

    if (!/^[0-9]*$/.test(addScheduleEnd_ID) || addScheduleEnd_ID == "") {
        alert("请输入非负数！");
        $('#addScheduleEnd_ID').focus();
        return flag = 3;
    } else if (parseInt(addScheduleEnd_ID) <= parseInt(addScheduleInit_ID)) {
        alert('重新输入！');
        $('#addScheduleEnd_ID').focus();
        return flag = 6;
    }
    if (!/^#([0-9a-fA-F]{3,6})$/.test(addProgressColor) && addProgressColor != "") {
        alert("配置的颜色不符合要求，请输入16进制的RGB表达式！如：#FFF000")
        $('#addScheduleColor').val('');
        return flag = 4;
    }

    if (flag != 0) {
        alert("有些信息没有配置成功！")
    }
    else {
        var data = {
            "LineId": addScheduleLine,
            "LineName": addLineName,
            "LineLength": addLineLength,
            "StartId": addScheduleInit_ID,
            "EndId": addScheduleEnd_ID,
            "structureId": structId,
            "Unit": addLineUnit,
            "Color": addProgressColor,
        };
        var url = apiurl + '/struct/' + structId + '/constructLine/add?token=' + getCookie('token');
        $.ajax({
            //async: false,//同步
            type: 'post',
            url: url,
            data: data,
            statusCode: {
                202: function () {
                    alert('保存成功');
                    $('#tableSchedule').dataTable().fnDestroy();
                    getLine();
                    $("#addScheduleModal").modal("hide");
                },
                403: function () {
                    alert("权限验证出错");
                    logOut();
                },
                400: function () {
                    alert("添加失败,线路编号已存在");
                    $('#addScheduleLine').val('');
                },
                500: function () {
                    alert("添加失败");
                    $('#addScheduleLine').val('');
                },
                404: function () {
                    alert('url错误');
                },
                405: function () {
                    alert('抱歉，没有增加施工线路权限');
                }
            }
        });
    }
});

/************************************** end 增加施工线路 **************************************/
/************************************** 修改 修改施工线路 **************************************/

var ModifyLineId;
$('#tableSchedule').on('click', 'a.editor_edit', function (e) {
    e.preventDefault();
    //$("#modifySchedulestr").val(structName);
    var tr = $(this).parents('tr');
    var selectedRow = tr[0];
    ModifyLineId = selectedRow.cells[0].innerText;

    var url = apiurl + '/struct/' + structId + '/constructInfo/list?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].LineId == ModifyLineId) {

                    $('#modifyScheduleLine ').val(data[i].LineName);
                    $('#modifyScheduleLength').val(data[i].LineLength);
                    $('#modifyScheduleInit_id').val(data[i].StartId);
                    $('#modifyScheduleEnd_id').val(data[i].EndId);
                    $('#modifyProgressUnit').val(data[i].Unit1);
                    $('#modifyProgressColor').val(data[i].Color);
                }
            }
        }
    })
});


//线路 修改重置
$('#btnResetModifySchedule').on('click', function () {
    $('#modifyScheduleLine ').val('');
    $('#modifyScheduleLength').val('');
    $('#modifyScheduleInit_id').val('');
    $('#modifyScheduleEnd_id').val('');
    $('#modifyProgressUnit option[value="米"]').attr("Selected", true);
    $('#modifyProgressColor').val('');
})

//线路保存修改

$('#btnSaveModifySchedule').click(function () {
    var modifyScheduleLength = document.getElementById("modifyScheduleLength").value;
    var modifyScheduleLine = document.getElementById("modifyScheduleLine").value;

    var modifyScheduleInit_id = document.getElementById("modifyScheduleInit_id").value;
    var modifyScheduleEnd_id = document.getElementById("modifyScheduleEnd_id").value;

    var modifyProgressUnit = document.getElementById("modifyProgressUnit").value;
    var modifyProgressColor = document.getElementById("modifyProgressColor").value;

    var flag = 0;
    if (modifyScheduleLine == "") {
        $('#modifyScheduleLine').focus();
        return flag = 1;
    }
    //验证数字
    if (!/^\d{1,8}$/.test(modifyScheduleLength) || modifyScheduleLength == "") {
        if (!/^\d{1,8}\.\d{1,4}$/.test(modifyScheduleLength)) {
            alert("请输入正确的格式！");
            $('#modifyScheduleLength').focus();
            return flag = 2
        }
    }

    if (!/^[0-9]*$/.test(modifyScheduleInit_id) || modifyScheduleInit_id == "") {
        alert("请输入非负数！");
        $('#modifyScheduleInit_id').focus();
        return flag = 3;
    }
    if (!/^[0-9]*$/.test(modifyScheduleEnd_id) || modifyScheduleEnd_id == "") {
        alert("请输入非负数！");
        $('#modifyScheduleEnd_id').focus();
        return flag = 4;
    } else if (parseInt(modifyScheduleEnd_id) <= parseInt(modifyScheduleInit_id)) {
        alert("请重新输入！");
        $('#modifyScheduleEnd_id').focus();
        return flag = 6;
    }
    if (!/^#([0-9a-fA-F]{6})$/.test(modifyProgressColor) && modifyProgressColor != "") {
        alert("配置的颜色不符合要求，请输入16进制的RGB表达式！如：#FFF000")
        $('#modifyProgressColor').val('');
        return flag = 5;
    }

    if (flag != 0) {
        alert("有些信息没有配置成功！")
    }
    else {
        var data = {
            "LineName": modifyScheduleLine,
            "LineLength": modifyScheduleLength,
            "StartId": modifyScheduleInit_id,
            "EndId": modifyScheduleEnd_id,
            "Unit": modifyProgressUnit,
            "Color": modifyProgressColor,
        };
        var url = apiurl + '/constructLine/modify/' + parseInt(ModifyLineId) + '?token=' + getCookie('token');
        $.ajax({
            type: 'post',
            url: url,
            data: data,
            statusCode: {
                202: function () {
                    $('#tableSchedule').dataTable().fnDestroy();
                    getLine();
                    alert("修改成功");
                    $("#modifyScheduleModal").modal("hide");
                },
                403: function () {
                    alert("权限验证出错");
                    logOut();
                },
                400: function () {
                    alert("修改失败");
                },
                500: function () {
                    alert("修改失败");
                },
                404: function () {
                    alert('url错误');
                },
                405: function () {
                    alert('抱歉，没有修改施工线路权限');
                }
            }
        });
    }
});

/************************************** end 修改施工线路 **************************************/

/************************************** 删除施工线路 **************************************/
var deleteLineId;
$('#tbodySchedule').on('click', 'a.editor_delete', function (e) {
    e.preventDefault();
    var tr = $(this).parents('tr');
    var selectedRow = tr[0];
    deleteLineId = selectedRow.cells[0].innerText;
    $('#alertMsg').text("确定删除线路 “" + selectedRow.cells[1].innerText + "”？");
});

//线路确定删除
$('#btnLineDelete').click(function () {
    var url = apiurl + '/constructLine/remove/' + parseInt(deleteLineId) + '?token=' + getCookie('token');
    $.ajax({
        //async: false,//同步
        type: 'post',
        url: url,
        statusCode: {
            202: function () {
                alert('删除成功');
                $('#tableSchedule').dataTable().fnDestroy();
                getLine();
                $("#deleteLineModal").modal("hide");
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
                alert('抱歉，没有删除施工线路权限');
            }
        }
    });
});
/************************************** end 删除施工线路 **************************************/
