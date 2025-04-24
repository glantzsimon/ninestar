using K9.Base.DataAccessLayer.Attributes;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Globalisation.Dictionary), ListName = Globalisation.Strings.Names.Articles, PluralName = Globalisation.Strings.Names.Articles, Name = Globalisation.Strings.Names.Article)]
    public class Article : BaseLikeable, ILikeable
    {

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.PublishedOnLabel)]
        public DateTime? PublishedOn { get; set; }

        public bool IsPublished => PublishedOn.HasValue;

        public string PublishedOnText => PublishedOn.HasValue ? PublishedOn.Value.ToString("MMMM d, yyyy") : "";

        public string PublishedByText =>
            TemplateParser.Parse(Globalisation.Dictionary.PublishedOnBy, new { PublishedOnText, Author });

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TitleLabel)]
        public string Title { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.AuthorLabel)]
        public string Author { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.SlugLabel)]
        public string Slug { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.ImageUrlLabel)]
        public string ImageUrl { get; set; }

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

        [NotMapped]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.ArticleComments)]
        public List<ArticleComment> Comments { get; set; }
        
    }
}
