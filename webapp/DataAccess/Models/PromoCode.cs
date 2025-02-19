using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Extensions;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;
using K9.DataAccessLayer.Helpers;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = Globalisation.Strings.Names.PromoCodes, PluralName = Globalisation.Strings.Names.PromoCodes, Name = Globalisation.Strings.Names.PromoCode)]
    public class PromoCode : ObjectBase
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
        public int NumberToCreate { get;set; }

        [UIHint("SubscriptionType")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        public MembershipOption.ESubscriptionType SubscriptionType { get; set; }

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
        public string SubscriptionTypeName => SubscriptionType > 0 ? SubscriptionType.GetLocalisedLanguageName() : "";

        public string Details => GetDetails();

        public PromoCode()
        {
            Code = $"9STAR{GetCode(5)}";
        }

        private string GetDetails()
        {
            var sb = new StringBuilder();
            if (SubscriptionType > MembershipOption.ESubscriptionType.Free)
            {
                switch (SubscriptionType)
                {
                    case MembershipOption.ESubscriptionType.WeeklyPlatinum:
                        sb.Append(Globalisation.Dictionary.platinum_weekly_membership_description);
                        break;

                    case MembershipOption.ESubscriptionType.MonthlyPlatinum:
                        sb.Append(Globalisation.Dictionary.platinum_monthly_membership_description);
                        break;
                    
                    case MembershipOption.ESubscriptionType.AnnualPlatinum:
                        sb.Append(Globalisation.Dictionary.platinum_annual_membership_description);
                        break;
                    
                    case MembershipOption.ESubscriptionType.LifeTimePlatinum:
                        sb.Append(Globalisation.Dictionary.platinum_lifetime_membership_description);
                        break;

                }
            }

            return sb.ToString();
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
    }
}
