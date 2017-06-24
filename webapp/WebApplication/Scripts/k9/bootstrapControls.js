function initBootstrapControls(config)
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

    initBootstrapDateTimePickers();
    initBootstrapSelect();
    initDateTimeValidation();
}