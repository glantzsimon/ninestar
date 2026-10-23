using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using K9.WebApplication.Enums;
using K9.WebApplication.ViewModels.MobileApi;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    // Deliberately avoids BaseNineStarKiController's public UI/upload actions and
    // session-writing initialisation. The plugin ApiController remains independent.
    public abstract class MobileApiControllerBase : Controller
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Culture = CultureInfo.InvariantCulture,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            TypeNameHandling = TypeNameHandling.None,
            Converters = { new StringEnumConverter() }
        };

        protected MobileApiControllerBase(INineStarKiPackage package)
        {
            Package = package;
        }

        protected INineStarKiPackage Package { get; }

        protected int GetAuthenticatedUserId()
        {
            // An API key or account number never establishes a mobile user identity.
            return Request.IsAuthenticated ? ResolveCurrentUserId() : 0;
        }

        protected virtual int ResolveCurrentUserId() => Current.UserId;

        protected virtual DateTime GetUtcNow() => DateTime.UtcNow;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            Response.SuppressFormsAuthenticationRedirect = true;
            Response.TrySkipIisCustomErrors = true;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            if (Request.ContentLength > 65536)
            {
                filterContext.Result = ApiError(413, "request_too_large");
                return;
            }
            // Mobile clients maintain a cookie jar and echo the request token
            // obtained from /account/csrf. Cookie-authenticated writes need CSRF
            // protection even when their body is JSON.
            if (!string.Equals(Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(Request.HttpMethod, "HEAD", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    AntiForgery.Validate(Request.Cookies[AntiForgeryConfig.CookieName]?.Value,
                        Request.Headers["X-CSRF-Token"] ?? Request.Form["__RequestVerificationToken"]);
                }
                catch (HttpAntiForgeryException)
                {
                    filterContext.Result = ApiError(400, "invalid_csrf_token");
                }
            }
        }

        protected ActionResult RequireUser(out int userId, bool paid = false, bool admin = false)
        {
            userId = GetAuthenticatedUserId();
            if (userId <= 0) return ApiError(401, "authentication_required");
            var isAdmin = Package.UserService.UserIsAdmin(userId);
            if (admin && !isAdmin) return ApiError(403, "administrator_required");
            if (paid && !isAdmin && Package.MembershipService.GetActiveUserMembership(userId)?.IsAuthorisedToViewPaidContent() != true)
                return ApiError(403, "membership_required");
            return null;
        }

        protected bool ValidInput(object input)
        {
            if (input == null || !ModelState.IsValid) return false;
            return Validator.TryValidateObject(input, new ValidationContext(input), new List<ValidationResult>(), true);
        }

        protected void SavePreferences(int id, MobileCalculationRequest settings)
        {
            var users = Package.UserService;
            users.UpdateUserPreference(id, Constants.SessionConstants.UserTimeZone, settings.UserTimeZoneId);
            users.UpdateUserPreference(id, Constants.SessionConstants.UserCalculationMethod, settings.CalculationMethod);
            users.UpdateUserPreference(id, Constants.SessionConstants.DefaultCalculatorType, settings.CalculatorType);
            users.UpdateUserPreference(id, Constants.SessionConstants.UserHousesDisplay, settings.HousesDisplay);
            users.UpdateUserPreference(id, Constants.SessionConstants.BirthTimeIsKnown, settings.Person.TimeOfBirthKnown);
            users.UpdateUserPreference(id, Constants.SessionConstants.InvertDailyAndHourlyKiForSouthernHemisphere, settings.InvertDailyAndHourlyKiForSouthernHemisphere);
            users.UpdateUserPreference(id, Constants.SessionConstants.InvertDailyAndHourlyCycleKiForSouthernHemisphere, settings.InvertDailyAndHourlyCycleKiForSouthernHemisphere);
        }

        protected void SaveLastRequest<T>(string section, T request)
        {
            // Store input only, never a paid response. Identity forms part of the
            // session key so logging in or impersonating cannot reveal another
            // account's birth details. Retrieval rechecks current entitlement.
            var id = GetAuthenticatedUserId();
            if (id > 0 && Session != null)
                Session["MobileLast:" + id + ":" + section] = JsonConvert.SerializeObject(request, JsonSettings);
        }

        protected T ReadLastRequest<T>(string section) where T : class
        {
            var id = GetAuthenticatedUserId();
            if (id <= 0 || Session == null) return null;
            var json = Session["MobileLast:" + id + ":" + section] as string;
            return json == null ? null : JsonConvert.DeserializeObject<T>(json, JsonSettings);
        }

        protected NineStarKiModel CalculateProfile(INineStarKiService service, MobileCalculationRequest request,
            bool cycles = false, bool moon = false, bool own = false)
        {
            return service.CalculateNineStarKiProfile(request.Person.ToPerson(), false, own,
                request.GetSelectedDate(GetUtcNow()), request.CalculationMethod, request.CalculatorType,
                cycles, false, request.UserTimeZoneId, request.HousesDisplay,
                request.InvertDailyAndHourlyKiForSouthernHemisphere,
                request.InvertDailyAndHourlyCycleKiForSouthernHemisphere,
                EDisplayDataForPeriod.SelectedDate, moon);
        }

        protected MobileCalculationRequest ReadOwnCalculationRequest(int userId)
        {
            var user = Package.UserService.Find(userId);
            var info = Package.UserInfosRepository.Find(e => e.UserId == userId).FirstOrDefault();
            if (user == null || info == null || user.BirthDate == default(DateTime)) return null;
            var users = Package.UserService;
            return new MobileCalculationRequest
            {
                Person = new MobilePersonRequest
                {
                    Name = user.FullName, DateOfBirth = user.BirthDate.Date, Gender = user.Gender,
                    BirthTimeZoneId = info.BirthTimeZoneId, TimeOfBirth = info.TimeOfBirth,
                    TimeOfBirthKnown = users.GetUserPreference(userId, Constants.SessionConstants.BirthTimeIsKnown, info.TimeOfBirth != TimeSpan.Zero)
                },
                UserTimeZoneId = users.GetUserPreference(userId, Constants.SessionConstants.UserTimeZone, info.BirthTimeZoneId),
                CalculationMethod = users.GetUserPreference(userId, Constants.SessionConstants.UserCalculationMethod, info.CalculationMethod),
                CalculatorType = users.GetUserPreference(userId, Constants.SessionConstants.DefaultCalculatorType, info.CalculatorType),
                HousesDisplay = users.GetUserPreference(userId, Constants.SessionConstants.UserHousesDisplay, info.HousesDisplay),
                InvertDailyAndHourlyKiForSouthernHemisphere = users.GetUserPreference(userId, Constants.SessionConstants.InvertDailyAndHourlyKiForSouthernHemisphere, info.InvertDailyAndHourlyKiForSouthernHemisphere),
                InvertDailyAndHourlyCycleKiForSouthernHemisphere = users.GetUserPreference(userId, Constants.SessionConstants.InvertDailyAndHourlyCycleKiForSouthernHemisphere, info.InvertDailyAndHourlyKiForSouthernHemisphere)
            };
        }

        protected ContentResult ApiSuccess(object data)
        {
            return ApiJson(200, new { success = true, data, error = (object)null });
        }

        protected ContentResult ApiError(int statusCode, string code)
        {
            // Codes are protocol values; clients localise their own error messages.
            return ApiJson(statusCode, new { success = false, data = (object)null, error = new { code } });
        }

        private ContentResult ApiJson(int statusCode, object payload)
        {
            Response.StatusCode = statusCode;
            Response.SuppressFormsAuthenticationRedirect = true;
            Response.TrySkipIisCustomErrors = true;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return Content(JsonConvert.SerializeObject(payload, JsonSettings), "application/json", Encoding.UTF8);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            // Do not include account/birth data or exception details in the response.
            Package.Logger.Error("Mobile API failed: " + filterContext.Exception.GetType().Name);
            filterContext.Result = ApiError(500, "internal_error");
            filterContext.ExceptionHandled = true;
        }
    }
}
