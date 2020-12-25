using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public interface IDonationService
    {
        void CreateDonation(Donation donation);
        int GetFundsReceivedToDate();
        StripeModel GetDonationStripeModel();
    }
}