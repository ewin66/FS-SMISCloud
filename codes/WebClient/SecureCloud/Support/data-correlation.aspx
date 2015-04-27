<%@ Page Title="" Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="data-correlation.aspx.cs" Inherits="SecureCloud.Support.data_correlation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/datepicker.css" rel="stylesheet" />
    <link href="../resource/css/unionboostrap.css" rel="stylesheet" />
    <link href="../resource/css/windowbox.css" rel="stylesheet" />
    <link href="../resource/library/tableTools/css/TableTools.css" rel="stylesheet" />
    <link href="../resource/library/data-tables/css/dataTable.css" rel="stylesheet" />
    <%--<link href="../resource/css/glyphicons.css" rel="stylesheet" />--%>
    <link rel="shortcut icon" href="favicon.ico" />
   <%-- 不在同一目录下，找上层--%>
    <%--<link href="../resource/css/halflings.css" rel="stylesheet" />--%>        
    <link href="../data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />   
    <link href="../resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />

    <link href="css/commonSupport.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid" style="min-width: 941px;">
        <!-- BEGIN PAGE HEADER-->
        <div class="row-fluid">
            <ul class="breadcrumb">
                <li>
                    <i class="icon-bar-chart"></i>
                    <a href="data-correlation.aspx">数据关联</a>
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
                <div class="box-content" id="comm1_graph">
                    <div class="form-horizontal">
                        <div class="box">
                            <div class="box-header">
                                <div class="box-icon">
                                    <i class="icon-bar-chart"></i>
                                    <span>关联对象</span>
                                    <a href="#" class="box-collapse pull-right">
                                        <img id="expand_collapse" alt="" src="../resource/img/toggle-collapse.png" />
                                    </a>
                                </div>
                            </div>
                            <%--<div class="box-content" id="dataList">
                                <table>
                                    <tr>
                                        <td style="width: 350px;text-align: left;vertical-align: top;">
                                            <div class="control-group　">
                                                <table>
                                                    <tr>
                                                        <td style="width: 74px;text-align: left;"><b>监测因素:</b></td>
                                                        <td>
                                                            <select id="factorList1" name='factorList1' data-size="10" title="请选择">
                                                            </select>
                                                        </td>
                                                        <td><b>监测点位置:</b></td>
                                                        <td style="display: block;">
                                                            <select id="sensorList1" data-placeholder="请选择" name='sensorList1' class="chzn-select" multiple="multiple" data-size="10" title="请选择">
                                                            </select>
                                                        </td>
                                                        </tr>
                                                    <tr>
                                                        <td style="width: 102px;text-align: left;"><b>关联监测因素:</b></td>
                                                        <td>
                                                            <select id="factorList2" name='factorLis2' class="width100 selectpicker" data-size="10" title="请选择">
                                                            </select>
                                                        </td>
                                                        <td><b>关联监测点位置:</b></td>
                                                        <td style="display: block;">
                                                            <select id="sensorList2" data-placeholder="请选择" name='sensorList2' class="chzn-select " multiple="multiple" data-size="10" title="请选择">
                                                            </select>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            </td>
                                            <td style="width: 50%;vertical-align: top;text-align: left;" >
                                            <div class="control-group" id="factor-correlation">
                                                <table>
                                                    <tr>
                                                         
                                                        </tr>
                                                    <tr>
                                                        
                                                        <td></td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                       </tr>
                                    <tr>
                                        <td style="width: 60%;">
                                            <div class="control-group" id="selectTime">
                                                <table>
                                                    <tr>
                                                        <td><b>时段选择:</b></td>
                                                        <td>
                                                            <div id="dpform1" class="input-append date">
                                                                <input type="text" id="dpform" style="width: 150px;" />
                                                                <span class="add-on" style="height: 20px;">
                                                                    <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                                                </span>

                                                            </div>
                                                        </td>
                                                        <td><b>至</b></td>
                                                        <td>
                                                            <div id="dpdend1" class="input-append date">
                                                                <input type="text" id="dpdend" style="width: 150px;" />
                                                                <span class="add-on" style="height: 20px;">
                                                                    <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                                                </span>

                                                            </div>
                                                        </td>
                                                         <td id="tdBtn">

                                            <input type="button" id="btnQuery" value="查询" class="btn btn-success btndataquery" style="height: 30px;"  /></td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>

                            </div>--%>
                            <div class="box-content" id="dataList">
                            <table>
                                <tr>
                                    <td style="text-align: left;vertical-align: top;">
                                        <div class="control-group　">
                                            <table>
                                                <tr>
                                                    <td style="width: 88px;text-align: left;"><b>监测因素:</b></td>
                                                    <td style="padding-right: 20px;">
                                                        <select id="factorList1" name='factorList1' data-size="10" title="请选择">
                                                        </select>
                                                    </td>
                                                    <td ><b>监测点位置:</b></td>
                                                    <td>
                                                        <select id="sensorList1" data-placeholder="请选择" name='sensorList1' class="chzn-select" multiple="multiple" data-size="10" title="请选择">
                                                        </select>
                                                    </td>
                                                </tr>
                                                <tr id="factor-correlation">
                                                    
                                                     <td><b>关联监测因素:</b></td>
                                                    <td>
                                                        <select id="factorList2" name='factorLis2' class="width100 selectpicker" data-size="10" title="请选择">
                                                        </select>
                                                    </td>
                                                    <td><b>关联监测点位置:</b></td>
                                                    <td>
                                                        <select id="sensorList2" data-placeholder="请选择" name='sensorList2' class="chzn-select " multiple="multiple" data-size="10" title="请选择">
                                                        </select>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                   </tr>
                                <tr>
                                   <td style="text-align: left;" >
                                        <div class="control-group" id="selectTime">
                                            <table>
                                                <tr>
                                                    <td style="width: 88px;text-align: left;"><b>时段选择:</b></td>
                                                    <td>
                                                        <div id="dpform1" class="input-append date">
                                                            <input type="text" id="dpform" style="width: 180px;" />
                                                            <span class="add-on" style="height: 20px;">
                                                                <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                                            </span>

                                                        </div>
                                                    </td>
                                                    <td style="width: 120px;text-align: center;"><b>至</b></td>
                                                    <td>
                                                        <div id="dpdend1" class="input-append date">
                                                            <input type="text" id="dpdend" style="width: 180px;" />
                                                            <span class="add-on" style="height: 20px;">
                                                                <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                                            </span>

                                                        </div>
                                                    </td>
                                                    <td>
                                                        <input type="button" id="btnQuery" value="查询" class="btn btn-success btndataquery" style="height: 30px;"/>

                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            
                        </div>
                        </div>

                        


                        <div>
                            <div class="box" id="comm1" style="height: 360px;">
                            </div>
                            <div id="comm1_error" class="box" style="display: none;">
                            </div>
                            <div id="comm2_error" class="box" style="display: none;">
                            </div>

                        </div>

                    </div>
                   <%-- <div id="tip" style="/*text-align: left;*/"></div>--%>
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
    <script src="../dataCorrelation/double.js"></script>
    
    <script src="../data/js/bootstrap.min.js"></script>
    <script src="../data/js/bootstrap-datetimepicker.js"></script>    
    <script src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>

    <script src="js/data-correlation.js"></script>
    <%--<script>
        jQuery(document).ready(function () {
            // initiate layout and plugins
            App.setPage("other");
            App.init();
        });
    </script>--%>
    <script>
     
    </script>
  
</asp:Content>

