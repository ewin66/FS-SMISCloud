/**
 * ---------------------------------------------------------------------------------
 * <copyright file="userAccessVerification.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2014 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述：系统使用者访问网站页面时进行权限验证的js文件
 *
 * 创建标识：PengLing20141202
 *
 * 修改标识：
 * 修改描述：
 * </summary>
 * ---------------------------------------------------------------------------------
 */

/**
 * 验证用户访问权限
 */
function verifyUserAccess() {
    // console.log("loginname = " + getCookie("loginname"));
    if (getCookie("loginname") == "") {
        window.parent.location.href = "/login.html";
    }
}

/**
 * 设置cookie值
 */
function setCookie(cName, value, expiredays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + expiredays);
    document.cookie = cName + "=" + encodeURIComponent(value) + ";path=/" +
    ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString());
}

/**
 * 获取cookie值
 */
function getCookie(cName) {
    if (document.cookie.length > 0) {
        var cStart, cEnd;
        cStart = document.cookie.indexOf(cName + "=");
        if (cStart != -1) {
            cStart = cStart + cName.length + 1;
            cEnd = document.cookie.indexOf(";", cStart);
            if (cEnd == -1) cEnd = document.cookie.length;
            return decodeURIComponent(document.cookie.substring(cStart, cEnd));
        }
    }
    return "";
}

/**
 * 删除cookie值
 */
function delCookie(cName) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(cName);
    document.cookie = cName + "=" + cval + ";path=/" + ";expires=" + exp.toGMTString();
}