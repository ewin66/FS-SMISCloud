<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="windRoseTheme.aspx.cs" Inherits="SecureCloud.commFactor.windRoseTheme" %>

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

    <script src="../resource/js/jquery-1.8.3.min.js"></script>
    <script src="../commFactor/js/windRoseTheme.js"></script>
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
                <div class="box-header">
                    <div class="box-icon">
                        <b>图形展示:</b>
                        <select id="style" name="style" class="width100">
                            <option value="windRose">风玫瑰图</option>
                            <option value="windCurve">风曲线图</option>
                        </select>
                        
                    </div>
                </div>
                <div class="box-content">
                    <div class="row-fluid">
                        <div class="form-horizontal">
                            <div class="box-content" id="comm1_graph">
                                <div id="iframe">
                                    <iframe id="ifm" name="ifm" frameborder="0" scrolling="no" width="100%" onload="iframeAutoFit(this)"></iframe>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

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

 <%--   <script src="../resource/js/app.js"></script>
    <script src="../resource/js/securecloud.js"></script>--%>
   
<%--    <script src="../resource/library/highcharts/highcharts.js"></script>
    <script src="../resource/library/highcharts/exporting.js"></script>
    <script src="../resource/library/tableTools/js/ZeroClipboard.js"></script>
    <script src="../resource/library/tableTools/js/TableTools.min.js"></script>--%>

<%--    <script src="../resource/js/datepickerInit.js"></script>--%>
<%--    <script src="../resource/js/dataTableInit.js"></script>--%>

<%--    <script src="../commFactor/js/dataParse.js"></script>--%>

</body>
</html>