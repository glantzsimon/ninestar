using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using NLog;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class PredictionsController : BaseNineStarKiController
    {
        public PredictionsController (ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
        }

        [Route("predictions")]
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
