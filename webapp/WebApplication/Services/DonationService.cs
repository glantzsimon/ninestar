using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class DonationService : BaseService, IDonationService
    {
        private readonly IRepository<Donation> _donationRepository;
        private readonly IEmailTemplateService _emailTemplateService;
        
        public DonationService(IRepository<Donation> donationRepository, INineStarKiBasePackage package, IEmailTemplateService emailTemplateService)
            : base(package)
        {
            _donationRepository = donationRepository;
            _emailTemplateService = emailTemplateService;
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
                My.Logger.Error($"DonationService => CreateDonation => {ex.GetFullErrorMessage()}");
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
                    Customer = contact.FullName,
                    CustomerEmail = contact.EmailAddress,
                    Amount = donation.DonationAmount.ToFormattedString(),
                    donation.Currency,
                    LinkToSummary = My.UrlHelper.AbsoluteAction("Index", "Donations"),
                });

            try
            {
                My.Mailer.SendEmail(
                    subject,
                    body,
                    My.WebsiteConfiguration.SupportEmailAddress,
                    My.WebsiteConfiguration.CompanyName);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
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
                    Amount = donation.DonationAmount.ToFormattedString(),
                    donation.Currency,
                });

            try
            {
                My.Mailer.SendEmail(
                    subject,
                    body,
                    contact.EmailAddress,
                    contact.FullName);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
            }
        }
    }
}