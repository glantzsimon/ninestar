using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Services
{
    public class DonationService : IDonationService
    {
        private readonly IRepository<Donation> _donationRepository;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly INineStarKiPackage _nineStarKiPackage;
        private readonly ILogger _logger;
        private readonly IMailer _mailer;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public INineStarKiPackage Package { get; }

        public DonationService(IRepository<Donation> donationRepository, INineStarKiPackage nineStarKiPackage, IEmailTemplateService emailTemplateService)
        {
            _donationRepository = donationRepository;
            _emailTemplateService = emailTemplateService;
            Package = nineStarKiPackage;
        }

        public void CreateDonation(Donation donation, Contact contact)
        {
            try
            {
                _donationRepository.Create(donation);
                SendEmailToNineStar(donation, contact);
                SendEmailToCustomer(donation, contact);
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

        private void SendEmailToNineStar(Donation donation, Contact contact)
        {
            var subject = "We have received a donation";
            var body = _emailTemplateService.ParseForContact(
                subject,
                Dictionary.DonationReceivedEmail,
                contact,
                new
                {
                    Customer = contact.Name,
                    CustomerEmail = contact.EmailAddress,
                    Amount = donation.DonationAmount,
                    donation.Currency,
                    LinkToSummary = _urlHelper.AbsoluteAction("Index", "Donations"),
                });

            try
            {
                Package.Mailer.SendEmail(
                    subject,
                    body,
                    Package.WebsiteConfiguration.SupportEmailAddress,
                    Package.WebsiteConfiguration.CompanyName);
            }
            catch (Exception ex)
            {
                Package.Logger.Error(ex.GetFullErrorMessage());
            }
        }

        private void SendEmailToCustomer(Donation donation, Contact contact)
        {
            var subject = Dictionary.ThankyouForDonationEmailTitle;
            var body = _emailTemplateService.ParseForContact(
                subject,
                Dictionary.DonationThankYouEmail,
                contact,
                new
                {
                    Customer = contact.Name,
                    CustomerName = contact.FirstName,
                    donation.CustomerEmail,
                    Amount = donation.DonationAmount,
                    donation.Currency,
                });

            try
            {
                Package.Mailer.SendEmail(
                    subject,
                    body,
                    Package.WebsiteConfiguration.SupportEmailAddress,
                    Package.WebsiteConfiguration.CompanyName);
            }
            catch (Exception ex)
            {
                Package.Logger.Error(ex.GetFullErrorMessage());
            }
        }
    }
}