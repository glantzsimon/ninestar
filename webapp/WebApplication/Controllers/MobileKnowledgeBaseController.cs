using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/knowledge-base")]
    public class MobileKnowledgeBaseController : MobileApiControllerBase
    {
        private readonly INineStarKiService _profiles;
        public MobileKnowledgeBaseController(INineStarKiPackage package, INineStarKiService profiles) : base(package) { _profiles = profiles; }

        [HttpGet, Route("")]
        public ActionResult Index()
        {
            return ApiSuccess(MobileCalculationMapper.KnowledgeBase(_profiles.GetNineStarKiSummaryViewModel()));
        }
    }
}
