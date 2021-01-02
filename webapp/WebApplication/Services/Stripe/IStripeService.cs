using K9.WebApplication.Models;
using Stripe;
using Stripe.Checkout;

namespace K9.WebApplication.Services.Stripe
{
    public interface IStripeService
    {
        Session CreateSession(StripeModel model);
        Customer GetCustomer(string sessionId);
        PaymentIntent GetPaymentIntent(string sessionId);
        PaymentIntent GetPaymentIntent(StripeModel model);
        PaymentIntent GetPaymentIntentById(string id);
    }
}