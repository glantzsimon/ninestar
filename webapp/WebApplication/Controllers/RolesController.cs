using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Authentication;
using K9.WebApplication.Filters;
using K9.WebApplication.UnitsOfWork;

namespace K9.WebApplication.Controllers
{
	[Authorize]
	[RequirePermissions(Role = RoleNames.Administrators)]
	public class RolesController : BaseController<Role>
	{
		public RolesController(IControllerPackage<Role> controllerPackage) : base(controllerPackage)
		{
		}
	}
}
