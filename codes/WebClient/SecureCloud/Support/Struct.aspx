<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Support/SiteSupport.Master" CodeBehind="Struct.aspx.cs" Inherits="SecureCloud.Support.Struct" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../resource/library/data-tables/DT_bootstrap.css" rel="stylesheet" />
    <%--<link href="../resource/library/bootstrap/css/bootstrap-cerulean.css" rel="stylesheet" />--%>
    <%--       <script src="../resource/js/jquery-1.9.1.min.js"></script>--%>
    <style>
        .width206 {
            width: 206px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="addStructureform" runat="server">
        <div class="container-fluid">

            <div class="portlet box light-grey">
                <div class="portlet-title">
                    <h4>
                        <img src="/resource/img/settings.png" style="width: 30px; height: 30px" />
                        结构物管理</h4>
                </div>

                <div class="portlet-body" style="width: auto">
                    <div class="clearfix">
                        <div class="row-fluid">
                            <div class="form-horizontal">
                                <div class="span1" style="margin: 0px 15px 10px 0px">
                                    <input id="btadd" type="button" class="btn blue editor_add" value="增加结构物" href='#addStructModal' data-toggle='modal' />
                                </div>
                            </div>
                        </div>
                    </div>

                    <table id="structTable" class="table table-striped table-bordered" style="width: 100%;">
                        <thead>
                            <tr>
                                <th>结构物名称</th>
                                <th>结构物类型</th>
                                <th>项目状态</th>
                                <th>施工单位</th>                                
                                <th>操作</th>
                                <th>方案配置</th>
                                <th>热点图配置</th>
                                <th>施工线路|截面配置</th>
                            </tr>
                        </thead>
                        <tbody id="StructTbody"></tbody>
                    </table>

                </div>
            </div>
        </div>

        <!-- 添加结构物 -->
        <div id="addStructModal" class="modal modal800 hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3 id="addStructureModalTitle">增加结构物</h3>
            </div>
            <div class="modal-body" style="max-height: 400px;">
                <div class="form-horizontal">

                    <div id="organizationStructSelect" class="control-group">
                        <label class="control-label">组织名称</label>
                        <div class="controls">
                            <select id="organizationName" name="organizationName">
                            </select>
                        </div>
                    </div>

                    <div id="organizationStruct" class="control-group">
                        <label class="control-label" for="road">组织名称</label>
                        <div class="controls">
                            <input type="text" id="structNameView" name="structNameView" placeholder="" class="width206" readonly="true" />
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="road">结构物名称</label>
                        <div class="controls">
                            <input type="text" id="structName" name="structName" placeholder="结构物名称" class="width206" maxlength="50" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="china">结构物类型</label>
                        <div class="controls">
                            <select id="structType" name="structType">
                            </select>
                        </div>
                    </div>

                    <fieldset id="region">
                        <div class="control-group">
                            <label class="control-label">结构物所在省</label>
                            <div class="controls">
                                <select class="province" id="province" name="province"></select>
                            </div>
                        </div>
                        <div class="control-group">
                            <label class="control-label">结构物所在市</label>
                            <div class="controls">
                                <select class="city" id="city" name="city"></select>
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label">结构物所在县</label>
                            <div class="controls">
                                <select class="area" id="county" name="county"></select>
                            </div>
                        </div>
                    </fieldset>

                    <div id="structAddress" class="control-group">
                        <label class="control-label" for="road">地址</label>
                        <div class="controls">
                            <input type="text" id="address" name="address" placeholder="地址" class="width206" maxlength="50" />
                        </div>
                    </div>

                    <div id="structLongitude" class="control-group">
                        <label class="control-label" for="zip">经度</label>
                        <div class="controls">
                            <input type="text" id="longitude" name="longitude" placeholder="经度" class="width206" maxlength="30" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>

                    <div id="structLatitude" class="control-group">
                        <label class="control-label" for="mobile">纬度</label>
                        <div class="controls">
                            <input type="text" id="latitude" name="latitude" placeholder="纬度" class="width206" maxlength="30" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label">项目状态</label>
                        <div class="controls">
                            <select data-size="10" title="请选择" id="projectStatus">
                                <option value="1">运营期</option>
                                <option value="0">施工期</option>
                            </select>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label">施工单位</label>
                        <div class="controls">
                            <input type="text" id="companyName" name="companyName" placeholder="" class="width206" maxlength="50" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label">项目描述</label>
                        <div class="controls">
                            <%--<input type="text" id="description" name="description" style="width:206px;height:206px" />--%>
                            <textarea id="description" rows="10" cols="30" style="width: 206px; height: 206px" placeholder="" maxlength="100"></textarea>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">关闭</button>
                <input id="btnReset" type="button" value="重置" class="btn" />
                <input id="btnSave" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>


        <!-- 查看结构物 -->
        <div id="viewStructModal" class="modal hide fade">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>查看结构物</h3>
            </div>
            <div class="modal-body">
                <table id="manuscriptInfo">
                    <tr>
                        <td><strong>结构物名称:</strong></td>
                        <td id="tdOrgnName"></td>
                    </tr>
                    <tr>
                        <td><strong>DTU:</strong></td>
                        <td id="Td1"></td>
                    </tr>
                    <tr>
                        <td><strong>传感器:</strong></td>
                        <td id="tdSensor"></td>
                    </tr>
                </table>
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">关闭</button>
            </div>
        </div>

        <!-- 编辑结构物 -->
        <div id="editModal" class="hide fade">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>编辑结构物</h3>
            </div>
            <div class="modal-body" style="max-height: 400px;">
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">关闭</button>
                <input id="Reset2" type="reset" value="重置" class="btn" />
                <input id="Submit2" type="submit" value="保存" class="btn btn-primary" />
            </div>
        </div>

        <!-- 删除结构物Modal Delete Struct -->
        <div id="deleteStructModal" class="modal hide fade">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>删除结构物</h3>
            </div>
            <div class="modal-body">
                <p id="alertMsg"></p>
            </div>
            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">取消</button>
                <a href="#" id="btnDelete" class="btn red">确定</a>
            </div>
        </div>


        <!-- 上传结构物热点图 Modal uploading StructImg -->
        <div id="uploadingImgModal" class="modal hide fade">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>上传结构物热点图</h3>
            </div>
            <div class="modal-body" style="max-height: 500px; max-width: 500px;">
                <div class="form-horizontal">
                    <div class="control-group" align="center">
                        <div id="imgContain" style="width: 400px; height: 200px;">
                            <img id="imgHotspot" style="width: 400px; height: 200px;" />
                        </div>
                    </div>
                </div>
                <input type="file" id="fulAchievements" name="fulAchievements" value="浏览" />
                <div class="form-horizontal">
                    <div class="control-group" align="left">
                        <input id="btnUp" type="button" value="上传" class="btn btn-primary blue" style="margin-right: 10px;" />
                        <%--<input id="btnSaveHot" type="button" value="保存" class="btn btn-primary blue" style="margin-right: 10px;" />--%>
                        <input id="btnstationing" type="button" value="布点" class="btn btn-primary blue" />
                    </div>
                </div>

            </div>

            <div class="modal-footer">
                <button class="btn" data-dismiss="modal">取消</button>
            </div>
        </div>

        <asp:HiddenField ID="HiddenFlag" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenOrgName" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HiddenOrgId" runat="server" ClientIDMode="Static" />
    </form>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script type="text/javascript" src="/resource/js/jquery-2.0.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script src="../resource/library/data-tables/js/jquery.dataTables.js"></script>
    <script src="../resource/library/data-tables/DT_bootstrap.js"></script>
    <script src="js/ajaxfileupload.js"></script>
    <script src="js/Struct.js"></script>

</asp:Content>
