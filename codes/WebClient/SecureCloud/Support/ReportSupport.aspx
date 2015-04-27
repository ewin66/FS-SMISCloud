<%@ Page Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="ReportSupport.aspx.cs" Inherits="SecureCloud.Support.ReportSupport" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../resource/library/data-tables/DT_bootstrap.css" rel="stylesheet" />
    <link href="../resource/css/unionboostrap.css" rel="stylesheet" />
    <link href="../resource/css/windowbox.css" rel="stylesheet" />
    <%--<link href="//cdn.datatables.net/1.10.2/css/jquery.dataTables.min.css" rel="stylesheet" />--%>
    <link href="../resource/library/DataTables-1.10.2/css/jquery.dataTables.min.css" rel="stylesheet" />
    <%--时间控件--%>
    <link href="../data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />
    <%--下拉列表--%>
    <link href="../resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />
    <style>
        .width300 {
            width: 350px;
        }

        .report-control-label {
            float: left;
            width: 100px;
            padding-top: 5px;
            text-align: right;
        }

        .report-controls {
            margin-left: 120px;
        }

        .report-modal {
            position: fixed;
            top: 10%;
            left: 50%;
            z-index: 1050;
            width: 560px;
            margin-left: -450px;
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
            vertical-align: middle;
        }

        .chzn-container .chzn-results {
            margin: 0 4px 4px 0;
            height: 130px;
            padding: 0 0 0 4px;
            position: relative;
            overflow-x: hidden;
            overflow-y: auto;
            -webkit-overflow-scrolling: touch;
            font-weight: normal   !important;
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
                        报表管理</h4>
                </div>
                <div class="portlet-body" style="width: auto">
                    <div class="clearfix">
                        <div class="row-fluid">
                            <div class="form-horizontal">
                                <div style="margin: 0 15px 10px 0; float: left">
                                    <input id="btnMultidown" type="button" class="btn blue" value="批量下载" />
                                </div>
                                <div style="margin: 0 15px 10px 0; float: left">
                                    <a id="btnMultiupload" href="#" type="button" class="btn green">批量上传</a>
                                </div>
                                <div id="MultiDel_Div" style="margin: 0 15px 10px 0; float: left">
                                    <a id="btnMultiDel" href="#" type="button" class="btn ">批量删除</a>
                                </div>
                                <div id="ManualUpload_Div" style="margin: 0 15px 10px 0; float: left">
                                    <a id="btnManualupload" href="#" type="button" class="btn purple">人工上传</a>
                                </div>
                                <div id="alertTip" style="margin: 10px 15px; float: left">
                                </div>
                            </div>
                        </div>
                    </div>
                    <table id="ReportTable" class="table table-striped table-bordered table-hover">
                        <thead>
                            <tr>
                                <th style="width: 3%; min-width: 40px">
                                    <input type="checkbox" class="group-checkable" data-set="#ReportTable .checkboxes" /></th>
                                <th style="width: 15%">报表名称</th>
                                <th style="width: 15%">组织名称</th>
                                <th style="width: 15%">结构物名称</th>
                                <th style="width: 7%">类型</th>
                                <th style="width: 8%">生成时间</th>
                                <th style="width: 7%" align="center">状态</th>
                                <th style="width: 33%" align="center">操作</th>
                            </tr>
                        </thead>
                        <tbody id="ReportTbody">
                        </tbody>
                    </table>
                </div>

            </div>
        </div>
    </div>

    <!-- 批量上传完整报表 -->
    <div id="MultiuploadModel" class="modal hide fade" tabindex="-1" role="dialog">
        <div class="modal-header">
            <button type="button" id="Button1" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>批量上传完整报表</h3>

        </div>
        <div class="modal-body" style="max-height: 500px; max-width: 500px;">
            <div class="form-horizontal">

                <div class="control-group" id="MutiInputfile">
                </div>
                <div class="control-group" align="left">
                    <img id="multi_loading" src="images/loading.gif" style="display: none" />
                </div>
            </div>

        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">取消</button>
            <input id="btn_upload_multi" type="button" value="上传" class="btn btn-primary" />
        </div>
    </div>


    <!-- 上传完整报表 -->
    <div id="UploadReportModal" class="modal hide fade" tabindex="-1" role="dialog">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3 id="UploadReportModalTitle">上传完整报表</h3>

        </div>
        <div class="modal-body" style="max-height: 500px; max-width: 500px;">
            <div class="form-horizontal">

                <div class="control-group">
                    <input type="file" id="ReportFile" name="file" size="60" value="浏览" />
                </div>
                <div class="control-group">
                    <img id="loading" src="images/loading.gif" style="display: none" />
                </div>

            </div>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">取消</button>
            <input id="btn_upload" type="button" value="上传" class="btn btn-primary" />
        </div>
    </div>

    <%--人工上传报表重命名--%>
    <div id="renameRptModal" class="modal  hide fade" tabindex="-1" role="dialog" aria-hidden="true" style="max-width: 600px;">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>人工上传报表重命名</h3>
        </div>
        <div class="modal-body" style="max-height: 600px; max-width: 600px;">
            <div class="form-horizontal">

                <div class="control-group">
                    <label class="report-control-label" for="rename_org">组织</label>
                    <div class="report-controls">
                        <input type="text" id="rename_org" readonly="readonly" class="width300 " />
                    </div>
                </div>
                <div class="control-group">
                    <label class="report-control-label" for="rename_struct">结构物</label>
                    <div class="report-controls">
                        <input type="text" id="rename_struct" readonly="readonly" class="width300 " />
                    </div>
                </div>
                <div class="control-group">
                    <label class="report-control-label" for="rename_rptType">报表类型</label>
                    <div class="report-controls">
                        <input type="text" id="rename_rptType" readonly="readonly" class="width300 " />
                    </div>
                </div>
                <div class="control-group">
                    <label class="report-control-label" for="rename_date">生成日期</label>
                    <div class="report-controls">
                        <input type="text" id="rename_date" readonly="readonly" class="width300 " />
                    </div>
                </div>
               <%-- <div class="control-group">
                    <label class="report-control-label" for="rename_original">上传文件名</label>
                    <div class="report-controls">
                        <input type="text" id="rename_original" readonly="readonly" class="width300 " />
                    </div>
                </div>--%>
                <div class="control-group">
                    <label class="report-control-label" for="rename_old">报表旧名称</label>
                    <div class="report-controls">
                        <input type="text" id="rename_old" readonly="readonly" class="width300 " />
                    </div>
                </div>
                <div class="control-group">
                    <label class="report-control-label" for="rename_new">报表新名称</label>
                    <div class="report-controls">
                        <input type="text" id="rename_new" placeholder="重命名的报表名称" maxlength="40" class="width300 " />
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <button id="rename_cancel" class="btn" data-dismiss="modal">取消</button>
            <input id="rename_reset" type="button" value="重置" class="btn" />
            <input id="rename_sumit" type="button" value="确定" class="btn btn-primary" />
        </div>
    </div>

    <%--人工上传报表--%>
    <div id="ManualUploadModal" class="report-modal  hide fade " tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="width: 900px;">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3 id="ManualUploadModalTile">人工上传报表</h3>
        </div>
        <div class="modal-body" style="min-width: 400px; max-height: 600px; overflow-y: hidden;">
            <div class="box" style="margin-top: 0; min-height: 525px;">
                <div class="box-header">
                    <div class="row-fluid">
                        <div class="form-horizontal">
                            <div style="margin: 10px 10px 10px 10px; float: left">
                                <span style="margin-right: 10px;">组织:</span>
                                <select id="OrgList" name="OrgList" class="width300 chzn-select " style="width: 300px;" data-size="10" data-placeholder="请选择">
                                </select>
                            </div>
                            <div style="margin: 10px 10px 10px 10px; float: left">
                                <span style="margin-left: 20px; margin-right: 10px;">结构物:</span>
                                <select id="StructList" name="StructList" class="width300 chzn-select" style="width: 300px;" data-size="10" data-placeholder="请选择">
                                </select>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="box-content">
                    <div id="StartRow_0" class="row-fluid">
                        <div class="form-horizontal">
                            <div style="margin: 10px 10px 10px 10px; float: left">

                                <select id="FactorList_0" name="FactorList" class="width300  chzn-select" style="width: 150px;" data-size="10" data-placeholder="请选择监测因素">
                                </select>
                            </div>
                            <div style="margin: 10px 10px 10px 10px; float: left">

                                <select id="DateType_0" name="DateType" class="width300 chzn-select " style="width: 150px;" data-size="10" data-placeholder="请选择报表类型">
                                </select>
                            </div>
                            <div style="margin: 10px 10px 10px 10px; float: left" class=" input-append date">
                                <input id="RptDate_0" class="ui_timepicker " style="width: 120px;" type="text" placeholder="报表生成日期" />
                                <span class="add-on" style="height: 20px;">
                                    <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                </span>
                            </div>
                            <div style="margin: 10px 10px 10px 10px; float: left; width: 230px;">
                                <input style="width: 100%" type="file" id="RptFile_0" name="file" size="40" value="浏览" />
                            </div>
                            <div style="margin: 10px 10px 10px 10px; float: right;">
                                <img id="expand_collapse_0" alt="" src="/resource/img/toggle-expand.png" style="width: 30px; height: 30px;" onclick="AddRpt()" />
                            </div>
                        </div>
                    </div>
                    <div id="clearfix" class="clearfix"></div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <button id="manual_upload_cancel" class="btn" data-dismiss="modal">取消</button>
            <input id="manual_upload_reset" type="button" value="重置" class="btn" />
            <input id="btn_manual_upload" type="button" value="确定" class="btn btn-primary" />
        </div>
    </div>
    <!-- 批量删除完整报表 -->
    <div id="MultideleteModel" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>删除完整报表</h3>
        </div>
        <div class="modal-body">
            <p id="alertMsg_MultiDel"></p>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">取消</button>
            <input id="btn_delete_multi" type="button" value="确定" class="btn btn-primary" />
        </div>
    </div>
    <!-- 删除完整报表 -->
    <div id="deleteRptModal" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>删除完整报表</h3>
        </div>
        <div class="modal-body">
            <p id="alertMsg_delete"></p>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">取消</button>
            <input id="btn_delete" type="button" value="确定" class="btn btn-primary" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script src="../resource/js/jquery-1.8.3.min.js"></script>
    <%--<script src="../resource/js/jquery-2.0.3.min.js"></script>--%>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <%-- <script src="//cdn.datatables.net/1.10.2/js/jquery.dataTables.min.js"></script>--%>
    <script src="../resource/library/DataTables-1.10.2/js/jquery.dataTables.min.js"></script>
    <script src="../resource/library/data-tables/DT_bootstrap.js"></script>
    <%--日期控件--%>
    <%--   <script src="../data/js/bootstrap.min.js"></script>--%>
    <script src="../data/js/bootstrap-datetimepicker.js"></script>
    <script src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>
    <%--下拉列表--%>
    <script src="../resource/library/chosen-bootstrap/chosen/chosen.jquery.js"></script>

    <%--这个路径下的ajaxfileupload.js源代码经过修改--%>
    <script src="/Support/lib/ajaxfileupload.js"></script>
    <script src="/Support/js/ReportSupport.js"></script>


</asp:Content>


