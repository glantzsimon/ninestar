using System;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using NLog;
using System.Web.Mvc;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;

namespace K9.WebApplication.Controllers
{
    public class KnowledgeBaseController : BaseNineStarKiController
    {
        private readonly IAuthentication _authentication;
        private readonly INineStarKiService _nineStarKiService;
        private readonly IBiorhythmsService _biorhythmsService;

        public KnowledgeBaseController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService, IBiorhythmsService biorhythmsService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _authentication = authentication;
            _nineStarKiService = nineStarKiService;
            _biorhythmsService = biorhythmsService;
        }

        [Route("learn")]
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
