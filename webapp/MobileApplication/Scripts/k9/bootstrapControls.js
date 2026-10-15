function bootstrapControls(config) {

    function initBootstrapDateTimePickers() {
        $("div.dateonly").datetimepicker({
            format: "L"
        });

        $("div.datetime").datetimepicker({
            format: "L"
        });

        $("div.dateonly, div.datetime").on("dp.change", function (e) {
            var linkId = e.target.firstElementChild.id;
            var formattedDate = e.date.format("YYYY-MM-DD");
            $("input[linkid=" + linkId + "]").val(formattedDate);
        });
    }

    function initBootstrapSelect() {
        $(".selectpicker").selectpicker({
            size: 8
        });
        $("select").each(function () {
            this.getSelectedText = function () {
                return $(this).parent().find("li[data-original-index=" + this.selectedIndex + "] span.text").html();
            };
        });
    }

    function initTextScroller() {
        $(".scroller-container").vTicker({
            speed: 500,
            pause: 3000,
            showItems: 3,
            animation: "fade",
            mousePause: false,
            height: 0,
            direction: "up"
        });

    }

    function initDateTimeValidation() {
        $.validator.methods.date = function (value, element) {
            return this.optional(element) ||
                moment(value, config.dateFormat, true).isValid() ||
                moment(value, "YYYY-MM-DD", true).isValid();
        };
    }

    function initToolTips() {
        $('[data-toggle="tooltip"]').tooltip();
    }

    function selectFirstFormInput() {
        $("form").find("input[type=text], textarea, select").filter(":not(#main-search):visible:first").focus();
    }

    function initCollapsiblePanels() {
        $("a.collapsible-panel-link").mouseup(function () {
            var el = $(this);
            var i = el.find("i.fa");
            if (el.attr("aria-expanded") !== "true") {
                i.removeClass("fa-caret-down");
                i.addClass("fa-caret-up");
            } else {
                i.removeClass("fa-caret-up");
                i.addClass("fa-caret-down");
            }
        });

        $(".collapsible-panel-anchor").click(function () {
            var el = $(this);
            var panelId = el.attr("data-panel-id");

            window.setTimeout(function () {
                $("[data-panel-id='" + panelId + "'].panel-collapse").collapse("show");
            }, 200);
        });
    }

    function initGauges(maxValue) {
        var opts = {
            angle: 0.15, // The span of the gauge arc
            lineWidth: 0.44,
            radiusScale: 1, // Relative radius
            pointer: {
                length: 0.6, // Relative to gauge radius
                strokeWidth: 0.035, // The thickness
                color: "#000000"
            },
            limitMax: false, // If false, max value increases automatically if value > maxValue
            limitMin: false, // If true, the min value of the gauge will be fixed
            colorStart: "#6FADCF",
            colorStop: "#8FC0DA",
            strokeColor: "#E0E0E0",
            generateGradient: true,
            highDpiSupport: true
        };

        $(".gauge").each(function () {
            var $el = $(this);
            var value = parseInt($el.attr("data-value") || 0);
            var maxValue = parseInt($el.attr("data-max-value") || 10);
            var isInverted = $el.attr("data-invert") === "true" ? true : false;

            if (isInverted) {
                opts.percentColors = [[0.0, "#08F321"], [0.50, "#FCDA14"], [1.0, "#FD0510"]];
            }
            else {
                opts.percentColors = [[0.0, "#FD0510"], [0.50, "#FCDA14"], [1.0, "#08F321"]];
            }

            var gauge = new Gauge($el[0]).setOptions(opts);
            gauge.maxValue = maxValue;
            gauge.setMinValue(0);
            gauge.animationSpeed = 32;
            gauge.set(value);
        });
    }

    function initDataTables() {
        $(".k9-datatable").DataTable({
            "columnDefs": customColumnDefs || [],
            "order" : customOrder || []
        });
    }

    var init = function () {
        initBootstrapDateTimePickers();
        initBootstrapSelect();
        initDateTimeValidation();
        initToolTips();
        initTextScroller();
        initCollapsiblePanels();
        initGauges();
        initDataTables();
    };

    return {
        init: init,
        selectFirstFormInput: selectFirstFormInput
    };

}