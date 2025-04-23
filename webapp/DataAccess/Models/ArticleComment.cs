using System;
using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Globalisation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Dictionary), ListName = Strings.Names.ArticleComments, PluralName = Strings.Names.ArticleComments, Name = Strings.Names.ArticleComment)]
    public class ArticleComment : ObjectBase
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

        [UIHint("Comments")]
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.Comment)]
        [StringLength(500, ErrorMessage = "Comments must be 500 characters or fewer.")]
        public string Comment { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.PostedOn)]
        public DateTime PostedOn { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; }

        [NotMapped]
        public bool IsByModerator { get; set; }

        [NotMapped]
        public UserInfo UserInfo { get; set; }

        [NotMapped]
        public int LikeCount { get; set; }

        [NotMapped]
        public bool IsLikedByCurrentUser { get; set; }

        public string Username => User.FirstName ?? User.Username;

        public string AvatarImageUrl => UserInfo?.AvatarImageUrl;

        public string LikeSummary
        {
            get
            {
                if (IsLikedByCurrentUser && LikeCount == 1)
                    return "You like this";

                if (IsLikedByCurrentUser && LikeCount > 1)
                    return $"You and {LikeCount - 1} other{(LikeCount > 2 ? "s" : "")}";

                if (LikeCount > 0)
                    return $"{LikeCount} like{(LikeCount > 1 ? "s" : "")}";

                return string.Empty;
            }
        }

    }
}
