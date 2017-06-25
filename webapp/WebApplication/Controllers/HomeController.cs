using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Unauthorized()
		{
			return View();
		}
	}
}
