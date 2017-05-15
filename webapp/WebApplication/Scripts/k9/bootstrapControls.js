$(function ()
{

    function initBootstrapDateTimePickers()
    {
        $("div.dateonly").datetimepicker({
            format: "L"
        });

        $("div.datetime").datetimepicker();
    }

    initBootstrapDateTimePickers();

})