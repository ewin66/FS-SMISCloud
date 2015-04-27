//坐标轴

function two_Axis(allData, allUnit) {
    var p1 = document.getElementById("factorList1").value;
    var valueNumber1 = new Array();
    valueNumber1 = p1.split("/");
    var p2 = document.getElementById("factorList2").value;
    var valueNumber2 = new Array();
    valueNumber2 = p2.split("/");
    var axisData = [];
    var columns = new Array();
    columns.push(valueNumber1[2], valueNumber2[2]);
    var unit = allUnit; //后期可能存在坐标和符号对不上
    //var a = unit.length;
    //var newUnit = new Array();
    //newUnit.push(unit[0],unit[a-1])
    if (columns.length == 2) {
        for (var i = 0; i < columns.length; i++) {
            var labelarry = [];
            var titlearry = [];
            var color = '';
            var axisValue = [];
            var a = '';
            if ((i + 1) % 2 == 0) {
                color = "#89A54E";
                a = "left";
            } else {
                color = "#4572A7";
                a = "right";
            }
            labelarry = {
                "align": a,
                "x": 3,
                "y": 16,
                //"formatter": function () { return Highcharts.numberFormat(this.value, 0); },
                "formatter": function() {
                    return this.value;
                },
                //"format":this.value+unit[i],
                "style": { "color": color },
            };
            titlearry = {
                "text": columns[i] + '(' + unit[i] + ')',
                "style": { "color": color }
            };
            //坐标轴
            if ((i + 1) % 2 == 0) {
                axisValue = {
                    //"min": -0.01,
                    //"max": 100,
                    "labels": labelarry,
                    "title": titlearry,
                    "opposite": true,
                    "showFirstLabel": false
                };
            } else {
                axisValue = {
                    // "min": -0.01,
                    //"max": 100,
                    "labels": labelarry,
                    "title": titlearry,
                    "showFirstLabel": false
                };
            }
            axisData.push(axisValue);
        }
        return axisData;
    } else {
    }
}

var type = '';

//拼接highchar中的双坐标轴series
function tow_series(data,unit) {
    var p1 = document.getElementById("factorList1").value;
    var valueNumber1 = new Array();
    valueNumber1= p1.split("/");
    var p2 = document.getElementById("factorList2").value;
    var valueNumber2 = new Array();
    valueNumber2 = p2.split("/");
    var factorId = new Array();
    factorId.push(valueNumber1[0], valueNumber2[0]);
    var seriesData = [];
    var columns = data;
    var unit = unit;
    var b1 = $("#sensorList1" + " option:selected").length;
    var b2 = $("#sensorList2" + " option:selected").length;
    var dataSeries = [];
    
    for (var i = 0; i < columns.length; i++) {        
        var array = new Array();
        var color = '';
        var axisValue = [];
        var yAxis = "";
//判断谁是显示的        
        
        if (i >= b1 ) {
            //color = "#89A54E";
            yAxis = 1;
        }
        else {
            //color = "#4572A7";
            yAxis = 0;
        }
         seriesValue = {
            "name": data[i].name,
            //"color": color,
            "type": data[i].type,
            "yAxis": yAxis,
            "data": data[i].data,
            "tooltip": { "valueSuffix": unit[i] }
        };
        seriesData.push(seriesValue);
    }
    return {
        dataSeries: seriesData
    }
}


//三个坐标
function three_Axis(allData, allUnit) {
    var p1 = document.getElementById("factorList1").value;
    var valueNumber1 = new Array();
    valueNumber1 = p1.split("/");
    var p2 = document.getElementById("factorList2").value;
    var valueNumber2 = new Array();
    valueNumber2 = p2.split("/");
    var a = allUnit.length;
    var unit = allUnit;
    var unit1 = [];
    var name = new Array();
    var newUnit = new Array();
    if (valueNumber1[0] == 5) {
        name.push(valueNumber2[2], "温度", "湿度");
        newUnit.push(unit[2], unit[0], unit[1]);
    } else if (valueNumber2[0] == 5) {
        name.push(valueNumber1[2], "温度", "湿度");
        newUnit.push(unit[0], unit[a - 2], unit[a - 1]);
    } else if (valueNumber1[0] == 18) {
        name.push(valueNumber2[2], "风速", "风向角");
        newUnit.push(unit[2], unit[0], unit[1]);
    } else if (valueNumber2[0] == 18) {
        name.push(valueNumber1[2], "风速", "风向角");
        newUnit.push(unit[0], unit[a - 2], unit[a - 1]);
    }
    var axisData = [];
    var columns = allData;
    for (var i = 0; i < 3; i++) {
        var labelarry = [];
        var titlearry = [];
        var color = '';
        var axisValue = [];
        var a = '';
        if ((i + 2) % 3 == 0) { //坐标轴2
            color = "#89A54E";
            a = "right";
        } else if ((i + 2) % 3 == 2) { //坐标轴1
            color = "#4572A7";
            a = "left";
        } else { //坐标轴3
            color = "#64324A";
            a = "right";
        }
        labelarry = {
            //会自动调节距离
            //"align": a,
            //"x": 3,
            //"y": 16,
            //"formatter": function () { return Highcharts.numberFormat(this.value, 0); },
            "formatter": function() {
                return this.value;
            },
            //"format":this.value+unit[i],
            "style": { "color": color },
        };
        titlearry = {
            "text": name[i] + '(' + newUnit[i] + ')',
            "style": { "color": color }
        };
        //坐标轴
        if ((i + 2) % 3 == 0) { //右边第一个
            axisValue = {
                //"min": 0,
                //"max": 100,
                "labels": labelarry,
                "title": titlearry,
                "opposite": true,
                "showFirstLabel": false
            };
        } else if ((i + 2) % 3 == 2) { //左边第一个
            axisValue = {
                //"min": 0,
                //"max": 100,
                "labels": labelarry,
                "title": titlearry,
                "showFirstLabel": false
            }
        } else {
            axisValue = {
                //右边第二个
                //"min": 0,
                //"max": 100,
                "labels": labelarry,
                "title": titlearry,
                "opposite": true,
                "showFirstLabel": false
            };
        };
        axisData.push(axisValue);
    }
    return axisData;
}


//拼接highchar中的三个坐标轴series

function three_series(data, unit) {
    var p1 = document.getElementById("factorList1").value;
    var valueNumber1 = new Array();
    valueNumber1 = p1.split("/");
    var p2 = document.getElementById("factorList2").value;
    var valueNumber2 = new Array();
    valueNumber2 = p2.split("/");
    var factorId = new Array();
    factorId.push(valueNumber1[0], valueNumber2[0]);
    //判断是前两个为两个坐标还是后两个
    var seriesData = [];
    var columns = data;
    var unit = unit;
    var dataSeries = [];
    for (var i = 0; i < columns.length; i++) {
        var array = new Array();
        var color = '';
        var axisValue = [];
        seriesValue = {
            "name": data[i].name,
            //"color": color,
            "type": "spline",
            "yAxis": data[i].yAxis,
            "data": data[i].data,
            "tooltip": { "valueSuffix": unit[i] }
        };
        seriesData.push(seriesValue);
    }
    return {
        dataSeries: seriesData
    }
}
