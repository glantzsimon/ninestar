using System;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System.Web.Mvc;
using K9.SharedLibrary.Helpers;

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

        public ActionResult Preview(int id)
        {
            var article = _articlesService.GetArticle(id);
            return RedirectToAction("Details", new { id = article.Id, slug = article.Slug });
        }

        [Route("latest-articles/{id:int}/{slug?}", Name = "BlogDetails")]
        public ActionResult Details(int id, string slug = null)
        {
            var article = _articlesService.GetArticle(id);
            if (article == null || !article.PublishedOn.HasValue)
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

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

