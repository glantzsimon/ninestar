using K9.Base.WebApplication.Filters;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    public class ConsultationController : BaseNineStarKiController
    {
        private readonly IConsultationService _consultationService;
        private readonly IRepository<Slot> _slotsRepository;

        public ConsultationController(INineStarKiPackage nineStarKiPackage, IConsultationService consultationService, IRepository<Slot> slotsRepository)
            : base(nineStarKiPackage)
        {
            _consultationService = consultationService;
            _slotsRepository = slotsRepository;
        }

        [Route("consultation/book")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult BookConsultationStart(EConsultationDuration duration = EConsultationDuration.OneHour)
        {
            return View(new Consultation
            {
                ConsultationDuration = duration
            });
        }

        [Route("consultation/book")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BookConsultation(Consultation consultation)
        {
            return View(consultation);
        }

        [HttpPost]
        public ActionResult ProcessConsultation(PurchaseModel purchaseModel)
        {
            try
            {
                var contact = My.ContactService.Find(purchaseModel.ContactId);

                var consultationId = _consultationService.CreateConsultation(new Consultation
                {
                    ConsultationDuration = (EConsultationDuration)purchaseModel.Quantity,
                    ContactId = purchaseModel.ContactId
                }, contact);

                return Json(new
                {
                    success = true,
                    idProperty = "consultationId",
                    id = consultationId
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"SupportController => ProcessConsultation => Error: {ex.GetFullErrorMessage()}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Route("consultation/booking-success")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult BookConsultationSuccess()
        {
            return View();
        }

        [Route("consultation/schedule")]
        public ActionResult ScheduleConsultation(int consultationId)
        {
            var consultation = _consultationService.Find(consultationId);

            if (consultation == null)
            {
                Logger.Error($"ConsultationController => ScheduleConsultation => Consultation Not Found => ConsultationId: {consultationId}");
                return HttpNotFound();
            }

            var userConsultation = _consultationService.FindUserConsultation(consultationId, Current.UserId);
            if (userConsultation == null)
            {
                Logger.Error($"ConsultationController => ScheduleConsultation => UserConsultation Not Found => ConsultationId: {consultationId} UserId: {Current.UserId}");
                return HttpNotFound();
            }

            var freeSlots = _consultationService.GetAvailableSlots().Where(e =>
                            e.ConsultationDuration == consultation.ConsultationDuration).ToList();

            return View(new ScheduleConsultationViewModel
            {
                Consultation = consultation,
                AvailableSlots = freeSlots
            });
        }

        [Route("consultation/confirm-timeslot")]
        public ActionResult ConfirmSlot(int consultationId, int slotId)
        {
            var selectedSlot = _consultationService.FindSlot(slotId);
            var consultation = _consultationService.Find(consultationId);

            if (selectedSlot == null || consultation == null)
            {
                return HttpNotFound();
            }

            return View("ScheduleConsultationConfirm", new ScheduleConsultationViewModel
            {
                Consultation = consultation,
                SelectedSlot = selectedSlot
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("consultation/select-timeslot")]
        public ActionResult SelectSlot(ScheduleConsultationViewModel model)
        {
            var selectedSlot = _consultationService.FindSlot(model.SelectedSlot.Id);
            var consultation = _consultationService.Find(model.Consultation.Id);

            if (selectedSlot == null || consultation == null)
            {
                return HttpNotFound();
            }

            try
            {
                _consultationService.SelectSlot(model.Consultation.Id, model.SelectedSlot.Id);
                return RedirectToAction("ScheduleConsultationSuccess", new { consultationId = model.Consultation.Id, selectedSlotId = model.SelectedSlot.Id });
            }
            catch (Exception e)
            {
                My.Logger.Error($"ConsultationController => SelectSlot => Error: {e.GetFullErrorMessage()}");
                ModelState.AddModelError("", Globalisation.Dictionary.FriendlyErrorMessage);
            }

            return View("ScheduleConsultationConfirm", new ScheduleConsultationViewModel
            {
                Consultation = consultation,
                SelectedSlot = selectedSlot
            });
        }

        [Route("consultation/schedule-success")]
        public ActionResult ScheduleConsultationSuccess(int consultationId, int selectedSlotId)
        {
            var selectedSlot = _consultationService.FindSlot(selectedSlotId);
            var consultation = _consultationService.Find(consultationId);

            if (selectedSlot == null || consultation == null)
            {
                return HttpNotFound();
            }

            return View(new ScheduleConsultationViewModel
            {
                SelectedSlot = selectedSlot,
                Consultation = consultation
            });
        }

        [RequirePermissions(Role = RoleNames.Administrators)]
        [Route("consultation/create-free-slots")]
        public ActionResult CreateFreeSlots()
        {
            _consultationService.CreateFreeSlots();
            return RedirectToAction("ViewAvailableSlots");
        }

        [RequirePermissions(Role = RoleNames.Administrators)]
        [Route("consultation/view-free-slots")]
        public ActionResult ViewAvailableSlots()
        {
            return View("", _consultationService.GetAvailableSlots());
        }

        [RequirePermissions(Role = RoleNames.Administrators)]
        [Route("consultation/view-calendar")]
        public ActionResult ViewCalendar()
        {
            return View(new ScheduleConsultationViewModel { Date = DateTime.Today.GetStartOfWeek() });
        }

        [Route("consultation/calendar")]
        public JsonResult Calendar(DateTime date, int? consultationId = null)
        {
            var consultation = consultationId.HasValue ? _consultationService.Find(consultationId.Value) : null;
            var allBookings = _consultationService.GetAllSlotsAndBookings(date, consultationId);
            return Json(allBookings.Select(e => new
            {
                id = e.Id,
                title = e.Title,
                start = e.StartsOn,
                end = e.EndsOn,
                isTaken = e.IsTaken,
                name = e.Name,
                confirmUrl = Url.Action("ConfirmSlot", "Consultation", new { consultationId, slotId = e.Slot.Id })
            }), JsonRequestBehavior.AllowGet);
        }

        [RequirePermissions(Role = RoleNames.Administrators)]
        [Route("consultation/calendar/update-slot")]
        public JsonResult UpdateSlot(int id, DateTime startDateTime)
        {
            try
            {
                var slot = _slotsRepository.Find(id);
                slot.StartsOn = startDateTime;
                _slotsRepository.Update(slot);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error($"ConsultationController => UpdateSlot => {e.GetFullErrorMessage()}");
                return Json(new { success = false, error = Dictionary.FriendlyErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [RequirePermissions(Role = RoleNames.Administrators)]
        [Route("consultation/calendar/create-slot")]
        public JsonResult CreateSlot(DateTime startDateTime, EConsultationDuration duration)
        {
            try
            {
                var slot = new Slot
                {
                    StartsOn = startDateTime,
                    ConsultationDuration = duration
                };

                _slotsRepository.Create(slot);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error($"ConsultationController => CreateSlot => {e.GetFullErrorMessage()}");
                return Json(new { success = false, error = Dictionary.FriendlyErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [RequirePermissions(Role = RoleNames.Administrators)]
        [Route("consultation/calendar/delete-slot")]
        public JsonResult DeleteSlot(int id)
        {
            try
            {
                _slotsRepository.Delete(id);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error($"ConsultationController => DeleteSlot => {e.GetFullErrorMessage()}");
                return Json(new { success = false, error = Dictionary.FriendlyErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }

    }
}

