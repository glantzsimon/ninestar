using K9.DataAccess.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using K9.WebApplication.Filters;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{
	[RequirePermissions(Role = RoleNames.Administrators)]
	public class RolesController : BaseController<Role>
	{
		public RolesController(IRepository<Role> repository, ILogger logger, IDataTableAjaxHelper<Role> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles) : base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
		}
	}
}
