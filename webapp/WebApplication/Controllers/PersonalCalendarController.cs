using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using System;
using System.Globalization;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RoutePrefix("personal-calendar")]
    public class PersonalCalendarController : Controller
    {
        private const string CulturePreferenceKey = "PersonalCalendarCulture";
        private readonly IGoogleCalendarService _calendarService;
        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private static readonly string[] EnergyImages = { "water", "soil", "thunder", "wind", "coreearth", "heaven", "lake", "mountain", "fire" };

        public PersonalCalendarController(IGoogleCalendarService calendarService, IUserService userService, IMembershipService membershipService)
        {
            _calendarService = calendarService;
            _userService = userService;
            _membershipService = membershipService;
        }

        [HttpGet]
        [Route("month")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Month(int? year = null, int? month = null)
        {
            var timer = Stopwatch.StartNew();
            Debug.WriteLine("Personal calendar Month: action entered.");
            var userId = Current.UserId;
            if (!CanAccess(userId))
                return new HttpStatusCodeResult(403);
            Debug.WriteLine($"Personal calendar Month: access check completed at {timer.ElapsedMilliseconds} ms.");
            if (!ModelState.IsValid || year.HasValue != month.HasValue ||
                (year.HasValue && (year < 1900 || year > 2100 || month < 1 || month > 12)))
                return new HttpStatusCodeResult(400);
            try
            {
                var today = _calendarService.GetCalendarToday(userId);
                Debug.WriteLine($"Personal calendar Month: local today resolved at {timer.ElapsedMilliseconds} ms.");
                var start = new DateTime(year ?? today.Year, month ?? today.Month, 1);
                var entries = _calendarService.GetCalendarEntries(userId, start, start.AddMonths(1), GetClientDisconnectedToken());
                Debug.WriteLine($"Personal calendar Month: calendar entries returned at {timer.ElapsedMilliseconds} ms.");
                return Json(new
                {
                    Year = start.Year,
                    Month = start.Month,
                    Title = start.ToString("Y", CultureInfo.CurrentCulture),
                    Today = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Offset = ((int)start.DayOfWeek + 6) % 7,
                    Entries = entries.Select(e => new
                    {
                        Date = e.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        DateLabel = e.Date.ToString("D", CultureInfo.CurrentCulture),
                        Day = e.Date.Day,
                        e.YearHouse,
                        e.MonthHouse,
                        e.DayHouse,
                        e.AfternoonDayHouse,
                        e.Summary,
                        e.Description,
                        Houses = string.Join(" · ", new[]
                        {
                            K9.Globalisation.Dictionary.Year + ": " + GetEnergyName(e.YearHouse),
                            K9.Globalisation.Dictionary.Month + ": " + GetEnergyName(e.MonthHouse),
                            K9.Globalisation.Dictionary.Day + ": " + GetEnergyName(e.DayHouse)
                        }),
                        EnergyName = GetEnergyName(e.DayHouse),
                        ImageUrl = MediaService.BaseImagesPath + "/ninestar/energies/" + EnergyImages[e.DayHouse - 1] + ".png"
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (OperationCanceledException)
            {
                // The superseded browser request no longer needs a response.
                return new EmptyResult();
            }
            catch (InvalidOperationException)
            {
                return new HttpStatusCodeResult(409);
            }
        }

        private CancellationToken GetClientDisconnectedToken()
        {
            try
            {
                return Response?.ClientDisconnectedToken ?? CancellationToken.None;
            }
            catch (PlatformNotSupportedException)
            {
                // Non-IIS hosts may not expose disconnect notification.
                return CancellationToken.None;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("subscription")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Subscription()
        {
            var userId = Current.UserId;
            if (!CanAccess(userId))
                return new HttpStatusCodeResult(403);
            var token = _calendarService.GetOrCreateSubscriptionToken(userId);
            _userService.UpdateUserPreference(userId, CulturePreferenceKey,
                (K9.Globalisation.Dictionary.Culture ?? CultureInfo.CurrentUICulture).Name);
            // Keep the case-sensitive token in the query, outside lower-case route generation.
            var url = Url.Action("Feed", "PersonalCalendar", null, Request.Url.Scheme) + "?token=" + Uri.EscapeDataString(token);
            return Json(new { Url = url });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("revoke")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Revoke()
        {
            _calendarService.RevokeSubscription(Current.UserId);
            return Json(new { Success = true });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("feed")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Feed(string token)
        {
            var userId = _calendarService.GetUserIdFromSubscriptionToken(token);
            if (!userId.HasValue || !CanAccess(userId.Value))
                return HttpNotFound();
            var previousCulture = Thread.CurrentThread.CurrentCulture;
            var previousUiCulture = Thread.CurrentThread.CurrentUICulture;
            try
            {
                var name = _userService.GetUserPreference(userId.Value, CulturePreferenceKey, "en-GB");
                var culture = CultureInfo.GetCultureInfo(name);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                var today = _calendarService.GetCalendarToday(userId.Value);
                return Content(_calendarService.GenerateCalendar(userId.Value, today.AddMonths(-1), today.AddYears(1).AddDays(1)),
                    "text/calendar", Encoding.UTF8);
            }
            catch (InvalidOperationException) { return new HttpStatusCodeResult(409); }
            finally
            {
                Thread.CurrentThread.CurrentCulture = previousCulture;
                Thread.CurrentThread.CurrentUICulture = previousUiCulture;
            }
        }

        private bool CanAccess(int userId)
        {
            return userId > 0 && (_userService.UserIsAdmin(userId) ||
                _membershipService.GetActiveUserMembership(userId)?.IsAuthorisedToViewPaidContent() == true);
        }

        private static string GetEnergyName(int number)
        {
            return number.ToString(CultureInfo.InvariantCulture) + " " + new NineStarKiEnergy((ENineStarKiEnergy)number).EnergyName;
        }
    }
}
