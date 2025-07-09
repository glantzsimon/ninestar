using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.Base.WebApplication.Extensions;
using K9.DataAccessLayer.Helpers;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Strings.Grammar.MasculineDefiniteArticle, IndefiniteArticleName = Strings.Grammar.MasculineIndefiniteArticle)]
    [Name(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.UserMembership, PluralName = Globalisation.Strings.Names.MembershipOptions, ListName = Globalisation.Strings.Names.UserMembershipsListName)]
    [DefaultPermissions(Role = RoleNames.DefaultUsers)]
    public class UserMembership : ObjectBase, IUserData
    {
        public const int TotalFreeReadings = 2;

        [UIHint("User")]
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [UIHint("MembershipOption")]
        [Required]
        [ForeignKey("MembershipOption")]
        public int MembershipOptionId { get; set; }

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.StartsOnLabel)]
        public DateTime StartsOn { get; set; }

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EndsOnLabel)]
        public DateTime EndsOn { get; set; }

        public string ValidUntil => MembershipOption.IsForever
                ? Globalisation.Dictionary.Forever
                : EndsOn.ToLongLocalDateString();

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.AutoRenewLabel)]
        public bool IsAutoRenew { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.DeactivatedLabel)]
        public bool IsDeactivated { get; set; }

        public virtual User User { get; set; }

        public virtual MembershipOption MembershipOption { get; set; }

        [LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
        public string UserName { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        [LinkedColumn(LinkedTableName = "MembershipOption", LinkedColumnName = "Name")]
        public string MembershipOptionName { get; set; }
        
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.NumberOfProfileReadingsLabel)]
        public int ComplementaryPersonalChartReadingCount { get; set; } = TotalFreeReadings;

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.NumberOfPredictionsReadingsLabel)]
        public int ComplementaryPredictionsReadingCount { get; set; } = TotalFreeReadings;

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.NumberOfCompatibilityReadingsLabel)]
        public int ComplementaryCompatibilityReadingCount { get; set; } = TotalFreeReadings;

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.NumberOfKarmicReadingsLeft)]
        public int ComplementaryKarmicReadingCount { get; set; } = TotalFreeReadings;

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.IsActiveLabel)]
        public bool IsActive => (DateTime.Today.IsBetween(StartsOn.Date, EndsOn.Date) || MembershipOption?.SubscriptionType == MembershipOption.ESubscriptionType.LifeTimePlatinum) && !IsDeactivated;

        public TimeSpan Duration => EndsOn.Subtract(StartsOn);
        
        public bool IsAuthorisedToViewPaidContent() =>
            MembershipOption?.SubscriptionType > MembershipOption.ESubscriptionType.Free && IsActive;

        public bool IsUnlimited() => IsActive && MembershipOption.IsUnlimited;

        public bool IsUnlimitedTrial() =>
            (ComplementaryPredictionsReadingCount + ComplementaryPersonalChartReadingCount) > 0;

        [Display(ResourceType = typeof(Globalisation.Dictionary),
            Name = Globalisation.Strings.Labels.AccountNumberLabel)]
        public string AccountNumber => GetAccountNumber();
        
        private string GetAccountNumber()
        {
            return Name.ToSixDigitCode();
        }

    }
}
