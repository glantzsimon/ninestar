using System;
using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Extensions;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Strings.Grammar.MasculineDefiniteArticle, IndefiniteArticleName = Strings.Grammar.MasculineIndefiniteArticle)]
    [Name(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.UserPromotion, PluralName = Globalisation.Strings.Names.UserPromotions)]
    [DefaultPermissions(Role = RoleNames.Administrators)]
    public class UserPromotion : ObjectBase, IUserData
    {
        [UIHint("Promotion")]
        [Required]
        [ForeignKey("Promotion")]
        public int PromotionId { get; set; }

        [UIHint("User")]
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual Promotion Promotion { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.UserNameLabel)]
        [LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
        public string UserName { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.PromotionLabel)]
        [LinkedColumn(LinkedTableName = "Promotion", LinkedColumnName = "Code")]
        public string PromotionName { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SentOnLabel)]
        public DateTime SentOn { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.UsedOnLabel)]
        public DateTime? UsedOn { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        public string SubscriptionTypeName => Promotion?.MembershipOption?.SubscriptionType > 0 ? Promotion.MembershipOption.SubscriptionType.GetLocalisedLanguageName() : "";
    }
}
