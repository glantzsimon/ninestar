using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers.Html;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/admin/articles")]
    public class MobileAdminArticlesController : MobileApiControllerBase
    {
        private readonly IArticlesService _articles;
        private readonly IRepository<Article> _repository;
        public MobileAdminArticlesController(INineStarKiPackage package, IArticlesService articles, IRepository<Article> repository) : base(package)
        { _articles = articles; _repository = repository; }

        [HttpGet, Route("")]
        public ActionResult List(int afterId = 0, int limit = 50)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!ModelState.IsValid || afterId < 0 || limit < 1 || limit > 100) return ApiError(400, "invalid_page");
            return ApiSuccess(_articles.GetArticles().Where(e => e.Id > afterId).OrderBy(e => e.Id).Take(limit).Select(e => new
            {
                e.Id, e.Title, e.Summary, e.Slug, e.ImageUrl, e.PublishedOn, e.Author, e.TagsText
            }).ToArray());
        }

        [HttpPost, Route("save"), ValidateInput(false)]
        public ActionResult Save(MobileArticleWriteRequest request)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!ValidInput(request) || (request.Tags?.Length ?? 0) > 50 ||
                request.Tags?.Any(e => string.IsNullOrWhiteSpace(e) || e.Length > 100) == true)
                return ApiError(400, "invalid_article");
            var article = request.Id.HasValue ? _articles.GetArticle(request.Id.Value) : new Article();
            if (article == null) return ApiError(404, "article_not_found");
            article.Title = request.Title;
            article.Summary = request.Summary;
            article.HtmlBody = request.HtmlBody;
            article.ImageUrl = request.ImageUrl;
            article.TagsText = JsonConvert.SerializeObject((request.Tags ?? new string[0]).Select(e => new { Value = e }));
            HtmlMarkupHelper.ConvertToHtml(ref article);
            if (request.Id.HasValue) _articles.SaveArticle(article); else _articles.CreateArticle(article);
            return ApiSuccess(new { article.Id, article.Title, article.Slug, article.PublishedOn });
        }

        [HttpPost, Route("publication")]
        public ActionResult Publication(int articleId, bool published)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            var article = _repository.Find(articleId);
            if (article == null) return ApiError(404, "article_not_found");
            article.PublishedOn = published ? DateTime.UtcNow : (DateTime?)null;
            _repository.Update(article);
            return ApiSuccess(new { article.Id, article.PublishedOn });
        }

        [HttpPost, Route("delete")]
        public ActionResult Delete(int articleId)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (_repository.Find(articleId) == null) return ApiError(404, "article_not_found");
            _articles.DeleteArticle(articleId);
            return ApiSuccess(new { Deleted = true });
        }
    }
}
