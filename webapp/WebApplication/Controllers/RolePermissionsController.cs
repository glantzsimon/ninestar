using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using K9.WebApplication.Filters;
using K9.WebApplication.UnitsOfWork;
using K9.WebApplication.ViewModels;

namespace K9.WebApplication.Controllers
{
	[Authorize]
	[RequirePermissions(Role = RoleNames.Administrators)]
	public class RolePermissionsController : BaseController<RolePermission>
	{
		private readonly IRepository<Role> _roleRepository;

		public RolePermissionsController(IRepository<Role> roleRepository, IControllerPackage<RolePermission> controllerPackage)
			: base(controllerPackage)
		{
			_roleRepository = roleRepository;
		}

		[Authorize]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditPermissionsForRole(int id = 0)
		{
			return EditMultiple<Role, Permission>(_roleRepository.Find(id));
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
