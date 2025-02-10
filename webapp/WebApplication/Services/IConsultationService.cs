using K9.DataAccessLayer.Models;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IConsultationService
    {
        void CreateConsultation(Consultation consultation, Contact contact);
        Consultation Find(int id);
        Slot FindSlot(int id);
        void SelectSlot(int consultationId, int slotId);
        void CreateFreeSlots();
        List<Slot> GetAvailableSlots();
    }
}