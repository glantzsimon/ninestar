using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Models
{
    public class MembershipModel
    {
        public MembershipModel(int userId, MembershipOption membershipOption, UserMembership activeUserMembership = null, Promotion promotion = null)
        {
            MembershipOption = membershipOption;
            ActiveUserMembership = activeUserMembership;
            UserId = userId;
            Promotion = promotion;
            
            if (MembershipOption != null)
            {
                if (promotion != null)
                {
                    membershipOption.Price = promotion.SpecialPrice;
                }

                MembershipOption.PriceIncludingDiscountForRemainingPreviousSubscription = MembershipOption.Price - (ActiveUserMembership?.CostOfRemainingActiveSubscription ?? 0);
            }
        }

        public MembershipOption MembershipOption { get; }
        public UserMembership ActiveUserMembership { get; }
        public Promotion Promotion { get; set; }
        public int UserId { get; }
        public bool IsSelected { get; set; }
        public bool IsSelectable { get; set; }
        public bool IsSubscribed { get; set; }

        public double SubscriptionPrice => MembershipOption.Price;

        public string MembershipDisplayCssClass => IsSelected ? "membership-selected" : IsUpgrade ? "membership-upgrade" : "";

        public string MembershipHoverCssClass => IsSelected ? "" : "shadow-hover";

        public int ActiveUserMembershipId => ActiveUserMembership?.Id ?? 0;

        public bool IsUpgrade => ActiveUserMembership != null &&
                                 ActiveUserMembership.MembershipOption.CanUpgradeTo(MembershipOption);

    }
}