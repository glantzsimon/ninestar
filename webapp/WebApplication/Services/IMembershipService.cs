using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IMembershipService
    {
        UserMembershipViewModel GetMembershipViewModel(int? userId = null);
        MembershipModel GetSwitchMembershipModel(int membershipOptionId);
        MembershipModel GetPurchaseMembershipModel(int membershipOptionId);
        
        void CreateFreeMembership(int userId);
        void ProcessPurchaseWithPromoCode(int userId, string code);
        void ProcessPurchase(PurchaseModel purchaseModel);
        void AssignMembershipToUser(int membershipOptionId, int userId, PromoCode promoCode = null);

        void CreateComplementaryUserConsultation(int userId,
            EConsultationDuration duration = EConsultationDuration.OneHour);

        /// <summary>
        /// Switch memberships without processing payment (downgrade or scheduled switch)
        /// </summary>
        /// <param name="membershipOptionId"></param>
        void ProcessSwitch(int membershipOptionId);

        List<DataAccessLayer.Models.UserMembership> GetActiveUserMemberships(int userId, bool includeScheduled = false);
        DataAccessLayer.Models.UserMembership GetActiveUserMembership(int? userId = null);
        DataAccessLayer.Models.UserMembership GetActiveUserMembership(string accountNumber);
    }
}