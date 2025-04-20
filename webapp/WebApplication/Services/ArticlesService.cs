using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class ArticlesService : BaseService, IArticlesService
    {
        private readonly IRepository<Article> _articlesRepository;
        private readonly IRepository<Tag> _tagsRepository;
        private readonly IRepository<ArticleTag> _articleTagsRepository;

        public ArticlesService(INineStarKiBasePackage my, IRepository<Article> articlesRepository, IRepository<Tag> tagsRepository, IRepository<ArticleTag> articleTagsRepository)
            : base(my)
        {
            _articlesRepository = articlesRepository;
            _tagsRepository = tagsRepository;
            _articleTagsRepository = articleTagsRepository;
        }

        public Article GetArticle(int id)
        {
            var article = _articlesRepository.Find(id);
            if (article != null)
            {
                article.Tags = GetTagsForArticle(id);
            }

            return article;
        }

        public List<Article> GetArticles()
        {
            var articles = _articlesRepository.List().OrderByDescending(e => e.CreatedOn).ToList();
            foreach (var article in articles)
            {
                article.Tags = GetTagsForArticle(article.Id);
            }

            return articles;
        }

        public List<string> GetAllTags()
        {
            return _tagsRepository.List().Select(e => e.Name).OrderBy(e => e).ToList();
        }

        public void CreateArticle(Article article)
        {
            _articlesRepository.Create(article);
            ProcessTags(article);
        }

        public void SaveArticle(Article article)
        {
            _articlesRepository.Update(article);
            ProcessTags(article);
        }

        private List<Tag> GetTagsForArticle(int id)
        {
            var tags = _tagsRepository.GetQuery(
                    $"SELECT * FROM {nameof(Tag)} WHERE Id IN " +
                    $"(SELECT {nameof(ArticleTag.TagId)} FROM {nameof(ArticleTag)} WHERE {nameof(ArticleTag.ArticleId)} = {id})")
                .ToList();
            return tags;
        }

        private void ProcessTags(Article article)
        {
            var tagNames = article.TagsText?
                               .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(t => t.Trim().ToLowerInvariant())
                               .Distinct()
                               .ToList() ?? new List<string>();

            var existingTags = _tagsRepository.List().Where(t => tagNames.Contains(t.Name)).ToList();
            var existingTagNames = existingTags.Select(t => t.Name).ToHashSet();
            var missingTagNames = tagNames.Where(t => !existingTagNames.Contains(t)).ToList();

            _tagsRepository.CreateBatch(missingTagNames.Select(e => new Tag { Name = e }).ToList());

            // Clear old tags
            _articleTagsRepository.GetQuery($"DELETE FROM {nameof(ArticleTag)} WHERE {nameof(ArticleTag.ArticleId)} = {article.Id}");

            // Add new tags
            var articleTags = _tagsRepository.List().Where(t => tagNames.Contains(t.Name)).ToList();
            _articleTagsRepository.CreateBatch(articleTags.Select(e => new ArticleTag
            {
                ArticleId = article.Id,
                TagId = e.Id
            }).ToList());
        }
    }
}