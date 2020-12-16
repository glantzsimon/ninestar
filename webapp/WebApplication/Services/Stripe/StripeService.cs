using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using Stripe;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Services.Stripe
{
    public class StripeService : IStripeService
    {
        private readonly Config.StripeConfiguration _stripeConfig;

        public StripeService(IOptions<Config.StripeConfiguration> stripeConfig)
        {
            _stripeConfig = stripeConfig.Value;
        }

        public StripeChargeResultModel Charge(StripeModel model)
        {
            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = model.StripeEmail,
                SourceToken = model.StripeToken,
                Description = model.Description
            });

            // Do not charge for zero amount
            var charge = model.AmountInCents > 0 ? charges.Create(new StripeChargeCreateOptions
            {
                Amount = (int)model.AmountInCents,
                Description = model.Description,
                Currency = model.LocalisedCurrencyThreeLetters,
                CustomerId = customer.Id
            }) : new StripeCharge();

            return new StripeChargeResultModel(charge, customer);
        }

        public List<StripeCharge> GetCharges()
        {
            StripeConfiguration.SetApiKey(_stripeConfig.SecretKey);
            var allCharges = new List<StripeCharge>();
            var stripeCharges = new List<StripeCharge>();
            var chargeService = new StripeChargeService();
            var total = 0;
            var noItemsReturned = false;

            while (total == 0 && !noItemsReturned || stripeCharges.Any())
            {
                stripeCharges = chargeService.List(
                    new StripeChargeListOptions()
                    {
                        Limit = 100,
                        StartingAfter = allCharges.LastOrDefault()?.Id
                    }
                ).ToList();
                allCharges.AddRange(stripeCharges);
                total += allCharges.Count;
                noItemsReturned = total == 0;
            }

            return allCharges.ToList();
        }

        public List<Donation> GetDonations()
        {
            var stripeCharges = GetCharges().ToList();
            return stripeCharges.Select(c =>
                new Donation
                {
                    StripeId = c.Id,
                    Customer = c.Customer?.Description ?? c.Source?.Card?.Name,
                    Currency = c.Currency.ToUpper(),
                    CustomerEmail = c.Customer?.Email,
                    DonationDescription = c.Description + (c.Refunded ? " (refunded)" : ""),
                    DonatedOn = c.Created,
                    DonationAmount = (c.Amount / 100) * (c.Refunded ? -1 : 1),
                    Status = c.Status
                }).ToList();
        }
    }
}