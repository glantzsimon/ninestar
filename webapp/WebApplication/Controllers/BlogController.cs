using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System.Web.Mvc;

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

        [Route("{id:int}")]
        public ActionResult Details(int id)
        {
            var article = _articlesService.GetArticle(id);
            if (article != null)
            {
                if (article.PublishedOn.HasValue)
                {
                    return View(article);
                }
            }
            return HttpNotFound();
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

