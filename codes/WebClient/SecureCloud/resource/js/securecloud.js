/*
//-----------------securecloud.js为“云平台”公用js文件-----------------//
*/

$(function () {
    if (getCookie("OrgLogo") != null && getCookie("OrgLogo") != "" && getCookie("OrgLogo") != "null") {
        var img = '<img src="/resource/img/OrgLogo/' + getCookie("OrgLogo") + '" style="width:100px;height:20px;" alt="logo" />';
        $('.OrgLogoContain').html(img);
    }
    else {
        var img = '<img src="/resource/uipackage/logo/logo.png" style="width:100px;height:20px;" alt="logo" />';
        $('.OrgLogoContain').html(img);
    }
})

//接口访问url公共部分
function getRootPath() {
    var strFullPath = window.document.location.href;
    var strPath = window.document.location.pathname;
    //考虑移动端点击登录时候不会执行这段js，直接进入页面strPath获取的是'/',定位'/'在'http://192.168.1.103/'里的位置会返回5，从而出错，现在默认去掉字符串的最后一位
    if (strPath == '/') {
        var prePath = strFullPath.substring(0, strFullPath.length - 1);
    }
    else {
        var pos = strFullPath.indexOf(strPath);
        var prePath = strFullPath.substring(0, pos);
    }      
    return prePath;
}
var apiurl = getRootPath() + "/Proxy.ashx?path=";

function logOut() {
    var url = getCookie("loginUrl");
    if (getCookie("loginUrl") == "") {
        url = "/login.html";
    }

    var token = getCookie('token');
    if (token != null && token != '') {
        $.ajax({
            type: 'post',
            async: false,
            url: apiurl + '/user/logout/' + token
        });        

        delCookie("loginname");
        delCookie("userId");
        delCookie("orgId");
        delCookie("organization");
        delCookie("systemName");
        delCookie("roleId");
        delCookie("nowStructId");
        delCookie('nowStructName');
        delCookie('CurrentSectionId');
        delCookie('SectionStructName');
        delCookie("supportStructIds");
        delCookie("OrgLogo");
        delCookie("token");
    }
    window.top.location.href = url;
}


//验证用户访问权限
//function verifyUserAccess() {
//    //alert(getCookie("username"));
//    if (getCookie("username") == "undefined" || getCookie("username") == null || getCookie("username") == "") {
//        window.parent.location.href = "login.aspx";
//    }
//}

//设置cookie值
function setCookie(c_name, value, expiredays) {
    var exdate = new Date()
    exdate.setDate(exdate.getDate() + expiredays)
    document.cookie = c_name + "=" + encodeURIComponent(value) + ";path=/" +
    ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString());
}

//获取cookie值
function getCookie(c_name) {
    if (document.cookie.length > 0) {
        c_start = document.cookie.indexOf(c_name + "=")
        if (c_start != -1) {
            c_start = c_start + c_name.length + 1
            c_end = document.cookie.indexOf(";", c_start)
            if (c_end == -1) c_end = document.cookie.length
            return decodeURIComponent(document.cookie.substring(c_start, c_end))
        }
    }
    return ""
}

//删除cookie值
function delCookie(c_name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(c_name);
    document.cookie = c_name + "=" + cval + ";path=/" + ";expires=" + exp.toGMTString();
}

//字符串缓冲
function StringBuffer() {
    this.data = [];
}

StringBuffer.prototype.append = function () {
    this.data.push(arguments[0]);
    return this;
}

StringBuffer.prototype.toString = function () {
    return this.data.join("");
}

function alertTips(parameters, tipcolor, containerId, timeout) {

    $('#' + containerId).html('<div class="row-fluid" id="alert-tip' + containerId + '" style="display: none;"><span class="label" id="alert-tip-text'+containerId+'" style="margin-top: 5px;">01</span></div>');

    var alerttext = $('#alert-tip-text'+containerId);

    if (alerttext.text() == parameters)
        return;

    $('#alert-tip'+containerId).slideToggle();
    $('#alert-tip-text'+containerId).html(parameters);
    $('#alert-tip-text'+containerId).remove('label-success').remove('label-important');
    $('#alert-tip-text' + containerId).addClass(tipcolor);
    if (timeout != undefined) {
        setTimeout(function () {
            $('#alert-tip' + containerId).slideToggle();
            $('#alert-tip-text' + containerId).html(' ');
        }, timeout);
    }     
}

//input中屏蔽特殊字符的输入，如：<input disabled="disabled" type="text" id='userNameToEdit' onkeypress="TextValidate()" />
function TextValidate() {
    var code;
    var character;
    if (document.all) //判断是否是IE浏览器
    {
        code = window.event.keyCode;
    }
    else {
        code = arguments.callee.caller.arguments[0].which;
    }
    var character = String.fromCharCode(code);

    var txt = new RegExp("[ ,\\`,\\~,\\!,\\@,\#,\\$,\\%,\\^,\\+,\\*,\\&,\\\\,\\/,\\?,\\|,\\:,\\.,\\<,\\>,\\{,\\},\\(,\\),\\',\\;,\\=,\"]");
    //特殊字符正则表达式
    if (txt.test(character)) {
        alert("密码不可以包含以下特殊字符:\n , ` ~ ! @ # $ % ^ + & * \\ / ? | : . < > {} () [] \" ");
        if (document.all) {
            window.event.returnValue = false;
        }
        else {
            arguments.callee.caller.arguments[0].preventDefault();
        }
    }
}

