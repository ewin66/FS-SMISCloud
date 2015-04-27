<%@ Page Title="" Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="SolutionConfig.aspx.cs" Inherits="SecureCloud.Support.SolutionConfig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../resource/library/data-tables/DT_bootstrap.css" rel="stylesheet" />
    <link href="../resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
    <link href="/resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />
    <link href="/resource/library/bootstrap-switch/css/bootstrap-switch.css" rel="stylesheet"/>
    <link href="../data/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />     
    <style>
        table input{
          width:100px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="SolutionConfig" runat="server">
        <div class="container-fluid">
            <div class="row-fluid">
                <div class="portlet box light-grey">
                    <div class="portlet-title">
                        <h4>
                            <img src="/resource/img/settings.png" style="width: 30px; height: 30px" />
                            <span>方案配置管理—</span><span id="spanStructName"></span>
                        </h4>
                    </div>
                    <div class="portlet-body">
                        <div class="row-fluid">
                            <div class="span12">
                                <!--BEGIN TABS-->
                                <div class="tabbable tabbable-custom">
                                    <ul class="nav nav-tabs">
                                        <li id="tabFactor" class="active"><a href="#tab_Factor" data-toggle="tab" style="font-size: 16px">监测因素配置</a></li>
                                        <li id="tabFactorUnit"><a href="#tab_FactorUnit" data-toggle="tab" style="font-size: 16px">监测因素单位配置</a></li>
                                        <li id="tabDtu" style="display: none;"><a href="#tab_DTU" data-toggle="tab" style="font-size: 16px">DTU配置</a></li>
                                        <li id="tabSensor" style="display: none;"><a href="#tab_sensor" data-toggle="tab" style="font-size: 16px">传感器配置</a></li>
                                        <li id="tabSensorGroup" style="display: none;"><a href="#tab_sensorGroup" data-toggle="tab" style="font-size: 16px">传感器分组配置</a></li>
                                        <li id="tabAggConfig"><a href="#tab_aggconfig" data-toggle="tab" style="font-size: 16px">聚集条件配置</a></li>
                                        <li id="tabThreshold" style="display: none;"><a href="#tab_threshold" data-toggle="tab" style="font-size: 16px">阈值配置</a></li>
                                        <li id="tabValidate" style="display: none;"><a href="#tab_Validate" data-toggle="tab" style="font-size: 16px">数据验证配置</a></li>
                                    </ul>
                                    <div class="tab-content">
                                        <!--start tab_Factor-->
                                        <div class="tab-pane row-fluid  active" id="tab_Factor">
                                            <div class="portlet-body">
                                                <div class="clearfix">
                                                    <div class="row-fluid">
                                                        <div class="form-horizontal">
                                                            <div style="float: right">
                                                                <input type="button" class="btn blue editor_Factort" value="修改监测因素" href="#addFactorModal" data-toggle='modal' style="display: block;" />
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="clearfix">
                                                        <div class="row-fluid">
                                                            <div class="form-horizontal">
                                                            </div>
                                                            <div class="" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                                                <div class="body" style="max-height: 540px;">
                                                                    <div class="form-horizontal">
                                                                        <div class="box">
                                                                            <div class="box-header">
                                                                                <div class="box-icon">
                                                                                    已配置监测因素
                                                                                </div>
                                                                            </div>
                                                                            <div id="factorListAdded" class="box-content">
                                                                            </div>
                                                                            <div style="float: right; margin-top: 10px;">
                                                                                <input id="factorNext" type="button" class="btn blue" value="下一步" />
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
                                        <!--end tab_Factor-->
                                        
                                        <!--start tab_FactorUnit-->
                                        <div class="tab-pane row-fluid" id="tab_FactorUnit">
                                            <div class="row-fluid">
                                                <div class="alert alert-warn">
                                                    填写规则：<br />
                                                    <b>所有的监测项都要选择展示单位，并且对于表面位移监测（内部位移监测/桥墩倾斜监测/建筑物倾斜）不同的监测项对应的展示单位要一致！才可以进行“下一步”的操作</b><br/>
                                                </div>
                                            </div>
                                            <table id="FactorUnitTable" width="100%" class="table table-striped table-bordered table-hover" style="width: 100%">
                                                <thead>
                                                    <tr>
                                                        <th>监测因素名称</th>
                                                        <th>监测项</th>
                                                        <th>展示单位</th>
                                                       <%-- <th>操作</th>--%>
                                                    </tr>
                                                </thead>
                                                <tbody id="tbodyFactorUnit">
                                                </tbody>
                                            </table>
                                            <div  style="float: right;margin-top: 10px">
                                                <input id="saveUnits"type="button" class="btn blue" value="保存修改" onclick="saveConfig()"/>
                                                <input id="FactorUnit" type="button" class="btn" value="下一步" onclick="FactorUnitnextOnclick()" />
                                            </div>


                                        </div>
                                        <!--end tab_FactorUnit-->
                                        
                                        <div class="tab-pane row-fluid" id="tab_DTU">
                                            <div class="portlet-body">
                                                <div class="clearfix">
                                                    <div class="row-fluid">
                                                        <div class="form-horizontal">
                                                            <div class="span1" style="margin-bottom: 10px;">
                                                                <input id="btnAddDTU" type="button" class="btn blue" value="新增DTU" href='#addDtuModal' data-toggle='modal' style="display: block;" />
                                                            </div>
                                                             <div class="span1" style="margin-bottom: 10px;">
                                                                <input id="btnAddDTU_exit" type="button" class="btn blue" value="新增DTU映射" href='#addDtuModal_exit' data-toggle='modal' style="display: block;" />
                                                            </div>
                                                            <%-- <div class="span1" style="margin-bottom: 10px;">
                                        <input id="btnDelete" type="button" class="btn blue" value="批量删除" style="display: block;" />
                                    </div>--%>
                                                            <div class="span10">
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <%--<hr />--%>
                                                <table id="dtuTable" width="100%" class="table table-striped table-bordered table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th>DTU编号</th>
                                                            <th>连接类型</th>
                                                            <th>采集粒度</th>
                                                            <th>SIM卡号</th>                                                            
                                                            <th>服务器IP</th>
                                                            <th>端口</th>
                                                            <th>文件路径</th>
                                                            <th>操作</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody id="dtuTbody">
                                                    </tbody>
                                                </table>
                                                <div class="clearfix">
                                                    <div class="row-fluid">
                                                        <div class="form-horizontal">
                                                            <div class="span10">
                                                            </div>
                                                            <div class="span1">
                                                            </div>
                                                            <div class="span1" style="margin-top: 10px">
                                                                <input id="dtuNext" type="button" class="btn" value="下一步" onclick="DTUnextOnclick()" data-toggle='modal' />
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                        <!--end tab_DTU-->

                                        <!-- start sensor config -->
                                        <div class="tab-pane row-fluid" id="tab_sensor">
                                            <div class="clearfix">
                                                <div class="row-fluid">
                                                    <div class="form-horizontal">
                                                        <div class="span2" style="margin-bottom: 10px;">
                                                            <input id="btnAddSensor" type="button" class="btn blue" value="增加传感器" href='#addSensorModal' data-toggle='modal' style="display: block;" />
                                                        </div>
                                                        <div class="span10">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            
                                            <table id="sensorTable" width="100%" class="table table-striped table-bordered">
                                                <thead>
                                                    <tr>
                                                        <th>归属DTU编号</th>
                                                        <th>模块号</th>
                                                        <th>通道号</th>
                                                        <th>产品类型</th>
                                                        <th>产品型号</th>
                                                        <th>传感器位置</th> 
                                                        <th>传感器类型</th>     
                                                        <th>是否启用</th>  <%--2-26  --%>                                             
                                                        <th>操作</th>
                                                    </tr>
                                                </thead>
                                                <tbody id="sensorTbody">
                                                </tbody>
                                            </table>

                                            <div class="clearfix">
                                                <div class="row-fluid">
                                                    <div class="form-horizontal">
                                                        <div class="span10">
                                                        </div>
                                                        <div class="span1">
                                                        </div>
                                                        <div class="span1" style="margin-top: 10px">
                                                            <input id="sensorNext" type="button" class="btn blue" value="下一步" onclick="SensornextOnclick()" data-toggle='modal' />
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                        <!-- end sensor config -->


                                        <div class="tab-pane row-fluid" id="tab_sensorGroup">                                            
                                            <div class="tabbable tabbable-custom">
                                                <div class="clearfix" id="info">
                                                            <div class="row-fluid">
                                                                <div class="form-horizontal">
                                                                    <div class="span6" style="margin-bottom: 10px;color:red">
                                                                        没有可分组的传感器
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                <ul class="nav nav-tabs" style="display:none;">
                                                    <li id="tabCeXie" style="display:none;"><a href="#tab_CeXie" data-toggle="tab" >测斜分组</a></li>
                                                    <li id="tabChenJiang" style="display:none;"><a href="#tab_ChenJiang" data-toggle="tab" ><span class="chenjiang-name"></span></a></li>                            
                                                    <li id="tabJinRunXian" style="display:none;"><a href="#tab_JinRunXian" data-toggle="tab" >浸润线分组</a></li>
                                                </ul>
                                                <div class="tab-content" id="groupTabContainer">
                                                    <div class="tab-pane row-fluid" id="tab_CeXie">
                                                        <div class="clearfix">
                                                            <div class="row-fluid">
                                                                <div class="form-horizontal">
                                                                    <div class="span2" style="margin-bottom: 10px;">
                                                                        <input id="btnAddSensorGroupCeXie" type="button" class="btn blue" value="增加测斜传感器组" href='#addSensorGroupCeXie' data-toggle='modal' />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <table id="SensorGroupCeXieTable" width="100%" class="table table-striped table-bordered">
                                                            <thead>
                                                                <tr>
                                                                    <th>传感器组类型</th>
                                                                    <th>传感器组位置</th>
                                                                    <th>组内传感器-深度</th>
                                                                    <th>操作</th>                                                                  
                                                                </tr>
                                                            </thead>
                                                            <tbody id="SensorGroupCeXieTbody">
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                    <div class="tab-pane row-fluid" id="tab_ChenJiang">
                                                        <div class="clearfix">
                                                            <div class="row-fluid">
                                                                <div class="form-horizontal">
                                                                    <div class="span2" style="margin-bottom: 10px;">
                                                                        <input id="btnAddSensorGroupChenJiang" type="button" class="btn blue" value="增加挠度传感器组" href='#addSensorGroupChenJiang' data-toggle='modal' />

                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <table id="SensorGroupChenJiangTable" width="100%" class="table table-striped table-bordered">
                                                            <thead>
                                                                <tr>
                                                                    <th>传感器组类型</th>
                                                                    <th>传感器组位置</th>
                                                                    <th>组内传感器-距离-是否是基准点</th>
                                                                    <th>操作</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody id="SensorGroupChenJiangTbody">
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                    <div class="tab-pane row-fluid" id="tab_JinRunXian">
                                                        <div class="clearfix">
                                                            <div class="row-fluid">
                                                                <div class="form-horizontal">
                                                                    <div class="span2" style="margin-bottom: 10px;">
                                                                        <input id="btnAddSensorGroupJinRunXian" type="button" class="btn blue" value="增加浸润线传感器组" href='#addSensorGroupJinRunXian' data-toggle='modal' />

                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <table id="SensorGroupJinRunXianTable" width="100%" class="table table-striped table-bordered">
                                                            <thead>
                                                                <tr>
                                                                    <th>传感器组类型</th>
                                                                    <th>传感器组位置</th>
                                                                    <th>组内传感器-高度</th>
                                                                    <th>操作</th>                                                                  
                                                                </tr>
                                                            </thead>
                                                            <tbody id="SensorGroupJinRunXianTbody">
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                            
                                            <div class="clearfix">
                                                <div class="row-fluid">
                                                    <div class="form-horizontal">
                                                        <div class="span10">
                                                        </div>
                                                        <div class="span1">
                                                        </div>
                                                        <div class="span1" style="margin-top: 10px">
                                                            <input id="sensorGroupNext" type="button" class="btn blue" value="下一步" onclick="groupConfigNext()" data-toggle='modal' />
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                        <!-- start agg config -->
                                        <div class="tab-pane row-fluid" id="tab_aggconfig">
                                            <div class="clearfix">
                                                <div class="row-fluid">
                                                    <div class="form-horizontal">
                                                        <div class="span2" style="margin-bottom: 10px;">
                                                            <%--<input id="btnAddAggConfig" type="button" class="btn blue" value="新增聚集配置" href='#editAggConfig' data-toggle='modal' style="display: block;" />--%>
                                                            <input id="btnAddAggConfig" type="button" class="btn blue" value="新增聚集配置" data-toggle='modal' style="display: block;" />
                                                        </div>
                                                        <div class="span10">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            
                                            <table id="aggconfigtable" width="100%" class="table table-striped table-bordered">
                                                <thead>
                                                    <tr>
                                                        <th>监测因素</th>
                                                        <th>类型</th>
                                                        <th>状态</th>
                                                        <th>详细信息</th>
                                                        <th>操作</th>
                                                    </tr>
                                                </thead>
                                                <tbody id="aggconfigtablebody">
                                                </tbody>
                                            </table>

                                            <div class="clearfix">
                                                <div class="row-fluid">
                                                    <div class="form-horizontal">
                                                        <div class="span10">
                                                        </div>
                                                        <div class="span1">
                                                        </div>
                                                        <div class="span1" style="margin-top: 10px">
                                                            <input id="Button3" type="button" class="btn blue" value="下一步" onclick="aggConfigNextOnclick()" data-toggle='modal' />
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                        <!-- end agg config -->

                                        <!-- start threshold config -->
                                        <div class="tab-pane row-fluid" id="tab_threshold">
                                            <ul id="ul-Threshold" class="nav nav-tabs">
                                                                                       
                                            </ul>
                                            <div class="row-fluid">
                                                <div class="alert alert-warn">
                                                    <b>阈值填写格式：</b>例如一级阈值范围为1至2之间和大于5，按(1,2);(5,+)的格式填写。其中+表示正无穷，-表示负无穷。注意输入区间不要有重叠。请切换到英文输入法。
                                                </div>
                                            </div>
                                            <table id="table-Threshold" width="100%" class="table table-striped table-bordered table-hover" style="width:100%">
                                                <thead>
                                                    <tr>
                                                        <th>设备名称</th>
                                                        <th>监测项</th>
                                                        <th>一级阈值</th>
                                                        <th>二级阈值</th>
                                                        <th>三级阈值</th>
                                                        <th>四级阈值</th>
                                                        <th>操作</th>
                                                    </tr>
                                                </thead>
                                                <tbody id="tbody-Threshold">

                                                </tbody>
                                            </table>
                                            <div class="clearfix">
                                                <div class="row-fluid">
                                                    <div class="form-horizontal">
                                                        <div class="span10">
                                                        </div>
                                                        <div class="span1">
                                                        </div>
                                                        <div class="span1" style="margin-top: 10px">
                                                            <input id="Button1" type="button" class="btn blue" value="下一步" onclick="thresholdConfigNext()" data-toggle='modal' />
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- end threshold config -->
                                        
                                        <!-- start validate config -->
                                        <div class="tab-pane row-fluid" id="tab_Validate">
                                            <ul id="ul_validate" class="nav nav-tabs">
                                                                                       
                                            </ul>
                                            <div class="row-fluid">
                                                <div class="alert alert-warn">
                                                    填写规则：<br />
                                                    如果启用合理性验证,则必须<b>完整</b>填写:<b>合理值下限(数字)</b>和<b>合理值上限(数字)</b>;<br />
                                                    如果启用稳定性验证,则必须<b>完整</b>填写:<b>窗口大小(正整数)</b>、<b>K值(正数)</b>、<b>D值(正整数)</b>、<b>ReCalc值(正整数)</b>
                                                </div>
                                            </div>
                                            <table id="table-validate" width="100%" class="table table-striped table-bordered table-hover" style="width:100%">
                                                <thead>
                                                    <tr>
                                                        <th>设备名称</th>
                                                        <th>监测项</th>
                                                        <th>合理性验证</th>
                                                        <th>合理值下限</th>
                                                        <th>合理值上限</th>
                                                        <th>稳定性验证</th>
                                                        <th>窗口大小</th>
                                                        <th>K值</th>
                                                        <th>D值</th>
                                                        <th>ReCalc值</th>
                                                        <th>操作</th>
                                                    </tr>
                                                </thead>
                                                <tbody id="tbody-validate">

                                                </tbody>
                                            </table>                                           
                                        </div>
                                        <!-- end validate config -->

                                    </div>
                                </div>
                                <!--END TABS-->
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- 添加Modal Add DTU -->
        <div id="addDtuModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3 id="addDtuModalTitle">增加DTU</h3>
            </div>
            <div class="modal-body">
                <div class="form-horizontal">
                    <div class="control-group">
                        <label class="control-label">结构物</label>
                        <div class="controls">
                            <input id="addDTUstr" name="addDTUstr" type="text" readonly="true" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">DTU编号</label>
                        <div class="controls">
                            <input id="addDTUnumber" name="addDTUnumber" type="text" pattern="^\d{8}$" title="8位数字编号" placeholder="8位数字编号" maxlength="8"/>
                            <p id="addDtuRepet" style="display: none"><span style="color: red;">此DTU编号已被使用</span></p>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">DTU厂商</label>
                        <div class="controls">
                            <select id="addDTUFactory" name="addDTUFactory" class="chzn-select">
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">DTU型号</label>
                        <div class="controls">
                            <select id="addDTUModel" name="addDTUFactory">
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">连接类型</label>
                        <div class="controls">
                            <input id="addConnectType" name="addConnectType" type="text" readonly="true" />
                        </div>
                    </div>                    
                    <div class="control-group">
                        <label class="control-label">采集粒度</label>
                        <div class="controls">
                            <select id="addDTUcollectGranularity" name="addDTUcollectGranularity">
                                <option value="5">5分钟</option>
                                <option value="15">15分钟</option>
                                <option value="30" selected="selected">30分钟</option>
                                <option value="45">45分钟</option>
                                <option value="60">60分钟</option>
                            </select>
                        </div>
                    </div>
                    <div class="control-group gprs-control">
                        <label class="control-label">SIM卡号</label>
                        <div class="controls">
                            <input id="addDTUsim" name="addDTUsim" type="text" pattern="^((\(\d{3}\))|(\d{3}\-))?18\d{9}|13\d{9}|15\d{9}$" title="11位SIM卡号" placeholder="11位SIM卡号" maxlength="11"/>
                        </div>
                    </div>
                    <div class="control-group gprs-control">
                        <label class="control-label">端口</label>
                        <div class="controls">
                            <input id="addDTUport" name="addDTUport" type="text" pattern="^(\d)+$" title="要求数字" placeholder="要求数字" maxlength="5" />
                            <p id="addDTUportRange" style="display: none"><span style="color: red;">端口号需为小于65536的正整数</span></p>
                        </div>
                    </div>
                    <div class="control-group gprs-control">
                        <label class="control-label">服务器IP</label>
                        <div class="controls">
                            <input id="addDTUip" name="addDTUip" type="text" pattern="^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$" title="有效的IP地址" placeholder="有效的IP地址" />
                        </div>
                    </div> 
                    <div class="control-group local-control" style="display:none;">
                        <label class="control-label">文件路径</label>
                        <div class="controls">
                            <input id="addDTUFile" name="addDTUFile" type="text" title="文件绝对路径" placeholder="文件绝对路径" />
                        </div>
                    </div>                    
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">关闭</button>
                <input id="btnResetDTU" type="button" value="重置" class="btn" />
                <input id="btnSaveDTU" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>
        <!-- end 添加Modal Add DTU -->

         <!-- 添加Modal Add DTU映射 -->
        <div id="addDtuModal_exit" class="modal hide fade" style="height:auto;" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3 id="H1">增加DTU</h3>
            </div>
            <div class="modal-body">
                <div class="form-horizontal" style="height:280px;">
                    <div class="control-group">
                        <label class="control-label">结构物</label>
                        <div class="controls">
                            <input id="addDTUstr_exit" name="addDTUstr_exit" type="text" readonly="true" />
                        </div>
                    </div>  
                    <div class="control-group">
                        <label class="control-label">组织下DTU列表</label>
                        <div class="controls">
                            <select class="selectpicker" data-size="5" id="DTU_List_exit" title="请选择"></select>
                        </div>
                    </div>                 
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">关闭</button>              
                <input id="btnSaveDTU_exit" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>
        <!-- end 添加Modal Add DTU -->

        <!-- 修改DTU Modal modify DTU -->
        <div id="modifyDTUModal" class="modal hide fade">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>修改DTU</h3>
            </div>
            <div class="modal-body">
                <div class="form-horizontal">
                    <div class="control-group">
                        <label class="control-label">结构物</label>
                        <div class="controls">
                            <input id="modifyDTUstr" name="modifyDTUstr" type="text" readonly="true" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">DTU编号</label>
                        <div class="controls">
                            <input id="modifyDTUnumber" name="modifyDTUnumber" type="text" pattern="^\d{8}$" title="8位数字编号" placeholder="8位数字编号" maxlength="8" />
                            <p id="modifyDtuRepet" style="display: none"><span style="color: red;">此DTU编号已被使用</span></p>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">DTU厂商</label>
                        <div class="controls">
                            <select id="modifyDTUFactory" name="modifyDTUFactory" class="chzn-select">
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">DTU型号</label>
                        <div class="controls">
                            <select id="modifyDTUModel" name="modifyDTUModel">
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">连接类型</label>
                        <div class="controls">
                            <input id="modifyConnectType" name="modifyConnectType" type="text" readonly="true" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">采集粒度</label>
                        <div class="controls">
                            <select id="modifyDTUcollectGranularity" class="collectGranularityEdit" name="modifyDTUcollectGranularity">
                                <option value="5">5分钟</option>
                                <option value="15">15分钟</option>
                                <option value="30">30分钟</option>
                                <option value="45">45分钟</option>
                                <option value="60">60分钟</option>
                            </select>
                        </div>
                    </div>
                    <div class="control-group gprs-control">
                        <label class="control-label">SIM卡号</label>
                        <div class="controls">
                            <input id="modifyDTUsim" name="modifyDTUsim" type="text" pattern="^((\(\d{3}\))|(\d{3}\-))?18\d{9}|13\d{9}|15\d{9}$" title="11位SIM卡号" placeholder="11位SIM卡号" maxlength="11" />
                        </div>
                    </div>                    
                    <div class="control-group gprs-control">
                        <label class="control-label">端口</label>
                        <div class="controls">
                            <input id="modifyDTUport" name="modifyDTUport" type="text" pattern="^(\d)+$" title="要求数字" placeholder="要求数字" maxlength="5" />
                            <p id="modifyDTUportRange" style="display: none"><span style="color: red;">端口号需小于65536</span></p>
                        </div>
                    </div>
                    <div class="control-group gprs-control">
                        <label class="control-label">服务器IP</label>
                        <div class="controls">
                            <input id="modifyDTUip" name="modifyDTUip" type="text" pattern="^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$" title="有效的IP地址" placeholder="有效的IP地址" />
                        </div>
                    </div>
                    <div class="control-group local-control" style="display:none;">
                        <label class="control-label">文件路径</label>
                        <div class="controls">
                            <input id="modifyDTUFile" name="modifyDTUFile" type="text" title="文件绝对路径" placeholder="文件绝对路径" />                            
                        </div>
                    </div>                                    
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">关闭</button>
                <input id="btnResetModifyDTU" type="button" value="重置" class="btn" />
                <input id="btnSaveModifyDTU" type="button" value="保存并修改" class="btn btn-primary" />
            </div>
        </div>
        <!-- end 修改DTU Modal modify DTU -->

        <!-- 删除 DTU Modal Delete DTU -->
        <div id="deleteDTUModal" class="modal hide fade">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>删除DTU</h3>
            </div>
            <div class="modal-body">
                <p id="pDeleteDTU"></p>
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">取消</button>
                <a href="#" id="btnDeleteDTU" class="btn btn-danger" data-dismiss="modal">确定</a>
            </div>
        </div>
        <!-- end 删除 DTU Modal Delete DTU -->
       
        <!-- 查看 view Sensor model -->
        <div id="viewSensorModal" class="modal hide fade" style="width:760px;">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>查看传感器</h3>
            </div>
            <div class="modal-body">
                <div class="form-horizontal">
                    <div class="control-group"  align="center">
                        <table id="viewSensorTable" style="margin:auto;font-size:16px;">
                            <tr>
                                <td><strong>监测因素：</strong></td>
                                <td id="viewSensorFactor"></td>
                            </tr>
                            <tr>
                                <td><strong>类型：</strong></td>
                                <td id="viewIdentification"></td>
                            </tr>
                           <%-- 2-26--%>
                            <tr>
                                <td><strong>是否启用：</strong></td>
                                <td id="viewControl"></td>
                            </tr>
                            <tr id="viewDtu">
                                <td><strong>归属DTU编号：</strong></td>
                                <td id="viewSensorDTU"></td>
                            </tr>
                            <tr id="viewModule">
                                <td><strong>模块号：</strong></td>
                                <td id="viewSensorModule"></td>
                            </tr>
                            <tr id="viewChannel">
                                <td><strong>通道号：</strong></td>
                                <td id="viewSensorChannel"></td>
                            </tr>
                            <tr>
                                <td><strong>产品类型：</strong></td>
                                <td id="viewSensorType"></td>
                            </tr>
                            <tr>
                                <td><strong>产品型号：</strong></td>
                                <td id="viewSensorTypeNumber"></td>
                            </tr>
                            <tr>
                                <td><strong>位置：</strong></td>
                                <td id="viewSensorPosition"></td>
                            </tr>
                            <%--添加滚动条--%>
                            <tr id="correntSensor"  style="display:none;">
                                <td><strong>关联传感器：</strong></td>
                                <td id="viewCorrent1" style="">
                                    <div  style="overflow-y:auto; width:600px; height:30px;">
                                        <span id="viewCorrent"></span>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td><strong>计算公式：</strong></td>
                                <td id="viewSensorFormula"></td>
                            </tr>
                            <tr>
                                <td><strong>参数：</strong></td>
                                <td id="viewSensorParam"></td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">关闭</button>
            </div>
        </div>

        <!-- 添加 add Sensor model -->
        <div id="addSensorModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="max-height:600px;">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3 id="addSensorModalTitle">增加传感器</h3>
            </div>
            <div class="modal-body" style="max-height: 400px;">
                <div class="form-horizontal">
                    <div class="control-group">
                        <label class="control-label">监测因素</label>
                        <div class="controls">
                            <select id="addSensorFactor" name="addSensorFactor">
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">类型</label>
                        <div class="controls">
                            <select id="addSensorIdentification" name="addSensorFactor">
                            <option value="0">实体</option>
                            <option value="1">数据</option>
                            <option value="2">组合</option>

                            </select>
                        </div>
                    </div>
                     <div class="control-group">
                        <label class="control-label">是否启用</label>
                        <div class="controls">
                            <select id="addControl" name="addSensorFactor">
                            <option value="0">是</option>
                            <option value="1">否</option>
                            </select>
                        </div>
                    </div>
                    <div class="control-group"  id="dtuNo">
                        <label class="control-label">归属DTU编号</label>
                        <div class="controls">
                            <select id="addSensorDTU" name="addSensorDTU">
                            </select>
                        </div>
                    </div>
                    <div class="control-group" id="module">
                        <label class="control-label">模块号</label>
                        <div class="controls">
                            <input id="addSensorModule" name="addSensorModule" type="text"  pattern="^(\d)+$" title="要求数字" placeholder="要求数字" maxlength="5"/>
                           
                        </div>
                    </div>
                    <div class="control-group" id="channel">
                        <label class="control-label">通道号</label>
                        <div class="controls">
                            <input id="addSensorChannelNumber" name="addSensorChannelNumber" type="text"  pattern="^(\d)+$" title="1~32的通道号" placeholder="1~32的通道号" maxlength="2"/>
                             <p id="addSensorRepet" style="display: none"><span style="color: red;">此通道号已被使用</span></p>
                            <p id="addSensorChannelRange" style="display: none"><span style="color: red;">通道号需在1~32之间</span></p>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">产品类型</label>
                        <div class="controls">
                            <select id="addSensorType" name="addSensorType" class="chzn-select">
                            </select>
                        </div>
                    </div>
                    <%--<div class="control-group">
                        <label class="control-label">test</label>
                        <div class="controls">
                            <select id="test" name="test" class="chzn-select">
                               
                            </select>
                        </div>
                    </div>--%>
                    <div class="control-group">
                        <label class="control-label">产品型号</label>
                        <div class="controls">
                            <select id="addSensorTypeNumber" name="addSensorTypeNumber">
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">位置</label>
                        <div class="controls">
                            <input id="addSensorPosition" name="addSensorPosition" type="text" placeholder=" " />
                        </div>
                    </div>
                     <div class="control-group" style="display:none;" id="corrent">
                        <label class="control-label">关联传感器</label>
                        <div class="controls">
                            <select id="addCorrentPosition" name="addSensorTypeNumber"  multiple="multiple" data-placeholder="请选择">
                            </select>                            
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">计算公式</label>
                        <div class="controls">
                            <textarea id="addSensorFormula" name="addSensorFormula" disabled="disabled" cols="20" rows="2"></textarea>
                            <%--<input id="addSensorFormula" name="addSensorFormula" type="text" disabled="disabled" placeholder=" " />--%>
                        </div>
                    </div>
                    <div class="control-group">
                        <table id="addSensorTable" class="table table-striped table-bordered" style="width: 400px; margin-left: auto; margin-right: auto;">
                            <thead>
                                <tr>
                                    <th>参数</th>
                                    <th>值</th>
                                </tr>
                            </thead>
                            <tbody id="addSensorTbody">
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button id="btnCloseSensor" class="btn" data-dismiss="modal">关闭</button>
                <input id="btnResetSensor" type="button" value="重置" class="btn" />
                <input id="btnSaveCloseSensor" type="button" value="保存并关闭" class="btn btn-primary" />
                <input id="btnSaveSensor" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>
        <!-- end add Sensor model -->

          <!-- 修改 modify Sensor model -->
        <div id="modifySensorModal" class="modal modal800 hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="max-height:600px;">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>修改传感器</h3>
            </div>
            <div class="modal-body" style="max-height: 520px;">
                <div class="form-horizontal">
                    <div class="control-group">
                        <label class="control-label">监测因素</label>
                        <div class="controls">
                            <select id="modifySensorFactor" name="modifySensorFactor">
                            </select>
                        </div>
                    </div>
                     <div class="control-group">
                        <label class="control-label">类型</label>
                        <div class="controls">
                            <input id="modifyIdentity" name="modifySensorChannelNumber" type="text"  readonly="true"/>                           
                        </div>
                    </div>
                   <%-- 2-26--%>
                      <div class="control-group">
                        <label class="control-label">是否启用</label>
                        <div class="controls">
                            <select id="modifyControl" name="modifySensorControl">
                             <option value="0">是</option>
                            <option value="1">否</option>
                            </select>                  
                        </div>
                    </div>
                    <div class="control-group" id="modifyDTU">
                        <label class="control-label">归属DTU编号</label>
                        <div class="controls">
                            <select id="modifySensorDTU" name="modifySensorDTU">
                            </select>
                        </div>
                    </div>
                    <div class="control-group" id="modifyModule">
                        <label class="control-label">模块号</label>
                        <div class="controls">
                            <input id="modifySensorModule" name="modifySensorModule" type="text"  pattern="^(\d)+$" title="要求数字" placeholder="要求数字" maxlength="5"/>
                            
                        </div>
                    </div>
                    <div class="control-group" id="modifyChannel">
                        <label class="control-label">通道号</label>
                        <div class="controls">
                            <input id="modifySensorChannelNumber" name="modifySensorChannelNumber" type="text"  pattern="^(\d)+$" title="1~32的通道号" placeholder="1~32的通道号" maxlength="2"/>
                            <p id="modifySensorRepet" style="display: none"><span style="color: red;">此通道号已被使用</span></p>
                            <p id="modifySensorChannelRange" style="display: none"><span style="color: red;">通道号需在1~32之间</span></p>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">产品类型</label>
                        <div class="controls">
                            <select id="modifySensorType" name="modifySensorType" class="chzn-select">
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">产品型号</label>
                        <div class="controls">
                            <select id="modifySensorTypeNumber" name="modifySensorTypeNumber">
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">位置</label>
                        <div class="controls">
                            <input id="modifySensorPosition" name="modifySensorPosition" type="text" placeholder=" " />
                        </div>
                    </div>
                    <div class="control-group" style="display:none;" id="modifyCorrent">
                        <label class="control-label">关联传感器</label>
                        <div class="controls">
                            <select id="modifySensor" name="addSensorTypeNumber" class="chzn-select"  multiple="multiple" data-placeholder="请选择">
                            </select>                            
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">计算公式</label>
                        <div class="controls">
                            <textarea id="modifySensorFormula" name="modifySensorFormula" disabled="disabled" cols="20" rows="2"></textarea>
                            <%--<input id="modifySensorFormula" name="modifySensorFormula" type="text" disabled="disabled" placeholder=" " />--%>
                        </div>
                    </div>
                    <div class="control-group">
                        <table id="modifySensorTable" class="table table-striped table-bordered" style="width: 400px; margin-left: auto; margin-right: auto;">
                            <thead>
                                <tr>
                                    <th>参数</th>
                                    <th>值</th>
                                </tr>
                            </thead>
                            <tbody id="modifySensorTbody">
                               <%-- <tr id="id-123">
                                    <td>k</td>
                                    <td><input name="moduleAdd" type="text"/></td>
                                </tr>
                                <tr id="id-2">
                                    <td>kt</td>
                                    <td><input name="moduleAdd" type="text" /></td>
                                </tr>
                                <tr id="id-3">
                                    <td>f0</td>
                                   <td><input name="moduleAdd" type="text"  /></td>
                                </tr>
                                <tr id="id-4">
                                    <td>t0</td>
                                    <td><input name="moduleAdd" type="text"/></td>
                                </tr>--%>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">关闭</button>
                <input id="btnResetModifySensor" type="button" value="重置" class="btn" />
                <input id="btnSaveCloseModifySensor" type="button" value="保存并修改" class="btn btn-primary" />
            </div>
        </div>
        <!-- end add Sensor model -->

        <!-- 删除  Modal Delete 传感器 -->
        <div id="deleteSensorModal" class="modal hide fade">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>删除传感器</h3>
            </div>
            <div class="modal-body">
                <p id="pDeleteSensor"></p>
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">取消</button>
                <a href="#" id="btnDeleteSensor" class="btn btn-danger" data-dismiss="modal">确定</a>
            </div>
        </div>
        <!-- end 删除 Modal Delete 传感器 -->


        <!-- 修改监测因素 -->
        <div id="addFactorModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div id="modal_header" class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h2>修改监测因素</h2>
            </div>
            <div id="modal-body" class="modal-body" style="max-height: 540px;">
                <div class="form-horizontal">

                    <div id="viewOrganization" class="control-group">
                        <label class="control-label">结构物名称</label>
                        <div class="controls">
                            <input type="text" id="organizationNameview" name="organizationNameview" placeholder="" class="width206" readonly="true" />
                        </div>
                    </div>

                    <div id="factorListChoosen" class="box">

                        <%--<table>
                            <tr>
                                <th>
                                    <label class="control-label"><b>变形主题：</b></label>
                                </th>
                                <th style="width: 113px;">
                                    <input type="checkbox" class="checkboxes" checked="checked" value="1" /><font style="font-weight: normal;">表面变形</font>
                                </th>
                                <th style="width: 100px">
                                    <input type="checkbox" /><font style="font-weight: normal;">桥墩沉降</font>
                                </th>
                                <th style="width: 100px">
                                    <input type="checkbox" /><font style="font-weight: normal;">桥梁裂缝</font>
                                </th>
                            </tr>
                        </table>

                        <hr style="border: 1px #cccccc dashed;" />--%>
                    </div>

                </div>
            </div>
            <div id="modal-footer" class="modal-footer">
                <button class="btn" data-dismiss="modal">关闭</button>
                <input id="btnResetFactor" type="button" value="重置" class="btn" />
                <input id="btnSaveFactor" type="button" value="保存" class="btn" />
            </div>
        </div>
        

        <!--增加测斜传感器分组-->
        <div id="addSensorGroupCeXie" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="max-height:800px;">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>增加测斜传感器分组</h3>
            </div>
            <div class="modal-body" style="height: 600px;">
                <div class="form-horizontal">                                                                           
                    <div class="control-group">
                        <label class="control-label">传感器组位置</label>
                        <div class="controls">
                           <input id="SensorGroupCeXieLocation" name="SensorGroupCeXieLocation" type="text" /> 
                        </div>
                    </div>                                                       
                    <div class="control-group">
                        <label class="control-label">组内传感器位置</label>
                        <div class="controls">
                            <select data-placeholder="请选择"  id="sensorGroupListCeXie" name="sensorGroupListCeXie" class="chzn-select" multiple="multiple" tabindex="6">
                                
                            </select> 
                        </div>
                    </div>
                    <div class="control-group">
                        <table class="table table-striped table-bordered" style="width: 400px; margin-left: auto; margin-right: auto;">
                            <thead>
                                <tr>
                                    <th>位置</th>
                                    <th>深度(小数,单位:米)</th>
                                </tr>
                            </thead>
                            <tbody id="TbodyCexie">
                               <%-- <tr>
                                    <td>X-1</td>
                                    <td><input type="text" /></td>
                                </tr>       --%>                         
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button id="btnCeXieDis" class="btn" data-dismiss="modal">关闭</button>                               
                <input id="btnCeXieSave" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>

        <!--修改测斜传感器分组-->
        <div id="editSensorGroup" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="max-height:800px;">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>修改测斜传感器分组</h3>
            </div>
            <div class="modal-body" style="height: 600px;">
                <div class="form-horizontal">                                                                         
                    <div class="control-group">
                        <label class="control-label">传感器组位置</label>
                        <div class="controls">
                           <input id="editSensorGroupCeXieLocation" name="SensorGroupCeXieLocation" type="text" /> 
                        </div>
                    </div>                                                       
                    <div class="control-group">
                        <label class="control-label">组内传感器位置</label>
                        <div class="controls">
                            <select data-placeholder="请选择"  id="editsensorGroupListCeXie" name="sensorGroupListCeXie" class="chzn-select" multiple="multiple" tabindex="6">
                                
                            </select> 
                        </div>
                    </div>
                    <div class="control-group">
                        <table class="table table-striped table-bordered" style="width: 400px; margin-left: auto; margin-right: auto;">
                            <thead>
                                <tr>
                                    <th>位置</th>
                                    <th>深度(小数,单位:米)</th>
                                </tr>
                            </thead>
                            <tbody id="editTbodyCexie">
                                                   
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button id="btnCeXieDisEdit" class="btn" data-dismiss="modal">关闭</button>                              
                <input id="btnCeXieEdit" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>
        
        <!--增加挠度传感器分组-->
        <div id="addSensorGroupChenJiang" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="max-height:800px;">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>增加<span class="chenjiang-name">挠度传感器分组</span></h3>
            </div>
            <div class="modal-body" style="height: 600px;">
                <div class="form-horizontal">                                                                           
                    <div class="control-group">
                        <label class="control-label">传感器组位置</label>
                        <div class="controls">
                           <input id="SensorGroupChenJiangLocation" name="SensorGroupChenJiangLocation" type="text" /> 
                        </div>
                    </div>                                                       
                    <div class="control-group">
                        <label class="control-label">组内传感器位置</label>
                        <div class="controls">
                            <select data-placeholder="请选择"  id="sensorGroupListChenJiang" name="sensorGroupListChenJiang" class="chzn-select" multiple="multiple" tabindex="6">
                                
                            </select> 
                        </div>
                    </div>
                    <div class="control-group">
                        <table class="table table-striped table-bordered" style="width: 400px; margin-left: auto; margin-right: auto;">
                            <thead>
                                <tr>
                                    <th>位置</th>
                                    <th>距离(整数,单位:米)</th>
                                    <th>是否是基准点</th>
                                </tr>
                            </thead>
                            <tbody id="TbodyChenJiang">
                               <%-- <tr>
                                    <td>X-1</td>
                                    <td><input type="text" /></td>
                                </tr>       --%>                         
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button id="btnChenJiangDis" class="btn" data-dismiss="modal">关闭</button>                             
                <input id="btnChenJiangSave" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>

        <!--修改沉降传感器分组-->
        <div id="editSensorGroupChenJiang" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="max-height:800px;">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>修改<span class="chenjiang-name">挠度传感器分组</span></h3>
            </div>
            <div class="modal-body" style="height: 600px;">
                <div class="form-horizontal">                                                                         
                    <div class="control-group">
                        <label class="control-label">传感器组位置</label>
                        <div class="controls">
                           <input id="editSensorGroupChenJiangLocation" name="SensorGroupChenJiangLocation" type="text" /> 
                        </div>
                    </div>                                                       
                    <div class="control-group">
                        <label class="control-label">组内传感器位置</label>
                        <div class="controls">
                            <select data-placeholder="请选择"  id="editsensorGroupListChenJiang" name="sensorGroupListChenJiang" class="chzn-select" multiple="multiple" tabindex="6">
                                
                            </select> 
                        </div>
                    </div>
                    <div class="control-group">
                        <table class="table table-striped table-bordered" style="width: 400px; margin-left: auto; margin-right: auto;">
                            <thead>
                                <tr>
                                    <th>位置</th>
                                    <th>距离(整数,单位:米)</th>
                                    <th>是否是基准点</th>
                                </tr>
                            </thead>
                            <tbody id="editTbodyChenJiang">
                                                   
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button id="btnChenJiangDisEdit" class="btn" data-dismiss="modal">关闭</button>                               
                <input id="btnChenJiangEdit" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>
        
        <!--增加浸润线传感器分组-->
        <div id="addSensorGroupJinRunXian" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="max-height:800px;">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>增加浸润线传感器分组</h3>
            </div>
            <div class="modal-body" style="height: 600px;">
                <div class="form-horizontal">                                                                           
                    <div class="control-group">
                        <label class="control-label">传感器组位置</label>
                        <div class="controls">
                           <input id="SensorGroupJinRunXianLocation" name="SensorGroupJinRunXianLocation" type="text" /> 
                        </div>
                    </div>                                                       
                    <div class="control-group">
                        <label class="control-label">组内传感器位置</label>
                        <div class="controls">
                            <select data-placeholder="请选择"  id="sensorGroupListJinRunXian" name="sensorGroupListJinRunXian" class="chzn-select" multiple="multiple" tabindex="6">
                                
                            </select> 
                        </div>
                    </div>
                    <div class="control-group">
                        <table class="table table-striped table-bordered" style="width: 400px; margin-left: auto; margin-right: auto;">
                            <thead>
                                <tr>
                                    <th>位置</th>
                                    <th>高度(小数,单位:米)</th>
                                </tr>
                            </thead>
                            <tbody id="TbodyJinRunXian">
                               <%-- <tr>
                                    <td>X-1</td>
                                    <td><input type="text" /></td>
                                </tr>       --%>                         
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button id="btnJinRunXianDis" class="btn" data-dismiss="modal">关闭</button>                              
                <input id="btnJinRunXianSave" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>

        <!--修改浸润线传感器分组-->
        <div id="editSensorGroupJinRunXian" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="max-height:800px;">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>修改浸润线传感器分组</h3>
            </div>
            <div class="modal-body" style="height: 600px;">
                <div class="form-horizontal">                                                                         
                    <div class="control-group">
                        <label class="control-label">传感器组位置</label>
                        <div class="controls">
                           <input id="editSensorGroupJinRunXianLocation" name="SensorGroupJinRunXianLocation" type="text" /> 
                        </div>
                    </div>                                                       
                    <div class="control-group">
                        <label class="control-label">组内传感器位置</label>
                        <div class="controls">
                            <select data-placeholder="请选择"  id="editsensorGroupListJinRunXian" name="sensorGroupListJinRunXian" class="chzn-select" multiple="multiple" tabindex="6">
                                
                            </select> 
                        </div>
                    </div>
                    <div class="control-group">
                        <table class="table table-striped table-bordered" style="width: 400px; margin-left: auto; margin-right: auto;">
                            <thead>
                                <tr>
                                    <th>位置</th>
                                    <th>高度(小数,单位:米)</th>
                                </tr>
                            </thead>
                            <tbody id="editTbodyJinRunXian">
                                                   
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button id="btnJinRunXianDisEdit" class="btn" data-dismiss="modal">关闭</button>                               
                <input id="btnJinRunXianEdit" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>
        <!--聚集条件配置-->
        <div id="editAggConfig" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" style="width: 650px">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3 id="aggFormTitle">聚集条件配置</h3>
            </div>
            <div class="modal-body" style="height: 600px;">
                <div class="form-horizontal">
                    <div class="control-group">
                        <label class="control-label">监测因素：</label>
                        <div class="controls">
                             <select style="width: 200px" id="aggFactor" title="请选择">
                             
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">类型：</label>
                        <div class="controls">
                             <select style="width: 100px" id="aggType">
                             
                            </select>
                        </div>
                    </div>
                    <div class="control-group" id="configEnable">
                        <label class="control-label">状态：</label>
                        <div class="controls">
                            <input type="checkbox" name="aggconfig-switch-CheckBox" id="aggConfigEnable"  data-on-text="启用" data-off-text="禁用"/>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">聚集方式：</label>
                        <div class="controls">
                            <select style="width: 100px" id="aggWay">
                               
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">每天时间范围：</label>
                        <div class="controls">
                            <select style="width: 100px" name="agghour" id="beginDataTime"></select>
                            <span style="font-size: 14px">至</span>
                            <select style="width: 100px" name="agghour" id="endDataTime"></select>
                        </div>
                    </div>
                     
                    <div class="control-group" id ="monthAggDayRang">
                         <label class="control-label">聚集日期范围：</label>
                        <div class="controls">
                                                                        
                            <select style="width: 100px" id="beginMonthDay">
                               
                            </select>
                            <span style="font-size: 14px">至</span>
                            <select style="width: 100px" id="endMonthDay">
                               
                            </select>
                        </div>
                    </div>
                    
                    <div class="control-group" id ="weekAggDayRang">
                         <label class="control-label">聚集日期范围：</label>
                        <div class="controls">                                             
                            <select style="width: 100px" id="beginWeekDay">
                              
                            </select>
                            <span style="font-size: 14px">至</span>
                            <select style="width: 100px" id="endWeekDay">
                                
                            </select>
                        </div>
                    </div>
                    <div class="control-group" id="monthTiming">
                        <label class="control-label">定时模式：</label>
                        <div class="controls">
                            <label class="radio line">
                                <input type="radio" name="optionsRadios" value="option1" checked="checked"/>每个月的
                                <select style="width: 100px" id="monthAggTimeDay">
                                   
                                </select>
                                <select style="width: 100px"  id="beginMonthAggTime1">
							
							    </select>
                            </label>
                            <label class="radio line">
                                <input type="radio" name="optionsRadios" value="option2"/>每个月的
                                <select style="width: 100px" id="aggWeekIndex">
                                    <option id="1">第一个</option>
                                    <option id="2">第二个</option>
                                    <option id="3">第三个</option>
                                    <option id="4">第四个</option>
                                    <option id="-1">最后一个</option>
                                </select>
                                <select style="width: 100px" id="monthAggTimeDayofWeek">
                                </select>
                                <select style="width: 100px"  id="beginMonthAggTime2">
							
							    </select>
                            </label>
                        </div>
                    </div>
                    <div class="control-group" id="weekTiming">
                        <label class="control-label">定时模式：</label>
                        <div class="controls">
                            <select style="width: 100px" id="weekAggTimeDay">
                              
                            </select>
                            <select style="width: 100px"  id="beginWeekAggTime">
							
							</select>
                        </div>
                    </div>
                    <div class="control-group" id="dayTimeing">
                        <label class="control-label">定时模式：</label>
                        <div class="controls">
                            <select style="width: 100px" id="beginDayAggTime">
								
								
							</select>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button id="btnAggConfigClose" class="btn" data-dismiss="modal">关闭</button>                               
                <input id="btnAggConfigSave" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>

    </form>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <%--<script type="text/javascript" src="/resource/js/jquery-2.0.3.min.js"></script>--%>
    <script src="../resource/js/jquery-1.8.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script src="/resource/library/data-tables/js/jquery.dataTables.js"></script>
    <script src="/resource/library/data-tables/DT_bootstrap.js"></script>
    <script src="/resource/library/bootstrap/js/bootstrap-select.js"></script>   
    <script src="/resource/library/chosen-bootstrap/chosen/chosen.jquery.js"></script>
    <script src="/resource/library/bootstrap-switch/js/bootstrap-switch.js"></script>
    <script src="../data/js/bootstrap-datetimepicker.js"></script>    
    <script src="../data/js/bootstrap-datetimepicker.zh-CN.js"></script>
    <script src="/Support/js/factorConfig.js"></script>
    <script src="/Support/js/factorUnitConfig.js"></script>
    <script src="/Support/js/DTU.js"></script>
    <script src="/Support/js/sensorConfig.js"></script>
    <script src="/Support/js/threshold.js"></script>
     <script src="js/sensorGroup.js"></script>
    <script src="js/validatorConfig.js"></script>
    <script src="js/aggregateConfig.js"></script>
    <%--<script>
        $(function () {
            //加载数据然后.chosen()
            /* no_results_text 无搜刮成果显示的文本
               allow_single_de 是否容许作废选择
               max_ed_options 当为多选时，最多选择个数    */
            var option;
            for (var i = 0; i < 5; i++) {
                option+= '<option>何足道'+i+'</option>';
            }
            $("#test").html(option);
            $('#test').chosen({
                no_results_text: "没有找到",
                allow_single_de: true
            });

            setTimeout(function () {
                //清楚内容前的操作，清楚后重新.chosen()
                $("#test").parent().children().remove('div');
                $("#test").removeClass();
                var option;
                for (var i = 0; i < 5; i++) {
                    option += '<option>呵呵' + i + '</option>';
                }
                $("#test").html(option);
                $("#test").addClass("chzn-select");

                //加载默认选项
                $("#test").val("呵呵1");
                $("#test").chosen({
                    no_results_text: "没有找到",
                    allow_single_de: true
                });             
            }, 3000)
        })
        
    </script>--%>

    <%--<script>
        //jquery chosen操作范例！！！
        $(document).ready(function () {

            //初始化
            $(".chzn-select").chosen();

            //单选select 数据同步
            chose_get_ini('#dl_chose');

            //change 事件
            $('#dl_chose').change(function () {
                alert(chose_get_value('#dl_chose') + ' : ' + chose_get_text('#dl_chose'));
            });

            //多选select 数据同步
            chose_get_ini('#dl_chose2');

            //change 事件
            $('#dl_chose2').change(function () {
                alert(chose_get_value('#dl_chose2') + ' : ' + chose_get_text('#dl_chose2'));
            });
        });

        //select 数据同步
        function chose_get_ini(select) {
            $(select).chosen().change(function () { $(select).trigger("liszt:updated"); });
        }

        //单选select 数据初始化
        function chose_set_ini(select, value) {
            $(select).attr('value', value);
            $(select).trigger("liszt:updated");
        }

        //单选select value获取
        function chose_get_value(select) {
            return $(select).val();
        }

        //select text获取，多选时请注意
        function chose_get_text(select) {
            return $(select + " option:selected").text();
        }

        //多选select 数据初始化
        function chose_mult_set_ini(select, values) {
            var arr = values.split(',');
            var length = arr.length;
            var value = '';
            for (i = 0; i < length; i++) {
                value = arr[i];
                $(select + " [value='" + value + "']").attr('selected', 'selected');
            }
            $(select).trigger("liszt:updated");
        }

    </script>--%>
</asp:Content>
