using System.Web.Mvc;
using K9.DataAccess.Models;

namespace K9.WebApplication.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Test()
		{
			return View(new UserAccount.LoginModel()
			{
				UserName = "sglantz"
			});
		}
	}
}
