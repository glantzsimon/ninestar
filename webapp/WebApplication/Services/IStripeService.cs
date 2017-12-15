using K9.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public interface IStripeService
    {
        void Charge(StripeModel model);
    }
}