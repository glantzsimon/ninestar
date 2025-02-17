using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Extensions;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Helpers;
using NLog;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        private readonly IRepository<User> _userRepository;
        private readonly DefaultValuesConfiguration _defaultValues;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public ConsultationService(IRepository<Consultation> consultationRepository, ILogger logger, IMailer mailer, IOptions<WebsiteConfiguration> config, IAuthentication authentication, IRepository<UserConsultation> userConsultationRepository, IRepository<Slot> slotRepository, IContactService contactService, IOptions<DefaultValuesConfiguration> defaultValue, IRepository<User> userRepository)
        {
            _consultationRepository = consultationRepository;
            _logger = logger;
            _mailer = mailer;
            _authentication = authentication;
            _userConsultationRepository = userConsultationRepository;
            _slotRepository = slotRepository;
            _contactService = contactService;
            _userRepository = userRepository;
            _defaultValues = defaultValue.Value;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public UserConsultation FindUserConsultation(int consultationId, int userId)
        {
            return _userConsultationRepository
                .Find(e => e.ConsultationId == consultationId && e.UserId == userId).FirstOrDefault();
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
            var myTimeZoneId = _defaultValues.CurrentTimeZone;
            var userTimeZoneId = SessionHelper.GetCurrentUserTimeZone();
            var tomorrow = DateTime.UtcNow.AddDays(2);

            // Give two days notice
            var userDatAfterTomorrow = tomorrow.ToZonedTime(_defaultValues.CurrentTimeZone);
            var userDayAfterTomorrowUtc = userDatAfterTomorrow.ToDateTimeUtc().Date;

            // Starting from tomorrow, user's local time
            var slots = _slotRepository.Find(e => !e.IsTaken && e.StartsOn > userDayAfterTomorrowUtc).ToList();
            slots.ForEach((e) =>
            {
                e.UserTimeZone = userTimeZoneId;
                e.MyTimeZone = myTimeZoneId;
            });
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

        public int CreateConsultation(Consultation consultation, Contact contact, int? userId = null)
        {
            userId = userId.HasValue ? userId : Current.UserId;

            try
            {
                _consultationRepository.Create(consultation);
                _userConsultationRepository.Create(new UserConsultation
                {
                    UserId = userId.Value,
                    ConsultationId = consultation.Id
                });

            }
            catch (Exception ex)
            {
                _logger.Error($"ConsultationService => CreateConsultation => {ex.GetFullErrorMessage()}");
            }

            try
            {
                var user = _userRepository.Find(userId.Value);

                SendEmailToNineStar(consultation, user);
                SendEmailToUser(consultation, user);
            }
            catch (Exception e)
            {
                _logger.Error($"ConsultationService => CreateConsultation => Error sending emails {e.GetFullErrorMessage()}");
            }

            return consultation.Id;
        }

        public void CreateFreeSlots()
        {
            var myTimeZone = _defaultValues.CurrentTimeZone;
            var slots = new List<Slot>();
            var startDay = GetNextTuesday();
            var timeZone = DateTimeZoneProviders.Tzdb[myTimeZone];
            var now = SystemClock.Instance.GetCurrentInstant();
            var offset = timeZone.GetUtcOffset(now).ToTimeSpan();

            for (int weekNumber = 0; weekNumber < 5; weekNumber++)
            {
                var day = new DateTime(startDay.Ticks);
                for (int dayNumber = 0; dayNumber < 3; dayNumber++)
                {
                    var startTime = new DateTimeOffset(day.Year, day.Month, day.Day, 11, 0, 0, offset);
                    CreateSlotsForDay(slots, startTime);
                    day = day.AddDays(1);
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

            startTime = startTime.AddHours(3);

            slots.Add(new Slot
            {
                ConsultationDuration = EConsultationDuration.TwoHours,
                StartsOn = startTime
            });

            startTime = startTime.AddHours(5);

            slots.Add(new Slot
            {
                ConsultationDuration = EConsultationDuration.OneHour,
                StartsOn = startTime
            });
        }

        public void SelectSlot(int consultationId, int slotId)
        {
            Consultation consultation = null;

            try
            {
                consultation = Find(consultationId);

                if (consultation.ScheduledOn.HasValue)
                {
                    var existingSlot = _slotRepository.Find(e => e.Id == consultation.SlotId).FirstOrDefault();
                    if (existingSlot != null)
                    {
                        existingSlot.IsTaken = false;
                        _slotRepository.Update(existingSlot);
                    }
                }

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
                var user = _userRepository.Find(contact.UserId ?? Current.UserId);
                SendAppointmentConfirmationEmailToUser(consultation, user);
                SendAppointmentConfirmationEmailToNineStar(consultation, user);
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


        private void SendEmailToNineStar(Consultation consultation, User user)
        {
            var template = Dictionary.ConsultationBookedEmail;
            var title = "We have received a consultation booking!";
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                ContactName = user.FullName,
                CustomerEmail = user.EmailAddress,
                user.PhoneNumber,
                Duration = consultation.DurationDescription,
                Price = consultation.FormattedPrice,
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl)
            }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private void SendEmailToUser(Consultation consultation, User user)
        {
            var template = Dictionary.ConsultationBookedThankYouEmail;
            var title = Dictionary.ThankyouForBookingConsultationEmailTitle;
            var contact = _contactService.Find(user.EmailAddress);

            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                user.FirstName,
                Duration = consultation.DurationDescription,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                ScheduleUrl = _urlHelper.AbsoluteAction("ScheduleConsultation", "Consultation", new { consultationId = consultation.Id }),
                PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                TermsOfServiceLink = _urlHelper.AbsoluteAction("TermsOfService", "Home"),
                UnsubscribeLink = _urlHelper.AbsoluteAction("UnsubscribeUser", "Account", new { externalId = user.Name }),
                DateTime.Now.Year
            }), user.EmailAddress, user.FullName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private void SendAppointmentConfirmationEmailToNineStar(Consultation consultation, User user)
        {
            var template = Dictionary.ConsultationScheduledEmail;
            var title = "We have received a consultation booking!";
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                ContactName = user.FullName,
                CustomerEmail = user.EmailAddress,
                user.PhoneNumber,
                Duration = consultation.DurationDescription,
                ScheduledOn = consultation.ScheduledOnMyTime.Value.ToString(DataAccessLayer.Constants.FormatConstants.AppointmentDisplayDateTimeFormat),
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl)
            }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private void SendAppointmentConfirmationEmailToUser(Consultation consultation, User user)
        {
            var template = Dictionary.ConsultationScheduledThankYouEmail;
            var title = Dictionary.ThankyouForBookingConsultationEmailTitle;
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                user.FirstName,
                Duration = consultation.DurationDescription,
                ScheduledOn = consultation.ScheduledOnLocalTime.Value.ToString(DataAccessLayer.Constants.FormatConstants.AppointmentDisplayDateTimeFormat),
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                RescheduleUrl = _urlHelper.AbsoluteAction("ScheduleConsultation", "Consultation", new { consultationId = consultation.Id }),
                PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                TermsOfServiceLink = _urlHelper.AbsoluteAction("TermsOfService", "Home"),
                UnsubscribeLink = _urlHelper.AbsoluteAction("UnsubscribeUser", "Account", new { externalId = user.Name }),
                DateTime.Now.Year
            }), user.EmailAddress, user.FullName, _config.SupportEmailAddress, _config.CompanyName);
        }
    }
}