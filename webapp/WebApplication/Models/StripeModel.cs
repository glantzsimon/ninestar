using K9.Globalisation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;

namespace K9.WebApplication.Models
{
    public class StripeModel
    {
        public string PublishableKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DonationAmountLabel)]
        [DataType(DataType.Currency)]
        public double DonationAmount { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.AmountToDonateLabel)]
        [DataType(DataType.Currency)]
        public double AmountToDonate => DonationAmount;

        public double DonationAmountInCents => DonationAmount * 100;

        public string LocalisedCurrencyThreeLetters => GetLocalisedCurrency();

        public string Locale => Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();

        public string Description { get; set; }

        public string StripeToken { get; set; }

        public string StripeTokenType	 { get; set; }

        public string StripeEmail { get; set; }

        public string StripeBillingName { get; set; }

        public string StripeBillingAddressCountry { get; set; }

        public string StripeBillingAddressCountryCode { get; set; }

        public string StripeBillingAddressZip { get; set; }

        public string StripeBillingAddressLine1 { get; set; }

        public string StripeBillingAddressCity { get; set; }

        public string StripeBillingAddressState { get; set; }

        private static string GetLocalisedCurrency()
        {
            try
            {
                return new RegionInfo(Thread.CurrentThread.CurrentUICulture.LCID).ISOCurrencySymbol;
            }
            catch (Exception e)
            {
                return "USD";
            }
        }
    }
}