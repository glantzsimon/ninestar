using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class MailingListsController : BaseNineStarKiController<MailingList>
    {
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;
        private readonly IRepository<UserMembership> _userMembershipsRepository;

        public MailingListsController (IControllerPackage<MailingList> controllerPackage, INineStarKiPackage nineStarKiPackage, IRepository<MembershipOption> membershipOptionsRepository, IRepository<UserMembership> userMembershipsRepository)
            : base(controllerPackage, nineStarKiPackage)
        {
            _membershipOptionsRepository = membershipOptionsRepository;
            _userMembershipsRepository = userMembershipsRepository;
        }
            
        public override ActionResult Index()
        {
            var memberships = _userMembershipsRepository.List().Select(e =>
            {
                e.User = My.UsersRepository.Find(e.UserId);
                e.MembershipOption = _membershipOptionsRepository.Find(m => m.Id == e.MembershipOptionId)
                    .FirstOrDefault();
                return e;
            });

            var model = new UserMembershipsViewModel
            {
                UserMemberships = memberships
                    .OrderByDescending(e => e.IsActive)
                    .ThenByDescending(e => e.StartsOn)
                    .ToList()
            };

            return View(model);
        }
    }
}