using System;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System.Web.Mvc;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Helpers;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("latest-articles")]
    public partial class BlogController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;
        private readonly IArticlesService _articlesService;

        public BlogController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService, IArticlesService articlesService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
            _articlesService = articlesService;
        }

        [Route("")]
        public ActionResult Index()
        {
            return View(new BlogViewModel
            {
                Articles = _articlesService.GetArticles(true),
                Tags = _articlesService.GetAllTags()
            });
        }

        public ActionResult View(int id)
        {
            var article = _articlesService.GetArticle(id);
            TempData.Keep();
            return RedirectToAction("Details", new { id = article.Id, slug = article.Slug });
        }

        [Route("{id:int}/{slug?}")]
        public ActionResult Details(int id, string slug = null)
        {
            var article = _articlesService.GetArticle(id);
            if (article == null || (!article.PublishedOn.HasValue && !SessionHelper.CurrentUserIsAdmin()))
            {
                return HttpNotFound();
            }

            var correctSlug = article.Title.Slugify();
            if (!string.Equals(slug, correctSlug, StringComparison.OrdinalIgnoreCase))
            {
                // Redirect to canonical URL with slug
                return RedirectToRoutePermanent("Default", new { controller = "Blog", action = "Details", id, slug = correctSlug });
            }

            return View(article);
        }

        [Route("post-comment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult PostComment(ArticleComment model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetErrorMessage();
                TempData["ErrorMessage"] = error ?? Dictionary.FriendlyErrorMessage;
                TempData["IsError"] = true;
                return RedirectToAction("View", new { id = model.ArticleId });
            }

            var comment = new ArticleComment
            {
                ArticleId = model.ArticleId,
                UserId = Current.UserId,
                Comment = model.Comment,
                IsApproved = false
            };

            try
            {
                _articlesService.CreateArticleComment(comment);
            }
            catch (Exception e)
            {
                Logger.Error(e.GetFullErrorMessage());
                TempData["ErrorMessage"] = Dictionary.FriendlyErrorMessage;
                TempData["IsError"] = true;
                return RedirectToAction("View", new { id = model.ArticleId });
            }

            TempData["IsSuccess"] = true;
            return RedirectToAction("View", new { id = model.ArticleId });
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}