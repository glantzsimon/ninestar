using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/iching")]
    public class MobileIChingController : MobileApiControllerBase
    {
        private readonly IIChingService _iching;
        public MobileIChingController(INineStarKiPackage package, IIChingService iching) : base(package) { _iching = iching; }

        [HttpGet, Route("help")]
        public ActionResult Help() => ApiSuccess(new { Help = MobileHelpMapper.IChing() });

        [HttpPost, Route("cast")]
        public ActionResult Cast()
        {
            var reading = new IChingViewModel(_iching.GenerateHexagram());
            return ApiSuccess(new
            {
                Help = MobileHelpMapper.IChing(),
                Primary = MobileCalculationMapper.Hexagram(reading.Hexagram),
                Transformed = MobileCalculationMapper.Hexagram(reading.TransformedHexagram)
            });
        }
    }
}
