function initBootstrapControls(config) {

    function initBootstrapDateTimePickers() {
        $("div.dateonly").datetimepicker({
            locale: "fr",
            format: "L"
        });

        $("div.datetime").datetimepicker({
            locale: config.language,
            format: "L"
        });

        $("input[type=datetime]").removeAttr("data-val-date");
    }

    function initBootstrapSelect() {
        $(".selectpicker").selectpicker({
            size: 8
        });
    }

    initBootstrapDateTimePickers();
    initBootstrapSelect();
}