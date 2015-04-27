<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SettleContrast.aspx.cs" Inherits="SecureCloud.MonitorProject.SettleContrast" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <link href="../resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/datepicker.css" rel="stylesheet" />
    <link href="../resource/css/unionboostrap.css" rel="stylesheet" />
    <link href="../resource/css/windowbox.css" rel="stylesheet" />
    <link href="../resource/library/tableTools/css/TableTools.css" rel="stylesheet" />
    <%--<link href="../resource/library/data-tables/css/datatable.min.css" rel="stylesheet" />--%>
    <link href="../resource/library/data-tables/css/dataTable.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/bootstrap.min.css" rel="stylesheet" />

    <link href="../resource/css/metro.css" rel="stylesheet" />
    <%--<link href="../resource/css/style.css" rel="stylesheet" />--%>
    <link href="../resource/css/style_responsive.css" rel="stylesheet" />

     <link href="../data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />

    <script src="../resource/js/jquery-1.8.3.min.js"></script>
    <style>
        .width100 {
            width: 150px;
        }
    </style>
</head>
<body>
    <form id="Comm_Factor1" runat="server">
        <div class="container-fluid">
            <div class="box">
               <%-- <div class="box-header">
                    <div class="box-icon">
                        <img alt="" src="../resource/img/chart.png" />&nbsp;图形数据 
                    </div>--%>
                    <div id="tip"  style="text-align:center"></div>
                <%--</div>--%>
                <div class="box-content">
                    <div class="row-fluid">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <b>分组:</b>
                                <select id="sensorList" name='sensorList' class="width100 selectpicker" title="请选择">
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
                                            <span class="add-on"style="height:20px;">
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
                   
                            <div class="box-content" id="comm1_graph">
                                <div class="box span3" id="comm1" style="height: 350px; width: 98%; display: none;">
                                </div>
                                <div id="comm1_error" style="display: none;">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="box">
                <div class="box-header">
                    <div class="box-icon">
                        <b>日报表展示</b>
                       <a href="#" class="box-collapse pull-right">
                            <img id="Img1" alt="" src="../resource/img/toggle-collapse.png" />
                        </a>
                    </div>
                </div>
                <div id="settleDaliyReport" class="box-content" style="display: none;">
                    <div class="row-fluid">
                        <div class="form-horizontal">
                            <div style="height: 20px;">
                                <div class="control-group">
                                    <b>监测时间:</b>
                                    <div id="dpform4" class="input-append date">
                                        <input type="text" id="dpform3" />
                                        <span class="add-on" style="height: 20px;">
                                            <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                        </span>
                                    </div>
                                    <input type="button" id="btn_Query" value="查询" class="btn btn-success btndataquery" name="assign" style="height: 30px;" />
                                </div>
                            </div>
                            <div class="box-content" id="daliyReport">
                                <div class="portlet-body">
                                    <div id="tab-SettleDaliyReport" class="tab-pane row-fluid active">
                                        <div class="portlet-body">
                                            <table id="tableSettleDaliyReport" width="100%" class="table table-striped table-bordered">
                                                <thead>
                                                    <tr>
                                                       <%-- <th　style="display: none;">设备编号</th>--%>
                                                        <th>设备位置</th>
                                                        <th>累计变量(mm)</th>
                                                        <th>本次变量(mm)</th>
                                                        <th>上次变量(mm)</th>
                                                        <th>采样时间</th>
                                                    </tr>
                                                </thead>
                                                <tbody id="tbodySettleDaliyReport">
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="box">
                <div class="box-header">
                    <div class="box-icon">
                        <%--<img alt="" src="../resource/img/document-spreadsheet.png" />
                        列表数据--%> <a href="#" class="box-collapse pull-right">
                            <img id="expand_collapse" alt="" src="../resource/img/toggle-collapse.png" />
                        </a>
                    </div>
                </div>
                <div class="box-content data-table-content" style="display: none;">
                    <div id="show_table" style="margin-bottom: 40px;">
                    </div>
                </div>
            </div>
            <%--<div id="DI" style="height:130px;"></div>--%>
            
        </div>

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
    <script src="../resource/js/securecloud.js"></script>
   
    <script src="../resource/library/highcharts/highcharts.js"></script>
    <script src="../resource/library/highcharts/exporting.js"></script>
    <script src="../resource/library/tableTools/js/ZeroClipboard.js"></script>
    <script src="../resource/library/tableTools/js/TableTools.min.js"></script>

    <script src="../data/js/bootstrap-datetimepicker.js"></script> 
    <script src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>

    <script src="../resource/js/datepickerInit.js"></script>
    <script src="../resource/js/dataTableInit.js"></script>
    <script src="../resource/js/common.js"></script>
    <script src="../commFactor/js/SettleContrast.js"></script>
    <script src="../resource/js/jquery.number.min.js"></script>
    <script src="../commFactor/js/dataParse.js"></script>
</body>
</html>