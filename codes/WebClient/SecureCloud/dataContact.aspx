<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="dataContact.aspx.cs" Inherits="SecureCloud.dataContact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
    <link href="/resource/library/bootstrap/css/datepicker.css" rel="stylesheet" />
    <link href="/resource/css/unionboostrap.css" rel="stylesheet" />
    <link href="/resource/css/windowbox.css" rel="stylesheet" />
    <link href="/resource/library/tableTools/css/TableTools.css" rel="stylesheet" />
    <link href="/resource/library/data-tables/css/dataTable.css" rel="stylesheet" />
    <link rel="shortcut icon" href="favicon.ico" />

    <link href="data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />     
    <link href="/resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />
    <style>
        .mySelfCss {
            font-size: inherit;
        }
        .ui_timepicker {
            width: 140px;
        }
        .ui_time {
          width: 140px;  
        }
       
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid " style="min-width: 980px;">
        <div class="row-fluid">
            <ul class="breadcrumb" style="margin-top: 10px;">
                <li>
                    <i class="icon-home"></i>
                    <a href="/index.aspx">主页</a>
                     <i class="icon-angle-right"></i>
                </li>
                <li><a href="javascript:;">结构物</a><i class="icon-angle-right"></i></li>
                <li>
                    <small class="dropdown" style="display: inline-block;">
                        <a href="#" class="dropdown-toggle" data-toggle="dropdown"><i class="icon-angle-down"></i></> 
                        </a>
                        <ul class="dropdown-menu"></ul>
                    </small>
                </li>
            </ul>
            <div class="portlet box light-grey">
                <div class="portlet-body" id="comm1_graph">
                    <div class="form-horizontal" >
                        <div class="box" >
                            <div class="box-header">
                                <div class="box-icon">
                                    <i class="icon-bar-chart"></i>
                                    <span>对比对象</span>

                                    <a href="#" class="box-collapse pull-right">
                                        <img id="dataList" alt="" src="../resource/img/toggle-collapse.png" />
                                    </a>
                                </div>
                            </div>
                            <div class="box-content">
                                <table>
                                    <tr>
                                        <td style="width: 350px;text-align: left; vertical-align: top;">
                                            <div class="control-group">
                                                <table>
                                                    <tr>
                                                        <td><b>监测因素:</b></td>
                                                        <td>
                                                            <select id="factorList" name='factorList' class="width100 selectpicker" data-size="10" title="请选择">
                                                            </select>
                                                        </td>
                                                        </tr>
                                                    <tr>
                                                        <td><b>监测点位置:</b></td>
                                                        <td style="width: 120px;">
                                                            <select id="sensorList" name='sensorList' class="chzn-select" multiple="multiple" data-placeholder="请选择">
                                                            </select>
                                                        </td>

                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                       <%-- <td style="width: 10px;"></td>--%>
                                        <td style=" width:547px;text-align: left;vertical-align: top;">
                                            <div id="timeTable">
                                                <div class="control-group">
                                                    <table class="mySelfCss">
                                                        <tr>
                                                            <td><b>对比时段1:</b></td>
                                                            <td>
                                                                <div id="dpform1" class="input-append date">
                                                                    <input type="text" id="dform1" class="ui_timepicker" />
                                                                    <span class="add-on" style="height: 20px;">
                                                                        <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                                                    </span>
                                                                </div>
                                                            </td>
                                                            <td><b>至</b></td>
                                                            <td>
                                                                <div id="dpdend1" class="input-append date">
                                                                    <input type="text" id="ddend1" class="ui_time" />
                                                                    <span class="add-on" style="height: 20px;">
                                                                        <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                                                    </span>
                                                                </div>
                                                            </td>

                                                        </tr>
                                                    </table>
                                                </div>
                                                <div class="control-group">
                                                    <table class="mySelfCss">
                                                        <tr>
                                                            <td><b>对比时段2:</b></td>
                                                            <td>
                                                                <div id="dpform2" class="input-append date">
                                                                    <input type="text" id="dform2" class="ui_timepicker" />
                                                                    <span class="add-on" style="height: 20px;">
                                                                        <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                                                    </span>
                                                                </div>
                                                            </td>
                                                            <td style="text-align: center;"><b>至</b></td>
                                                            <td>
                                                                <div id="dpdend2" class="input-append date">
                                                                    <input type="text" id="ddend2" class="ui_time" />
                                                                    <span class="add-on" style="height: 20px;">
                                                                        <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                                                    </span>
                                                                </div>
                                                            </td>
                                                            <td>
                                                                <img id="expand_collapse" alt="" src="/resource/img/toggle-expand.png" style="width: 40px; height: 40px;" onclick="addTimeTable()" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </td>
                                          <td style="text-align: left; vertical-align: top;">
                                            <input type="button" id="btnQuery" value="查询" class="btn btn-success btndataquery" style="float: right" />

                                        </td>
                                    </tr>
                                   <%-- <tr>
                                      
                                    </tr>--%>
                                </table>
                            </div>
                        </div>

                        <div>
                            <div class="box" id="comm1" style="height: 360px;">
                            </div>
                            <div id="comm1_error" class="box" style="display: none;">
                            </div>
                        </div>
                    </div>
                    <%--<div id="tip" style="text-align: left;"></div>--%>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
         
<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script src="resource/js/jquery-1.8.3.min.js"></script>
     
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script src="/resource/library/data-tables/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="/resource/library/data-tables/DT_bootstrap.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-datepicker.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-dropdown.js"></script>

    <script src="../resource/library/highcharts/highcharts.js"></script>
    <script src="../resource/library/highcharts/exporting.js"></script>
    <script src="../resource/library/tableTools/js/ZeroClipboard.js"></script>
    <script src="../resource/library/tableTools/js/TableTools.min.js"></script>

    <script src="/resource/library/chosen-bootstrap/chosen/chosen.jquery.js"></script>
    
    <script src="data/js/bootstrap.min.js"></script>
    <script src="data/js/bootstrap-datetimepicker.js"></script> 

    <script src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>   

    <script src="data/dataInit.js"></script>
    <script src="dataContrast/monitorFactorStation.js"></script>
    <script src="resource/js/highchartTemplate.js"></script>
    <script src="commFactor/js/dataParse.js"></script>
    
    
    <script>
        jQuery(document).ready(function () {
            App.setPage("other");
            App.init();
        });
    </script>
</asp:Content>
