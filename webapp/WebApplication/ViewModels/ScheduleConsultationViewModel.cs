using System;
using K9.DataAccessLayer.Models;
using System.Collections.Generic;

namespace K9.WebApplication.ViewModels
{
    public class ScheduleConsultationViewModel
    {
        public DateTime Date { get; set; }
        public Consultation Consultation { get; set; }
        public List<Slot> AvailableSlots { get; set; }
        public Slot SelectedSlot { get; set; }
    }
}