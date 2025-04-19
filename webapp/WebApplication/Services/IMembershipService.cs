using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IMembershipService : IBaseService
    {
        UserMembershipViewModel GetMembershipViewModel(int? userId = null);
        MembershipModel GetSwitchMembershipModel(int membershipOptionId, int? userId = null);
        MembershipModel GetPurchaseMembershipModel(int membershipOptionId, string promoCode = "");
        
        void CreateFreeMembership(int userId);
        void ScheduleRemindersForUser(int userId);

        /// <summary>
        /// Returns true if no payment is required. Returns false if payment is requried. Errors if an exception occurs
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool CreateMembershipFromPromoCode(int userId, string code);
        void ProcessPurchase(PurchaseModel purchaseModel);
        void AssignMembershipToUser(int membershipOptionId, int userId, Promotion promotion = null);

        void CreateComplementaryUserConsultation(int userId,
            EConsultationDuration duration = EConsultationDuration.OneHour);
        
        List<UserMembership> GetActiveUserMemberships(int userId, bool includeScheduled = false);
        UserMembership GetActiveUserMembership(int? userId = null);
        UserMembership GetActiveUserMembership(string accountNumber);
        MembershipOption GetMembershipOption(int id);
    }
}