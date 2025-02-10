using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Constants;
using K9.WebApplication.Helpers;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using K9.DataAccessLayer.Enums;

namespace K9.WebApplication.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IRepository<Consultation> _consultationRepository;
        private readonly ILogger _logger;
        private readonly IMailer _mailer;
        private readonly IAuthentication _authentication;
        private readonly IRepository<UserConsultation> _userConsultationRepository;
        private readonly IRepository<Slot> _slotRepository;
        private readonly IContactService _contactService;
        private readonly DefaultValuesConfiguration _defaultValues;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public ConsultationService(IRepository<Consultation> consultationRepository, ILogger logger, IMailer mailer, IOptions<WebsiteConfiguration> config, IAuthentication authentication, IRepository<UserConsultation> userConsultationRepository, IRepository<Slot> slotRepository, IContactService contactService, IOptions<DefaultValuesConfiguration> defaultValue)
        {
            _consultationRepository = consultationRepository;
            _logger = logger;
            _mailer = mailer;
            _authentication = authentication;
            _userConsultationRepository = userConsultationRepository;
            _slotRepository = slotRepository;
            _contactService = contactService;
            _defaultValues = defaultValue.Value;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public Consultation Find(int id)
        {
            var consultationUserTimeZone = SessionHelper.GetCurrentUserTimeZone();
            var myTimeZone = _defaultValues.CurrentTimeZone;

            var consultation = _consultationRepository.Find(id);
            consultation.UserTimeZone = consultationUserTimeZone;
            consultation.MyTimeZone = myTimeZone;

            return consultation;
        }

        public List<Slot> GetAvailableSlots()
        {
            var userTimeZoneId = SessionHelper.GetCurrentUserTimeZone();
            var myTimeZone = _defaultValues.CurrentTimeZone;
            var userTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userTimeZoneId);
            var userNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, userTimeZoneInfo);

            // Starting from tomorrow, user's local time
            var slots = _slotRepository.Find(e => !e.IsTaken && e.StartsOn.Date > userNow.Date).ToList();
            slots.ForEach(e => e.UserTimeZone = userTimeZoneId);
            return slots;
        }

        public Slot FindSlot(int id)
        {
            var consultationUserTimeZone = SessionHelper.GetCurrentUserTimeZone();
            var myTimeZone = _defaultValues.CurrentTimeZone;

            var slot = _slotRepository.Find(id);
            slot.UserTimeZone = consultationUserTimeZone;
            slot.MyTimeZone = myTimeZone;

            return slot;
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
                        UserId = Current.UserId,
                        ConsultationId = consultation.Id
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"ConsultationService => CreateConsultation => {ex.GetFullErrorMessage()}");
            }

            try
            {
                SendEmailToNineStar(consultation, contact);
                SendEmailToCustomer(consultation, contact);
            }
            catch (Exception e)
            {
                _logger.Error($"ConsultationService => CreateConsultation => Error sending emails {e.GetFullErrorMessage()}");
            }
        }

        public void CreateFreeSlots()
        {
            var slots = new List<Slot>();
            var startDay = GetNextTuesday();

            for (int weekNumber = 0; weekNumber < 8; weekNumber++)
            {
                for (int dayNumber = 0; dayNumber < 3; dayNumber++)
                {
                    var startTime = new DateTimeOffset(startDay.Year, startDay.Month, startDay.Day, 11, 0, 0, TimeSpan.Zero);
                    CreateSlotsForDay(slots, startTime);
                    startDay = startDay.AddDays(1);
                }
                startDay = startDay.AddDays(7);
            }

            foreach (var slot in slots)
            {
                var existingSlot = _slotRepository.Find(e => e.StartsOn == slot.StartsOn).FirstOrDefault();
                if (existingSlot == null)
                {
                    _slotRepository.Create(slot);
                }
            }
        }

        private static void CreateSlotsForDay(List<Slot> slots, DateTimeOffset startTime)
        {
            slots.Add(new Slot
            {
                ConsultationDuration = EConsultationDuration.OneHour,
                StartsOn = startTime
            });

            startTime = startTime.AddHours(2);

            slots.Add(new Slot
            {
                ConsultationDuration = EConsultationDuration.TwoHours,
                StartsOn = startTime
            });

            startTime = startTime.AddHours(3);

            slots.Add(new Slot
            {
                ConsultationDuration = EConsultationDuration.OneHour,
                StartsOn = startTime
            });

            startTime = startTime.AddHours(2);

            slots.Add(new Slot
            {
                ConsultationDuration = EConsultationDuration.TwoHours,
                StartsOn = startTime
            });
        }

        public void SelectSlot(int consultationId, int slotId)
        {
            Consultation consultation = null;

            try
            {
                consultation = Find(consultationId);
                var slot = FindSlot(slotId);

                slot.IsTaken = true;
                consultation.ScheduledOn = slot.StartsOn;
                consultation.SlotId = slot.Id;

                _consultationRepository.Update(consultation);
                _slotRepository.Update(slot);
            }
            catch (Exception e)
            {
                _logger.Error($"ConsultationService => SelectSlot => {e.GetFullErrorMessage()}");
                throw;
            }

            try
            {
                var contact = _contactService.Find(consultation.ContactId);

                SendAppointmentConfirmationEmailToCustomer(consultation, contact);
                SendAppointmentConfirmationEmailToNineStar(consultation, contact);
            }
            catch (Exception e)
            {
                _logger.Error($"ConsultationService => SelectSlot => Error Sending Emails => {e.GetFullErrorMessage()}");
            }
        }

        private static DateTime GetNextTuesday()
        {
            DateTime today = DateTime.Today;
            int daysUntilNextTuesday = ((int)DayOfWeek.Tuesday - (int)today.DayOfWeek + 7) % 7;

            // Ensure it's not today (if today is Tuesday, skip to next week)
            if (daysUntilNextTuesday == 0)
            {
                daysUntilNextTuesday = 7;
            }

            return today.AddDays(daysUntilNextTuesday);
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

        private void SendAppointmentConfirmationEmailToNineStar(Consultation consultation, Contact contact)
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
                ScheduledOn = consultation.ScheduledOnMyTime.Value.ToString(DataAccessLayer.Constants.FormatConstants.AppointmentDisplayDateTimeFormat),
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl)
            }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private void SendAppointmentConfirmationEmailToCustomer(Consultation consultation, Contact contact)
        {
            var template = Dictionary.ConsultationBookedThankYouEmail;
            var title = Dictionary.ThankyouForBookingConsultationEmailTitle;
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                contact.FirstName,
                Duration = consultation.DurationDescription,
                ScheduledOn = consultation.ScheduledOnLocalTime.Value.ToString(DataAccessLayer.Constants.FormatConstants.AppointmentDisplayDateTimeFormat),
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                UnsubscribeLink = _urlHelper.AbsoluteAction("Unsubscribe", "Account", new { code = contact.Name }),
                DateTime.Now.Year
            }), contact.EmailAddress, contact.FullName, _config.SupportEmailAddress, _config.CompanyName);
        }
    }
}