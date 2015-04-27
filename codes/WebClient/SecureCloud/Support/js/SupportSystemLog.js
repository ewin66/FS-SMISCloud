$(function () {
    $('#logSupport').addClass('active');
    $('#SupportSystemLog').addClass('active');


    GetSupportSystemLogTable();

    $('#btnDownload').click(function () {
        var url_dowmload = apiurl + '/syslog' + '?token=' + getCookie("token");
        var href = '/ExcelDownload.ashx?Url=' + url_dowmload;
        window.open(href);
    })
})

function GetSupportSystemLogTable() {
    $('#SupportSystemLogTable').dataTable().fnDestroy();
    var url = apiurl + '/syslog' + '?token=' + getCookie("token");
    var url_count = apiurl + '/syslog-count' + '?token=' + getCookie("token");
    $('#SupportSystemLogTable').dataTable({
        "aLengthMenu": [
                [10, 25, 50, -1],
                [10, 25, 50, "All"]
        ],

        "iDisplayLength": 25,
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "aoColumns": [
                { "mData": 'systemlog_time' },
               { "mData": 'systemrlog_level' },
               { "mData": 'systemrlog_processname' },
               { "mData": 'systemrlog_filename' },
               { "mData": 'systemrlog_codenum' },
               { "mData": 'systemrlog_msg' },
               { "mData": 'systemrlog_exception' }
        ],
        "bSort": false,
        "sPaginationType": "full_numbers",
        //"bFilter": false,//禁用搜索框
        "bProcessing": true,
        "bServerSide": true,
        "sAjaxSource": "/systemlog.ashx?now=" + Math.random() + "&Url=" + url + "&Url_count=" + url_count
    });
}

