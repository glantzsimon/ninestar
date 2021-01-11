using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class UserCreditPacksController : BaseController<UserCreditPack>
    {
        public UserCreditPacksController(IControllerPackage<UserCreditPack> controllerPackage)
            : base(controllerPackage)
        {
        }
    }
}
