using System.Web.Mvc;
using K9.SharedLibrary.Models;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class HomeController : BaseController
	{
		public ActionResult Index()
		{
			return View();
		}

		public HomeController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles) : base(logger, dataSetsHelper, roles)
		{
		}

		public override string GetObjectName()
		{
			return string.Empty;
		}
	}
}
