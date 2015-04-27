/**
 * ---------------------------------------------------------------------------------
 * <copyright file="chartTemplate.js" company="江苏飞尚安全监测咨询有限公司">
 * Copyright (C) 2015 飞尚科技
 * 版权所有。
 * </copyright>
 * ---------------------------------------------------------------------------------
 * <summary>
 * 文件功能描述：highcharts模板
 *
 * 创建标识：PengLing20150309
 *
 * 修改标识：
 * 修改描述：
 * </summary>
 * ---------------------------------------------------------------------------------
 */

// 柱状图
var stackedChartOptions = {
    chart: {
        type: 'column',
        inverted: true,
        marginBottom: 80,
        marginLeft: 140,
        //spacingLeft: 10,
        backgroundColor: '#f7f7f7',
        events: { }
    },
    title: {
        text: ''
    },
    xAxis: {
        labels: {
            //rotation: -30,
            //align: 'right'
        },
        categories: []
    },
    yAxis: {
        min: 0,
        title: {
            text: ''
        },
        stackLabels: {
            enabled: true,
            style: {
                fontWeight: 'bold',
                color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
            }
        }
    },
    legend: {
        //reversed: true,
        align: 'center',
        verticalAlign: 'bottom',
        x: 0,
        y: 10,
        floating: true,
        backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
        borderColor: '#CCC',
        borderWidth: 1,
        shadow: false
    },
    tooltip: {
        enabled: false
    },
    plotOptions: {
        series: {
            cursor: 'pointer',
            point: {
                events: {}
            }
        },
        column: {
            stacking: 'normal',
            dataLabels: {
                enabled: true,
                color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white',
                style: {
                    textShadow: '0 0 3px black'
                }
            }
        }
    },
    credits: {
        enabled: false
    },
    series: []
};

// 饼图
var pieChartOptions = {
    chart: {
        //backgroundColor: '#f7f7f7'
        events: { }
    },
    title: {
        text: ''
    },
    tooltip: {
        enabled: false
    },
    plotOptions: {
        pie: {
            allowPointSelect: true,
            cursor: 'pointer',
            dataLabels: {
                enabled: true,
                format: '<b>{point.name}</b>: {y} 个',
                style: {
                    color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                }
            },
            point: {
                events: {}
            }
        }
    },
    credits: {
        enabled: false
    },
    series: [{
        type: 'pie',
        data: []
    }]
};

// 面积图
var areaChartOptions = {
    chart: {
        type: 'area',
        backgroundColor: '#f7f7f7',
        events: {}
    },
    title: {
        text: ''
    },
    legend: {
        enabled: false
    },
    xAxis: {
        type: 'datetime',
        //categories: ['01:00', '02:00', '03:00', '04:00', '05:00', '06:00', '07:00', '08:00']
        dateTimeLabelFormats: {
            //second: '%H:%M:%S',
            minute: '%H:%M:%S',
            //hour: '%H:%M:%S',
            day: '%Y-%m-%d',
            month: '%b %y',
            //year: '%Y-%m-%d'
        }
    },
    yAxis: {
        title: {
            text: ''
        },
        labels: {
            formatter: function () { }
        }
    },
    tooltip: {
        formatter: function () { }
    },
    plotOptions: {
        area: {
            fillOpacity: 0.8,
            marker: {
                enabled: false
            }
        },
        series: {
            turboThreshold: 0 // Set it to 0 disable. Defaults to 1000.
        }
    },
    credits: {
        enabled: false
    },
    series: [{
        data: [],
        lineWidth: 0
    }]
};


//通用的柱状图展示
function CreateHighchartBar(renderTo, title1) {

    var template = {
        chart: {
            type: 'column',
            inverted: true,
            marginBottom: 80,
            marginLeft: 140,
            renderTo: renderTo,
            backgroundColor: '#f7f7f7'
        },
        title: {
            text: title1
        },
        xAxis: {
            categories: [],
            labels: {
                rotation: -0,
                style: {
                    fontSize: '13px',
                    fontFamily: 'Verdana, sans-serif'
                }
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: ''
            }
        },
        legend: {
            enabled: false
        },
        tooltip: {
            pointFormat: '该类传感器的个数为: <b>{point.y} 个</b>',
        },
        plotOptions: {
            series: {
                cursor: 'pointer',
                point: {
                    events: {}
                }
            }
        },
        credits: {
            enabled: false
        },
        series: []
    };
    return template;
}
