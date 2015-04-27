$(function () {
    GetAccountInfo();
    var username = getCookie('loginname');
    $('#lblUser').html(username);
});

var oldpassword = '';
function GetAccountInfo() {
    var url = apiurl + '/user/' + getCookie("userId") + '/info?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                return;
            }
            $('#orgnization').empty();
            $('#struct').empty();
            $('#userName').val(data.userName);
            
            oldpassword = data.password;
            $('#role').val(data.roleCode);
            data.email == null ? $('#mail').val('无') : $('#mail').val(data.email);
            data.phone == null ? $('#tel').val('无') : $('#tel').val(data.phone);
            if (data.orgs != null) {
                for (var j = 0; j < data.orgs.length; j++) {
                    var option = $("<option>").text(data.orgs[j].name).val(data.userName)
                    $('#orgnization').append(option);

                }
            }
            if (data.structs != null) {
                for (var k = 0; k < data.structs.length; k++) {
                    var option = $("<option>").text(data.structs[k].name).val(data.userName)
                    $('#struct').append(option);
                }
            }
          
        },
        error: function () {
            alert("获取用户信息出错！");
        }
    });
}

$('#btnReset_edit').click(function () {

    $('#Old_password').val('');
    $('#New_password').val('');
    $('#Confirm_New_password').val('');
});

$('#btnSave_edit').click(function () {
    var oldPwd = $('#Old_password').val();
    var newPwd = $('#New_password').val();
    var confirm_password = $('#Confirm_New_password').val();
    if (oldPwd != oldpassword) {
        alert("旧密码输入不正确");
        $('#Old_password').val('');
        $('#Old_password').focus();
    } else {
        if (!/^[a-zA-Z0-9]{1,15}$/.test(confirm_password) || confirm_password == "") {
            $('#Confirm_New_password').focus();
        }
        else if (newPwd != confirm_password) {
            alert("两次密码输入不一致");
            $('#Confirm_New_password').focus();
        }
        else {
            var url = apiurl + '/user/modify-info' + '?token=' + getCookie("token");
            $.ajax({
                url: url,
                type: 'post',
                dataType: 'json',
                data: {
                    userId: getCookie("userId"),
                    oldPwd: oldPwd,
                    newPwd: newPwd,
                },
                statusCode: {
                    202: function () {
                        alert("密码修改成功,请重新登陆");
                        window.location.href = '/login.html';
                    },
                    400: function () {
                        alert("密码修改成功");
                    },
                    500: function () {
                        alert("密码修改失败");
                    }
                },
                success: function () {
                    $('#Old_password').val('');
                    $('#New_password').val('');
                    $('#Confirm_New_password').val('');
                }
            });
        }
    }
});

/**
 * 创建所有结构物告警徽章及告警信息
 */
function createWarningBadgeAndContentByUser(userId) {
    var url = apiurl + '/user/' + userId + '/warning-count/unprocessed' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data == null || data.count == 0) {
                return;
            }
            var warningCount = data.count;
            $('.badge').html(warningCount);

            $('.notification').html('');
            $('.notification').append('<li><p>存在' + warningCount + '个未确认告警</p></li>');

            createWarningContentByUser(userId);
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取用户未确认告警数目时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function createWarningContentByUser(userId) {
    var url = apiurl + '/user/' + userId + '/warnings/unprocessed' + '?token=' + getCookie("token") + '&startRow=1&endRow=15';
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                return;
            }

            var count = 0;
            var sb = new StringBuffer();
            for (var i = 0; i < data.length; i++) {
                if (i === 0) {
                    setCookie('nowStructId', data[i].structId, null);
                }
                var warnings = data[i].warnings;
                if (warnings.length > 0) {
                    for (var j = 0; j < warnings.length; j++) {
                        if (count <= 5) {
                            var content = warnings[j].source + warnings[j].content;
                            if (content.length > 22) {
                                content = content.substring(0, 21);
                                content = content + '…';
                            }
                            sb.append('<li><a href="/DataWarningTest.aspx?structId=' + data[i].structId + '"><span class="label label-info"><i class="icon-bell"></i></span>' + content + '&nbsp;&nbsp;&nbsp;&nbsp;<span class="time">' + nowDateInterval(GetMilliseconds(warnings[j].time)) + '</span></a></li>');
                        }
                        count++;
                    }
                }
            }
            $('.notification').append(sb.toString());
            $('.notification').append('<li class="external"><a href="/DataWarningTest.aspx">更多<i class="m-icon-swapright"></i></a></li>');
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取用户未确认告警内容时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}


