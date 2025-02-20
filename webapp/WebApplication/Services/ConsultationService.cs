using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Extensions;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class ConsultationService : BaseService, IConsultationService
    {
        private readonly IContactService _contactService;
        private readonly IRepository<Consultation> _consultationRepository;
        private readonly IRepository<UserConsultation> _userConsultationRepository;
        private readonly IRepository<Slot> _slotRepository;
        
        public ConsultationService(INineStarKiBasePackage my, IContactService contactService, IRepository<Consultation> consultationRepository, IRepository<UserConsultation> userConsultationRepository, IRepository<Slot> slotRepository)
            : base(my)
        {
            _contactService = contactService;
            _consultationRepository = consultationRepository;
            _userConsultationRepository = userConsultationRepository;
            _slotRepository = slotRepository;
        }

        public UserConsultation FindUserConsultation(int consultationId, int userId)
        {
            return _userConsultationRepository
                .Find(e => e.ConsultationId == consultationId && e.UserId == userId).FirstOrDefault();
        }

        public Consultation Find(int id)
        {
            var consultationUserTimeZone = SessionHelper.GetCurrentUserTimeZone();
            var myTimeZone = My.DefaultValuesConfiguration.CurrentTimeZone;

            var consultation = _consultationRepository.Find(id);
            consultation.UserTimeZone = consultationUserTimeZone;
            consultation.MyTimeZone = myTimeZone;

            return consultation;
        }

        public List<Slot> GetAvailableSlots()
        {
            var myTimeZoneId = My.DefaultValuesConfiguration.CurrentTimeZone;
            var userTimeZoneId = SessionHelper.GetCurrentUserTimeZone();
            var tomorrow = DateTime.UtcNow.AddDays(2);

            // Give two days notice
            var userDatAfterTomorrow = tomorrow.ToZonedTime(My.DefaultValuesConfiguration.CurrentTimeZone);
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
            var myTimeZone = My.DefaultValuesConfiguration.CurrentTimeZone;

            var slot = _slotRepository.Find(id);
            slot.UserTimeZone = consultationUserTimeZone;
            slot.MyTimeZone = myTimeZone;

            return slot;
        }

        public int CreateConsultation(Consultation consultation, Contact contact, int? userId = null, bool isComplementary = false)
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
                My.Logger.Error($"ConsultationService => CreateConsultation => {ex.GetFullErrorMessage()}");
            }

            try
            {
                var user = My.UsersRepository.Find(userId.Value);

                if (isComplementary)
                {
                    SendEmailToNineStarAboutComplimentary(consultation, user);
                }
                else
                {
                    SendEmailToNineStar(consultation, user);
                    SendEmailToUser(consultation, user);
                }
            }
            catch (Exception e)
            {
                My.Logger.Error($"ConsultationService => CreateConsultation => Error sending emails {e.GetFullErrorMessage()}");
            }

            return consultation.Id;
        }

        public void CreateFreeSlots()
        {
            var myTimeZone = My.DefaultValuesConfiguration.CurrentTimeZone;
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
                My.Logger.Error($"ConsultationService => SelectSlot => {e.GetFullErrorMessage()}");
                throw;
            }

            try
            {
                var contact = _contactService.Find(consultation.ContactId);
                var user = My.UsersRepository.Find(contact.UserId ?? Current.UserId);
                SendAppointmentConfirmationEmailToUser(consultation, user);
                SendAppointmentConfirmationEmailToNineStar(consultation, user);
            }
            catch (Exception e)
            {
                My.Logger.Error($"ConsultationService => SelectSlot => Error Sending Emails => {e.GetFullErrorMessage()}");
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

        private void SendEmailToNineStarAboutComplimentary(Consultation consultation, User user)
        {
            var template = Dictionary.ComplimentaryConsultationBookedEmail;
            var title = "A complimentary consultation has been booked!";
            My.Mailer.SendEmail(title, TemplateParser.Parse(template, new
            {
                Title = title,
                ContactName = user.FullName,
                CustomerEmail = user.EmailAddress,
                user.PhoneNumber,
                Duration = consultation.DurationDescription,
                Price = consultation.FormattedPrice,
                Company = My.WebsiteConfiguration.CompanyName,
                ImageUrl = My.UrlHelper.AbsoluteContent(My.WebsiteConfiguration.CompanyLogoUrl)
            }), My.WebsiteConfiguration.SupportEmailAddress, My.WebsiteConfiguration.CompanyName, My.WebsiteConfiguration.SupportEmailAddress, My.WebsiteConfiguration.CompanyName);
        }

        private void SendEmailToNineStar(Consultation consultation, User user)
        {
            var template = Dictionary.ConsultationBookedEmail;
            var title = "We have received a consultation booking!";
            My.Mailer.SendEmail(title, TemplateParser.Parse(template, new
            {
                Title = title,
                ContactName = user.FullName,
                CustomerEmail = user.EmailAddress,
                user.PhoneNumber,
                Duration = consultation.DurationDescription,
                Price = consultation.FormattedPrice,
                Company = My.WebsiteConfiguration.CompanyName,
                ImageUrl = My.UrlHelper.AbsoluteContent(My.WebsiteConfiguration.CompanyLogoUrl)
            }), My.WebsiteConfiguration.SupportEmailAddress, My.WebsiteConfiguration.CompanyName, My.WebsiteConfiguration.SupportEmailAddress, My.WebsiteConfiguration.CompanyName);
        }

        private void SendEmailToUser(Consultation consultation, User user)
        {
            var template = Dictionary.ConsultationBookedThankYouEmail;
            var title = Dictionary.ThankyouForBookingConsultationEmailTitle;
            var contact = _contactService.Find(user.EmailAddress);

            My.Mailer.SendEmail(title, TemplateParser.Parse(template, new
            {
                Title = title,
                user.FirstName,
                Duration = consultation.DurationDescription,
                ImageUrl = My.UrlHelper.AbsoluteContent(My.WebsiteConfiguration.CompanyLogoUrl),
                ScheduleUrl = My.UrlHelper.AbsoluteAction("ScheduleConsultation", "Consultation", new { consultationId = consultation.Id }),
                PrivacyPolicyLink = My.UrlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                TermsOfServiceLink = My.UrlHelper.AbsoluteAction("TermsOfService", "Home"),
                UnsubscribeLink = My.UrlHelper.AbsoluteAction("UnsubscribeUser", "Account", new { externalId = user.Name }),
                DateTime.Now.Year
            }), user.EmailAddress, user.FullName, My.WebsiteConfiguration.SupportEmailAddress, My.WebsiteConfiguration.CompanyName);
        }

        private void SendAppointmentConfirmationEmailToNineStar(Consultation consultation, User user)
        {
            var template = Dictionary.ConsultationScheduledEmail;
            var title = "We have received a consultation booking!";
            My.Mailer.SendEmail(title, TemplateParser.Parse(template, new
            {
                Title = title,
                ContactName = user.FullName,
                CustomerEmail = user.EmailAddress,
                user.PhoneNumber,
                Duration = consultation.DurationDescription,
                ScheduledOn = consultation.ScheduledOnMyTime.Value.ToString(DataAccessLayer.Constants.FormatConstants.AppointmentDisplayDateTimeFormat),
                Company = My.WebsiteConfiguration.CompanyName,
                ImageUrl = My.UrlHelper.AbsoluteContent(My.WebsiteConfiguration.CompanyLogoUrl)
            }), My.WebsiteConfiguration.SupportEmailAddress, My.WebsiteConfiguration.CompanyName, My.WebsiteConfiguration.SupportEmailAddress, My.WebsiteConfiguration.CompanyName);
        }

        private void SendAppointmentConfirmationEmailToUser(Consultation consultation, User user)
        {
            var template = Dictionary.ConsultationScheduledThankYouEmail;
            var title = Dictionary.ThankyouForBookingConsultationEmailTitle;
            My.Mailer.SendEmail(title, TemplateParser.Parse(template, new
            {
                Title = title,
                user.FirstName,
                Duration = consultation.DurationDescription,
                ScheduledOn = consultation.ScheduledOnLocalTime.Value.ToString(DataAccessLayer.Constants.FormatConstants.AppointmentDisplayDateTimeFormat),
                ImageUrl = My.UrlHelper.AbsoluteContent(My.WebsiteConfiguration.CompanyLogoUrl),
                RescheduleUrl = My.UrlHelper.AbsoluteAction("ScheduleConsultation", "Consultation", new { consultationId = consultation.Id }),
                PrivacyPolicyLink = My.UrlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                TermsOfServiceLink = My.UrlHelper.AbsoluteAction("TermsOfService", "Home"),
                UnsubscribeLink = My.UrlHelper.AbsoluteAction("UnsubscribeUser", "Account", new { externalId = user.Name }),
                DateTime.Now.Year
            }), user.EmailAddress, user.FullName, My.WebsiteConfiguration.SupportEmailAddress, My.WebsiteConfiguration.CompanyName);
        }
    }
}