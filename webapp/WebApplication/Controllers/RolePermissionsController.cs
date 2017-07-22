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
	[Authorize]
	[RequirePermissions(Role = RoleNames.Administrators)]
	public class RolePermissionsController : BaseController<RolePermission>
	{
		private readonly IRepository<Role> _roleRepository;

		public RolePermissionsController(IRepository<RolePermission> repository, IRepository<Role> roleRepository, ILogger logger, IDataTableAjaxHelper<RolePermission> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles) : base(repository, logger, ajaxHelper, dataSetsHelper, roles)
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
