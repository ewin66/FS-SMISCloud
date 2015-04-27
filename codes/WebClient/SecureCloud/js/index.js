var structs;
var map;
var bound = [];
var slider;
var shigongMarkers = [];
var yunyingMarkers = [];
var markers = [];
var markersByType = [];
var selectedMarker;

// 响应分辨率
function init() {
    // 设置内容高度
    var heightTotal = $(window).height() - 30;
    var heightHeader = $('header').height();
    var heightFooter = $('footer').height();

    var heightContent = heightTotal - heightHeader - heightFooter;
    if (heightContent < 565) {
        heightContent = 565;
    }
    if (heightContent < 600) {
        $('#content .col').css('margin', 0);
    }

    $('#intro').height(heightContent);
    $('#loc').height(heightContent);
    $('#map').height(heightContent);
    $('#content').height(heightContent - 50);

    // 设置intro图片分布    
    var imgWidthTotal = $('.wrap').width() - 10;
    var imgWidth = imgWidthTotal / 4 - 10;
    var imgHeight = imgWidth / 16 * 9;
    var margin = (imgWidthTotal - imgWidth * 4) / 4;
    $('.structImg').height(imgHeight);
    $('.structImg').width(imgWidth);
    $('.imgList div').css('margin-right', margin);

    if (map != null) {
        map.centerAndZoom(new BMap.Point(105.381382, 35.00116), 5);
    }
}

function drawBoundary() {
    // 边界
    var provinces = ["广西-#C8C1E3", "广东-#FBC5DC", "湖南-#DBEDC7", "贵州-#E7CCAF", "云南-#DBEDC7",
        "福建-#FEFCBF", "江西-#E7CCAF", "浙江-#C8C1E3", "安徽-#FBC5DC", "湖北-#C8C1E3",
        "河南-#DBECC8", "江苏-#DBECC8", "四川-#FCFBBB", "海南省-#FCFBBB", "山东-#FCFBBB", "辽宁-#FCFBBB",
        "新疆-#FCFBBB", "西藏-#E7CCAF", "陕西-#E7CCAF", "河北-#E7CCAF", "黑龙江-#E7CCAF", "宁夏-#FBC5DC",
        "内蒙古自治区-#DBEDC7", "青海-#DBEDC7", "甘肃-#C8C1E3", "山西-#FBC5DC", "吉林省-#C8C1E3",
        "北京-#FBC5DC", "天津-#C8C1E3", "三河市-#E7CCAF", "上海-#FCFBBB", "重庆市-#FBC5DC", "香港-#C8C1E3"
    ];

    for (var i = 0; i < provinces.length; i++) {
        getBoundary(provinces[i]);
    }
}

function getBoundary(data) {
    var bdary = new BMap.Boundary();
    bdary.get(data.split("-")[0], function (rs) {
        var color = data.split("-")[1];

        var count = rs.boundaries.length;
        for (var i = 0; i < count; i++) {
            var ply = new BMap.Polygon(rs.boundaries[i], { strokeWeight: 1, strokeOpacity: 0.5, fillColor: color, strokeColor: "#000000" });
            bound.push(ply);
            map.addOverlay(ply);
        }
    });
}

function clearBoundary() {
    for (var i = 0; i < bound.length; i++) {
        map.removeOverlay(bound[i]);
    }
}

function onyunyingCheck() {
    if ($('#chYY').attr('checked') == 'checked') {
        for (var i = 0; i < yunyingMarkers.length; i++) {
            yunyingMarkers[i].show();
        }
    } else {
        for (var i = 0; i < yunyingMarkers.length; i++) {
            yunyingMarkers[i].hide();
        }
    }
}

function onshigongCheck() {
    if ($('#chSG').attr('checked') == 'checked') {
        for (var i = 0; i < shigongMarkers.length; i++) {
            shigongMarkers[i].show();
        }
    } else {
        for (var i = 0; i < shigongMarkers.length; i++) {
            shigongMarkers[i].hide();
        }
    }
}

function onstructTypeChange() {
    var items = '';
    var count = 0;
    var type = $('#structType').val();
    if (type == '所有类型') {
        count = markers.length;
        for (var i = 0; i < markers.length; i++) {
            items += '<p class="structItem" onclick="onstructItemClick(this)" onmouseover="onstructItemMouseover(this)" onmouseout="onstructItemMouseout(this)">' +
                markers[i][0] +
                '</p>';
            markers[i][1].show();
        }
    } else {
        for (var i = 0; i < markers.length; i++) {
            markers[i][1].hide();
        }
        count = markersByType[type].length;
        for (var i = 0; i < markersByType[type].length; i++) {
            items += '<p class="structItem" onclick="onstructItemClick(this)" onmouseover="onstructItemMouseover(this)" onmouseout="onstructItemMouseout(this)">' +
                markersByType[type][i][0] +
                '</p>';
            markersByType[type][i][1].show();
        }
    }
    $('#structList').html(items);
    $('#structCount').html(count);
    //map.centerAndZoom(new BMap.Point(105.381382, 35.00116), 5);
}

function onstructItemClick(sender) {
    clearBoundary();
    var structName = $(sender).html();
    if (selectedMarker != null) {
        selectedMarker.setAnimation(null);
    }
    for (var i = 0; i < markers.length; i++) {
        var m = markers[i];
        if (m[0] == structName) {
            m[1].setAnimation(BMAP_ANIMATION_BOUNCE);
            map.centerAndZoom(m[1].getPosition(), 13);
            var _iw = createInfoWindow(i);
            m[1].openInfoWindow(_iw);

            selectedMarker = m[1];
            break;
        }
    }
}

function onstructItemMouseover(sender) {
    $(sender).css('background', '#ccc');
}

function onstructItemMouseout(sender) {
    $(sender).css('background', '#f2f2f2');
}

function createInfoWindow(i) {
    var struct = structs[i];
    var iw = new BMap.InfoWindow("", { enableMessage: false });
    iw.setTitle("<label style=\"font-size: 16px;font-weight: bold;color: #CD533F;font-family: 'microsoft yahei';\">" + struct.structName + "</label>");
    //iw.setContent(sName);
    return iw;
}

function StructListControl() {
    // 设置默认停靠位置和偏移量  
    this.defaultAnchor = BMAP_ANCHOR_TOP_RIGHT;
    this.defaultOffset = new BMap.Size(0, 0);
}

function drawMap() {
    map = new BMap.Map("map");
    map.setMapStyle({ style: 'hardedge' });
    map.centerAndZoom(new BMap.Point(105.381382, 35.00116), 5);
    map.enableInertialDragging();

    map.clearOverlays();
    // 边界
    drawBoundary();

    // 标点        
    var url = apiurl + '/struct/intro?token=';
    $.ajax({
        url: url,
        type: 'get',
        cache: false,
        async: false,
        success: function (d) { structs = d; }
    });

    // 转换 marker
    var data = structs;
    for (var index = 0; index < data.length; index++) {
        var struct = data[index];
        var point = new BMap.Point(struct.longitude, struct.latitude);
        var myIcon;
        var marker;
        if (struct.projectStatus == "施工期") {
            myIcon = new BMap.Icon("../resource/img/googleMap/marker_orange.png", new BMap.Size(20, 34), { anchor: new BMap.Size(10, 34) });
            marker = new BMap.Marker(point, { icon: myIcon }); // 创建标注
            shigongMarkers.push(marker);
        } else {
            myIcon = new BMap.Icon("../resource/img/googleMap/marker_green.png", new BMap.Size(20, 34), { anchor: new BMap.Size(10, 34) });
            marker = new BMap.Marker(point, { icon: myIcon }); // 创建标注
            yunyingMarkers.push(marker);
        }
        markers.push([struct.structName, marker]);
        if (markersByType[struct.structType] == null) {
            markersByType.push(struct.structType);
            markersByType[struct.structType] = [];
            markersByType[struct.structType].push([struct.structName, marker]);
        } else {
            markersByType[struct.structType].push([struct.structName, marker]);
        }

        map.addOverlay(marker);

        (function() {
            var i = index;
            var _iw = createInfoWindow(i);
            var _marker = marker;
            _marker.addEventListener("click", function () {
                clearBoundary();
                if (selectedMarker != null) {
                    selectedMarker.setAnimation(null);
                }
                _marker.setAnimation(BMAP_ANIMATION_BOUNCE);
                map.centerAndZoom(_marker.getPosition(), 13);
                selectedMarker = _marker;

                this.openInfoWindow(_iw);                
            });
        })();
    }

    // 控件
    StructListControl.prototype = new BMap.Control();

    StructListControl.prototype.initialize = function(map) {
        // 类型option
        var options = '';
        for (var i = 0; i < markersByType.length; i++) {
            options += '<option value="' + markersByType[i] + '">' + markersByType[i] + '</option>';
        }
        // 结构物item
        var items = '';
        for (var i = 0; i < markers.length; i++) {
            items += '<p class="structItem" onclick="onstructItemClick(this)" onmouseover="onstructItemMouseover(this)" onmouseout="onstructItemMouseout(this)">' +
                markers[i][0] +
                '</p>';
        }

        var div = document.createElement("div");
        $(div).html('<div id="mapIcon" onselectstart="return false;">' +
                    '<div id="iconBg"></div>' +
                    '<div id="iconWrap">' +
                        '<div>' +
                            '现云平台共接入结构物数量:<strong>' + data.length.toString() + '</strong>个' +
                        '</div>' +
                        '<br />' +
                        '<div class="yyWrap">' +
                            //'<input name="state" type="checkbox" id="chYY" onclick="onyunyingCheck()" checked="checked"/>' +
                            //'<label for="chYY">运营期:<strong>' + yunyingMarkers.length.toString() + '</strong>个</label>' +
                            '<span class="colorFlag"></span>' +
                            '<span class="txt">运营期:<strong>' + yunyingMarkers.length.toString() + '</strong>个</span>' +
                        '</div>' +
                        '<div class="sgWrap">' +
                            //'<input type="checkbox" name="state" id="chSG" onclick="onshigongCheck()" checked="checked"/>' +
                            //'<label for="chSG">施工期:<strong>' + shigongMarkers.length.toString() + '个</strong></label>' +
                            '<span class="colorFlag"></span>' +
                            '<span class="txt">施工期:<strong>' + shigongMarkers.length.toString() + '个</strong></span>' +
                        '</div>' +
                        '<div class="control">' +
                            '<a href="javascript:void(0);" onclick="resetMap()">重置地图</a>' +
                            '<a href="javascript:void(0);" onclick="zoomMap(\'+\')">放大</a>' +
                            '<a href="javascript:void(0);" onclick="zoomMap(\'-\')">缩小</a>' +
                        '</div>' +
                        '<br />' +
                        '<div>' +
                            '<label for="structType"></label>' +
                            '<select id="structType" onchange="onstructTypeChange()" >' +
                                '<option value="所有类型">所有类型</option>' +
                                options +
                            '</select>' +
                            '<strong id="structCount">' + data.length.toString() + '</strong>个' +                            
                        '</div>' +                        
                        '<div id="structList">' +
                            items +
                        '</div>' +
                    '</div>' +
                '</div>');

        // 添加DOM元素到地图中   
        map.getContainer().appendChild(div);
        // 将DOM元素返回  
        return div;
    };

    var cr = new StructListControl();
    map.addControl(cr);    

    var opts = { anchor: BMAP_ANCHOR_BOTTOM_RIGHT, offset: new BMap.Size(5, 5) };
    map.addControl(new BMap.ScaleControl(opts));
    //opts = { type: BMAP_NAVIGATION_CONTROL_LARGE };
    //map.addControl(new BMap.NavigationControl(opts));
}

function zoomMap(forward) {
    var zoomNow = map.getZoom();
    if (forward == '+') {
        map.setZoom(zoomNow + 1);
    } else if (forward == '-') {
        map.setZoom(zoomNow - 1);
    }
}

function resetMap() {
    clearBoundary();
    drawBoundary();

    $('#structType').val('所有类型');
    $('#structType').change();

    map.centerAndZoom(new BMap.Point(105.381382, 35.00116), 5);
}

$(document).ready(function () {
    init();

    $(window).unbind().resize(function () { init(); });
    $(window)
        .bind('mousewheel', function (event, delta) {
            if (delta > 0) {
                slider.goToPrevSlide();
            } else {
                slider.goToNextSlide();
            }
            return false;
        });
    //$('#map')
    //    .bind('mousewheel', function (event, delta) {
    //        var zoom = map.getZoom();
    //        if (delta > 0) {
    //            map.setZoom(zoom + 1);
    //        } else {
    //            map.setZoom(zoom - 1);
    //        }
    //        return false;
    //    });
    $(window).keydown(function (event) {
        switch (event.which) {
            case 37:
            case 38:
                slider.goToPrevSlide();
                break;
            case 39:
            case 40:
                slider.goToNextSlide();
                break;
            default:
                break;
        }
    });


    $('li.parent').hover(function () {
        if ($(this).find('> ul').css('display') == "none") {
            $(this).find('> ul').slideDown(200);
            slide = true;
        }
    }, function () {
        if (slide == true) {
            $(this).find('> ul').slideUp();
            slide = false;
        }
    });

    drawMap();

    $('#structList').bind('mousewheel', function (event, delta) {
        var offset = $('#structList').scrollTop();
        if (delta > 0) {
            offset -= 20;
        } else {
            offset += 20;
        }
        $('#structList').scrollTop(offset);
        return false;
    });

    slider = $('.bxslider').bxSlider({
        pager: false,
        onSlideAfter: function ($slideElement, oldIndex, newIndex) {
            switch (newIndex) {
                case 0:
                    // 导航条样式
                    $('header .nav > ul > li > a').css('color', '#c3c3c3');
                    $('header .nav > ul > li > a').hover(
                        function () {
                            $(this).css('color', '#fff');
                        },
                        function () {
                            $(this).css('color', '#c3c3c3');
                        }
                    );
                    break;
                case 1:
                case 2:
                    // 导航条样式
                    $('header .nav > ul > li > a').css('color', '#000');
                    $('header .nav > ul > li > a').hover(
                        function () {
                            $(this).css('color', '#c3c3c3');
                        },
                        function () {
                            $(this).css('color', '#000');
                        }
                    );
                    break;
            }
        }
    });
});