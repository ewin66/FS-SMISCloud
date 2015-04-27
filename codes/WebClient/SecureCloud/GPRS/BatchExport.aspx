<%@ Page Title="" Language="C#" MasterPageFile="~/GPRS/SiteGPRS.Master" AutoEventWireup="true" CodeBehind="BatchExport.aspx.cs" Inherits="SecureCloud.GPRS.BatchExport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
    <%--<link href="../resource/library/bootstrap/css/datepicker.css" rel="stylesheet" />--%>
    <link href="../resource/css/unionboostrap.css" rel="stylesheet" />
    
     <link href="../data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />
    
    <link href="../resource/library/data-tables/DT_bootstrap.css" rel="stylesheet" />
    <link href="../resource/library/data-tables/css/dataTable.css" rel="stylesheet" />
    <link href="../resource/library/tableTools/css/TableTools.css" rel="stylesheet" />
    
    <%--<link href="/resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />--%>  <%--可搜索的select标签--%>
    
    <style>
        .width150 {
            width: 150px;
        }
        .width200 {
            width: 200px; 
        }     
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="formBatchExport" runat="server">
       <div class="container-fluid">
           <div class="box">
                <div class="box-content">
                    <div class="row-fluid">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <b>DTU编号-模块号:</b>
                                <select id="dtu-module" name='dtuModule' class="width200 selectpicker" data-size="10" title="请选择">
                                </select>
                                <select id="date" name="date" class="width150">
                                    <option value="day" class="btn-date">最近一天</option>
                                    <option value="week" class="btn-date">最近一周</option>
                                    <option value="month" class="btn-date">最近一月</option>                                   
                                     <option value="other" class="btn-minimize-other">其他</option>
                                </select>
                                <input type="button" id="btnQuery" value="查询" class="btn btn-success btndataquery" />
                            </div>
                            <div class="other-search" style="display: none;">
                                <div class="control-group">
                                    <label class="control-label">
                                        <b>监测时间从:</b></label>
                                    <div class="controls">
                                        <div id="dpform1" class="input-append date">
                                            <input type="text" id="dpform"/>
                                            <span class="add-on" style="height:20px;">
                                                <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                            </span>
                                        </div>
                                        <b>至</b>
                                        <div id="dpdend1" class="input-append date">
                                            <input type="text" id="dpdend"/>
                                            <span class="add-on" style="height:20px;">
                                                <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div> 

                            <div id="tipNoSensor" class="span2"></div>

                            <%-- 无数据提示区域 --%>
                            <div id="tipNoData-sensor" class="hide">
                                <span class="text-15 label-important">此测点该时间段内无数据</span>
                            </div>
                            <%-- 表格区域 --%>
                            <div class="box-content data-table-content" style="display: none;">
                                <div id="table-sensor" style="margin-bottom: 40px;"></div>
                            </div>                                          
                        </div> <%-- /div.form-horizontal --%>
                    </div>
                </div>
           </div> 
        </div>
    </form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script type="text/javascript" src="../resource/js/jquery-2.0.3.min.js"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script type="text/javascript" src="/resource/library/data-tables/DT_bootstrap.js"></script>
    <script type="text/javascript" src="../resource/library/bootstrap/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="../resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script type="text/javascript" src="../resource/library/bootstrap/js/bootstrap-dropdown.js"></script>
    
    <script src="../data/js/bootstrap.min.js"></script>
    <script src="../data/js/bootstrap-datetimepicker.js"></script> 
    <script src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>
    <script type="text/javascript" src="../resource/js/datepickerInit.js"></script>
    
    <script type="text/javascript" src="../resource/library/data-tables/js/jquery.dataTables.js"></script>
    <script type="text/javascript" src="../resource/library/data-tables/DT_bootstrap.js"></script>
    <script type="text/javascript" src="../resource/library/tableTools/js/ZeroClipboard.js"></script>
    <script type="text/javascript" src="../resource/library/tableTools/js/TableTools.min.js"></script>
    
    <script type="text/javascript" src="../resource/js/dataTableInit.js"></script>

    <script type="text/javascript" src="js/batchExport.js"></script>
</asp:Content>
