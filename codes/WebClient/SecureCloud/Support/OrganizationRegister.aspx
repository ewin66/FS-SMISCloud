<%@ Page Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="OrganizationRegister.aspx.cs" Inherits="SecureCloud.Support.OrganizationRegister" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../resource/library/data-tables/DT_bootstrap.css" rel="stylesheet" />
    <style>
        .width206 {
            width: 206px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="portlet box light-grey">
                <div class="portlet-title">
                    <h4>
                        <img src="/resource/img/settings.png" style="width: 30px; height: 30px" />
                        组织管理</h4>
                </div>

                <div class="portlet-body">
                    <div class="clearfix">
                        <div class="row-fluid">
                            <div class="form-horizontal">
                                <div class="span1" style="margin: 0px 15px 10px 0px">
                                    <input id="btadd" type="button" class="btn blue editor_add" value="注册组织" href='#addOrganiztionModal' data-toggle='modal' />
                                </div>
                            </div>
                        </div>
                    </div>

                    <table id="organizationTable" class="table table-striped table-bordered table-hover">
                        <thead>
                            <tr>
                                <th style="width: 15%">组织名称</th>
                                <th style="width: 15%">省、市、县</th>
                                <th>街道地址</th>
                                <th>邮政编码</th>
                                <th>联系电话</th>
                                <th>传真</th>
                                <th>网站</th>
                                <th style="width: 10%">系统名称</th>
                                <th style="width: 10%">系统简称</th>
                                <th style="width: 5%">组织Logo配置</th>
                                <th>操作</th>
                                <th style="width: 5%">结构物配置</th>
                            </tr>
                        </thead>
                        <tbody id="organizationTbody"></tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>

    <!-- 添加组织 -->
    <form id="addOrganizationForm" style="overflow-x: hidden; overflow-y: hidden;">
        <div id="addOrganiztionModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3 id="addOrganizationModalTitle">增加组织</h3>
            </div>
            <div class="modal-body" style="max-height: 400px;">
                <div class="form-horizontal">

                    <div id="Organization" class="control-group">
                        <label class="control-label">组织名称</label>
                        <div class="controls">
                            <input type="text" id="organizationName" name="organizationName" placeholder="组织名称" title="" pattern="^[a-zA-Z0-9\u4E00-\u9FA5\-]+$" class="width206" maxlength="40" />
                            <font class="red" style="color: red">*</font>
                            <label class="label label-mini label-important">保存后不可修改</label>
                        </div>
                    </div>

                    <div id="viewOrganization" class="control-group">
                        <label class="control-label">组织名称</label>
                        <div class="controls">
                            <input type="text" id="organizationNameview" name="organizationNameview" placeholder="组织名称" class="width206" readonly="true" />
                        </div>
                    </div>

                    <fieldset id="region">
                        <div class="control-group">
                            <label class="control-label">组织所在省</label>
                            <div class="controls" id="provinceList">
                                <select class="province" id="province" name="province"></select>
                            </div>
                        </div>
                        <div class="control-group">
                            <label class="control-label">组织所在市</label>
                            <div class="controls">
                                <select class="city" id="city" name="city"></select>
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label">组织所在县</label>
                            <div class="controls">
                                <select class="area" id="county" name="county"></select>
                            </div>
                        </div>
                    </fieldset>

                    <div class="control-group">
                        <label class="control-label" for="road">街道地址</label>
                        <div class="controls">
                            <input type="text" id="address" name="address" placeholder="" class="width206" maxlength="50" />
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="zip">邮政编码</label>
                        <div class="controls">
                            <input type="text" id="zipcode" name="zipcode" placeholder="" class="width206" pattern="^\d{6}$" title="6位数字" maxlength="6" />
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="mobile">联系电话</label>
                        <div class="controls">
                            <input type="text" id="phone" name="phone" placeholder="" class="width206" title="座机或者手机号码" pattern="(^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$)|(^0{0,1}1[3|4|5|6|7|8|9][0-9]{9}$)" maxlength="30" />
                        </div>
                    </div>

                    <div id="organizationfax" class="control-group">
                        <label class="control-label" for="fax">&nbsp;&nbsp;传&nbsp;&nbsp;真</label>
                        <div class="controls">
                            <input type="text" id="faxnum" name="faxnum" placeholder="" class="width206" maxlength="30" title="" pattern="^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$" />
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="web">&nbsp;&nbsp;网&nbsp;&nbsp;站</label>
                        <div class="controls">
                            <input type="text" id="website" name="website" placeholder="" class="width206" maxlength="30" />
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="OrgSystemName">系统名称</label>
                        <div class="controls">
                            <input type="text" id="OrgSystemName" placeholder="登录后的系统名称" maxlength="40" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>
                    
                    <div class="control-group">
                        <label class="control-label" for="systemAbbreviation">系统简称</label>
                        <div class="controls">
                            <input type="text" id="systemAbbreviation" placeholder="项目仪表盘中的系统简称" maxlength="10" />
                            <span class="red" style="color: red">* (字数不超过10个)</span>
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
    </form>

    <!-- 查看组织 -->
    <div id="viewOrganizationModal" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>查看组织</h3>
        </div>
        <div class="modal-body">
            <table id="manuscriptInfo" class="table table-striped table-bordered" style="width: 100%;">
                <thead>
                    <tr>
                        <th>组织名称</th>
                        <th>结构物名称</th>
                        <th>结构物类型</th>
                        <th>施工单位</th>
                        <th>项目描述</th>
                    </tr>
                </thead>
                <tbody id="viewOrganiztionTbody"></tbody>
            </table>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">关闭</button>
        </div>
    </div>

    <!-- 删除组织Modal Delete Organization -->
    <div id="deleteOrganizationModal" class="modal hide fade" style="width: 400px">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>删除组织</h3>
        </div>
        <div class="modal-body">
            <p id="alertMsg">确认要删除行?</p>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal">取消</button>
            <a href="#" id="btnDelete" class="btn red">确定</a>
        </div>
    </div>

    <div id="uploadOrgLogo" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>上传组织logo</h3>
        </div>
        <div class="modal-body">
            <div class="form-horizontal">
                <div class="control-group" align="center">
                    <div id="imgContain" style="width: 170px; height: 50px;">
                        <%--<img id="imgOrgLogo" src="" style="width: 160px; height: 40px;" />--%>
                    </div>
                </div>
            </div>
            <input type="file" id="fulAchievements" name="fulAchievements" value="浏览" />
            <div class="form-horizontal">
                <div class="control-group" align="left">
                    <input id="btnUp" type="button" value="上传" class="btn btn-primary blue" style="margin-right: 10px;" />
                </div>
            </div>

        </div>

        <div class="modal-footer">
            <button id="btnCloseLogo" class="btn" data-dismiss="modal">取消</button>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script type="text/javascript" src="/resource/js/jquery-2.0.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script src="../resource/library/data-tables/js/jquery.dataTables.js"></script>
    <script src="../resource/library/data-tables/DT_bootstrap.js"></script>
    <script src="js/ajaxfileupload.js"></script>
    <script src="js/Organization.js"></script>


</asp:Content>

