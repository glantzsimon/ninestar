using K9.Base.WebApplication.Helpers;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class RelationshipsController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;

        public RelationshipsController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _nineStarKiService = nineStarKiService;
            SetBetaWarningSessionVariable();
        }

        [Route("relationships")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("relationships/calculate-compatibility")]
        public ActionResult Compatibility()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (24), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth
            };
            return View(new NineStarKiModel(personModel));
        }

        [Route("relationships/calculate-compatibility")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Compatibility(NineStarKiModel model)
        {
            if (model.PersonModel != null)
            {
                model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel);
            }
            return View("Index", model);
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
