using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Helpers;
using K9.SharedLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;
using K9.DataAccessLaye.Attributes;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;

namespace K9.DataAccessLayer.Models
{
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = Globalisation.Strings.Names.Promotions, PluralName = Globalisation.Strings.Names.Promotions, Name = Globalisation.Strings.Names.Promotion)]
    public class Promotion : ObjectBase, IValidatableObject
    {
        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.CodeLabel)]
        [StringLength(10)]
        [MaxLength(10)]
        [MinLength(5)]
        [Index(IsUnique = true)]
        public string Code { get; set; }
        
        [UIHint("MembershipOption")]
        [Required]
        [ForeignKey("MembershipOption")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        public int MembershipOptionId { get; set; }

        public virtual MembershipOption MembershipOption { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        [LinkedColumn(LinkedTableName = "MembershipOption", LinkedColumnName = "Name")]
        public string MembershipOptionName { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary),
            Name = Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        [LinkedColumn(LinkedTableName = "MembershipOption", LinkedColumnName = "Name")]
        public string MembershipName => MembershipOption?.SubscriptionTypeNameLocal;

        [UIHint("Discount")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.DiscountLabel)]
        public EDiscount Discount { get; set; }

        private int? discountPercent;

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.DiscountPercentLabel)]
        public int DiscountPercent
        {
            get
            {
                if (!discountPercent.HasValue)
                {
                    discountPercent = Discount.GetAttribute<DiscountAttribute>().DiscountPercent;
                }

                return discountPercent.Value;
            }
        }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.DiscountAmountLabel)]
        public double DiscountFactorAmount => (double)DiscountPercent / 100f;
        
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TotalPriceLabel)]
        [DataType(DataType.Currency)]
        public double SpecialPrice { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TotalPriceLabel)]
        [DataType(DataType.Currency)]
        public double FullPrice => MembershipOption?.Price ?? 0;

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TotalPriceLabel)]
        public string FormattedSpecialPrice => SpecialPrice == 0 ? Globalisation.Dictionary.Free : SpecialPrice.ToString("C0", CultureInfo.GetCultureInfo("en-US"));

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TotalPriceLabel)]
        public string FormattedFullPrice => FullPrice == 0 ? Globalisation.Dictionary.Free : FullPrice.ToString("C0", CultureInfo.GetCultureInfo("en-US"));

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        public string SubscriptionTypeName => MembershipOption?.SubscriptionTypeNameLocal;
        
        public string PriceDescription => GetPriceDescription();

        [NotMapped] public string PromoLink { get; set; }

        public Promotion()
        {
            Code = $"9STAR{GetCode(5)}";
        }

        public double GetSpecialPrice(double price)
        {
            return price - (price * DiscountFactorAmount);
        }

        private string GetPriceDescription()
        {
            if (SpecialPrice == 0)
            {
                return Globalisation.Dictionary.FreeOfCharge;
            }
            else
            {
                return TemplateParser.Parse(Globalisation.Dictionary.ForTheDiscountedPriceOf, new
                {
                    DiscountedPrice = FormattedSpecialPrice
                });
            }
        }
        
        private string GetCode(int max)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < max; i++)
            {
                var number = Methods.RandomGenerator.Next(0, 26);
                char letter = (char)('A' + number);
                sb.Append(letter);
            }

            return sb.ToString();
        }


        #region Validation

        new public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MembershipOptionId == 0)
            {
                yield return new ValidationResult("You must select a membership", new[] { "MembershipOptionId" });
            }
            if (SpecialPrice > 0 && Discount == 0)
            {
                yield return new ValidationResult("Please input the discount amount", new[] { "Discount" });
            }

            base.Validate(validationContext);
        }

        #endregion
    }
}
