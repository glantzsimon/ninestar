using K9.DataAccessLayer.Models;
using System;

namespace K9.WebApplication.ViewModels.MobileApi
{
    public sealed class MobileEntitlementsResponse
    {
        public DateTime AsOfUtc { get; set; }
        public bool Paid { get; set; }
        public bool Unlimited { get; set; }
        public bool WebsiteAccess { get; set; }
        public bool MobileAccess { get; set; }
        public MobileEntitlementSourceResponse[] Sources { get; set; }
    }

    public sealed class MobileEntitlementSourceResponse
    {
        public string Provider { get; set; }
        public MembershipOption.ESubscriptionType SubscriptionType { get; set; }
        public string State { get; set; }
        public string EndsOn { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
        public bool? AutoRenewEnabled { get; set; }
        public string ProductId { get; set; }
        public string NextProductId { get; set; }
        public MembershipOption.ESubscriptionType? NextSubscriptionType { get; set; }
        public DateTime? ChangeEffectiveAtUtc { get; set; }
        public DateTime? VerifiedAtUtc { get; set; }
    }
}
