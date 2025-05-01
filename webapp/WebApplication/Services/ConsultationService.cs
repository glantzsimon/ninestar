using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Extensions;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class ConsultationService : BaseService, IConsultationService
    {
        private readonly IContactService _contactService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IRepository<Consultation> _consultationRepository;
        private readonly IRepository<UserConsultation> _userConsultationRepository;
        private readonly IRepository<Slot> _slotRepository;

        public ConsultationService(INineStarKiBasePackage my, IContactService contactService, IEmailTemplateService emailTemplateService, IRepository<Consultation> consultationRepository, IRepository<UserConsultation> userConsultationRepository, IRepository<Slot> slotRepository)
            : base(my)
        {
            _contactService = contactService;
            _emailTemplateService = emailTemplateService;
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

            if (consultation != null)
            {
                consultation.UserTimeZone = consultationUserTimeZone;
                consultation.MyTimeZone = myTimeZone;
            }

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

        public List<ConsultationBookingViewModel> GetAllSlotsAndBookings(DateTime date, int? consultationId = null)
        {
            var myTimeZoneId = My.DefaultValuesConfiguration.CurrentTimeZone;
            var userTimeZoneId = SessionHelper.GetCurrentUserTimeZone();
            var startOfWeek = date.GetStartOfWeek();
            var endOfWeek = startOfWeek.AddDays(7);

            // Starting from tomorrow, user's local time
            var consultation = consultationId.HasValue && consultationId > 0 ? Find(consultationId.Value) : null;
            var slots = consultationId.HasValue && consultationId > 0
                ? _slotRepository.Find(e => e.StartsOn >= startOfWeek && e.StartsOn <= endOfWeek && e.ConsultationDuration == consultation.ConsultationDuration
                                            && (!e.IsTaken)).ToList()
                : SessionHelper.CurrentUserIsAdmin()
                    ? _slotRepository.Find(e => e.StartsOn >= startOfWeek && e.StartsOn <= endOfWeek).ToList()
                    : _slotRepository.Find(e => e.StartsOn >= startOfWeek && e.StartsOn <= endOfWeek && !e.IsTaken).ToList();

            slots.ForEach((e) =>
            {
                e.UserTimeZone = userTimeZoneId;
                e.MyTimeZone = myTimeZoneId;
            });

            return slots.Select(e =>
            {
                var existingConsultation = _consultationRepository.Find(c => c.SlotId == e.Id).FirstOrDefault();
                var item = new ConsultationBookingViewModel
                {
                    Slot = e,
                    Consultation = Find(existingConsultation?.Id ?? 0),
                    UserConsultation = FindUserConsultation(consultation?.Id ?? 0),
                };
                return item;
            }).ToList();
        }

        public Slot FindSlot(int id)
        {
            var consultationUserTimeZone = SessionHelper.GetCurrentUserTimeZone();
            var myTimeZone = My.DefaultValuesConfiguration.CurrentTimeZone;

            var slot = _slotRepository.Find(id);

            if (slot != null)
            {
                slot.UserTimeZone = consultationUserTimeZone;
                slot.MyTimeZone = myTimeZone;
            }

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
                consultation = _consultationRepository.Find(consultationId);
                if (consultation == null)
                {
                    My.Logger.Error($"ConsultationService => SelectSlot => Consultation {consultationId} not found");
                    throw new Exception("Consultation not found");
                }

                if (consultation.ScheduledOn.HasValue)
                {
                    var existingSlots = _slotRepository.Find(e => e.Id == consultation.SlotId).ToList();
                    foreach (var existingSlot in existingSlots)
                    {
                        if (existingSlot.Id != slotId)
                        {
                            existingSlot.IsTaken = false;
                            _slotRepository.Update(existingSlot);
                        }
                    }
                }

                var slot = _slotRepository.Find(slotId);
                if (slot == null)
                {
                    My.Logger.Error($"ConsultationService => SelectSlot => Slot {slotId} not found");
                    throw new Exception("Slot not found");
                }

                slot.IsTaken = true;
                _slotRepository.Update(slot);

                consultation.ScheduledOn = slot.StartsOn;
                consultation.SlotId = slot.Id;
                _consultationRepository.Update(consultation);

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
            var title = "A complimentary consultation has been booked!";
            var body = _emailTemplateService.ParseForUser(
                title,
                Dictionary.ComplimentaryConsultationBookedEmail,
                user,
                new
                {
                    Title = title,
                    ContactName = user.FullName,
                    CustomerEmail = user.EmailAddress,
                    user.PhoneNumber,
                    Duration = consultation.DurationDescription,
                    Price = consultation.FormattedPrice,
                });

            try
            {
                My.Mailer.SendEmail(
                    title,
                    body,
                    My.WebsiteConfiguration.SupportEmailAddress,
                    My.WebsiteConfiguration.CompanyName);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
            }
        }

        private void SendEmailToNineStar(Consultation consultation, User user)
        {
            var subject = "We have received a consultation booking!";
            var body = _emailTemplateService.ParseForUser(
                subject,
                Dictionary.ConsultationBookedEmail,
                user,
                new
                {
                    ContactName = user.FullName,
                    CustomerEmail = user.EmailAddress,
                    user.PhoneNumber,
                    Duration = consultation.DurationDescription,
                    Price = consultation.FormattedPrice
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

        private void SendEmailToUser(Consultation consultation, User user)
        {
            var subject = Dictionary.ThankyouForBookingConsultationEmailTitle;
            var body = _emailTemplateService.ParseForUser(
                subject,
                Dictionary.ConsultationBookedThankYouEmail,
                user,
                new
                {
                    user.FirstName,
                    Duration = consultation.DurationDescription,
                    ImageUrl = My.UrlHelper.AbsoluteContent(My.WebsiteConfiguration.CompanyLogoUrl),
                    ScheduleUrl = My.UrlHelper.AbsoluteAction("ScheduleConsultation", "Consultation", new { consultationId = consultation.Id })
                });

            try
            {
                My.Mailer.SendEmail(
                    subject,
                    body,
                    user.EmailAddress,
                    user.FullName);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
            }
        }

        private void SendAppointmentConfirmationEmailToNineStar(Consultation consultation, User user)
        {
            var subject = "A user has scheduled a consultation!";
            var body = _emailTemplateService.ParseForUser(
                subject,
                Dictionary.ConsultationScheduledEmail,
                user,
                new
                {
                    ContactName = user.FullName,
                    CustomerEmail = user.EmailAddress,
                    user.PhoneNumber,
                    Duration = consultation.DurationDescription,
                    ScheduledOn = consultation.ScheduledOnMyTime.Value.ToString(DataAccessLayer.Constants.FormatConstants.AppointmentDisplayDateTimeFormat)
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

        private void SendAppointmentConfirmationEmailToUser(Consultation consultation, User user)
        {
            var subject = Dictionary.ThankyouForBookingConsultationEmailTitle;
            var body = _emailTemplateService.ParseForUser(
                subject,
                Dictionary.ConsultationScheduledThankYouEmail,
                user,
                new
                {
                    user.FirstName,
                    Duration = consultation.DurationDescription,
                    ScheduledOn = consultation.ScheduledOnLocalTime.Value.ToString(DataAccessLayer.Constants.FormatConstants.AppointmentDisplayDateTimeFormat),
                    ImageUrl = My.UrlHelper.AbsoluteContent(My.WebsiteConfiguration.CompanyLogoUrl),
                    RescheduleUrl = My.UrlHelper.AbsoluteAction("ScheduleConsultation", "Consultation", new { consultationId = consultation.Id })
                });

            try
            {
                My.Mailer.SendEmail(
                    subject,
                    body,
                    user.EmailAddress,
                    user.FullName);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
            }
        }

        private UserConsultation FindUserConsultation(int consultationId)
        {
            var userConsultation = _userConsultationRepository
                .Find(e => e.ConsultationId == consultationId).FirstOrDefault();

            if (userConsultation != null)
            {
                userConsultation.User = My.UsersRepository.Find(userConsultation.UserId);
            }

            return userConsultation;
        }
    }
}