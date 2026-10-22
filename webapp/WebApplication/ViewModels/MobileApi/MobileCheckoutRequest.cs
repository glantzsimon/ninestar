using K9.DataAccessLayer.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels.MobileApi
{
    public enum MobileCheckoutKind { Membership, Consultation, Donation }

    public class MobileCheckoutRequest
    {
        public Guid RequestId { get; set; }
        public MobileCheckoutKind Kind { get; set; }
        public int MembershipOptionId { get; set; }
        public EConsultationDuration Duration { get; set; } = EConsultationDuration.OneHour;
        [Range(1, 100000)] public double DonationAmount { get; set; } = 1;
        [StringLength(100)] public string PromoCode { get; set; }
    }

    public class MobileCheckoutIntent
    {
        public Guid RequestId { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public long AmountInMinorUnits { get; set; }
        public string Currency { get; set; }
        public bool Completed { get; set; }
        public int? ConsultationId { get; set; }
    }
}
