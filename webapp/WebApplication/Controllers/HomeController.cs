using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using NLog;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class HomeController : BaseNineStarKiController
    {
        private readonly IAuthentication _authentication;
        private readonly INineStarKiService _nineStarKiService;

        public HomeController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _authentication = authentication;
            _nineStarKiService = nineStarKiService;
        }

        public ActionResult Index()
        {
            if (_authentication.IsAuthenticated)
            {
                return RedirectToAction("MyProfile", "NineStarKi");
            }
            return View();
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
