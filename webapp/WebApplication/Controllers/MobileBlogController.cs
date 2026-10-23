using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/blog")]
    public class MobileBlogController : MobileApiControllerBase
    {
        private readonly IArticlesService _articles;
        private readonly IRepository<ArticleComment> _comments;
        public MobileBlogController(INineStarKiPackage package, IArticlesService articles, IRepository<ArticleComment> comments) : base(package)
        { _articles = articles; _comments = comments; }

        [HttpGet, Route("")]
        public ActionResult Index(int offset = 0, int limit = 20)
        {
            if (!ModelState.IsValid || offset < 0 || limit < 1 || limit > 100) return ApiError(400, "invalid_page");
            return ApiSuccess(_articles.GetArticles(true).Skip(offset).Take(limit).Select(e => ArticleData(e, false)).ToArray());
        }

        [HttpGet, Route("article")]
        public ActionResult Article(int id)
        {
            var article = _articles.GetArticle(id);
            if (article == null || (!article.IsPublished && !Package.UserService.UserIsAdmin(GetAuthenticatedUserId())))
                return ApiError(404, "article_not_found");
            return ApiSuccess(new
            {
                Article = ArticleData(article, true),
                Comments = _articles.GetArticleComments(id, ECommentFilter.ApprovedOnly).Select(CommentData).ToArray()
            });
        }

        [HttpGet, Route("tags")]
        public ActionResult Tags() => ApiSuccess(_articles.GetAllTags().Select(e => new { e.Id, e.Name }).ToArray());

        [HttpPost, Route("comment")]
        public ActionResult Comment(int articleId, string comment)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!ValidComment(comment)) return ApiError(400, "invalid_comment");
            var article = _articles.GetArticle(articleId);
            if (article == null || !article.IsPublished) return ApiError(404, "article_not_found");
            var entity = new ArticleComment { ArticleId = articleId, UserId = id, Comment = comment.Trim(), IsApproved = false };
            _articles.CreateArticleComment(entity);
            return ApiSuccess(new { entity.Id, AwaitingModeration = true });
        }

        [HttpPost, Route("comment/edit")]
        public ActionResult EditComment(int commentId, string comment)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!ValidComment(comment)) return ApiError(400, "invalid_comment");
            var entity = _comments.Find(commentId);
            if (entity == null || entity.UserId != id) return ApiError(404, "comment_not_found");
            _articles.EditComment(commentId, comment);
            return ApiSuccess(new { Updated = true });
        }

        [HttpPost, Route("comment/delete")]
        public ActionResult DeleteComment(int commentId)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            var entity = _comments.Find(commentId);
            if (entity == null || entity.UserId != id) return ApiError(404, "comment_not_found");
            _articles.DeleteComment(commentId);
            return ApiSuccess(new { Deleted = true });
        }

        [HttpPost, Route("like")]
        public ActionResult Like(int articleId, int? commentId = null)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            var article = _articles.GetArticle(articleId);
            if (article == null || !article.IsPublished) return ApiError(404, "article_not_found");
            if (commentId.HasValue)
            {
                var comment = _comments.Find(commentId.Value);
                if (comment == null || comment.ArticleId != articleId || !comment.IsApproved || comment.IsRejected)
                    return ApiError(404, "comment_not_found");
            }
            var result = _articles.ToggleLike(articleId, commentId);
            return ApiSuccess(new { result.Count, result.ToggleState, result.LikeSummary });
        }

        [HttpGet, Route("admin/moderation")]
        public ActionResult Moderation()
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            return ApiSuccess(_articles.GetDashboardViewModel().Comments.Select(CommentData).ToArray());
        }

        [HttpPost, Route("admin/moderate")]
        public ActionResult Moderate(int commentId, string decision)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            var entity = _comments.Find(commentId);
            if (entity == null) return ApiError(404, "comment_not_found");
            if (decision == "approve") _articles.ApproveComment(commentId);
            else if (decision == "reject") _articles.RejectComment(commentId);
            else if (decision == "delete")
            {
                _articles.DeleteComment(commentId);
                return ApiSuccess(new { Deleted = true });
            }
            else return ApiError(400, "invalid_moderation_decision");
            return ApiSuccess(CommentData(_comments.Find(commentId)));
        }

        [HttpPost, Route("admin/comment/edit")]
        public ActionResult ModerateEdit(int commentId, string text)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!ValidComment(text)) return ApiError(400, "invalid_comment");
            if (_comments.Find(commentId) == null) return ApiError(404, "comment_not_found");
            _articles.EditComment(commentId, text);
            return ApiSuccess(CommentData(_comments.Find(commentId)));
        }

        private static bool ValidComment(string text) => !string.IsNullOrWhiteSpace(text) && text.Length <= 500;

        private static object ArticleData(Article article, bool includeBody) => new
        {
            article.Id, article.Title, article.Author, article.Slug, article.ImageUrl, article.Summary,
            HtmlBody = includeBody ? article.HtmlBody : null, article.PublishedOn, article.LikeCount, article.ViewCount,
            article.IsLikedByCurrentUser, article.LikeSummary,
            Tags = article.Tags?.Select(e => new { e.Id, e.Name }).ToArray()
        };

        private static object CommentData(ArticleComment comment) => new
        {
            comment.Id, comment.ArticleId, comment.Comment, comment.PostedOn, comment.IsApproved, comment.IsRejected,
            Author = comment.User?.FirstName, comment.AvatarImageUrl, comment.LikeCount, comment.IsLikedByCurrentUser, comment.LikeSummary
        };
    }
}
