using System;

namespace K9.WebApplication.Exceptions
{
    public class InvalidPromoCodeException : Exception
    {
        public InvalidPromoCodeException(string promoCode) 
            : base($"Invalid promo code '{promoCode}'")
        {
        }
    }
}