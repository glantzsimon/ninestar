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

    displaySpinnerOnFormSubmit();

})