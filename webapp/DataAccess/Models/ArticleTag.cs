using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Globalisation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Dictionary), ListName = Strings.Names.ArticleTags, PluralName = Strings.Names.ArticleTags, Name = Strings.Names.ArticleTag)]
    public class ArticleTag : ObjectBase
    {
        
        [Required]
        [ForeignKey("Article")]
        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }

        [Required]
        [ForeignKey("Tag")]
        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
