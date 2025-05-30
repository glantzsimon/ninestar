﻿using K9.DataAccessLayer.Models;
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
        public string Title => Slot.IsTaken ? UserConsultation?.User.FullName : IsFree ? Dictionary.Available : Dictionary.Unavailable;
        public DateTimeOffset? StartsOn => Slot?.StartsOnLocalTime;
        public DateTimeOffset? EndsOn => Slot?.EndsOnLocalTime;
        public bool IsTaken => Slot.IsTaken;
        public bool IsFree => !IsTaken && StartsOn >= DateTime.UtcNow.ToLocalTime();
        public string Name => Slot.IsTaken ? UserConsultation?.User.FullName ?? "" : "";
    }
}