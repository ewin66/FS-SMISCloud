<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="dataContact3.aspx.cs" Inherits="SecureCloud.dataContact3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
    <link href="/resource/library/bootstrap/css/datepicker.css" rel="stylesheet" />
    <link href="/resource/css/unionboostrap.css" rel="stylesheet" />
    <link href="/resource/css/windowbox.css" rel="stylesheet" />
    <link href="/resource/library/tableTools/css/TableTools.css" rel="stylesheet" />
    <link href="/resource/library/data-tables/css/dataTable.css" rel="stylesheet" />
    <%--<link href="/resource/css/glyphicons.css" rel="stylesheet" />--%>
    <link rel="shortcut icon" href="favicon.ico" />
    <%--<link href="resource/css/halflings.css" rel="stylesheet" />--%>        
    <link href="data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />   
    <link href="/resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid" style="min-width:817px;">
        
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
                        <a href="#" class="dropdown-toggle" data-toggle="dropdown"><i class="icon-angle-down"></i></> 
                        </a>
                        <ul class="dropdown-menu">
                        </ul>
                    </small>
                </li>
            </ul>
             <div class="portlet box light-grey">
            <div class="portlet-body " id="comm1_graph">
            <div class="form-horizontal">
               <div class="box">
                        <div class="box-header">
                            <div class="box-icon">
                                <i class="icon-bar-chart"></i>
                                <span>关联对象</span>
                                <a href="#" class="box-collapse pull-right">
                                    <img id="expand_collapse" alt="" src="../resource/img/toggle-collapse.png" />
                                </a>
                            </div>
                        </div>
                        <div class="box-content">
                            <table>
                                <tr>
                                    <td style="text-align: left;vertical-align: top;">
                                        <div class="control-group　">
                                            <table>
                                                <tr>
                                                    <td style="width: 88px;text-align: left;"><b>监测因素:</b></td>
                                                    <td style="padding-right: 20px;">
                                                        <select id="factorList1" name='factorList1' data-size="10" title="请选择">
                                                        </select>
                                                    </td>
                                                    <td ><b>监测点位置:</b></td>
                                                    <td>
                                                        <select id="sensorList1" data-placeholder="请选择" name='sensorList1' class="chzn-select" multiple="multiple" data-size="10" title="请选择">
                                                        </select>
                                                    </td>
                                                </tr>
                                                <tr id="factor-correlation">
                                                    
                                                     <td><b>关联监测因素:</b></td>
                                                    <td>
                                                        <select id="factorList2" name='factorLis2' class="width100 selectpicker" data-size="10" title="请选择">
                                                        </select>
                                                    </td>
                                                    <td><b>关联监测点位置:</b></td>
                                                    <td>
                                                        <select id="sensorList2" data-placeholder="请选择" name='sensorList2' class="chzn-select " multiple="multiple" data-size="10" title="请选择">
                                                        </select>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                   </tr>
                                <tr>
                                   <td style="text-align: left;" >
                                        <div class="control-group" id="selectTime">
                                            <table>
                                                <tr>
                                                    <td style="width: 88px;text-align: left;"><b>时段选择:</b></td>
                                                    <td>
                                                        <div id="dpform1" class="input-append date">
                                                            <input type="text" id="dpform" style="width: 180px;" />
                                                            <span class="add-on" style="height: 20px;">
                                                                <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                                            </span>

                                                        </div>
                                                    </td>
                                                    <td style="width: 120px;text-align: center;"><b>至</b></td>
                                                    <td>
                                                        <div id="dpdend1" class="input-append date">
                                                            <input type="text" id="dpdend" style="width: 180px;" />
                                                            <span class="add-on" style="height: 20px;">
                                                                <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                                                            </span>

                                                        </div>
                                                    </td>
                                                    <td>
                                                        <input type="button" id="btnQuery" value="查询" class="btn btn-success btndataquery" style="height: 30px;"/>

                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            
                        </div>
                    </div>

               

                
                    <div >
                        <div class="box" id="comm1" style="height: 360px;">
                        </div>
                         <div id="comm1_error" class="box" style="display: none;">
                        </div>
                        <div id="comm2_error" class="box" style="display: none;">
                        </div>
                    </div>

                </div>
                 <div id="tip" style="text-align: left;"></div>
            </div>
        </div>
    </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script src="resource/js/jquery-1.8.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script src="/resource/library/data-tables/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="/resource/library/data-tables/DT_bootstrap.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-datepicker.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script src="../resource/library/bootstrap/js/bootstrap-dropdown.js"></script>

    <script src="../resource/library/highcharts/highcharts.js"></script>
    <script src="../resource/library/highcharts/exporting.js"></script>
    <script src="../resource/library/tableTools/js/ZeroClipboard.js"></script>
    <script src="../resource/library/tableTools/js/TableTools.min.js"></script>

    <script src="/resource/library/chosen-bootstrap/chosen/chosen.jquery.js"></script>

    <script src="commFactor/js/highcharDoubleAxis.js"></script>
    
    <script src="resource/js/highchartTemplate2.js"></script>
    <script src="dataCorrelation/double.js"></script>
    <script src="data/js/bootstrap.min.js"></script>
    <script src="data/js/bootstrap-datetimepicker.js"></script>    
    <script src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>

    <script src="dataCorrelation/factorCorrelation.js"></script>
    <script>
        jQuery(document).ready(function () {
            // initiate layout and plugins
            App.setPage("other");
            App.init();
        });
    </script>
    <script>
     
    </script>
  <%--  <script>
        var data = [{ "sensorid": 159, "location": "十四标边坡第三级平台中间", "columns": ["温度Temp-1", "湿度Wet-1"], "unit": ["°C", "%RH"], "data": [{ "value": [29.12, 92.6], "acquisitiontime": "\/Date(1404922324000+0800)\/" }, { "value": [29.08, 92.6], "acquisitiontime": "\/Date(1404924124000+0800)\/" }, { "value": [29.00, 93.8], "acquisitiontime": "\/Date(1404925924000+0800)\/" }, { "value": [28.76, 95.1], "acquisitiontime": "\/Date(1404927724000+0800)\/" }, { "value": [28.68, 95.7], "acquisitiontime": "\/Date(1404929524000+0800)\/" }, { "value": [28.56, 95.7], "acquisitiontime": "\/Date(1404931324000+0800)\/" }, { "value": [28.40, 95.0], "acquisitiontime": "\/Date(1404933124000+0800)\/" }, { "value": [28.56, 95.0], "acquisitiontime": "\/Date(1404934924000+0800)\/" }, { "value": [28.32, 95.7], "acquisitiontime": "\/Date(1404936724000+0800)\/" }, { "value": [28.20, 96.3], "acquisitiontime": "\/Date(1404938524000+0800)\/" }, { "value": [28.08, 97.6], "acquisitiontime": "\/Date(1404940324000+0800)\/" }, { "value": [27.96, 98.2], "acquisitiontime": "\/Date(1404942124000+0800)\/" }, { "value": [27.84, 98.2], "acquisitiontime": "\/Date(1404943924000+0800)\/" }, { "value": [27.96, 98.2], "acquisitiontime": "\/Date(1404945724000+0800)\/" }, { "value": [28.40, 97.6], "acquisitiontime": "\/Date(1404947524000+0800)\/" }, { "value": [29.08, 95.8], "acquisitiontime": "\/Date(1404949324000+0800)\/" }, { "value": [29.84, 92.7], "acquisitiontime": "\/Date(1404951124000+0800)\/" }, { "value": [31.12, 88.4], "acquisitiontime": "\/Date(1404952924000+0800)\/" }, { "value": [31.88, 85.9], "acquisitiontime": "\/Date(1404954724000+0800)\/" }, { "value": [35.88, 71.4], "acquisitiontime": "\/Date(1404958017000+0800)\/" }, { "value": [36.44, 68.3], "acquisitiontime": "\/Date(1404958827000+0800)\/" }, { "value": [37.44, 63.8], "acquisitiontime": "\/Date(1404960627000+0800)\/" }, { "value": [38.80, 60.0], "acquisitiontime": "\/Date(1404962427000+0800)\/" }, { "value": [39.52, 56.7], "acquisitiontime": "\/Date(1404964227000+0800)\/" }, { "value": [40.04, 55.5], "acquisitiontime": "\/Date(1404966027000+0800)\/" }, { "value": [40.52, 54.9], "acquisitiontime": "\/Date(1404969320000+0800)\/" }, { "value": [41.32, 53.0], "acquisitiontime": "\/Date(1404970204000+0800)\/" }, { "value": [41.52, 51.0], "acquisitiontime": "\/Date(1404972004000+0800)\/" }, { "value": [41.68, 47.0], "acquisitiontime": "\/Date(1404973804000+0800)\/" }, { "value": [39.64, 52.1], "acquisitiontime": "\/Date(1404975604000+0800)\/" }, { "value": [37.36, 60.4], "acquisitiontime": "\/Date(1404977404000+0800)\/" }, { "value": [37.96, 59.8], "acquisitiontime": "\/Date(1404979204000+0800)\/" }, { "value": [37.44, 61.8], "acquisitiontime": "\/Date(1404981004000+0800)\/" }, { "value": [36.32, 64.3], "acquisitiontime": "\/Date(1404982804000+0800)\/" }, { "value": [35.60, 67.5], "acquisitiontime": "\/Date(1404984604000+0800)\/" }, { "value": [35.16, 69.4], "acquisitiontime": "\/Date(1404986404000+0800)\/" }, { "value": [34.28, 73.2], "acquisitiontime": "\/Date(1404988204000+0800)\/" }, { "value": [33.28, 77.6], "acquisitiontime": "\/Date(1404990004000+0800)\/" }, { "value": [32.44, 81.4], "acquisitiontime": "\/Date(1404991804000+0800)\/" }, { "value": [32.00, 83.3], "acquisitiontime": "\/Date(1404993604000+0800)\/" }, { "value": [31.72, 86.5], "acquisitiontime": "\/Date(1404995404000+0800)\/" }, { "value": [31.36, 88.4], "acquisitiontime": "\/Date(1404997204000+0800)\/" }, { "value": [31.16, 89.1], "acquisitiontime": "\/Date(1404999004000+0800)\/" }, { "value": [30.80, 90.9], "acquisitiontime": "\/Date(1405000804000+0800)\/" }, { "value": [30.52, 94.8], "acquisitiontime": "\/Date(1405002604000+0800)\/" }, { "value": [30.16, 95.4], "acquisitiontime": "\/Date(1405004404000+0800)\/" }, { "value": [29.92, 96.0], "acquisitiontime": "\/Date(1405006204000+0800)\/" }, { "value": [29.64, 96.6], "acquisitiontime": "\/Date(1405008004000+0800)\/" }, { "value": [29.40, 97.8], "acquisitiontime": "\/Date(1405009804000+0800)\/" }, { "value": [29.04, 98.4], "acquisitiontime": "\/Date(1405011604000+0800)\/" }, { "value": [28.76, 99.6], "acquisitiontime": "\/Date(1405013404000+0800)\/" }, { "value": [28.52, 100.0], "acquisitiontime": "\/Date(1405015204000+0800)\/" }, { "value": [28.28, 100.0], "acquisitiontime": "\/Date(1405017004000+0800)\/" }, { "value": [28.16, 100.0], "acquisitiontime": "\/Date(1405018804000+0800)\/" }, { "value": [28.04, 100.0], "acquisitiontime": "\/Date(1405020604000+0800)\/" }, { "value": [27.96, 100.0], "acquisitiontime": "\/Date(1405022404000+0800)\/" }, { "value": [27.72, 100.0], "acquisitiontime": "\/Date(1405024204000+0800)\/" }, { "value": [27.68, 100.0], "acquisitiontime": "\/Date(1405026004000+0800)\/" }, { "value": [27.76, 100.0], "acquisitiontime": "\/Date(1405027804000+0800)\/" }, { "value": [27.80, 100.0], "acquisitiontime": "\/Date(1405029604000+0800)\/" }, { "value": [27.72, 100.0], "acquisitiontime": "\/Date(1405031404000+0800)\/" }, { "value": [28.08, 99.5], "acquisitiontime": "\/Date(1405033204000+0800)\/" }, { "value": [28.96, 97.1], "acquisitiontime": "\/Date(1405035004000+0800)\/" }, { "value": [30.80, 91.6], "acquisitiontime": "\/Date(1405036804000+0800)\/" }, { "value": [32.80, 84.1], "acquisitiontime": "\/Date(1405038604000+0800)\/" }, { "value": [34.48, 77.8], "acquisitiontime": "\/Date(1405040404000+0800)\/" }, { "value": [37.12, 68.3], "acquisitiontime": "\/Date(1405043209000+0800)\/" }, { "value": [37.76, 65.8], "acquisitiontime": "\/Date(1405044170000+0800)\/" }] }];


        $(function () {
            $('#dataContact3').addClass('active');

            structShow(getCookie("nowStructId"));

            $('.selectpicker').selectpicker();

            $('#sensorList1').val('1');

            $('#sensorList1').chosen({
                no_results_text: "没有找到",
                allow_single_de: true
            });

            $('#sensorList2').val('1');

            $('#sensorList2').chosen({
                no_results_text: "没有找到",
                allow_single_de: true
            });

            var doubleAxis = twoAxis(data);
            var seriesData = series(data);
            var tableValus = seriesData.tableValues;
            var columns = data[0].columns;
            var unit = data[0].unit;
            comm_chart = createHighchartComm1('comm1', '温湿度关联趋势图', doubleAxis, seriesData.dataSeries, 1);
            var chart = new Highcharts.Chart(comm_chart);


                        <option value="1">温度</option>
                        <option value="2">降雨量</option>
                        <option value="3">地下水位</option>
                        <option value="4">应变</option>
                        <option value="5">应力</option>
                        <option value="6">风速风向</option>
                        <option value="7">湿度</option>
                        <option value="8">伸缩缝</option>
                        <option value="9">渗流</option>
                        <option value="10">表面位移</option>
                        <option value="11">索力</option>



            $('#factorList1').change(function () {
                var a = parseInt($(this).val());
                var option = '';
                switch (a) {
                    case 1:
                        option += '<option>湿度</option>';
                        option += '<option>伸缩缝</option>';
                        break;
                    case 2:
                        option += '<option>地下水位</option>';
                        option += '<option>渗流</option>';
                        option += '<option>表面位移</option>';
                        break;
                    case 3:
                    case 9:
                    case 10:
                        option += '<option>降雨量</option>';
                        break;
                    case 4:
                        option += '<option>应力</option>';
                        break;
                    case 5:
                        option += '<option>应变</option>';
                        break;
                    case 6:
                        option += '<option>索力</option>';
                        $('#comm1').highcharts({
                            xAxis: {
                                categories: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月']
                            },
                            yAxis: [{
                                lineWidth: 1,
                                title: {
                                    text: '风速(m/s)'
                                }
                            }, {
                                title: {
                                    text: '风向(°)'
                                },
                                lineWidth: 1,
                                opposite: true
                            }, {
                                title: {
                                    text: '索力(kN)'
                                },
                                lineWidth: 1,
                                opposite: true
                            }],
                            series: [{
                                data: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                                name: '风速',
                                yAxis: 0
                            }, {
                                data: [4, 1, 5, 8, 7, 10, 13, 11, 11],
                                yAxis: 1,
                                name: '风向'
                            }, {
                                data: [9, 10, 11, 12, 13, 14, 15, 16, 17],
                                step: 'left',
                                yAxis: 2,
                                name: '索力'
                            }]

                        });
                        break;
                    case 7:
                    case 8:
                        option += '<option>温度</option>';
                        break;
                    case 11:
                        option += '<option>风速风向</option>';
                        break;                 
                }
                $('#factorList2').html(option);
                $('#factorList2').selectpicker('refresh');
            })
        })

      
    </script>--%>
</asp:Content>
