using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using K9.WebApplication.Filters;
using K9.WebApplication.Helpers;
using K9.WebApplication.ViewModels;
using NLog;

namespace K9.WebApplication.Controllers
{
	[RequirePermissions(Role = RoleNames.Administrators)]
	public class UserRolesController : BaseController<UserRole>
	{
		private readonly IRepository<User> _userRepository;

		public UserRolesController(IRepository<UserRole> repository, IRepository<User> userRepository, ILogger logger, IDataTableAjaxHelper<UserRole> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles)
			: base(repository, logger, ajaxHelper, dataSetsHelper, roles)
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
