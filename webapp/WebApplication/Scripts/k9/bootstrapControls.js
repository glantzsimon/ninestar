function initBootstrapControls(config) {

    function initBootstrapDateTimePickers() {
        $("div.dateonly").datetimepicker({
            locale: config.language
        });

        $("div.datetime").datetimepicker({
            locale: config.language
        });
    }

    function initBootstrapSelect() {
        $(".selectpicker").selectpicker({
            size: 8
        });
    }

    initBootstrapDateTimePickers();
    initBootstrapSelect();
}