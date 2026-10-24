using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/support")]
    public class MobileSupportController : MobileApiControllerBase
    {
        private readonly IRecaptchaService _captcha;
        private readonly IEmailTemplateService _templates;
        private readonly IDonationService _donations;
        public MobileSupportController(INineStarKiPackage package, IRecaptchaService captcha,
            IEmailTemplateService templates, IDonationService donations) : base(package)
        { _captcha = captcha; _templates = templates; _donations = donations; }

        [HttpPost, Route("contact")]
        public ActionResult Contact(MobileContactRequest request)
        {
            if (!ValidInput(request)) return ApiError(400, "invalid_contact_request");
            if (string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Body) ||
                request.Subject.Contains("\r") || request.Subject.Contains("\n")) return ApiError(400, "invalid_contact_request");
            if (!_captcha.Validate(request.CaptchaToken)) return ApiError(400, "invalid_captcha");
            var contact = Package.ContactService.GetOrCreateContact("", request.Name, request.EmailAddress);
            var body = _templates.ParseForContact(request.Subject, K9.Globalisation.Dictionary.SupportQueryReceivedEmail, contact, new
            {
                Customer = HttpUtility.HtmlEncode(request.Name), CustomerEmail = HttpUtility.HtmlEncode(request.EmailAddress),
                Subject = HttpUtility.HtmlEncode(request.Subject), Query = HtmlUtilityText(request.Body),
                UnformattedQuery = request.Body
            });
            Package.Mailer.SendEmail(request.Subject, body, Package.WebsiteConfiguration.SupportEmailAddress, Package.WebsiteConfiguration.CompanyName);
            var title = K9.Globalisation.Dictionary.EmailThankYouTitle;
            var reply = _templates.ParseForContact(title, K9.Globalisation.Dictionary.SupportQueryThankYouEmail, contact, new { contact.FirstName });
            Package.Mailer.SendEmail(title, reply, contact.EmailAddress, contact.Name);
            return ApiSuccess(new { Sent = true });
        }

        [HttpPost, Route("newsletter")]
        public ActionResult Newsletter(MobileContactRequest request)
        {
            if (!ValidInput(request)) return ApiError(400, "invalid_contact_request");
            if (!_captcha.Validate(request.CaptchaToken)) return ApiError(400, "invalid_captcha");
            // Existing contacts need a confirmation path; do not silently change
            // an unsubscribed person's preference from an anonymous request.
            if (Package.ContactService.Find(request.EmailAddress) == null)
                Package.ContactsRepository.Create(new Contact { FullName = request.Name, EmailAddress = request.EmailAddress, IsUnsubscribed = false });
            return ApiSuccess(new { Accepted = true });
        }

        [HttpGet, Route("donations/total")]
        public ActionResult DonationTotal() => ApiSuccess(new { Total = _donations.GetFundsReceivedToDate() });

        private static string HtmlUtilityText(string text) => HttpUtility.HtmlEncode(text).Replace("\r\n", "\n").Replace("\n", "<br />");
    }

}
