using K9.WebApplication.Attributes;
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
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("free-hexagram-generator")]
        [HttpPost]
        [CalculatorTrial]
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

