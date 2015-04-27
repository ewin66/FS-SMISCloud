<%@ Page Title="" Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="DataDownload.aspx.cs" Inherits="SecureCloud.Support.OriginalDataDownload" %>
<asp:Content ID="Content1"     ContentPlaceHolderID   ="head" runat="server">
     <link href="../resource/library/data-tables/DT_bootstrap.css" rel="stylesheet" />
     <link href="../resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
     <link href="../resource/library/bootstrap/css/datepicker.css" rel="stylesheet" />


     <link href="../data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />
     <link href="../resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />   
    <style>
        .width206 {
            width: 206px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <ul class="breadcrumb" style="margin-top: 5px;background-color:#AAAAAA;">               
                <h4>
                    <img src="/resource/img/document-spreadsheet.png" style="width: 30px; height: 30px" />
                    <span><b style="font-size:19px;color:white;">数据下载</b></span>
                </h4>               
            </ul>
            <div class="form-horizontal">
                <div class="control-group">
                    <b class="control-label" style="width:50px;">结构物:</b>
                    <div class="controls" style="margin-left: 60px;">
                        <select id="structList" name='Structs' data-size="10" title="请选择"></select>
                        
                        <input id="factorNext" type="button" class="btn green" style="margin-left: 20px" value="数据下载" />
                        
                        <%--<b>监测时间：从</b>
                        <input type="text" id="Text3"/>
                        <span class="add-on">
                            <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                        </span>
                        <b>至</b>
                        <input type="text" id="Text4"/>
                        <span class="add-on">
                            <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                        </span>--%>
                </div>
                <div style="margin-top: 20px">
                    <b>监测时间：从</b>
                    <div id="formTime" class="input-append date">
                        <input type="text" id="dpfrom"/>
                        <span class="add-on">
                            <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                        </span>
                    </div>
                    <b>至</b>
                    <div id="endTime" class="input-append date">
                        <input type="text" id="dpend"/>
                        <span class="add-on">
                            <i data-time-icon="icon-time" data-date-icon="icon-calendar"></i>
                        </span>
                    </div>
                </div>
                <div id ="factorBox" class="box">
                    <div class="box-header">
                        <div class="box-icon">
                            监测因素
                            <span>
                                <input type="button" size="" id="CheckedAll"  value="全 选"/>
                            </span>
                            <span>
                                <input type="button" id="CheckedNo" value="全不选"/>  
                            </span>
                        </div>
                    </div>
                    <div id="factorList" class="box-content"/>
                </div>
            </div>
         </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script src="../resource/js/jquery-1.8.3.min.js"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script type="text/javascript" src="../resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script type="text/javascript" src="../resource/library/bootstrap/js/bootstrap-dropdown.js"></script>
    <script type="text/javascript" src="../resource/library/tableTools/js/ZeroClipboard.js"></script>
    <script type="text/javascript" src="../resource/library/chosen-bootstrap/chosen/chosen.jquery.js"></script>
    <script type="text/javascript" src="../data/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="../data/js/bootstrap-datetimepicker.js"></script> 

    <script type="text/javascript" src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>   
    <script type="text/javascript" src="js/DataDownload.js"></script>
</asp:Content>
