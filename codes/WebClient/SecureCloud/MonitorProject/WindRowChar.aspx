<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WindRowChar.aspx.cs" Inherits="SecureCloud.MonitorProject.WindRowChar" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/datepicker.css" rel="stylesheet" />
    <link href="../resource/css/unionboostrap.css" rel="stylesheet" />
    <link href="../resource/css/windowbox.css" rel="stylesheet" />
    <%-- <link href="../resource/library/data-tables/css/datatable.min.css" rel="stylesheet" />--%>
    <link href="../resource/library/data-tables/css/dataTable.css" rel="stylesheet" />
    <link href="../resource/library/tableTools/css/TableTools.css" rel="stylesheet" />
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
<form id="WindRoseChar" runat="server">
        <div class="container-fluid">
            <div class="box">
                <%--<div class="box-header">
                    <div class="box-icon">
                        <img alt="" src="../resource/img/chart.png" />&nbsp;图形数据 
                    </div>--%>
                    <div id="tip"  style="text-align:center"></div>
               <%-- </div>--%>
                <div class="box-content">
                    <div class="row-fluid">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <b>设备位置:</b>
                                <select id="sensorList" name='sensorList' class="span5 selectpicker" multiple="multiple" data-size="10" title="请选择">
                                </select>
                                <select id="date" name="date" class="width100">
                                    <option value="month" class="btn-date">最近一个月</option>
                                    <option value="quarter" class="btn-date">最近一个季度</option>
                                    <option value="year" class="btn-date" selected>最近一年</option>
                                    <option value="other" class="btn-minimize-other">其他</option>
                                </select>
                         <%--       <div style="display: inline;">
                                    <input type="text" id="dpform" class="width100" value="2013-09-29" disabled="disabled" />
                                    <input type="text" id="dpdend" class="width100" value="2013-09-29" disabled="disabled" />
                                </div>--%>
                                <input type="button" id="btnQuery" value="查询" class="btn btn-success btndataquery" />
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
                                            <span class="add-on"style="height:20px;">
                                                <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                            </span>

                                        </div>
                                       
                                    </div>
                                </div>
                            </div>

                            <div id="container1" style="float: left; margin: 40px;">
                                <%--<p>10分钟内的极值风速为: 9.6m/s</p>
                <p>风向为：N</p>
                <p>仰角为：45°</p>--%>
                            </div>
                            <div id="content" style="min-width: 420px; max-width: 600px; height: 400px; margin: 0 auto"></div>
                            <div style="display: none">
                                <table id="freq" border="0" cellspacing="0" cellpadding="0">
                                    <tr nowrap bgcolor="#CCCCFF">
                                        <th colspan="9" class="hdr">Table of Frequencies (percent)</th>
                                    </tr>
                                    <tr nowrap bgcolor="#CCCCFF">
                                        <th class="freq">Direction</th>
                                        <th class="freq">&lt; 0.5 m/s</th>
                                        <th class="freq">0.5-2 m/s</th>
                                        <th class="freq">2-4 m/s</th>
                                        <th class="freq">4-6 m/s</th>
                                        <th class="freq">6-8 m/s</th>
                                        <th class="freq">8-10 m/s</th>
                                        <th class="freq">&gt; 10 m/s</th>
                                        <th class="freq">Total</th>
                                    </tr>
                                    <tbody id="winddata">
                                    </tbody>
                                    <%--   <tr nowrap>
                                            <td class="totals">Total</td>
                                            <td class="totals">0</td>
                                            <td class="totals">0</td>
                                            <td class="totals">0</td>
                                            <td class="totals">0</td>
                                            <td class="totals">0</td>
                                            <td class="totals">0</td>
                                            <td class="totals">0</td>
                                            <td class="totals">&nbsp;</td>
                                        </tr>--%>
                                </table>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

           <%-- <div class="box">
                <div class="box-header">
                    <div class="box-icon">
                        <img alt="" src="../resource/img/document-spreadsheet.png" />
                        列表数据 <a href="#" class="box-collapse pull-right">
                            <img id="expand_collapse" alt="" src="../resource/img/toggle-collapse.png" />
                        </a>
                    </div>
                </div>
                <div class="box-content data-table-content" style="display: none;">
                    <div id="show_table" style="margin-bottom: 40px;">
                    </div>
                </div>
            </div>--%>
            <div id="DI" style="height:130px;"></div>
        </div>
        <asp:HiddenField ID="HiddenFactorNo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenSensorId" runat="server" ClientIDMode="Static" />
    </form>

    <script src="../resource/library/breakpoints/breakpoints.js"></script>
    <script src="../resource/library/data-tables/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="/resource/library/data-tables/DT_bootstrap.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-datepicker.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-dropdown.js"></script>
      <script src="../resource/library/bootstrap/js/bootstrap.min.js"></script>

    <script src="../resource/js/app.js"></script>
    <script src="../resource/js/securecloud.js"></script>

    <script src="../resource/library/highcharts/highcharts.js"></script>
    <script src="../resource/library/highcharts/highcharts-more.js"></script>
    <script src="../resource/library/highcharts/exporting.js"></script>
    <script src="../resource/js/data.js"></script>

    <script src="../data/js/bootstrap-datetimepicker.js"></script> 
    <script src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>

    <script src="../resource/library/tableTools/js/ZeroClipboard.js"></script>
    <script src="../resource/library/tableTools/js/TableTools.min.js"></script>
    
    <script src="../resource/js/RoseDate.js"></script>
    <script src="../resource/js/dataTableInit.js"></script>

    <script src="../commFactor/js/WindRose.js"></script>
</body>
</html>
