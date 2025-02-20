using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("i-ching")]
    public class IChingController : BaseNineStarKiController
    {
        private readonly IIChingService _ichingService;

        public IChingController(INineStarKiControllerPackage nineStarKiControllerPackage, IIChingService ichingService)
            : base(nineStarKiControllerPackage)
        {
            _ichingService = ichingService;
        }

        [Route("")]
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

