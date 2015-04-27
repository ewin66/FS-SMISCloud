$(function () {
    var factorId = document.getElementById('HiddenFactorNo').value;
    var sensorId = document.getElementById('HiddenSensorId').value;
    var flag = document.getElementById('HiddenFlag').value;

    $('#monitorList').addClass('active');

    setTimeout(function () {
        $('#factor_' + factorId).addClass('active');
    }, 1000)

    var src1 = "";
    var src2 = "";
    switch (flag) {
        case 0: $('#tab1').addClass('active'); break;
        case 1: $('#tab2').addClass('active'); break;
        default: $('#tab1').addClass('active'); break;
    }
    src1 = "../MonitorProject/WindRowChar.aspx?factorid=" + factorId + "&sensorId=" + sensorId;
    src2 = "../MonitorProject/DoubleChar.aspx?factorid=" + factorId + "&sensorId=" + sensorId;

    $('#ifm1').attr("src", src1);
    $('#ifm2').attr("src", src2);
});

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
    window.setInterval("reinitIframe1()", 200);
    window.setInterval("reinitIframe2()", 200);
}

//针对opera safari
function reinitIframe1() {
    var iframe = document.getElementByIdx_x_x_x("ifm1");
    try {
        var bHeight = iframe.contentWindow.document.body.scrollHeight;
        var dHeight = iframe.contentWindow.document.documentElement.scrollHeight;
        var height = Math.max(bHeight, dHeight);
        iframe.height = height;
    } catch (ex) { }
}

function reinitIframe2() {
    var iframe = document.getElementByIdx_x_x_x("ifm2");
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
