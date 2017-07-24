using System.Web.Mvc;
using K9.DataAccess.Config;
using K9.DataAccess.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using K9.WebApplication.EventArgs;
using K9.WebApplication.Filters;
using K9.WebApplication.Helpers;
using NLog;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
	[Authorize]
	[RequirePermissions(Role = RoleNames.Administrators)]
	public class UsersController : BaseController<User>
	{
		private readonly IOptions<DatabaseConfiguration> _dataConfig;

		public UsersController(IRepository<User> repository, ILogger logger, IDataTableAjaxHelper<User> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles, IOptions<DatabaseConfiguration> dataConfig)
			: base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
			_dataConfig = dataConfig;
			RecordCreated += UsersController_RecordCreated;
		}

		void UsersController_RecordCreated(object sender, CrudEventArgs e)
		{
			var user = e.Item as User;
			WebSecurity.CreateAccount(user.Username, _dataConfig.Value.DefaultUserPassword, !user.AccountActivated);
		}

	}
}
