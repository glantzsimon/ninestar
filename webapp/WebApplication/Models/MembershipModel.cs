using K9.DataAccessLayer.Models;
using System;

namespace K9.WebApplication.Models
{
    public class MembershipModel
    {
        public MembershipModel(int userId, MembershipOption membershipOption, UserMembership activeUserMembership = null)
        {
            MembershipOption = membershipOption;
            ActiveUserMembership = activeUserMembership;
            UserId = userId;
        }

        public MembershipOption MembershipOption { get; }
        public UserMembership ActiveUserMembership { get; }
        public int UserId { get; }
        public bool IsSelected { get; set; }
        public bool IsSelectable { get; set; }
        public bool IsSubscribed { get; set; }

        public UserMembership NewMembership => GetNewMembership();

        public string MembershipDisplayCssClass => IsSelected ? "membership-selected" : IsUpgrade ? "membership-upgrade" : "";

        public string MembershipHoverCssClass => IsSelected ? "" : "shadow-hover";

        public int ActiveUserMembershipId => ActiveUserMembership?.Id ?? 0;

        public bool IsUpgrade => ActiveUserMembership != null &&
                                 ActiveUserMembership.MembershipOption.CanUpgradeTo(MembershipOption);

        public bool IsScheduledSwitch => !IsUpgrade && IsSelectable && ActiveUserMembership != null && ActiveUserMembership.EndsOn > DateTime.Today;

        public bool IsPayable => IsSelectable && ActiveUserMembership?.CostOfRemainingActiveSubscription < MembershipOption.Price;

        /// <summary>
        /// Returns true when the user is upgrading but the new plan is shorter-term and costs less, despite being an upgrade
        /// </summary>
        public bool IsExtendedSwitch => IsUpgrade && !IsPayable;

        private UserMembership GetNewMembership()
        {
            if (IsExtendedSwitch)
            {   
                return new UserMembership
                {
                    UserId = UserId,
                    MembershipOptionId = MembershipOption.Id,
                    StartsOn = DateTime.Today,
                    EndsOn = GetAdjustedMembershipEndDate(DateTime.Today),
                    IsAutoRenew = true
                };
            }

            var startsOn = IsScheduledSwitch ? ActiveUserMembership.EndsOn.AddDays(1) : DateTime.Today;
            return new UserMembership
            {
                UserId = UserId,
                MembershipOptionId = MembershipOption.Id,
                StartsOn = startsOn,
                EndsOn = MembershipOption.IsMonthly ? startsOn.AddMonths(1) : startsOn.AddYears(1),
                IsAutoRenew = true
            };
        }

        private DateTime GetAdjustedMembershipEndDate(DateTime startsOn)
        {
            var subscriptionRatio = ActiveUserMembership.CostOfRemainingActiveSubscription / MembershipOption.Price;
            var subscriptionStandardEndDate = MembershipOption.IsAnnual ? startsOn.AddYears(1) : startsOn.AddMonths(1);
            var subscriptionStandardDuration = startsOn.Subtract(subscriptionStandardEndDate);
            var newTicks = subscriptionStandardDuration.Ticks * subscriptionRatio;
            var adjustedDuration = TimeSpan.FromTicks((long)newTicks);

            return startsOn.Add(adjustedDuration);
        }
    }
}