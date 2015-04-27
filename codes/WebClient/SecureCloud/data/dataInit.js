//function dateInput() {
//    $("#dpform1").datetimepicker({        
//        showSecond: true,
//        timeFormat: 'hh:mm:ss',
//        stepHour: 1,
//        stepMinute: 1,
//        stepSecond: 1
//    })
//    $("#dpdend1").datetimepicker({       
//        showSecond: true,
//        timeFormat: 'hh:mm:ss',
//        stepHour: 1,
//        stepMinute: 1,
//        stepSecond: 1
//    })
//    $(".ui_timepicker").datetimepicker({       
//        showSecond: true,
//        timeFormat: 'hh:mm:ss',
//        stepHour: 1,
//        stepMinute: 1,
//        stepSecond: 1
//    })
//    $(".ui_time").datetimepicker({
//        //showOn: "button",
//        //buttonImage: "./css/images/icon_calendar.gif",
//        //buttonImageOnly: true,
//        showSecond: true,
//        timeFormat: 'hh:mm:ss',
//        stepHour: 1,
//        stepMinute: 1,
//        stepSecond: 1
//    })

//}
//时分秒都可以选
$('#dpform1').datetimepicker({
    format: 'yyyy-MM-dd hh:mm:ss',
    language: 'pt-BR',
   
});
$('#dpdend1').datetimepicker({
    format: 'yyyy-MM-dd hh:mm:ss',
    language: 'pt-BR',
    
});
$('#dpform2').datetimepicker({
    format: 'yyyy-MM-dd hh:mm:ss',
    language: 'pt-BR',
    
});
$('#dpdend2').datetimepicker({
    format: 'yyyy-MM-dd hh:mm:ss',
    language: 'pt-BR',
    
});

$('#struct-List').change(function () {//根据监测因素变换测点        
    flag = 2;
});
$('#factorList').change(function () {//根据监测因素变换测点
    flag = 2;

});


var flag = 2;
function addTimeTable() {

    flag++;
    if (flag >5) {
        alert("最多只可以选择5个对比时段！");
        return;
    }
    $('#timeTable img').remove();
    var sb = '';
    sb = '<div class="control-group"><table class="mySelfCss"><tr><td><b>对比时段' + flag
       + ':</b></td><td><div class="controls" style="margin-left:0;"><div class="input-append date"><input type="text"  class="ui_timepicker"/>' +
        '<span class="add-on" style="height:20px;"><i data-time-icon="icon-time" data-date-icon="icon-calendar"></i></span></div></td>' +
        '<td><b>至</b></td>' +
        '<td><div class="input-append date"><input type="text"  class="ui_time"/><span class="add-on" style="height:20px;"><i data-time-icon="icon-time" data-date-icon="icon-calendar"></i></span></div></td>';


    sb += '<td><img id="expand_expand" alt="" src="/resource/img/toggle-expand.png" style="width: 40px; height: 40px;" onclick="addTimeTable()" /></td>';
    sb += '<td><img id="expand_collapse" alt="" src="/resource/img/toggle-collapse.png" style="width: 40px; height: 40px;" onclick="deleteTimeTable()" /></div></div></td></tr></table>';
    $('#timeTable').append(sb);


    $('.input-append ').datetimepicker({
        format: 'yyyy-MM-dd hh:mm:ss',
        language: 'pt-BR',
       
    });


}

function deleteTimeTable() {
    flag = 2;
    var sb = '';
    sb ='<div class="control-group"><table class="mySelfCss"><tr><td><b>对比时段1' 
       + ':</b></td><td><div class="controls" style="margin-left:0;"><div class="input-append date"><input type="text" id="dpform1" class="ui_timepicker"/>' +
        '<span class="add-on" style="height:20px;"><i data-time-icon="icon-time" data-date-icon="icon-calendar"></i></span></div></td>' +
        '<td><b>至</b></td>' +
        '<td><div class="input-append date"><input type="text" id="dpdend1" class="ui_time"/><span class="add-on" style="height:20px;"><i data-time-icon="icon-time" data-date-icon="icon-calendar"></i></span></td></tr></table></div>';

    sb += '<div class="control-group"><table class="mySelfCss"><tr><td><b>对比时段2'
       + ':</b></td><td><div class="controls" style="margin-left:0;"><div class="input-append date"><input type="text" id="dpform2" class="ui_timepicker" />' +
        '<span class="add-on" style="height:20px;"><i data-time-icon="icon-time" data-date-icon="icon-calendar"></i></span></div></td>' +
        '<td><b>至</b></td>' +
        '<td><div class="input-append date"><input type="text" id="dpdend2" class="ui_time"/><span class="add-on" style="height:20px;"><i data-time-icon="icon-time" data-date-icon="icon-calendar"></i></span></div>';
    sb += '<td><img id="expand_collapse" alt="" src="/resource/img/toggle-expand.png" style="width: 40px; height: 40px;" onclick="addTimeTable()" /></td></tr></table></div>';

    $('#timeTable').html(sb);
    $("#dpform1").val(showdate(-1));
    $("#dpdend1").val(showdate(0));
    $("#dpform2").val(showdate(-2));
    $("#dpdend2").val(showdate(-1));
    $('.input-append ').datetimepicker({
        format: 'yyyy-MM-dd hh:mm:ss',
        language: 'pt-BR'
    });
    //dateInput();  
}


