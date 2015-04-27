$(function () {

    var factorid = document.getElementById('HiddenFactorNo').value;
    var sensorId = document.getElementById('HiddenSensorId').value;

    $('#monitorList').addClass('active');

    setTimeout(function () {
        $('#factor_' + factorid).addClass('active');
    }, 1000)
    ////沉降点、组标志
    MonitorTab(factorid, sensorId);
});

////////////tab标题////////////
function MonitorTab(factorid, sensorId) {
    var tabFirstTitle = '';
    var tabSecoTitle = '';
    var tabFistContent = '';
    var tabSecoContent = '';
    var tabfirstName = '';
    var tabSecoName = '';
    var tabFirstSrc = '';
    var tabSecoSrc = '';

    tabfirstName = "单点变化";
    tabSecoName = "分组对比";

    tabFirstSrc = "../MonitorProject/OneChar.aspx?factorId=" + factorid + "&sensorId=" + sensorId;
    tabSecoSrc = "../MonitorProject/SettleContrast.aspx?factorId=" + factorid;

    tabFirstTitle = '<li class="active">' + '<a href="#tab1" data-toggle="tab">' + tabfirstName + '</a>' + '</li>';
    tabFistContent = '<div class="tab-pane active" id="tab1">'
        + '<div class="scroller" data-height="290px" data-always-visible="1" data-rail-visible1="1">'
        + '<div id="iframe">'
        + '<iframe id="ifm" name="ifm" src="' + tabFirstSrc + '" frameborder="0" scrolling="no" width="100%" onload="iframeAutoFit(this)"></iframe>'
        + '</div>'
        + '</div>'
        + '</div>';

    tabSecoTitle = '<li>' + '<a href="#tab2" data-toggle="tab">' + tabSecoName + '</a>' + '</li>';
    tabSecoContent = '<div class="tab-pane " id="tab2">'
    + '<div class="scroller" data-height="290px" data-always-visible="1" data-rail-visible1="1">'
    + '<div id="iframe">'
    + '<iframe id="ifm" name="ifm" src="' + tabSecoSrc + '" frameborder="0" scrolling="no" width="100%" onload="iframeAutoFit(this)"></iframe>'
    + '</div>'
    + '</div>'
    + '</div>';
    var t = tabFirstTitle + tabSecoTitle;
    var tt = tabFistContent + tabSecoContent;
    $('#tab').append(tabFirstTitle + tabSecoTitle);
    $('#tabContent').append(tabFistContent + tabSecoContent);
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