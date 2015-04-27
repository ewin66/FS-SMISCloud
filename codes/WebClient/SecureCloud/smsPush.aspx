<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="smsPush.aspx.cs" Inherits="SecureCloud.smsPush" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="resource/library/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" />
    <link href="resource/library/data-tables/DT_bootstrap.css" rel="stylesheet" />
    <link href="/resource/library/bootstrap/css/bootstrap-select.css" rel="stylesheet" />
    <link href="/resource/css/unionboostrap.css" rel="stylesheet" />
    <link href="/resource/css/windowbox.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="portlet box light-grey">
                <div class="portlet-title">
                    <h4><i class="icon-mobile-phone"></i>预警设置</h4>
                </div>
                <div class="portlet-body">
                    <div class="clearfix">
                        <div class="row-fluid">
                            <div class="span3">
                                <div class="form-horizontal">
                                    <div class="span1" style="margin: 0px 15px 10px 0px">
                                        <input id="btnadd" type="button" class="btn blue" value="添加接收人" href='#addParm_alert' data-toggle='modal' />
                                    </div>
                                </div>
                            </div>
                            <div class="span7"></div>
                            <div id="showMsg" class="span2"></div>
                        </div>
                    </div>

                    <table class="table table-striped table-bordered table-hover" id="parm_alert_table">
                        <thead>
                            <tr>
                                <th>接收人姓名</th>
                                <th>接收人电话</th>
                                <th>接收人邮箱</th>
                                <th>接收方式</th>
                                <th>接受告警级别</th>
                                <th>角色</th>                               
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody id="parm_alert_tbody">
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>

    <form id="addParm_alertForm">
        <div id="addParm_alert" class="modal modal800 hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" >
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                <h3>增加短信接收人</h3>
            </div>
            <div class="modal-body" style="max-height: 520px;">
                <div class="form-horizontal">

                    <div class="control-group">
                        <label class="control-label" for="Parm_alertName">短信接收人姓名</label>
                        <div class="controls">
                            <input type="text" id="Parm_alertName" placeholder="" maxlength="15" />
                            <font class="red" style="color: red">*</font>                           
                        </div>
                    </div>
                  
                    <div class="control-group">
                        <label class="control-label" for="Parm_alertPhone">接收人电话</label>
                        <div class="controls">
                            <input type="text" id="Parm_alertPhone" placeholder="联系电话" title="" pattern="(^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$)|(^0{0,1}1[3|4|5|6|7|8|9][0-9]{9}$)" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="Parm_alertMial">接收人邮箱</label>
                        <div class="controls">
                            <input type="text" id="Parm_alertMial" placeholder="" title="联系邮箱" pattern="^([a-zA-Z0-9]+[_|\_|\.|\-]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.|\-]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$" />
                            <font class="red" style="color: red">*</font>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="Parm_alertMode">接收人接收方式</label>
                        <div class="controls">
                            <select id="Parm_alertMode" class="width100 selectpicker" data-size="10" title="请选择">
                                <option value="true">短信</option>
                                <option value="false">邮箱</option>                               
                            </select>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="Parm_alertLevel">接收等级</label>
                        <div class="controls">
                            <select id="Parm_alertLevel" class="width100 selectpicker" data-size="10" title="请选择">
                                <option value="1">一级</option>
                                <option value="2">二级</option>
                                <option value="3">三级</option>
                                <option value="4">四级</option>
                            </select>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label" for="Parm_alertRole">角色</label>
                        <div class="controls">
                            <select id="Parm_alertRole" class="width100 selectpicker" data-size="10" title="请选择">
                                <option value="1">用户</option>
                                <option value="0">技术支持</option>
                            </select>
                        </div>
                    </div>

                </div>
            </div>
            <div class="modal-footer">
                <button id="btnCancel" class="btn" data-dismiss="modal">关闭</button>
                <input id="btnReset" type="button" value="重置" class="btn" />
                <input id="btnSave" type="button" value="保存" class="btn btn-primary" />
            </div>
        </div>
    </form>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jqueryversion" runat="server">
    <script src="resource/js/jquery-1.8.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script src="/resource/library/data-tables/jquery.dataTables.js"></script>
    <script src="/resource/library/data-tables/DT_bootstrap.js"></script>
    <script src="/resource/library/bootstrap/js/bootstrap-select.js"></script>
    <script src="/resource/library/bootstrap/js/bootstrap-dropdown.js"></script>  
    <script>
        jQuery(document).ready(function () {
            // initiate layout and plugins
            App.setPage("other");
            App.init();
        });
    </script>
    <script src="/resource/js/Param_Alert.js"></script>

    
</asp:Content>

