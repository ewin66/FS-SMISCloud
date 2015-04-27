<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VibrationCharReticulatedShell.aspx.cs" Inherits="SecureCloud.MonitorProject.VibrationCharReticulatedShell" %>

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
    <link href="../resource/library/DataTables-1.10.2/css/jquery.dataTables.min.css" rel="stylesheet" />
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
    <form id="VibrationReticulatedShell" runat="server">
        <div class="tabbable tabbable-custom">
            <ul class="nav nav-tabs">
                <li id="tabRt" class="active"><a href="#tab_Rt" data-toggle="tab">实时振动</a></li>
                <li id="tabHistory"><a href="#tab_History" data-toggle="tab">历史查询</a></li>
            </ul>

            <div class="tab-content">
                <div class="tab-pane row-fluid  active" style="overflow: hidden" id="tab_Rt">
                    <div class="box">
                        <div id="tipRt" style="text-align: center"></div>

                        <div class="box-content">
                            <div class="row-fluid">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <b>设备位置:</b>
                                        <select id="sensorListRt" name='sensorList' class="width100 selectpicker" data-size="10" title="请选择">
                                        </select>
                                        <b>触发时段:</b>
                                        <input type="text" id="txtRtBatch" value="" readonly="readonly" />
                                        <input type="checkbox" id="chkShowTable" /><b>  显示数据表格</b>
                                    </div>

                                    <div class="box-content span11" id="rt_graph">
                                        <div>
                                            <div class="span11">
                                                 <%--提示信息位置影响 rt_ori2_error rt_ori1_error--%>
                                                <div id="rt_ori2_error" style="display: none;">
                                                </div>
                                                <div class="box" id="rt_ori" style="height: 360px; display: none; padding: 10px">
                                                </div>
                                                <div id="rt_ori1_error" style="display: none;">
                                                </div>
                                            </div>
                                        </div>
                                        <div>
                                            <div class="span11">
                                                <div class="box" id="rt_ori2" style="height: 360px; display: none; padding: 10px">
                                                </div>
                                              
                                            </div>

                                        </div>
                                    </div>
                                    <div class="box-content span11" id="rt_graph2">
                                        <div>
                                            <div class="span11">
                                                <div class="box" id="rt" style="height: 360px; display: none; padding: 10px">
                                                </div>
                                                <div id="rt_error" style="display: none;">
                                                </div>
                                            </div>
                                        </div>
                                        <div>
                                            <div class="span11">
                                                <div class="box" id="rt2" style="height: 360px; display: none; padding: 10px">
                                                </div>
                                                <div id="rt_error2" style="display: none;">
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
                                <%-- <img alt="" src="../resource/img/document-spreadsheet.png" />
                        列表数据--%> <a href="#" class="box-collapse pull-right">
                            <img id="Img1" alt="" src="../resource/img/toggle-collapse.png" />
                        </a>
                            </div>
                        </div>
                        <div class="box-content data-table-content" style="display: none;">
                            <div id="rt_table" style="margin-bottom: 40px; display: none">
                            </div>
                        </div>
                    </div>
                </div>

                <div class="tab-pane row-fluid" style="overflow: hidden" id="tab_History">
                    <div class="box">
                        <div id="tip" style="text-align: center"></div>

                        <div class="box-content">
                            <div class="row-fluid">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <b>设备位置:</b>
                                        <select id="sensorList" name='sensorList' class="width100 selectpicker" data-size="10" title="请选择">
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
                                                <b>监测时间从:</b></label>
                                            <div class="controls">
                                                <div id="dpform1" class="input-append date">
                                                    <input type="text" id="dpform" />
                                                    <span class="add-on" style="height: 20px;">
                                                        <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                                    </span>

                                                </div>
                                                <b>至</b>
                                                <div id="dpdend1" class="input-append date">
                                                    <input type="text" id="dpdend" />
                                                    <span class="add-on" style="height: 20px;">
                                                        <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                                    </span>

                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                    <div>
                                        <b>触发时段:</b>
                                        <select id="TimeList" name='TimeList' class="width100 selectpicker" data-size="10" title="无">
                                        </select>
                                    </div>

                                    <div class="box-content span12" id="comm1_graph">
                                        <div>
                                            <div class="span8">
                                                <%--提示信息位置影响 comm2_ori_error comm1_ori_error--%>
                                                <div id="comm2_ori_error" style="display: none;">
                                                </div>
                                                <div class="box" id="comm1_ori" style="height: 360px; display: none; padding: 10px">
                                                </div>
                                                <div id="comm1_ori_error" style="display: none;">
                                                </div>
                                            </div>
                                            <div class="span3">
                                                <div class="box" id="warnBox" style="display: none">
                                                    <table id="warnTable" class="warnTable">
                                                        <tr class="heightTr">
                                                            <th class="headTh">位置:</th>
                                                            <th id="XsensorLocation" class="Th"></th>
                                                        </tr>
                                                        <tr class="heightTr">
                                                            <th class="headTh">历史告警:</th>
                                                            <th id="warnTh" class="Th">
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
                                          <div>
                                            <div class="span8">
                                                <div class="box" id="comm2_ori" style="height: 360px; display: none; padding: 10px">
                                                </div>
                                              
                                            </div>
                                            <div class="span3">
                                                <div class="box" id="warnBox2" style="display: none">
                                                    <table id="warnTable2" class="warnTable">
                                                        <tr class="heightTr">
                                                            <th class="headTh">位置:</th>
                                                            <th id="XsensorLocation2" class="Th"></th>
                                                        </tr>
                                                        <tr class="heightTr">
                                                            <th class="headTh">历史告警:</th>
                                                            <th id="warnTh2" class="Th">
                                                                <a id="XsensorLink2" style="cursor: pointer">查看</a>
                                                            </th>
                                                        </tr>
                                                        <tr class="heightTr">
                                                            <th class="headTh">最大值:</th>
                                                            <th id="XmaxValue2" class="Th"></th>
                                                        </tr>
                                                        <tr class="heightTr">
                                                            <th class="headTh">最小值:</th>
                                                            <th id="XminValue2" class="Th"></th>
                                                        </tr>
                                                        <tr class="heightTr">
                                                            <th class="headTh">一级阈值:</th>
                                                            <th id="XfirstThreshold2" class="Th"></th>
                                                        </tr>
                                                        <tr class="heightTr">
                                                            <th class="headTh">二级阈值:</th>
                                                            <th id="XsecondThreshold2" class="Th"></th>
                                                        </tr>
                                                        <tr class="heightTr">
                                                            <th class="headTh">三级阈值:</th>
                                                            <th id="XthirdThreshold2" class="Th"></th>
                                                        </tr>
                                                        <tr class="heightTr">
                                                            <th class="headTh">四级阈值:</th>
                                                            <th id="XfourthThreshold2" class="Th"></th>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                      
                                    </div>

                                    <div class="box-content span12" id="comm2_graph">
                                        <div>
                                            <div class="span8">
                                                <div class="box" id="comm1" style="height: 360px; display: none; padding: 10px">
                                                </div>
                                                <div id="comm1_error" style="display: none;">
                                                </div>
                                            </div>
                                        </div>
                                        <div>
                                            <div class="span8">
                                                <div class="box" id="comm2" style="height: 360px; display: none; padding: 10px">
                                                </div>
                                                <div id="comm2_error" style="display: none;">
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
                                <%-- <img alt="" src="../resource/img/document-spreadsheet.png" />
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
                </div>
            </div>
            <div id="DI" style="height: 130px;"></div>
        </div>
        <asp:HiddenField ID="HiddenFactorNo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenSensorId" runat="server" ClientIDMode="Static" />
    </form>
    <script src="/resource/js/jquery-1.8.3.min.js"></script>

    <script src="../resource/library/breakpoints/breakpoints.js"></script>
    <script src="../resource/library/DataTables-1.10.2/js/jquery.dataTables.min.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-datepicker.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap.min.js"></script>

    <script src="../resource/js/app.js"></script>
    <script src="../resource/js/securecloud.js"></script>

    <script src="/resource/js/common.js"></script>

    <script src="../resource/library/bootstrap/js/bootstrap-dropdown.js"></script>

    <%-- <script src="../resource/library/echarts/echarts-all.js"></script>
    <script src="../resource/library/echarts/echarts-blue.js"></script>--%>
    <script src="../resource/library/highcharts/highcharts.js"></script>
    <script src="../resource/library/highcharts/exporting.js"></script>


    <script src="../resource/library/tableTools/js/ZeroClipboard.js"></script>
    <script src="../resource/library/tableTools/js/TableTools.min.js"></script>

    <script src="../data/js/bootstrap-datetimepicker.js"></script>
    <script src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>

    <script src="../resource/js/datepickerInit.js"></script>
    <%--<script src="../resource/js/dataTableInit.js"></script>--%>

    <script src="../commFactor/js/dataParse_ReticulatedShell.js"></script>
    <script src="../commFactor/js/Vibration_ReticulatedShell.js"></script>
</body>
</html>

