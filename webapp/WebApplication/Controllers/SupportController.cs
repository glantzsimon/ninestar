using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.ViewModels;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Web.Mvc;
using K9.Base.DataAccessLayer.Models;

namespace K9.WebApplication.Controllers
{
    public class SupportController : BaseNineStarKiController
    {
        private readonly ILogger _logger;
        private readonly IMailer _mailer;
        private readonly IDonationService _donationService;
        private readonly IContactService _contactService;
        private readonly IRecaptchaService _recaptchaService;
        private readonly RecaptchaConfiguration _recaptchaConfig;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public SupportController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IMailer mailer, IOptions<WebsiteConfiguration> config, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IOptions<StripeConfiguration> stripeConfig, IDonationService donationService, IMembershipService membershipService, IContactService contactService, IOptions<RecaptchaConfiguration> recaptchaConfig, IRecaptchaService recaptchaService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
        {
            _logger = logger;
            _mailer = mailer;
            _donationService = donationService;
            _contactService = contactService;
            _recaptchaService = recaptchaService;
            _recaptchaConfig = recaptchaConfig.Value;
            _config = config.Value;
            _urlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.RecaptchaSiteKey = _recaptchaConfig.RecaptchaSiteKey;
            return View("ContactUs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(ContactUsViewModel model)
        {
            if (!Helpers.Environment.IsDebug)
            {
                var encodedResponse = Request.Form[RecaptchaResult.ResponseFormVariable];
                var isCaptchaValid = _recaptchaService.Validate(encodedResponse);

                if (!isCaptchaValid)
                {
                    ModelState.AddModelError("", Dictionary.InvalidRecaptcha);
                    return View("ContactUs", model);
                }
            }

            var body = TemplateProcessor.PopulateTemplate(Dictionary.SupportQueryReceived, new
            {
                Customer = model.Name,
                CustomerEmail = model.EmailAddress,
                model.Subject,
                Query = model.Body
            });
                
            try
            {
                _mailer.SendEmail(
                    model.Subject,
                    body,
                    _config.SupportEmailAddress,
                    _config.CompanyName,
                    _config.SupportEmailAddress,
                    _config.CompanyName);

                var contact = _contactService.GetOrCreateContact("", model.Name, model.EmailAddress);
                SendEmailToCustomer(contact);

                return RedirectToAction("ContactUsSuccess");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetFullErrorMessage());
                return View("FriendlyError");
            }
        }

        public ActionResult ContactUsSuccess()
        {
            return View();
        }

        [Route("donate")]
        public ActionResult DonateStart()
        {
            return View(new Donation
            {
                DonationAmount = 10,
                DonationDescription = Dictionary.DonationToNineStar
            });
        }

        [Route("donate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Donate(Donation donation)
        {
            return View(donation);
        }

        [HttpPost]
        public ActionResult ProcessDonation(PurchaseModel purchaseModel)
        {
            try
            {
                var contact = _contactService.Find(purchaseModel.ContactId);

                _donationService.CreateDonation(new Donation
                {
                    Currency = purchaseModel.Currency,
                    Customer = purchaseModel.CustomerName,
                    CustomerEmail = purchaseModel.CustomerEmailAddress,
                    DonationDescription = purchaseModel.Description,
                    DonatedOn = DateTime.Now,
                    DonationAmount = purchaseModel.Amount
                }, contact);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.Error($"SupportController => ProcessDonation => Error: {ex.GetFullErrorMessage()}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Route("donate/success")]
        public ActionResult DonationSuccess(string sessionId)
        {
            return View();
        }

        [Route("donate/cancel/success")]
        public ActionResult DonationCancelSuccess()
        {
            return View();
        }

        public override string GetObjectName()
        {
            throw new NotImplementedException();
        }

        private void SendEmailToCustomer(Contact contact)
        {
            var template = Dictionary.SupportQuery;
            var title = Dictionary.EmailThankYouTitle;
            if (contact != null)
            {
                _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
                {
                    Title = title,
                    contact.FirstName,
                    PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                    TermsOfServiceLink = _urlHelper.AbsoluteAction("TermsOfService", "Home"),
                    UnsubscribeLink = _urlHelper.AbsoluteAction("UnsubscribeContact", "Account", new { externalId = contact.Name }),
                    DateTime.Now.Year
                }), contact.EmailAddress, contact.FirstName, _config.SupportEmailAddress,
                    _config.CompanyName);
            }
        }
    }
}
