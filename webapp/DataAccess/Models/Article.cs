using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public string PublishedOnText => PublishedOn.HasValue ? PublishedOn.Value.ToString("MMMM d, yyyy") : "";

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TitleLabel)]
        public string Title { get; set; }
        
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.SummaryLabel)]
        public string Summary { get; set; }

        [UIHint("Body")]
        [Required]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.BodyLabel)]
        public string HtmlBody { get; set; }

        [UIHint("Tags")]
        [NotMapped]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TagsLabel)]
        public string TagsText { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TagsLabel)]
        public List<Tag> Tags { get; set; }

    }
}
