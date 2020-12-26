using System;
using System.Collections.Generic;
using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Extensions;
using K9.Base.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;

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
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.MonthlyStandardMembership)]
            MonthlyStandard = 1,
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.AnnualStandardMembership)]
            AnnualStandard = 2,
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.MonthlyPlatinumMembership)]
            MonthlyPlatinum = 10,
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.AnnualPlatinumMembership)]
            AnnualPlatinum = 11
        }

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

        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionCostLabel)]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MaxNumberOfProfileReadingsLabel)]
        public int MaxNumberOfProfileReadings { get; set; }

        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MaxNumberOfCompatibilityReadingsLabel)]
        public int MaxNumberOfCompatibilityReadings { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionCostLabel)]
        public string FormattedPrice => Price.ToString("C0", CultureInfo.GetCultureInfo("en-US"));

        public string CssClassName => GetCssClassName();

        public string MembershipMedalElement => GetMembershipMedalElement();

        public string MembershipMedalElementLocal => GetLocalisedPropertyValue(nameof(MembershipMedalElement));

        public string MembershipPeriod => SubscriptionType == ESubscriptionType.AnnualPlatinum ||
                                                SubscriptionType == ESubscriptionType.AnnualStandard
            ? "Annual"
            : "Monthly";

        public string MembershipPeriodLocal => GetLocalisedPropertyValue(nameof(MembershipPeriod));

        public bool IsFree => SubscriptionType == ESubscriptionType.Free;

        public bool IsMonthly =>
            new[] { ESubscriptionType.MonthlyPlatinum, ESubscriptionType.MonthlyStandard }.Contains(SubscriptionType);

        public bool IsAnnual =>
            new[] { ESubscriptionType.AnnualPlatinum, ESubscriptionType.AnnualStandard }.Contains(SubscriptionType);

        public bool IsUpgradable => SubscriptionType < ESubscriptionType.AnnualPlatinum;

        public bool IsUnlimited => SubscriptionType == ESubscriptionType.AnnualPlatinum || SubscriptionType == ESubscriptionType.MonthlyPlatinum;

        public bool CanUpgradeTo(MembershipOption membershipOption)
        {
            if (SubscriptionType < membershipOption?.SubscriptionType)
            {
                if (SubscriptionType == ESubscriptionType.AnnualStandard &&
                    membershipOption.SubscriptionType == ESubscriptionType.MonthlyPlatinum)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private string GetCssClassName()
        {
            if (SubscriptionType == ESubscriptionType.AnnualPlatinum ||
                SubscriptionType == ESubscriptionType.MonthlyPlatinum)
            {
                return "platinum";
            }

            if (SubscriptionType == ESubscriptionType.AnnualStandard ||
                SubscriptionType == ESubscriptionType.MonthlyStandard)
            {
                return "standard";
            }

            return "free";
        }

        private string GetMembershipMedalElement()
        {
            if (SubscriptionType == ESubscriptionType.AnnualPlatinum ||
                SubscriptionType == ESubscriptionType.MonthlyPlatinum)
            {
                return "PlatinumMembership";
            }

            if (SubscriptionType == ESubscriptionType.AnnualStandard ||
                SubscriptionType == ESubscriptionType.MonthlyStandard)
            {
                return "StandardMembership";
            }

            return "FreeMembership";
        }
    }
}