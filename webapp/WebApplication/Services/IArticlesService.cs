using K9.DataAccessLayer.Models;
using System.Collections.Generic;
using K9.DataAccessLayer.Enums;
using K9.WebApplication.ViewModels;

namespace K9.WebApplication.Services
{
    public interface IArticlesService
    {
        Article GetArticle(int id);
        List<Article> GetArticles(bool publishedOnly = false);
        List<Tag> GetAllTags();
        List<ArticleComment> GetArticleComments(int articleId, ECommentFilter filter = ECommentFilter.ApprovedOnly);
        void SaveArticle(Article article);
        void CreateArticle(Article article);
        void DeleteArticle(int id);
        void CreateArticleComment(ArticleComment comment);
        void CreateArticleView(int id);
        (int Count, bool ToggleState, string LikeSummary) ToggleLike(int articleId, int? articleCommentId = null);
        void DeleteComment(int id);
        void EditComment(int id, string comment);
        void RejectComment(int id);
        void ApproveComment(int id);
        BlogModeratorViewModel GetDashboardViewModel();
    }
}