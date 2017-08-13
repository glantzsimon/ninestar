using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.WebApplication.UnitsOfWork;

namespace K9.WebApplication.Controllers
{
	public class NewsController : BaseController<NewsItem>
	{
		public NewsController(IControllerPackage<NewsItem> controllerPackage)
			: base(controllerPackage)
		{
		}
		
		public override ActionResult Index()
		{
			return View(Repository.List());
		}
		
	}
}
