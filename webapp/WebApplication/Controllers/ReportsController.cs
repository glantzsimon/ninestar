using K9.Base.WebApplication.Filters;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("reports")]
    public partial class ReportsController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;
        private readonly IAstronomyService _astronomyService;
        private readonly IAstrologyService _astrologyService;
        private readonly IAIService _aiService;
        private readonly IPdfService _pdfService;
        private readonly IReportsService _reportsService;

        public ReportsController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService, IAstronomyService astronomyService, IAstrologyService astrologyService, IAIService aiService, IPdfService pdfService, IReportsService reportsService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
            _astronomyService = astronomyService;
            _astrologyService = astrologyService;
            _aiService = aiService;
            _pdfService = pdfService;
            _reportsService = reportsService;
        }

        [Route("yearly-report")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public async Task<ActionResult> YearlyReport(Guid userId)
        {
            var report = await _reportsService.GetYearlyReport(userId);
            ViewBag.Report = report;
            return View();
        }

        [Route("yearly-report/pdf")]
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult YearlyReportPdf()
        {
            var account = My.AccountService.GetAccount(Current.UserId);
            var pdfBytes = _pdfService.UrlToPdf(My.UrlHelper.AbsoluteAction("YearlyReport", "Reports", new { userId = account.UserInfo.UniqueIdentifier }));
            return File(pdfBytes, "application/pdf", $"9 Star Ki Yearly Report.pdf");
        }

        [RequirePermissions(Role = RoleNames.Administrators)]
        [Route("yearly-report/prompt")]
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public async Task<ActionResult> YearlyReportPrompt()
        {
            var report = await _reportsService.GetYearlyReport(Current.UserId);
            return Content(report, "text/plain");
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

