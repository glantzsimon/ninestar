using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using NLog;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class ErrorController : BaseNineStarKiController
	{

	    public ErrorController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IMembershipService membershipService)
	        : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
	    {
	    }

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

	    public override string GetObjectName()
	    {
	        return string.Empty;
	    }
    }
}
