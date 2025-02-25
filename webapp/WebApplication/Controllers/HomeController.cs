using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    public class HomeController : BaseNineStarKiController
    {
        public HomeController(INineStarKiPackage nineStarKiPackage)
            : base(nineStarKiPackage)
        {
        }

        [OutputCache(Duration = 2592000, VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
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
        [OutputCache(Duration = 2592000, VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult GptInfo()
        {
            return View();
        }
        
        [Route("privacy-policy")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        [Route("terms-of-service")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult TermsOfService()
        {
            return View();
        }

        [Route("how-to-remove-your-data")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
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
