using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("blog")]
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

        public ActionResult Index()
        {
            ViewBag.AllTags = _articlesService.GetAllTags();
            return View(_articlesService.GetArticles());
        }

        public ActionResult Details(int id)
        {
            return View(_articlesService.GetArticle(id));
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

