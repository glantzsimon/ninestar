using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Services
{
    public interface IDonationService
    {
        void CreateDonation(Donation donation);
    }
}