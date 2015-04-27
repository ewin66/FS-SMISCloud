<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SecureCloud.index" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="renderer" content="webkit" />
    <title></title>

    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <meta content="" name="description" />
    <meta content="" name="author" />

    <style>
        .dtStyle {
            color: white;
            margin-left: 60px;
        }
    </style>

    <link href="/resource/library/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="/resource/css/metro.css" rel="stylesheet" />
    <link href="/resource/library/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" />

    <link href="/resource/library/font-awesome/css/font-awesome.css" rel="stylesheet" />
    <link href="/resource/css/style.css" rel="stylesheet" />
    <link href="/resource/css/style_responsive.css" rel="stylesheet" />
    <link href="/resource/css/style_default.css" rel="stylesheet" id="style_color" />
    <link href="/resource/library/gritter/css/jquery.gritter.css" rel="stylesheet" />
    <link href="/resource/library/uniform/css/uniform.default.css" rel="stylesheet" />
    <link rel="shortcut icon" href="favicon.ico" />
    <link href="/resource/css/windowbox.css" rel="stylesheet" />

    <script src="/resource/js/jquery-1.8.3.min.js"></script>
    <style>
        .portfolio-text h4 {
            color: #555d69;
            font-size: 20px;
            font-weight: 400;
            margin-top: 10px;
        }
        /* extended dropdowns */
        .dropdown-menu.extended {
            min-width: 160px !important;
            max-width: 330px !important;
            width: 300px !important;
            background-color: #ffffff !important;
        }

        .dropdown-menu.notification li a .time {
            font-size: 12px;
            font-style: italic;
            font-weight: 600;
            position: absolute;
            right: 10px;
        }

        /* 固定左侧菜单栏 */
        .page-sidebar {
            position: fixed;
        }
    </style>
    <!--隐藏百度地图logo的样式-->
    <style type="text/css">
        .anchorBL {
            display: none;
        }
    </style>
    <script>
        $(function () {
            var color = getCookie('color');
            if (color != '' && color != null) {
                $('#style_color').attr("href", "resource/css/style_" + color + ".css");
            }
        });
    </script>
    <script src="js/googleanalytics.js"></script>
</head>
<body class="fixed-top">
    <div class="header navbar navbar-inverse navbar-fixed-top">
        <!-- BEGIN TOP NAVIGATION BAR -->
        <div class="navbar-inner">
            <div class="container-fluid">
                <!-- BEGIN LOGO -->
                <div>
                    <a class="brand" href="index.aspx">
                        <div class="OrgLogoContain" style="width: 100px; height: 20px;">
                        </div>
                    </a>
                    <!-- END LOGO -->
                </div>
                <%-- <div class="span8">
                    <marquee class="pageT" scrollamount="4" direction="left" behavior="scroll" style="margin-top: 10px;"><font color="#ffffff" size="5px;" ></font></marquee>
                </div>--%>
                <div class="span10" style="margin-top: 10px;" align="left">
                        <font id="SysName" color="#ffffff" size="5px;"></font>
                </div>
                <%--<div>--%>
                    <!-- BEGIN RESPONSIVE MENU TOGGLER -->
                    <a href="javascript:;" class="btn-navbar collapsed" data-toggle="collapse" data-target=".nav-collapse">
                        <img src="/resource/img/menu-toggler.png" alt="" />
                    </a>
                    <!-- END RESPONSIVE MENU TOGGLER -->


                    <!-- BEGIN TOP NAVIGATION MENU -->
                    <ul class="nav pull-right">
                        <!-- modified by xiezhen -->
                        <!-- BEGIN NOTIFICATION DROPDOWN -->

                        <li class="dropdown" id="header_notification_bar">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                                <i class="icon-warning-sign"></i>
                                <span class="badge"></span>
                            </a>
                            <ul class="dropdown-menu extended notification">
                                <%--<li>
                                <p>存在14个未确认告警！</p>
                            </li>
                            <li>
                                <a href="javascript:;" onclick="App.onNotificationClick(1)">
                                    <span class="label label-info"><i class="icon-bell"></i></span>
                                    New user registered. 
								<span class="time">Just now</span>
                                </a>
                            </li>
                             
                            
                            <li class="external">
								<a href="#">更多<i class="m-icon-swapright"></i></a>
							</li>--%>
                            </ul>
                        </li>


                        <li class="dropdown">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                                <span id="lblUser"></span>
                                <i class="icon-angle-down"></i>
                            </a>
                            <ul class="dropdown-menu">
                                <li><a href="/account.html"><i class="icon-user"></i>账户</a></li>
                                <li class="divider"></li>
                                <li><a id="btnLogout" href="javascript:logOut();"><i class="icon-key"></i>退出</a></li>
                            </ul>
                        </li>
                        <!-- END USER LOGIN DROPDOWN -->
                    </ul>
                    <!-- END TOP NAVIGATION MENU -->
                <%--</div>--%>
            </div>
        </div>
        <!-- END TOP NAVIGATION BAR -->
    </div>
    <!-- END HEADER -->
    <!-- BEGIN CONTAINER -->
    <div class="page-container row-fluid">
        <!-- BEGIN SIDEBAR -->
        <div class="page-sidebar nav-collapse collapse" style="z-index: 999">
            <!-- BEGIN SIDEBAR MENU -->
            <ul>
                <li>
                    <!-- BEGIN SIDEBAR TOGGLER BUTTON -->
                    <div class="sidebar-toggler hidden-phone"></div>
                    <!-- BEGIN SIDEBAR TOGGLER BUTTON -->
                </li>
                <li>
                    <!-- BEGIN RESPONSIVE QUICK SEARCH FORM -->
                    <form class="sidebar-search">
                        <div class="input-box">
                            <a href="javascript:;" class="remove"></a>
                            <input type="text" placeholder="搜索..." />
                            <input type="button" class="submit" disabled="disabled" value=" " />
                        </div>
                    </form>
                    <!-- END RESPONSIVE QUICK SEARCH FORM -->
                </li>
                <li class="index active has-sub">
                    <a href="/index.aspx">
                        <i class="icon-home"></i>
                        <span class="title">主页</span>
                        <span class="selected"></span>
                    </a>
                </li>
                <li class="has-sub" id="struct-menu">
                    <a href="javascript:;">
                        <i class="icon-file"></i>
                        <span class="title">结构物</span>
                        <span class="arrow "></span>
                    </a>
                    <ul class="sub structure-list">
                    </ul>
                </li>
                <%--<li class="topo">
                    <a href="/TOPO.aspx">
                        <i class="icon-picture"></i>
                        <span class="title">TOPO</span>
                        <span class="selected"></span>
                    </a>
                </li>--%>
                <li class="warning">
                    <a href="/userWarning.aspx">
                        <i class="icon-bell"></i>
                        <span class="title">告警管理</span>
                        <span class="selected"></span>
                    </a>
                </li>

                <li class="has-sub">
                    <a href="/Report.aspx">
                        <i class="icon-table"></i>
                        <span class="title">报表管理</span>
                        <span class="selected"></span>
                    </a>
                </li>

                <li class="has-sub " id="systemConfig">
                    <a href="javascript:;">
                        <i class="icon-wrench"></i>
                        <span class="title">系统配置</span>
                        <span class="arrow "></span>
                    </a>
                    <ul class="sub">

                        <li id="userLog"><a href="/UserLog.aspx">用户日志</a></li>
                        <li id="smsPush"><a href="/Param_Alert.aspx">告警推送配置</a></li>
                    </ul>
                </li>
                <%--<li class="has-sub ">
                    <a href="javascript:;">
                        <i class="icon-file"></i>
                        <span class="title">工作报表</span>
                        <span class="arrow "></span>
                    </a>
                    <ul class="sub">
                        <li><a href="/Statement.aspx?flag=0">异常报表</a></li>
                        <li><a href="/Statement.aspx?flag=1">日报表</a></li>
                        <li><a href="/Statement.aspx?flag=2">周报表</a></li>
                        <li><a href="/Statement.aspx?flag=3">月报表</a></li>
                        <li><a href="/Statement.aspx?flag=4">季报表</a></li>
                        <li><a href="/Statement.aspx?flag=5">年报表</a></li>
                    </ul>
                </li>--%>
                <%--<li class="has-sub " id="manage">
                    <a href="javascript:;">
                        <i class="icon-star"></i>
                        <span class="title">应急管理</span>
                        <span class="arrow "></span>
                    </a>
                    <ul class="sub">
                        <li><a href="#">视频</a></li>
                        <li><a href="#">车辆荷载</a></li>
                    </ul>
                </li>
                <li class="has-sub " id="systemConfig">
                    <a href="javascript:;">
                        <i class="icon-wrench"></i>
                        <span class="title">系统配置</span>
                        <span class="arrow "></span>
                    </a>
                    <ul class="sub">
                        <li><a href="#">用户配置</a></li>
                        <li><a href="#">阈值配置</a></li>
                        <li><a href="#">告警推送配置</a></li>
                    </ul>
                </li>--%>

                <%--<li class="has-sub" id="hotspotBU">
                    <a href="javascript:;">
                        <i class="icon-map-marker"></i>
                        <span class="title">热点图布点</span>
                        <span class="arrow "></span>
                    </a>
                    <ul class="sub">
                        <li><a href="TopoSettingSVG.aspx">SVG布点</a></li>
                        <li><a href="TopoSetting.aspx">位图布点</a></li>
                    </ul>
                </li>--%>

                <li class="">
                    <a href="javascript:logOut();">
                        <i class="icon-user"></i>
                        <span class="title">退出</span>
                    </a>
                </li>
            </ul>
        </div>
        <!-- END SIDEBAR -->
        <!-- BEGIN PAGE -->
        <div class="page-content">

            <div class="container-fluid">
                <!-- BEGIN PAGE HEADER-->
                <div class="row-fluid">
                    <!-- BEGIN STYLE CUSTOMIZER -->
                    <%-- <div class="color-panel hidden-phone">
                            <div class="color-mode-icons icon-color"></div>
                            <div class="color-mode-icons icon-color-close"></div>

                            <div class="color-mode">
                                <p>主题颜色</p>
                                <ul class="inline">
                                    <li class="color-black current color-default" data-style="default"></li>
                                    <li class="color-blue" data-style="blue"></li>
                                    <li class="color-brown" data-style="brown"></li>
                                    <li class="color-purple" data-style="purple"></li>
                                    <li class="color-white color-light" data-style="light"></li>
                                </ul>--%>
                    <%-- <label class="hidden-phone">
                                    <input type="checkbox" class="header" value="" />
                                    <span class="color-mode-label">监测项图表</span>
                                </label>--%>
                    <%--<label class="hidden-phone">
                                    <input type="checkbox" class="header" checked="checked" value="" />
                                    <span class="color-mode-label">GIS拓扑</span>
                                </label>
                                <label class="hidden-phone">
                                    <input type="checkbox" class="header" checked="checked" value="" />
                                    <span class="color-mode-label">结构物告警</span>
                                </label>
                                <label class="hidden-phone">
                                    <input type="checkbox" class="header" checked="checked" value="" />
                                    <span class="color-mode-label">结构物状态得分</span>
                                </label>
                                <label class="hidden-phone">
                                    <input type="checkbox" class="header" checked="checked" value="" />
                                    <span class="color-mode-label">资讯中心</span>
                                </label>--%>
                    <%--      </div>
                        </div>--%>
                    <!-- END BEGIN STYLE CUSTOMIZER -->
                    <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                    <%--  <h3 class="page-title">安心云—</h3>--%>
                    <ul class="breadcrumb" style="margin-top: 10px;">
                        <li>
                            <i class="icon-home"></i>
                            <a href="index.aspx">主页</a>
                        </li>
                    </ul>

                    <!-- END PAGE TITLE & BREADCRUMB-->
                </div>
                <!-- END PAGE HEADER-->

                <div class="row-fluid">

                    <!-- GIS拓扑 -->
                    <div class="span6">
                        <!-- BEGIN REGIONAL STATS PORTLET-->
                        <div class="portlet">
                            <div class="portlet-title">
                                <h4><i class="icon-globe"></i>GIS拓扑</h4>
                                <div class="tools">
                                    <a href="javascript:;" class="collapse"></a>
                                    <%--<a href="#gis-config" data-toggle="modal" class="config"></a>--%>
                                    <%--<a href="javascript:;" class="reload"></a>--%>
                                </div>
                            </div>
                            <div class="portlet-body">
                                <div id="tipNoData-map"></div>
                                <div id="map_canvas" style="height: 340px"></div>
                            </div>
                        </div>
                        <!-- END REGIONAL STATS PORTLET-->
                    </div>
                    <!-- END GIS拓扑 -->


                    <!-- 结构物状态 -->
                    <div class="span6">
                        <!-- BEGIN PORTLET-->
                        <div class="portlet paddingless">
                            <div class="portlet-title line">
                                <h4><i class="icon-tags"></i>结构物健康状态</h4>
                                <div class="tools">
                                    <a href="javascript:;" class="collapse"></a>
                                    <%--<a href="#gis-config" data-toggle="modal" class="config"></a>--%>
                                    <%--<a href="javascript:;" class="reload"></a>--%>
                                </div>
                            </div>
                            <div class="portlet-body">
                                <div id="tipNoData-health"></div>
                                <div class="scroller structure-status" id="structure-status" data-height="340" data-always-visible="1" data-rail-visible1="1">
                                </div>
                            </div>
                            <%--<div id="container" style="height:300px;"></div>--%>
                        </div>
                        <!-- END PORTLET-->
                    </div>
                    <!-- END 结构物状态 -->

                </div>
                <div class="clearfix"></div>

                <div class="row-fluid">

                    <!-- 结构物告警 -->
                    <div class="span6">
                        <!-- BEGIN PORTLET-->
                        <div class="portlet paddingless">
                            <div class="portlet-title line">
                                <h4><i class="icon-warning-sign"></i>结构物告警</h4>
                                <div class="tools">
                                    <a href="javascript:;" class="collapse"></a>

                                    <%--<a href="javascript:;" class="reload"></a>--%>
                                </div>
                            </div>
                            <div class="portlet-body">
                                <!--scroller有时候需要放在外层的div中才有效果-->
                                <div class="scroller">
                                    <div class="accordion" id="accordion1">
                                        <%--<div class="accordion-group">
                                            <div class="accordion-heading">
                                                <a class="accordion-toggle collapsed" data-toggle="collapse" data-parent="#accordion1" href="#collapse_1">
                                                    <i class="icon-angle-left"></i>
                                                    Collapsible Group Item #1
                                                </a>
                                            </div>
                                            <div id="collapse_1" class="accordion-body collapse in">
                                                <div class="accordion-inner">
                                                    Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod.
                                                </div>
                                            </div>
                                        </div>
                                        <div class="accordion-group">
                                            <div class="accordion-heading">
                                                <a class="accordion-toggle collapsed" data-toggle="collapse" data-parent="#accordion1" href="#collapse_2">
                                                    <i class="icon-angle-left"></i>
                                                    Collapsible Group Item #2
                                                </a>
                                            </div>
                                            <div id="collapse_2" class="accordion-body collapse">
                                                <div class="accordion-inner">
                                                    Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor.
                                                </div>
                                            </div>
                                        </div>--%>
                                    </div>
                                </div>
                                <!--BEGIN TABS-->
                                <%--  <div class="tabbable tabbable-custom tabs-left" id="struct-warning">
                                    <ul class="nav nav-tabs">
                                    </ul>
                                    <div class="tab-content scroller">
                                    </div>
                                </div>--%>
                                <!--END TABS-->
                            </div>
                        </div>
                        <!-- END PORTLET-->
                    </div>
                    <!-- END 结构物告警 -->

                    <!-- 资讯中心 -->
                    <div class="span6">
                        <!-- BEGIN PORTLET-->
                        <div class="portlet paddingless">
                            <div class="portlet-title line">
                                <h4><i class="icon-bar-chart"></i>工作报表</h4>
                                <div class="tools">
                                    <a href="javascript:;" class="collapse"></a>
                                    <%--<a href="#gis-config" data-toggle="modal" class="config"></a>
                                    <a href="javascript:;" class="reload"></a>
                                    <a href="javascript:;" class="remove"></a>--%>
                                </div>
                            </div>
                            <div class="portlet-body">
                                <!--BEGIN TABS-->
                                <div class="tabbable tabbable-custom" id="statement">
                                    <ul class="nav nav-tabs">
                                        <li class="active"><a href="#tab_day" data-toggle="tab">日报表</a></li>
                                        <li><a href="#tab_week" data-toggle="tab">周报表</a></li>
                                        <li><a href="#tab_month" data-toggle="tab">月报表</a></li>
                                        <li><a href="#tab_year" data-toggle="tab">年报表</a></li>
                                    </ul>
                                    <div class="tab-content scroller">
                                    </div>
                                </div>
                                <!--END TABS-->

                            </div>
                        </div>
                        <!-- END PORTLET-->
                    </div>
                    <!-- END 资讯中心 -->

                </div>
            </div>
        </div>
        <!-- END PAGE -->
    </div>
    <!-- END CONTAINER -->

    <div class="footer" id="copyright"  align="center">
        
        <div class="span pull-right">
            <span class="go-top"><i class="icon-angle-up"></i></span>
        </div>
    </div>

    <form runat="server">
        <asp:HiddenField ID="reportpath" runat="server" ClientIDMode="Static" />
    </form>



    <script src="/resource/library/breakpoints/breakpoints.js"></script>
    <script src="/resource/library/jquery-ui/jquery-ui-1.10.1.custom.min.js"></script>
    <script src="/resource/library/jquery-slimscroll/jquery.slimscroll.min.js"></script>
    <script src="/resource/library/bootstrap/js/bootstrap.min.js"></script>
    <script src="/resource/uipackage/uiTheme.js"></script>
    <script src="/resource/js/jquery.blockui.js"></script>
    <script src="resource/js/jquery.cookie.js"></script>
    <script src="resource/js/common.js"></script>

    <%-- <script src="/resource/library/jquery-slimscroll/jquery-ui-1.9.2.custom.min.js"></script>
    <script src="/resource/library/jquery-slimscroll/jquery.slimscroll.min.js"></script>--%>
    <%-- <script src="/resource/library/flot/jquery.flot.js"></script>
    <script src="/resource/library/flot/jquery.flot.resize.js"></script>--%>
    <script src="/resource/library/uniform/jquery.uniform.min.js"></script>
    <script src="/resource/library/gritter/js/jquery.gritter.min.js"></script>
    <script src="/resource/js/jquery.pulsate.min.js"></script>
    <script src="/resource/js/app.js"></script>
    <script src="resource/js/securecloud.js"></script>
    <script src="/resource/library/highcharts/highcharts.js"></script>

    <script type="text/javascript" src="http://api.map.baidu.com/api?v=2.0&ak=wEiighBCdHAkOrXRHDsqlgW5"></script>
    
    <script type="text/javascript" src="resource/js/mapGIS.js"></script>
    <script src="/resource/js/index.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            App.setPage("index");  // set current page
            App.init(); // init the rest of plugins and elements

            $('#struct-menu').click(function () {
                $('.has-sub').removeClass('active');
                $('#struct-menu').addClass('active');
            });

            $('#systemConfig').click(function () {
                $('.has-sub').removeClass('active');
                $('#systemConfig').addClass('active');
            });

            $('#manage').click(function () {
                $('.has-sub').removeClass('active');
                $('#manage').addClass('active');
            });

            $('#hotspotBU').click(function () {
                $('.has-sub').removeClass('active');
                $('#hotspotBU').addClass('active');
            });
        });
    </script>
    <script src="resource/library/bootstrap/js/bootstrap-tooltip.js"></script>
</body>
</html>
