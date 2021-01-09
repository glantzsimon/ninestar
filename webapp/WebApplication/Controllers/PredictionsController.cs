using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using NLog;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class PredictionsController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;

        public PredictionsController (ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _nineStarKiService = nineStarKiService;
        }

        [Route("predictions")]
        public ActionResult Index()
        {
            return View(_nineStarKiService.GetNineStarKiSummaryViewModel());
        }
            
       
        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}
