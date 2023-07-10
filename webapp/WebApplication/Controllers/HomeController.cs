using System;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using NLog;
using System.Web.Mvc;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;

namespace K9.WebApplication.Controllers
{
    public class HomeController : BaseNineStarKiController
    {
        private readonly IAuthentication _authentication;
        private readonly INineStarKiService _nineStarKiService;
        private readonly IBiorhythmsService _biorhythmsService;

        public HomeController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService, IBiorhythmsService biorhythmsService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _authentication = authentication;
            _nineStarKiService = nineStarKiService;
            _biorhythmsService = biorhythmsService;
        }

        public ActionResult Index()
        {
            if (_authentication.IsAuthenticated)
            {
                return RedirectToAction("MyProfile", "NineStarKi");
            }

            var dateOfBirth = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth,
                Gender = Methods.GetRandomGender()
            };
            
            var nineStarKiModel = new NineStarKiModel(personModel);
            nineStarKiModel.Biorhythms = _biorhythmsService.Calculate(personModel, DateTime.Today);

            return View(nineStarKiModel);
        }

        [Route("about")]
        public ActionResult About()
        {
            return View(_nineStarKiService.GetNineStarKiSummaryViewModel());
        }

        [Route("privacy-policy")]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}
