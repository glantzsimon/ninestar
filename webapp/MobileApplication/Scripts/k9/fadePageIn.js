function fadePageIn(config)
{

    function doFadeIn()
    {
        var delay = config.isFirstLoad ? 1000 : 200;
        $("div#pageFadeInOverlay").delay(delay).fadeOut(delay, function ()
        {
        });
    }

    function initScrollReveal()
    {
        var beforeReveal = function (el)
        {
            $(el).trigger("reveal");
        };

        window.sr = ScrollReveal();
        if (window.matchMedia("(min-width: 992px)").matches)
        {

            sr.reveal(".scrollFadeUp",
                {
                    duration: 1200,
                    distance: "100px",
                    scale: 1,
                    beforeReveal: function (el)
                    {
                        beforeReveal(el);
                    }
                });
            sr.reveal(".scrollFadeIn",
                {
                    duration: 1000,
                    distance: "4px",
                    scale: 1,
                    beforeReveal: function (el)
                    {
                        beforeReveal(el);
                    }
                });
        } else
        {
            beforeReveal(".scrollFadeUp");
        }
    }

    var init = function ()
    {
        initScrollReveal();
        doFadeIn();
    };

    return {
        init: init
    };

};