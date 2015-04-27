<%@ Page Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="DataWaringSupport.aspx.cs" Inherits="SecureCloud.Support.DataWaringSupport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/resource/library/DataTables-1.10.2/css/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="/resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />
    <link href="../data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />
    <link href="css/commonSupport.css" rel="stylesheet" />
    <link href="css/alarmPage.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid ">
        <!-- BEGIN PAGE HEADER-->
        <div class="row-fluid " style="min-width: 960px;">
            <ul class="breadcrumb" style="position: relative;">
                <li>
                    <i class="icon-bell"></i>
                    <a href="DataWaringSupport.aspx" style="text-decoration: none">告警管理</a>
                    <i class="icon-angle-right"></i>
                </li>
                <li><a href="javascript:;" style="text-decoration: none;cursor:text;">组织</a><i class="icon-angle-right"></i></li>
                <li>
                    <select id="listOrg" class="chzn-select dropdown-menu"  style="width: 300px;" data-size="10" data-placeholder="请选择">
                    </select>
                </li>
                <li><i class="icon-angle-right"></i><a href="javascript:;" style="text-decoration: none;cursor:text;">结构物</a><i class="icon-angle-right"></i></li>
                <li>
                    <select id="listStruct" class="chzn-select dropdown-menu" style="width: 300px;" data-size="10" data-placeholder="请选择">
                    </select>
                </li>
                <li style="position: absolute; right: 10px; top: 16px;">
                    <a style="cursor:pointer;color: rgb(0, 85, 128); text-decoration: none;" href="MainPage.aspx" target="_self">返回主页</a>
                </li>
            </ul>
        </div>
        <!-- END PAGE HEADER -->
        <div class="row-fluid nowrap" style="min-width: 960px;">
            <div class="portlet">
                <div class="portlet-title" style="margin-top: -5px;height: 35px;line-height: 35px;"> 
                    <div class="row-fluid" style="max-height: 20px;">
                        <div class="form-horizontal nowrap">
                            <div class="mynav nowrap" style="padding: 0 0 0 10px;">
                                <ul class="nav" style="min-width: 480px;">
                                    <li style="width: auto; font-size: 12px; font-weight: bold; color: #666;">
                                        <span title="技术支持未确认告警统计">未确认告警统计：</span>
                                    </li>
                                    <li style="margin-left: 10px;"><span id="warnlevel_1" style="color: #ff0000;"></span></li>
                                    <li><span id="warnlevel_2" style="color: #ff8000;"></span></li>
                                    <li><span id="warnlevel_3" style="color: #A757A8;"></span></li>
                                    <li><span id="warnlevel_4" style="color: #0000ff;"></span></li>
                                </ul>
                            </div>
                            <div class="nowrap " style="float: left;">
                                <ul class="mynav " style="padding: 0 0 0 10px;  min-width: 460px;">
                                    <li style="width: auto;">
                                        <i class="icon-info-sign" style="color: grey"></i>
                                        <span style="color: #666;">告警等级由高到低依次为：</span>
                                    </li>

                                    <li style="margin-left: 10px; width: auto; color: #ff0000;">一级&nbsp;<i class="icon-angle-right"></i></li>

                                    <li style="width: auto; color: #ff8000;">二级&nbsp;<i class="icon-angle-right"></i></li>

                                    <li style="width: auto; color: #A757A8;">三级&nbsp;<i class="icon-angle-right"></i></li>

                                    <li style="width: auto; color: #0000ff;">四级&nbsp;</li>
                                </ul>
                            </div>
                            <div class="tools " style="float: right; text-align: right; min-width: 20px;">
                                <a href="javascript:;" class="collapse"></a>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="clearfix"></div>
                    <div id="loading-alarm" class="fixed-marker-loading"></div>
                    <div>
                        <ul class="select" style="margin-top: 1px;">
                            <li class="select-list">
                                <dl id="select1">
                                    <dt>告警等级：</dt>
                                    <dd id="filter_level" class="selected"><a  href="javascript:;">全部</a></dd>
                                    <dd id="level_1"><a  href="javascript:;">一级</a></dd>
                                    <dd id="level_2"><a  href="javascript:;">二级</a></dd>
                                    <dd id="level_3"><a  href="javascript:;">三级</a></dd>
                                    <dd id="level_4"><a  href="javascript:;">四级</a></dd>
                                </dl>
                            </li>
                            <li class="select-list">
                                <dl id="select2">
                                    <dt>设备类型：</dt>
                                    <dd id="filter_device" class="selected"><a  href="javascript:;">全部</a></dd>
                                    <dd><a  href="javascript:;">DTU</a></dd>
                                    <dd><a  href="javascript:;">传感器</a></dd>
                                </dl>
                            </li>
                            <li class="select-list">
                                <dl id="select3">
                                    <dt>状态：</dt>
                                    <dd id="filter_unprocess" class="selected"><a  href="javascript:;" title="技术支持未确认告警">未确认</a></dd>
                                    <dd><a  href="javascript:;">已确认</a></dd>
                                    <dd><a  href="javascript:;">已下发</a></dd>
                                    <dd><a  href="javascript:;">全部</a></dd>
                                </dl>
                            </li>
                        </ul>
                    </div>
                    <div class="filter nowrap" style="height: 34px; min-width: 767px;">
                        <div style="float: left; font-weight: bold; width: auto; line-height: 34px;">
                            告警产生时间：
                        </div>
                        <div style="float: left; width: auto;margin-left: 5px;">
                            <div class=" form-horizontal" id="tab_time">
                                <div class=" input-append date">
                                    <input id="dpfrom" class="ui_timepicker " type="text" placeholder="起始时间" />
                                    <span class="add-on" style="height: 20px;">
                                        <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                    </span>
                                </div>
                                <b>至</b>

                                <div class=" input-append date">
                                    <input id="dpend" class="ui_timepicker " type="text" placeholder="终止时间" />
                                    <span class="add-on" style="height: 20px;">
                                        <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="select row-fluid" style="width: auto; padding-top: 5px; margin-top:-12px;">
            <div class="form-horizontal">
                <div style="float: left; width:auto">
                    <ul style="margin-left: 2px;">
                        <li class="select-result">
                            <dl>
                                <dt>结构物告警&nbsp;<i class="icon-angle-right"></i></dt>
                                <dd class="select-no">暂时没有选择过滤条件</dd>
                            </dl>
                        </li>
                    </ul>
                </div>
                <div class="btn-small btn-group" style="float: right; width: auto; margin-left: 10px;">
                    <input type="button" id="filter_summit" class="btn green" value="确定" />
                </div>
            </div>
        </div>
        <div class="row-fluid">
            <div class="form-horizontal">
                <div class="filter nowrap" style="height: 34px; line-height: 34px; min-width: 960px;">
                    <div style="float: left; font-weight: bold; width: auto;">
                        排序方式：
                    </div>
                    <div style="float: left; width:auto; margin-left: 20px; padding-top: 6px;">
                        <div class="row-fluid">
                            <div class="form-horizontal">
                                <div style="float: left; width: auto;">
                                    <ul>
                                        <li class="dropdown">
                                            <a  href="javascript:;" class="dropdown-toggle" data-toggle="dropdown">
                                                <span id="sortDevice">告警源</span>
                                                <i class="icon-angle-down"></i>
                                            </a>
                                            <ul class="dropdown-menu">
                                                <li><a  href="javascript:;" id="deviceDesc">告警源位置降序</a></li>
                                                <li class="divider"></li>
                                                <li><a  href="javascript:;" id="deviceAsc">告警源位置升序</a></li>
                                                <li class="divider"></li>
                                            </ul>
                                        </li>
                                    </ul>
                                </div>
                                <div style="float: left; width: auto; margin-left: 10px;">
                                    <ul>
                                        <li class="dropdown">
                                            <a  href="javascript:;" class="dropdown-toggle" data-toggle="dropdown">
                                                <span id="sortLevel">等级从高到低</span>
                                                <i class="icon-angle-down"></i>
                                            </a>
                                            <ul class="dropdown-menu">
                                                <li><a  href="javascript:;" id="levelDesc">等级从高到低</a></li>
                                                <li class="divider"></li>
                                                <li><a  href="javascript:;" id="levelAsc">等级从低到高</a></li>
                                                <li class="divider"></li>
                                            </ul>
                                        </li>
                                    </ul>
                                </div>
                                <div style="float: left; width: auto; margin-left: 10px;">
                                    <ul>
                                        <li class="dropdown">
                                            <a  href="javascript:;" class="dropdown-toggle" data-toggle="dropdown">
                                                <span id="sortTime">告警产生时间</span>
                                                <i class="icon-angle-down"></i>
                                            </a>
                                            <ul class="dropdown-menu">
                                                <li><a  href="javascript:;" id="timeDesc">告警时间降序</a></li>
                                                <li class="divider"></li>
                                                <li><a  href="javascript:;" id="timeAsc">告警时间升序</a></li>
                                                <li class="divider"></li>
                                            </ul>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="btn-small btn-group" style="float: right; margin-top: 5px;">
            <input type="button" id="btnDownload" class="btn green" value="导出到Excel" />
        </div>
        <table class="table table-striped table-bordered table-hover" id="warnTable">
            <thead>
                <tr>
                    <th class="sorting_disabled" style="width: 15%">告警源</th>
                    <th style="width: 5%">等级</th>
                    <th style="width: 15%">产生时间</th>
                    <th style="width: 15%">可能原因</th>
                    <th style="width: 17%">告警信息</th>
                    <th style="width: 8%">状态</th>
                    <th style="width: 10%">确认信息</th>
                    <th style="width: 15%">操作</th>
                </tr>
            </thead>
            <tbody id="warnTbody">
            </tbody>
        </table>
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
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script src="/resource/js/jquery-1.8.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script src="/resource/library/DataTables-1.10.2/js/jquery.dataTables.min.js"></script>
    <script src="/data/js/bootstrap-datetimepicker.js"></script>
    <script src="/data/js/bootstrap-datetimepicker.zh-CN.js"></script>
    <script src="/resource/library/chosen-bootstrap/chosen/chosen.jquery.js"></script>
    <script src="js/DataWaringSupport.js"></script>
</asp:Content>
