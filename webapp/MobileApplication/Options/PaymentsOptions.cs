using K9.Base.Globalisation;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.Options
{
    public class PaymentsOptions
    {
        public PaymentsOptions(int id, int quantity, double amount, string description, string quantityDescription, string backUrlAction, string backUrlController, string successUrl, string failureUrl, string postPaymentUrl)
        {
            Id = id;
            Quantity = quantity;
            Amount = amount;
            Description = description;
            QuantityDescription = quantityDescription;
            BackUrlAction = backUrlAction;
            BackUrlController = backUrlController;
            SuccessUrl = successUrl;
            FailureUrl = failureUrl;
            PostPaymentUrl = postPaymentUrl;
        }

        public int Id { get; set; }

        public int Quantity { get; set; }

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.AmountToPayLabel)]
        [DataType(DataType.Currency)]
        public double Amount { get; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TotalLabel)]
        [DataType(DataType.Currency)]
        public double Total => Amount;

        public long AmountInCents => (long)(Amount * 100);

        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NameLabel)]
        [StringLength(128)]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
        [EmailAddress(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmailAddressLabel)]
        [StringLength(255)]
        public string EmailAddress { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PhoneNumberLabel)]
        [StringLength(255)]
        public string PhoneNumber { get; set; }

        public string Description { get; }

        public string QuantityDescription { get; }

        public string BackUrlAction { get; }

        public string BackUrlController { get; }

        public string SuccessUrl { get; set; }

        public string FailureUrl { get; set; }

        public string PostPaymentUrl { get; set; }
    }
}