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

        [Route("free-hexagram-generator")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult _GenerateButtonForm()
        {
            return PartialView();
        }

        [Route("free-hexagram-generator")]
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

