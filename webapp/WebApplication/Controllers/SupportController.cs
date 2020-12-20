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
using K9.WebApplication.Services.Stripe;
using NLog;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class SupportController : BaseNineStarKiController
    {
        private readonly ILogger _logger;
        private readonly IMailer _mailer;
        private readonly IStripeService _stripeService;
        private readonly IDonationService _donationService;
        private readonly StripeConfiguration _stripeConfig;
        private readonly WebsiteConfiguration _config;

        public SupportController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IMailer mailer, IOptions<WebsiteConfiguration> config, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IStripeService stripeService, IOptions<StripeConfiguration> stripeConfig, IDonationService donationService, IMembershipService membershipService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _logger = logger;
            _mailer = mailer;
            _stripeService = stripeService;
            _donationService = donationService;
            _stripeConfig = stripeConfig.Value;
            _config = config.Value;
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

        [Route("donate/start")]
        public ActionResult DonateStart()
        {
            return View(new StripeModel
            {
                DonationAmount = 10
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Donate(StripeModel model)
        {
            model.PublishableKey = _stripeConfig.PublishableKey;
            return View(model);
        }

        [Route("donate/success")]
        public ActionResult DonationSuccess()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("donate/processing")]
        public ActionResult DonateProcess(StripeModel model)
        {
            try
            {
                model.Description = Dictionary.DonationToNineStar;
                _stripeService.Charge(model);
                _donationService.CreateDonation(new Donation
                {
                    Currency = model.LocalisedCurrencyThreeLetters,
                    Customer = model.StripeBillingName,
                    CustomerEmail = model.StripeEmail,
                    DonationDescription = model.Description,
                    DonatedOn = DateTime.Now,
                    DonationAmount = model.AmountToDonate
                });
                return RedirectToAction("DonationSuccess");
            }
            catch (Exception ex)
            {
                _logger.Error($"SupportController => Donate => Donation failed: {ex.Message}");
                ModelState.AddModelError("", ex.Message);
            }

            return View("Donate", model);
        }

        public override string GetObjectName()
        {
            throw new NotImplementedException();
        }
    }
}
