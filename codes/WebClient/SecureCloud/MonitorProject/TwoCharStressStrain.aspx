<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TwoCharStressStrain.aspx.cs" Inherits="SecureCloud.MonitorProject.TwoCharStressStrain" %>

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
    <%-- <link href="../resource/library/data-tables/css/datatable.min.css" rel="stylesheet" />--%>
    <link href="../resource/library/data-tables/css/dataTable.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/bootstrap.min.css" rel="stylesheet" />

    <link href="../resource/css/metro.css" rel="stylesheet" />
    <%--<link href="../resource/css/style.css" rel="stylesheet" />--%>
    <link href="../resource/css/style_responsive.css" rel="stylesheet" />

     <link href="../data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />

    <script src="../resource/js/jquery-1.8.3.min.js"></script>
    <script src="../commFactor/js/highchartband_StressStrain.js"></script>
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
            height: 36px;
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
    <form id="TwoCharStressStrain" runat="server">
     <div>
            <div class="box">
               <%-- <div class="box-header">
                    <div class="box-icon">
                        <img src="../resource/img/chart.png" />&nbsp;图形数据
                    </div>--%>
                    <div id="tip"  style="text-align:center"></div>
               <%-- </div>--%>
                <div class="box-content">
                    <div class="row-fluid">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <b>设备位置：</b>
                                <select id="sensorList" name='sensorList' class="width100 selectpicker" data-size="10" title="请选择">
                                </select>

                                <select id="date" name="date" class="span2">
                                    <option value="day" class="btn-date">最近一天</option>
                                    <option value="week" class="btn-date">最近一周</option>
                                    <option value="month" class="btn-date">最近一月</option>                                    
                                    
                                    <option value="other" class="btn-minimize-other">其他</option>
                                </select>
                                <input type="button" id="btnQuery" value="查询" class="btn btn-success btndataquery" name="assign" />
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

                            <hr />
                          <div class="box-content span12" id="alldiv">
                                <div>
                                    <div class="span8">
                                        <div class="box" id="chart_x" style="height: 360px; display: none;"></div>
                                        <div id="x_error" style="display: none;"></div>
                                    </div>
                                    <div class="span3">
                                        <div class="box" id="XwarnBox" style="display: none">
                                            <table id="XwarnTable" class="warnTable">
                                                <tr class="heightTr">
                                                    <th class="headTh">位置:</th>
                                                    <th id="XsensorLocation" class="Th"></th>
                                                </tr>
                                                <tr class="heightTr">
                                                    <th class="headTh">告警:</th>
                                                    <th class="Th">
                                                        <a id="XsensorLink" style="cursor: pointer">查看</a>
                                                    </th>
                                                </tr>
                                                <tr class="heightTr">
                                                    <th class="headTh">最大值:</th>
                                                    <th id="XmaxValue" class="Th"></th>
                                                </tr>
                                                <tr class="heightTr">
                                                    <th class="headTh">最小值:</th>
                                                    <th id="XminValue" class="Th"></th>
                                                </tr>
                                                <tr class="heightTr">
                                                    <th class="headTh">一级阈值:</th>
                                                    <th id="XfirstThreshold" class="Th"></th>
                                                </tr>
                                                <tr class="heightTr">
                                                    <th class="headTh">二级阈值:</th>
                                                    <th id="XsecondThreshold" class="Th"></th>
                                                </tr>
                                                <tr class="heightTr">
                                                    <th class="headTh">三级阈值:</th>
                                                    <th id="XthirdThreshold" class="Th"></th>
                                                </tr>
                                                <tr class="heightTr">
                                                    <th class="headTh">四级阈值:</th>
                                                    <th id="XfourthThreshold" class="Th"></th>
                                                </tr>
                                                
                                            </table>
                                        </div>
                                    </div>
                                </div>
                                <br />

                                <div>
                                    <div class="span8">
                                        <div id="chart_y" class="box" style="height: 360px; display: none;"></div>
                                        <div id="y_error" style="display: none;"></div>
                                    </div>

                                    <div class="span3">
                                         <div class="box" id="YwarnBox" style="display:none">
                                        <table id="YwarnTable" class="warnTable">
                                            <tr class="heightTr">
                                                <th class="headTh">位置:</th>
                                                <th id="YsensorLocation" class="Th"></th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">告警:</th>
                                                <th class="Th">
                                                    <a id="YsensorLink" style="cursor: pointer">查看</a> 
                                                </th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">最大值:</th>
                                                <th id="YmaxValue" class="Th"></th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">最小值:</th>
                                                <th id="YminValue" class="Th"></th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">一级阈值:</th>
                                                <th id="YfirstThreshold" class="Th"></th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">二级阈值:</th>
                                                <th id="YsecondThreshold" class="Th"></th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">三级阈值:</th>
                                                <th id="YthirdThreshold" class="Th"></th>
                                            </tr>
                                            <tr class="heightTr">
                                                <th class="headTh">四级阈值:</th>
                                                <th id="YfourthThreshold" class="Th"></th>
                                            </tr>
                                            
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
                       <%-- <img src="../resource/img/document-spreadsheet.png" />
                        列表数据--%>
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

    <script src="../commFactor/js/Comm_Factor2_StressStrain.js"></script>
    <script src="../commFactor/js/dataParse_StressStrain.js"></script>
</body>
</html>
