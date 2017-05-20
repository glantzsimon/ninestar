using System;
using System.Linq;
using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.DataAccess.Respositories;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Constants;
using Newtonsoft.Json;
using NLog;

namespace K9.WebApplication.Controllers
{

	public abstract class BaseController<T> : Controller where T : ObjectBase, IIdentity
	{

		#region Variables

		private readonly IRepository<T> _repository;
		private readonly ILogger _logger;

		#endregion


		#region EventHandlers

		protected override void Dispose(bool disposing)
		{
			if (_repository != null)
			{
				_repository.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion


		#region Constructors

		protected BaseController(IRepository<T> repository, ILogger logger)
		{
			_repository = repository;
			_logger = logger;
		}

		#endregion


		#region Views

		[Authorize]
		public virtual ActionResult Index()
		{
			return View("Index");
		}

		[Authorize]
		public virtual ActionResult Details(int id = 0)
		{
			T item = _repository.Find(id);
			if (item == null)
			{
				return HttpNotFound();
			}
			return View(item);
		}

		#endregion

		#region DataTable

		[Authorize]
		public virtual ActionResult List()
		{
			_logger.Info(this.GetQueryString());
			try
			{
				var json = JsonConvert.SerializeObject(new { data = _repository.List() }, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddThh:mm:ssZ" });
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.Error(ex.Message);
				throw;
			}
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
					_repository.Create(item);
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
			var item = _repository.Find(id);
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
					_repository.Update(item);
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
			T item = _repository.Find(id);
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
					_repository.Delete(id);
					return RedirectToAction("Index");
				}
				catch (Exception ex)
				{
					_logger.Error(ex.Message);
					throw;
				}
			}

			T item = _repository.Find(id);
			if (item == null)
			{
				return HttpNotFound();
			}
			return View(item);
		}

		#endregion


	}
}