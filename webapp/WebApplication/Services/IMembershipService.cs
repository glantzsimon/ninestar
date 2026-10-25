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
        /// <summary>Read an existing active membership without creating a free grant.</summary>
        UserMembership GetExistingActiveMembership(int authenticatedUserId);
        /// <summary>
        /// OAuth only: look up an existing active membership for the VERIFIED
        /// website user ID. Never create a free membership or accept a
        /// chat-supplied account number as proof of identity.
        /// </summary>
        UserMembership GetExistingActiveMembershipForOAuth(int authenticatedUserId);
        UserMembership GetActiveUserMembership(string accountNumber);
        MembershipOption GetMembershipOption(int id);
        void UseComplementaryPersonalChartReading(int userId);
        void UseComplementaryPredictionsReading(int userId);
        void UseComplementaryCompatibilityReading(int userId);
        void UseComplementaryKarmicReading(int userId);
        void AllocateFreeReadingsToAllAccounts();
    }
}
