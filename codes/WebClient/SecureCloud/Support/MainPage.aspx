<%@ Page Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="MainPage.aspx.cs" Inherits="SecureCloud.Support.MainPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../resource/library/data-tables/DT_bootstrap.css" rel="stylesheet" />
    <link href="../resource/library/data-tables/css/dataTable.css" rel="stylesheet" />
    <%--<link href="../resource/library/tableTools/css/TableTools.css" rel="stylesheet" />--%>
    
    <link href="css/mainPage.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <!-- BEGIN PAGE HEADER-->
        <div class="row-fluid">
            <ul class="breadcrumb" style="margin-top: 10px;">
                <li>
                    <i class="fa fa-home"></i>
                    <a href="MainPage.aspx">主页</a>
                </li>
            </ul>
        </div>
        <!-- part1: 项目仪表盘 -->
        <div class="row-fluid">
            <div class="portlet">
                <div class="portlet-title line">
                    <h4><i class="fa fa-tachometer"></i>项目仪表盘<span id="title-ProjectDashboard" class="fontSize16"></span></h4>
                    <div class="tools">
                        <a href="javascript:;" class="collapse"></a>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="row-fluid">
                        <div id="loading-projectDashboard" class="fixed-marker-loading"></div>
                        <div id="projectDashboard" class="span12" style="margin-left: 0;">
                            <div id="container-userProjectsStatus" class="span12">
                                <div class="scroller" data-height="500" data-always-visible="0" data-rail-visible1="1">
                                    <div id="chart-userProjectsStatus"></div>
                                </div>
                            </div>
                            <div id="container-projectStructsStatus" class="span6" style="display: none;">
                                <div class="box" style="height: 498px;">
                                    <div class="box-header">
                                        <div id="title-projectStructsStatus" class="fontWeightBold position-left-center"></div>
                                    </div>
                                    <div class="box-content" style="overflow: auto;">
                                        <div class="marginBottom10">
                                            <span id="category-abnormalStruct" class="category-struct">存在异常的结构物</span>&nbsp;&nbsp;|&nbsp;&nbsp;
                                            <span id="category-allStruct" class="like-a category-struct">全部</span>&nbsp;&nbsp;|&nbsp;&nbsp; 
                                            <span id="category-normalStruct" class="like-a category-struct">正常的结构物</span>
                                        </div>
                                        <div class="scroller" data-height="420" data-always-visible="0" data-rail-visible1="1">
                                            <table id="table-projectStructsStatus" class="table table-striped table-bordered table-hover">
                                                <thead>
                                                    <tr>
                                                        <th style="width: 32%;">结构物</th>
                                                        <th style="width: 13%;" title="结构物的状态">状态</th>
                                                        <th style="width: 20%;" title="最近24小时技术支持及客户未确认的告警总数">最近24小时未确认告警数</th>
                                                        <th style="width: 18%;" title="当前离线或者从未上线DTU的个数">不在线DTU个数</th>
                                                        <th title="在线DTU下当前异常传感器的个数">异常传感器个数</th>
                                                    </tr>
                                                </thead>
                                                <tbody id="tbody-projectStructsStatus"></tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%--/div#container-projectStructsStatus--%>
                        </div>
                    </div>
                    <%--/div.row-fluid--%>
                </div>
            </div>
        </div>
        <!-- end part1: 项目仪表盘 -->
        <!-- part2: 结构物状态 -->
        <div class="row-fluid">
            <div id="part-structStatus" class="portlet display-none">
                <div class="portlet-title line">
                    <h4>
                        <i class="fa fa-road"></i>
                        <span id="title-structStatus-projectName">项目简称</span>
                        <i class="fa fa-angle-right"></i>
                        <span id="title-structStatus-structName">结构物名称</span>
                    </h4>
                    <div class="tools">
                        <a href="javascript:;" class="collapse"></a>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="accordion" id="accordion-structStatus">
                        <div class="accordion-group">
                            <div class="accordion-heading">
                                <a id="aStructAlarm" class="accordion-toggle collapsed accordion-toggle-struct" data-toggle="collapse" data-parent="#accordion-structStatus" href="#collapse-alarm">
                                    <i class="fa fa-bell"></i>
                                    <span>最近24小时未确认告警<span id="title-accordion-alarm" class="fontSize14"></span></span>
                                    <i class="fa fa-angle-up position-absolute-right accordion-icon-struct"></i>
                                </a>
                            </div>
                            <div id="collapse-alarm" class="collapse">
                                <div class="accordion-inner">
                                    <div id="chart-alarmStatus"></div>
                                </div>
                            </div>
                        </div>
                        <div class="accordion-group">
                            <div class="accordion-heading">
                                <a id="aStructDtu" class="accordion-toggle collapsed accordion-toggle-struct" data-toggle="collapse" data-parent="#accordion-structStatus" href="#collapse-dtu">
                                    <i class="fa fa-signal"></i>
                                    <span>DTU状态<span id="title-accordion-dtu" class="fontSize14"></span></span>
                                    <i class="fa fa-angle-up position-absolute-right accordion-icon-struct"></i>
                                </a>
                            </div>
                            <div id="collapse-dtu" class="collapse">
                                <div class="accordion-inner">
                                    <div class="marginBottom10">
                                        <span id="category-offlineDtu" class="category-dtu">离线DTU</span>&nbsp;&nbsp;|&nbsp;&nbsp;
                                        <span id="category-neverUplineDtu" class="like-a category-dtu">从未上线DTU</span>&nbsp;&nbsp;|&nbsp;&nbsp;
                                        <span id="category-allDtu" class="like-a category-dtu">全部</span>&nbsp;&nbsp;|&nbsp;&nbsp; 
                                        <span id="category-onlineDtu" class="like-a category-dtu">在线DTU</span>
                                    </div>
                                    <%-- 结构物下DTU最新状态列表 --%>
                                    <div class="accordion" id="accordion-listDtuStatus">
                                        <div class="accordion-group">
                                            <div class="accordion-heading">
                                                <a class="accordion-toggle collapsed accordion-toggle-dtu" data-toggle="collapse" data-parent="#accordion-listDtuStatus" href="#collapse-dtu-1">
                                                    <i class="fa fa-thumb-tack"></i>
                                                    <span>DTU当前状态1</span>
                                                    <i class="fa fa-angle-up position-absolute-right accordion-icon-dtu"></i>
                                                </a>
                                            </div>
                                            <div id="collapse-dtu-1" class="collapse display-none" style="height: auto;">
                                            </div>
                                        </div>
                                        <div class="accordion-group">
                                            <div class="accordion-heading">
                                                <a class="accordion-toggle collapsed accordion-toggle-dtu" data-toggle="collapse" data-parent="#accordion-listDtuStatus" href="#collapse-dtu-2">
                                                    <i class="fa fa-thumb-tack"></i>
                                                    <span>DTU当前状态2</span>
                                                    <i class="fa fa-angle-up position-absolute-right accordion-icon-dtu"></i>
                                                </a>
                                            </div>
                                            <div id="collapse-dtu-2" class="collapse display-none" style="height: auto;">
                                            </div>
                                        </div>
                                    </div>
                                    <%-- end 结构物下DTU最新状态列表 --%>
                                </div>
                            </div>
                        </div>
                        <%--start 传感器状态--%>
                        <div class="accordion-group">
                            <div class="accordion-heading">
                                <a id="aStructSensor" class="accordion-toggle collapsed accordion-toggle-struct" data-toggle="collapse" data-parent="#accordion-structStatus" href="#collapse-sensor">
                                    <i class="fa fa-bar-chart"></i>
                                    <span id="startLook">传感器状态<span id="title-accordion-sensor" class="fontSize14"></span></span>
                                    <i class="fa fa-angle-up position-absolute-right accordion-icon-struct"></i>
                                </a>
                            </div>

                            <div id="collapse-sensor" class="collapse">
                                <div class="accordion-inner">
                                    <div class="portlet-title line">
                                        <span class="fontSize14"><i class="fa fa-bars "></i>传感器采集的最新状态统计</span>
                                         <i class="fa fa-angle-down position-absolute-right" id="load1"></i>
                                    </div>
                                    <div class="portlet-body" id="modfy1" >
                                          <span id="alertInfor" style="color: red;"></span>
                                        <div class="row-fluid display-none" id="allInfor">
                                            <%--<div id="loading-sensorDashboard" class="marker-loading"></div>--%>
                                            <div id="sensorDashboard" class="span12" style="margin-left: 0;">
                                                <div id="loadChart" class="span12">
                                                    <div id="chart-sensorDashboard" style="height: 400px;"></div>
                                                </div>
                                                <div id="sensorInformatons" class="span6 display-none">
                                                    <div class="box" style="height: 398px;">
                                                        <div class="box-header">
                                                            <div id="sensorPostionInformation" class="fontWeightBold position-left-center"></div>
                                                        </div>
                                                        <div class="box-content">
                                                            <div class="scroller" data-height="320" data-always-visible="0" data-rail-visible1="1">
                                                            <table id="tableSensorsInformaton" class="table table-striped table-bordered table-hover">
                                                                <thead>
                                                                    <tr>
                                                                        <th style="width: 50%;">传感器位置</th>
                                                                        <th style="width: 50%;">采集时间</th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody id="tbodySensorsInformaton">
                                                                </tbody>
                                                            </table>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="oneChart" class="row-fluid display-none">
                                            <div class="portlet-title line">
                                                <span class="fontSize14"><i class="fa fa-credit-card"></i>传感器最新信息</span>
                                                <i class="fa fa-angle-down position-absolute-right" id="load2"></i>
                                            </div>
                                            <div class="box-content" id="alertInformation" >
                                                <table id="tableOneSensorInformation" class="table table-striped table-bordered table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 20%;">传感器位置</th>
                                                            <th style="width: 5%;">归属DTU</th>
                                                            <th style="width: 5%;">DTU状态</th>
                                                            <th style="width: 5%;">模块号</th>
                                                            <th style="width: 5%;">通道号</th>
                                                            <th style="width: 20%;">上次采集状态</th>
                                                             <th style="width: 5%;">传感器状态</th>
                                                            <th style="width: 15%;">操作</th>
                                                            <th style="width: 20%;">即时采集结果</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody id="tbodyOneSensorInformation"></tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="row-fluid display-none"  id="lastTime">
                                            <div class="portlet-title line">
                                                <span class="fontSize14"><i class="fa fa-clock-o"></i>最近24小时异常</span>
                                                <i class="fa fa-angle-down position-absolute-right" id="load3"></i>
                                            </div>
                                            <div class="portlet-body"  id="modfy">
                                                <span id="alertOneInfor" style="color: red;"></span>
                                                <div class="row-fluid" id="allOneInfor">
                                                    <%--<div id="oneSensorChart" class="marker-loading"></div>--%>
                                                    <div id="sensorChart" class="span12" style="margin-left: 0;">
                                                        <div id="loadSensorChart" class="span12">
                                                            <div id="sensor-Chart" style="height: 400px;"></div>
                                                        </div>
                                                        <div id="sensorListInformation" class="span6 display-none" >
                                                            <div class="box" style="height: 398px;">
                                                                <div class="box-header">
                                                                    <div id="sensor-ListHeader" class="fontWeightBold position-left-center"></div>
                                                                </div>
                                                                <div class="box-content">
                                                                    <div class="scroller" data-height="320" data-always-visible="0" data-rail-visible1="1">
                                                                    <table id="tableSensorListInformation" class="table table-striped table-bordered table-hover">
                                                                        <thead>
                                                                            <tr>
                                                                                <th style="width: 50%;">传感器位置</th>
                                                                                <th style="width: 50%;">采集时间</th>
                                                                            </tr>
                                                                        </thead>
                                                                        <tbody id="tbodySensorListInformation">
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
                                    </div>
                                </div>
                                <%--/div#container-projectStructsStatus--%>
                            </div>
                        </div>
                        <%--end 传感器状态-- %>
                    <%--/div.row-fluid--%>
                    </div>
                </div>
            </div>
        </div>
        <!-- end part2: 结构物状态 -->
    </div>
    
    <%-- 查看DTU基本信息 Modal --%>
    <div id="viewDtuModal" class="modal hide fade" style="width: 800px;">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3><span id="viewDtuModal-dtuNo"></span> DTU基本信息</h3>
        </div>
        <div class="modal-body">
            <div id="prompt-noData-dtuInfo"></div>
            <table id="table-dtuInfo" class="table table-striped table-bordered" style="width: 100%;">
                <thead>
                    <tr>
                        <th>厂家</th>
                        <th>型号</th>
                        <th>网络类型</th>
                        <th>SIM卡号</th>
                        <th>采集粒度(分钟)</th>
                        <th>工作模式</th>
                    </tr>
                </thead>
                <tbody id="tbody1-dtuInfo"></tbody>
                <thead>
                    <tr>
                        <th>主数据中心IP</th>
                        <th>主数据中心端口</th>
                        <th>副数据中心IP</th>
                        <th>副数据中心端口</th>
                        <th>封包间隔(ms)</th>
                        <th>重连次数</th>
                    </tr>
                </thead>
                <tbody id="tbody2-dtuInfo"></tbody>
            </table>
        </div>
        <div class="modal-footer">
            <button class="btn blue" data-dismiss="modal">关闭</button>
        </div>
    </div>
    <%-- end 查看DTU基本信息 Modal --%>

    <!-- 修改DTU远程配置 Modal -->
    <div id="modifyDtuRemoteConfig" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>修改DTU远程配置</h3>
        </div>
        <div class="modal-body" style="max-height: 460px;">
            <div class="form-horizontal">
                <div class="control-group">
                    <label class="control-label">DTU编号</label>
                    <div class="controls">
                        <input id="modifyDtu" name="modifyDtu" type="text" readonly="readonly" />
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">数据中心IP1</label>
                    <div class="controls">
                        <input id="modifyIp1" name="modifyIp1" type="text" pattern="^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$" title="有效的IP地址" placeholder="有效的IP地址" />
                        <span style="color: red">*</span>
                        <span id="resultOfModifyIp1" class="position-absolute-right-0"></span>
                        <p id="modifyIp1Range" class="modifyRange" style="display: none;"><span style="color: red;">请保证有效的IP地址</span></p>
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">端口Port1</label>
                    <div class="controls">
                        <input id="modifyPort1" name="modifyPort1" type="text" pattern="^[1-9]\d{3}|[1-5]\d{4}|6[0-5]{2}[0-3][0-5]$" title="要求数字" placeholder="要求数字" maxlength="5" />
                        <span style="color: red">*</span>
                        <span id="resultOfModifyPort1" class="position-absolute-right-0"></span>
                        <p id="modifyPort1Range" class="modifyRange" style="display: none;"><span style="color: red;">请注意端口号在1000-65535之间</span></p>
                    </div>
                </div> 
                <div class="control-group">
                    <label class="control-label">数据中心IP2</label>
                    <div class="controls">
                        <input id="modifyIp2" name="modifyIp2" type="text" pattern="^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$" title="有效的IP地址" placeholder="有效的IP地址" />
                        <span style="color: red">*</span>
                        <span id="resultOfModifyIp2" class="position-absolute-right-0"></span>
                        <p id="modifyIp2Range" class="modifyRange" style="display: none;"><span style="color: red;">请保证有效的IP地址</span></p>
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">端口Port2</label>
                    <div class="controls">
                        <input id="modifyPort2" name="modifyPort2" type="text" pattern="^[1-9]\d{3}|[1-5]\d{4}|6[0-5]{2}[0-3][0-5]$" title="要求数字" placeholder="要求数字" maxlength="5" />
                        <span style="color: red">*</span>
                        <span id="resultOfModifyPort2" class="position-absolute-right-0"></span>
                        <p id="modifyPort2Range" class="modifyRange" style="display: none;"><span style="color: red;">请注意端口号在1000-65535之间</span></p>
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">工作模式</label>
                    <div class="controls">
                        <select id="modifyDtuMode" name="modifyDtuMode">
                            <option value="TCP" selected="selected">TCP</option>
                            <option value="TRNS">TRNS</option>
                            <option value="UDP">UDP</option>
                        </select>
                        <span id="resultOfModifyDtuMode" class="position-absolute-right-0"></span>
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">封包间隔时间</label>
                    <div class="controls">
                        <input id="modifyPacketInterval" name="modifyPacketInterval" type="text" pattern="^(\d)+$" title="数字" placeholder="要求数字" />
                        <span>ms</span>
                        <span style="color: red">*</span>
                        <span id="resultOfModifyPacketInterval" class="position-absolute-right-0"></span>
                        <p id="modifyPacketIntervalRange" class="modifyRange" style="display: none;"><span style="color: red;">请注意封包间隔时间不为空，且为数字</span></p>
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">重连次数</label>
                    <div class="controls">
                        <input id="modifyReconnectionCount" name="modifyReconnectionCount" type="text" pattern="^(\d)+$" title="数字" placeholder="要求数字" />
                        <span style="color: red">*</span>
                        <span id="resultOfModifyReconnectionCount" class="position-absolute-right-0"></span>
                        <p id="modifyReconnectionCountRange" class="modifyRange" style="display: none;"><span style="color: red;">请注意重连次数不为空，且为数字</span></p>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <div style="text-align: left; font-size: 16px; margin-bottom: 16px;">
                <span id="promptDtuRemoteConfig" style="font-weight: bold;"></span>
                <span id="timeContainerDtuRemoteConfig" style="display: none; margin-left: 10%;">剩余等待时间: <span id="timeDtuRemoteConfig" style="color: #ff0000"></span>秒</span>
            </div>
            <button id="btnClose" class="btn dark-gray" data-dismiss="modal">关闭</button>
            <input id="btnResetModifyDtuRemoteConfig" type="button" value="重置" class="btn blue" />
            <input id="btnSendModifyDtuRemoteConfig" type="button" value="下发" class="btn red btn-primary" />
        </div>
    </div>
    <!-- end 修改DTU远程配置 Modal -->
    
    <!-- 修改DTU远程配置"下发"确认框 modal -->
    <div id="modalConfirmDtuRemoteConfig" class="modal hide fade" style="top:2%; width:500px; left: 52%">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>确认修改DTU远程配置</h3>
        </div>
        <div class="modal-body">
            <p style="color: #ff0000">如果下发, DTU参数将被修改, 确认下发?</p>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">否</button>
            <a href="javascript:;" id="btnConfirmDtuRemoteConfigSend" class="btn red">是</a>
        </div>
    </div>
    <!-- end 修改DTU远程配置"下发"确认框 modal -->
    
    <!-- DTU"重启"确认框 modal -->
    <div id="modalConfirmDtuRestart" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>确认重启DTU</h3>
        </div>
        <div class="modal-body">
            <p style="color: #ff0000">重启DTU可能无响应, 确认重启?</p>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">否</button>
            <a href="javascript:;" id="btnConfirmDtuRestart" class="btn red">是</a>
        </div>
    </div>
    <!-- end DTU"重启"确认框 modal -->

    <!-- 传感器即时采集"下发"确认框 modal -->
    <div id="modalConfirmRealtime" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>即时采集</h3>
        </div>
        <div class="modal-body">
            <p style="color: #ff0000">确认下发?</p>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">否</button>
            <a href="javascript:;" id="btnConfirmRealtimeSend" class="btn red">是</a>
        </div>
    </div>
    <!-- end 传感器即时采集"下发"确认框 modal -->
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script src="../resource/library/DataTables-1.10.2/js/jquery.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">   
    <script type="text/javascript" src="/resource/library/highcharts/highcharts.js"></script>
    <script type="text/javascript" src="js/highcharts-exporting.js"></script>
    
    <script src="../resource/library/DataTables-1.10.2/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="../resource/library/data-tables/DT_bootstrap.js"></script>
    <%--<script type="text/javascript" src="../resource/library/tableTools/js/TableTools.min.js"></script>--%>
    
    <script type="text/javascript" src="js/chartTemplate.js"></script>
    <script type="text/javascript" src="js/mainPage.js"></script>
    <script type="text/javascript" src="js/dtuRemoteManage.js"></script>
    <script type="text/javascript" src="js/sensorRealtimeAcquisition.js"></script>

    <script src="/resource/js/jquery.number.min.js"></script>

</asp:Content>
