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
	public class UserRolesController : BaseController<UserRole>
	{
		private readonly IRepository<User> _userRepository;
		
		public UserRolesController(IControllerPackage<UserRole> controllerPackage, IRepository<User> userRepository)
			: base(controllerPackage)
		{
			_userRepository = userRepository;
		}

		[Authorize]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditRolesForUser(int id = 0)
		{
			return EditMultiple<User, Role>(_userRepository.Find(id));
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
