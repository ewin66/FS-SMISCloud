<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeepDispChar.aspx.cs" Inherits="SecureCloud.MonitorProject.DeepDispChar" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />

    <link href="../resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/datepicker.css" rel="stylesheet" />

    <link href="../resource/css/unionboostrap.css" rel="stylesheet" />
    <link href="../resource/css/windowbox.css" rel="stylesheet" />
    <link href="../resource/library/tableTools/css/TableTools.css" rel="stylesheet" />
    <%-- <link href="../resource/library/data-tables/css/datatable.min.css" rel="stylesheet" />--%>
    <link href="../resource/library/data-tables/css/dataTable.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/bootstrap.min.css" rel="stylesheet" />

    <link href="../resource/css/metro.css" rel="stylesheet" />
    <%--<link href="../resource/css/style.css" rel="stylesheet" />--%>

    <link href="../resource/css/style_responsive.css" rel="stylesheet" />
    <%--<link href="http://netdna.bootstrapcdn.com/twitter-bootstrap/2.2.2/css/bootstrap-combined.min.css" rel="stylesheet"/>--%>

     <link href="../data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />


    <script src="../resource/js/jquery-1.8.3.min.js"></script>
    <script src="../resource/js/securecloud.js"></script>

    <style>
        .width100 {
            width: 150px;
        }
    </style>
    <title></title>
</head>
<body>
    <form id="DeepDispChar" runat="server">
        <div>
            <div class="box">
                <%--<div class="box-header">
                    <div class="box-icon">
                        <img alt="" src="../resource/img/chart.png" />&nbsp;图形数据 
                    </div>--%>
                    <div id="tip"  style="text-align: center"></div>
               <%-- </div>--%>
                <div class="box-content">
                    <div class="row-fluid">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <b>测斜管组名称:</b>
                                <select id="sensorList" name='sensorList' class="selectpicker" title="请选择">
                                </select>
                                <b>方向选择:</b>
                                <select id="XY_direction" name="XY_direction" class="width100">
                                    <option value="y">Y方向</option>
                                    <option value="x">X方向</option>
                                    <option value="xy">X,Y方向</option>
                                </select>
                                <select id="date" name="date" class="width100">
                                    <option value="day" class="btn-date">最近一天</option>
                                    <option value="week" class="btn-date">最近一周</option>
                                    <option value="month" class="btn-date">最近一月</option>                                    
                                    
                                    <option value="other" class="btn-minimize-other">其他</option>
                                </select>
                                <%--<input type="button" id="btnQuery" value="查询" class="btn btn-success btndataquery" />--%>
                            </div>
                            <div class="other-search" style="display: none;">
                                <div class="control-group">
                                    <label class="control-label">
                                        <b>监测时间从:</b></label>
                                    <div class="controls">
                                        <div id="dpform1" class="input-append date">
                                            <input type="text" id="dpform"/>
                                            <span class="add-on" style="height:20px;">
                                                <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                            </span>

                                        </div>
                                        <b>至</b>
                                        <div id="dpdend1" class="input-append date">
                                            <input type="text" id="dpdend"/>
                                            <span class="add-on" style="height:20px;">
                                                <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                            </span>

                                        </div>
                                        &nbsp;<input type="button" id="btnQuery" value="查询" class="btn btn-success btndataquery" name="assign" />
                                       
                                    </div>

                                </div>
                            </div>
                            <div class="box-content" id="deep_displacement_graph">
                                <div class="box row-fluid span3" id="X_leiji" style="height: 700px; width: 20%; display: none;">
                                </div>
                                <div class="box row-fluid span3" id="X_leiji_error" style="width: 20%; display: none;">
                                </div>
                                <div class="box row-fluid span3" id="Y_leiji" style="height: 700px; width: 20%; display: none;">
                                </div>
                                <div class="box row-fluid span3" id="Y_leiji_error" style="width: 20%; display: none;">
                                </div>
                                <div class="box row-fluid span7" id="X_dot" style="height: 340px; width: 53%; display: none;">
                                </div>
                                <div class="box row-fluid span7" id="X_dot_error" style="width: 53%; display: none;">
                                </div>
                                <div class="box row-fluid span7" id="Y_dot" style="height: 340px; width: 53%; display: none;">
                                </div>
                                <div class="box row-fluid span7" id="Y_dot_error" style="width: 53%; display: none;">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <hr />
            </div>
            <div class="box">
                <div class="box-header">
                    <div class="box-icon">
                        <%--<img alt="" src="../resource/img/document-spreadsheet.png" />
                        对比数据--%> 
                      <a href="#" class="box-collapse pull-right">
                          <img id="expand_collapse_comp" alt="" src="../resource/img/toggle-collapse.png" />
                      </a>
                    </div>
                </div>
                <div class="box-content data-table-content" style="display: none;">
                    <div id="show_table_comp" style="margin-bottom: 40px;">
                    </div>
                </div>
            </div>

            <div class="box">
                <div class="box-header">
                    <div class="box-icon">
                        <%--<img alt="" src="../resource/img/document-spreadsheet.png" />
                        列表数据 --%><a href="#" class="box-collapse pull-right">
                            <img id="expand_collapse" alt="" src="../resource/img/toggle-collapse.png" />
                        </a>
                    </div>
                </div>
                <div class="box-content data-table-content" style="display: none;">
                    <div id="show_table" style="margin-bottom: 40px;">
                    </div>
                </div>
            </div>
        </div>
        <%--         <asp:HiddenField ID="Sensor_id" runat="server" ClientIDMode="Static" />--%>
        <asp:HiddenField ID="Sensor_id" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenFactorNo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenSensorId" runat="server" ClientIDMode="Static" />
    </form>
    <script src="../resource/library/breakpoints/breakpoints.js"></script>
    <script src="../resource/library/data-tables/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="/resource/library/data-tables/DT_bootstrap.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-datepicker.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap.min.js"></script>


    <script src="../resource/js/app.js"></script>
    <%--<script src="../resource/js/securecloud.js"></script>--%>



    <script src="../resource/library/highcharts/highcharts.js"></script>
    <script src="../resource/library/highcharts/exporting.js"></script>
    <script src="../resource/library/tableTools/js/ZeroClipboard.js"></script>
    <script src="../resource/library/tableTools/js/TableTools.min.js"></script>
    
    <script src="../data/js/bootstrap-datetimepicker.js"></script> 
    <script src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>

    <script src="../resource/js/datepickerInit.js"></script>
    <script src="../resource/js/dataTableInit.js"></script>

    <script src="../commFactor/js/Deep_Displacement.js"></script>
    <script src="../commFactor/js/dataParse.js"></script>
    <!--[if lt IE 9]>
        <script src="../Resource/compatible/html5shiv.js"></script>
        <script src="../Resource/compatible/respond.min.js"></script>
    <![endif]-->

</body>
</html>
