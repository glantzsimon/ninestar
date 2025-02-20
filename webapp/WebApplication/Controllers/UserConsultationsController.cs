using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.WebApplication.Packages;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class UserConsultationsController : BaseNineStarKiController<UserConsultation>
    {
        public UserConsultationsController(IControllerPackage<UserConsultation> controllerPackage, INineStarKiPackage nineStarKiPackage)
            : base(controllerPackage, nineStarKiPackage)
        {
        }
    }
}
