using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = Globalisation.Strings.Names.PromoCodes, PluralName = Globalisation.Strings.Names.PromoCodes, Name = Globalisation.Strings.Names.PromoCode)]
    public class PromoCode : ObjectBase
    {
        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NameLabel)]
        [StringLength(10)]
        [Range(10, 10)]
        [Index(IsUnique = true)]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.NumberOfCreditsLabel)]
        public int Credits { get; set; }

        [UIHint("SubscriptionType")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.SubscriptionCostLabel)]
        public MembershipOption.ESubscriptionType SubscriptionType { get; set; }

        public string Details => GetDetails();

        public PromoCode()
        {
            Code = $"9STAR{GetCode()}";
        }

        private string GetDetails()
        {
            var sb = new StringBuilder();
            if (SubscriptionType > MembershipOption.ESubscriptionType.Free)
            {
                switch (SubscriptionType)
                {
                    case MembershipOption.ESubscriptionType.MonthlyStandard:
                        sb.Append($"<h4><strong>{Globalisation.Dictionary.MonthlyStandardMembership}</strong></h4>");
                        sb.Append(Globalisation.Dictionary.standard_membership_description);
                        break;

                    case MembershipOption.ESubscriptionType.MonthlyPlatinum:
                        sb.Append($"<h4><strong>{Globalisation.Dictionary.MonthlyPlatinumMembership}</strong></h4>");
                        sb.Append(K9.Globalisation.Dictionary.platinum_membership_description);
                        break;

                    case MembershipOption.ESubscriptionType.AnnualStandard:
                        sb.Append($"<h4><strong>{Globalisation.Dictionary.AnnualStandardMembership}</strong></h4>");
                        sb.Append(K9.Globalisation.Dictionary.standard_membership_description);
                        break;

                    case MembershipOption.ESubscriptionType.AnnualPlatinum:
                        sb.Append($"<h4><strong>{Globalisation.Dictionary.AnnualPlatinumMembership}</strong></h4>");
                        sb.Append(K9.Globalisation.Dictionary.platinum_membership_description);
                        break;

                }
            }

            if (Credits > 0)
            {
                sb.Append($"<h4><strong>{Globalisation.Dictionary.Credits}:</strong></h4>");
                sb.Append($"<p>{Credits}</p>");
            }

            return sb.ToString();
        }

        private string GetCode()
        {
            var sb = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < 5; i++)
            {
                var number = random.Next(0, 26);
                char letter = (char)('A' + number);
                sb.Append(letter);
            }

            return sb.ToString();
        }
    }
}
