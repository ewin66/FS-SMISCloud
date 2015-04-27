<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProgressConfig.aspx.cs" Inherits="SecureCloud.ProgressConfig" %>

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

        .bootstrap-select:not([class*="span"]):not([class*="col-"]):not([class*="form-control"]) {
            width: 249px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="DataWarningTest" runat="server">
        <div class="container-fluid">
            <div class="row-fluid">
                <ul class="breadcrumb" style="margin-top: 10px;">
                    <li>
                        <i class="icon-home"></i>
                        <a href="/index.aspx">主页</a>
                        <i class="icon-angle-right"></i>
                    </li>
                    <li><a href="javascript:;">结构物</a><i class="icon-angle-right"></i></li>
                    <li>
                        <small class="dropdown" style="display: inline-block;">
                            <a href="javascript:;" class="dropdown-toggle" data-toggle="dropdown"><i class="icon-angle-down"></i></> 
                            </a>
                            <ul class="dropdown-menu">
                            </ul>
                        </small>
                    </li>
                </ul>

                <!-- BEGIN 进度设置-->
                <div class="portlet box light-grey">
                    <div class="portlet-title">
                        <h4><i class="icon-star"></i><span id="ProgressTitle">施工进度</span></h4>
                    </div>
                    <div class="portlet-body">
                        <div id="tab-ProgressConfig" class="tab-pane row-fluid active">
                            <div class="portlet-body">
                                <div class="clearfix">
                                    <div class="row-fluid">
                                        <div class="form-horizontal">
                                            <div class="span2" style="margin-bottom: 10px;">
                                                <input id="btnAddProgress" type="button" class="btn blue" value="施工进度维护" href='#addProgressModal' data-toggle='modal' style="display: block;" />
                                            </div>
                                            <div class="span10">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <table id="tableProgress" width="100%" class="table table-striped table-bordered">
                                    <thead>
                                        <tr>
                                            <th style="display: none;">进度编号</th>
                                            <th style="display: none;">线路编号</th>
                                            <th>线路</th>
                                            <th>线路长度</th>
                                            <th>已施工长度</th>
                                            <th>当前进度</th>
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

        <!-- Modal -->
        <!-- add Progress model -->
        <div id="addProgressModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="max-height: 600px;">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3 id="addProgressModalTitle">施工进度维护</h3>
            </div>
            <div class="modal-body" style="max-height: 400px;">
                <div class="form-horizontal">
                    <div class="control-group" style="display: none;">
                        <label class="control-label">线路编号</label>
                        <div class="controls">
                            <input type="text" id="addLineId" name="addSectionName" readonly="true" style="width: 172px;" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">线路名称</label>
                        <div class="controls">
                            <select id="addProgressLine" name="modifyDTUFactory" class="chzn-select" style="width: 243px;">
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">线路长度</label>
                        <div class="controls">
                            <input type="text" id="addProgressName" name="addSectionName" readonly="true" style="width: 172px;" />
                            <input type="text" id="addLineUnit" name="addSectionName" readonly="true" style="width: 45px;" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">最近进度</label>
                        <div class="controls">
                            <input type="text" id="addProgressNumber" name="addSectionName" readonly="true" style="width: 235px;" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">已施工长度</label>
                        <div class="controls">
                            <input type="text" id="addProgressLength" name="addSectionName" placeholder="数字" style="width: 235px;" />
                            <span style="color: red">*</span>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">记录时间</label>
                        <div class="controls">
                            <div id="dpform1" class="input-append date">
                                <input type="text" id="addProgressData" class="ui_timepicker" style="width: 208px;" />
                                <span class="add-on" style="height: 20px;">
                                    <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button id="btnCloseProgress" class="btn" data-dismiss="modal">关闭</button>
                    <input id="btnResetProgress" type="button" value="重置" class="btn" />
                    <%--<input id="btnSaveCloseSchedule" type="button" value="保存并关闭" class="btn btn-primary" />--%>
                    <input id="btnSaveProgress" type="button" value="保存" class="btn btn-primary" />
                </div>
            </div>
        </div>
        <!-- end add Progress model -->

        <!-- strat 修改 model -->
        <div id="modifyProgressModal" class="modal hide fade">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>修改施工进度</h3>
            </div>
            <div class="modal-body">
                <div class="form-horizontal">
                    <div class="control-group" style="display: none;">
                        <label class="control-label">线路编号</label>
                        <div class="controls">
                            <input type="text" id="modifyLineId" name="addSectionName" readonly="true" style="width: 172px;" />
                        </div>
                    </div>
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
    <script src="../resource/js/Progress.js"></script>
</asp:Content>
