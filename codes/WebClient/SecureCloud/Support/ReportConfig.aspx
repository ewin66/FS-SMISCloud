<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Support/SiteSupport.Master" CodeBehind="ReportConfig.aspx.cs" Inherits="SecureCloud.Support.ReportConfig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--<link href="../resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />--%>
    <link href="../resource/library/data-tables/DT_bootstrap.css" rel="stylesheet" />
    <link href="../resource/css/unionboostrap.css" rel="stylesheet" />
    <link href="../resource/css/windowbox.css" rel="stylesheet" />
    <%--<link href="//cdn.datatables.net/1.10.2/css/jquery.dataTables.min.css" rel="stylesheet" />--%>
    <link href="../resource/library/DataTables-1.10.2/css/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="../resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />
    <style>
        .reportConfig-modal {
            position: fixed;
            top: 10%;
            left: 50%;
            z-index: 1050;
            width: 560px;
            margin-left: -410px;
            background-color: #fff;
            border: 1px solid #999;
            border: 1px solid rgba(0,0,0,0.3);
            -webkit-border-radius: 6px;
            -moz-border-radius: 6px;
            border-radius: 6px;
            outline: 0;
            -webkit-box-shadow: 0 3px 7px rgba(0,0,0,0.3);
            -moz-box-shadow: 0 3px 7px rgba(0,0,0,0.3);
            box-shadow: 0 3px 7px rgba(0,0,0,0.3);
            -webkit-background-clip: padding-box;
            -moz-background-clip: padding-box;
            background-clip: padding-box;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">

            <div class="portlet box light-grey">

                <div class="portlet-title">
                    <h4>
                        <img src="/resource/img/settings.png" style="width: 30px; height: 30px" />
                        报表配置</h4>
                </div>

                <div class="portlet-body" style="width: auto">
                    <div class="clearfix">
                        <div class="row-fluid">
                            <div class="form-horizontal">
                                <div class="span1" style="margin: 0px 15px 10px 0px">
                                    <input id="btnadd" type="button" class="btn blue editor_add" value="增加报表配置" href='#addRptConfigModal' data-toggle='modal' />
                                </div>
                                <%-- <div class="span4">
                    <div class="row-fluid alert-tip" style="display: none;">
                        <!--这里的01不能去掉，span标签里必须有东西，不然IE下没效果-->
                        <span class="alert-tip-text label" style="margin-top: 5px;">01</span>
                    </div>
                </div>--%>
                            </div>
                        </div>
                    </div>
                    <table id="RptConfigTable" class="table table-striped table-bordered" style="width: 100%;">
                        <thead>
                            <tr>
                                <th style="width: 10%">组织</th>
                                <th style="width: 10%">结构物</th>
                                <th style="width: 10%">报表名称</th>
                                <th style="width: 10%">类型</th>
                                <th style="width: 10%">生成周期</th>
                                <th style="width: 10%">报表模板</th>
                                <th style="width: 10%">日数据采样时间点</th>
                                <th style="width: 10%">需要确认</th>
                                <th style="width: 10%">启用状态</th>
                                <th style="width: 10%;">操作</th>
                                <%--</tr>--%>
                        </thead>
                        <tbody id="RptConfigTbody"></tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>

    <%--<!-- 添加报表配置 -->--%>
    <div id="addRptConfigModal" class="reportConfig-modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="width: 800px">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3 id="addRptConfigModalTitle">增加报表配置</h3>
        </div>
        <div class="modal-body" style="max-height: 800px; overflow-y: hidden;">
            <div class="form-horizontal">
                <fieldset id="add_fieldset">
                    <div id="OrgDiv" class="control-group">
                        <label class="control-label">组织</label>
                        <div class="controls">
                            <select id="Org" name="Org" class="width100 " style="width: 500px;" data-size="10" title="请选择">
                            </select>
                        </div>
                    </div>
                    <div id="StructDiv" class="control-group">
                        <label class="control-label">结构物</label>
                        <div class="controls">
                            <select id="Struct" name="Struct" class="width100 " style="width: 500px;" data-size="10" title="请选择">
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label" for="ReportName_add">报表名称</label>
                        <div class="controls">
                            <input type="text" id="ReportName_add" style="width: 488px;" placeholder="报表名称最多50字" />
                        </div>
                    </div>
                </fieldset>

                <fieldset id="edit_fieldset">
                    <div id="org_edit_div" class="control-group">
                        <label class="control-label" for="org_edit">组织</label>
                        <div class="controls">
                            <input type="text" id="org_edit" placeholder="组织" readonly="readonly" class="width100 " style="width: 488px;" />
                        </div>
                    </div>
                    <div id="struct_edit_div" class="control-group">
                        <label class="control-label" for="struct_edit">结构物</label>
                        <div class="controls">
                            <input type="text" id="struct_edit" placeholder="结构物" readonly="readonly" class="width100 " style="width: 488px;" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label" for="ReportName_edit">报表名称</label>
                        <div class="controls">
                            <input type="text" id="ReportName_edit" placeholder="报表名称" readonly="readonly" class="width100 " style="width: 488px;" />
                        </div>
                    </div>
                </fieldset>

                <div id="RptDateTypeDiv" class="control-group">
                    <label class="control-label">类型</label>
                    <div class="controls">
                        <select class="width100 chzn-select" style="width: 488px;" data-size="10" data-placeholder="请选择" id="DateType_add">
                            <option value="1">日报表</option>
                            <option value="2">周报表</option>
                            <option value="3">月报表</option>
                            <option value="4">年报表</option>
                        </select>
                    </div>
                </div>
                <div class="control-group" id="dataDate_add_Div" style="display: block">
                    <label class="control-label">日数据采样时间点</label>
                    <div class="controls">
                        <select class="width100  chzn-select" style="width: 488px;" data-size="10" data-placeholder="请选择" id="dataDate_add">
                        </select>
                    </div>
                </div>
                <div id="Template_add_Div" class="control-group">
                    <label class="control-label">报表模板</label>
                    <div class="controls">
                        <select id="Template_add" class="width100 chzn-select" multiple="multiple" style="width: 488px;" data-size="10" data-placeholder="请选择">
                        </select>
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">需要二次确认</label>
                    <div class="controls">
                        <select class="width100  chzn-select" style="width: 488px;" data-size="10" data-placeholder="请选择" id="confirm_add">
                            <option value="0">否</option>
                            <option value="1">是</option>
                        </select>
                    </div>
                </div>

                <div class="control-group" id="IsEnabled_edit_Div">
                    <label class="control-label">启用状态</label>
                    <div class="controls">
                        <select id="IsEnabled_edit" class="width100  chzn-select" style="width: 488px;" data-placeholder="请选择" data-size="10">
                            <option value="1">启用</option>
                            <option value="0">禁用</option>

                        </select>
                    </div>
                </div>


                <div class="control-group">
                    <label class="control-label">生成周期</label>
                    <div class="controls">
                        <input id="createInterval_add" style="width: 488px;" type="text" placeholder="定时生成报表的周期" />
                    </div>
                </div>
                <div class="control-group">
                    <div class=" controls alert alert-warn" style="width: 454px;">
                        <b>生成周期填写格式：</b>
                        <br />
                        1.格式：[秒] [分] [小时] [月内日期] [月] [周内日期] [年(可选)]；
                        <br />
                        2.说明：各个域之间以空格分隔开,在英文状态下输入，详细说明见配置手册；
                        <br />
                        3.示例：每天6:00AM执行一次 (0 0 6 * * ?)。
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <button id="btnCancel_add" class="btn" data-dismiss="modal">关闭</button>
            <input id="btnReset_add" type="button" value="重置" class="btn" />
            <input id="btnSave_add" type="button" value="保存" class="btn btn-primary" />
        </div>
    </div>


    <%--删除报表配置--%>
    <div id="deleteRptConfigModal" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>删除报表配置</h3>
        </div>
        <div class="modal-body">
            <p id="alertMsg_delete"></p>
        </div>
        <div class="modal-footer">
            <button id="btnCancel" class="btn" data-dismiss="modal">取消</button>
            <a href="#" id="btnDelete" class="btn red">确定</a>
        </div>
    </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script src="../resource/js/jquery-1.8.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <%--<script src="../resource/library/bootstrap/js/bootstrap-select.js"></script>--%>
    <script src="../resource/library/data-tables/js/jquery.dataTables.js"></script>
    <script src="../resource/library/data-tables/DT_bootstrap.js"></script>
    <script src="../resource/library/chosen-bootstrap/chosen/chosen.jquery.js"></script>
    <%--这个路径下的ajaxfileupload.js源代码经过修改--%>
    <script src="/Support/lib/ajaxfileupload.js"></script>
    <script src="/Support/js/ReportConfig.js"></script>
</asp:Content>
