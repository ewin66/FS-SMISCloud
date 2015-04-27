<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InfiltrationLineChar.aspx.cs" Inherits="SecureCloud.MonitorProject.InfiltrationLineChar" %>

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
    <link href="../resource/library/data-tables/css/dataTable.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../resource/css/metro.css" rel="stylesheet" />
    <%--<link href="../resource/css/style.css" rel="stylesheet" />--%>
    <link href="../resource/css/style_responsive.css" rel="stylesheet" />

     <link href="../data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />


    <style>
        .width100 {
            width: 150px;
        }

        .warnTable {
            margin: 0 auto;
            width: 100%;
            clear: both;
            border-collapse: collapse;
        }

        .heightTr {
            height: 40px;
        }

        .headTh {
            padding: 3px 0px 0px 10px;
            font-weight: bold;
            border-bottom: 1px solid #aaa;
        }

        .Th {
            border-bottom: 1px solid #aaa;
            font-weight: normal;
        }
    </style>
</head>
<body>
    <form id="InfiltrationLine" runat="server">
        <div>
            <div class="box">
                <div class="box-content">
                    <div class="row-fluid">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <b>坝体位置:</b>
                                <select id="sensorList" name='sensorList' class="width100 selectpicker" multiple="multiple" data-size="10" title="请选择">
                                </select>
                                <select id="date" name="date" class="width100">
                                    <option value="day" class="btn-date">最近一天</option>
                                    <option value="week" class="btn-date">最近一周</option>
                                    <option value="month" class="btn-date">最近一月</option>                                    
                                    <option value="other" class="btn-minimize-other">其他</option>
                                </select>
                                <input type="button" id="btnQuery" value="查询" class="btn btn-success btndataquery" />
                            </div>
                            <div class="other-search" style="display: none;">
                                <div class="control-group">
                                    <label class="control-label">
                                        <b>从:</b></label>
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
                                       
                                    </div>

                                </div>
                            </div>

                            <div class="box-content span12" id="comm1_graph">
                                <div class="span6" id="comm_graph1" style="height: auto"></div>
                                <div class="box span5" id="comm_graph2" style="height: 500px;"></div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="box">
                <div class="box-header">
                    <div class="box-icon">
                        <a href="#" class="box-collapse pull-right">
                            <img id="expand_collapse" alt="" src="../resource/img/toggle-collapse.png" />
                        </a>
                    </div>
                </div>
                <div class="box-content data-table-content" style="display: none;">
                    <div id="show_table" style="margin-bottom: 40px;">
                    </div>
                </div>
            </div>
            <div id="DI" style="height:130px;"></div>
        </div>

        <asp:HiddenField ID="HiddenFactorNo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenSensorId" runat="server" ClientIDMode="Static" />
    </form>

     <script src="/resource/js/jquery-1.8.3.min.js"></script>

    <script src="../resource/library/breakpoints/breakpoints.js"></script>   
    <script src="/resource/library/data-tables/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="/resource/library/data-tables/DT_bootstrap.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-datepicker.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap.min.js"></script>

    <script src="../resource/js/app.js"></script>
    <script src="../resource/js/securecloud.js"></script>

    <script src="../commFactor/js/highchartband.js"></script>
    
    <script src="/resource/js/common.js"></script>
      
    <script src="../resource/library/bootstrap/js/bootstrap-dropdown.js"></script>

    <script src="../resource/library/highcharts/highcharts.js"></script>
    <script src="../resource/library/highcharts/exporting.js"></script>
    <script src="../resource/library/tableTools/js/ZeroClipboard.js"></script>
    <script src="../resource/library/tableTools/js/TableTools.min.js"></script>

    <script src="../data/js/bootstrap-datetimepicker.js"></script> 
    <script src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>

    <script src="../resource/js/datepickerInit.js"></script>
    <script src="../resource/js/dataTableInit.js"></script>

    <script src="../commFactor/js/dataParse.js"></script>
    <script src="../commFactor/js/InfiltrationLine.js"></script>
</body>
</html>
