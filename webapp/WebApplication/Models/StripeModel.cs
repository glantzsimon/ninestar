using System;
using K9.Globalisation;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;

namespace K9.WebApplication.Models
{
    public class StripeModel
    {
        public string PublishableKey { get; set; }
        private const string AutoLocale = "auto";
        public int MembershipOptionId { get; set; }

        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DonationAmountLabel)]
        [DataType(DataType.Currency)]
        public double DonationAmount { get; set; }
        
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.AmountToDonateLabel)]
        [DataType(DataType.Currency)]
        public double AmountToDonate => DonationAmount;

        public double DonationAmountInCents => DonationAmount * 100;

        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionCostLabel)]
        [DataType(DataType.Currency)]
        public double SubscriptionAmount { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionDiscountLabel)]
        [DataType(DataType.Currency)]
        public double SubscriptionDiscount { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.TotalLabel)]
        [DataType(DataType.Currency)]
        public double SubscriptionNetAmount => SubscriptionAmount - SubscriptionDiscount;

        public double SubscriptionAmountInCents => SubscriptionNetAmount * 100;

        public double AmountInCents => MembershipOptionId > 0 ? SubscriptionAmountInCents : DonationAmountInCents ;
        
        public string Locale => GetLocale();

        public string LocalisedCurrencyThreeLetters { get; set; }

        public string Description { get; set; }

        public string StripeToken { get; set; }

        [Required(ErrorMessageResourceType = typeof(Base.Globalisation.Dictionary), ErrorMessageResourceName = Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Base.Globalisation.Dictionary), ErrorMessageResourceName = Base.Globalisation.Strings.ErrorMessages.InvalidEmailAddress)]
        [EmailAddress(ErrorMessageResourceType = typeof(Base.Globalisation.Dictionary), ErrorMessageResourceName = Base.Globalisation.Strings.ErrorMessages.InvalidEmailAddress)]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmailAddressLabel)]
        [StringLength(255)]
        public string StripeEmail { get; set; }

        public string StripeBillingName { get; set; }

        public static string GetLocalisedCurrency()
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

        private static string GetLocale()
        {
            try
            {
                var locale = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
                return string.IsNullOrEmpty(locale) ? AutoLocale : locale;
            }
            catch (Exception e)
            {
                return AutoLocale;
            }
        }
    }
}