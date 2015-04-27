<%@ Page Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="SupportLog.aspx.cs" Inherits="SecureCloud.Support.SupportLog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="//cdn.datatables.net/1.10.2/css/jquery.dataTables.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="SupportLog" runat="server">
        <div class="container-fluid">
            <div class="row-fluid">
                <div class="portlet box light-grey">
                    <div class="portlet-title"  style="height: 37px">
                        <h4><i class="icon-book"></i>用户日志</h4>
                    </div>
                    <div class="portlet-body">
                        <div class="btn-small btn-group">
                            <input type="button" id="btnDownload" class="btn green" value="导出到Excel" />
                        </div>
                        <table class="table table-striped table-bordered table-hover" id="SupportLogTable">
                            <thead>
                                <tr>
                                    <th>操作时间</th>
                                    <th>设备类型</th>
                                    <th>操作内容</th>
                                    <th>操作参数</th>
                                </tr>
                            </thead>
                            <tbody id="SupportLogTbody">
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script src="/resource/js/jquery-1.8.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script src="//cdn.datatables.net/1.10.2/js/jquery.dataTables.min.js"></script>
    <script src="/resource/library/bootstrap/js/bootstrap-tooltip.js"></script>
    <script src="js/SuportLog.js"></script>
</asp:Content>
