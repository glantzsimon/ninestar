using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;

namespace K9.WebApplication.Services.Stripe
{
    public class StripeService : IStripeService
    {
        public StripeService(IOptions<Config.StripeConfiguration> stripeConfig)
        {
            var stripeConfig1 = stripeConfig.Value;
            StripeConfiguration.ApiKey = stripeConfig1.SecretKey;
        }

        public Session CreateSession(StripeModel model)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = model.AmountAsLong,
                            Currency = model.LocalisedCurrencyThreeLetters.ToLower(),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = model.Description,
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = model.SuccessUrl,
                CancelUrl = model.CancelUrl
            };
            var service = new SessionService();
            return service.Create(options);
        }

        public Customer GetCustomer(string sessionId)
        {
            var sessionService = new SessionService();
            Session session = sessionService.Get(sessionId);

            var customerService = new CustomerService();
            return customerService.Get(session.CustomerId);
        }

        public PaymentIntent GetPaymentIntent(string sessionId)
        {
            var sessionService = new SessionService();
            Session session = sessionService.Get(sessionId);
            
            return session.PaymentIntent;
        }

        public PaymentIntent GetPaymentIntentById(string id)
        {
            var paymentIntentService = new PaymentIntentService();
            return paymentIntentService.Get(id);
        }

        public PaymentIntent GetPaymentIntent(StripeModel model)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = model.AmountAsLong,
                Description = model.Description,
                Currency = "usd",
            };

            var service = new PaymentIntentService();
            return service.Create(options);
        }
    }
}