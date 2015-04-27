<%@ Page Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="TopoSettingSVG.aspx.cs" Inherits="SecureCloud.TopoSettingSVG" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />
    <style>
        .ajax-loader {
            position: absolute;
            display: block;
            /* text-indent: -9999px;*/
            width: 100px;
            height: 100px;
            outline: none;
            background: url(/resource/img/ajax-loader.gif) no-repeat;
            cursor: pointer;
            /*padding-bottom: 100px;*/
            z-index: 2;
        }

        .btn {
            padding: 5px 10px;
        }

        #modalConfigHotspot .tooltip-inner {
            text-align: left;
            max-width: 400px;
            margin-left: 180px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row-fluid">
        <div class="box">
            <div class="box-header">
                <div class="box-icon">
                    <h3 class="page-title">
                        <span>热点图动态配置—</span>
                        <span id="mainHeapMapStructName"></span>
                    </h3>
                </div>
            </div>
            <div id="boxContent" class="box-content">
                <div class="row-fluid">
                    <div class="form-horizontal">
                        <button id="btnSelectAll" class="btn gray" disabled="disabled">选择全部</button>
                        <button id="btnCancelSelect" class="btn gray" disabled="disabled">取消选择</button>
                        <button id="btnBatchDelete" class="btn gray" disabled="disabled">删除</button>
                    </div>
                </div>
                <div class="row-fluid">
                    <div class="form-horizontal">
                        <div id="svgContainer" class="span9">
                            <ul id="theDropDownMenu" class="dropdown-menu" role="menu" style="position:absolute;">
                              <li role="presentation"><a role="menuitem" tabindex="1" href="#"></a></li>
                              <li role="presentation"><a role="menuitem" tabindex="2" href="#"></a></li>
                              <li role="presentation"><a role="menuitem" tabindex="3" href="#"></a></li>
                            </ul>
                        </div>
                        <div id='svgLoading' class='ajax-loader' style='display: none;'></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <!-- 添加热点 config modal -->
    <div id="modalConfigHotspot" class="modal hide fade" aria-hidden="true" style="display: none;">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3 id="titleConfigHotspot">新建/编辑热点</h3>
        </div>
        <div class="modal-body" style="max-height: 500px;">
            <div class="form-horizontal">
                <div class="control-group">
                    <label class="control-label">坐标</label>
                    <div class="controls">
                        <input type="text" id="configAxis" name="configAxis" readonly="readonly" />
                        <input type="hidden" id="configHotspotX" />
                        <input type="hidden" id="configHotspotY" />
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">类型</label>
                    <div class="controls">
                        <select id="configHotspotType" name="configHotspotType" class="chzn-select">
                            <option value="1" selected="selected">传感器热点</option>
                            <option value="2">截面热点</option>
                        </select>
                    </div>
                </div>
                <div class="control-group" id="group_configProductType">
                    <label class="control-label">产品类型</label>
                    <div class="controls">
                        <select id="configProductType" name="configProductType" class="chzn-select">
                        </select>
                    </div>
                </div>
                <div class="control-group">
                    <label id="labelSensorOrSection" class="control-label">传感器/截面</label>
                    <div class="controls">
                        <select id="configSensorOrSection" name='configSensorOrSection' class="chzn-select"></select>
                        <%--<select id="configSensorOrSection" name='configSensorOrSection' class="selectpicker" style="width: 220px;" data-size="10" title="请选择"></select>--%>
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label"><span id="labelPath">path</span></label>
                    <div class="controls">
                        <textarea id="configPath" style="width: 206px; height: 206px" maxlength="1000"></textarea>
                        <%--<span style="color: red">*</span>--%>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <button class="btn" id="btnConfigModalCancel" data-dismiss="modal">取消</button>
            <a href="javascript:;" id="btnConfigModalSave" class="btn red">保存</a>
        </div>
    </div>
    <!-- end 添加热点 config modal --> 
    
    <!-- 删除热点 config modal -->
    <div id="modalDeleteHotspot" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>删除热点</h3>
        </div>
        <div class="modal-body">
            <p id="deletionMsg"></p>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">否</button>
            <a href="javascript:;" id="btnDeleteConfirm" class="btn red">是</a>
        </div>
    </div>
    <!-- end 删除热点 config modal --> 
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <%--<script type="text/javascript" src="/resource/js/jquery-2.0.3.min.js"></script>--%>
    <script type="text/javascript" src="/resource/js/jquery-1.8.3.min.js"></script> <%--<此版本支持chosen>--%>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script type="text/javascript" src="/resource/library/chosen-bootstrap/chosen/chosen.jquery.js"></script>
    <script type="text/javascript" src="/resource/js/topoSettingSVG.js"></script>
</asp:Content>
