(function ($) {
    'use strict';

    $(function () {
        var $calendar = $('#personal-calendar');
        if (!$calendar.length || $calendar.closest('.paywall-remove-element').length) return;

        var $grid = $calendar.find('.personal-calendar-grid'),
            $status = $calendar.find('.personal-calendar-status'),
            $copy = $calendar.find('.calendar-copy'),
            $link = $calendar.find('#personal-calendar-link'),
            currentYear, currentMonth, today, entries = [], request, requestVersion = 0,
            selectedDate, subscriptionBusy = false, $placeholder, previousFocus;

        function status(message) { $status.text(message || ''); }
        function select(entry) {
            selectedDate = entry.Date;
            $grid.find('.personal-calendar-day').removeClass('is-selected').attr('aria-pressed', 'false');
            $grid.find('[data-date="' + entry.Date + '"]').addClass('is-selected').attr('aria-pressed', 'true');
            $calendar.find('.calendar-selected-date').text(entry.DateLabel);
            $calendar.find('.calendar-selected-houses').text(entry.Houses);
            var $description = $calendar.find('.calendar-selected-description').empty();
            (entry.Description || '').split(/\r?\n\s*\r?\n/).forEach(function (paragraph) {
                $('<p>').text(paragraph).appendTo($description);
            });
        }

        function loadMonth(year, month) {
            var version = ++requestVersion;
            if (request) request.abort();
            $calendar.attr('aria-busy', 'true');
            $grid.find('.personal-calendar-day, .personal-calendar-empty').remove();
            $calendar.find('.calendar-selected-date, .calendar-selected-houses, .calendar-selected-description').empty();
            status($calendar.attr('data-loading'));
            request = $.getJSON($calendar.attr('data-month-url'), year ? { year: year, month: month } : {})
                .done(function (data) {
                    if (version !== requestVersion) return;
                    currentYear = data.Year;
                    currentMonth = data.Month;
                    today = data.Today;
                    entries = data.Entries;
                    $calendar.find('.personal-calendar-month').text(data.Title);
                    for (var i = 0; i < data.Offset; i++) $('<div class="personal-calendar-empty" aria-hidden="true">').appendTo($grid);
                    entries.forEach(function (entry) {
                        var numbers = entry.YearHouse + '.' + entry.MonthHouse + '.' + entry.DayHouse;
                        var $day = $('<button type="button" class="personal-calendar-day" aria-pressed="false">')
                            .attr('data-date', entry.Date).attr('aria-label', entry.DateLabel + ' · ' + entry.Houses)
                            .attr('title', entry.EnergyName).toggleClass('is-today', entry.Date === today);
                        $('<span>').text(entry.Day).appendTo($day);
                        $('<img>').attr({ src: entry.ImageUrl, alt: entry.EnergyName }).appendTo($day);
                        $('<span class="calendar-day-numbers">').text(numbers).appendTo($day);
                        $day.on('click', function () { select(entry); }).appendTo($grid);
                    });
                    var chosen = entries.filter(function (e) { return e.Date === selectedDate; })[0] ||
                        entries.filter(function (e) { return e.Date === today; })[0] || entries[0];
                    if (chosen) select(chosen);
                    $calendar.find('.calendar-previous').prop('disabled', currentYear === 1900 && currentMonth === 1);
                    $calendar.find('.calendar-next').prop('disabled', currentYear === 2100 && currentMonth === 12);
                    status('');
                }).fail(function (xhr, result) {
                    if (result !== 'abort' && version === requestVersion)
                        status($calendar.attr(xhr.status === 409 ? 'data-unavailable' : 'data-error'));
                }).always(function () {
                    if (version === requestVersion) $calendar.attr('aria-busy', 'false');
                });
        }

        function navigate(delta) {
            if (!currentYear) return;
            var date = new Date(currentYear, currentMonth - 1 + delta, 1);
            if (date.getFullYear() < 1900 || date.getFullYear() > 2100) return;
            loadMonth(date.getFullYear(), date.getMonth() + 1);
        }
        $calendar.find('.calendar-previous').on('click', function () { navigate(-1); });
        $calendar.find('.calendar-next').on('click', function () { navigate(1); });
        $calendar.find('.calendar-today').on('click', function () { selectedDate = null; loadMonth(); });

        function expand(value) {
            if (value) {
                previousFocus = document.activeElement;
                $placeholder = $('<div>').insertBefore($calendar);
                $calendar.appendTo(document.body);
                $calendar.attr({ role: 'dialog', 'aria-modal': 'true', 'aria-label': $calendar.attr('data-title') });
            } else {
                $calendar.insertBefore($placeholder);
                $placeholder.remove();
                $calendar.removeAttr('role aria-modal aria-label');
            }
            $calendar.toggleClass('is-expanded', value);
            $('body').toggleClass('personal-calendar-open', value);
            $calendar.find('.calendar-expand').attr('aria-expanded', value ? 'true' : 'false')
                .find('span').text($calendar.attr(value ? 'data-close' : 'data-expand'));
            $calendar.find('.calendar-expand i').toggleClass('fa-expand', !value).toggleClass('fa-compress', value);
            if (value) $calendar.find('.calendar-expand').focus();
            else if (previousFocus) previousFocus.focus();
        }
        $calendar.find('.calendar-expand').on('click', function () { expand(!$calendar.hasClass('is-expanded')); });
        $(document).on('keydown.personalCalendar', function (e) {
            if (!$calendar.hasClass('is-expanded')) return;
            if (e.key === 'Escape') { e.preventDefault(); expand(false); }
            if (e.key === 'Tab') {
                var $focusable = $calendar.find('button:visible:not(:disabled), input:visible, a:visible'),
                    first = $focusable[0], last = $focusable[$focusable.length - 1];
                if (e.shiftKey && document.activeElement === first) { e.preventDefault(); last.focus(); }
                else if (!e.shiftKey && document.activeElement === last) { e.preventDefault(); first.focus(); }
            }
        });

        function post(url) {
            return $.post(url, { __RequestVerificationToken: $calendar.find('[name="__RequestVerificationToken"]').val() });
        }
        function copyLink() {
            $link[0].focus();
            $link[0].select();
            if (navigator.clipboard && window.isSecureContext) {
                navigator.clipboard.writeText($link.val()).then(function () {
                    status($calendar.attr('data-copied'));
                }, function () { status($calendar.attr('data-copy-error')); });
            } else {
                try { status($calendar.attr(document.execCommand('copy') ? 'data-copied' : 'data-copy-error')); }
                catch (e) { status($calendar.attr('data-copy-error')); }
            }
        }
        $copy.on('click', function () {
            if (subscriptionBusy) return;
            if ($link.val()) { copyLink(); return; }
            subscriptionBusy = true;
            $copy.prop('disabled', true);
            post($calendar.attr('data-subscription-url')).done(function (data) {
                $link.val(data.Url);
                $calendar.find('.calendar-link-container, .calendar-disable').show();
                copyLink();
            }).fail(function () { status($calendar.attr('data-error')); })
                .always(function () { subscriptionBusy = false; $copy.prop('disabled', false); });
        });
        $calendar.find('.calendar-disable').on('click', function () {
            if (subscriptionBusy) return;
            subscriptionBusy = true;
            $calendar.find('.calendar-copy, .calendar-disable').prop('disabled', true);
            post($calendar.attr('data-revoke-url')).done(function () {
                $link.val('');
                $calendar.find('.calendar-link-container, .calendar-disable').hide();
                status($calendar.attr('data-revoked'));
            }).fail(function () { status($calendar.attr('data-error')); })
                .always(function () {
                    subscriptionBusy = false;
                    $calendar.find('.calendar-copy, .calendar-disable').prop('disabled', false);
                });
        });

        loadMonth();
    });
})(jQuery);
