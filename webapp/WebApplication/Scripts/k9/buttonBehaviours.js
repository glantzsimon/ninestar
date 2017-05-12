$(function ()
{

    function displaySpinnerOnFormSubmit() {
        $("form").submit(function() {
            var button = $(this).find("button.btn");
            button.button('loading');
        });
    }

    displaySpinnerOnFormSubmit();

})