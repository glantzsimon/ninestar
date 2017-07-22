using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using K9.DataAccess.Extensions;
using K9.DataAccess.Models;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Extensions;
using K9.WebApplication.Filters;
using K9.WebApplication.Helpers;
using K9.WebApplication.ViewModels;
using Microsoft.Ajax.Utilities;
using NLog;

namespace K9.WebApplication.Controllers
{

	public class UserRolesController : BaseController<UserRole>
	{
		private readonly IRepository<User> _userRepository;

		public UserRolesController(IRepository<UserRole> repository, IRepository<User> userRepository, ILogger logger, IDataTableAjaxHelper<UserRole> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles)
			: base(repository, logger, ajaxHelper, dataSetsHelper, roles)
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
			ViewBag.SubTitle = string.Format("{0} {1}", Dictionary.Edit, typeof (UserRole).GetPluralName());

			return View(MultiSelectViewModel.Create<UserRole, User, Role>(user, Repository.GetAllBy<User, Role>(user.Id)));
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditRolesForUser(MultiSelectViewModel model)
		{
			SetTitle();
			ViewBag.SubTitle = string.Format("{0} {1}", Dictionary.Edit, typeof(UserRole).GetPluralName());

			try
			{
				var itemsToDelete = model.Items.Where(x => x.Id > 0 && !x.IsSelected).Select(x => x.Id).ToList();
				Repository.DeleteBatch(itemsToDelete);

				var itemsToAdd = model.Items.Where(x => x.Id == 0 && x.IsSelected).Select(x => new UserRole
				{
					UserId = model.ParentId,
					RoleId = x.ChildId
				}).ToList();
				Repository.CreateBatch(itemsToAdd);
				
				return RedirectToAction("Index", this.GetFilterRouteValueDictionary());
			}
			catch (Exception ex)
			{
				Logger.Error(ex.Message);
				ModelState.AddModelError("", ex.Message);
			}

			return View(model);
		}
	}
}
