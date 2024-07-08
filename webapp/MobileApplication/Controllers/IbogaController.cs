using K9.Base.WebApplication.Controllers;
using K9.SharedLibrary.Models;
using NLog;
using System.Web.Mvc;
using K9.SharedLibrary.Helpers;

namespace K9.WebApplication.Controllers
{
    public class IbogaController : BaseController
	{

		public IbogaController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper)
			: base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
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
