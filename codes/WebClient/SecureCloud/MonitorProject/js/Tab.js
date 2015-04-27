$(function () {
    
    var structId = "";
    var themeid = document.getElementById('HiddenThemeNo').value;
    var factorId = document.getElementById('HiddenFactorId').value;
    var sensorId = document.getElementById('HiddenSensorId').value;
    
    setTimeout(function() {
        //var a1 = $('#theme_' + themeid);
        $('#theme_' + themeid).addClass('active');
    }, 1000);
    if (sensorId == 1755 || sensorId == 1747 || sensorId == 1753) {
        structId = 82;
    } else {
        structId = getCookie("nowStructId");
    }
    
    if (structId == null || structId == "") {
        alertTips('数据获取失败, 请尝试退回到上级,重新进入', 'label-important', 'tabtip', 5000);
        return;
    }
    else {
        //获得监测因素因子
        MonitorTab(themeid, structId, factorId, sensorId);
    }

    $('#monitorList').addClass('active');
    structShow(structId);

});

function structShow(structId) {
    var userId = getCookie('userId');
    if (userId === '' || userId === null) {
        alert('获取用户Id失败，请检查浏览器Cookie是否已启用');
        return;
    }
    var url = apiurl + '/user/' + userId + '/structs' + '?token=' + getCookie("token");
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
            var flag = true;
            for (var i = 0; i < data.length; i++) {
                if (data[i].structId == parseInt(structId)) { //选择那个哪个为第一个
                    $('.breadcrumb li small span').html(data[i].structName);
                    if (i == 0) {
                        flag = false;
                    }
                } else {
                    if (i > 0 && flag) {
                        sb.append('<li class="divider"></li>');
                    }
                    flag = true;
                }
            }
            $('.breadcrumb li small ul').html(sb.toString());
           // $('.breadcrumb li small ul').selectpicker('refresh');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("权限验证出错");
                logOut();
            } else if (XMLHttpRequest.status != 0) {
                alert("获取结构物列表时发生异常");
            }
        }
    });
}

////////////tab标题////////////
function MonitorTab(themeid, structId, fId, sId) {
    var url = apiurl + '/struct/' + structId + '/factors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data.length == 0) {
                alertTips('该结构物下没有监测因素', 'label-important', 'tabtip', 5000);
            }
            else {
                var tabfactortitle = '';
                var tabfirstContent = '';
                var tabOtherContent = '';
                var tabTitle = '';
                var tabContent = '';
                var dataTab = '';

                var flag = 0;
                //判断主题id号跟哪个data[i].factorid相同，相同则加载该主题下的tab页面
                for (var i = 0; i < data.length; i++) {
                    if (data[i].factorId == themeid) {
                        dataTab = data[i].children;
                        for (var j = 0; j < dataTab.length; j++) {
                            //切换监测因子tab页面，判断ifm跳转页面
                            var factorHref = "";
                            var factorId = '';
                            var valueNumber = '';
                            factorId = dataTab[j].factorId;
                            valueNumber = dataTab[j].valueNumber;

                            switch (factorId) {
                                //温湿度、风速风向（从菜单栏进去）、风速风向仰角双坐标轴-(factorId-30网壳三坐标)
                                case 5: factorHref = "DoubleChar.aspx?factorid=" + factorId + "&sensorId=" + sId;
                                    break;
                                //case 30: factorHref = "DoubleChar.aspx?factorid=" + factorId + "&sensorId=" + sId;
                                //    break;//
                                    //有沉降针对点、组
                                case 31:
                                case 42:
                                    factorHref = 'SettleGroup.aspx?factorid=' + factorId + "&sensorId=" + sId;
                                    break;
                                case 10: factorHref = "DeepDispChar.aspx?factorid=" + factorId + "&sensorId=" + sId;
                                    break;
                                case 33:
                                    factorHref = "VibrationChar.aspx?factorid=" + factorId + "&sensorId=" + sId;
                                    break; 
                                case 34:
                                    factorHref = "InfiltrationLineChar.aspx?factorid=" + factorId + "&sensorId=" + sId;
                                    break;
                            }
                            if (factorHref == "") {
                                switch (valueNumber) {
                                    case 1:
                                        if (factorId == 18)
                                        { break; }
                                        else if (factorId == 52) {//杆件应力
                                            factorHref = "TwoCharStressStrain.aspx?factorid=" + factorId + "&sensorId=" + sId;
                                        } else if (factorId == 54) {//网壳振动
                                            factorHref = "VibrationCharReticulatedShell.aspx?factorid=" + factorId + "&sensorId=" + sId;
                                        } 
                                        else {
                                            factorHref = "OneChar.aspx?factorid=" + factorId + "&sensorId=" + sId;
                                        }
                                        break;
                                    case 2:
                                        switch (factorId) {
                                            case 5: break;
                                            case 18: factorHref = "windRoseTheme.aspx?factorid=" + factorId + "&sensorId=" + sId; break;
                                            default: factorHref = "TwoChar.aspx?factorid=" + factorId + "&sensorId=" + sId;
                                        }
                                        break;
                                    case 3:
                                        switch (factorId) {
                                            case 30: factorHref = "windRoseTheme.aspx?factorid=" + factorId + "&sensorId=" + sId; break;
                                            case 56: factorHref = "SlightShockChar.aspx?factorid=" + factorId + "&sensorId=" + sId; break;
                                            default: factorHref = "ThreeChar.aspx?factorid=" + factorId + "&sensorId=" + sId;
                                        }
                                        break;
                                }
                            }
                            //setTimeout(function () {
                            //$('#factor_' + factorid).addClass('active');
                            //}, 1000)

                            //判断tab1与其他tab class="active"
                            var iii = 0;
                            if (dataTab[j].factorId == 51) {//杆件应变
                                iii = j;
                                flag = 1;
                            }
                            else {
                                if (fId == null || fId == '') {//从主题进入
                                    if (!flag) {
                                        iii = 0;
                                    } else {
                                        if (iii > 0) {
                                            iii = 0;
                                        } else {
                                            iii = 1;
                                        }
                                    }
                                    if (j == iii) {
                                        tabfactortitle = '<li class="active">' + '<a href="#tab' + (j + 1) + '" data-toggle="tab">' + dataTab[j].factorName + '</a>' + '</li>';
                                        tabfirstContent = '<div class="tab-pane active" id="tab' + (j + 1) + '">';
                                    } else {
                                        tabfactortitle = '<li>' + '<a href="#tab' + (j + 1) + '" data-toggle="tab">' + dataTab[j].factorName + '</a>' + '</li>';
                                        tabfirstContent = '<div class="tab-pane " id="tab' + (j + 1) + '">';
                                    }
                                } else {
                                    if (fId == factorId) {
                                        tabfactortitle = '<li class="active">' + '<a href="#tab' + (j + 1) + '" data-toggle="tab">' + dataTab[j].factorName + '</a>' + '</li>';
                                        tabfirstContent = '<div class="tab-pane active" id="tab' + (j + 1) + '">';
                                    } else {
                                        tabfactortitle = '<li>' + '<a href="#tab' + (j + 1) + '" data-toggle="tab">' + dataTab[j].factorName + '</a>' + '</li>';
                                        tabfirstContent = '<div class="tab-pane " id="tab' + (j + 1) + '">';
                                    }
                                }

                                tabOtherContent = '<div class="scroller" data-height="290px" data-always-visible="1" data-rail-visible1="1">'
                                    + '<div id="iframe">'
                                    + '<iframe id="ifm" name="ifm" src="' + factorHref + ' "frameborder="0" scrolling="no" width="100%" onload="iframeAutoFit(this)"></iframe>'
                                    + '</div>'
                                    + '</div>'
                                    + '</div>';
                                var tabfactorContent = tabfirstContent + tabOtherContent;
                                tabTitle += tabfactortitle;
                                tabContent += tabfactorContent;
                            }
                        }
                        $('#tab').append(tabTitle);
                        $('#tabContent').append(tabContent);
                    }
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else {
                alertTips('数据获取失败, 请尝试退回到上级,重新进入', 'label-important', 'tabtip', 5000);
            }
        }
    });
}
////////////iframe自适应高度//////////
var Sys = {};
var ua = navigator.userAgent.toLowerCase();
var s;
//各类浏览器及其版本的正则表达式
(s = ua.match(/msie ([\d.]+)/)) ? Sys.ie = s[1] :
(s = ua.match(/firefox\/([\d.]+)/)) ? Sys.firefox = s[1] :
(s = ua.match(/chrome\/([\d.]+)/)) ? Sys.chrome = s[1] :
(s = ua.match(/opera.([\d.]+)/)) ? Sys.opera = s[1] :
(s = ua.match(/version\/([\d.]+).*safari/)) ? Sys.safari = s[1] : 0;

if (Sys.opera || Sys.safari) {
    window.setInterval("reinitIframe()", 200);
}

//针对opera safari
function reinitIframe() {
    var iframe = document.getElementByIdx_x_x_x("ifm");
    try {
        var bHeight = iframe.contentWindow.document.body.scrollHeight;
        var dHeight = iframe.contentWindow.document.documentElement.scrollHeight;
        var height = Math.max(bHeight, dHeight);
        iframe.height = height;
    } catch (ex) { }
}

function iframeAutoFit(iframeObj) {
    setInterval(function () {
        if (!iframeObj)
            return;
        try {
            iframeObj.height = (iframeObj.Document ? iframeObj.Document.body.scrollHeight : iframeObj.contentDocument.body.offsetHeight) + 20;
        } catch (e) {
        }
    }, 200);
}




