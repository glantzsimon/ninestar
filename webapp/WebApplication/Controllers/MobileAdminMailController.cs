using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/admin/mail")]
    public class MobileAdminMailController : MobileApiControllerBase
    {
        private readonly IMailerService _mailer;
        private readonly IMailingListService _lists;
        private readonly IEmailTemplateService _templates;

        public MobileAdminMailController(INineStarKiPackage package, IMailerService mailer,
            IMailingListService lists, IEmailTemplateService templates) : base(package)
        { _mailer = mailer; _lists = lists; _templates = templates; }

        [HttpGet, Route("lists")]
        public ActionResult Lists()
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            // Metadata only: do not hydrate every automatic list's recipients.
            return ApiSuccess(_lists.GetMailingListListItems().Select(e => new { e.Id, e.Name, e.Value }).ToArray());
        }

        [HttpPost, Route("test")]
        public ActionResult Test(int templateId)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (_templates.Find(templateId) == null) return ApiError(404, "template_not_found");
            _mailer.TestEmailTemplate(templateId);
            return ApiSuccess(new { Sent = true });
        }

        [HttpPost, Route("send-user")]
        public ActionResult SendUser(int templateId, int userId)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (_templates.Find(templateId) == null) return ApiError(404, "template_not_found");
            var user = Package.UserService.Find(userId);
            if (user == null) return ApiError(404, "user_not_found");
            _mailer.SendEmailTemplateToUser(templateId, user);
            return ApiSuccess(new { Sent = true });
        }

        [HttpPost, Route("send-list")]
        public ActionResult SendList(int templateId, int mailingListId)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (_templates.Find(templateId) == null) return ApiError(404, "template_not_found");
            var list = _lists.Find(mailingListId, true);
            if (list == null) return ApiError(404, "mailing_list_not_found");
            return ApiSuccess(_mailer.SendEmailTemplateToUsers(templateId, list.Users).Select(e => new
            {
                e.UserId, e.RecipientName, e.EmailAddress, e.IsSuccess,
                ErrorCode = e.IsSuccess ? null : "mail_send_failed"
            }).ToArray());
        }
    }
}
