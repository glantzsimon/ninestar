using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{

	public class UserRolesController : BaseController<UserRole>
	{
		public UserRolesController(IRepository<UserRole> repository, ILogger logger, IDataTableAjaxHelper<UserRole> ajaxHelper, IDataSetsHelper dataSetsHelper)
			: base(repository, logger, ajaxHelper, dataSetsHelper)
		{
		}
	}
}
