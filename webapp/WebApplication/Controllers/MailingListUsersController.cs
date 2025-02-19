using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.Base.WebApplication.ViewModels;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
	public class MailingListUsersController : BaseController<MailingListUser>
	{
		private readonly IRepository<User> _userRepository;
		
		public MailingListUsersController(IControllerPackage<MailingListUser> controllerPackage, IRepository<User> userRepository)
			: base(controllerPackage)
		{
			_userRepository = userRepository;
		}

		[Authorize]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditMailingListsForUser(int id = 0)
		{
			return EditMultiple<User, MailingList>(_userRepository.Find(id));
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditMailingListsForUser(MultiSelectViewModel model)
		{
			return EditMultiple<User, MailingList>(model);
		}

	}
}
