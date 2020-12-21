using K9.Base.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IMembershipService
    {
        MembershipViewModel GetMembershipViewModel(int? userId = null);
        MembershipModel GetSwitchMembershipModel(int membershipOptionId);
        MembershipModel GetPurchaseMembershipModel(int membershipOptionId);
        StripeModel GetPurchaseStripeModel(int membershipOptionId);
        void ProcessPurchase(StripeModel model);
        /// <summary>
        /// Switch memberships without processing payment (downgrade or scheduled switch)
        /// </summary>
        /// <param name="membershipOptionId"></param>
        void ProcessSwitch(int membershipOptionId);
        List<UserMembership> GetActiveUserMemberships(int? userId = null, bool includeScheduled = false);
        UserMembership GetActiveUserMembership(int? userId = null);
        UserMembership GetScheduledSwitchUserMembership(int? userId = null);
        bool GetProfileReading(int? userId, DateTime dateOfBirth, EGender gender, bool createIfNull = true);
        bool GetRelationshipCompatibilityReading(int? userId, DateTime dateOfBirth, EGender gender, DateTime secondDateOfBirth, EGender secondGender, bool createIfNull = true);
    }
}