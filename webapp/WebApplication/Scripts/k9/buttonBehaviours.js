$(function ()
{

    function displaySpinnerOnFormSubmit() {
        $("form").submit(function () {
            if ($(this).valid()) {
                var button = $(this).find("button.btn");
                button.button('loading');
            }
        });
    }

    function displayPageSpinnerOnLinkClick() {
        $("a").click(function () {
            var href = ($(this).attr("href"));
            if (href) {
                if (href !== "#") {
                    $("#pageSpinner").show();
                    $("#pageOverlay").show();
                }
            }
        });
    }

    displaySpinnerOnFormSubmit();
    displayPageSpinnerOnLinkClick();

})