using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("i-ching")]
    public class IChingController : BaseNineStarKiController
    {
        private readonly IIChingService _ichingService;

        public IChingController(INineStarKiPackage nineStarKiPackage, IIChingService ichingService)
            : base(nineStarKiPackage)
        {
            _ichingService = ichingService;
        }

        [Route("")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Toss()
        {
            var model = new IChingViewModel(_ichingService.GenerateHexagram());
            return View("Index", model);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

