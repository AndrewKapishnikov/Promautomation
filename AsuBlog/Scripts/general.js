$(window).on('load', function () {
    var $preloader = $('#animbox'),
        $svg_anm = $preloader.find('.svg_anm');
    $svg_anm.fadeOut();
    $preloader.delay(500).fadeOut('slow');
});


//Collapse TreeView
$(document).ready(function () {
 
    //$(".navbar-collapse").collapse('hide');
    $(".treeview li>ul>li>ul").css('display', 'none'); // Hide all 2-level ul 
    $(".collapsibleTree").click(function (e) {
        e.preventDefault();
        $(this).toggleClass("collapseTree expand");
        $(this).closest('li').children('ul').slideToggle();
    });

    $('.limenu_div').click(function (e) {
        e.preventDefault();
        $(this).next('.submenu').addClass('flag-up');
        $('.submenu:not(.flag-up)').slideUp();
                
        $(this).next('.submenu').slideToggle();
        $(this).next('.submenu').removeClass('flag-up');
    });

    $(window).resize(function () { // задаем функцию при срабатывании события "resize" на объекте window
        var width = $(window).width();// ширина области просмотра браузера
        if (width >= 768 && width <= 1120)
        {
            $('.submenu').css('display', 'none');
            //$('.submenu').each(function (i, elem) {
            //    $(elem).css('display', 'none');
            //});
        }
        
        if (width > 1120)
            $('.submenu').css('display', 'block');

        //if ($('.navbar').width() >= 768)
        //    $('.submenu').css('display', 'none');
    });

    $('.generalcontainer').click(function (event) {
     
        var _opened = $(".navbar-collapse").hasClass("in"); //когда navbar-collapse открыт этот div имеет класс in
        if (_opened === true) {
            $("button.navbar-toggle").click();
            $('.submenu').css('display', 'none');
        }
    });
    $('footer').click(function (event) {

        $("#nav-toggle").prop('checked', false);
        var _opened = $(".navbar-collapse").hasClass("in"); //когда navbar-collapse открыт этот div имеет класс in
        if (_opened === true) {
            $("button.navbar-toggle").click();
            $('.submenu').css('display', 'none');
        }

        if ($(window).width() < 1120) {
            $("#buttonbox").prop('checked', false);
        }
    });

    $('#buttontoggle').click(function () {

        $("#nav-toggle").prop('checked', false);

    });
    
    $('.containermain').click(function () {

        $("#nav-toggle").prop('checked', false);

    });
   

    $('.containermain-inner').click(function () {
        if ($(window).width() < 1120) {
        $("#buttonbox").prop('checked', false);
        }
    });

    $('#search-form').submit(function () {
        if ($("#s").val().trim())
            return true;
        return false;
    });
   
    $('#myCanvas').tagcanvas({
        textColour: '#00002f',
        textHeight: '25',
        maxBrightness: '10.0',
        minBrightness: '0.19',
        shape: 'vcylinder',
        maxSpeed: 0.07,
        depth: 0.4,
        weight: true
    });

   $(".compItem").click(function (e) {
       e.preventDefault();
       $('#modDialog').modal('show');
   
    });

    $('.maindiv').click(function () {
       if ($(window).width() < 1120) {
            $("#buttonbox").prop('checked', false);
        }
    });

  

});
