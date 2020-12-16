using Stripe;

namespace K9.WebApplication.Models
{
    public class StripeChargeResultModel
    {
        public StripeCharge StripeCharge { get; }
        public StripeCustomer StripeCustomer { get; }

        public StripeChargeResultModel(StripeCharge stripeCharge, StripeCustomer stripeCustomer)
        {
            StripeCharge = stripeCharge;
            StripeCustomer = stripeCustomer;
        }
    }
}