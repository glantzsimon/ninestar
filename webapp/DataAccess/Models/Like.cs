using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Globalisation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Dictionary), ListName = Strings.Names.Likes, PluralName = Strings.Names.Likes, Name = Strings.Names.Like)]
    public class Like : ObjectBase, IValidatableObject
    {
        [ForeignKey("Article")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Article)]
        public int? ArticleId { get; set; }

        public virtual Article Article { get; set; }

        [ForeignKey("ArticleComment")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.ArticleComment)]
        public int? ArticleCommentId { get; set; }

        public virtual ArticleComment ArticleComment { get; set; }

        [Required]
        [ForeignKey("User")]
        [Display(ResourceType = typeof(Base.Globalisation.Dictionary), Name = Base.Globalisation.Strings.Names.User)]
        public int UserId { get; set; }

        public virtual User User { get; set; }
        
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.PostedOn)]
        public DateTime LikedOn  { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; }
        
        public string Username => User.FirstName ?? User.Username;

        #region Validation

        new public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!ArticleId.HasValue && !ArticleCommentId.HasValue)
            {
                yield return new ValidationResult("A Like must be associated with eithe an Article or an Article Comment", new[] { nameof(Like.ArticleId) });
                yield return new ValidationResult("A Like must be associated with eithe an Article or an Article Comment", new[] { nameof(Like.ArticleCommentId) });
            }
            
            base.Validate(validationContext);
        }

        #endregion
    }
}
