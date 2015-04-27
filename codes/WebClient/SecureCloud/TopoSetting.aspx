<%@ Page Language="C#" MasterPageFile="~/Support/SiteSupport.Master" AutoEventWireup="true" CodeBehind="TopoSetting.aspx.cs" Inherits="SecureCloud.TopoSetting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row-fluid">
        <div class="box">
            <div class="box-header">
                <div class="box-icon">
                    <h3 class="page-title">热点图动态配置</h3>
                </div>
            </div>
            <div class="box-content">
                <div class="row-fluid">
                    <div class="form-horizontal">
                        <div class="span9">
                            <div class="row-fluid" id="imgContain" style="position: relative;">
                                <ul id="theDropDownMenu" class="dropdown-menu" role="menu" style="position:absolute;">
                                  <li role="presentation"><a role="menuitem" tabindex="1" href="#"></a></li>
                                  <li role="presentation"><a role="menuitem" tabindex="2" href="#"></a></li>
                                </ul>
                                <img id="img" src="\" ondrop="drop(event)" ondragover="allowDrop(event)" style="width:100%;height:100%" />
                            </div>
                        </div>
                        <div class="span3">
                            <div class="box" style="margin-top: 20%;">
                                <div class="box-header">
                                    <h4 style="text-align: center">图例</h4>
                                </div>
                                <div class="box-content" style="overflow: auto; height: 400px">
                                    <table class="table table-striped table-bordered table-hover">
                                        <thead>
                                            <tr>
                                                <th>图片</th>
                                                <th>传感器名称</th>
                                                <th>位置描述</th>
                                            </tr>
                                        </thead>
                                        <tbody id="legend"></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
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
    <script type="text/javascript" src="/resource/js/jquery-2.0.3.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
    <script type="text/javascript" src="/resource/js/topoSetting.js"></script>

    <%--<script>
        var widthh, heightt;
        $(function () {
            $("#imgContain img").each(function () {
                var w = $("#imgContain").width();
                var img_w = $(this).width();//图片宽度 
                var img_h = $(this).height();//图片高度 

                var height = (w * img_h) / img_w; //高度等比缩放 
                $(this).css({ "width": w, "height": height });//设置缩放后的宽度和高度 

            });
            $("#img").click(function (e) {
                //offset()用法：获取匹配元素在当前视口的相对偏移
                widthh = e.pageX - $(this).offset().left || e.originalEvent.layerX - $(this).offset().left || 0;
                heightt = e.pageY - $(this).offset().top || e.originalEvent.layerY - $(this).offset().top || 0;
                var percent_x = widthh / $("#img").width();
                var percent_y = heightt / $("#img").height();
                $('#div_inc_x').html('X（百分比）' + percent_x.toFixed(2) * 100 + '%');
                $('#div_inc_y').html('Y:（百分比）' + percent_y.toFixed(2) * 100 + '%');

                //显示热点管理界面
                $("#imgmapModal").modal("show");

                setCookie("Percent_X", percent_x);
                setCookie("Percent_Y", percent_y);
            })

            $("#btnSave").click(function (e) {
                e.preventDefault();
                var hotpot = addHotpot(getCookie("Percent_X") * $("#img").width(), getCookie("Percent_Y") * $("#img").height(), $("#sensorList").val());
                $('#imgContain').append(hotpot);
                $("#imgmapModal").modal("hide");
            })
        })

        function addHotpot(LoactionX, LocationY, option) {
            if (option == "1") {
                var hotpot = "<a href='#' id='' class='marker' style='left:" + LoactionX + "px;top: " + LocationY + "px' onclick='delHotPot(" + LoactionX / $("#img").width() + "," + LocationY / $("#img").height() + ")'></a>";
            }
            else if (option == "2") {
                var hotpot = "<a href='#' id='' class='marker_square' style='left:" + LoactionX + "px;top: " + LocationY + "px' onclick='delHotPot(" + LoactionX / $("#img").width() + "," + LocationY / $("#img").height() + ")'></a>";
            }
            else {
                var hotpot = "<a href='#' id='' class='marker_triangle' style='left:" + LoactionX + "px;top: " + LocationY + "px' onclick='delHotPot(" + LoactionX / $("#img").width() + "," + LocationY / $("#img").height() + ")'></a>";
            }
            return hotpot;
        }

        function delHotPot(percent_x, percent_y) {
            $('#div_x').html('X（百分比）' + percent_x.toFixed(2) * 100 + '%');
            $('#div_y').html('Y:（百分比）' + percent_y.toFixed(2) * 100 + '%');

            $("#delhotpot").modal("show");
        }
    </script>--%>
</asp:Content>
