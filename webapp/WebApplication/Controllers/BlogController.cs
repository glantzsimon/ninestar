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

        public BlogController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
        }

        public ActionResult Index()
        {
            return View(new NumerologyModel());
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

