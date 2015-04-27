// ҳ���ʼ
var themetext = {
    title: "",
    copyright: ""
};


$(function () {
    //console.debug("����");
    
    initThemeText();
    var id = document.getElementsByTagName("title");
    if (typeof (id) != "undefined" && themetext.title != null) {
        document.title = themetext.title;
    }

    id = document.getElementById("copyright");
    if (typeof (id) != "undefined" && themetext.copyright != null) {
        id.innerHTML = themetext.copyright;
    }
   
});

function initThemeText() {
   
    var dataroot = "/resource/uipackage/data/data.json";
    $.ajaxSettings.async = false;
    $.getJSON(dataroot, function (data) {
        //console.debug(data.title);
        //console.debug(data.copyright);
        themetext.title = data.title;
        themetext.copyright = data.copyright;
    });
    // var text = { "title": "", "copyright": "Copyright &copy; 2014 <a class='copyright' href='http://www.free-sun.com.cn'>���пƼ�</a> &ndash; <a class='copyright' href='http://www.miitbeian.gov.cn'>��ICP��13030678��</a>" };
   // var text = { "title": "", "copyright": ""};
}