using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/biorhythms")]
    public class MobileBiorhythmsController : MobileApiControllerBase
    {
        private readonly IBiorhythmsService _biorhythms;
        private readonly INineStarKiService _profiles;
        public MobileBiorhythmsController(INineStarKiPackage package, IBiorhythmsService biorhythms, INineStarKiService profiles) : base(package)
        { _biorhythms = biorhythms; _profiles = profiles; }

        [HttpPost, Route("calculate")]
        public ActionResult Calculate(MobileCalculationRequest request)
        {
            var error = RequireUser(out var userId, paid: true);
            if (error != null) return error;
            if (!ValidInput(request)) return ApiError(400, "invalid_calculation_request");
            var result = MobileCalculationMapper.Biorhythms(_biorhythms.Calculate(CalculateProfile(_profiles, request), request.GetSelectedDate(GetUtcNow())));
            SaveLastRequest("biorhythms", request);
            return ApiSuccess(result);
        }

        [HttpGet, Route("last")]
        public ActionResult Last()
        {
            var error = RequireUser(out var id, paid: true);
            if (error != null) return error;
            var request = ReadLastRequest<MobileCalculationRequest>("biorhythms");
            return request == null ? (ActionResult)ApiError(404, "reading_not_found") : Calculate(request);
        }
    }
}
