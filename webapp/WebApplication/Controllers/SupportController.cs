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

namespace K9.WebApplication.Controllers
{
    public class SupportController : BaseNineStarKiController
    {
        private readonly ILogger _logger;
        private readonly IMailer _mailer;
        private readonly IDonationService _donationService;
        private readonly IContactService _contactService;
        private readonly StripeConfiguration _stripeConfig;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public SupportController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IMailer mailer, IOptions<WebsiteConfiguration> config, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IOptions<StripeConfiguration> stripeConfig, IDonationService donationService, IMembershipService membershipService, IContactService contactService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _logger = logger;
            _mailer = mailer;
            _donationService = donationService;
            _contactService = contactService;
            _stripeConfig = stripeConfig.Value;
            _config = config.Value;
            _urlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View("ContactUs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(ContactUsViewModel model)
        {
            try
            {
                _mailer.SendEmail(
                    model.Subject,
                    model.Body,
                    _config.SupportEmailAddress,
                    _config.CompanyName,
                    model.EmailAddress,
                    model.Name);

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
            ViewBag.PublishableKey = _stripeConfig.PublishableKey;
            return View(donation);
        }

        [Route("donate/success")]
        public ActionResult DonationSuccess(string sessionId)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProcessDonation(PaymentModel paymentModel)
        {
            try
            {
                var contact = _contactService.Find(paymentModel.ContactId);

                _donationService.CreateDonation(new Donation
                {
                    Currency = paymentModel.Currency,
                    Customer = paymentModel.CustomerName,
                    CustomerEmail = paymentModel.CustomerEmailAddress,
                    DonationDescription = paymentModel.Description,
                    DonatedOn = DateTime.Now,
                    DonationAmount = paymentModel.Amount
                }, contact);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.Error($"SupportController => ProcessDonation => Error: {ex.GetFullErrorMessage()}");
                return Json(new { success = false, error = ex.Message });
            }
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
            if (contact != null && !contact.IsUnsubscribed)
            {
                _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
                {
                    Title = title,
                    contact.FirstName,
                    PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                    UnsubscribeLink = _urlHelper.AbsoluteAction("Unsubscribe", "Contacts", new { code = contact.Name }),
                    DateTime.Now.Year
                }), contact.EmailAddress, contact.FirstName, _config.SupportEmailAddress,
                    _config.CompanyName);
            }
        }
    }
}
