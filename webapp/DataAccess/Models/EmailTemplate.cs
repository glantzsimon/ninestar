using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        [AllowHtml]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.BodyLabel)]
        public string HtmlBody { get; set; }
        
    }
}
