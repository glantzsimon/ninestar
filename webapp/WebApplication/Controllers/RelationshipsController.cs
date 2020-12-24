using K9.Base.WebApplication.Helpers;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

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

        [Route("relationships/compatibility")]
        public ActionResult Compatibility()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (24), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth
            };
            return View(new CompatibilityModel(new NineStarKiModel(personModel), new NineStarKiModel(personModel)));
        }

        [Route("relationships/compatibility")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Compatibility(CompatibilityModel model)
        {
            if (model.NineStarKiModel1?.PersonModel != null && model.NineStarKiModel2?.PersonModel != null)
            {
                model = _nineStarKiService.CalculateCompatibility(model.NineStarKiModel1.PersonModel, model.NineStarKiModel2.PersonModel);
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
