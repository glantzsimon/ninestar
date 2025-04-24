using System.ComponentModel.DataAnnotations.Schema;
using K9.Base.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;

namespace K9.DataAccessLayer.Models
{
    public abstract class BaseLikeable : ObjectBase, ILikeable
    {
        [NotMapped]
        public int LikeCount { get; set; }

        [NotMapped]
        public bool IsLikedByCurrentUser { get; set; }

        public string LikeSummary
        {
            get
            {
                if (IsLikedByCurrentUser && LikeCount == 1)
                    return Dictionary.YouLikeThis;

                if (IsLikedByCurrentUser && LikeCount > 1)
                {
                    var otherText = Dictionary.Other.ToLower();
                    return TemplateParser.Parse(Dictionary.YouAndOthersLikeThis, new
                    {
                        LikeCount = LikeCount - 1,
                        OtherText = LikeCount > 2 ? otherText.Pluralise() : otherText
                    });
                }

                if (LikeCount > 0)
                {
                    var likesText = (LikeCount > 1 ? Dictionary.Likes : Dictionary.Like).ToLower();
                    return $"{LikeCount} {likesText}";
                }

                return Dictionary.NoLikesYet;
            }
        }
    }
}
