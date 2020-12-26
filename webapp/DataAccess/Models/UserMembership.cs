using System;
using System.Collections.Generic;
using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.SharedLibrary.Extensions;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Strings.Grammar.MasculineDefiniteArticle, IndefiniteArticleName = Strings.Grammar.MasculineIndefiniteArticle)]
    [Name(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.MembershipOption, PluralName = Globalisation.Strings.Names.MembershipOptions)]
    [DefaultPermissions(Role = RoleNames.DefaultUsers)]
    public class UserMembership : ObjectBase, IUserData
    {
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("MembershipOption")]
        public int MembershipOptionId { get; set; }

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.StartsOnLabel)]
        public DateTime StartsOn { get; set; }

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EndsOnLabel)]
        public DateTime EndsOn { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.AutoRenewLabel)]
        public bool IsAutoRenew { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.DeactivatedLabel)]
        public bool IsDeactivated { get; set; }

        public virtual User User { get; set; }

        public virtual MembershipOption MembershipOption { get; set; }

        public virtual ICollection<UserProfileReading> ProfileReadings { get; set; }

        public virtual ICollection<UserRelationshipCompatibilityReading> RelationshipCompatibilityReadings { get; set; }

        [LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
        public string UserName { get; set; }

        [LinkedColumn(LinkedTableName = "MembershipOption", LinkedColumnName = "Description")]
        public string MembershipOptionName { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.NumberOfProfileReadingsLeft)]
        public int NumberOfProfileReadingsLeft => MembershipOption.MaxNumberOfProfileReadings - ProfileReadings?.Count ?? 0;

        [Display(ResourceType = typeof(Globalisation.Dictionary),
            Name = Globalisation.Strings.Labels.NumberOfProfileReadingsLeft)]
        public string NumberOfProfileReadingsLeftText => MembershipOption.IsUnlimited
            ? Globalisation.Dictionary.Unlimited : NumberOfProfileReadingsLeft.ToString();

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.NumberOfRelationshipCompatibilityReadingsLeft)]
        public int NumberOfRelationshipCompatibilityReadingsLeft => MembershipOption.MaxNumberOfCompatibilityReadings - RelationshipCompatibilityReadings?.Count ?? 0;

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.NumberOfRelationshipCompatibilityReadingsLeft)]
        public string NumberOfRelationshipCompatibilityReadingsLeftText => MembershipOption.IsUnlimited
            ? Globalisation.Dictionary.Unlimited : NumberOfRelationshipCompatibilityReadingsLeft.ToString();

        [NotMapped] 
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.NumberOfRelationshipCompatibilityReadingsLeft)]
        public int NumberOfCredits { get; set; }

        public bool IsActive => DateTime.Today.IsBetween(StartsOn.Date, EndsOn.Date) && !IsDeactivated;

        public TimeSpan Duration => EndsOn.Subtract(StartsOn);

        public double CostOfRemainingActiveSubscription => GetCostOfRemainingActiveSubscription();

        private double GetCostOfRemainingActiveSubscription()
        {
            var timeRemaining = EndsOn.Subtract(DateTime.Today);
            var percentageRemaining = (double)timeRemaining.Ticks / (double)Duration.Ticks;
            return MembershipOption.Price * percentageRemaining;
        }
    }
}
