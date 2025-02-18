using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class HomeController : BaseNineStarKiController
    {
        private readonly IAuthentication _authentication;
        private readonly INineStarKiService _nineStarKiService;
        private readonly IBiorhythmsService _biorhythmsService;

        public HomeController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService, IBiorhythmsService biorhythmsService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
        {
            _authentication = authentication;
            _nineStarKiService = nineStarKiService;
            _biorhythmsService = biorhythmsService;
        }

        public ActionResult Index()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth,
                Gender = Methods.GetRandomGender()
            };
            return View(new NineStarKiModel(personModel));
        }

        [Route("ai-gpt-astrologer")]
        public ActionResult GptInfo()
        {
            return View();
        }
        
        [Route("privacy-policy")]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        [Route("terms-of-service")]
        public ActionResult TermsOfService()
        {
            return View();
        }

        [Route("how-to-remove-your-data")]
        public ActionResult HowToRemoveYourData()
        {
            return View();
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}
