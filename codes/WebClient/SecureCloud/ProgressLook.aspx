<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProgressLook.aspx.cs" Inherits="SecureCloud.ProgressLook" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="//cdn.datatables.net/1.10.2/css/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="/resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
    <link href="data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />
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
    <form id="DataWarningTest" runat="server">
        <div class="container-fluid">
            <div class="row-fluid">
                <!-- BEGIN 进度设置-->
                <div class="portlet box light-grey">
                    <div class="portlet-title">
                        <h4><i class="icon-star"></i><span>施工进度历史记录---</span><span id="LineName"></span></h4>
                    </div>
                    <div class="portlet-body">
                        <div id="tab-ProgressConfig" class="tab-pane row-fluid active">
                            <div class="portlet-body">
                                <div class="clearfix">
                                    <div class="row-fluid">
                                        <div class="form-horizontal">
                                            <div style="margin: 10px 15px">
                                                <input id="btndelete" type="button" class="btn blue" value="批量删除" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <table id="tableProgress" width="100%" class="table table-striped table-bordered">
                                    <thead>
                                        <tr>
                                            <th style="display: none;">编号</th>
                                            <th id="th_checkbox" style="width:35px;">
                                                <input type="checkbox" class="group-checkable" data-set="#tableProgress .checkboxes" /></th>
                                            <th>线路长度</th>
                                            <th>已施工长度</th>
                                            <th>进度</th>
                                            <th>记录时间</th>
                                            <th>操作</th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyProgress">
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- END 进度设置 PORTLET-->
            </div>
        </div>

        <!-- strat 修改 model -->
        <div id="modifyProgressModal" class="modal hide fade">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>修改施工进度</h3>
            </div>
            <div class="modal-body">
                <div class="form-horizontal">
                    <div class="control-group">
                        <label class="control-label">线路名称</label>
                        <div class="controls">
                            <input type="text" id="modifyProgressLine" name="addSectionName" readonly="true" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">线路长度</label>
                        <div class="controls">
                            <input type="text" id="modifyProgressName" name="addSectionName" readonly="true" style="width: 145px;" />
                            <input type="text" id="modifyProgressUnit" name="addSectionName" readonly="true" style="width: 45px;" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">已施工长度</label>
                        <div class="controls">
                            <input type="text" id="modifyProgressLength" name="addSectionName" placeholder="数字" />
                            <span style="color: red">*</span>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">记录时间</label>
                        <div class="controls">
                            <div id="dpdend1" class="input-append date">
                                <input type="text" id="modifyProgressData" class="ui_time" style="width: 179px;" />
                                <span class="add-on" style="height: 20px;">
                                    <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button class="btn" data-dismiss="modal">关闭</button>
                    <input id="btnResetModifyProgress" type="button" value="重置" class="btn" />
                    <input id="btnSaveModifyProgress" type="button" value="保存并修改" class="btn btn-primary" />
                </div>
            </div>
        </div>
        <!-- end 修改  model -->

        <!-- strat 删除 model -->
        <div id="deleteProgressModal" class="modal hide fade" style="width: 400px">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>删除施工进度</h3>
            </div>
            <div class="modal-body">
                <p id="alertMsg"></p>
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">取消</button>
                <a href="#" id="btnProgressDelete" class="btn red">确定</a>
            </div>
        </div>
        <!-- end 删除 model -->
        <asp:HiddenField ID="HiddenSensorId" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenSensorLocation" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenStartTime" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenEndTime" runat="server" ClientIDMode="Static" />
    </form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script type="text/javascript" src="/resource/js/jquery-2.0.3.min.js"></script>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script src="//cdn.datatables.net/1.10.2/js/jquery.dataTables.min.js"></script>
    <script src="../resource/js/dataTableInit.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-dropdown.js"></script>
    <script src="data/js/bootstrap-datetimepicker.js"></script>
    <script src="data/js/bootstrap-datetimepicker.zh-CN.js"></script>
    <script src="/resource/js/jquery.number.min.js"></script>

    <script>
        jQuery(document).ready(function () {
            App.setPage("other");
            App.init();
        });
    </script>

    <script src="../resource/js/historyProgress.js"></script>
</asp:Content>
