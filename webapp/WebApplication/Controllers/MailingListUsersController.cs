using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.Base.WebApplication.ViewModels;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.WebApplication.Packages;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
	public class MailingListUsersController : BaseNineStarKiController<MailingListUser>
	{
		public MailingListUsersController(IControllerPackage<MailingListUser> controllerPackage, INineStarKiPackage nineStarKiPackage)
			: base(controllerPackage, nineStarKiPackage)
		{
		}

		[Authorize]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditMailingListsForUser(int id = 0)
		{
			return EditMultiple<User, MailingList>(My.UsersRepository.Find(id));
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
