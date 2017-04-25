using System;
using System.Data.Entity;
using System.Web.Mvc;
using K9.DataAccess.Extensions;
using K9.DataAccess.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Interfaces;
using K9.WebApplication.Constants;
using NLog;

namespace K9.WebApplication.Controllers
{

	public abstract class BaseController<T> : Controller where T : ObjectBase, IIdentity
	{

		#region Variables

		private readonly DbContext _db;
		private readonly ILogger _logger;

		#endregion


		#region EventHandlers

		protected override void Dispose(bool disposing)
		{
			if (_db != null)
			{
				_db.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion


		#region Constructors

		protected BaseController(DbContext db, ILogger logger)
		{
			_db = db;
			_logger = logger;
		}

		#endregion


		#region Views

		[Authorize]
		[OutputCache(CacheProfile = Site.CacheProfiles.Moderate, VaryByParam = "page", VaryByHeader = "Accept-Language")]
		public ActionResult Index()
		{
			return View("Index");
		}

		[Authorize]
		public virtual ActionResult Details(int id = 0)
		{
			T item = _db.Find<T>(id);
			if (item == null)
			{
				return HttpNotFound();
			}
			return View(item);
		}

		#endregion


		#region CRUD

		[Authorize(Roles = UserRoles.Administrators)]
		public virtual ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = UserRoles.Administrators)]
		public virtual ActionResult Create(T item)
		{
			if (ModelState.IsValid)
			{
				try
				{
					_db.Create(item);
					return RedirectToAction("Index");
				}
				catch (Exception ex)
				{
					_logger.Error(ex.Message);
					throw;
				}
			}

			return View(item);
		}

		[Authorize(Roles = UserRoles.Administrators)]
		public virtual ActionResult Edit(int id = 0)
		{
			var item = _db.Find<T>(id);
			if (item == null)
			{
				return HttpNotFound();
			}
			return View(item);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = UserRoles.Administrators)]
		public virtual ActionResult Edit(T item)
		{
			if (ModelState.IsValid)
			{
				try
				{
					_db.Update(item);
					return RedirectToAction("Index");
				}
				catch (Exception ex)
				{
					_logger.Error(ex.Message);
					throw;
				}
			}

			return View(item);
		}

		[Authorize(Roles = UserRoles.Administrators)]
		public virtual ActionResult Delete(int id)
		{
			T item = _db.Find<T>(id);
			if (item == null)
			{
				return HttpNotFound();
			}
			return View(item);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize]
		public virtual ActionResult DeleteConfirmed(int id = 0)
		{
			if (ModelState.IsValid)
			{
				try
				{
					_db.Delete<T>(id);
					return RedirectToAction("Index");
				}
				catch (Exception ex)
				{
					_logger.Error(ex.Message);
					throw;
				}
			}

			T item = _db.Find<T>(id);
			if (item == null)
			{
				return HttpNotFound();
			}
			return View(item);
		}

		#endregion


	}
}