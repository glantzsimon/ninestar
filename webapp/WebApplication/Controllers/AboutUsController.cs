using System.Web.Mvc;
using K9.SharedLibrary.Models;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class AboutUsController : BaseController
	{

		public AboutUsController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles)
			: base(logger, dataSetsHelper, roles)
		{
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult OurMission()
		{
			return View();
		}

		public ActionResult Team()
		{
			return View();
		}

		public override string GetObjectName()
		{
			return string.Empty;
		}
	}
}
