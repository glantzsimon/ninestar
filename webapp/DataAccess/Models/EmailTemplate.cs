using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Enums;
using K9.SharedLibrary.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace K9.DataAccessLayer.Models
{
    [Name(ResourceType = typeof(Globalisation.Dictionary), ListName = Globalisation.Strings.Names.EmailTemplates, PluralName = Globalisation.Strings.Names.EmailTemplates, Name = Globalisation.Strings.Names.EmailTemplate)]
    public class EmailTemplate : ObjectBase
    {
        [Required]
        [StringLength(256)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.SubjectLabel)]
        public string Subject { get; set; }

        public ESystemEmailTemplate SystemEmailTemplate { get; set; }

        [Required]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.BodyLabel)]
        public string HtmlBody { get; set; }

        [UIHint("MembershipOption")]
        [ForeignKey("MembershipOption")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        public int? MembershipOptionId { get; set; }

        public virtual MembershipOption MembershipOption { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.SubscriptionTypeLabel)]
        [LinkedColumn(LinkedTableName = "MembershipOption", LinkedColumnName = "Name")]
        public string MembershipOptionName { get; set; }

        [UIHint("Promotion")]
        [ForeignKey("Promotion")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.PromotionLabel)]
        public int? PromotionId { get; set; }

        public virtual Promotion Promotion { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.PromotionLabel)]
        [LinkedColumn(LinkedTableName = "Promotion", LinkedColumnName = "Name")]
        public string PromotionName { get; set; }
    }
}
