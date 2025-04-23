using K9.DataAccessLayer.Models;
using System.Collections.Generic;
using K9.DataAccessLayer.Enums;

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
        void DeleteArtciel(int id);
        void CreateArticleComment(ArticleComment comment);
    }
}