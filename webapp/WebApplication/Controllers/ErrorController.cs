using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
	public class ErrorController : Controller
	{
		public ActionResult Index()
		{
			return View("FriendlyError");
		}

		public ActionResult NotFound()
		{
			return View("NotFound");
		}

		public ActionResult Unauthorized()
		{
			return View("Unauthorized");
		}
	}
}
