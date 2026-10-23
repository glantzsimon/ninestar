using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Enums;
using K9.DataAccessLayer.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/admin/users")]
    public class MobileAdminUsersController : MobileApiControllerBase
    {
        private readonly IPromotionService _promotions;
        public MobileAdminUsersController(INineStarKiPackage package, IPromotionService promotions) : base(package) { _promotions = promotions; }

        [HttpGet, Route("{userId:int}")]
        public ActionResult Account(int userId)
        {
            var error = RequireUser(out var actorId, admin: true);
            if (error != null) return error;
            var user = Package.UserService.Find(userId);
            if (user == null || user.IsDeleted) return ApiError(404, "user_not_found");
            var membership = Package.MembershipService.GetActiveUserMembership(userId);
            return ApiSuccess(new
            {
                user.Id, user.Username, user.FirstName, user.LastName, user.FullName,
                user.EmailAddress, user.PhoneNumber, user.AccountActivated,
                AllowMarketingEmails = !user.IsUnsubscribed,
                Calculation = ReadOwnCalculationRequest(userId),
                Membership = membership == null ? null : new
                {
                    membership.Id, membership.MembershipOptionId, membership.StartsOn, membership.EndsOn,
                    membership.IsActive, membership.IsAutoRenew, membership.IsDeactivated,
                    membership.ComplementaryPersonalChartReadingCount, membership.ComplementaryPredictionsReadingCount,
                    membership.ComplementaryCompatibilityReadingCount, membership.ComplementaryKarmicReadingCount
                }
            });
        }

        [HttpPost, Route("create")]
        public ActionResult Create(UserAccount.RegisterModel model)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!Request.IsSecureConnection && !Request.IsLocal) return ApiError(400, "https_required");
            if (!ValidInput(model)) return ApiError(400, "invalid_user_request");
            // The normal registration owner creates the membership credentials,
            // initial roles and confirmation flow. No generic User insert.
            var result = Package.AccountService.Register(model);
            return result.IsSuccess ? (ActionResult)ApiSuccess(new { Created = true }) : ApiError(422, "user_creation_failed");
        }

        [HttpPost, Route("update")]
        public ActionResult Update(int userId, MobileAccountUpdateRequest request)
        {
            var error = RequireUser(out var actorId, admin: true);
            if (error != null) return error;
            if (!ValidInput(request) || !ValidInput(request.Settings)) return ApiError(400, "invalid_user_request");
            var user = Package.UserService.Find(userId);
            if (user == null) return ApiError(404, "user_not_found");
            if (Package.UsersRepository.Find(e => e.Id != userId && e.EmailAddress == request.EmailAddress).Any())
                return ApiError(409, "email_unavailable");
            user.FirstName = request.FirstName; user.LastName = request.LastName;
            user.FullName = request.FirstName + " " + request.LastName;
            user.EmailAddress = request.EmailAddress; user.PhoneNumber = request.PhoneNumber;
            user.BirthDate = request.Settings.Person.DateOfBirth.Value.Date;
            user.Gender = request.Settings.Person.Gender; user.IsUnsubscribed = !request.AllowMarketingEmails;
            Package.UsersRepository.Update(user);
            var info = Package.UserService.GetOrCreateUserInfo(userId);
            info.TimeOfBirth = request.Settings.Person.TimeOfBirth;
            info.BirthTimeZoneId = request.Settings.Person.BirthTimeZoneId;
            info.CalculationMethod = request.Settings.CalculationMethod;
            info.CalculatorType = request.Settings.CalculatorType;
            info.HousesDisplay = request.Settings.HousesDisplay;
            info.InvertDailyAndHourlyKiForSouthernHemisphere = request.Settings.InvertDailyAndHourlyKiForSouthernHemisphere;
            Package.UserInfosRepository.Update(info);
            SavePreferences(userId, request.Settings);
            var contact = Package.ContactService.Find(user.EmailAddress);
            if (contact != null) { contact.IsUnsubscribed = user.IsUnsubscribed; Package.ContactsRepository.Update(contact); }
            return ApiSuccess(new { Updated = true });
        }

        [HttpPost, Route("delete")]
        public ActionResult Delete(int userId)
        {
            var error = RequireUser(out var actorId, admin: true);
            if (error != null) return error;
            if (userId == actorId) return ApiError(409, "use_own_account_deletion");
            if (Package.UserService.Find(userId) == null) return ApiError(404, "user_not_found");
            Package.UserService.DeleteUser(userId);
            return ApiSuccess(new { Deleted = true });
        }

        [HttpPost, Route("activate")]
        public ActionResult Activate(int userId)
        {
            var error = RequireUser(out var actorId, admin: true);
            if (error != null) return error;
            var result = Package.AccountService.ActivateAccount(userId);
            if (result.Result != EActivateAccountResult.Success && result.Result != EActivateAccountResult.AlreadyActivated)
                return ApiError(422, "activation_failed");
            return ApiSuccess(new { Activated = true });
        }

        [HttpPost, Route("assign-membership")]
        public ActionResult AssignMembership(int userId, int membershipOptionId)
        {
            var error = RequireUser(out var actorId, admin: true);
            if (error != null) return error;
            if (Package.UserService.Find(userId) == null || Package.MembershipService.GetMembershipOption(membershipOptionId) == null)
                return ApiError(404, "assignment_target_not_found");
            Package.MembershipService.AssignMembershipToUser(membershipOptionId, userId);
            return ApiSuccess(new { Assigned = true });
        }

        [HttpPost, Route("assign-consultation")]
        public ActionResult AssignConsultation(int userId, EConsultationDuration duration = EConsultationDuration.OneHour)
        {
            var error = RequireUser(out var actorId, admin: true);
            if (error != null) return error;
            if (!Enum.IsDefined(typeof(EConsultationDuration), duration)) return ApiError(400, "invalid_duration");
            if (Package.UserService.Find(userId) == null) return ApiError(404, "user_not_found");
            Package.MembershipService.CreateComplementaryUserConsultation(userId, duration);
            return ApiSuccess(new { Assigned = true });
        }

        [HttpPost, Route("assign-promotion")]
        public ActionResult AssignPromotion(int userId, string code)
        {
            var error = RequireUser(out var actorId, admin: true);
            if (error != null) return error;
            if (Package.UserService.Find(userId) == null || string.IsNullOrWhiteSpace(code) || code.Length > 100 || _promotions.Find(code) == null)
                return ApiError(400, "invalid_promotion_assignment");
            var free = Package.MembershipService.CreateMembershipFromPromoCode(userId, code);
            return ApiSuccess(new { Assigned = true, FreeMembershipCreated = free });
        }

        [HttpPost, Route("impersonate")]
        public ActionResult Impersonate(int userId)
        {
            if (!Request.IsAuthenticated || !Package.UserService.UserIsAdmin(Package.Authentication.CurrentUserId))
                return ApiError(403, "administrator_required");
            var user = Package.UserService.Find(userId);
            if (user == null) return ApiError(404, "user_not_found");
            Current.StartImpersonating(userId, user.Username);
            Package.UserService.InitUserPreferences(userId);
            return ApiSuccess(new { Impersonating = true, UserId = userId });
        }

        [HttpPost, Route("stop-impersonating")]
        public ActionResult StopImpersonating()
        {
            if (!Request.IsAuthenticated || !Package.UserService.UserIsAdmin(Package.Authentication.CurrentUserId))
                return ApiError(403, "administrator_required");
            Current.StopImpersonating();
            Package.UserService.InitUserPreferences(Package.Authentication.CurrentUserId);
            return ApiSuccess(new { Impersonating = false });
        }
    }
}
