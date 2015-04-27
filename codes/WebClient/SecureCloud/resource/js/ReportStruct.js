
$(function () {
    $('#data-contact').addClass('active');
    $('#ReportStruct').addClass('active');


    var nowstructId = null;
    if (location.href.split('=')[1] == null || location.href.split('=')[1] == undefined) {
        nowstructId = getCookie("nowStructId");
    } else {
        nowstructId = location.href.split('=')[1];
    }

    if (nowstructId != null && nowstructId != undefined && nowstructId != "") {
        setCookie('nowStructId', nowstructId);
    }
    structShow(nowstructId);
    getReportTables(nowstructId);

});

function structShow(nowstructId) {

    var userId = getCookie('userId');
    if (userId === '' || userId === null) {
        // alert('获取用户Id失败，请检查浏览器Cookie是否已启用');
        window.location.href = '/login.html';
        return;
    }
    var url = apiurl + '/user/' + userId + '/structs' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data === null || data.length === 0) {
                return;
            }
            var sb = new StringBuffer();
            var flag = true;
            for (var i = 0; i < data.length; i++) {
                if (data[i].structId == parseInt(nowstructId)) {
                    $('.breadcrumb li small a').html(data[i].structName + '<i class="icon-angle-down"></i>');
                    if (i == 0) {
                        flag = false;
                    }
                } else {
                    if (i > 0 && flag) {
                        sb.append('<li class="divider"></li>');
                    }
                    flag = true;
                    sb.append('<li><a href="/ReportStruct.aspx?id=' + data[i].structId + '">' + data[i].structName + '</a></li>');
                }
            }
            $('.breadcrumb li small ul').html(sb.toString());
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("权限验证出错");
                logOut();
            }
            else if (XMLHttpRequest.status == 400) {
                //alert("参数错误");
            }
            else if (XMLHttpRequest.status == 500) {
                //alert("内部异常");
            }
            else {
                //alert('url错误');
            }
        }
    });
}

function getReportTables(structId) {

    var url_day = apiurl + '/struct/' + structId + '/report/day' + '?token=' + getCookie("token");
    var url_day_count = apiurl + '/struct/' + structId + '/report-count/day' + '?token=' + getCookie("token");
    setTableOption('DayTable', url_day, url_day_count);

    var url_week = apiurl + '/struct/' + structId + '/report/week' + '?token=' + getCookie("token");
    var url_week_count = apiurl + '/struct/' + structId + '/report-count/week' + '?token=' + getCookie("token");
    setTableOption('WeekTable', url_week, url_week_count);

    var url_month = apiurl + '/struct/' + structId + '/report/month' + '?token=' + getCookie("token");
    var url_month_count = apiurl + '/struct/' + structId + '/report-count/month' + '?token=' + getCookie("token");
    setTableOption('MonthTable', url_month, url_month_count);

    var url_year = apiurl + '/struct/' + structId + '/report/year' + '?token=' + getCookie("token");
    var url_year_count = apiurl + '/struct/' + structId + '/report-count/year' + '?token=' + getCookie("token");
    setTableOption('YearTable', url_year, url_year_count);
}

function setTableOption(tableId, url, url_count) {
    $('#' + tableId).dataTable({
        "bAutoWidth": false,
        "aLengthMenu": [
                [10, 25, 50, -1],
                [10, 25, 50, "All"]
        ],

        "iDisplayLength": 10,//每页显示个数
        "bStateSave": true,
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "aoColumns": [
               { "mData": 'report_name' },
               { "mData": 'report_time' },
               { "mData": 'report_download' }

        ],
        "bSort": false,//是否启动各个字段的排序功能
        "sPaginationType": "full_numbers",//默认翻页样式设置
        //"bFilter": false,//禁用搜索框
        "bProcessing": true,//table数据载入时，是否显示进度提示
        "bServerSide": true,//是否启动服务端数据导入，即要和AjaxSource结合使用
        "sAjaxSource": "/reportHandler.ashx?now=" + Math.random() + "&Url=" + url + "&Url_count=" + url_count
    });
}

function DownLoadReport(rpturl) {
    var url = '/DownLoad.ashx?fileName=' + rpturl;
    window.open(url);
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

