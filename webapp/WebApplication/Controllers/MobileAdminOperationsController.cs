using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/admin/operations")]
    public class MobileAdminOperationsController : MobileApiControllerBase
    {
        private readonly ILogService _logs;
        private readonly IMailChimpService _mailChimp;
        private readonly IRepository<Donation> _donations;
        public MobileAdminOperationsController(INineStarKiPackage package, ILogService logs, IMailChimpService mailChimp,
            IRepository<Donation> donations) : base(package)
        { _logs = logs; _mailChimp = mailChimp; _donations = donations; }

        [HttpPost, Route("import-donors")]
        public ActionResult ImportDonors(bool confirm = false)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!ModelState.IsValid || !confirm) return ApiError(400, "confirmation_required");
            return ApiSuccess(new { Imported = Package.ContactService.ImportContactsFromDonations(_donations.List()) });
        }

        [HttpGet, Route("logs")]
        public ActionResult Logs(int offset = 0, int limit = 50)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!ModelState.IsValid || offset < 0 || limit < 1 || limit > 100) return ApiError(400, "invalid_page");
            return ApiSuccess(_logs.GetLogItems().OrderByDescending(e => e.LoggedOn).Skip(offset).Take(limit)
                .Select(e => new { e.LoggedOn, e.ClassName, e.MethodName, e.ErrorMessage }).ToArray());
        }

        [HttpPost, Route("allocate-free-readings")]
        public ActionResult AllocateFreeReadings(bool confirm = false)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!ModelState.IsValid || !confirm) return ApiError(400, "confirmation_required");
            Package.MembershipService.AllocateFreeReadingsToAllAccounts();
            return ApiSuccess(new { Allocated = true });
        }

        [HttpPost, Route("sync-mailchimp")]
        public ActionResult SyncMailChimp(bool confirm = false)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!ModelState.IsValid || !confirm) return ApiError(400, "confirmation_required");
            _mailChimp.AddAllContacts();
            return ApiSuccess(new { Synced = true });
        }
    }
}
