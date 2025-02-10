using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Constants;
using K9.Base.WebApplication.Controllers;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SessionHelper = K9.Base.WebApplication.Helpers.SessionHelper;

namespace K9.WebApplication.Controllers
{
    public class BaseNineStarKiController : BaseController
    {
        private readonly IMembershipService _membershipService;
        private readonly IRepository<Role> _rolesRepository;
        private readonly IRepository<UserRole> _userRolesRepository;

        public BaseNineStarKiController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles,
            IAuthentication authentication, IFileSourceHelper fileSourceHelper, IMembershipService membershipService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _membershipService = membershipService;
            _rolesRepository = rolesRepository;
            _userRolesRepository = userRolesRepository;
            SetBetaWarningSessionVariable();
            SetSessionRoles(Current.UserId);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        
            ViewBag.DeviceType = GetDeviceType();
        }

        public ActionResult SetLanguage(string languageCode, string cultureCode)
        {
            Session[SessionConstants.LanguageCode] = languageCode;
            Session[SessionConstants.CultureCode] = cultureCode;
            return Redirect(Request.UrlReferrer?.ToString());
        }

        public UserMembership GetActiveUserMembership()
        {
            if (Authentication.IsAuthenticated)
            {
                return _membershipService.GetActiveUserMembership(Authentication.CurrentUserId);
            }

            return null;
        }

        public ActionResult RedirectToLastSaved()
        {
            // Redirect to previous profile or compatibility reading if set
            var lastProfile = Helpers.SessionHelper.GetLastProfile(true, false);
            var lastCompatibility = Helpers.SessionHelper.GetLastCompatibility(true, false);
            var lastPrediction = Helpers.SessionHelper.GetLastPrediction(true, false);
            var lastBiorhythm = Helpers.SessionHelper.GetLastBiorhythm(true, false);
            var lastKnowledgeBase = Helpers.SessionHelper.GetLastKnowledgeBase(true, false);

            var lastItems = new List<RetrieveLastModel>
            {
                lastProfile,
                lastCompatibility,
                lastPrediction,
                lastBiorhythm,
                lastKnowledgeBase
            };

            var lastStoredItem = lastItems.OrderByDescending(e => e.StoredOn).FirstOrDefault();

            if (lastStoredItem != null && lastStoredItem.StoredOn.Value.Date == DateTime.Today)
            {
                switch (lastStoredItem.Section)
                {
                    case ESection.Profile:
                        return RedirectToAction("RetrieveLastProfile", "PersonalChart");

                    case ESection.Compatibility:
                        return RedirectToAction("RetrieveLastCompatibility", "PersonalChart");

                    case ESection.Predictions:
                        return RedirectToAction("RetrieveLastPrediction", "Predictions");

                    case ESection.Biorhythms:
                        return RedirectToAction("RetrieveLastBiorhythms", "Biorhythms");

                    case ESection.KnowledgeBase:
                        return RedirectToAction("RetrieveLastKnowledgeBaseSection", "KnowledgeBase");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public EDeviceType GetDeviceType()
        {
            return new BrowserInfo(Request.Headers["User-Agent"]).DeviceType;
        }

        public void SetSessionRoles(int userId)
        {
            Helpers.SessionHelper.SetCurrentUserRoles(_rolesRepository, _userRolesRepository, userId);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }

        private static void SetBetaWarningSessionVariable()
        {
            var numberOfDisplays = Helpers.SessionHelper.GetIntValue(Constants.SessionConstants.BetaWarningDisplay);
            if (numberOfDisplays < 1)
            {
                numberOfDisplays++;
                SessionHelper.SetValue(Constants.SessionConstants.BetaWarningDisplay, numberOfDisplays);
            }
            else
            {
                SessionHelper.SetValue(Constants.SessionConstants.BetaWarningHide, true);
            }
        }
    }
}
