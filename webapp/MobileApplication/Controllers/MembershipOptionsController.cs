using K9.Base.DataAccessLayer.Config;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class MembershipOptionsController : BaseController<MembershipOption>
    {
        private readonly IOptions<DatabaseConfiguration> _dataConfig;
        private readonly IRoles _roles;

        public MembershipOptionsController(IControllerPackage<MembershipOption> controllerPackage, IOptions<DatabaseConfiguration> dataConfig, IRoles roles)
            : base(controllerPackage)
        {
            _dataConfig = dataConfig;
            _roles = roles;
        }
    }
}
