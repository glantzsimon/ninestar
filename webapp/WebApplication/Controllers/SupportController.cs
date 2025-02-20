using K9.Base.WebApplication.ViewModels;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers.Html;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class SupportController : BaseNineStarKiController
    {
        private readonly IRecaptchaService _recaptchaService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly RecaptchaConfiguration _recaptchaConfig;
        private readonly IDonationService _donationService;

        public SupportController(INineStarKiControllerPackage nineStarKiControllerPackage, IDonationService donationService, IOptions<RecaptchaConfiguration> recaptchaConfig, IRecaptchaService recaptchaService, IEmailTemplateService emailTemplateService)
            : base(nineStarKiControllerPackage)
        {
            _donationService = donationService;
            _recaptchaService = recaptchaService;
            _emailTemplateService = emailTemplateService;
            _recaptchaConfig = recaptchaConfig.Value;
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

            var contact = Package.ContactService.GetOrCreateContact("", model.Name, model.EmailAddress);
            var body = _emailTemplateService.ParseForContact(
                model.Subject,
                Dictionary.SupportQueryReceived1,
                contact,
                new
                {
                    Customer = model.Name,
                    CustomerEmail = model.EmailAddress,
                    model.Subject,
                    Query = HtmlFormatter.ConvertNewlinesToParagraphs(model.Body),
                    UnformattedQuery = $"%0D%0A %0D%0A {model.Body}"
                });

            try
            {
                Package.Mailer.SendEmail(
                    model.Subject,
                    body,
                    Package.WebsiteConfiguration.SupportEmailAddress,
                    Package.WebsiteConfiguration.CompanyName);

                SendEmailToCustomer(contact);

                return RedirectToAction("ContactUsSuccess");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.GetFullErrorMessage());
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
                var contact = Package.ContactService.Find(purchaseModel.ContactId);

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
                Logger.Error($"SupportController => ProcessDonation => Error: {ex.GetFullErrorMessage()}");
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
            var template = Dictionary.SupportQueryThankYou;
            var title = Dictionary.EmailThankYouTitle;

            if (contact != null)
            {
                var body = _emailTemplateService.ParseForContact(
                    title,
                    Dictionary.SupportQueryThankYou,
                    contact,
                    new
                    {
                        contact.FirstName
                    });

                try
                {
                    Package.Mailer.SendEmail(
                        title,
                        body,
                        Package.WebsiteConfiguration.SupportEmailAddress,
                        Package.WebsiteConfiguration.CompanyName);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.GetFullErrorMessage());
                }
            }
        }
    }
}
