using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{

	public class RolesPermissionsController : BaseController<RolePermission>
	{
		public RolesPermissionsController(IRepository<RolePermission> repository, ILogger logger, IDataTableAjaxHelper<RolePermission> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles) : base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
		}
	}
}
