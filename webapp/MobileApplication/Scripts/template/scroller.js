function scroller()
{

    function initScroller()
    {
        $(".scroll").click(function (event)
        {
            event.preventDefault();
            $('html,body').animate({ scrollTop: $(this.hash).offset().top }, 1000);
        });
    }

    function scrollToTop()
    {
        $().UItoTop({ easingType: 'easeOutQuart' });
    }

    function initParallax() {
        $('.jarallax').jarallax({
            speed: 0.5,
            imgWidth: 1366,
            imgHeight: 768
        });
    }

    var init = function ()
    {
        initScroller();
        scrollToTop();
        initParallax();
    };

    return {
        init: init
    }

}