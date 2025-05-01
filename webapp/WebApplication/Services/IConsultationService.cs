using K9.DataAccessLayer.Models;
using K9.WebApplication.ViewModels;
using System;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IConsultationService : IBaseService
    {
        int CreateConsultation(Consultation consultation, Contact contact, int? userId = null, bool isComplementary = false);
        Consultation Find(int id);
        UserConsultation FindUserConsultation(int consultationId, int userId);
        Slot FindSlot(int id);
        void SelectSlot(int consultationId, int slotId);
        void CreateFreeSlots();
        List<Slot> GetAvailableSlots();
        List<ConsultationBookingViewModel> GetAllSlotsAndBookings(DateTime date);
    }
}