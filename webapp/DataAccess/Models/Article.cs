using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Globalisation.Dictionary), ListName = Globalisation.Strings.Names.Articles, PluralName = Globalisation.Strings.Names.Articles, Name = Globalisation.Strings.Names.Article)]
    public class Article : ObjectBase
    {

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.PublishedOnLabel)]
        public DateTime? PublishedOn { get; set; }

        public bool IsPublished => PublishedOn.HasValue;

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TitleLabel)]
        public string Title { get; set; }

        [UIHint("Body")]
        [Required]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.BodyLabel)]
        public string HtmlBody { get; set; }

    }
}
