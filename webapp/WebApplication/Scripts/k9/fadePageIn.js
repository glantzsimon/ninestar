function fadePageIn()
{

    function doFadeIn() {
        $("div#pageFadeInOverlay").fadeOut(800);
    }

    var init = function() {
        doFadeIn();
    };

    return {
        init: init
    }

};