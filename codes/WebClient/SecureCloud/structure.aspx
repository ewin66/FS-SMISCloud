<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="structure.aspx.cs" Inherits="SecureCloud.structure" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />

    <link href="resource/library/jquery-bxslider/jquery.bxslider.css" rel="stylesheet" />

    <link href="resource/css/HotSpot.css" rel="stylesheet" />
    <style>
        .desc a:link, .desc a:active, .desc a:hover, .desc a:visited {
            text-decoration: none;
        }
        .tooltip-inner {
            max-width:500px;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- config 健康状态 -->

    <div id="health-config" class="modal hide">
        <div class="modal-header">
            <button data-dismiss="modal" class="close" type="button"></button>
            <h3>健康状态配置</h3>
        </div>
        <div class="modal-body">

            <div class="col-lg-12">
                <div class="input-group">
                    <span class="input-group-addon">
                        <input type="checkbox" checked="checked" />变形监测状态&nbsp;&nbsp;
                    </span>
                    <span class="input-group-addon">
                        <input type="checkbox" checked="checked" />应力/应变监测状态&nbsp;&nbsp;
                    </span>
                    <span class="input-group-addon">
                        <input type="checkbox" checked="checked" />受力监测状态&nbsp;&nbsp;
                    </span>
                    <span class="input-group-addon">
                        <input type="checkbox" checked="checked" />环境监测状态&nbsp;&nbsp;
                    </span>

                </div>

            </div>

            <button style="margin-top: 10px;" type="button" class="btn green">确定</button>

        </div>
    </div>
    <!-- config end -->

    <div class="container-fluid">
        <div class="row-fluid">
            <!-- 页面主题配置 -->
            <ul class="breadcrumb" style="margin-top: 10px;">
                <li>
                    <i class="icon-home"></i>
                    <a href="/index.aspx">主页</a>
                    <i class="icon-angle-right"></i>
                </li>
                <li><a href="javascript:;">结构物</a><i class="icon-angle-right"></i></li>
                <li>
                    <small class="dropdown" style="display: inline-block;">
                        <a href="javascript:;" class="dropdown-toggle" data-toggle="dropdown">
                            <i class="icon-angle-down"></i> 
                        </a>
                        <ul class="dropdown-menu">
                        </ul>
                    </small>
                </li>

            </ul>
            <!-- END 页面主题配置 -->
        </div>

        <!-- 结构物健康状态 -->
        <div class="row-fluid">
            <div class="span12">
                <div class="portlet paddingless">
                    <div class="portlet-title line">
                        <h4><i class="icon-ok"></i>健康状态</h4>
                        <div class="tools">
                            <a href="javascript:;" class="collapse"></a>
                            <%--<a href="#health-config" data-toggle="modal" class="config"></a>
                                <a href="javascript:;" class="reload"></a>--%>
                            <%--<a href="javascript:;" class="remove"></a>--%>
                        </div>
                    </div>
                    <div class="portlet-body" id="factor_status">

                        <div id="dashboard">
                        </div>
                    </div>
                </div>
            </div>           
            <%--<div class="tile bg-green">
                <div class="tile-body">
                    <i class="icon-th-large"></i>
                </div>
                <div class="tile-object">
                    <div class="name">
                        变形主题
                    </div>
                    <div class="number">
                        优
                    </div>
                </div>
            </div>   --%>         
        </div>
        <!-- end 结构物健康状态 -->

        <div class="clearfix"></div>

        <div class="row-fluid">
            <!-- topo -->
            <div class="span12">
                <!-- BEGIN PORTLET-->
                <div class="portlet paddingless">
                    <div class="portlet-title line" id="self-adaption_width">
                        <h4><i class="icon-map-marker"></i>Topo展示</h4>
                        <div class="tools">
                            <a href="javascript:;" class="collapse"></a>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div id="bxsliderThumbnailContainer" class="span2">
                            <%-- begin slider for section thumbnail --%>                               
                            <ul id="bxsliderThumbnail" class="bxslider"></ul>
                            <%-- end slider --%>
                        </div>
                        <%-- 热点图展示 --%>
                        <div class="span8" id="spanHeapMap">
                            <div class="row-fluid" id="topoContainer" style="width: 100%; height: auto; bottom: 15px; position: relative">
                                <a href='#' id='loading' class='marker' style='display: none; top: 0;'></a>
                                <img id="heapMapEle" style="display: block; width: 100%; height: auto;" />
                                <%--获得svg对象的前提--%>
                                <object id="svgEle" type="image/svg+xml" style="display: none; width: 100%; height: auto;"></object>

                                <%-- 干滩数据 --%>
                                <div id="divGanTan" style="position:relative; display:none; margin-top: 30px; ">
                                    <img id="imgGanTan" src="/resource/img/gantan.jpg" style="width:100%;" />
                                    <div id="gantanData"></div>
                                </div>
                                <%-- 实时数据 --%>
                                <div id="realData"></div>
                            </div>
                        </div>
                        <%--图例 --%>
                        <div class="span2" id="spanHeapMapLegend" style="margin-left: 10px; margin-top: 3px;"></div>
                    </div>
                </div>
                <!-- END PORTLET-->
            </div>
            <!-- end topo -->
        </div>

        <div class="clearfix"></div>

        <div class="row-fluid">

            <!-- 当前结构物告警 -->
            <div class="span6">
                <!-- BEGIN PORTLET-->
                <div class="portlet paddingless">
                    <div class="portlet-title line">
                        <h4><i class="icon-warning-sign"></i>结构物告警</h4>
                        <div class="tools">
                            <a href="javascript:;" class="collapse"></a>
                            <%-- <a href="#gis-config" data-toggle="modal" class="config"></a>
                                <a href="javascript:;" class="reload"></a>--%>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div class="scroller" data-always-visible="1" data-rail-visible1="1" id="warningScroller">
                            <ul class="feeds warningList">
                            </ul>
                        </div>
                    </div>
                </div>
                <!-- END PORTLET-->
            </div>
            <!-- END 当前结构物告警 -->

            <!-- 报表中心 -->
            <div class="span6">
                <!-- BEGIN PORTLET-->
                <div class="portlet paddingless">
                    <div class="portlet-title line">
                        <h4><i class="icon-bar-chart"></i>工作报表</h4>
                        <div class="tools">
                            <a href="javascript:;" class="collapse"></a>
                            <%--  <a href="#gis-config" data-toggle="modal" class="config"></a>
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
            <!-- END 报表中心 -->
        </div>

        <div class="clearfix"></div>


        <div class="row-fluid">
            <!-- 监测项图表 -->
            <div class="span12">
                <div class="portlet paddingless">
                    <div class="portlet-title line">
                        <h4><i class="icon-bell"></i>监测项图表</h4>
                        <div class="tools">
                            <a href="javascript:;" class="collapse"></a>
                            <%--<a href="#health-config" data-toggle="modal" class="config"></a>
                                <a href="javascript:;" class="reload"></a>
                                <a href="javascript:;" class="remove"></a>--%>
                        </div>
                    </div>
                    <div class="portlet-body" id="structchart">

                    </div>
                </div>
            </div>
            <!-- END 监测项图表 -->

        </div>
    </div>

    <form runat="server">
        <asp:HiddenField ID="reportpath" runat="server" ClientIDMode="Static" />
    </form>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="jqueryversion" runat="server">
    <script type="text/javascript" src="/resource/js/jquery-2.0.3.min.js"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footer" runat="server">
    <script src="/resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script src="/resource/library/highcharts/highcharts.js"></script>
    <script src="/resource/library/highcharts/highcharts-more.js"></script>
    <script src="/commFactor/js/dataParse.js"></script>
    <script src="/resource/js/StructChart.js"></script>
    <script src="/resource/js/structure.js"></script>
    <script src="commFactor/js/highcharDoubleAxis.js"></script>

    <script type="text/javascript" src="/resource/library/jquery-bxslider/jquery.bxslider.min.js"></script>
    <script type="text/javascript" src="/resource/library/bootstrap/js/bootstrap-tooltip.js"></script>

    <script>
        jQuery(document).ready(function () {
            App.setPage("index");  // set current page
            App.init(); // init the rest of plugins and elements        

        });
    </script>
    
    <script type="text/javascript" src="/resource/js/HotSpot.js"></script>
    <script src="resource/js/HotSpotRShell.js"></script>

</asp:Content>
