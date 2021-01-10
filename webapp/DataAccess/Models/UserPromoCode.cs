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
    [Name(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Credit, PluralName = Globalisation.Strings.Names.Credits)]
    [DefaultPermissions(Role = RoleNames.DefaultUsers)]
    public class UserPromoCode : ObjectBase, IUserData
    {
        [UIHint("PromoCode")]
        [Required]
        [ForeignKey("PromoCode")]
        public int PromoCodeId { get; set; }

        [UIHint("User")]
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual PromoCode PromoCode { get; set; }

        [LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
        public string UserName { get; set; }

        [LinkedColumn(LinkedTableName = "PromoCode", LinkedColumnName = "Code")]
        public string PromoCodeName { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        public string SubscriptionTypeName => PromoCode.SubscriptionType > 0 ? PromoCode.SubscriptionType.GetLocalisedLanguageName() : "";

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.NumberOfCreditsLabel)]
        public int? Credits => PromoCode?.Credits;
    }
}
