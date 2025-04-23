using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class ArticlesService : BaseService, IArticlesService
    {
        private readonly IRepository<Article> _articlesRepository;
        private readonly IRepository<Tag> _tagsRepository;
        private readonly IRepository<ArticleTag> _articleTagsRepository;
        private readonly IRepository<ArticleComment> _articleCommentsRepository;
        private readonly IRepository<UserInfo> _userInfosRepository;
        private readonly IRepository<ArticleCommentLike> _articleCommentLikesRepository;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IUserService _userService;

        public ArticlesService(INineStarKiBasePackage my, IRepository<Article> articlesRepository, IRepository<Tag> tagsRepository, IRepository<ArticleTag> articleTagsRepository, IRepository<ArticleComment> articleCommentsRepository, IRepository<UserInfo> userInfosRepository, IRepository<ArticleCommentLike> articleCommentLikesRepository, IEmailTemplateService emailTemplateService, IUserService userService)
            : base(my)
        {
            _articlesRepository = articlesRepository;
            _tagsRepository = tagsRepository;
            _articleTagsRepository = articleTagsRepository;
            _articleCommentsRepository = articleCommentsRepository;
            _userInfosRepository = userInfosRepository;
            _articleCommentLikesRepository = articleCommentLikesRepository;
            _emailTemplateService = emailTemplateService;
            _userService = userService;
        }

        public Article GetArticle(int id)
        {
            var article = _articlesRepository.Find(id);
            if (article != null)
            {
                article.Tags = GetTagsForArticle(id);
                article.TagsText = ConvertTagsToTagsText(article.Tags);
                article.Author = GetAuthor(article.CreatedBy);
                article.Comments = SessionHelper.CurrentUserIsAdmin() ? GetArticleComments(id) : GetArticleComments(id, ECommentFilter.ApprovedOnly);
            }

            return article;
        }

        public List<ArticleComment> GetArticleComments(int articleId, ECommentFilter filter = ECommentFilter.All)
        {
            var article = _articlesRepository.Find(articleId);
            if (article == null)
                return new List<ArticleComment>();

            var comments = _articleCommentsRepository.Find(e => e.ArticleId == articleId);
            switch (filter)
            {
                case ECommentFilter.ApprovedOnly:
                    comments = comments.Where(c => c.IsApproved).ToList();
                    break;
                case ECommentFilter.UnapprovedOnly:
                    comments = comments.Where(c => !c.IsApproved).ToList();
                    break;
            }

            foreach (var comment in comments)
            {
                comment.User = My.UsersRepository.Find(comment.UserId);
                comment.UserInfo = _userInfosRepository.Find(u => u.UserId == comment.UserId).FirstOrDefault();
                comment.Article = article;
                comment.IsByModerator = _userService.UserIsAdmin(comment.UserId);
                comment.LikeCount =
                    _articleCommentLikesRepository.GetCount(
                        $"{nameof(ArticleCommentLike.ArticleCommentId)} =  {comment.Id}");
                comment.IsLikedByCurrentUser = _articleCommentLikesRepository.Exists(e =>
                    e.ArticleCommentId == comment.Id && e.UserId == Current.UserId);
            }

            return comments;
        }

        public int ToggleCommentLike(int articleCommentId)
        {
            var existing = _articleCommentLikesRepository.Find(e => e.ArticleCommentId == articleCommentId && e.UserId == Current.UserId).FirstOrDefault();

            if (existing != null)
            {
                _articleCommentLikesRepository.Delete(articleCommentId);
            }
            else
            {
                _articleCommentLikesRepository.Create(new ArticleCommentLike
                {
                    ArticleCommentId = articleCommentId,
                    UserId = Current.UserId,
                    LikedOn = DateTime.UtcNow
                });
            }

            return _articleCommentLikesRepository.GetCount(
                $"{nameof(ArticleCommentLike.ArticleCommentId)} = {articleCommentId}");
        }

        public List<Article> GetArticles(bool publishedOnly = false)
        {
            var articles = publishedOnly ? _articlesRepository.Find(e => e.PublishedOn.HasValue) : _articlesRepository.List();
            articles = articles.OrderByDescending(e => e.CreatedOn).ToList();
            foreach (var article in articles)
            {
                article.Tags = GetTagsForArticle(article.Id);
                article.TagsText = ConvertTagsToTagsText(article.Tags);
                article.Author = GetAuthor(article.CreatedBy);
            }

            return articles;
        }

        public List<Tag> GetAllTags()
        {
            return _tagsRepository.List().OrderBy(e => e.Name).ToList();
        }

        public void CreateArticle(Article article)
        {
            article.Slug = article.Title.Slugify();
            _articlesRepository.Create(article);
            ProcessTags(article);
        }

        public void SaveArticle(Article article)
        {
            article.Slug = article.Title.Slugify();
            _articlesRepository.Update(article);
            ProcessTags(article);
        }

        public void DeleteArticle(int id)
        {
            var article = GetArticle(id);
            var tagsToDelete = _articleTagsRepository.Find(e => e.ArticleId == id);
            _articleTagsRepository.DeleteBatch(tagsToDelete);
            _articlesRepository.Delete(id);
        }

        public void CreateArticleComment(ArticleComment comment)
        {
            _articleCommentsRepository.Create(comment);
            var user = My.UsersRepository.Find(comment.UserId);
            SendEmailToNineStar(comment, user);
        }

        public void DeleteComment(int id)
        {
            var comment = _articleCommentsRepository.Find(id);
            if (comment == null || comment.UserId != Current.UserId)
                throw new UnauthorizedAccessException();

            _articleCommentsRepository.Delete(id);
        }

        public void EditComment(int id, string comment)
        {
            var entity = _articleCommentsRepository.Find(id);
            if (entity == null || entity.UserId != Current.UserId)
                throw new UnauthorizedAccessException();

            entity.Comment = comment.Trim();
            entity.LastUpdatedOn = DateTime.UtcNow;
            _articleCommentsRepository.Update(entity);
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
            if (string.IsNullOrEmpty(article.TagsText))
                return;

            var tagValues = JsonConvert.DeserializeObject<List<TagValue>>(article.TagsText) ?? new List<TagValue>();

            var slugs = tagValues
                .Select(t => t.Value.Slugify())
                .Distinct()
                .ToList();

            var displayValues = tagValues
                .GroupBy(t => t.Value.Slugify())
                .ToDictionary(
                    g => g.Key,
                    g => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.First().Value.ToLower())
                );

            var existingTags = _tagsRepository.List().Where(t => slugs.Contains(t.Slug)).ToList();
            var existingSlugs = existingTags.Select(t => t.Slug).ToHashSet();
            var missingSlugs = slugs.Where(slug => !existingSlugs.Contains(slug)).ToList();

            _tagsRepository.CreateBatch(missingSlugs.Select(slug => new Tag
            {
                Name = displayValues[slug],
                Slug = slug
            }).ToList());

            _articleTagsRepository.GetQuery($"DELETE FROM {nameof(ArticleTag)} WHERE {nameof(ArticleTag.ArticleId)} = {article.Id}");

            var allTags = _tagsRepository.List().Where(t => slugs.Contains(t.Slug)).ToList();

            _articleTagsRepository.CreateBatch(allTags.Select(tag => new ArticleTag
            {
                ArticleId = article.Id,
                TagId = tag.Id
            }).ToList());
        }

        private string ConvertTagsToTagsText(IEnumerable<Tag> tags)
        {
            var tagValues = tags
                .Where(t => !string.IsNullOrWhiteSpace(t.Name))
                .Select(t => new { value = t.Name })
                .ToList();

            return JsonConvert.SerializeObject(tagValues);
        }

        private string GetAuthor(string username)
        {
            var user = My.UsersRepository.Find(e => e.Username == username).FirstOrDefault();
            return user == null ? username : user.FullName;
        }

        private void SendEmailToNineStar(ArticleComment comment, User user)
        {
            var article = GetArticle(comment.ArticleId);
            var subject = "A customer has commented on an article";
            var body = _emailTemplateService.ParseForUser(
                subject,
                Dictionary.CommentReceivedEmail,
                user,
                new
                {
                    Customer = user.FullName,
                    CustomerEmail = user.EmailAddress,
                    ArticleName = article.Title,
                    CommentText = comment.Comment,
                    LinkToArticle = My.UrlHelper.AbsoluteAction("View", "Blog", new { id = comment.ArticleId }),
                });

            try
            {
                My.Mailer.SendEmail(
                    subject,
                    body,
                    My.WebsiteConfiguration.SupportEmailAddress,
                    My.WebsiteConfiguration.CompanyName);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
            }
        }

    }
}