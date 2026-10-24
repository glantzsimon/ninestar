using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/reports")]
    public class MobileReportsController : MobileApiControllerBase
    {
        private readonly IReportsService _reports;
        private readonly IAIService _ai;
        public MobileReportsController(INineStarKiPackage package, IReportsService reports, IAIService ai) : base(package)
        { _reports = reports; _ai = ai; }

        [HttpGet, Route("yearly")]
        public ActionResult Yearly()
        {
            var error = RequireUser(out var id, paid: true);
            if (error != null) return error;
            var report = _reports.GetYearlyReport(id);
            return ApiSuccess(new
            {
                Profile = MobilePersonalChartMapper.Map(report.NineStarKiModel, report.NineStarKiModel.TimeOfBirthKnown),
                Planner = MobileCalculationMapper.Planner(report.YearlyPlannerModel), report.PeriodDateString,
                // The PDF remains a file download through the existing protected action.
                PdfUrl = Url.Action("YearlyReportPdf", "Reports")
            });
        }

        [HttpGet, Route("yearly/prompt")]
        public ActionResult Prompt()
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            return ApiSuccess(new { Prompt = _ai.GetYearlyReportPrompt(_reports.GetYearlyReport(id)) });
        }
    }
}
