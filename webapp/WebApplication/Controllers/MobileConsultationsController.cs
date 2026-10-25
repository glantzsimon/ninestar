using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/consultations")]
    public class MobileConsultationsController : MobileApiControllerBase
    {
        private readonly IConsultationService _consultations;
        private readonly IRepository<Slot> _slots;
        private readonly DbContext _database;
        public MobileConsultationsController(INineStarKiPackage package, IConsultationService consultations,
            IRepository<Slot> slots, DbContext database) : base(package)
        { _consultations = consultations; _slots = slots; _database = database; }

        [HttpGet, Route("me")]
        public ActionResult Mine()
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            return ApiSuccess(Package.UserService.GetPendingConsultations(id).Select(e => new
            {
                e.ConsultationId, e.Consultation.ConsultationDuration, e.Consultation.DurationDescription,
                e.Consultation.ScheduledOn, e.Consultation.EndsOn, e.Consultation.SlotId,
                e.Consultation.Price, e.Consultation.CompletedOn
            }).ToArray());
        }

        [HttpGet, Route("slots")]
        public ActionResult Slots(int consultationId)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (_consultations.FindUserConsultation(consultationId, id) == null) return ApiError(404, "consultation_not_found");
            var consultation = _consultations.Find(consultationId);
            if (consultation == null || consultation.IsDeleted) return ApiError(404, "consultation_not_found");
            return ApiSuccess(_consultations.GetAvailableSlots()
                .Where(e => !e.IsTaken && e.StartsOn > GetUtcNow() && e.ConsultationDuration == consultation.ConsultationDuration)
                .Select(SlotData).ToArray());
        }

        [HttpGet, Route("calendar")]
        public ActionResult Calendar(int consultationId, DateTime date)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!ModelState.IsValid || date.Year < 1900 || date.Year > 2100) return ApiError(400, "invalid_calendar_date");
            if (_consultations.FindUserConsultation(consultationId, id) == null) return ApiError(404, "consultation_not_found");
            // Availability only: never return another customer's name or booking.
            return ApiSuccess(_consultations.GetAllSlotsAndBookings(date, consultationId).Select(e => new
            {
                Slot = SlotData(e.Slot), e.IsTaken,
                IsAvailable = !e.IsTaken && e.Slot.StartsOn > GetUtcNow()
            }).ToArray());
        }

        [HttpPost, Route("select-slot")]
        public ActionResult SelectSlot(int consultationId, int slotId)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!ModelState.IsValid || consultationId <= 0 || slotId <= 0) return ApiError(400, "invalid_booking_request");
            using (var transaction = _database.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                if (_consultations.FindUserConsultation(consultationId, id) == null) return ApiError(404, "consultation_not_found");
                var consultation = _consultations.Find(consultationId);
                var slot = _consultations.FindSlot(slotId);
                if (consultation == null || consultation.IsDeleted || slot == null || slot.IsDeleted) return ApiError(404, "booking_not_found");
                if (slot.StartsOn <= GetUtcNow() || slot.ConsultationDuration != consultation.ConsultationDuration ||
                    (slot.IsTaken && consultation.SlotId != slotId)) return ApiError(409, "slot_unavailable");
                _consultations.SelectSlot(consultationId, slotId);
                transaction.Commit();
                return ApiSuccess(new { ConsultationId = consultationId, Slot = SlotData(slot) });
            }
        }

        [HttpPost, Route("admin/create-free-slots")]
        public ActionResult CreateFreeSlots()
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            _consultations.CreateFreeSlots();
            return ApiSuccess(new { Created = true });
        }

        [HttpPost, Route("admin/slot")]
        public ActionResult SaveSlot(int? slotId, DateTimeOffset startsOn, EConsultationDuration duration)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!ModelState.IsValid || !Enum.IsDefined(typeof(EConsultationDuration), duration) || startsOn <= GetUtcNow())
                return ApiError(400, "invalid_slot_request");
            var slot = slotId.HasValue ? _slots.Find(slotId.Value) : new Slot();
            if (slot == null) return ApiError(404, "slot_not_found");
            if (slot.IsTaken) return ApiError(409, "slot_in_use");
            slot.StartsOn = startsOn;
            slot.ConsultationDuration = duration;
            if (slotId.HasValue) _slots.Update(slot); else _slots.Create(slot);
            return ApiSuccess(SlotData(slot));
        }

        [HttpPost, Route("admin/slot/delete")]
        public ActionResult DeleteSlot(int slotId)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            var slot = _slots.Find(slotId);
            if (slot == null) return ApiError(404, "slot_not_found");
            if (slot.IsTaken) return ApiError(409, "slot_in_use");
            _slots.Delete(slotId);
            return ApiSuccess(new { Deleted = true });
        }

        private static object SlotData(Slot slot) => slot == null ? null : new
        {
            slot.Id, slot.StartsOn, slot.EndsOn, slot.ConsultationDuration, slot.DurationDescription, slot.IsTaken
        };
    }
}
