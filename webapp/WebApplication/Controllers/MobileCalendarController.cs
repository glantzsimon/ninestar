using K9.Globalisation;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/calendar")]
    public class MobileCalendarController : MobileApiControllerBase
    {
        private readonly IGoogleCalendarService _calendar;
        public MobileCalendarController(INineStarKiPackage package, IGoogleCalendarService calendar) : base(package) { _calendar = calendar; }

        [HttpGet, Route("month")]
        public ActionResult Month(int? year = null, int? month = null)
        {
            var error = RequireUser(out var id, paid: true);
            if (error != null) return error;
            if (!ModelState.IsValid || year.HasValue != month.HasValue ||
                (year.HasValue && (year < 1900 || year > 2100 || month < 1 || month > 12))) return ApiError(400, "invalid_calendar_month");
            try
            {
                var today = _calendar.GetCalendarToday(id);
                var start = new DateTime(year ?? today.Year, month ?? today.Month, 1);
                var entries = _calendar.GetCalendarEntries(id, start, start.AddMonths(1), DisconnectedToken());
                return ApiSuccess(new
                {
                    Help = K9.WebApplication.Helpers.MobileHelpMapper.Calendar(),
                    Year = start.Year, Month = start.Month, Title = start.ToString("Y", CultureInfo.CurrentCulture),
                    Today = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), Offset = ((int)start.DayOfWeek + 6) % 7,
                    Entries = entries.Select(e => new
                    {
                        Date = e.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        e.YearHouse, e.MonthHouse, e.DayHouse, e.AfternoonDayHouse, e.Summary, e.Description
                    }).ToArray()
                });
            }
            catch (InvalidOperationException) { return ApiError(409, "calendar_details_required"); }
            catch (OperationCanceledException) { return ApiError(408, "request_cancelled"); }
        }

        [HttpPost, Route("subscription")]
        public ActionResult Subscription()
        {
            var error = RequireUser(out var id, paid: true);
            if (error != null) return error;
            if (!Request.IsSecureConnection && !Request.IsLocal) return ApiError(400, "https_required");
            var token = _calendar.GetOrCreateSubscriptionToken(id);
            Package.UserService.UpdateUserPreference(id, "PersonalCalendarCulture", (Dictionary.Culture ?? CultureInfo.CurrentUICulture).Name);
            var url = Url.Action("Feed", "PersonalCalendar", null, Request.Url.Scheme) + "?token=" + Uri.EscapeDataString(token);
            return ApiSuccess(new { Url = url });
        }

        [HttpPost, Route("revoke")]
        public ActionResult Revoke()
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            _calendar.RevokeSubscription(id);
            return ApiSuccess(new { Revoked = true });
        }

        private CancellationToken DisconnectedToken()
        {
            try { return Response.ClientDisconnectedToken; }
            catch (PlatformNotSupportedException) { return CancellationToken.None; }
        }
    }
}
