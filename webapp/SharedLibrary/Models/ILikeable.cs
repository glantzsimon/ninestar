
namespace K9.SharedLibrary.Models
{
    public interface ILikeable
    {
        int LikeCount { get; set; }
        bool IsLikedByCurrentUser { get; set; }
        string LikeSummary { get;  }
    }
}
