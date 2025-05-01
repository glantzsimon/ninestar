using K9.DataAccessLayer.Models;
using K9.Globalisation;
using System;

namespace K9.WebApplication.ViewModels
{
    public class ConsultationBookingViewModel
    {
        public UserConsultation UserConsultation { get; set; }
        public Consultation Consultation { get; set; }
        public Slot Slot { get; set; }

        public int? Id => Slot?.Id;
        public string Title => UserConsultation?.User.FullName ?? Dictionary.Available;
        public DateTime? StartsOn => Slot?.StartsOnLocalTime.Value.DateTime;
        public DateTime? EndsOn => Slot?.EndsOnLocalTime.Value.DateTime;
        public bool IsTaken => Slot.IsTaken;
        public string Name => UserConsultation?.User.FullName ?? "";
    }
}