using System.Web.Mvc;
using K9.DataAccess.Config;
using K9.DataAccess.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using K9.WebApplication.EventArgs;
using K9.WebApplication.Filters;
using K9.WebApplication.UnitsOfWork;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
	[Authorize]
	[RequirePermissions(Role = RoleNames.Administrators)]
	public class UsersController : BaseController<User>
	{
		private readonly IOptions<DatabaseConfiguration> _dataConfig;

		public UsersController(IControllerPackage<User> controllerPackage, IOptions<DatabaseConfiguration> dataConfig)
			: base(controllerPackage)
		{
			_dataConfig = dataConfig;
			RecordCreated += UsersController_RecordCreated;
			RecordBeforeCreate += UsersController_RecordBeforeCreate;
		}

		void UsersController_RecordBeforeCreate(object sender, CrudEventArgs e)
		{
			var user = e.Item as User;
			user.Password = _dataConfig.Value.DefaultUserPassword;
		}

		void UsersController_RecordCreated(object sender, CrudEventArgs e)
		{
			var user = e.Item as User;
			WebSecurity.CreateAccount(user.Username, user.Password, !user.AccountActivated);
		}

	}
}
