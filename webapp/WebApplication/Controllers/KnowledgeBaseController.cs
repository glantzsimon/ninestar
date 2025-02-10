using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Services;
using NLog;
using System.Web.Mvc;
using K9.Base.DataAccessLayer.Models;

namespace K9.WebApplication.Controllers
{
    public class KnowledgeBaseController : BaseNineStarKiController
    {
        private readonly IAuthentication _authentication;
        private readonly INineStarKiService _nineStarKiService;
        private readonly IBiorhythmsService _biorhythmsService;

        public KnowledgeBaseController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService, IBiorhythmsService biorhythmsService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
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

        [Route("learn/retrieve-last")]
        [Authorize]
        public ActionResult RetrieveLastKnowledgeBaseSection(bool todayOnly = false)
        {
            var lastKnowledgeBaseBookmark = SessionHelper.GetLastKnowledgeBase(todayOnly).Value;
            if (lastKnowledgeBaseBookmark != null)
            {
                return Redirect(Url.Action("Index") + $"#{lastKnowledgeBaseBookmark}");
            }
            return RedirectToAction("Index");
        }
        
        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}
