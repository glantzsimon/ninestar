using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/compatibility")]
    public class MobileCompatibilityController : MobileApiControllerBase
    {
        private readonly INineStarKiService _profiles;
        public MobileCompatibilityController(INineStarKiPackage package, INineStarKiService profiles) : base(package) { _profiles = profiles; }

        [HttpGet, Route("help")]
        public ActionResult Help() => ApiSuccess(new { Help = MobileHelpMapper.Compatibility() });

        [HttpPost, Route("calculate")]
        public ActionResult Calculate(MobileCompatibilityRequest request)
        {
            var error = RequireUser(out var userId, paid: true);
            if (error != null) return error;
            if (!ValidInput(request)) return ApiError(400, "invalid_compatibility_request");
            // CalculateCompatibility forwards directly to the profile service,
            // which expects the birth time combined exactly once.
            var first = request.FirstPerson.ToPerson();
            var second = request.SecondPerson.ToPerson();
            var hide = request.HideSexualChemistry || !first.IsSixteenOrOver() || !second.IsSixteenOrOver();
            var result = _profiles.CalculateCompatibility(first, second, hide, request.CalculationMethod, request.CalculatorType);
            SaveLastRequest("compatibility", request);
            return ApiSuccess(MobileCalculationMapper.Compatibility(result, hide, Url.Action("Index", "KnowledgeBase", null, Request.Url.Scheme)));
        }

        [HttpGet, Route("last")]
        public ActionResult Last()
        {
            var error = RequireUser(out var id, paid: true);
            if (error != null) return error;
            var request = ReadLastRequest<MobileCompatibilityRequest>("compatibility");
            return request == null ? (ActionResult)ApiError(404, "reading_not_found") : Calculate(request);
        }
    }
}
