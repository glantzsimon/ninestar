using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class PermissionsController : BaseController<Permission>
	{
		public PermissionsController(IRepository<Permission> repository, ILogger logger, IDataTableAjaxHelper<Permission> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles) : base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
		}
	}
}
