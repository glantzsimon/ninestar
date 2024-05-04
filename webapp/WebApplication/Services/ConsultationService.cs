using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IRepository<Consultation> _consultationRepository;
        private readonly ILogger _logger;
        private readonly Helpers.IMailer _mailer;
        private readonly IAuthentication _authentication;
        private readonly IRepository<UserConsultation> _userConsultationRepository;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public ConsultationService(IRepository<Consultation> consultationRepository, ILogger logger, Helpers.IMailer mailer, IOptions<WebsiteConfiguration> config, IAuthentication authentication, IRepository<UserConsultation> userConsultationRepository)
        {
            _consultationRepository = consultationRepository;
            _logger = logger;
            _mailer = mailer;
            _authentication = authentication;
            _userConsultationRepository = userConsultationRepository;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }
        
        public void CreateConsultation(Consultation consultation, Contact contact)
        {
            try
            {
                _consultationRepository.Create(consultation);

                if (_authentication.IsAuthenticated)
                {
                    _userConsultationRepository.Create(new UserConsultation
                    {
                        UserId = _authentication.CurrentUserId,
                        ConsultationId = consultation.Id
                    });
                }

                SendEmailToNineStar(consultation, contact);
                SendEmailToCustomer(consultation, contact);
            }
            catch (Exception ex)    
            {
                _logger.Error($"ConsultationService => CreateConsultation => {ex.GetFullErrorMessage()}");
            }
        }

        private void SendEmailToNineStar(Consultation consultation, Contact contact)
        {
            var template = Dictionary.ConsultationBookedEmail;
            var title = "We have received a consultation booking!";
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                ContactName = contact.FullName,
                CustomerEmail = contact.EmailAddress,
                contact.PhoneNumber,
                Duration = consultation.DurationDescription,
                Price = consultation.FormattedPrice,
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl)
            }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private void SendEmailToCustomer(Consultation consultation, Contact contact)
        {
            var template = Dictionary.ConsultationBookedThankYouEmail;
            var title = Dictionary.ThankyouForBookingConsultationEmailTitle;
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                contact.FirstName,
                Duration = consultation.DurationDescription,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                UnsubscribeLink = _urlHelper.AbsoluteAction("Unsubscribe", "Account", new { code = contact.Name }),
                DateTime.Now.Year
            }), contact.EmailAddress, contact.FullName, _config.SupportEmailAddress, _config.CompanyName);
        }
    }
}