$(document).ready(function () {
    $('header .nav > ul > li > a').css('color', '#000');
    $('header .nav > ul > li > a').hover(
        function () {
            $(this).css('color', '#c3c3c3');
        },
        function () {
            $(this).css('color', '#000');
        }
    );

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

	$('nav strong').click(function() {        
       $('nav ul').toggle();
    });
  
});