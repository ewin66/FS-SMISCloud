<%@ Page Language="C#" MasterPageFile="~/orgin.Master" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="SecureCloud.Report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="//cdn.datatables.net/1.10.2/css/jquery.dataTables.min.css" rel="stylesheet" />
    <%--<link href="resource/library/data-tables/css/jquery.dataTables.css" rel="stylesheet" />--%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="portlet box light-grey">
                <div class="portlet-title">
                    <h4><i class="icon-table"></i>报表管理</h4>
                </div>
                <div class="portlet-body">
                    <div id="statement" class="tabbable tabbable-custom">
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="#tab_day" data-toggle="tab" style="font-size: 16px">日报表</a></li>
                            <li><a href="#tab_week" data-toggle="tab" style="font-size: 16px">周报表</a></li>
                            <li><a href="#tab_month" data-toggle="tab" style="font-size: 16px">月报表</a></li>
                            <li><a href="#tab_year" data-toggle="tab" style="font-size: 16px">年报表</a></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane row-fluid  active" id="tab_day">
                                <table class="table table-striped table-bordered table-hover" id="table_day">
                                    <thead>
                                        <tr>
                                            <th>报表名称</th>
                                            <th>生成时间</th>
                                            <th>操作</th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbody_day">
                                    </tbody>
                                </table>
                            </div>
                            <div class="tab-pane row-fluid" id="tab_week">
                                 <table class="table table-striped table-bordered table-hover" id="table_week">
                                    <thead>
                                        <tr>
                                            <th>报表名称</th>
                                            <th>生成时间</th>
                                            <th>操作</th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbody_week">
                                    </tbody>
                                </table>
                            </div>
                            <div class="tab-pane row-fluid " id="tab_month">
                                 <table class="table table-striped table-bordered table-hover" id="table_month">
                                    <thead>
                                        <tr>
                                           <th>报表名称</th>
                                            <th>生成时间</th>
                                            <th>操作</th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbody_month">
                                    </tbody>
                                </table>
                            </div>
                            <div class="tab-pane row-fluid" id="tab_year">
                                <table class="table table-striped table-bordered table-hover" id="table_year">
                                    <thead>
                                        <tr>
                                            <th>报表名称</th>
                                            <th>生成时间</th>
                                            <th>操作</th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbody_year">
                                    </tbody>
                                </table>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script src="resource/js/jquery-1.8.3.min.js"></script>
    <%--<script src="/resource/library/data-tables/js/jquery.dataTables.min.js"></script>--%>
    <script src="//cdn.datatables.net/1.10.2/js/jquery.dataTables.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script>
        jQuery(document).ready(function () {
            // initiate layout and plugins
            App.setPage("other");
            App.init();
        });
    </script>
    <script src="/resource/js/Report.js"></script>
</asp:Content>
