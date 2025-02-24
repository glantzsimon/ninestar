using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Extensions;
using K9.Base.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace K9.DataAccessLayer.Models
{
    [Grammar(ResourceType = typeof(Base.Globalisation.Dictionary), DefiniteArticleName = Base.Globalisation.Strings.Grammar.DefiniteArticleWithApostrophe, IndefiniteArticleName = Base.Globalisation.Strings.Grammar.FeminineIndefiniteArticle)]
    [Name(ResourceType = typeof(Dictionary), ListName = Strings.Names.MembershipOptions, PluralName = Strings.Names.MembershipOptions, Name = Strings.Names.Donation)]
    [Description(UseLocalisedString = true, ResourceType = typeof(Dictionary))]
    [DefaultPermissions(Role = RoleNames.PowerUsers)]
    public class MembershipOption : ObjectBase
    {
        public const int Unlimited = int.MaxValue;

        public enum ESubscriptionType
        {
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Free)]
            Free = 0,
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.WeeklyPlatinumMembership)]
            WeeklyPlatinum = 9,
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.MonthlyPlatinumMembership)]
            MonthlyPlatinum = 10,
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.AnnualPlatinumMembership)]
            AnnualPlatinum = 11,
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.LifeTimePlatinumMembership)]
            LifeTimePlatinum = 12
        }

        [UIHint("SubscriptionType")]
        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MembershipLabel)]
        public ESubscriptionType SubscriptionType { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MembershipLabel)]
        public string SubscriptionTypeNameLocal => SubscriptionType > 0 ? SubscriptionType.GetLocalisedLanguageName() : "";

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionDetailsLabel)]
        [Required(ErrorMessageResourceType = typeof(Base.Globalisation.Dictionary), ErrorMessageResourceName = Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        public string SubscriptionDetails { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionDetailsLabel)]
        public string SubscriptionDetailsLocal => GetLocalisedPropertyValue(nameof(SubscriptionDetails));

        public string GetSubscriptionDetails() => Promotion == null
            ? SubscriptionTypeNameLocal
            : $"{SubscriptionTypeNameLocal} ({Dictionary.SpecialPromotion})";

        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionCostLabel)]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionCostLabel)]
        [DataType(DataType.Currency)]
        public double PriceIncludingDiscountForRemainingPreviousSubscription { get; set; }

        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NumberOfProfileReadingsLabel)]
        public int NumberOfProfileReadings { get; set; }

        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NumberOfCompatibilityReadingsLabel)]
        public int NumberOfCompatibilityReadings { get; set; }

        [NotMapped] public Promotion Promotion { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionCostLabel)]
        public string FormattedPrice => Price.ToString("C0", CultureInfo.GetCultureInfo("en-US"));

        public string GetDiscountText() => Promotion != null ? $"({Dictionary.SpecialPromotion})" : "";

        public string CssClassName => GetCssClassName();

        public string MembershipMedalElement => GetMembershipMedalElement();

        public string MembershipMedalElementLocal => GetLocalisedPropertyValue(nameof(MembershipMedalElement));

        public string MembershipPeriod => GetMembershipPeriod();

        public string MembershipPeriodLocal => GetLocalisedPropertyValue(nameof(MembershipPeriod));

        public bool IsFree => SubscriptionType == ESubscriptionType.Free;

        public bool IsForever => SubscriptionType == ESubscriptionType.LifeTimePlatinum;

        public bool IsMonthly => SubscriptionType == ESubscriptionType.MonthlyPlatinum;

        public bool IsAnnual => SubscriptionType == ESubscriptionType.AnnualPlatinum;

        public bool IsWeekly =>
            new[] { ESubscriptionType.WeeklyPlatinum }.Contains(SubscriptionType);

        public bool IsUpgradable => SubscriptionType < ESubscriptionType.LifeTimePlatinum;

        public bool IsUnlimited => SubscriptionType >= ESubscriptionType.WeeklyPlatinum;

        public bool CanUpgradeTo(MembershipOption membershipOption)
        {
            return SubscriptionType < membershipOption.SubscriptionType;
        }

        public string SubscriptionTypeText => Name.SplitOnCapitalLetter();

        private string GetCssClassName()
        {
            if (SubscriptionType == ESubscriptionType.AnnualPlatinum ||
                SubscriptionType == ESubscriptionType.MonthlyPlatinum ||
                SubscriptionType == ESubscriptionType.WeeklyPlatinum ||
                SubscriptionType == ESubscriptionType.LifeTimePlatinum)
            {
                return "platinum";
            }
            
            return "free";
        }

        private string GetMembershipMedalElement()
        {
            if (SubscriptionType == ESubscriptionType.AnnualPlatinum ||
                SubscriptionType == ESubscriptionType.MonthlyPlatinum ||
                SubscriptionType == ESubscriptionType.WeeklyPlatinum ||
                SubscriptionType == ESubscriptionType.LifeTimePlatinum)
            {
                return "PlatinumMembership";
            }
            
            return "FreeMembership";
        }

        private string GetMembershipPeriod()
        {
            if (SubscriptionType == ESubscriptionType.AnnualPlatinum)
            {
                return "Yearly";
            }

            if (SubscriptionType == ESubscriptionType.MonthlyPlatinum)
            {
                return "Monthly";
            }

            if (SubscriptionType == ESubscriptionType.WeeklyPlatinum)
            {
                return "Weekly";
            }

            if (SubscriptionType == ESubscriptionType.Free)
            {
                return "Free";
            }

            return "Lifetime";
        }
    }
}