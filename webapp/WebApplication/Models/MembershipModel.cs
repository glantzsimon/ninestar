using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Models
{
    public class MembershipModel
    {
        public MembershipModel(int userId, MembershipOption membershipOption, UserMembership activeUserMembership = null)
        {
            MembershipOption = membershipOption;
            ActiveUserMembership = activeUserMembership;
            UserId = userId;

            if (MembershipOption != null)
            {
                MembershipOption.PriceIncludingDiscount = MembershipOption.Price - (ActiveUserMembership?.CostOfRemainingActiveSubscription ?? 0);
            }
        }

        public MembershipOption MembershipOption { get; }
        public UserMembership ActiveUserMembership { get; }
        public int UserId { get; }
        public bool IsSelected { get; set; }
        public bool IsSelectable { get; set; }
        public bool IsSubscribed { get; set; }

        public double SubscriptionPrice => MembershipOption.PriceIncludingDiscount;
            
        public string MembershipDisplayCssClass => IsSelected ? "membership-selected" : IsUpgrade ? "membership-upgrade" : "";

        public string MembershipHoverCssClass => IsSelected ? "" : "shadow-hover";

        public int ActiveUserMembershipId => ActiveUserMembership?.Id ?? 0;

        public bool IsUpgrade => ActiveUserMembership != null &&
                                 ActiveUserMembership.MembershipOption.CanUpgradeTo(MembershipOption);
        
        public bool IsPayable => IsSelectable && ActiveUserMembership?.CostOfRemainingActiveSubscription < MembershipOption.Price;

        /// <summary>
        /// Returns true when the user is upgrading but the new plan is shorter-term and costs less, despite being an upgrade
        /// </summary>
        public bool IsExtendedSwitch => IsUpgrade && !IsPayable;
    }
}