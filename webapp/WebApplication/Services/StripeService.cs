using K9.WebApplication.Models;
using Stripe;

namespace K9.WebApplication.Services
{
    public class StripeService : IStripeService
    {
        public void Charge(StripeModel model)
        {
            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = model.StripeEmail,
                SourceToken = model.StripeToken,
                Description = model.Description
            });

            charges.Create(new StripeChargeCreateOptions
            {
                Amount = (int)model.DonationAmountInCents,
                Description = model.Description,
                Currency = model.LocalisedCurrencyThreeLetters,
                CustomerId = customer.Id
            });
        }
    }
}