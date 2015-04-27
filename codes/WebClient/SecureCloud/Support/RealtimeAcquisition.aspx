<%@ Page Title="" Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="RealtimeAcquisition.aspx.cs" Inherits="SecureCloud.Support.RealtimeAcquisition" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../resource/library/data-tables/DT_bootstrap.css" rel="stylesheet" />
    <link href="/resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />

    <link href="css/commonSupport.css" rel="stylesheet" />

    <style>
        .width200 {
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
                    <a href="RealtimeAcquisition.aspx">即时采集</a>
                    <i class="icon-angle-right"></i>
                </li>
                <li><a href="javascript:;">组织</a><i class="icon-angle-right"></i></li>
                <li>
                    <select id="listOrg" class="chzn-select dropdown-menu" style="width: 300px;" data-size="10" data-placeholder="请选择">
                    </select>
                </li>
                <li><i class="icon-angle-right"></i><a href="javascript:;">结构物</a><i class="icon-angle-right"></i></li>
                <li>
                    <select id="listStruct" class="chzn-select dropdown-menu" style="width: 300px;" data-size="10" data-placeholder="请选择">
                    </select>
                </li>
            </ul>
        </div>
        <!-- END PAGE HEADER -->

        <!-- BEGIN PAGE BODY-->
        <div class="row-fluid">
            <div class="portlet box light-grey">
                <div class="portlet-body" style="width:auto;">
                    <div class="clearfix">
                        <div class="row-fluid">
                            <div id="loading-realtime" class="fixed-marker-loading"></div>
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <b class="control-label" style="width: 100px;">归属DTU编号:</b>
                                    <div class="controls" style="margin-left: 110px;">
                                        <select id="dtu-realtime" name='dtuRealtime' class="width200 selectpicker" data-size="10" title="请选择">
                                        </select>
                                    </div>
                                </div>                               
                                <div style="margin: 10px 15px;">
                                    <input id="btnBatchSend" type="button" class="btn blue" value="批量下发" />
                                    <span style="color:red;">(注意: 一次最多操作5个传感器)</span>
                                </div>
                                <div id="prompt-dtuTransfered" class="label-red" style="margin: 0 15px; width: auto; display: none;">
                                    该DTU为传输型DTU, 因此不能对该DTU下任何传感器进行"即时采集"
                                </div>
                            </div>
                        </div>
                        <div id="tip-realtime" class="fixed-prompt"></div>
                    </div>
                    <table id="realtimeAcqusition-table" class="table table-striped table-bordered table-hover">
                        <thead>
                            <tr>
                                <th style="width: 15px;">
                                    <input type="checkbox" class="group-checkable" data-set="#realtimeAcqusition-table .checkboxes" />
                                </th>
                                <th style="width: 15%">传感器</th>
                                <th style="width: 10%">模块号</th>
                                <th style="width: 10%">通道号</th>
                                <th style="width: 10%">下发即时采集</th>
                                <th>即时采集结果</th>
                            </tr>
                        </thead>
                        <tbody id="realtimeAcqusition-tbody">
                        </tbody>                    
                    </table>
                </div>
            </div>
        </div>
        <!-- END PAGE BODY -->
    </div>

    <!-- 传感器即时采集"下发"确认框 modal -->
    <div id="modalConfirmRealtime" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>即时采集</h3>
        </div>
        <div class="modal-body">
            <p style="color: #ff0000">确认下发?</p>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">否</button>
            <a href="javascript:;" id="btnConfirmRealtimeSend" class="btn red">是</a>
        </div>
    </div>
    <!-- end 传感器即时采集"下发"确认框 modal -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <%--<script type="text/javascript" src="../resource/js/jquery-2.0.3.min.js"></script>--%>
    <script type="text/javascript" src="../resource/js/jquery-1.8.3.min.js"></script> <%-- 支持chosen --%>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script type="text/javascript" src="../resource/library/bootstrap/js/bootstrap-dropdown.js"></script>
        
    <script type="text/javascript" src="../resource/library/data-tables/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="../resource/library/data-tables/DT_bootstrap.js"></script>
    
    <script src="/resource/library/chosen-bootstrap/chosen/chosen.jquery.js"></script>

    <script type="text/javascript" src="js/realtimeAcquisition.js"></script>
</asp:Content>

