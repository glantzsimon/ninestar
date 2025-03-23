using K9.WebApplication.Packages;
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

        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("ai-chatgpt-astrologer")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult GptInfo()
        {
            return View();
        }
        
        [Route("privacy-policy")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        [Route("terms-of-service")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult TermsOfService()
        {
            return View();
        }

        [Route("faq")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult FAQ()
        {
            return View();
        }

        [Route("how-to-remove-your-data")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult HowToRemoveYourData()
        {
            return View();
        }

        [Route("calculation-methods")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult CalculationMethods()
        {
            return View();
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}
