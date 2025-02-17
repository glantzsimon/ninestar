using System.Linq;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using System.Web.Mvc;
using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class UserMembershipsController : BaseController<UserMembership>
    {
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;
        private readonly IRepository<User> _usersRepository;

        public UserMembershipsController(IControllerPackage<UserMembership> controllerPackage, IRepository<MembershipOption> membershipOptionsRepository, IRepository<User> usersRepository)
            : base(controllerPackage)
        {
            _membershipOptionsRepository = membershipOptionsRepository;
            _usersRepository = usersRepository;
        }

        public override ActionResult Index()
        {
            var memberships = ControllerPackage.Repository.List().Select(e =>
            {
                e.User = _usersRepository.Find(e.UserId);
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
