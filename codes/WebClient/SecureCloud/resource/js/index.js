
$(function () {
    //创建index页面标题
    var systemName = getCookie('systemName');
    
    if (systemName != null && systemName != ""&&systemName!="null") {
        //$('.pageT font').append(systemName);
        $('#SysName').append(systemName);
    }
    else {
        //$('.pageT font').append("(该用户下暂无组织)");
        $('#SysName').append("(该用户下暂无组织)");
    }

    var username = getCookie('loginname');

    $('#lblUser').html(username);
    createStrucMenu();

    createWarningBadgeAndContentByUser();
    $("[data-toggle='tooltip']").tooltip();
    getReports();
})

//生成结构物菜单、结构物状态
function createStrucMenu() {
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
        success: function (data) {
            if (data == null || data.length == 0) {
                $('#tipNoData-health').html('<span class="label label-important label-mini">当前用户无结构物</span>');
                return;
            }
            var strucSB = new StringBuffer();
            //var sb = new StringBuffer();
            var strucStatusSB = new StringBuffer();
            var str;
            //sb.append("<ul class='feeds'>");
            //去掉图片<img src="resource/img/logo_azteca.jpg" alt="" />
            for (var i = 0; i < data.length; i++) {
                if (i === 0) {
                    setCookie('nowStructId', data[i].structId, null); // set for click the menu '告警管理' directly.
                }
                strucSB.append('<li><a href="/structure.aspx?id=' + data[i].structId + '&imagename=' + data[i].imageName + '">' + data[i].structName + '</a></li>');
                strucStatusSB.append(' <div class="row-fluid portfolio-block "><div class="span10 portfolio-text">');
                strucStatusSB.append('<div class="portfolio-text-info"><h4>');
                strucStatusSB.append('<a href="/structure.aspx?id=' + data[i].structId + '&imagename=' + data[i].imageName + '">' + data[i].structName + '</a>');
                strucStatusSB.append('</h4></div></div><div class="span1"><div class="portfolio-info">');
                //if (data[i].worstWarning === null) {
                //    strucStatusSB.append('无');
                //} else {
                    switch (data[i].status) {
                        case "差":
                            strucStatusSB.append('<span style="color:#ff0000">差');
                            break;
                        case "劣":
                            strucStatusSB.append('<span style="color:#ff8000">劣');
                            break;
                        case "中":
                            strucStatusSB.append('<span style="color:#a757a8">中');
                            break;
                        case "良":
                            strucStatusSB.append('<span style="color:#0000ff">良');
                            break;
                        case "优":
                            strucStatusSB.append('<span style="color:#00ff00">优');
                            break;
                        default:
                            strucStatusSB.append('<span style="color:#555D69">无');

                    //}

                }
                    strucStatusSB.append('</span></div></div><div class="span1 portfolio-btn"><a href="/structure.aspx?id=' + data[i].structId + '&imagename=' + data[i].imageName + '" ><img src="/resource/img/right.png"></img></a></div></div>');

            //    var str = "";
            //    str = " <li><div class='col1'><div class='cont'><div class='cont-col1'><div class='label label-success'><i class='icon-list-alt'></i></div></div> ";
            //    str += "  <div class='cont-col2'><div class='desc'>最新年报：<a href='javascript:;'>2013年" + data[i].structName + "健康状况报告&nbsp;&nbsp;&nbsp;&nbsp;<span class='label label-important label-mini'>查看<i class='icon-share-alt'></i></span></a></div></div>";
            //    str += "</div></div><div class='col2'><div class='date'> 2014/02/01</div></div></li>";//最新年报

            //    str += " <li><div class='col1'><div class='cont'><div class='cont-col1'><div class='label label-success'><i class='icon-list-alt'></i></div></div> ";
            //    str += "  <div class='cont-col2'><div class='desc'>最新月报：<a href='javascript:;'>2014年3月" + data[i].structName + "健康状况报告&nbsp;&nbsp;&nbsp;&nbsp;<span class='label label-important label-mini'>查看<i class='icon-share-alt'></i></span></a></div></div>";
            //    str += "</div></div><div class='col2'><div class='date'> 2014/04/01</div></div></li>";//最新月报


            //    str += " <li><div class='col1'><div class='cont'><div class='cont-col1'><div class='label label-success'><i class='icon-list-alt'></i></div></div> ";
            //    str += "  <div class='cont-col2'><div class='desc'>最新日报：<a href='javascript:;'>2014年4月8日" + data[i].structName + "健康状况报告&nbsp;&nbsp;&nbsp;&nbsp;<span class='label label-important label-mini'>查看<i class='icon-share-alt'></i></span></a></div></div>";
            //    str += "</div></div><div class='col2'><div class='date'> 2014/04/09</div></div></li>";//最新日报
            //    sb.append(str);
            }

            $('.structure-list').html(strucSB.toString());
            $('.structure-status').html(strucStatusSB.toString());

            //sb.append("</ul>");
            //$('#statement').html('');
            //$('#statement').append(sb.toString());
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                //alert("权限验证出错");
                logOut();
            }
            else if (XMLHttpRequest.status == 400) {
                $('.structure-status').html("<span class='label label-important label-mini'>结构物健康状态加载失败</span>");
                //alert("参数错误");
            }
            else if (XMLHttpRequest.status == 500) {
                $('.structure-status').html("<span class='label label-important label-mini'>结构物健康状态加载失败</span>");
                //alert("内部异常");
            }
            else {
                $('.structure-status').html("<span class='label label-important label-mini'>结构物健康状态加载失败</span>");
                //alert('url错误');
            }
        }       
    });
}

/**
 * 创建所有结构物告警徽章及告警信息
 */
function createWarningBadgeAndContentByUser() {
    var userId = getCookie('userId');
    if (userId === '' || userId === null) {
        alert('获取用户Id失败，请检查浏览器Cookie是否已启用');
        return;
    }
    var url = apiurl + '/user/' + userId + '/warning-count/unprocessed' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data == null || data.count == 0) {
                $('#accordion1').html('<span class="label label-important label-mini">无告警数据</span>');
                return;
            }
            var warningCount = data.count;
            $('.badge').html(warningCount);

            $('.notification').html('');
            $('.notification').append('<li><p>存在' + warningCount + '个未确认告警</p></li>');

            createWarningContentByUser(userId);
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取用户未确认告警数目时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

// 创建所有结构物告警信息
function createWarningContentByUser(userId) {
    var url = apiurl + '/user/' + userId + '/warnings/unprocessed' + '?token=' + getCookie("token") + '&startRow=1&endRow=15';
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            if (data == null || data.length == 0) {
                return;
            }

            var len = data.length;
            var count = 0;
            var badgeSB = new StringBuffer();
            //告警列表
            var sb = new StringBuffer();
            for (var i = 0; i < len; i++) {
                sb.append('<div class="accordion-group"><div class="accordion-heading">');
                sb.append('<a class="accordion-toggle collapsed" data-toggle="collapse" data-parent="#accordion1" href="#collapse_' + i + '">');
                sb.append('<i class="icon-angle-left"></i><span>' + data[i].structName + '</span>');
                sb.append('<span style="position:absolute; right:6%;" onclick="showMoreWarnings(' + data[i].structId + ')">更多...</span></a></div>');
                
                if (i === 0) {
                    sb.append('<div id="collapse_' + i + '" class="accordion-body collapse in">');
                    sb.append('<div class="accordion-inner"><ul class="feeds">');
                } else {
                    sb.append('<div id="collapse_' + i + '" class="accordion-body collapse">');
                    sb.append('<div class="accordion-inner"><ul class="feeds">');
                }

                var warnings = data[i].warnings;
                if (warnings.length == 0) {
                    sb.append('<span class="label label-important label-mini">该结构物无告警数据</span>');
                }
                else {
                    for (var j = 0; j < warnings.length; j++) {
                        if (j >= 10) { // warning count is limited up to 10.
                            break;
                        }
                        sb.append('<li><a href="#"><div class="col1"><div class="cont"><div class="cont-col1"><div class="label label-important">');
                        sb.append('<i class="icon-bell"></i></div></div><div class="cont-col2"><div class="desc">' + warnings[j].source + warnings[j].content);
                        sb.append('<a href="/DataWarningTest.aspx?structId=' + data[i].structId + '"><span class="label label-important label-mini">处理<i class="icon-share-alt"></i></span></a></div></div></div>');
                        sb.append('</div><div class="col2"><div class="date">' + nowDateInterval(GetMilliseconds(warnings[j].time)) + '</div></div></a></li>');

                        if (count <= 5) {
                            var content = warnings[j].source + warnings[j].content;
                            if (content.length > 22) {
                                content = content.substring(0, 21);
                                content = content + '…';
                            }
                            badgeSB.append('<li><a href="/DataWarningTest.aspx" onclick="setnowStructId(' + data[i].structId + ')" ><span class="label label-info"><i class="icon-bell"></i></span>' + content + '&nbsp;&nbsp;&nbsp;&nbsp;<span class="time">' + nowDateInterval(GetMilliseconds(warnings[j].time)) + '</span></a></li>');
                        }
                        count++;
                    }
                }
                sb.append('</ul></div></div></div>');
            }
            $('#accordion1').html(sb.toString());

            $('.notification').append(badgeSB.toString());
            $('.notification').append('<li class="external"><a href="/DataWarningTest.aspx">更多<i class="m-icon-swapright"></i></a></li>');
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取用户未确认告警内容时发生异常.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }        
    });
}

function showMoreWarnings(structId) {
    window.location.href = '/DataWarningTest.aspx?structId=' + structId;
}

function setnowStructId(structId) {
    setCookie('nowStructId', structId);
}


//function strucStatusChart() {
//    $('#container').highcharts({
//        chart: {
//            type: 'bar'
//        },
//        title: {
//            text: '各结构物健康得分'
//        },

//        xAxis: {
//            categories: ['K765边坡', 'K775边坡', 'K791边坡'],
//            title: {
//                text: null
//            }
//        },
//        yAxis: {
//            min: 0,
//            max:100,
//            title: {
//                text: '健康得分 (%)',
//                align: 'high'
//            },
//            labels: {
//                overflow: 'justify'
//            },
//            //plotBands: [
//            //           {
//            //               from: 0,
//            //               to: 20,
//            //               color: '#FF0000' // red
//            //           },
//            //           {
//            //               from: 20,
//            //               to: 40,
//            //               color: '#FF7F00' // yellow
//            //           },
//            //           {
//            //               from: 40,
//            //               to: 60,
//            //               color: '#C800FF' // green
//            //           },
//            //           {
//            //               from: 60,
//            //               to: 80,
//            //               color: '#0000FF' // green
//            //           },
//            //           {
//            //               from: 80,
//            //               to: 100,
//            //               color: '#00FF00' // green
//            //           }
//            //]
//        },
//        tooltip: {
//            valueSuffix: ' %'
//        },
//        plotOptions: {
//            bar: {
//                dataLabels: {
//                    enabled: true
//                }
//            }
//        },
//        legend: {
//            layout: 'vertical',
//            align: 'right',
//            verticalAlign: 'top',
//            x: -40,
//            y: 100,
//            floating: true,
//            borderWidth: 1,
//            backgroundColor: '#FFFFFF',
//            shadow: true
//        },
//        credits: {
//            enabled: false
//        },
//        series: [{
//            showInLegend: false,
//            name: '',
//            data: [{ 'color': '#00FF00', 'y': 92 }, { 'color': '#00FF00', 'y': 98 }, { 'color': '#C800FF', 'y': 60 }]
//        }]
//    });
//}


function getReports() {
    var orgId = getCookie("orgId");
    if (orgId === '' || orgId === null) {
        alert('获取用户Id失败，请检查浏览器Cookie是否已启用');
        return;
    }
    var sb = new StringBuffer();
    var str = "";
    var temp = "";
    var url_day = apiurl + '/org/' + orgId + '/report/day' + '?token=' + getCookie("token");
    $.ajax({
        url: url_day,
        type: 'get',
        dataType: 'json',
        success: function (data) {
            //str += '<ul class="nav nav-tabs">';
            //str += '<li class="active"><a href="#tab_day" data-toggle="tab">日报表</a></li>';
            //str += '<li><a href="#tab_week" data-toggle="tab">周报表</a></li>';
            //str += '<li ><a href="#tab_month" data-toggle="tab">月报表</a></li>';
            //str += '<li ><a href="#tab_year" data-toggle="tab">年报表</a></li>';
            //str += '</ul>';
            //str += '<div class="tab-content scroller" style="overflow: hidden; width: auto; height: 290px;">';

            str += '<div class="tab-pane active" id="tab_day">';
            str += '<div class="scroller" data-height="290px" data-always-visible="1" data-rail-visible1="1">';
            str += '<ul class="feeds">';
            if (data.length == 0) {
                str += '<span class="label label-important label-mini">无日报表</span>';
            }
            else {
                for (var i = 0; i < data.length; i++) {
                    temp = "";
                    temp += '组织: ' + data[i].OrgName + ';&nbsp;&nbsp;' + '结构物: ' + data[i].StructName + ';&nbsp;&nbsp;' + '报表类型: ' + data[i].DateType + ';&nbsp;&nbsp;' + '生成日期: ' + data[i].time;
                    str += '<li><div class="col1"><div class="cont"><div class="cont-col1"><div class="label label-success"><i class="icon-list-alt"></i></div></div>';
                    str += '<div class="cont-col2"><div class="desc">';
                    str += '<span data-toggle="tooltip" data-placement="bottom" title=" ' + temp + '">' + data[i].reportName + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</span>';
                    str += '<a class="label label-important label-mini " onclick="DowmLoadReport(\'' + encodeURIComponent(data[i].url) + '\')">下载<i class="icon-share-alt"></i></a>';
                    str += '</div></div></div></div>';
                    str += '<div class="col2">';
                    str += '<div class="date">' + data[i].time + '</div></div></li>';
                }
            }
            str += '</ul>';
            str += '</div></div>';
            var url_week = apiurl + '/org/' + orgId + '/report/week' + '?token=' + getCookie("token");
            $.ajax({
                url: url_week,
                type: 'get',
                dataType: 'json',
                success: function (data) {
                    str += '<div class="tab-pane" id="tab_week">';
                    str += '<div class="scroller" data-height="290px" data-always-visible="1" data-rail-visible1="1">';
                    str += '<ul class="feeds">';
                    if (data.length == 0) {
                        str += '<span class="label label-important label-mini">无周报表</span>';
                    }
                    else {
                        for (var i = 0; i < data.length; i++) {
                            temp = "";
                            temp += '组织: ' + data[i].OrgName + ';&nbsp;&nbsp;' + '结构物: ' + data[i].StructName + ';&nbsp;&nbsp;' + '报表类型: ' + data[i].DateType + ';&nbsp;&nbsp;' + '生成日期: ' + data[i].time;
                            str += '<li><div class="col1"><div class="cont"><div class="cont-col1"><div class="label label-success"><i class="icon-list-alt"></i></div></div>';
                            str += '<div class="cont-col2"><div class="desc">';
                            str += '<span data-toggle="tooltip" data-placement="bottom" title=" ' + temp + '">' + data[i].reportName + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</span>';
                            str += '<a class="label label-important label-mini" onclick="DowmLoadReport(\'' + encodeURIComponent(data[i].url) + '\')">下载<i class="icon-share-alt"></i></a>';
                            str += '</div></div></div></div>';
                            str += '<div class="col2">';
                            str += '<div class="date">' + data[i].time + '</div></div></li>';
                        }
                    }
                    str += '</ul>';
                    str += '</div></div>';
                    var url_month = apiurl + '/org/' + orgId + '/report/month' + '?token=' + getCookie("token");
                    $.ajax({
                        url: url_month,
                        type: 'get',
                        dataType: 'json',
                        success: function (data) {
                            str += '<div class="tab-pane" id="tab_month">';
                            str += '<div class="scroller" data-height="290px" data-always-visible="1" data-rail-visible1="1">';
                            str += '<ul class="feeds">';
                            if (data.length == 0) {
                                str += '<span class="label label-important label-mini">无月报表</span>';
                            }
                            else {
                                for (var i = 0; i < data.length; i++) {
                                    temp = "";
                                    temp += '组织: ' + data[i].OrgName + ';&nbsp;&nbsp;' + '结构物: ' + data[i].StructName + ';&nbsp;&nbsp;' + '报表类型: ' + data[i].DateType + ';&nbsp;&nbsp;' + '生成日期: ' + data[i].time;
                                    str += '<li><div class="col1"><div class="cont"><div class="cont-col1"><div class="label label-success"><i class="icon-list-alt"></i></div></div>';
                                    str += '<div class="cont-col2"><div class="desc">';
                                    str += '<span data-toggle="tooltip" data-placement="bottom" title=" ' + temp + '">' + data[i].reportName + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</span>';
                                    str += '<a class="label label-important label-mini" onclick="DowmLoadReport(\'' + encodeURIComponent(data[i].url) + '\')">下载<i class="icon-share-alt"></i></a>';
                                    str += '</div></div></div></div>';
                                    str += '<div class="col2">';
                                    str += '<div class="date">' + data[i].time + '</div></div></li>';
                                }
                            }

                            str += '</ul>';
                            str += '</div></div>';

                            var url_year = apiurl + '/org/' + orgId + '/report/year' + '?token=' + getCookie("token");
                            $.ajax({
                                url: url_year,
                                type: 'get',
                                dataType: 'json',
                                success: function (data) {
                                    str += '<div class="tab-pane" id="tab_year">';
                                    str += '<div class="scroller" data-height="290px" data-always-visible="1" data-rail-visible1="1">';
                                    str += '<ul class="feeds">';
                                    if (data.length == 0) {
                                        str += '<span class="label label-important label-mini">无年报表</span>';
                                    }
                                    else {
                                        for (var i = 0; i < data.length; i++) {
                                            temp = "";
                                            temp += '组织: ' + data[i].OrgName + ';&nbsp;&nbsp;' + '结构物: ' + data[i].StructName + ';&nbsp;&nbsp;' + '报表类型: ' + data[i].DateType + ';&nbsp;&nbsp;' + '生成日期: ' + data[i].time;
                                            str += '<li><div class="col1"><div class="cont"><div class="cont-col1"><div class="label label-success"><i class="icon-list-alt"></i></div></div>';
                                            str += '<div class="cont-col2"><div class="desc">';
                                            str += '<span data-toggle="tooltip" data-placement="bottom" title=" ' + temp + '">' + data[i].reportName + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</span>';
                                            str += '<a class="label label-important label-mini" onclick="DowmLoadReport(\'' + encodeURIComponent(data[i].url) + '\')">下载<i class="icon-share-alt"></i></a>';
                                            str += '</div></div></div></div>';
                                            str += '<div class="col2">';
                                            str += '<div class="date">' + data[i].time + '</div></div></li>';
                                        }
                                    }
                                    str += '</ul>';
                                    str += '</div></div>';
                                    sb.append(str);
                                    $('#statement .tab-content').append(sb.toString());
                                },
                                error: function () {
                                }
                            })

                        },
                        error: function () {

                        }
                    })

                },
                error: function () {

                }
            })

        },
        error: function () {

        }
    })




    //var sb = new StringBuffer();
    //var str = "";
    //str += '<ul class="nav nav-tabs">';
    //str += '<li class="active"><a href="#tab_2_1" data-toggle="tab">日报表</a></li>';
    //str += '<li ><a href="#tab_2_2" data-toggle="tab">月报表</a></li>';
    //str += '<li ><a href="#tab_2_3" data-toggle="tab">年报表</a></li>';
    //str += '</ul>';
    //str += '<div class="tab-content" style="overflow: hidden; width: auto; height: 290px;">';

    //str += '<div class="tab-pane active" id="tab_2_1">';
    //str += '<div class="scroller" data-height="290px" data-always-visible="1" data-rail-visible1="1"> <h2>adfajf</h2> </div>';
    //str += '</div>';

    //str += '<div class="tab-pane " id="tab_2_2">';
    //str += '<div class="scroller" data-height="290px" data-always-visible="1" data-rail-visible1="1"> <h2>hjkghk</h2> </div>';
    //str += '</div>';

    //str += '<div class="tab-pane " id="tab_2_3">';
    //str += '<div class="scroller" data-height="290px" data-always-visible="1" data-rail-visible1="1"> <h2>afsfgsdfgdfghjkghk</h2> </div>';
    //str += '</div>';

    //str += '</div>';

    //sb.append(str);

    //$('#statement').html('');
    //$('#statement').append(sb.toString());


    //$('#statement').html("<span class='label label-important label-mini'>无报表</span>");
    //$('#statement').removeClass('scroller');

}

function DowmLoadReport(rpturl) {
    var url = '/DownLoad.ashx?fileName=' + rpturl;
    window.open(url);
}

//function saveImageAs(imgOrURL) {
//    imgOrURL = '/img/'+imgOrURL;
//    if (typeof imgOrURL == 'object')
//        imgOrURL = imgOrURL.src;
//    window.win = open(imgOrURL);
//    setTimeout('win.document.execCommand("SaveAs")', 500);
//}