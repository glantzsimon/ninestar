using K9.Base.DataAccessLayer.Models;
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
    public class UserMembershipsController : BaseNineStarKiController<UserMembership>
    {
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;
        
        public UserMembershipsController(IControllerPackage<UserMembership> controllerPackage, IRepository<MembershipOption> membershipOptionsRepository, INineStarKiPackage nineStarKiPackage)
            : base(controllerPackage, nineStarKiPackage)
        {
            _membershipOptionsRepository = membershipOptionsRepository;
        }

        public override ActionResult Index()
        {
            var memberships = ControllerPackage.Repository.List().ToList();
            if (memberships.Count > 0)
            {
                var userIds = memberships.Select(e => e.UserId).Distinct().ToList();
                var optionIds = memberships.Select(e => e.MembershipOptionId).Distinct().ToList();
                var users = My.UsersRepository.Find(e => userIds.Contains(e.Id)).ToDictionary(e => e.Id);
                var options = _membershipOptionsRepository.Find(e => optionIds.Contains(e.Id)).ToDictionary(e => e.Id);
                foreach (var membership in memberships)
                {
                    users.TryGetValue(membership.UserId, out var user);
                    options.TryGetValue(membership.MembershipOptionId, out var option);
                    membership.User = user;
                    membership.MembershipOption = option;
                }
            }

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
