using K9.DataAccess.Config;
using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.EventArgs;
using K9.WebApplication.Helpers;
using NLog;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
	public class UsersController : BaseController<User>
	{
		public UsersController(IRepository<User> repository, ILogger logger, IDataTableAjaxHelper<User> ajaxHelper, IDataSetsHelper dataSetsHelper)
			: base(repository, logger, ajaxHelper, dataSetsHelper)
		{

			RecordCreated += UsersController_RecordCreated;

		}

		void UsersController_RecordCreated(object sender, CrudEventArgs e)
		{
			var user = e.Item as User;
			WebSecurity.CreateAccount(user.Username, AppConfig.DefaultUserPassword);
		}
	}
}
