
// 页面初始
$(function () {
    $('#dataServices').addClass('active');
    $('#dataDownLoad').addClass('active');
    structView();
    initDateTime();
});

//刷新监测因素列表
function refreshFactorList() {
    var obj = document.getElementById("structList");

    if (obj == null || obj.length == 0) {
        document.getElementById("factorBox").style.display = "none";
        return;
    } else {
        document.getElementById("factorBox").style.display = "";
    }

    var structId = parseInt(obj.options[obj.selectedIndex].id);
   // var structId = struct.;
    factorListView(structId);
}

// 显示监测因素列表
function factorListView(structId) {
    var url = apiurl + '/struct/' + structId + '/factor-config' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.length >= 1) {
                var sb = new StringBuffer();
                var bFirst = true;
                for (var i = 0; i < data.length; i++) {
                    //sb.append('<table><tr><th><b>' + data[i].factorName + ':</b></th>');
                    bFirst = true;
                    for (var j = 0; j < data[i].children.length; j++) {
                        if (data[i].children[j].choose) {
                            if (bFirst) {
                                sb.append('<table><tr><th><b>' + data[i].factorName + ':</b></th>');
                                bFirst = false;
                            }
                            sb.append('<th style="width: 100px;"><input type="checkbox" class="checkboxes" name="factorItems" checked="checked" value="' + data[i].children[j].factorId + '" /><font style="font-weight: normal;">' + data[i].children[j].factorName);
                        }

                        if (data[i].children[j].description != null) {
                            sb.append('(' + data[i].children[j].description + ')');
                        }
                        sb.append('</font></th>');
                    }
                    sb.append('</tr></table>');
                    if (i != data.length - 1 && !bFirst) {
                        sb.append('<hr style="border: 1px #cccccc dashed;" />');
                    }
                }
                $("#factorList").html(sb.toString());
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("权限验证出错");
                logOut();
            }
            else if (XMLHttpRequest.status == 400) {
                alert("参数错误");
            }
            else if (XMLHttpRequest.status == 500) {
                alert("内部异常");
            }
            else {
                alert('url错误');
            }
        }
    });
}

//显示结构物列表
function structView() {
    var url = apiurl + '/user/' + userId + '/struct/list' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                $('#structList').html("无");
                return;
            }
            try {
                var sb = new StringBuffer();
                $.each(data, function(index, item) {
                    sb.append('<option id="' + item.structId + '">' + item.structName + '</option>');
                });
                $('#structList').html(sb.toString());

                $('#structList').chosen({
                    //max_selected_options: 1,
                    no_results_text: "没有找到",
                    allow_single_de: true
                });

                refreshFactorList();

            } catch(err) {
                alert(err);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("权限验证出错");
                logOut();
            }
            else if (XMLHttpRequest.status == 400) {
                alert("参数错误");
            }
            else if (XMLHttpRequest.status == 500) {
                alert("内部异常");
            }
            else {
                alert('url错误');
            }
        }
    });
}

//确定时间
function showdate(n) {
    var uom = new Date();
    uom.setDate(uom.getDate() + n);

    uom = uom.getFullYear() + "-" + (uom.getMonth() + 1) + "-" + uom.getDate();
    return uom.replace(/\b(\w)\b/g, '0$1');
}

function initDateTime() {
    $('.input-append ').datetimepicker({
        format: 'yyyy-MM-dd',
        language: 'pt-BR'
    });
    $("#dpfrom").val(showdate(-1));
    $("#dpend").val(showdate(0));
}

$('#structList').change(function () {
    refreshFactorList();
});

//全选  
$("#CheckedAll").click(function () {
    $('[name=factorItems]:checkbox').attr('checked', true);
});
//全不选  
$("#CheckedNo").click(function () {
    $('[name=factorItems]:checkbox').attr('checked', false);
});





