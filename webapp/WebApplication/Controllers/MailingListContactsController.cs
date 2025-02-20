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
	public class MailingListContactsController : BaseNineStarKiController<MailingListContact>
	{
		public MailingListContactsController(IControllerPackage<MailingListContact> controllerPackage, INineStarKiPackage nineStarKiPackage)
			: base(controllerPackage, nineStarKiPackage)
		{
		}

		[Authorize]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditMailingListsForContact(int id = 0)
		{
			return EditMultiple<Contact, MailingList>(Package.ContactsRepository.Find(id));
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditMailingListsForContact(MultiSelectViewModel model)
		{
			return EditMultiple<Contact, MailingList>(model);
		}

	}
}
