using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;
using Stripe;
using System.Collections.Generic;

namespace K9.WebApplication.Services.Stripe
{
    public interface IStripeService
    {
        StripeChargeResultModel Charge(StripeModel model);
        List<StripeCharge> GetCharges();
        List<Donation> GetDonations();
    }
}