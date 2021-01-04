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
        MembershipModel GetSwitchMembershipModelBySubscriptionType(MembershipOption.ESubscriptionType subscriptionType);
        MembershipModel GetPurchaseMembershipModelBySubscriptionType(MembershipOption.ESubscriptionType subscriptionType);
        void CreateFreeMembership(int userId);
        void ProcessPurchase(PaymentModel paymentModel);
        void ProcessCreditsPurchase(PaymentModel paymentModel);
      
        /// <summary>
        /// Switch memberships without processing payment (downgrade or scheduled switch)
        /// </summary>
        /// <param name="membershipOptionId"></param>
        void ProcessSwitch(int membershipOptionId);
       
        List<UserMembership> GetActiveUserMemberships(int? userId = null, bool includeScheduled = false);
        UserMembership GetActiveUserMembership(int? userId = null);
        bool IsCompleteProfileReading(int? userId, DateTime dateOfBirth, EGender gender);
        bool IsCompleteRelationshipCompatibilityReading(int? userId, DateTime dateOfBirth, EGender gender, DateTime secondDateOfBirth, EGender secondGender);
    }
}