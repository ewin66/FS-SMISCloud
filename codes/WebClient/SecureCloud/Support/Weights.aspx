<%@ Page Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="Weights.aspx.cs" Inherits="SecureCloud.Support.Weights" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/resource/css/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
    <link href="/resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <h3 class="page-title"><span>组织—</span>

                <small class="dropdown" style="display: inline-block;">
                    <a href="javascript:;" class="dropdown-toggle" data-toggle="dropdown"><span id="spanOrg"><i class="icon-angle-down"></i></span>
                    </a>
                    <ul id="OrgDropdownList" class="dropdown-menu" style="height:200px;overflow:auto">
                        <%--<li><a href="#">澜沧江大桥 </a></li>
                        <li class="divider"></li>
                        <li><a href="#">于家宝</a></li>--%>
                    </ul>
                </small>
                <span>结构物—</span>

                <small class="dropdown" style="display: inline-block;">
                    <a href="javascript:;" class="dropdown-toggle" data-toggle="dropdown"><span id="spanStruct"><i class="icon-angle-down"></i></span>
                    </a>
                    <ul id="StructDropdownList" class="dropdown-menu" style="height:200px;overflow:auto">
                        <%--<li><a href="/Support/Weights.aspx?orgId=1&structId=2">K775边坡</a></li>
                        <li class="divider"></li>
                        <li><a href="#">K791边坡</a></li>--%>
                    </ul>
                </small>
            </h3>
        </div>
        <p>当前结构物权重配置总进度 (<span class="barAllText">0</span>%)</p>
        <div class="progress" style="width: 80%;">
            <div id="barAll" class="bar" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width: 0%">
                <span class="sr-only barAllText">0</span>% (已完成)
            </div>
        </div>
        <div class="box">
            <div class="box-header">
                <div class="box-icon">
                    权重管理
                </div>
            </div>
            <div class="box-content">
                <div class="tabbable tabbable-custom">
                    <ul class="nav nav-tabs">
                        <li id="ThemeLi" class="active"><a href="#tabTheme" data-toggle="tab">主题权重配置</a></li>
                        <li id="Sub-factorsLi"><a href="#tabSub-factors" data-toggle="tab">子因素权重配置</a></li>
                        <li id="SensorsLi"><a href="#tabSensors" data-toggle="tab">传感器权重配置</a></li>
                    </ul>
                    <div class="tab-content">
                        <div class="tab-pane active" id="tabTheme">
                            <p>已配置权重百分比 (<span class="barThemeText">0</span>%)</p>
                            <div class="progress" style="width: 80%;">
                                <div id="barTheme" class="bar" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width: 0%">
                                    <span class="sr-only barThemeText">0</span>% (已配置)
                                </div>
                            </div>
                            <div class="box">
                                <div class="box-content">
                                    <div id="ThemeContent" class="form-horizontal">
                                        <%--<div class="control-group">
                                            <label for="amountTheme1">环境主题权重:</label>
                                            <div class="span2">
                                                <input type="text" id="amountTheme1" style="width: 50px;" />
                                            </div>
                                            <div class="span6">
                                                <div id="sliderTheme1" style="height: 12px"></div>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label for="amountTheme2">变形主题权重:</label>
                                            <div class="span2">
                                                <input type="text" id="amountTheme2" style="width: 50px;" />
                                            </div>
                                            <div class="span6">
                                                <div id="sliderTheme2" style="height: 12px"></div>
                                            </div>
                                        </div>--%>
                                    </div>
                                </div>
                                <div style="float: right;">
                                    <input id="btnSaveTheme" type="button" class="btn green" value="保存配置" />
                                    <input id="btnSaveNextTheme" type="button" class="btn green" value="保存配置并下一步" />
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabSub-factors">
                            <p>已配置权重百分比 (<span class="barSub-factorsText">0</span>%)</p>
                            <div class="progress" style="width: 80%;">
                                <div id="barSub-factors" class="bar" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width: 0%">
                                    <span class="sr-only barSub-factorsText">0</span>% (已配置)
                                </div>
                            </div>
                            <div class="box">
                                <div class="box-content">
                                    <div class="form-horizontal">
                                        <div class="control-group">
                                            <b>选择主题:</b>
                                            <select id="ThemeList-Sub-factors" name='sensorList' class="selectpicker" data-size="4" style="width: 300px;" title="请选择">
                                                <%--<option value="1">环境</option>
                                                <option value="2">变形</option>--%>
                                            </select>
                                        </div>
                                        <div id="Sub-factorsContent">
                                            <%--<div class="control-group">
                                                <label for="amount">表面位移权重:</label>
                                                <div class="span2">
                                                    <input type="text" id="Text1" style="width: 50px;" />
                                                </div>
                                                <div class="span6">
                                                    <div id="v-slider" style="height: 12px"></div>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label for="amount">内部位移权重:</label>
                                                <div class="span2">
                                                    <input type="text" id="Text2" style="width: 50px;" />
                                                </div>
                                                <div class="span6">
                                                    <div id="v-slider2" style="height: 12px"></div>
                                                </div>
                                            </div>--%>
                                        </div>
                                    </div>
                                </div>
                                <div style="float: right;">
                                    <input id="btnSaveSub-factors" type="button" class="btn green" value="保存配置" />
                                    <input id="btnSaveNextSub-factors" type="button" class="btn green" value="保存配置并下一步" />
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="tabSensors">
                            <p>已配置权重百分比 (<span class="barSensorsText">0</span>%)</p>
                            <div class="progress" style="width: 80%;">
                                <div id="barSensors" class="bar" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width: 0%">
                                    <span class="sr-only barSensorsText">0</span>% (已配置)
                                </div>
                            </div>
                            <div class="box">
                                <div class="box-content">
                                    <div class="form-horizontal">
                                        <div class="control-group">
                                            <b>选择主题:</b>
                                            <select id="ThemeList-Sensors" name='sensorList' class="selectpicker" data-size="4" style="width: 300px;" title="请选择">
                                                <%--<option value="1">变形</option>
                                                <option value="2">环境</option>--%>
                                            </select>
                                            <b>选择子因素:</b>
                                            <select id="Sub-factorsList-Sensors" name='sensorList2' class="selectpicker" data-size="4" style="width: 300px;" title="请选择">
                                                <%--<option value="1">表面位移</option>
                                                <option value="2">内部位移</option>--%>
                                            </select>
                                        </div>
                                        <div id="SensorsContent">
                                            <%--<div class="control-group">
                                                <label for="amount">传感器1权重:</label>
                                                <div class="span2">
                                                    <input type="text" id="Text3" style="width: 50px;" />
                                                </div>
                                                <div class="span6">
                                                    <div id="Div2" style="height: 12px"></div>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label for="amount">传感器2权重:</label>
                                                <div class="span2">
                                                    <input type="text" id="Text4" style="width: 50px;" />
                                                </div>
                                                <div class="span6">
                                                    <div id="Div3" style="height: 12px"></div>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label for="amount">传感器3权重:</label>
                                                <div class="span2">
                                                    <input type="text" id="amount3" style="width: 50px;" />
                                                </div>
                                                <div class="span6">
                                                    <div id="v-slider3" style="height: 12px"></div>
                                                </div>
                                            </div>--%>
                                        </div>
                                    </div>
                                </div>
                                <div style="float: right;">
                                    <input id="btnSaveSensors" type="button" class="btn green" value="保存配置" />
                                    <input id="btnSaveNextSensors" type="button" class="btn green" value="保存配置并下一步" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script type="text/javascript" src="/resource/js/jquery-2.0.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script src="/resource/js/jquery-ui-1.10.0.custom.min.js"></script>
    <script src="/resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script src="/Support/js/Weights.js"></script>

</asp:Content>
