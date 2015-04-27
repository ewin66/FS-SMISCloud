

$(function () {
    $('#systemConfig').addClass('active');
    $('#smsPush').addClass('active');
    $('.selectpicker').selectpicker();

    //var a = getCookie("nowStructId");
    //if (getCookie("nowStructId") == "" || getCookie("nowStructId") == undefined || getCookie("nowStructId") == null) {
    //    $('#monitorList').hide();
    //}
    //else {
    //    $('#monitorList').show();
    //}

    GetParam_AlertList();

    $('#btnSave').click(function () {
        SaveParamAlert();
    })

    $('#btnReset').click(function () {
        $('#addParm_alertForm')[0].reset();
    })
})



function GetParam_AlertList() {
    //$('#parm_alert_table').dataTable().fnDestroy();
    var url = apiurl + '/sms-user/list/' + getCookie("userId") + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                $('#parm_alert_tbody').html('');
                Parm_Alert_Table_To_datatable();
                return;
            }
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                sb.append('<tr><td>' + data[i].receiverName + '</td>');
                sb.append('<td>' + data[i].receiverPhone + '</td>');
                sb.append('<td>' + data[i].receiverMail + '</td>');
                if (data[i].receiveMode) {
                    sb.append('<td>短信</td>');
                } else {
                    sb.append('<td>邮箱</td>');
                }

                switch (parseInt(data[i].filterLevel)) {
                    case 1:
                        sb.append('<td>一级</td>');
                        break;
                    case 2:
                        sb.append('<td>二级</td>');
                        break;
                    case 3:
                        sb.append('<td>三级</td>');
                        break;
                    default:
                        sb.append('<td>四级</td>');
                }
                if (parseInt(data[i].roleId) == 1) {
                    sb.append('<td>用户</td>');
                }
                else {
                    sb.append('<td>技术支持</td>');
                }
                sb.append('<td><a class="label label-important" onclick="DeleteParamAlert('+data[i].receiverId+')">删除</a></td></tr>');

            }
            $('#parm_alert_tbody').html(sb.toString());
            Parm_Alert_Table_To_datatable();

        },
        error: function () {
            alertTips('获取用户列表失败', 'label-important', 'showMsg', 3000);
        }
    })
}


function SaveParamAlert() {
    var receiverName = $('#Parm_alertName').val();
    receiverName = receiverName.replace(/\s+/g, '');
    var receiverPhone = $('#Parm_alertPhone').val();
    var receiverMail = $('#Parm_alertMial').val();
    var roleId = $('#Parm_alertRole').val();
    var filterLevel = $('#Parm_alertLevel').val();
    var userId = getCookie("userId");
    var receiveMode = $('#Parm_alertMode').val();

    if (receiverName == "") {
        $('#Parm_alertName').focus();
    }
    else if (!/(^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$)|(^0{0,1}1[3|4|5|6|7|8|9][0-9]{9}$)/.test(receiverPhone) || receiverPhone == "") {
        $('#Parm_alertPhone').focus();
    }
    else if(!/^([a-zA-Z0-9]+[_|\_|\.|\-]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.|\-]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/.test(receiverMail)||receiverMail==""){
        $('#Parm_alertMial').focus();
    }
    else {
        var url = apiurl + '/sms-user/add' + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            cache: false,
            data: {
                receiverName: receiverName,
                receiverPhone: receiverPhone,
                receiverMail:receiverMail,
                roleId: roleId,
                filterLevel: filterLevel,
                userId: userId,
                receiveMode:receiveMode
            },
            statusCode: {
                202: function () {
                    $('#parm_alert_table').dataTable().fnDestroy();
                    GetParam_AlertList();
                    $('#btnCancel').click();
                    alertTips('保存成功', 'label-success','showMsg',3000);
                },
                403: function () {
                    alert("权限验证出错");
                    logOut();
                },
                400: function () {
                    alertTips('保存失败', 'label-important', 'showMsg', 3000);
                },
                500: function () {
                    alertTips('保存失败', 'label-important', 'showMsg', 3000);
                },
                404: function () {
                    alertTips('保存失败', 'label-important', 'showMsg', 3000);
                }
            }
        })
    }   
}

function DeleteParamAlert(receiverId) {
    var isrun = confirm('是否要删除该用户?');
    if (isrun) {
        var url = apiurl + '/sms-user/remove/' + receiverId + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            cache: false,
            statusCode: {
                202: function () {
                    $('#parm_alert_table').dataTable().fnDestroy();
                    GetParam_AlertList();
                    alertTips('删除成功', 'label-important', 'showMsg', 3000);
                },
                403: function () {
                    alert("权限验证出错");
                    logOut();
                },
                400: function () {
                    alertTips('删除失败', 'label-important', 'showMsg', 3000);                   
                },
                500: function () {
                    alertTips('删除失败', 'label-important', 'showMsg', 3000);
                },
                404: function () {
                    alertTips('删除失败', 'label-important', 'showMsg', 3000);
                }
            }
        })
    }  
}

function Parm_Alert_Table_To_datatable() {

    $('#parm_alert_table').dataTable({
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
        "aoColumnDefs": [{
            'bSortable': false,
            'aTargets': [1,2,3,4]
        }]
    });
}