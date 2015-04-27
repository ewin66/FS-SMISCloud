function errorTip(stringId) { //没有数据给出提示
    var graphId = stringId.split(',');
    var errorTipstring = '<div id=\'error\' class=\'row-fluid dataerror-tip\'>' +
        '<div class="span3">' +
        '<span class=\'label label-important\' style=\'margin-left: 5px;margin-top: 10px;\'>抱歉，没有查询到任何有效的数据</span>' +
        '</div>' +
        '</div>';
    for (var i = 0; i < graphId.length; i++) {
        $('#' + graphId[i]).append(errorTipstring);
    }
}

function display(comm1) { //隐藏和展示跳转
    if (comm1 == "block") {
        $('#comm1_error').show();
        $('#comm1').hide();
        $('#comm2_error').hide();
    } else if (comm1 == "none") {
        $('#comm1_error').hide();
        $('#comm1').show();
        $('#comm2_error').hide();
    } else {
        $('#comm1_error').hide();
        $('#comm1').hide();
        $('#comm2_error').show();
    }
}

var comm_chart = "";
var structId = "";
$(function() {
    var id = "comm1_error";
    errorTip(id);
    $('#data-contact').addClass('active');
    $('#dataContact3').addClass('active');
    var nowstructId;
    if (location.href.split('=')[1] == null || location.href.split('=')[1] == undefined) {
        nowstructId = location.href.split('=')[1];
    } else {
        nowstructId = location.href.split('=')[1].replace(/#/, "");
    }
    if (nowstructId != null && nowstructId != undefined && nowstructId != "") {
        setCookie('nowStructId', nowstructId);
    }
    //初始化加载
    showStructList(getCookie("nowStructId"));
    getFactorGroup(getCookie("nowStructId"));

    //时间控件
    $('#dpform1').datetimepicker({
        format: 'yyyy-MM-dd hh:mm:ss',
        language: 'pt-BR'
    });
    $('#dpdend1').datetimepicker({
        format: 'yyyy-MM-dd hh:mm:ss',
        language: 'pt-BR'
    });

    //点击查询按钮
    $('#btnQuery').click(function() {
        loadChart();
        //$(".box-content").hide();
       // $('#expand_collapse').attr('src', '../resource/img/toggle-expand.png');
    });

    $('#factorList1').change(function() { //根据监测因素变换测点
        allData = [];
        var p1 = document.getElementById("factorList1").value;
        var valueNumber = new Array();
        valueNumber = p1.split("/");
        getCorrelationFactor(getCookie("nowStructId"), valueNumber[0]);
        getSensorList_correlation1(valueNumber[0], valueNumber[1]);
    });
    $('#factorList2').change(function() { //根据监测因素变换测点        
        allData = [];
        var p2 = document.getElementById("factorList2").value;
        var valueNumber2 = new Array();
        valueNumber2 = p2.split("/");
        getSensorList_correlation2(valueNumber2[0], valueNumber2[1]);
    });
});

//图标操作
$('a.box-collapse').click(function () {
    var $target = $(this).parent().parent().next('.box-content');
    if ($target.is(':visible')) {
        $('img', this).attr('src', '../resource/img/toggle-expand.png');
    } else {
        $('img', this).attr('src', '../resource/img/toggle-collapse.png');
    }
    $target.slideToggle();
});

var sensor1Select;
var sensor2Select;
function loadChart() {
    allData = [];
    allUnit = [];
    var b = $("#sensorList1" + " option:selected").length;
    var b3 = $("#sensorList2" + " option:selected").length;
    if (parseInt(chose_get_text('#factorList2')) == 0) {
        $('#factor-correlation').hide();
        $('#selectTime').hide();
        $('#btnQuery').hide();
        var error = document.getElementById("comm2_error");
        error.innerHTML = '<div id=\'error\' class=\'row-fluid dataerror-tip\'>' +
            '<div class="span3">' +
            '<span class=\'label label-important\' style=\'margin-left: 5px;margin-top: 10px;\'>抱歉，该监测因素没有关联的监测因素</span>' +
            '</div>' +
            '</div>';
        display('other');
        return;
    } else if (b == 0 || b3 == 0) {
        var error1 = document.getElementById("comm2_error");
        error1.innerHTML = '<div id=\'error\' class=\'row-fluid dataerror-tip\'>' +
            '<div class="span3">' +
            '<span class=\'label label-important\' style=\'margin-left: 5px;margin-top: 10px;\'>请同时选择关联监测因素的监测点！</span>' +
            '</div>' +
            '</div>';
        display('other');
        return;
        
    } else {
        getDateList();
    }
}

function showdate(n) {
    var uom = new Date();
    uom.setDate(uom.getDate() + n);
    uom = uom.getFullYear() + "-" + (uom.getMonth() + 1) + "-" + uom.getDate() + " " + uom.getHours() + ":" + uom.getMinutes() + ":" + uom.getSeconds();
    return uom.replace(/\b(\w)\b/g, '0$1'); //时间的格式
}

//默认最新时间
$("#dpform").val(showdate(-1));
$("#dpdend").val(showdate(0));

//结构物
function showStructList(structId) {
    var userId = getCookie('userId');
    if (userId === '' || userId === null) {
        alert('获取用户Id失败，请检查浏览器Cookie是否已启用');
        return;
    }
    var url = apiurl + '/user/' + userId + '/structs' + '?token=' + getCookie("token");
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                return;
            }
            var sb = new StringBuffer();
            var flag = true;
            for (var i = 0; i < data.length; i++) {
                if (data[i].structId == parseInt(structId)) { //选择那个哪个为第一个
                    $('.breadcrumb li small a').html(data[i].structName + '<i class="icon-angle-down"></i>');

                    setCookie('nowStructName', data[i].structName, null);
                    if (i == 0) {
                        flag = false;
                    }
                } else {
                    if (i > 0 && flag) {
                        sb.append('<li class="divider"></li>');
                    }
                    flag = true;
                    sb.append('<li><a href="/dataContact3.aspx?id=' + data[i].structId + '">' + data[i].structName + '</a></li>');
                }
            }
            $('.breadcrumb li small ul').html(sb.toString());
            $('.breadcrumb li small ul').selectpicker('refresh');
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取结构物列表失败.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

//获得监测因素
function getFactorGroup(structId) {
    $('#factorList1').children().remove();
    var url = apiurl + '/struct/' + structId + '/factors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                $('#factorList1').selectpicker('refresh');
                $('#factorList1').trigger("liszt:updated");
                return;
            }
            var monitorFactor1 = '';
            for (var i = 0; i < data.length; i++) {
                var factor = data[i].children;
                for (var j = 0; j < factor.length; j++) {
                    //if (factor[j].factorId != 51) {//于家堡问题
                    //    monitorFactor1 += '<option  value="' + factor[j].factorId + '/' + factor[j].valueNumber + '/' + factor[j].factorName + '">' + factor[j].factorName + '</option>';

                    //}
                    
                    if (factor[j].factorId == 51 || factor[j].factorId == 54) {
                        monitorFactor1 += "";
                    } else {
                        monitorFactor1 += '<option  value="' + factor[j].factorId + '/' + factor[j].valueNumber + '/' + factor[j].factorName + '">' + factor[j].factorName + '</option>';
                    }
                }
            }
            $('#factorList1').html(monitorFactor1);
            $('#factorList1').selectpicker('refresh');
            var p1 = document.getElementById("factorList1").value;
            var valueNumber = new Array();
            valueNumber = p1.split("/");
            getCorrelationFactor(getCookie("nowStructId"), valueNumber[0]);
            getSensorList_correlation1(valueNumber[0], valueNumber[1]);
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取监测因子列表失败.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });


}
//获得关联监测因素
function getCorrelationFactor(structId, factorId) {
    $('#factorList2').children().remove();
    var url = apiurl + '/struct/' + structId + '/factor/' + factorId + '/correlation?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                var a = document.getElementById("sensorList2");
                jsRemoveSelectedItemFromSelect(a);
                $('#sensorList2').trigger("liszt:updated");
                $('.selectpicker').selectpicker('refresh'); //没有的时候直接清空
                $('#factor-correlation').hide();
                $('#selectTime').hide();
                $('#btnQuery').hide();
                var error = document.getElementById("comm2_error");
                error.innerHTML = '<div id=\'error\' class=\'row-fluid dataerror-tip\'>' +
                    '<div class="span3">' +
                    '<span class=\'label label-important\' style=\'margin-left: 5px;margin-top: 10px;\'>抱歉，该监测因素没有关联的监测因素</span>' +
                    '</div>' +
                    '</div>';
                display('other');
                return;
            }
            $('#factor-correlation').show();
            var monitorFactor2 = '';
            for (var i = 0; i < data.length; i++) {
                monitorFactor2 += '<option  value="' + data[i].factorid + '/' + data[i].valuenumber + '/' + data[i].factorname + '">' + data[i].factorname + '</option>';
            }
            $('#factorList2').html(monitorFactor2);
            $('#factorList2').selectpicker('refresh');
            var p2 = document.getElementById("factorList2").value;
            var valueNumber = new Array();
            valueNumber = p2.split("/");
            getSensorList_correlation2(valueNumber[0], valueNumber[1]);
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取监测因子列表失败.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

//填充监测点参数要对应

function getOptions(factorId, valuenumber, data) {
    var option = "";
    for (var i = 0; i < data.length; i++) {
        for (var index = 0; index < data[i].sensors.length; index++) {
            if (valuenumber == 1 || factorId == 5 || factorId == 18) {
                option += '<option value="' + data[i].sensors[index].sensorId + '">' + data[i].sensors[index].location + '</option>';
            } else if (valuenumber == 2 || valuenumber == 4) {
                option += '<option value="' + data[i].sensors[index].sensorId + '">' + data[i].sensors[index].location + '-x</option>';
                option += '<option value="' + data[i].sensors[index].sensorId + '/1' + '">' + data[i].sensors[index].location + '-y</option>';
            } else {
                if (factorId == 30) {
                    option += '<option value="' + data[i].sensors[index].sensorId + '">' + data[i].sensors[index].location + '-风速</option>';
                    option += '<option value="' + data[i].sensors[index].sensorId + '/1' + '">' + data[i].sensors[index].location + '-风向</option>';
                    option += '<option value="' + data[i].sensors[index].sensorId + '/2' + '">' + data[i].sensors[index].location + '-风仰角</option>';
                } else {
                    option += '<option value="' + data[i].sensors[index].sensorId + '">' + data[i].sensors[index].location + '-x</option>';
                    option += '<option value="' + data[i].sensors[index].sensorId + '/1' + '">' + data[i].sensors[index].location + '-y</option>';
                    option += '<option value="' + data[i].sensors[index].sensorId + '/2' + '">' + data[i].sensors[index].location + '-z</option>';
                }
            }
        }
    }
    return option;
}


//监测点
function getSensorList_correlation1(factorId, valuenumber) {
    $('#sensorList1').children().remove();
    var structId = getCookie("nowStructId");
    var url = apiurl + '/struct/' + structId + '/factor/' + factorId + '/sensors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                $('#sensorList1').trigger("liszt:updated");
                $('.selectpicker').selectpicker('refresh');
                return;
            }
            var option = getOptions(factorId, valuenumber, data);
            $('#sensorList1').html(option);
            $('#sensorList1').val(data[0].sensors[0].sensorId); //可以模糊选择,第一默认     
            $('#sensorList1').trigger("liszt:updated");
            $('#sensorList1').chosen({
                no_results_text: "没有找到",
                allow_single_de: true
            });
            loadChart();
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取监测因子列表失败.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

//关联监测点
function getSensorList_correlation2(factorId, valuenumber) {
    $('#sensorList2').children().remove();
    var structId = getCookie("nowStructId");
    var url = apiurl + '/struct/' + structId + '/factor/' + factorId + '/sensors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
        dataType: 'json',
        cache: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                $('#sensorList2').trigger("liszt:updated");
                $('.selectpicker').selectpicker('refresh');
                return;
            }
            $('#selectTime').show();
            $('#btnQuery').show();
            var option = getOptions(factorId, valuenumber, data);
            $('#sensorList2').html(option);
            $('#sensorList2').val(data[0].sensors[0].sensorId); //可以模糊选择,第一默认 
            $('#sensorList2').trigger("liszt:updated");
            $('#sensorList2').chosen({
                // max_selected_options: 5,
                no_results_text: "没有找到",
                allow_single_de: true
            });
           
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("获取监测因子列表失败.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
}

function getUrl(a, start, end) {
    var interval = getInteval();
    var url;
    if (interval <= 6) {
        url = apiurl + "/sensor/" + a + "/data/" + start + '/' + end + '?token=' + getCookie('token');
    } else if (interval <= 30) {
        url = apiurl + "/sensor/" + a + "/data/" + start + '/' + end + '/' + '1/minute?token=' + getCookie('token');
    } else if (interval <= 90) {
        url = apiurl + "/sensor/" + a + "/data/" + start + '/' + end + '/' + '1/hour?token=' + getCookie('token');
    } else if (interval <= 365) {
        url = apiurl + "/sensor/" + a + "/data/" + start + '/' + end + '/' + '12/hour?token=' + getCookie('token');
    } else {
        url = apiurl + "/sensor/" + a + "/data/" + start + '/' + end + '/' + '1/day?token=' + getCookie('token');
    }
    return url;
}

var allData = new Array();
var allUnit = new Array();
//获取监测数据
function getDateList() {
    var a = getText("#sensorList1");
    var a1 = chose_get_value("#sensorList1"); //按数组循环               
    var b1 = get_direction("#sensorList1");
    var p = document.getElementById("factorList1").value;
    var valueNumber1 = new Array();
    valueNumber1 = p.split("/");

    var url = getUrl(a1, getTime("#dpform"), getTime("#dpdend"));
    $.ajax({
        url: url,
        type: 'get',
        async: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                display('block');
                //有待考虑
                if (valueNumber1[0] == 5 || valueNumber1[0] == 18) {
                    allUnit.push([], []);
                } else {
                    allUnit.push([]);
                }
                return;
            } else {
                display('none');
                for (var i = 0; i < data.length; i++) {
                    var unit1 = data[0].unit[0];
                    var unit = data[0].unit[1];
                    var array = new Array();
                    var array1 = new Array();
                    for (var j = 0; j < data[i].data.length; j++) {
                        var time = data[i].data[j].acquisitiontime.substring(6, 19); //时间                           

                        if (valueNumber1[0] == 5 || valueNumber1[0] == 18) {
                            if (data[i].data[j].value[0] != null) {
                                array.push([parseInt(time), data[i].data[j].value[0]]);
                            }
                            if (data[i].data[j].value[1] != null) {
                                array1.push([parseInt(time), data[i].data[j].value[1]]);
                            }
                        } else if (b1[i] == 0 && data[i].data[j].value[0] != null) {
                            //单方向和x轴方向
                            array.push([parseInt(time), data[i].data[j].value[0]]); //需要变动                                      
                        } else if (b1[i] == 1 && data[i].data[j].value[1] != null) {
                            array.push([parseInt(time), data[i].data[j].value[1]]);;
                        } else if (b1[i] == 2 && data[i].data[j].value[2] != null) {
                            array.push([parseInt(time), data[i].data[j].value[2]]);;
                        }
                    }

                    //监测因素数据
                    if (array1.length != 0) {
                        if (valueNumber1[0] == 5) {
                            allData.push({ name: "温度", data: array, yAxis: 1 }, { name: "湿度", data: array1, yAxis: 2 });
                        } else if (valueNumber1[0] == 18) {
                            allData.push({ name: "风速", data: array, yAxis: 1 }, { name: "风向角", data: array1, yAxis: 2 });
                        }
                        allUnit.push(unit1, unit);
                    } else {

                        if (b1[i] == 0) { //单方向和x轴方向
                            if (valueNumber1[1] == 1) {
                                if (valueNumber1[0] == 6) {
                                    allData.push({ name: data[i].location, data: array, type: "column" });
                                    var mun = 0;
                                    for (var n = 0; n < array.length; n++) {
                                        if (array[n][1] == 0) {
                                            mun += 0;
                                        } else {
                                            mun += 1;
                                        }
                                    }
                                    if (mun == 0) {
                                        alert("如果图上出来了" + a + "图例，而没有图形则该时间段数据为0!");
                                    }
                                } else {
                                    allData.push({ name: data[i].location, data: array, type: "spline" });
                                }
                            } else {
                                allData.push({ name: data[i].location + '-x', data: array, type: "spline" });
                            }
                        } else if (b1[i] == 1) {
                            allData.push({ name: data[i].location + '-y', data: array, type: "spline" });
                        } else if (b1[i] == 2) {
                            allData.push({ name: data[i].location + '-z', data: array, type: "spline" });
                        }

                        allUnit.push(unit1);
                    }
                }
            }
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("权限验证出错，禁止访问");
                logOut();
            } else if (xhr.status !== 0) { // aborted requests should be just ignored and no error message be displayed
                alert("数据获取失败, 请尝试重新查询.\r\n" + xhr.status + " : " + xhr.statusText);
            }
        }
    });
    var a3 = getText("#sensorList2");
    var a2 = chose_get_value("#sensorList2"); //按数组循环               
    var b2 = get_direction("#sensorList2");
    var p2 = document.getElementById("factorList2").value;
    var valueNumber2 = new Array();
    valueNumber2 = p2.split("/");
    var url2 = getUrl(a2, getTime("#dpform"), getTime("#dpdend"));

    $.ajax({
        url: url2,
        type: 'get',
        async: false,
        success: function(data) {
            if (data == null || data.length == 0) {
                //有待考虑
                if (valueNumber2[0] == 5 || valueNumber2[0] == 18) {
                    allUnit.push([], []);
                } else {
                    allUnit.push([]);
                }
                return;
            } else {
                display('none');
                for (var i = 0; i < data.length; i++) {
                    var unit2 = data[0].unit[0];
                    var unit3 = data[0].unit[1];
                    var array2 = new Array();
                    var array3 = new Array();
                    for (var j = 0; j < data[i].data.length; j++) {
                        var time = data[i].data[j].acquisitiontime.substring(6, 19); //时间                           
                        if (valueNumber2[0] == 5 || valueNumber2[0] == 18) {
                            if (data[i].data[j].value[0] != null) {
                                array2.push([parseInt(time), data[i].data[j].value[0]]);
                            }
                            if (data[i].data[j].value[2] != null) {
                                array3.push([parseInt(time), data[i].data[j].value[1]]);
                            }
                        } else if (b2[i] == 0 && data[i].data[j].value[0] != null) {
                            //单方向和x轴方向
                            array2.push([parseInt(time), data[i].data[j].value[0]]); //需要变动                                      
                        } else if (b2[i] == 1 && data[i].data[j].value[1] != null) {
                            array2.push([parseInt(time), data[i].data[j].value[1]]);
                        } else if (b2[i] == 2 && data[i].data[j].value[2] != null) {
                            array2.push([parseInt(time), data[i].data[j].value[2]]);
                        }
                    }
                    //关联监测因素的数据
                    if (array3.length != 0) {
                        if (valueNumber2[0] == 5) {
                            allData.push({ name: "温度", data: array2, yAxis: 1 }, { name: "湿度", data: array3, yAxis: 2 });
                        } else if (valueNumber2[0] == 18) {
                            allData.push({ name: "风速", data: array2, yAxis: 1 }, { name: "风向角", data: array3, yAxis: 2 });
                        }
                        allUnit.push(unit2, unit3);
                    } else {
                        if (b2[i] == 0) { //单方向和x轴方向
                            if (valueNumber2[1] == 1) {
                                if (valueNumber2[0] == 6) {
                                    allData.push({ name: data[i].location, data: array2, type: "column" });
                                    var mun1 = 0;
                                    for (var m = 0; m < array2.length; m++) {
                                        if (array2[m][1] == 0) {
                                            mun1 += 0;
                                        } else {
                                            mun1 += 1;
                                        }
                                    }
                                    if (mun1 == 0) {
                                        alert("如果图上出来了" + a3 + "图例，而没有图形则该时间段数据为0!");
                                    }
                                } else {
                                    allData.push({ name: data[i].location, data: array2, type: "spline" });
                                }
                            } else {
                                allData.push({ name: data[i].location + '-x', data: array2, type: "spline" });
                            }
                        } else if (b2[i] == 1) {
                            allData.push({ name: data[i].location + '-y', data: array2, type: "spline" });
                        } else if (b2[i] == 2) {
                            allData.push({ name: data[i].location + '-z', data: array2, type: "spline" });
                        }
                        allUnit.push(unit2);
                    }
                }
            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            }
        }
    });
    
    var allLength = $("#sensorList2" + " option:selected").length + $("#sensorList" + " option:selected").length;
    if (0 < allData.length && allData.length < allLength) {
        alert("如果图上某些传感器的图例没有显示，说明该传感器则该时间段没有数据!");
    }
    var seriesData;
    if (valueNumber1[0] == 5 || valueNumber1[0] == 18 || valueNumber2[0] == 5 || valueNumber2[0] == 18) {
        var threeAxis = three_Axis(allData, allUnit);
        seriesData = three_series(allData, allUnit);
        comm_chart = createHighchartComm2('comm1', '关联因素对比图', threeAxis, seriesData.dataSeries);
    } else {
        var doubleAxis = two_Axis(allData, allUnit);
        seriesData = tow_series(allData, allUnit);
        comm_chart = createHighchartComm1('comm1', '关联因素对比图', doubleAxis, seriesData.dataSeries);
    }
    comm_chart.tooltip.formatter = function() {
        var tooltipString = '<b>' + this.series.name + '</b>';
        tooltipString = tooltipString + '<br/><br/>采集时间:' + Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
        tooltipString = tooltipString + '<br/>' + '监测数据:' + this.y.toString() + '<b>' + allUnit[this.series.index] + '</b>';
        return tooltipString;
    }
    var chart = new Highcharts.Chart(comm_chart);
}


//清除所选项
function jsRemoveSelectedItemFromSelect(objSelect) {
    var length = objSelect.options.length - 1;
    for (var i = length; i >= 0; i--) {
        if (objSelect[i].value != '') {
            objSelect.options[i] = null;
        }
    }
}

function chose_get_text(select) {
    return $(select + " option:selected").length;
}
//获取多选列表
function chose_get_value(select) {
    var b = $(select + " option:selected").length;
    var cId = new Array();//获得设备Id    
    var p = new Array();//设备的Id及数据方向
    var i = 0;
    while (i < b) {
        var a = $(select + " option:selected")[i].value;
        p = a.split("/");
        cId.push(p[0]);
        i++;
    }
    return cId;
}

function get_direction(select) {
    var b = $(select + " option:selected").length;
    var dz = new Array(); //数据的方向
    var p = new Array(); //设备的Id及数据方向
    var idFz = new Array();
    var i = 0;
    while (i < b) {
        var a = $(select + " option:selected")[i].value;
        p = a.split("/");
        if (!p[1]) {
            p[1] = 0;
        }
        dz.push(p[1]);
        i++;
    }
    return dz;
}

//获得选中的option内容
function getText(select) {
    var b = $(select + " option:selected").length;
    var text = new Array();
    var i = 0;
    while (i < b) {
        var a = $(select + " option:selected")[i].text;
        text.push(a);
        i++;
    }
    return text;
}


//获取时间
function getTime(d1) {
    var b = 1;
    var a = new Array();
    var i = 0;
    while (i < b) {
        var c = $(d1)[i].value;
        //alert(c);
        a.push(c);
        i++;
    }
    return a;
}

//获得时间步长
function getInteval() {
    var divNum = 1000 * 3600 * 24;
    var startTimeArray = $("#dpform");
    var endTimeArray = $("#dpdend");
    var startTime = startTimeArray[0].value;
    var endTime = endTimeArray[0].value;
    startTime = startTime.replace(/-/g, "/");
    endTime = endTime.replace(/-/g, "/");
    var sTime = new Date(startTime);
    var eTime = new Date(endTime);
    var a = parseInt((eTime.getTime() - sTime.getTime()) / parseInt(divNum));
    return a;
}

