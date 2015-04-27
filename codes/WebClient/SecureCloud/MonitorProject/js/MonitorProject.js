var structId = "";

$(function () {
    var data = getCookie("dataStructId");
    if (data != null && data != '') {
        setCookie('nowStructId', data);
    }
    setTimeout(function () {
        structId = getCookie("nowStructId");
        
        
        if (structId == null || structId == "") {
            alertTips('结构物信息获取失败, 请尝试退回到上级,重新进入', 'label-important', 'tip', 5000);
            return;
        } else {
            //获得监测因素因子
            getFactor(structId);
        }

    }, 500);

});

//获得监测因素因子
function getFactor(structId) {
    var url = apiurl + '/struct/' + structId + '/factors?token=' + getCookie('token');

    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.length == 0) {
                //alert("没有监测因素");
                return;
            }
            else {
                var monitorElement = '';
                for (var i = 0; i < data.length; i++) {
                    monitorElement = '<a class="ajax-link"   href="../MonitorProject/Tab.aspx?themeId=' + data[i].factorId + '"  id="' + data[i].factorId + '" title="' + data[i].factorName + '">'
                    + data[i].factorName + '</a>';
                    var factor = data[i].children;
                    var monitorFactor = '';
                    for (var j = 0; j < factor.length; j++) {
                        if (factor[j].factorId != 51) {
                            monitorFactor += '<dt id="factor_' + factor[j].factorId + '"><a  class="ajax-link" href="../MonitorProject/Tab.aspx?themeId=' + data[i].factorId + '&factorId=' + factor[j].factorId + '" id="' + factor[j].factorId + '" title="' + factor[j].factorName + '">'
                                     + '<font color="white"   style="font-weight: normal;">' + factor[j].factorName + '</font></a></dt>';
                        }
                    }
                    $('#MonitorProject').append('<li id="theme_' + data[i].factorId + '">' + monitorElement + '<dl style="margin:0px">' + monitorFactor + '</dl>' + '</li>');
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("登陆已超时，请重新登陆");
                logOut();
            }
            else if (XMLHttpRequest.status == 400) {
                //alert("参数错误");
                $('#MonitorProject').html('<li style="text-align:center"><dl class="label label-important label-mini">获取监测因子列表失败</dl></li>');
            }
            else if (XMLHttpRequest.status == 500) {
                //alert("内部异常");
                $('#MonitorProject').html('<li style="text-align:center"><dl class="label label-important label-mini">获取监测因子列表失败</dl></li>');
            }
            else {
                //alert('url错误');
                $('#MonitorProject').html('<li style="text-align:center"><dl class="label label-important label-mini">获取监测因子列表失败</dl></li>');
            }
        }
    });
}

function clickMonitorFactor(factorId, sensorId) {
    var url = apiurl + '/struct/' + structId + '/factors?token=' + getCookie('token');

    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        async: false,
        success: function (data) {
            if (data.length == 0) {
                return;
            }
            else {
                var themeId;
                for (var i = 0; i < data.length; i++) {
                    var factor = data[i].children;
                    for (var j = 0; j < factor.length; j++) {
                        if (factor[j].factorId == factorId) {
                            themeId = data[i].factorId;
                            break;
                        }
                    }
                }
                if (themeId != null) {
                    location.assign('../MonitorProject/Tab.aspx?themeId=' + themeId +
                        '&factorId=' + factorId +
                        '&sensorId=' + sensorId);
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("登陆已超时，请重新登陆");
                logOut();
            }
            else if (XMLHttpRequest.status == 400) {
                //alert("参数错误");
                $('#MonitorProject').html('<li style="text-align:center"><dl class="label label-important label-mini">获取监测因子列表失败</dl></li>');
            }
            else if (XMLHttpRequest.status == 500) {
                //alert("内部异常");
                $('#MonitorProject').html('<li style="text-align:center"><dl class="label label-important label-mini">获取监测因子列表失败</dl></li>');
            }
            else {
                //alert('url错误');
                $('#MonitorProject').html('<li style="text-align:center"><dl class="label label-important label-mini">获取监测因子列表失败</dl></li>');
            }
        }
    });
}