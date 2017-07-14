using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using K9.WebApplication.Extensions;
using K9.WebApplication.Filters;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{

	public class UserRolesController : BaseController<UserRole>
	{
		private readonly IRepository<User> _userRepository;

		public UserRolesController(IRepository<UserRole> repository, IRepository<User> userRepository, ILogger logger, IDataTableAjaxHelper<UserRole> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles) : base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
			_userRepository = userRepository;
		}

		[Authorize]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditRolesForUser(int id = 0)
		{
			var user = _userRepository.Find(id);
			if (user == null)
			{
				return HttpNotFound();
			}

			if (user.IsSystemStandard)
			{
				return View("Unauthorized");
			}

			SetTitle();
			ViewBag.SubTitle = string.Format("{0} {1}", Dictionary.Edit, typeof(UserRole).GetPluralName());

			return View(item);
	}
}
