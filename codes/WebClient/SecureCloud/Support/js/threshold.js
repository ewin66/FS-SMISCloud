
$(function () {
    ThresholdPageLoad();
    
    $('#tabThreshold').click(function () {
        ThresholdPageLoad();
    })
})

function ThresholdPageLoad() {
    var url = apiurl + '/struct/' + structId + '/factor-config' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data == null || data.length == 0) {
                $('#table-Threshold').dataTable().fnDestroy();
                $("#tbody-Threshold").html("");
                Table_To_datatable_Threshold();
                return;
            }
            var sb = new StringBuffer();
            var flag = true;
            var firstFactorId;
            for (var i = 0; i < data.length; i++) {
                for (var j = 0; j < data[i].children.length; j++) {
                    if (data[i].children[j].choose) {
                        if (flag) {
                            sb.append('<li class="active"><a href="#" onclick="tableThresholdShow(this,' + data[i].children[j].factorId + ')">' + data[i].children[j].factorName);
                            if (data[i].children[j].description != null) {
                                sb.append('(' + data[i].children[j].description + ')');
                            }
                            sb.append('</a></li>');

                            firstFactorId = data[i].children[j].factorId;
                        }
                        else {
                            sb.append('<li><a href="#" onclick="tableThresholdShow(this,' + data[i].children[j].factorId + ')">' + data[i].children[j].factorName);
                            if (data[i].children[j].description != null) {
                                sb.append('(' + data[i].children[j].description + ')');
                            }
                            sb.append('</a></li>');
                        }
                        flag = false;
                    }
                }
            }
            $("#ul-Threshold").html(sb.toString());
            if (firstFactorId != null&&firstFactorId!=undefined) {
                tableThresholdShow(firstFactorId);
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
    })


    //var url = apiurl + '/struct/' + structId + '/factors' + '?token=' + getCookie("token");
    //$.ajax({
    //    url: url,
    //    type: 'get',
    //    dataType: 'json',
    //    cache: false,
    //    success: function (data) {
    //        if (data.length >= 1) {
    //            var sb = new StringBuffer();
    //            for (var i = 0; i < data.length; i++) {
    //                for (var j = 0; j < data[i].children.length; j++) {
    //                    if (i == 0 && j == 0) {
    //                        sb.append('<li class="active"><a href="#" onclick="tableThresholdShow(this,' + data[i].children[j].factorId + ')">' + data[i].children[j].factorName + '</a></li>');
    //                    }
    //                    else {
    //                        sb.append('<li><a href="#" onclick="tableThresholdShow(this,' + data[i].children[j].factorId + ')">' + data[i].children[j].factorName + '</a></li>');
    //                    }
    //                }
    //            }
    //            $("#ul-Threshold").html(sb.toString());
    //            tableThresholdShow(data[0].children[0].factorId);
    //        }
    //    },
    //    error: function () {
    //        alert("加载监测因素列表出错（阈值）");
    //    }
    //})
}

function tableThresholdShow(dom, factorId) {
    //判断参数个数，相当于js的函数重载
    if (arguments.length == 2) {
        $(dom).parent().siblings().removeClass("active");
        $(dom).parent().addClass("active");

        //tbody填充内容之前销毁table
        $('#table-Threshold').dataTable().fnDestroy();
        var url = apiurl + '/struct/' + structId + '/factor/' + factorId + '/threshold' + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'get',
            dataType: 'json',
            cache: false,
            success: function (data) {
                if (data == null || data.length == 0) {
                    $("#tbody-Threshold").html("");
                    Table_To_datatable_Threshold();
                }
                if (data.length >= 1) {
                    var sb = new StringBuffer();

                    sb.append('<tr><td></td><td>全部</td>');
                    sb.append('<td><input name="moduleAdd" type="text" onblur="check(this)" /></td>');
                    sb.append('<td><input name="moduleAdd" type="text" onblur="check(this)" /></td>');
                    sb.append('<td><input name="moduleAdd" type="text" onblur="check(this)" /></td>');
                    sb.append('<td><input name="moduleAdd" type="text" onblur="check(this)" /></td>');
                    sb.append('<td><a href="#" onclick="SaveAllThreshold(this,' + factorId + ')">全部保存</a> | <a href="#" onclick="DeleteAllThreshold(' + factorId + ')">全部清除</a></td></tr>');
                    for (var i = 0; i < data.length; i++) {
                        sb.append('<tr><td>' + data[i].location + '</td>');
                        sb.append('<td>' + data[i].itemName + '</td>');
                        sb.append('<td><input name="moduleAdd" type="text" value="' + data[i].threshold[0].value + '" onblur="check(this)" /></td>');
                        sb.append('<td><input name="moduleAdd" type="text" value="' + data[i].threshold[1].value + '" onblur="check(this)" /></td>');
                        sb.append('<td><input name="moduleAdd" type="text" value="' + data[i].threshold[2].value + '" onblur="check(this)"  /></td>');
                        sb.append('<td><input name="moduleAdd" type="text" value="' + data[i].threshold[3].value + '" onblur="check(this)" /></td>');
                        sb.append('<td><a href="#" onclick="SaveThreshold(this,' + data[i].sensorId + ',' + data[i].itemId + ',' + factorId + ')">保存</a> | <a href="#" onclick="DeleteThreshold(' + data[i].sensorId + ',' + data[i].itemId + ',' + factorId + ')">清除</a></td></tr>');
                    }
                    $("#tbody-Threshold").html(sb.toString());
                    Table_To_datatable_Threshold();
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
        })
    }
    if (arguments.length == 1) {
        //arguments[0]表示第一个参数，需要把arguments保存下来给onclick事件，因为等点击的时候arguments的值已经改变     
        var factorId_temp = arguments[0];
        $('#table-Threshold').dataTable().fnDestroy();
        
        var url = apiurl + '/struct/' + structId + '/factor/' + factorId_temp + '/threshold' + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'get',
            dataType: 'json',
            cache: false,
            success: function (data) {
                if (data == null || data.length == 0) {
                    $("#tbody-Threshold").html("");
                    Table_To_datatable_Threshold();
                }
                if (data.length >= 1) {
                    var sb = new StringBuffer();

                    sb.append('<tr><td></td><td>全部</td>');
                    sb.append('<td><input name="moduleAdd" type="text" onblur="check(this)" /></td>');
                    sb.append('<td><input name="moduleAdd" type="text" onblur="check(this)" /></td>');
                    sb.append('<td><input name="moduleAdd" type="text" onblur="check(this)" /></td>');
                    sb.append('<td><input name="moduleAdd" type="text" onblur="check(this)" /></td>');
                    sb.append('<td><a href="#" onclick="SaveAllThreshold(this,' + factorId_temp + ')">全部保存</a> | <a href="#" onclick="DeleteAllThreshold(' + factorId_temp + ')">全部清除</a></td></tr>');
                    for (var i = 0; i < data.length; i++) {
                        sb.append('<tr><td>' + data[i].location + '</td>');
                        sb.append('<td>' + data[i].itemName + '</td>');
                        sb.append('<td><input name="moduleAdd" type="text" value="' + data[i].threshold[0].value + '" onblur="check(this)" /></td>');
                        sb.append('<td><input name="moduleAdd" type="text" value="' + data[i].threshold[1].value + '" onblur="check(this)"  /></td>');
                        sb.append('<td><input name="moduleAdd" type="text" value="' + data[i].threshold[2].value + '" onblur="check(this)" /></td>');
                        sb.append('<td><input name="moduleAdd" type="text" value="' + data[i].threshold[3].value + '" onblur="check(this)" /></td>');
                        sb.append('<td><a href="#" onclick="SaveThreshold(this,' + data[i].sensorId + ',' + data[i].itemId + ',' + factorId_temp + ')">保存</a> | <a href="#" onclick="DeleteThreshold(' + data[i].sensorId + ',' + data[i].itemId + ',' + factorId_temp + ')">清除</a></td></tr>');
                    }
                    $("#tbody-Threshold").html(sb.toString());
                    Table_To_datatable_Threshold();
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
        })
    }
    
}

function SaveThreshold(dom, sensorId, itemId,factorId) {
    //eq() 方法返回被选元素中带有指定索引号的元素(索引号从 0 开始)。
    var input1 = $(dom).parent().siblings().eq(2).find("input").val();
    var input2 = $(dom).parent().siblings().eq(3).find("input").val();
    var input3 = $(dom).parent().siblings().eq(4).find("input").val();
    var input4 = $(dom).parent().prev().find("input").val();//和$(dom).parent().siblings().eq(5).find("input").val()相同

    if (input1 == "") {
        alert("一级阈值不能为空！");
        $(dom).parent().siblings().eq(2).find("input").focus();
    }
    else if (input2 == "" && input3 != "") {
        alert("二级阈值不能为空！");
        $(dom).parent().siblings().eq(3).find("input").focus();
    }
    else if (input3 == "" && input4 != "") {
        if (input2 == "") {
            alert("二级阈值不能为空！");
            $(dom).parent().siblings().eq(3).find("input").focus();
        }
        else {
            alert("三级阈值不能为空！");
            $(dom).parent().siblings().eq(4).find("input").focus();
        }
    }
    else {
        if (isInputAllOverlap(input1, input2, input3, input4)) {
            var url = apiurl + '/sensor/threshold/config' + '?token=' + getCookie("token");
            $.ajax({
                url: url,
                type: 'post',
                data: {
                    "": [{
                        "sensorId": sensorId,
                        "itemId": itemId,
                        "threshold": [
                            {
                                "level": 1,
                                "value": encodeURIComponent(input1)
                            },
                            {
                                "level": 2,
                                "value": encodeURIComponent(input2)
                            },
                            {
                                "level": 3,
                                "value": encodeURIComponent(input3)
                            },
                            {
                                "level": 4,
                                "value": encodeURIComponent(input4)
                            }
                        ]
                    }]
                },
                statusCode: {
                    202: function () {
                        alert("保存成功!");
                        //保存成功后不刷新页面
                        //tableThresholdShow(factorId);
                    },
                    403: function () {
                        alert("权限验证出错");
                        logOut();
                    },
                    400: function () {
                        alert("添加失败,参数错误");
                    },
                    500: function () {
                        alert("内部异常");
                    },
                    404: function () {
                        alert('url错误');
                    },
                    405: function () {
                        alert('抱歉，没有配置阈值权限');
                    }
                }
            })
        }
        else {
            alert("输入区间有重叠，请检查后再提交！");
        }
    }   
}

function DeleteThreshold(sensorId, itemId, factorId) {
    var isDel = confirm("是否要清除该行阈值？");
    if (isDel) {
        var url = apiurl + '/sensor/threshold/config' + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'post',
            data: {
                "": [{
                    "sensorId": sensorId,
                    "itemId": itemId,
                    "threshold": [
                        {
                            "level": 1,
                            "value": encodeURIComponent("")
                        },
                        {
                            "level": 2,
                            "value": encodeURIComponent("")
                        },
                        {
                            "level": 3,
                            "value": encodeURIComponent("")
                        },
                        {
                            "level": 4,
                            "value": encodeURIComponent("")
                        }
                    ]
                }]
            },
            statusCode: {
                202: function () {
                    alert("清除成功!");
                    tableThresholdShow(factorId);
                },
                403: function () {
                    alert("权限验证出错");
                    logOut();
                },
                400: function () {
                    alert("添加失败,参数错误");
                },
                500: function () {
                    alert("内部异常");
                },
                404: function () {
                    alert('url错误');
                },
                405: function () {
                    alert('抱歉，没有清除阈值权限');
                }
            }
        })
    }
}

function SaveAllThreshold(dom,factorId) {
    //eq() 方法返回被选元素中带有指定索引号的元素(索引号从 0 开始)。
    var input1 = $(dom).parent().siblings().eq(2).find("input").val();
    var input2 = $(dom).parent().siblings().eq(3).find("input").val();
    var input3 = $(dom).parent().siblings().eq(4).find("input").val();
    var input4 = $(dom).parent().prev().find("input").val();//和$(dom).parent().siblings().eq(5).find("input").val()相同

    if (input1 == "") {
        alert("一级阈值不能为空！");
        $(dom).parent().siblings().eq(2).find("input").focus();
    }
    else if (input2 == "" && input3 != "") {
        alert("二级阈值不能为空！");
        $(dom).parent().siblings().eq(3).find("input").focus();
    }
    else if (input3 == "" && input4 != "") {
        if (input2 == "") {
            alert("二级阈值不能为空！");
            $(dom).parent().siblings().eq(3).find("input").focus();
        }
        else {
            alert("三级阈值不能为空！");
            $(dom).parent().siblings().eq(4).find("input").focus();
        }      
    }
    else {
        //Ajax表单提交时，利用js中的encodeURIComponent进行编码，防止一些特殊字符：+,&,空格
        if (isInputAllOverlap(input1, input2, input3, input4)) {
            var url = apiurl + '/factor/threshold/config' + '?token=' + getCookie("token");
            $.ajax({
                url: url,
                type: 'post',
                dataType: 'json',
                data: {
                    "structId": structId,
                    "factorId": factorId,
                    "threshold": [
                        {
                            "level": 1,
                            "value": encodeURIComponent(input1)
                        },
                        {
                            "level": 2,
                            "value": encodeURIComponent(input2)
                        },
                        {
                            "level": 3,
                            "value": encodeURIComponent(input3)
                        },
                        {
                            "level": 4,
                            "value": encodeURIComponent(input4)
                        }
                    ]
                },
                statusCode: {
                    202: function () {
                        alert("保存成功!");
                        tableThresholdShow(factorId);
                    },
                    403: function () {
                        alert("权限验证出错");
                        logOut();
                    },
                    400: function () {
                        alert("添加失败,参数错误");
                    },
                    500: function () {
                        alert("内部异常");
                    },
                    404: function () {
                        alert('url错误');
                    },
                    405: function () {
                        alert('抱歉，没有配置阈值权限');
                    }
                }
            })
        }
        else {
            alert("输入区间有重叠，请检查后再提交！");
        }
    }       
}

function DeleteAllThreshold(factorId) {
    var isDel = confirm("是否要清除当前监测因素下所有阈值？");
    if (isDel) {
        var url = apiurl + '/factor/threshold/config' + '?token=' + getCookie("token");
        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            data: {
                "structId": structId,
                "factorId": factorId,
                "threshold": [
                    {
                        "level": 1,
                        "value": encodeURIComponent("")
                    },
                    {
                        "level": 2,
                        "value": encodeURIComponent("")
                    },
                    {
                        "level": 3,
                        "value": encodeURIComponent("")
                    },
                    {
                        "level": 4,
                        "value": encodeURIComponent("")
                    }
                ]
            },
            statusCode: {
                202: function () {
                    alert("清除成功!");
                    tableThresholdShow(factorId);
                },
                403: function () {
                    alert("权限验证出错");
                    logOut();
                },
                400: function () {
                    alert("添加失败,参数错误");
                },
                500: function () {
                    alert("内部异常");
                },
                404: function () {
                    alert('url错误');
                },
                405: function () {
                    alert('抱歉，没有清除阈值权限');
                }
            }
        })
    }
}

function Table_To_datatable_Threshold() {
    //下面的"aoColumnDefs"一段是指第0,2,3,4,5,6列不排序
    $('#table-Threshold').dataTable({
        "aLengthMenu": [
            [10, 25, 50, -1],
            [10, 25, 50, "All"]
        ],
        // set the initial value
        "iDisplayLength": 10,
        "sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
        "sPaginationType": "bootstrap",
        "oLanguage": {
            "sUrl": "/resource/language/zn_CN.txt"
        },
        "bDestroy": true,
        "bRetrieve": true,
        "bStateSave": true,
        "aoColumnDefs": [{
            'bSortable': false,
            'aTargets': [1,2,3,4,5,6]
        }]
    }); 
}

//检查单个输入框
function check(dom) {
    var input = $(dom).val();
    if (!isSBCcase(input)) {
        alert("不能使用全角字符，请切换到英文输入法");
        $(dom).focus();
        return;
    }
    var arrayCheck = input.split(';');
    if (arrayCheck == "") { return; }
    for (var i = 0; i < arrayCheck.length; i++) {
        //防止类似(1,2,3,4)输入
        if (arrayCheck[i].split(',').length > 2) {
            var a = arrayCheck[i].split(',').length;
            alert("输入有误，请重新输入!");
            $(dom).focus();
            return;
        }
        //var a = arrayCheck[i].substr(arrayCheck[i].length-1,1);
        if (arrayCheck[i].split(',')[1] == undefined || arrayCheck[i].substr(arrayCheck[i].length - 1, 1)!=')') {
            alert("输入有误，请重新输入!");
            $(dom).focus();
            return;
        }
        var minDigit = arrayCheck[i].split(',')[0].split('(')[1];
        var maxDigit = arrayCheck[i].split(',')[1].split(')')[0];
        if (minDigit == '+' || minDigit == '-' || maxDigit == '+' || maxDigit == '-') {
            if (minDigit == '+' || maxDigit == '-') {
                alert("输入有误，请重新输入!");
                $(dom).focus();
                return;
            }
        }
        else {
            if (!(isDigit(minDigit) && isDigit(maxDigit))) {
                alert("输入有误，请重新输入!");
                $(dom).focus();
                return;
            }
            if (parseFloat(minDigit) >= parseFloat(maxDigit)) {
                alert("输入有误，请重新输入!");
                $(dom).focus();
                return;
            }
        }
    }

}

//判断字符串是否为实数
function isDigit(s) {
    if (s != null && s != "") {
        return !isNaN(s);
    }
    return false;
}

//判断两个区间是否重叠
//区间(a,b)和(c,d),若满足a<b且c<d，有：如果b>c且d>a，则这两个区间重叠
function isOverlap(a, b, c, d) {
    if (a == '-') { a = -9.9e10; }
    if (b == '+') { b = 9.9e10; }
    if (c == '-') { c = -9.9e10; }
    if (d == '+') { d = 9.9e10; }
    if (parseFloat(b) > parseFloat(c) && parseFloat(d) > parseFloat(a)) {
        return false;
    }
    else {
        return true;
    }
}

function isInputAllOverlap(input1, input2, input3, input4) {
    var array = new Array();
    if (input1 != "") {
        var temp=input1.split(';');
        for (var i = 0; i < temp.length; i++) {
            array.push(temp[i]);
        }
    }
    if (input2 != "") {
        var temp = input2.split(';');
        for (var i = 0; i < temp.length; i++) {
            array.push(temp[i]);
        }
    }
    if (input3 != "") {
        var temp = input3.split(';');
        for (var i = 0; i < temp.length; i++) {
            array.push(temp[i]);
        }
    }
    if (input4 != "") {
        var temp = input4.split(';');
        for (var i = 0; i < temp.length; i++) {
            array.push(temp[i]);
        }
    }
    for (var i = 0; i < array.length; i++) {
        var min1 = array[i].split(',')[0].split('(')[1];
        var max1 = array[i].split(',')[1].split(')')[0];
        for (var j = 0; j < array.length; j++) {
            if (j != i) {
                var min2 = array[j].split(',')[0].split('(')[1];
                var max2 = array[j].split(',')[1].split(')')[0];
                if (!isOverlap(min1, max1, min2, max2)) {
                    return false;
                }
            }
        }
    }
    return true;
 
}

//判断输入是否包含全角字符
function isSBCcase(str) {
    //[\uFE30-\uFFA0]全角字符   
    var pattern = /[\uFE30-\uFFA0]/gi;
    if (pattern.test(str)) {       
        return false;
    } else {
        return true;
    }
}

function thresholdConfigNext() {
    $('#tabThreshold').removeClass('active');
    $('#tab_threshold').removeClass('active');
    $('#tabValidate').addClass('active');
    $('#tab_Validate').addClass('active');
}