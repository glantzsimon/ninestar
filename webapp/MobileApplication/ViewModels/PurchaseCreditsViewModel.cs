using System;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels
{
    public class PurchaseCreditsViewModel
    {
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "The value must be greater than zero.")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.NumberOfCreditsLabel)]
        public int NumberOfCredits { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.CreditsPurchaseAmountLabel)]
        public int NumberOfCreditsToPurchase => NumberOfCredits;

        [DataType(DataType.Currency)]
        public double PricePerCredit => 2;

        [DataType(DataType.Currency)]
        public double PricePerCredits10 => 1.7;

        [DataType(DataType.Currency)]
        public double PricePerCredits20 => 1.3;

        [DataType(DataType.Currency)]
        public double PricePerCredits30 => 0.9;

        [DataType(DataType.Currency)]
        public double TotalPrice => GetTotalPrice();

        private double GetTotalPrice()
        {
            if (NumberOfCredits >= 30)
            {
                return NumberOfCredits * PricePerCredits30;
            }

            if (NumberOfCredits >= 20)
            {
                return NumberOfCredits * PricePerCredits20;
            }

            if (NumberOfCredits >= 10)
            {
                return NumberOfCredits * PricePerCredits10;
            }

            return NumberOfCredits * PricePerCredit;
        }
    }
}