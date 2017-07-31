function bootstrapControls(config)
{

    function initBootstrapDateTimePickers()
    {
        $("div.dateonly").datetimepicker({
            format: "L"
        });

        $("div.datetime").datetimepicker({
            format: "L"
        });
    }

    function initBootstrapSelect()
    {
        $(".selectpicker").selectpicker({
            size: 8
        });
    }

    function initDateTimeValidation()
    {
        $.validator.methods.date = function (value, element)
        {
            return this.optional(element) || moment(value, config.dateFormat, true).isValid();
        }
    }

    function selectFirstformInput()
    {
        $("form").find("input[type=text], textarea, select").filter(":not(#main-search):visible:first").focus();
    }

    function initToolTips()
    {
        $('[data-toggle="tooltip"]').tooltip();
    }

    var init = function ()
    {
        initBootstrapDateTimePickers();
        initBootstrapSelect();
        initDateTimeValidation();
        selectFirstformInput();
        initToolTips();
    };

    return {
        init: init
    }

}