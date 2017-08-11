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

    function selectFirstFormInput()
    {
        $("form").find("input[type=text], textarea, select").filter(":not(#main-search):visible:first").focus();
    }

    function initToolTips()
    {
        $('[data-toggle="tooltip"]').tooltip();
    }

    function initFileInputs()
    {
        $(':file').on('fileselect', function (event, numFiles, fileName)
        {
            var fileDescription = numFiles > 1 ? numFiles + ' files selected' : fileName;
            var textInput = $(this).parents('.input-group').find(':text.file-label');
            
            if (textInput.length)
            {
                textInput.val(fileDescription);
            }
        });

        $(document).on('change', ':file', function () {
            var input = $(this);
            var numFiles = input.get(0).files ? input.get(0).files.length : 1;
            var fileName = input.val().replace(/\\/g, '/').replace(/.*\//, '');
            input.trigger('fileselect', [numFiles, fileName]);
        });
    }

    var init = function ()
    {
        initBootstrapDateTimePickers();
        initBootstrapSelect();
        initDateTimeValidation();
        initToolTips();
        initFileInputs();
    };

    return {
        init: init,
        selectFirstFormInput: selectFirstFormInput
    }

}