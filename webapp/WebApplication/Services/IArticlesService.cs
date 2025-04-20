using K9.DataAccessLayer.Models;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IArticlesService
    {
        Article GetArticle(int id);
        List<Article> GetArticles();
        List<string> GetAllTags();
        void SaveArticle(Article article);
        void CreateArticle(Article article);
    }
}