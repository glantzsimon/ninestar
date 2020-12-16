using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Helpers;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class HomeController : BaseController
    {
        
        public HomeController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            SetBetaWarningSessionVariable();
        }

        public ActionResult Index()
        {
            return View();
        }

        [Route("about")]
        public ActionResult About()
        {
            return View();
        }

        [Route("privacy-policy")]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
        
        public override string GetObjectName()
        {
            return string.Empty;
        }

        private static void SetBetaWarningSessionVariable()
        {
            var numberOfDisplays = Helpers.SessionHelper.GetIntValue(Constants.SessionConstants.BetaWarningDisplay);
            if (numberOfDisplays < 1)
            {
                numberOfDisplays++;
                SessionHelper.SetValue(Constants.SessionConstants.BetaWarningDisplay, numberOfDisplays);
            }
            else
            {
                SessionHelper.SetValue(Constants.SessionConstants.BetaWarningHide, true);
            }
        }
    }
}
