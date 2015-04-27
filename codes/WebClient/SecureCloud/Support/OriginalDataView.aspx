<%@ Page Title="" Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="OriginalDataView.aspx.cs" Inherits="SecureCloud.Support.OriginalDataView" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/datepicker.css" rel="stylesheet" />
    <link href="../resource/css/unionboostrap.css" rel="stylesheet" />
    
    <link href="../resource/library/tableTools/css/TableTools.css" rel="stylesheet" />
    <link href="../resource/library/data-tables/css/dataTable.css" rel="stylesheet" />
    <%--<link href="../resource/css/glyphicons.css" rel="stylesheet" />--%>
    <link rel="shortcut icon" href="favicon.ico" />
    <%--<link href="../resource/css/halflings.css" rel="stylesheet" />--%>

    <link href="../data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />
    <link href="../resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />

    <link href="css/commonSupport.css" rel="stylesheet" />

     <style>
        .width100 {
            width: 200px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <!-- BEGIN PAGE HEADER-->
        <div class="row-fluid">
            <ul class="breadcrumb">
                <li>
                    <i class="icon-bar-chart"></i>
                    <a href="OriginalDataView.aspx">原始数据查看</a>
                    <i class="icon-angle-right"></i>
                </li>
                <li><a href="javascript:;">组织</a><i class="icon-angle-right"></i></li>
                <li>
                    <select id="listOrg" class="chzn-select dropdown-menu" style="width: 300px;" data-size="10" data-placeholder="请选择">
                    </select>
                </li>
                <li><i class="icon-angle-right"></i><a href="javascript:;">结构物</a><i class="icon-angle-right"></i></li>
                <li>
                    <select id="structList" class="chzn-select dropdown-menu" style="width: 300px;" data-size="10" data-placeholder="请选择">
                    </select>
                </li>
            </ul>
        </div>
        <!-- END PAGE HEADER -->

        <div class="row-fluid">
            <div class="portlet box light-grey">
                <div class="box-content">
                    <%--<div class="portlet-body" style="width: auto">--%>
                    <%--<div class="box-content">--%>
                        <div class="row-fluid">
                            <div id="tip" class="fixed-prompt" style="text-align: center"></div>
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <b style="width: 75px; font-size: 12px;">监测因素：</b>
                                    <select id="factorList" name="factorList" data-size="10" title="请选择" class="width100 selectpicker"></select>
                                    <b style="margin-left: 10px; font-size: 12px;">设备位置 :</b>
                                    <select id="sensorList" name='sensorList' class="width100" multiple="multiple" data-size="10" title="请选择"></select>
                                    <select id="date" name="date" class="width100" style="height: 34px;">
                                        <option value="day" class="btn-date">最近一天</option>
                                        <option value="week" class="btn-date">最近一周</option>
                                        <option value="month" class="btn-date">最近一月</option>
                                        <option value="other" class="btn-minimize-other">其他</option>
                                    </select>
                                    <input type="button" id="btnQuery" value="查询" class="btn green" />
                                </div>
                                <%-- BEGIN 时间控件 --%>
                                <div class="other-search" style="margin-top: 20px; display: none;">
                                    <label class="control-label" style="width: 75px;font-size:12px;">
                                        <b>监测时间从: </b>
                                    </label>
                                    <div id="dpform1" class="input-append date">
                                        <input type="text" id="dpform" />
                                        <span class="add-on" style="height: 20px;">
                                            <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                        </span>

                                    </div>
                                    <b>至</b>
                                    <div id="dpdend1" class="input-append date">
                                        <input type="text" id="dpdend" />
                                        <span class="add-on" style="height: 20px;">
                                            <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                        </span>
                                    </div>
                                </div>
                                <%-- END 时间控件 --%>
                            </div>

                            <div class="box-content span12" id="graph">
                                <%-- 无数据提示区域 --%>
                                <div class="span10">
                                    <div class="box" id="datagraph" style="height: 360px; display: none;">
                                    </div>
                                    <div id="tipNoData" class="hide">
                                        <span class="label-important text-15">抱歉，没有查询到任何有效的数据</span>
                                    </div>
                                </div>

                                <div class="span3">
                                    <div class="box" id="warnBox" style="display: none">
                                        <table id="warnTable" class="warnTable">
                                            <tr class="heightTr">
                                                <th class="headTh">位置:</th>
                                                <th id="XsensorLocation" class="Th"></th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">历史告警:</th>
                                                <th id="warnTh" class="Th">
                                                    <a id="XsensorLink" style="cursor: pointer">查看</a>
                                                </th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">最大值:</th>
                                                <th id="XmaxValue" class="Th"></th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">最小值:</th>
                                                <th id="XminValue" class="Th"></th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">一级阈值:</th>
                                                <th id="XfirstThreshold" class="Th"></th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">二级阈值:</th>
                                                <th id="XsecondThreshold" class="Th"></th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">三级阈值:</th>
                                                <th id="XthirdThreshold" class="Th"></th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">四级阈值:</th>
                                                <th id="XfourthThreshold" class="Th"></th>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                                <%--<div id="tipNoData" class="hide">
                            <span class="label-important text-15">此测点该时间段内无数据</span>
                        </div>--%>
                                <%-- <div id="chart" class="span10">
                            <div class="box" id="datagraph" style="height: 360px;">
                            </div>
                        </div>--%>
                            </div>
                        </div>

                    

                <%--</div>--%>

                <%-- 表格区域 --%>
                <div id="box-table" class="box">
                    <div class="box-header">
                        <div class="box-icon">
                            <a href="#" class="box-collapse pull-right">
                                <img id="expand_collapse" alt="" src="../resource/img/toggle-collapse.png" />
                            </a>
                        </div>
                    </div>
                    <div class="box-content data-table-content" style="display: none;">
                        <div id="datatable" style="margin-bottom: 40px;">
                        </div>
                    </div>
                </div>
                <%--</div>--%>
            </div>
        </div>
    </div>
        </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script src="../resource/js/jquery-1.8.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script src="../resource/library/data-tables/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="/resource/library/data-tables/DT_bootstrap.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-datepicker.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-dropdown.js"></script>

    <script src="../resource/library/highcharts/highcharts.js"></script>
    <script src="../resource/library/highcharts/exporting.js"></script>
    <script src="../resource/library/tableTools/js/ZeroClipboard.js"></script>
    <script src="../resource/library/tableTools/js/TableTools.min.js"></script>

    <script src="../resource/library/chosen-bootstrap/chosen/chosen.jquery.js"></script>
    <script src="../resource/js/highchartTemplate2.js"></script>
  
    
    <script src="../data/js/bootstrap.min.js"></script>
    <script src="../data/js/bootstrap-datetimepicker.js"></script>    
    <script src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>
    <script src="../resource/js/datepickerInit.js"></script>
    <script type="text/javascript" src="../resource/js/dataTableInit.js"></script>
    <script src="js/originalData.js"></script>
</asp:Content>
