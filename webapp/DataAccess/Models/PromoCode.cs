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
using K9.SharedLibrary.Helpers;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = Globalisation.Strings.Names.PromoCodes, PluralName = Globalisation.Strings.Names.PromoCodes, Name = Globalisation.Strings.Names.PromoCode)]
    public class PromoCode : ObjectBase, IValidatableObject
    {
        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.CodeLabel)]
        [StringLength(10)]
        [MaxLength(10)]
        [MinLength(5)]
        [Index(IsUnique = true)]
        public string Code { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.NumberToCreateLabel)]
        public int NumberToCreate { get; set; }

        [UIHint("MembershipOption")]
        [Required]
        [ForeignKey("MembershipOption")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        public int MembershipOptionId { get; set; }

        public virtual MembershipOption MembershipOption { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        [LinkedColumn(LinkedTableName = "MembershipOption", LinkedColumnName = "Name")]
        public string MembershipOptionName { get; set; }

        [UIHint("Discount")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.DiscountLabel)]
        public EDiscount Discount { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SentOnLabel)]
        public DateTime? SentOn { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.UsedOnLabel)]
        public DateTime? UsedOn { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TotalPriceLabel)]
        [DataType(DataType.Currency)]
        public double TotalPrice { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TotalPriceLabel)]
        public string FormattedPrice => TotalPrice == 0 ? Globalisation.Dictionary.Free : TotalPrice.ToString("C0", CultureInfo.GetCultureInfo("en-US"));

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        public string SubscriptionTypeName => MembershipOption?.SubscriptionTypeNameLocal;

        public string Details => GetDetails();

        public PromoCode()
        {
            Code = $"9STAR{GetCode(5)}";
        }

        private string GetDetails()
        {
            if (MembershipOption != null)
            {
                var template = "";
                if (MembershipOption.SubscriptionType > MembershipOption.ESubscriptionType.Free)
                {
                    switch (MembershipOption.SubscriptionType)
                    {
                        case MembershipOption.ESubscriptionType.WeeklyPlatinum:
                            template = Globalisation.Dictionary.weekly_membership_description;
                            break;

                        case MembershipOption.ESubscriptionType.MonthlyPlatinum:
                            template = Globalisation.Dictionary.monthly_membership_description;
                            break;

                        case MembershipOption.ESubscriptionType.AnnualPlatinum:
                            template = Globalisation.Dictionary.annual_membership_description;
                            break;

                        case MembershipOption.ESubscriptionType.LifeTimePlatinum:
                            template = Globalisation.Dictionary.lifetime_membership_description;
                            break;

                    }
                }

                return TemplateProcessor.PopulateTemplate(template, new
                {
                    FullFeatureList = Globalisation.Dictionary.full_feature_list
                });
            }

            return string.Empty;
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MembershipOptionId == 0)
            {
                yield return new ValidationResult("You must select a membership", new[] { "MembershipOptionId" });
            }
            if (TotalPrice > 0 && Discount == 0)
            {
                yield return new ValidationResult("Please input the discount amount", new[] { "Discount" });
            }
        }

        #endregion
    }
}
