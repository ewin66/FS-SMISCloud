
var src = "";
$(function () {
    var sensorId = document.getElementById('HiddenSensorId').value; 
    var factorId = 18;
    factorId = location.href.split('=')[1].split('&')[0];
    var styleValue="";

    addChar(styleValue, factorId, sensorId);

    $('#style').change(function () {
        styleValue = $('#style :selected').text();
        addChar(styleValue, factorId, sensorId);
    });

});

function addChar(styleValue, factorId, sensorId) {
    switch (styleValue) {
        case "风玫瑰图": src = "../MonitorProject/WindRowChar.aspx?factorid=" + factorId + "&sensorId=" + sensorId; break;
        case "风曲线图":
            if (factorId == 30) {
                src = "../MonitorProject/ThreeChar.aspx?factorid=" + factorId + "&sensorId=" + sensorId; break;
            } else{
                src = "../MonitorProject/DoubleChar.aspx?factorid=" + factorId + "&sensorId=" + sensorId; break;
            }
        default: src = "../MonitorProject/WindRowChar.aspx?factorid=" + factorId + "&sensorId=" + sensorId; break;
    }
    $('#ifm').attr("src", src);
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



