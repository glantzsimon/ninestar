function calculatorMemory() {
    // Store form inputs only. Never persist tokens, credentials, result HTML or entitlements.
    var allowed = /^(?:(?:NineStarKiModel[12]\.)?PersonModel\.(?:Name|DateOfBirth|TimeOfBirth|BirthTimeZoneId|Gender)|CalculatorType|CalcType|CalculationMethod|TimeOfBirthKnown|HousesDisplay|DisplayDataForPeriod|SelectedDate|SelectedTime|UserTimeZoneId|InvertDailyAndHourlyKiForSouthernHemisphere|InvertDailyAndHourlyCycleKiForSouthernHemisphere|IsHideSexualChemistry)$/;

    var sharedKey = "NineStarKi.CalculatorPerson.v1";
    var birthFields = ["PersonModel.DateOfBirth", "PersonModel.TimeOfBirth", "PersonModel.BirthTimeZoneId", "PersonModel.Gender", "TimeOfBirthKnown"];

    function read(key) {
        try { return JSON.parse(localStorage.getItem(key)); } catch (e) { return null; }
    }

    function rememberPerson(values) {
        if (typeof values["PersonModel.DateOfBirth"] !== "string" || !values["PersonModel.DateOfBirth"]) return;
        var previous = read(sharedKey);
        // A DOB-only calculator may retain the time for the same person, but must
        // not attach the previous person's time/timezone to a different birth date.
        var person = previous && previous["PersonModel.DateOfBirth"] === values["PersonModel.DateOfBirth"] ? previous : {};
        birthFields.forEach(function (name) {
            if (Object.prototype.hasOwnProperty.call(values, name)) person[name] = values[name];
        });
        try { localStorage.setItem(sharedKey, JSON.stringify(person)); } catch (e) { /* Storage may be disabled. */ }
    }

    function init() {
        $("form[data-calculator-page]").each(function () {
            var form = this;
            var $form = $(form);
            var page = $form.attr("data-calculator-page");
            var key = "NineStarKi.CalculatorInputs.v1:" + page;
            var restoring = false;

            function fields() {
                return $form.find("input, select").filter(function () {
                    return allowed.test(this.name) && this.type !== "password" && this.type !== "file" &&
                        (this.type !== "hidden" || this.name === "CalcType" || $form.attr("data-calculator-hidden-inputs") === "true");
                });
            }

            function save(event) {
                if (restoring) return;
                var values = {};
                fields().each(function () {
                    if (this.type === "radio" && !this.checked) return;
                    // MVC emits a second hidden false value for each checkbox; it is excluded above.
                    values[this.name] = this.type === "checkbox" ? this.checked : ($(this).attr("data-calculator-date") || $(this).val());
                });
                try { localStorage.setItem(key, JSON.stringify({ values: values })); } catch (e) { /* Storage may be disabled. */ }
                if (event && event.type === "submit" && page !== "compatibility") rememberPerson(values);
            }

            if ($form.attr("data-calculator-restore") === "true") {
                try {
                    var stored = read(key);
                    var values = stored && stored.values && typeof stored.values === "object" ? stored.values : {};
                    var shared = read(sharedKey);
                    var savedPair = page === "compatibility" && Object.keys(values).some(function (name) {
                        return /^NineStarKiModel[12]\.PersonModel\./.test(name);
                    });
                    if (shared && typeof shared === "object" && !savedPair) {
                        birthFields.forEach(function (name) {
                            if (!Object.prototype.hasOwnProperty.call(shared, name)) return;
                            var target = page === "compatibility" ? "NineStarKiModel1." + name : name;
                            values[target] = shared[name];
                        });
                    }
                    if (Object.keys(values).length) {
                        restoring = true;
                        fields().each(function () {
                            if (!Object.prototype.hasOwnProperty.call(values, this.name)) return;
                            var value = values[this.name];
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
