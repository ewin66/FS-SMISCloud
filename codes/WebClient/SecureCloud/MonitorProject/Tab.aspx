<%@ Page Language="C#"  MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Tab.aspx.cs" Inherits="SecureCloud.MonitorProject.Tab" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../resource/library/bootstrap/css/datepicker.css" rel="stylesheet" />
    <link href="../resource/css/unionboostrap.css" rel="stylesheet" />
    <link href="../resource/css/windowbox.css" rel="stylesheet" />
    <script src="../resource/js/jquery-1.8.3.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="Tab" runat="server">
        <div class="container-fluid">
            <%--<div class="box">--%>
            <ul class="breadcrumb" style="margin-top: 10px;">
                <li>
                    <i class="icon-home"></i>
                    <a href="/index.aspx">主页</a>
                    <i class="icon-angle-right"></i>
                </li>
                <li> <span >结构物</span><i class="icon-angle-right"></i></li>
                <li>
                    <small class="dropdown" style="display: inline-block;">
                        <span>
                        </span>
                        <%--<ul class="dropdown-menu">
                        </ul>--%>
                    </small>
                </li>
            </ul>
            <div id="tabtip" style="text-align: center"></div>
            <div class="box-content">
                
                <div class="tabbable tabbable-custom boxless">
                    <ul id="tab" class="nav nav-tabs" style='font-size: 14pt'>
                    </ul>
                    <div id="tabContent" class="tab-content">
                    </div>
                </div>

            </div>
        </div>
    <%--</div>--%>
 <asp:HiddenField ID="HiddenThemeNo" runat="server" ClientIDMode="Static" />
 <asp:HiddenField ID="HiddenFactorId" runat="server" ClientIDMode="Static" />
 <asp:HiddenField ID="HiddenSensorId" runat="server" ClientIDMode="Static" />
</form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footer" runat="server">
    <script>
        jQuery(document).ready(function () {
            App.init(); // init the rest of plugins and elements
        });
    </script>

    <script src="js/Tab.js"></script>

</asp:Content>