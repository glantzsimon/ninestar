﻿using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IMembershipService
    {
        MembershipViewModel GetMembershipViewModel(int? userId = null);
        MembershipModel GetSwitchMembershipModel(int membershipOptionId);
        MembershipModel GetPurchaseMembershipModel(int membershipOptionId);
        
        void CreateFreeMembership(int userId);
        void ProcessPurchaseWithPromoCode(int userId, string code);
        void ProcessPurchase(PurchaseModel purchaseModel);
        void AssignMembershipToUser(int membershipOptionId, int? userId = null, PromoCode promoCode = null);
        
        /// <summary>
        /// Switch memberships without processing payment (downgrade or scheduled switch)
        /// </summary>
        /// <param name="membershipOptionId"></param>
        void ProcessSwitch(int membershipOptionId);

        List<UserMembership> GetActiveUserMemberships(int? userId = null, bool includeScheduled = false);
        UserMembership GetActiveUserMembership(int? userId = null);
    }
}