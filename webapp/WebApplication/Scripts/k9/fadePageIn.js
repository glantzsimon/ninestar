function fadePageIn()
{

    function doFadeIn() {
        $("div#pageFadeInOverlay").fadeOut(1200);
    }

    var init = function() {
        doFadeIn();
    };

    return {
        init: init
    }

};