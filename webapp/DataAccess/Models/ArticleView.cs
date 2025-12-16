using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Globalisation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Dictionary), ListName = Strings.Names.ArticleViews, PluralName = Strings.Names.ArticleViews, Name = Strings.Names.ArticleView)]
    public class ArticleView : ObjectBase
    {

        [Required]
        [ForeignKey("Article")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Article)]
        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }

        [Required]
        [ForeignKey("User")]
        [Display(ResourceType = typeof(Base.Globalisation.Dictionary), Name = Base.Globalisation.Strings.Names.User)]
        public int UserId { get; set; }

        public virtual User User { get; set; }
        
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.PostedOn)]
        public DateTime ViewedOn { get; set; } = DateTime.UtcNow;
    }
}
