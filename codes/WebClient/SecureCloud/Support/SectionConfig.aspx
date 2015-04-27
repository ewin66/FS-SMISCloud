<%@ Page Title="" Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="SectionConfig.aspx.cs" Inherits="SecureCloud.Support.SectionConfig" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/resource/library/data-tables/DT_bootstrap.css" rel="stylesheet" />
    <link href="/resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />
    
    <style>
        .absolute-marker-loading {
            position: absolute;
            top: 50px;
            left: 200px;
            display: block;
            width: 100px;
            height: 100px;
            outline: none;
            background: url(/resource/img/ajax-loader.gif) no-repeat;
            cursor: pointer;
            z-index: 2;
        }

        #modal-sectionInfo .control-group {
            margin-bottom: 15px;
        }
        #modal-sectionInfo .control-label {
            width: auto;
        }
        #modal-sectionInfo .controls {
            margin-left: 60px;
        }

        #group-copySection .control-group {
            margin-bottom: 0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="portlet box light-grey">
                <div class="portlet-title">
                    <h4>
                        <img src="/resource/img/settings.png" alt="标题图片" style="width: 30px; height: 30px;" />
                        <span>施工线路/截面配置管理—</span><span id="sectionStructName"></span>
                    </h4>
                </div>
                <div class="portlet-body">
                    <div class="row-fluid">
                        <div class="span12">
                            <!--BEGIN TABS-->
                            <div class="tabbable tabbable-custom">
                                <ul class="nav nav-tabs">
                                    <li id="tabScheduleConfig" class="active"><a href="#tab-ScheduleConfig" data-toggle="tab" style="font-size: 16px">施工线路</a></li>
                                    <li id="tabSectionConfig"><a href="#tab-SectionConfig" data-toggle="tab" style="font-size: 16px;">施工截面</a></li>
                                </ul>
                                <div class="tab-content">
                                    <!--start tab_ScheduleConfig-->
                                    <div id="tab-ScheduleConfig" class="tab-pane row-fluid active">
                                        <div class="portlet-body">
                                            <div class="clearfix">
                                                <div class="row-fluid">
                                                    <div class="form-horizontal">
                                                        <div class="span2" style="margin-bottom: 10px;">
                                                            <input id="btnAddLine" type="button" class="btn blue" value="增加施工线路" href='#addScheduleModal' data-toggle='modal' style="display: block;" />
                                                        </div>
                                                        <div class="span10">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <table id="tableSchedule" width="100%" class="table table-striped table-bordered">
                                                <thead>
                                                    <tr >
                                                        <th style="display:none;">线路编号</th>
                                                        <th>线路名称</th>
                                                        <th>线路长度</th>
                                                        <th>开始位置Id</th>
                                                        <th>结束位置Id</th>
                                                        <th>线路颜色</th>
                                                        <th>操作</th>
                                                    </tr>
                                                </thead>
                                                <tbody id="tbodySchedule"></tbody>
                                            </table>
                                        </div>
                                    </div>
                                    <!--end tab_ScheduleConfig-->

                                    <!-- start section config -->
                                    <div id="tab-SectionConfig" class="tab-pane row-fluid">
                                        <div class="clearfix">
                                            <div class="row-fluid">
                                                <div id="tip-sectionConfig"></div>
                                                <div class="form-horizontal">
                                                    <div class="span2" style="margin-bottom: 10px;">
                                                        <input id="btnAddSection" type="button" class="btn blue" value="新增截面" href='#addSectionModal' data-toggle='modal' style="display: block;" />
                                                    </div>
                                                    <div class="span10">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                            
                                        <table id="tableSection" class="table table-striped table-bordered">
                                            <thead>
                                                <tr>
                                                    <th>截面</th>
                                                    <th>施工状态</th>
                                                    <th>操作</th>
                                                    <th>热点图</th>
                                                </tr>
                                            </thead>
                                            <tbody id="tbodySection">
                                            </tbody>
                                        </table>
                                    </div>
                                    <!-- end section config -->
                                </div>
                            </div>
                            <!--END TABS-->
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>  
   
    <!-- add Schedule model -->
    <div id="addScheduleModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="max-height:600px;">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3 id="addScheduleModalTitle">增加施工线路</h3>
        </div>
        <div class="modal-body" style="max-height: 400px;">
            <div class="form-horizontal">
                <%--<div class="control-group">
                    <label class="control-label">结构物名称</label>
                    <div class="controls">
                        <input type="text" id="addScheduleStruct" name="addSectionStruct" readonly="true" value="" />
                    </div>
                </div>--%>
                <div class="control-group" style="display:none;">
                    <label class="control-label">线路编号</label>
                    <div class="controls">
                        <input type="text" id="addScheduleLine" name="addSectionName" pattern="^\d{3}$" title="数字编号" placeholder="数字编号" maxlength="8"/>
                         <p id="addLineRepet" style="display: none"><span style="color: red;">此线路编号已被使用</span></p>
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">线路名称</label>
                    <div class="controls">
                        <input type="text" id="addLineName" name="addSectionName" />
                        <span style="color: red">*</span>                        
                    </div>
                </div> 
                <div class="control-group">
                    <label class="control-label">线路长度</label>
                    <div class="controls">
                        <input type="text" id="addLineLength" name="addSectionName" placeholder="数字"/>
                        <span style="color: red">*</span>
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">单位</label>
                    <div class="controls">
                        <select id="addLineUnit" name="modifyDTUFactory" class="chzn-select">
                            <option value="米">米</option>
                            <option value="千米">千米</option>                                
                        </select>
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">开始位置Id</label>
                    <div class="controls">
                        <input type="text" id="addScheduleInit_ID" name="addSectionName" title="数字编号" placeholder="数字"/>
                        <span style="color: red">*</span>
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">结束位置Id</label>
                    <div class="controls">
                        <input type="text" id="addScheduleEnd_ID" name="addSectionName" title="数字编号" placeholder="数字" />
                        <span style="color: red">*</span>
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">线路颜色</label>
                    <div class="controls">
                        <input type="text" id="addScheduleColor" name="addSectionName" title="16进制的RGB表达形式" placeholder="16进制的RGB表达形式" />
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <button id="btnCloseSchedule" class="btn" data-dismiss="modal">关闭</button>
            <input id="btnResetSchedule" type="button" value="重置" class="btn" />
            <%--<input id="btnSaveCloseSchedule" type="button" value="保存并关闭" class="btn btn-primary" />--%>
            <input id="btnSaveSchedule" type="button" value="保存" class="btn btn-primary" />
        </div>
    </div>
    <!-- end add Schedule model -->
    
    <!-- strat 修改 model -->
    <div id="modifyScheduleModal" class="modal hide fade">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>修改施工线路</h3>
            </div>
            <div class="modal-body">
                <div class="form-horizontal">
                    
                    <div class="control-group">
                        <label class="control-label">线路名称</label>
                        <div class="controls">                           
                        <input type="text" id="modifyScheduleLine" name="addSectionName" />
                        <%--<span style="color: red">*</span>--%>
                       
                    </div>                          
                    </div>
                    <div class="control-group">
                        <label class="control-label">线路长度</label>
                        <div class="controls">
                            <input type="text" id="modifyScheduleLength" name="addSectionName" placeholder="数字"/>
                            <%--<span style="color: red">*</span>--%>
                        </div>
                    </div>
                    
                      <div class="control-group">
                        <label class="control-label">长度单位</label>
                        <div class="controls">                           
                              <input id="modifyProgressUnit" name="modifyDTUFactory" type="text"   readonly="true"/>
                        </div>
                    </div>               
                    <div class="control-group">
                        <label class="control-label">开始位置Id</label>
                        <div class="controls">
                            <input id="modifyScheduleInit_id" name="modifyDTUport" type="text"  placeholder="数字"/>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">结束位置Id</label>
                        <div class="controls">
                            <input id="modifyScheduleEnd_id" name="modifyDTUip" type="text" placeholder="数字" />
                        </div>
                    </div> 
                    
                  <div class="control-group">
                        <label class="control-label">线路颜色</label>
                        <div class="controls">
                            <input type="text" id="modifyProgressColor" name="addSectionName"  title="16进制的RGB表达形式" placeholder="16进制的RGB表达形式" />

                        </div>
                    </div>                                      
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">关闭</button>
                <input id="btnResetModifySchedule" type="button" value="重置" class="btn" />
                <input id="btnSaveModifySchedule" type="button" value="保存并修改" class="btn btn-primary" />
            </div>
        </div>
    <!-- end 修改  model --> 
    
    <!-- strat 删除 model -->
    <div id="deleteLineModal" class="modal hide fade" style="width: 400px">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>删除施工线路</h3>
        </div>
        <div class="modal-body">
            <p id="alertMsg"></p>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">取消</button>
            <a href="#" id="btnLineDelete" class="btn red">确定</a>
        </div>
    </div>
    <!-- end 删除 model -->
    

    <!-- 查看施工截面 -->
    <div id="getSectionModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="max-height:600px;">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3 id="getSectionModalTitle"></h3>
        </div>
        <div class="modal-body" style="max-height: 400px;">
            <div class="form-horizontal">
                <div class="control-group">
                    <label class="control-label">截面</label>
                    <div class="controls">
                        <input type="text" id="getSectionName" name="getSectionName" readonly="readonly" />
                    </div>
                </div>
                <div class="control-group">
                    <label class="control-label">施工状态</label>
                    <div class="controls">
                        <input type="text" id="getSectionStatus" name="getSectionStatus" readonly="readonly" />
                    </div>
                </div>
                <div class="control-group">
                    <!-- 查看施工截面图 Modal -->
                    <div>
                        <h4>施工截面图</h4>
                    </div>
                    <div style="position: relative; max-height: 500px; max-width: 500px;">
                        <div id="viewSectionImgLoader" class="absolute-marker-loading" style="display: none;"></div>
                        <div class="form-horizontal">
                            <div class="control-group" align="center">
                                <div id="viewContainerImgSection" style="width: 400px; height: 200px;">
                                    <img id="viewImgSection" src="/" alt="此处为施工截面图" style="width: 400px; height: 200px;" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <button id="btnClose" class="btn" data-dismiss="modal">关闭</button>
        </div>
    </div>
    <!-- end 查看施工截面 -->

    <!-- add/modify Section modal -->
    <div id="addSectionModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="max-height:600px;">
        <div class="modal-header" style="padding-top: 0; padding-bottom: 0;">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h4 id="addSectionModalTitle"></h4>
        </div>
        <div id="modal-sectionInfo" class="modal-body" style="max-height: 460px; margin-bottom: 5px;">
            <div class="form-horizontal">
                <div id="express-configuration" class="control-group">
                    <button id="btnCopySection" type="button" class="btn red" style="display: none;">快速配置</button>
                    <label id="labelCopySection" class="control-label" style="display: none; color: green;">源截面</label>
                    <div id="controlsCopySection" class="controls" style="display: none;">
                        <select id="selectCopySections" name="selectCopySections" class="chzn-select" data-placeholder="请选择">
                        </select>
                    </div>
                </div>
                <div class="control-group">
                    <label id="textAddOrModifySection" class="control-label">截面</label>
                    <div class="controls">
                        <input type="text" id="addSectionName" name="addSectionName" />
                        <span style="color: red;">*</span>
                        <span style="margin-left: 65px;">施工状态</span>
                        <select id="addSectionStatus" name="addSectionStatus" class="chzn-select" style="width: 100px;">
                            <option value="0">未施工</option>
                            <option value="1" selected="selected">施工中</option>
                            <option value="2">施工完成</option>
                        </select>
                    </div>
                </div>
                <!-- 上传施工截面图 -->
                <div id="group-uploadSection" class="control-group">
                    <div>
                        <h5>上传施工截面图</h5>
                    </div>
                    <div style="position: relative; max-height: 500px; max-width: 500px;">
                        <div id="uploadSectionImgLoader" class="absolute-marker-loading" style="display: none;"></div>
                        <div class="form-horizontal">
                            <div class="control-group" align="center">
                                <div id="uploadContainerImgSection" style="position: relative; width: 516px; height: 200px;">
                                    <img id="uploadImgSection" src="/" alt="此处为施工截面图" style="width: 516px; height: 200px; color: green;" />
                                </div>
                            </div>
                            <input type="file" id="fileSection" name="fileSection" />
                            <div class="control-group" style="margin-bottom: 10px;">
                                <input type="button" id="btnUploadSection" value="上传" class="btn btn-primary blue" />
                            </div>
                        </div>
                    </div>
                </div>
                <%-- begin 快速配置(拷贝)截面 --%>
                <div id="group-copySection" class="control-group" style="position: relative; display: none;">
                    <div id="copySectionImgLoader" class="absolute-marker-loading" style="display: none;"></div>
                    <div class="form-horizontal">
                        <div class="control-group" align="center">
                            <div id="copyContainerImgSection" style="position: relative; width: 400px; height: 200px;">
                                <img id="copyImgSection" src="/" alt="此处为施工截面图" style="width: 400px; height: 200px;" />
                            </div>
                        </div>
                        <%-- begin 源传感器/目标传感器 --%>
                        <div id="group_copySensors_title" style="margin-top: 15px;">
                            <span class="span6">源传感器</span>
                            <span class="span6">目标传感器</span>
                        </div>
                        <div id="group_copySensors_content"></div>
                        <%-- end 源传感器/目标传感器 --%>
                    </div>
                </div>
                <%-- end copy Section --%>
            </div>
        </div>
        <div class="modal-footer">
            <button id="btnCloseAddSection" class="btn" data-dismiss="modal">关闭</button>
            <input id="btnResetAddSection" type="button" value="重置" class="btn" />
            <input id="btnSaveCloseAddSection" type="button" value="保存并关闭" class="btn btn-primary hide" />
            <input id="btnSaveAddSection" type="button" value="保存" class="btn btn-primary hide" />
            <input id="btnResetModifySection" type="button" value="重置" class="btn hide" />
            <input id="btnSaveCloseModifySection" type="button" value="保存并关闭" class="btn btn-primary hide" />
            <input id="btnSaveModifySection" type="button" value="保存" class="btn btn-primary hide" />
        </div>
    </div>
    <!-- end add Section modal --> 

    <!-- delete Section modal -->
    <div id="deleteSectionModal" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>删除施工截面</h3>
        </div>
        <div class="modal-body">
            <p id="alertMsgDelete"></p>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">取消</button>
            <a href="javascript:;" id="btnDeleteConfirm" class="btn red">确定</a>
        </div>
    </div>
    <!-- end delete Section modal --> 
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script type="text/javascript" src="/resource/js/jquery-1.8.3.min.js"></script> <%--<此版本支持chosen>--%>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script type="text/javascript" src="/resource/library/data-tables/js/jquery.dataTables.js"></script>
    <script type="text/javascript" src="/resource/library/data-tables/DT_bootstrap.js"></script>
    
    <script type="text/javascript" src="/resource/library/chosen-bootstrap/chosen/chosen.jquery.js"></script>
    
    <script type="text/javascript" src="js/ajaxfileupload.js"></script>
    <script type="text/javascript" src="js/scheduleConfig.js"></script>   
    <script type="text/javascript" src="js/sectionConfig.js"></script>
</asp:Content>
