
var options_line = {
    data: {
        table: 'freq',
        startRow: 1,
        endRow: 17,
        endColumn: 7
    },

    chart: {
        polar: true,
        type: 'column',
        renderTo: 'content'
    },

    title: {
        text: ''
    },

    subtitle: {
        //text: '来源：江西飞尚科技有限公司'
    },
    credits: {
        href: '',
        text: ''
    },
    pane: {
        size: '85%'
    },

    legend: {
        reversed: true,
        align: 'right',
        verticalAlign: 'top',
        y: 100,
        layout: 'vertical'
    },

    xAxis: {
        tickmarkPlacement: 'on'
    },

    yAxis: {
        min: 0,
        endOnTick: false,
        showLastLabel: true,
        title: {
            text: ''
        },
        labels: {
            formatter: function () {
                return this.value + '%';
            }
        }
    },

    tooltip: {
        valueSuffix: '%',
        followPointer: true
    },

    plotOptions: {
        series: {
            stacking: 'normal',
            shadow: false,
            groupPadding: 0,
            pointPlacement: 'on'
        }
    }
};

$(function () {
    //$('.selectpicker').selectpicker();
    Highcharts.setOptions({
        lang: {
            printChart: "打印",
            downloadJPEG: "下载JPEG 图片",
            downloadPDF: "下载PDF文档",
            downloadPNG: "下载PNG 图片",
            downloadSVG: "下载SVG 矢量图",
            exportButtonTitle: "导出图片"
        }
    });

    getSensorList();

    $("#btnQuery").click(function () {
        loadChart();
    });

});

function GetWindRoses(sensorList) {
    var url = apiurl + '/wind/' + sensorList + '/stat-data/' + getStartdate() + '/' + getEnddate() + '?token=' + getCookie('token');

    $.ajax({
        url: url,
        type: 'get',

        success: function (data) {           
            var sb = null;
            sb = new StringBuffer();
            var dataValue = data.value;
       
            for (var i = 0; i < dataValue.length; i++) {
                var directCN = "";
                var directSort = "";
                var directEN = dataValue[i].direct;             
                //英文方向对应中文
                directCN = TodirectCN(directEN);
                //风频率数组排序
                var Value = [dataValue[i].percent1, dataValue[i].percent2, dataValue[i].percent3, dataValue[i].percent4, dataValue[i].percent5, dataValue[i].percent6, dataValue[i].percent7];
                var sortValue = windSort(Value);

                sb.append("<tr nowrap><td class='dir'>" + directCN + "</td>");
                sb.append("<td class='data'>" + sortValue[0] + "</td>");
                sb.append("<td class='data'>" + sortValue[1] + "</td>");
                sb.append("<td class='data'>" + sortValue[2] + "</td>");
                sb.append("<td class='data'>" + sortValue[3] + "</td>");
                sb.append("<td class='data'>" + sortValue[4] + "</td>");
                sb.append("<td class='data'>" + sortValue[5] + "</td>");
                sb.append("<td class='data'>" + sortValue[6] + "</td>");
                sb.append("<td class='data'>" + dataValue[i].totalPercent + "</td></tr>");
            }

            $("#winddata").html("");
            $("#winddata").append(sb.toString());
            var chart = new Highcharts.Chart(options_line);

            //tableManager('show_table', data_table_values,['风向', '风速(m/s)', '采集时间']);

        },
        error: function (XmlHttpRequest, textStatus, errorThrown) {
            alert(textStatus + "\nError: " + errorThrown);
        }
    })
}


function TodirectCN(directEN) {
    var directCN="";
    switch (directEN) {
        case "N": directCN = "北"; break;
        case "NNE": directCN = "北北东"; break;
        case "NE": directCN = "东北"; break;
        case "ENE": directCN = "东北东"; break;
        case "E": directCN = "东"; break;
        case "ESE": directCN = "东南东"; break;
        case "SE": directCN = "东南"; break;
        case "SSE": directCN = "南南东"; break;
        case "S": directCN = "南"; break;
        case "SSW": directCN = "南南西"; break;
        case "SW": directCN = "西南"; break;
        case "WSW": directCN = "西南西"; break;
        case "W": directCN = "西"; break;
        case "WNW": directCN = "西北西"; break;
        case "NW": directCN = "西北"; break;
        case "NNW": directCN = "北北西"; break;
    }
    return directCN;
}

function windSort(Value) {
    var temp = "";
    for (var i = 0; i < Value.length - 1; i++) {
        for (var j = i + 1; j < Value.length; j++) {
            if (Value[i] < Value[j]) {
                temp = Value[i];
                Value[i] = Value[j];
                Value[j] = temp;
            }
        }
    }
    return Value;
}

function loadChart() {
    var sensorList = [];
    var sensorId = $('#sensorList').find('option:selected');
    for (var i = 0; i < sensorId.length; i++) {
        sensorList[i] = sensorId[i].value;
    }
    if (sensorList.length == 0) {
    }
    else {
        GetWindRoses(sensorList);
    }

}

function getSensorList() {
    $('#sensorList').children().remove();
    var structid = getCookie("nowStructId");
    var factorId = document.getElementById('HiddenFactorNo').value;

    var url = apiurl + '/struct/' + structid + '/factor/' + factorId + '/sensors?token=' + getCookie('token');
    $.ajax({
        url: url,
        type: 'get',
         
        success: function (data) {
            if (data.length == 0) {
                $('.selectpicker').selectpicker('refresh');
                return;
            }
            var option = '';

            for (var i = 0; i < data.length; i++) {
                for (var index = 0; index < data[i].sensors.length; index++) {
                    option += '<option value=\'' + data[i].sensors[index].sensorId + '\'>' + data[i].sensors[index].location + '</option>';
                }
            }
            $('#sensorList').html(option);
            $('#sensorList').selectpicker();
            var defaultSensor = document.getElementById('HiddenSensorId').value;
            var defaultSensorID = "";
            if (defaultSensor == 0) {
                defaultSensorID = '';
            }
            else {
                defaultSensorID = defaultSensor;
            }         
            if (defaultSensorID == '') {
                defaultSensorID = data[0].sensors[0].sensorId;
                $('#sensorList').selectpicker('val', defaultSensorID);
            }
            else {
                $('#sensorList').selectpicker('val', parseInt(defaultSensorID));
            }
            loadChart();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status == 403) {
                alert("登录超时,请重新登录");
                logOut();
            } else {
                alertTips('传感器列表获取失败, 请尝试刷新页面', 'label-important', 'tip', 5000);
            }
        }
    });
}

//字符串缓冲
function StringBuffer() {
    this.data = [];
}

StringBuffer.prototype.append = function () {
    this.data.push(arguments[0]);
    return this;
}

StringBuffer.prototype.toString = function () {
    return this.data.join("");
}

