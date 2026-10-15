function imagePreloader(config) {

    var init = function() {
        $.fn.preloadImages(config.imageArray);
    };

    return {
        init: init
    };

}