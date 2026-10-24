using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace K9.WebApplication.ViewModels.MobileApi
{
    public class MobileArticleWriteRequest
    {
        public int? Id { get; set; }
        [Required, StringLength(500)] public string Title { get; set; }
        [Required, StringLength(4000)] public string Summary { get; set; }
        [Required, AllowHtml, StringLength(50000)] public string HtmlBody { get; set; }
        [StringLength(2000)] public string ImageUrl { get; set; }
        public string[] Tags { get; set; }
    }
}
