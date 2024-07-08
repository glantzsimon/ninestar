using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
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
        private readonly UrlHelper _urlHelper;

        public DonationService(IRepository<Donation> donationRepository, ILogger logger, IMailer mailer, IOptions<WebsiteConfiguration> config)
        {
            _donationRepository = donationRepository;
            _logger = logger;
            _mailer = mailer;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }
        
        public void CreateDonation(Donation donation, Contact contact)
        {
            try
            {
                _donationRepository.Create(donation);
                SendEmailToNineStar(donation);
                if (contact != null && !contact.IsUnsubscribed)
                {
                    SendEmailToCustomer(donation, contact);
                }
            }
            catch (Exception ex)    
            {
                _logger.Error($"DonationService => CreateDonation => {ex.GetFullErrorMessage()}");
            }
        }

        public int GetFundsReceivedToDate()
        {
            return GetSuccessfulDonations().Sum(d => (int)d.Amount);
        }

        private List<Donation> GetSuccessfulDonations()
        {
            return _donationRepository.List();
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

        private void SendEmailToCustomer(Donation donation, Contact contact)
        {
            var template = Dictionary.DonationThankYouEmail;
            var title = Dictionary.ThankyouForDonationEmailTitle;
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                CustomerName = contact.FirstName,
                donation.CustomerEmail,
                Amount = donation.DonationAmount,
                donation.Currency,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                UnsubscribeLink = _urlHelper.AbsoluteAction("Unsubscribe", "Account", new { code = contact.Name }),
                DateTime.Now.Year
            }), donation.CustomerEmail, donation.Customer, _config.SupportEmailAddress, _config.CompanyName);
        }
    }
}