<%@ Page Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="UserManger.aspx.cs" Inherits="SecureCloud.Support.UserManger" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
    <link href="/resource/library/data-tables/DT_bootstrap.css" rel="stylesheet" />
    <link href="/resource/css/unionboostrap.css" rel="stylesheet" />
    <link href="/resource/css/windowbox.css" rel="stylesheet" />
    <style>
        .width206 {
            width: 206px;
        }
        .width100 {
            width: 150px;
        }
        .bootstrap-select.btn-group.show-tick .dropdown-menu li.selected a i.check-mark {
            margin-top: -20px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="portlet box light-grey">
            <div class="portlet-title">
                <h4>
                    <img src="/resource/img/settings.png" style="width: 30px; height: 30px" />
                    用户管理</h4>
            </div>

            <div class="portlet-body" style="width: auto">
                <div class="clearfix">
                    <div class="row-fluid">
                        <div class="form-horizontal">
                            <div class="span1" style="margin: 0px 15px 10px 0px">
                                <input id="btnadd" type="button" class="btn blue editor_add" value="添加用户" href='#addUser' data-toggle='modal' />
                            </div>
                        </div>
                    </div>
                </div>

                <table id="userTable" class="table table-striped table-bordered">
                    <thead>
                        <tr>
                            <th style="width:10%;">用户名</th>
                            <%--<th>密码</th>--%>
                            <th style="width:10%;">角色</th>
                            <th style="width:10%;">邮箱</th>
                            <th style="width:10%;">联系电话</th>
                            <th style="width:20%;">关注组织</th>
                            <th style="width:20%;">关注结构物</th>
                            <%--<th style="width:100px;">系统名称</th>--%>
                            <th style="width:10%;">操作</th>
                        </tr>
                    </thead>
                    <tbody id="userTbody">
                        <%--<tr>
                            <td>admin</td>
                            <td>admin</td>
                            <td>普通用户</td>
                            <td>1@2/com</td>
                            <td>11111</td>
                            <td>安徽高速</td>
                            <td>K765,k775</td>
                            <td>安徽高速管理系统</td>
                            <td><a href='#'>修改</a> |<a href='#'>删除</a></td>
                        </tr>--%>
                    </tbody>
                </table>

            </div>
        </div>
    </div>

    <!--添加用户-->
    <form id="addUserForm">
        <div id="addUser" class="modal modal800 hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="width: 700px;">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>增加用户</h3>
            </div>
            <div class="modal-body" style="max-height: 520px;">
                <div class="form-horizontal">

                    <div class="control-group">
                        <label class="control-label" for="UserName">用户名</label>
                        <div class="controls">
                            <input type="text" id="UserName" placeholder="用户名" onblur="checkUserName(this)" title="用户名为数字、字母和下划线" pattern="^[a-z0-9_-]{1,15}$" maxlength="15" />
                            <font class="red" style="color: red">*6-15位数字、字母和下划线</font>
                            <label class="label label-mini label-success" style="display:none">可以使用</label>
                            <label class="label label-mini label-important" style="display:none">用户名被占用(如有疑问请与管理员联系)</label>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="Password">密码</label>
                        <div class="controls">
                            <input type="password" id="Password" placeholder="" title="" pattern="^[a-zA-Z0-9]{1,15}$" maxlength="15" />
                            <font class="red" style="color: red">*6-15位数字、字母和下划线</font>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="ConfirmPassword">确认密码</label>
                        <div class="controls">
                            <input type="password" id="ConfirmPassword" placeholder="" title="" pattern="^[a-zA-Z0-9]{1,15}$" maxlength="15" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="UserRole">角色</label>
                        <div class="controls">
                            <select id="UserRole" class="width100 selectpicker" data-size="10" title="请选择">
                            </select>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="UserEmail">邮箱</label>
                        <div class="controls">
                            <input type="text" id="UserEmail" placeholder="邮箱" title="" pattern="^([a-zA-Z0-9]+[_|\_|\.|\-]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.|\-]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$" maxlength="50" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="UserPhone">联系电话</label>
                        <div class="controls">
                            <input type="text" id="UserPhone" placeholder="联系电话" title="" pattern="(^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$)|(^0{0,1}1[3|4|5|6|7|8|9][0-9]{9}$)" maxlength="20" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="beOrgNameList">归属组织</label>
                        <div class="controls">
                            <select id="beOrgNameList" class="width100 selectpicker" style="width: 300px;" data-size="10" title="请选择">
                            </select>
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label" for="OrgNameList">关注组织</label>
                        <div class="controls">
                            <select id="OrgNameList" class="width100 selectpicker" multiple="multiple" style="width: 300px;" data-size="10" title="请选择">
                            </select>  
                            <input type="checkbox" class="checkbox" id="orgCheckAll" value="0"/> 组织全选
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="StructNameList">关注结构物</label>
                        <div class="controls">
                            <select id="StructNameList" class="width100 selectpicker" multiple="multiple" style="width: 300px;" data-size="10" title="请选择">
                            </select>
                            <input type="checkbox" class="checkbox" id="structCheckAll" value="1"/> 结构物全选
                        </div>
                    </div>

                    <%--<div class="control-group">
                        <label class="control-label" for="UserSystemName">系统名称</label>
                        <div class="controls">
                            <input type="text" id="UserSystemName" placeholder="用户登录后的系统名称" maxlength="40" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>--%>

                </div>
            </div>
            <div class="modal-footer">
                <button id="btnCancel_add" class="btn" data-dismiss="modal">关闭</button>
                <input id="btnReset_add" type="button" value="重置" class="btn" />
                <input id="btnSave" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>
    </form>


    <!--修改用户-->
    <form id="Form1">
        <div id="editUser" class="modal modal800 hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="width: 700px;">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>修改用户</h3>
            </div>
            <div class="modal-body" style="max-height: 520px;">
                <div class="form-horizontal">

                    <div class="control-group">
                        <label class="control-label" for="UserName_edit">用户名</label>
                        <div class="controls">
                            <input type="text" id="UserName_edit" placeholder="用户名" readonly="true" />                           
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="Password_edit">密码</label>
                        <div class="controls">
                            <input type="password" id="Password_edit" placeholder="" title="" pattern="^[a-zA-Z0-9]{1,15}$" maxlength="15" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="ConfirmPassword_edit">确认密码</label>
                        <div class="controls">
                            <input type="password" id="ConfirmPassword_edit" placeholder="" title="" pattern="^[a-zA-Z0-9]{1,15}$" maxlength="15" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="UserRole_edit">角色</label>
                        <div class="controls">
                            <select id="UserRole_edit" class="width100 selectpicker" data-size="10" title="请选择">
                            </select>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="UserEmail_edit">邮箱</label>
                        <div class="controls">
                            <input type="text" id="UserEmail_edit" placeholder="邮箱" title="" pattern="^([a-zA-Z0-9]+[_|\_|\.|\-]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.|\-]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="UserPhone_edit">联系电话</label>
                        <div class="controls">
                            <input type="text" id="UserPhone_edit" placeholder="联系电话" title="" pattern="(^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$)|(^0{0,1}1[3|4|5|6|7|8|9][0-9]{9}$)" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="beOrgNameList_edit">归属组织</label>
                        <div class="controls">
                           <input type="text" id="beOrgNameList_edit" placeholder="归属组织" readonly="true" />     
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="OrgNameList_edit">关注组织</label>
                        <div class="controls">
                            <select id="OrgNameList_edit" class="width100 selectpicker" multiple="multiple" style="width: 300px;" data-size="10" title="请选择">
                            </select>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="StructNameList_edit">关注结构物</label>
                        <div class="controls">
                            <select id="StructNameList_edit" class="width100 selectpicker" multiple="multiple" style="width: 300px;" data-size="10" title="请选择">
                            </select>
                        </div>
                    </div>

                    <%--<div class="control-group">
                        <label class="control-label" for="UserSystemName_edit">系统名称</label>
                        <div class="controls">
                            <input type="text" id="UserSystemName_edit" placeholder="用户登录后的系统名称" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>--%>

                </div>
            </div>
            <div class="modal-footer">
                <button id="btnCancel_edit" class="btn" data-dismiss="modal">关闭</button>
                <input id="btnReset_edit" type="button" value="重置" class="btn" />
                <input id="btnSave_edit" type="button" value="保存修改" class="btn btn-primary" />
            </div>
        </div>
    </form>

    <!--删除用户-->
    <div id="deleteUser" class="modal hide fade">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>删除用户</h3>
        </div>
        <div class="modal-body">
            <p id="alertMsg"></p>
        </div>
        <div class="modal-footer">
            <button id="btnCancel" class="btn" data-dismiss="modal">取消</button>
            <a id="btnDelete" class="btn red">确定</a>
        </div>
    </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script type="text/javascript" src="/resource/js/jquery-2.0.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script src="/resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script src="/resource/library/bootstrap/js/bootstrap-dropdown.js"></script>
    <script src="/resource/library/data-tables/js/jquery.dataTables.js"></script>
    <script src="/resource/library/data-tables/DT_bootstrap.js"></script>
    
    <script src="js/UserManger.js"></script>
</asp:Content>
