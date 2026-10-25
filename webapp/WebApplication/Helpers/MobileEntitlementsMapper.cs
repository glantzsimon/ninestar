using K9.DataAccessLayer.Models;
using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Globalization;

namespace K9.WebApplication.Helpers
{
    public static class MobileEntitlementsMapper
    {
        // Protocol projection only. Membership rules remain on UserMembership.
        // The UTC timestamp describes the snapshot, not a new legacy expiry rule.
        public static MobileEntitlementsResponse Map(int userId, UserMembership membership, DateTime utcNow)
        {
            var response = new MobileEntitlementsResponse
            {
                AsOfUtc = utcNow,
                Sources = new MobileEntitlementSourceResponse[0]
            };
            if (userId <= 0 || membership == null || membership.UserId != userId ||
                membership.MembershipOption == null || !membership.IsActive ||
                !Enum.IsDefined(typeof(MembershipOption.ESubscriptionType), membership.MembershipOption.SubscriptionType))
                return response;

            var option = membership.MembershipOption;
            response.Paid = membership.IsAuthorisedToViewPaidContent();
            response.Unlimited = membership.IsUnlimited();
            response.WebsiteAccess = response.Paid;
            response.MobileAccess = response.Paid;
            response.Sources = new[]
            {
                new MobileEntitlementSourceResponse
                {
                    Provider = "website",
                    SubscriptionType = option.SubscriptionType,
                    State = "active",
                    EndsOn = option.IsFree || option.IsForever ? null :
                        membership.EndsOn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    // Legacy IsAutoRenew is not verified provider renewal state.
                    // Apple-only fields stay null until that phase is implemented.
                }
            };
            return response;
        }
    }
}
