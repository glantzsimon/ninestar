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
        
        public UserMembershipsController(IControllerPackage<UserMembership> controllerPackage, IRepository<MembershipOption> membershipOptionsRepository, INineStarKiControllerPackage nineStarKiControllerPackage)
            : base(controllerPackage, nineStarKiControllerPackage)
        {
            _membershipOptionsRepository = membershipOptionsRepository;
        }

        public override ActionResult Index()
        {
            var memberships = ControllerPackage.Repository.List().Select(e =>
            {
                e.User = Package.UsersRepository.Find(e.UserId);
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
