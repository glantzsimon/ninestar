using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IMembershipService
    {
        MembershipViewModel GetMembershipViewModel(int? userId = null);
        MembershipModel GetSwitchMembershipModel(int id);
        MembershipModel GetPurchaseMembershipModel(int id);
        StripeModel GetPurchaseStripeModel(int id);
        void ProcessPurchase(StripeModel model);
        /// <summary>
        /// Switch memberships without processing payment (downgrade or scheduled switch)
        /// </summary>
        /// <param name="id"></param>
        void ProcessSwitch(int id);
        List<UserMembership> GetActiveUserMemberships(int? userId = null, bool includeScheduled = false);
        UserMembership GetActiveUserMembership(int? userId = null);
        UserMembership GetScheduledSwitchUserMembership(int? userId = null);
    }
}