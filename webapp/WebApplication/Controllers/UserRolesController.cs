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
	public class UserRolesController : BaseNineStarKiController<UserRole>
	{
		public UserRolesController(IControllerPackage<UserRole> controllerPackage, INineStarKiPackage nineStarKiPackage)
			: base(controllerPackage, nineStarKiPackage)
		{
		}

		[Authorize]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditRolesForUser(int id = 0)
		{
			return EditMultiple<User, Role>(My.UsersRepository.Find(id));
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditRolesForUser(MultiSelectViewModel model)
		{
			return EditMultiple<User, Role>(model);
		}

	}
}
