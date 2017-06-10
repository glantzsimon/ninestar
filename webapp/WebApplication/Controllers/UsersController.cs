using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class UsersController : BaseController<User>
	{
		public UsersController(IRepository<User> repository, ILogger logger, IDataTableAjaxHelper<User> ajaxHelper, IDataSetsHelper dataSetsHelper) 
			: base(repository, logger, ajaxHelper, dataSetsHelper)
		{
		}
	}
}
