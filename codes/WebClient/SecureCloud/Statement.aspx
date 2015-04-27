<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Statement.aspx.cs" Inherits="SecureCloud.Statement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <div class="row-fluid">

            <div class="span12">
                <h3 class="page-title"><span>安心云—</span>

                    <small class="dropdown" style="display: inline-block;">
                        <a href="javascript:;" class="dropdown-toggle" data-toggle="dropdown"><i class="icon-angle-down"></i></> 
                        </a>
                        <ul class="dropdown-menu">
                            <%--<li><a href="#">K775边坡</a></li>
                            <li class="divider"></li>
                            <li><a href="#">K791边坡</a></li>--%>
                        </ul>
                    </small>
                </h3>
            </div>

            <!-- 报表中心 -->
            <div class="portlet box light-grey">
                <div class="portlet-title">
                    <h4><i class="icon-bar-chart"></i>工作报表-</h4><h4 id="titleH"> </h4>
                </div>
                <div class="portlet-body">
                    <div class="clearfix">
                        <div class="row-fluid">
                            <div class="form-horizontal">
                                <div id="statement">
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- END 报表中心 -->
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script type="text/javascript" src="/resource/js/jquery-2.0.3.min.js"></script>
</asp:Content>


<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script>
        jQuery(document).ready(function () {
            App.setPage("other");  // set current page
            App.init(); // init the rest of plugins and element            
        });
    </script>
    <script src="resource/js/statement.js"></script>
</asp:Content>
