function initBootstrapControls(config)
{

    function initBootstrapDateTimePickers()
    {
        $("div.dateonly").datetimepicker({
            locale: config.language,
            format: "L"
        });

        $("div.datetime").datetimepicker({
            locale: config.language,
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

    initBootstrapDateTimePickers();
    initBootstrapSelect();
    initDateTimeValidation();
}