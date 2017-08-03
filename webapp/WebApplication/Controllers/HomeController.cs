using System.Web.Mvc;
using K9.SharedLibrary.Models;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class HomeController : BaseController
	{

		public HomeController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles)
			: base(logger, dataSetsHelper, roles)
		{
		}

		public ActionResult Index()
		{
			return View();
		}
		
		public override string GetObjectName()
		{
			return string.Empty;
		}
	}
}
