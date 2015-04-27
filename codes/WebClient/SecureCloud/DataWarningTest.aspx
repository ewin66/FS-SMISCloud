<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataWarningTest.aspx.cs" Inherits="SecureCloud.DataWarningTest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--<link href="//cdn.datatables.net/1.10.2/css/jquery.dataTables.min.css" rel="stylesheet" />--%>
    <link href="/resource/library/DataTables-1.10.2/css/jquery.dataTables.min.css" rel="stylesheet" />
    <style>
        .label-red {
            background-color: #ff0000;
            background-image: none !important;
            text-shadow: none !important;
        }

        .label-orange {
            background-color: #ff8000;
            background-image: none !important;
            text-shadow: none !important;
        }

        .label-purple {
            background-color: #A757A8;
            background-image: none !important;
            text-shadow: none !important;
        }

        .label-blue {
            background-color: #0000ff;
            background-image: none !important;
            text-shadow: none !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form_warningView" runat="server">
        <div class="container-fluid">
            <div class="row-fluid">
                <ul class="breadcrumb" style="margin-top:10px;">
                    <li>
                        <i class="icon-home"></i>
                        <a href="/index.aspx">主页</a>
                        <i class="icon-angle-right"></i>
                    </li>
                    <li><a href="javascript:;">结构物</a><i class="icon-angle-right"></i></li>
                    <li>
                        <small class="dropdown" style="display: inline-block;">
                            <a href="javascript:;" class="dropdown-toggle" data-toggle="dropdown"><i class="icon-angle-down"></i></a>
                            <ul class="dropdown-menu">
                            </ul>
                        </small>
                    </li>                  
                </ul>
                <!-- BEGIN EXAMPLE TABLE PORTLET-->
                <div class="portlet box light-grey">
                    <div class="portlet-title">
                        <h4><i class="icon-bell"></i><span id="WarningPageTitle">结构物数据告警</span></h4>
                    </div>
                    <div class="portlet-body">
                        <div class="clearfix">
                            <div class="row-fluid">
                                <div class="form-horizontal">
                                    <div class="span5">
                                        <ul class="nav nav-tabs">
                                            <li><a href="#" id="btnAllAlert">全部</a></li>
                                            <li><a href="#" id="btnProcessedAlert">已确认</a></li>
                                            <li class="active"><a href="#" id="btnUnprocessedAlert">未确认</a></li>
                                        </ul>
                                    </div>
                                    <div class="span4">
                                        <div class="row-fluid alert-tip" style="display: none;">
                                            <!--这里的01不能去掉，span标签里必须有东西，不然IE下没效果-->
                                            <span class="alert-tip-text label" style="margin-top: 5px;">01</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row-fluid">
                                <div class="form-horizontal">
                                    <%--<div style="margin: 10px 15px">
                                        <input id="btnBatchDelete" type="button" class="btn blue" value="批量删除" style="display: none" />
                                    </div>--%>
                                    <div style="margin: 10px 5px;">
                                        <input id="btnBatchConfirm" type="button" class="btn red" value="批量确认" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <%--<hr />--%>
                        <table class="table table-striped table-bordered table-hover" id="sample_1">
                            <thead>
                                <tr>
                                    <th id="th_checkbox"><input type="checkbox" class="group-checkable" data-set="#sample_1 .checkboxes" /></th>
                                    <th>告警源</th>
                                    <th>等级</th>
                                    <th>产生时间</th>
                                    <th>类型编号</th>
                                    <th>可能原因</th>
                                    <th>告警信息</th>
                                    <th>是否确认</th>   
                                    <th>确认信息</th>                               
                                </tr>
                            </thead>
                            <tbody id="tbody">
                            </tbody>
                        </table>
                    </div>
                </div>
                <!-- END EXAMPLE TABLE PORTLET-->
            </div>
        </div>

        <!-- Modal -->
        <div class="modal hide" id="myModal" tabindex="-1" role="dialog">
            <div class="modal-header">
                <button id="close" class="close" type="button" data-dismiss="modal"></button>
                <h5 id="myModalLabel">告警处理信息</h5>
            </div>
            <div class="modal-body">
                <div class="control-group">
                    <label>填写信息</label>
                    <input type="text" name="" id="warningText" size="30" style="width: 80%" maxlength="50" />
                </div>
            </div>
            <div class="modal-footer">
                <input type="button" class="btn red" value="确认提交" id="btnSubmitAlertConfirmationInfo" />
            </div>
        </div>

        <asp:HiddenField ID="HiddenSensorId" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenSensorLocation" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenStartTime" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenEndTime" runat="server" ClientIDMode="Static" />
    </form>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script src="/resource/js/jquery-1.8.3.min.js"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <%--<script src="//cdn.datatables.net/1.10.2/js/jquery.dataTables.min.js"></script>--%>
    <script src="/resource/library/DataTables-1.10.2/js/jquery.dataTables.min.js"></script>
    <script>
        jQuery(document).ready(function () {
            // initiate layout and plugins
            App.setPage("other");
            App.init();
        });
    </script>   
    <script src="/resource/js/DataWarningTest.js"></script>
</asp:Content>
