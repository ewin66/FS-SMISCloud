<%@ Page Title="" Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="SectionHeapMapConfig.aspx.cs" Inherits="SecureCloud.Support.SectionHeapMapConfig" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row-fluid">
        <div class="box">
            <div class="box-header">
                <div class="box-icon">
                    <h3 id="titleSectionHeapMapSvg" class="page-title"></h3>
                </div>
            </div>
            <div class="box-content">
                <div class="row-fluid">
                    <div class="form-horizontal">
                        <div class="span9">
                            <div class="row-fluid" id="imgContain" style="position: relative;">
                                <ul id="theDropDownMenu" class="dropdown-menu" role="menu" style="position:absolute;">
                                  <li role="presentation"><a role="menuitem" tabindex="1" href="#"></a></li>
                                  <li role="presentation"><a role="menuitem" tabindex="2" href="#"></a></li>
                                </ul>
                                <img id="img" src="\" ondrop="drop(event)" alt="此处为截面热点图" ondragover="allowDrop(event)" style="width: 100%; height: 100%;" />
                            </div>
                        </div>
                        <div class="span3">
                            <div class="box" style="margin-top: 20%;">
                                <div class="box-header">
                                    <h4 style="text-align: center">图例</h4>
                                </div>
                                <div class="box-content" style="overflow: auto; height: 400px">
                                    <table id="tableLegend" class="table table-striped table-bordered table-hover">
                                        <thead>
                                            <tr>
                                                <th>图片</th>
                                                <th>传感器名称</th>
                                                <th>位置描述</th>
                                            </tr>
                                        </thead>
                                        <tbody id="legend"></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <input type="button" value="返回" class="btn btn-primary blue" style="float: right; margin-right: 3%;" onclick="backSectionConfig();" />
    </div>
    <!-- 删除热点 config modal -->
    <div id="modalDeleteHotspot" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>删除热点</h3>
        </div>
        <div class="modal-body">
            <p id="deletionMsg"></p>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">否</button>
            <a href="javascript:;" id="btnDeleteConfirm" class="btn red">是</a>
        </div>
    </div>
    <!-- end 删除热点 config modal -->
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script type="text/javascript" src="/resource/js/jquery-2.0.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script type="text/javascript" src="/resource/library/data-tables/js/jquery.dataTables.js"></script>
    <script type="text/javascript" src="/resource/library/data-tables/DT_bootstrap.js"></script>
    <script type="text/javascript" src="js/sectionHeapMapConfig.js"></script>
</asp:Content>
