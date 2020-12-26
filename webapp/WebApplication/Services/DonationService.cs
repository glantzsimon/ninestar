using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Services
{
    public class DonationService : IDonationService
    {
        private readonly IRepository<Donation> _donationRepository;
        private readonly ILogger _logger;
        private readonly IMailer _mailer;
        private readonly WebsiteConfiguration _config;
        private readonly StripeConfiguration _stripeConfig;
        private readonly UrlHelper _urlHelper;

        public DonationService(IRepository<Donation> donationRepository, ILogger logger, IMailer mailer, IOptions<WebsiteConfiguration> config, IOptions<StripeConfiguration> stripeConfig)
        {
            _donationRepository = donationRepository;
            _logger = logger;
            _mailer = mailer;
            _config = config.Value;
            _stripeConfig = stripeConfig.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public StripeModel GetDonationStripeModel()
        {
            return new StripeModel
            {
                DonationAmount = 10,
                LocalisedCurrencyThreeLetters = StripeModel.GetSystemCurrencyCode(),
                PublishableKey = _stripeConfig.PublishableKey,
                Description = Dictionary.DonationToNineStar
            };
        }

        public void CreateDonation(Donation donation)
        {
            try
            {
                _donationRepository.Create(donation);
                SendEmailToNineStar(donation);
                SendEmailToCustomer(donation);
            }
            catch (Exception ex)
            {
                _logger.Error($"DonationService => CreateDonation => {ex.Message}");
            }
        }
        
        public int GetFundsReceivedToDate()
        {
            return GetSuccessfulDonations().Sum(d => (int) d.Amount);
        }

        private List<Donation> GetSuccessfulDonations()
        {
            return _donationRepository.Find(d => d.Status == "succeeded").ToList();
        }

        private void SendEmailToNineStar(Donation donation)
        {
            var template = Dictionary.DonationReceivedEmail;
            var title = "We have received a donation!";
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                donation.Customer,
                donation.CustomerEmail,
                Amount = donation.DonationAmount,
                donation.Currency,
                LinkToSummary = _urlHelper.AbsoluteAction("Index", "Donations"),
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl)
            }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private void SendEmailToCustomer(Donation donation)
        {
            var template = Dictionary.DonationThankYouEmail;
            var title = Dictionary.ThankyouForDonationEmailTitle;
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                donation.CustomerName,
                donation.CustomerEmail,
                Amount = donation.DonationAmount,
                donation.Currency,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                DateTime.Now.Year
            }), donation.CustomerEmail, donation.Customer, _config.SupportEmailAddress, _config.CompanyName);
        }
    }
}