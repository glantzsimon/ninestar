using K9.DataAccessLayer.Models;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime.Misc;

namespace K9.WebApplication.ViewModels
{
    public class UserMembershipsViewModel
    {
        public List<MembershipOption.ESubscriptionType> SubscriptionTypes { get; set; }
        public List<UserMembership> UserMemberships { get; set; }

        public UserMembershipsViewModel()
        {
            SubscriptionTypes = new ListStack<MembershipOption.ESubscriptionType>
            {
                MembershipOption.ESubscriptionType.Free,
                MembershipOption.ESubscriptionType.WeeklyPlatinum,
                MembershipOption.ESubscriptionType.AnnualPlatinum,
                MembershipOption.ESubscriptionType.LifeTimePlatinum,
            };
        }

        public List<UserMembership> GetMembershipsBySubscriptionType(MembershipOption.ESubscriptionType type)
        {
            if (UserMemberships == null)
            {
                return new List<UserMembership>();
            }

            return UserMemberships.Where(e => e.MembershipOption?.SubscriptionType == type)?.ToList() ?? new List<UserMembership>();
        }
    }
}