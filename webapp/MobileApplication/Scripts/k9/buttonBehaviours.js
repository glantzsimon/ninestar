function buttonBehaviours() {

    function displaySpinnerOnFormSubmit() {
        $("form").submit(function () {
            if ($(this).valid()) {
                var button = $(this).find("button.btn:submit:not('.payment-button')");
                button.button('loading');
                $("#pageSpinner").show();
                $("#pageOverlay").show();
            }
        });
    }

    function submitFormOnInputEnterKey() {
        $("input").keyup(function (e) {
            if (e.keyCode === 13) {
                $(this).closest("form").submit();
            }
        });
    }

    function displayPageSpinnerOnLinkClick() {
        $("a").click(function (e) {
            if (
                e.ctrlKey ||
                e.shiftKey ||
                e.metaKey ||
                (e.button && e.button === 1)
            ) {
                return;
            }

            var el = $(this);
            if (!el.hasClass("carousel-control") && !el.attr("target") && el.attr("data-toggle") !== "collapse") {
                var href = (el.attr("href"));
                if (href) {
                    if (href.indexOf("#") !== 0 && href.indexOf("mailto:") !== 0) {
                        $("#pageSpinner").show();
                        $("#pageOverlay").show();
                    } else {
                        if (href.length > 1) {
                            // Is bookmark
                            $(".navbar-collapse").removeClass("in");
                        }
                    }
                }
            }
        });
    }

    var init = function () {
        displaySpinnerOnFormSubmit();
        displayPageSpinnerOnLinkClick();
        submitFormOnInputEnterKey();
    };

    return {
        init: init,
        displayPageSpinnerOnLinkClick: displayPageSpinnerOnLinkClick
    };

};