using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.Base.WebApplication.ViewModels;
using K9.SharedLibrary.Authentication;
using K9.WebApplication.Packages;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
	[RequirePermissions(Role = RoleNames.Administrators)]
	public class RolePermissionsController : BaseNineStarKiController<RolePermission>
	{
		public RolePermissionsController(IControllerPackage<RolePermission> controllerPackage, INineStarKiPackage nineStarKiPackage)
			: base(controllerPackage, nineStarKiPackage)
		{
		}

		[Authorize]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditPermissionsForRole(int id = 0)
		{
			return EditMultiple<Role, Permission>(My.RolesRepository.Find(id));
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditPermissionsForRole(MultiSelectViewModel model)
		{
			return EditMultiple<Role, Permission>(model);
		}
	}
}
