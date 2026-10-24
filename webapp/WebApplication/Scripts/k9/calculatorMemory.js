function calculatorMemory() {
    // Store form inputs only. Never persist tokens, credentials, result HTML or entitlements.
    var allowed = /^(?:(?:NineStarKiModel[12]\.)?PersonModel\.(?:Name|DateOfBirth|TimeOfBirth|BirthTimeZoneId|Gender)|CalculatorType|CalcType|CalculationMethod|TimeOfBirthKnown|HousesDisplay|DisplayDataForPeriod|SelectedDate|SelectedTime|UserTimeZoneId|InvertDailyAndHourlyKiForSouthernHemisphere|InvertDailyAndHourlyCycleKiForSouthernHemisphere|IsHideSexualChemistry)$/;

    function init() {
        $("form[data-calculator-page]").each(function () {
            var form = this;
            var $form = $(form);
            var key = "NineStarKi.CalculatorInputs.v1:" + $form.attr("data-calculator-page");
            var restoring = false;

            function fields() {
                return $form.find("input, select").filter(function () {
                    return allowed.test(this.name) && this.type !== "password" && this.type !== "file" &&
                        (this.type !== "hidden" || this.name === "CalcType" || $form.attr("data-calculator-hidden-inputs") === "true");
                });
            }

            function save() {
                if (restoring) return;
                var values = {};
                fields().each(function () {
                    if (this.type === "radio" && !this.checked) return;
                    // MVC emits a second hidden false value for each checkbox; it is excluded above.
                    values[this.name] = this.type === "checkbox" ? this.checked : ($(this).attr("data-calculator-date") || $(this).val());
                });
                try { localStorage.setItem(key, JSON.stringify({ values: values })); } catch (e) { /* Storage may be disabled. */ }
            }

            if ($form.attr("data-calculator-restore") === "true") {
                try {
                    var stored = JSON.parse(localStorage.getItem(key));
                    if (stored && stored.values && typeof stored.values === "object") {
                        restoring = true;
                        fields().each(function () {
                            if (!Object.prototype.hasOwnProperty.call(stored.values, this.name)) return;
                            var value = stored.values[this.name];
                            if (this.type === "checkbox") this.checked = value === true;
                            else if (this.type === "radio") this.checked = this.value === value;
                            else if (typeof value === "string" || Array.isArray(value)) $(this).val(value);

                            var linkId = $(this).attr("linkid");
                            if (linkId && typeof value === "string" && /^\d{4}-\d{2}-\d{2}$/.test(value)) {
                                var display = document.getElementById(linkId);
                                var picker = $(display).closest(".date").data("DateTimePicker");
                                if (picker) picker.date(moment(value, "YYYY-MM-DD", true));
                            }
                        });
                        // These two handlers only update visibility. Do not trigger the
                        // calculator/method handlers, which also write account preferences.
                        fields().filter("[name='TimeOfBirthKnown'], [name='DisplayDataForPeriod']").trigger("change");
                        var $calculatorOptions = $form.find(".calculator-options input:checked");
                        if ($calculatorOptions.length) {
                            var advanced = $calculatorOptions.val() === "1";
                            $form.find(".advanced-calculator-options").toggle(advanced);
                            $form.find("div[data-input-id='Gender']").toggle(!advanced || $form.find("#CalculationMethod").val() === "0");
                        }
                        if ($.fn.selectpicker) $form.find("select.selectpicker").selectpicker("refresh");
                        restoring = false;
                    }
                } catch (e) { restoring = false; /* Malformed/unavailable storage must not block calculation. */ }
            }

            $form.on("input.calculatorMemory change.calculatorMemory", "input, select", save);
            $form.on("dp.change.calculatorMemory", ".date", save);
            $form.on("submit.calculatorMemory", save);
        });
    }

    return { init: init };
}
