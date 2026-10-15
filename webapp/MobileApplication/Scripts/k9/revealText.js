function revealText(config) {

    function expandCollapse(button) {
        var container = $(button).closest(".reveal-text");
        var span = $(button).find("a > span");
        var icon = $(button).find("i");

        if (container.hasClass("expanded")) {
            container.removeClass("expanded");
            span.html(config.moreText);
            icon.removeClass("fa-arrow-up");
            icon.addClass("fa-arrow-down");

            $.fn.scrollToTopOf(container);
        } else {
            container.addClass("expanded");
            span.html(config.lessText);
            icon.removeClass("fa-arrow-down");
            icon.addClass("fa-arrow-up");
        }
    }

    function init() {
        $(".reveal-text").each(function () {
            if ($(this).height() >= 900) {
                var fadeOutDiv = $("<div class='reveal-text-overlay'>&nbsp;</div>");
                $(this).append(fadeOutDiv);

                var revealButton =
                    $(
                        "<div class='reveal-text-button-container text-center'><a class='reveal-text-button'><i class='fa fa-arrow-down'></i> " +
                        "<span>" + config.moreText + "</span></a></div>");
                $(this).append(revealButton);

                revealButton.click(function () {
                    expandCollapse(this);
                });
            }
        });
    }

    return {
        init: init
    };

}