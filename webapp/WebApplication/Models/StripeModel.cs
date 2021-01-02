using K9.Globalisation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace K9.WebApplication.Models
{
    public class StripeModel
    {
        private const string AutoLocale = "auto";

        public int MembershipOptionId { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }

        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.AmountLabel)]
        [DataType(DataType.Currency)]
        public double Amount { get; set; }
        
        public double AmountInCents => Amount * 100;
        
        public long? AmountAsLong => GetAmountAsLong();
        
        private long? GetAmountAsLong()
        {
            long.TryParse(AmountInCents.ToString(), out var value);
            return value;
        }

        public string Locale => GetLocale();

        public string LocalisedCurrencyThreeLetters { get; set; }

        public string Description { get; set; }
        
        public static string GetSystemCurrencyCode()
        {
            return "usd";
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