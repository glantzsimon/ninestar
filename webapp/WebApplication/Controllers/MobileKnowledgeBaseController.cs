using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/knowledge-base")]
    public class MobileKnowledgeBaseController : MobileApiControllerBase
    {
        private readonly INineStarKiService _profiles;
        public MobileKnowledgeBaseController(INineStarKiPackage package, INineStarKiService profiles) : base(package) { _profiles = profiles; }

        [HttpGet, Route("")]
        public ActionResult Index()
        {
            var userId = GetAuthenticatedUserId();
            var includeMemberHelp = false;
            if (userId > 0)
            {
                var membership = Package.MembershipService.GetActiveUserMembership(userId);
                includeMemberHelp = Package.UserService.UserIsAdmin(userId) ||
                    (membership != null && (membership.IsAuthorisedToViewPaidContent() ||
                        membership.MembershipOption?.SubscriptionType >= K9.DataAccessLayer.Models.MembershipOption.ESubscriptionType.Free));
            }
            var help = MobileHelpMapper.KnowledgeBase(includeMemberHelp,
                Url.Action("Index", "KnowledgeBase", null, Request.Url.Scheme));
            return ApiSuccess(MobileCalculationMapper.KnowledgeBase(_profiles.GetNineStarKiSummaryViewModel(), help));
        }
    }
}
