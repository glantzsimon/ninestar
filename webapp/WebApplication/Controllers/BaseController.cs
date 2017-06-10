using System;
using System.Threading;
using System.Web.Mvc;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Extensions;
using K9.WebApplication.Helpers;
using Newtonsoft.Json;
using NLog;

namespace K9.WebApplication.Controllers
{

	public abstract class BaseController<T> : Controller, IBaseController where T : class, IObjectBase
	{

		#region Variables

		private readonly IRepository<T> _repository;
		private readonly ILogger _logger;
		private readonly IDataTableAjaxHelper<T> _ajaxHelper;
		private readonly IDataSetsHelper _dataSetsHelper;

		#endregion


		#region Properties

		public IRepository<T> Repository
		{
			get { return _repository; }
		}

		public IDataSetsHelper DropdownDataSets
		{
			get { return _dataSetsHelper; }
		}

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

		protected BaseController(IRepository<T> repository, ILogger logger, IDataTableAjaxHelper<T> ajaxHelper, IDataSetsHelper dataSetsHelper)
		{
			_repository = repository;
			_logger = logger;
			_ajaxHelper = ajaxHelper;
			_dataSetsHelper = dataSetsHelper;
		}

		#endregion


		#region Views

		[Authorize]
		public virtual ActionResult Index()
		{
			ViewBag.Title = string.Format("{0}{1}", typeof(T).GetPluralName(), GetStatelessFilterTitle());
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
			var type = typeof(T);
			ViewBag.Title = string.Format(Dictionary.DetailsText, type.GetName(), type.GetOfPreposition().ToLower());
			return View(item);
		}

		#endregion


		#region DataTable

		[Authorize]
		public virtual ActionResult List()
		{
			_ajaxHelper.LoadQueryString(HttpContext.Request.QueryString);
			_ajaxHelper.StatelessFilter = this.GetStatelessFilter();

			try
			{
				var recordsTotal = _repository.GetCount();
				var recordsFiltered = _repository.GetCount(_ajaxHelper.GetWhereClause(true));
				var data = _repository.GetQuery(_ajaxHelper.GetQuery(true));
				var json = JsonConvert.SerializeObject(new
				{
					draw = _ajaxHelper.Draw,
					recordsTotal,
					recordsFiltered,
					data
				}, new JsonSerializerSettings { DateFormatString = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.LongDatePattern });
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.Error(ex.Message);
				_logger.Info(_ajaxHelper.GetQuery(true));
				return View("_FriendlyError");
			}
		}

		#endregion


		#region CRUD

		[Authorize(Roles = UserRoles.Administrators)]
		public virtual ActionResult Create()
		{
			var statelessFilter = this.GetStatelessFilter();

			ViewBag.Title = string.Format("{0} {1}{2}", Dictionary.CreateNew, typeof(T).GetName(), GetStatelessFilterTitle());

			if (statelessFilter.IsSet())
			{
				var itemToCreate = Activator.CreateInstance<T>();
				itemToCreate.SetProperty(statelessFilter.Key, statelessFilter.Id);
				return View(itemToCreate);
			}

			
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
					return RedirectToAction("Index", this.GetFilterRouteValueDictionary());
				}
				catch (Exception ex)
				{
					_logger.Error(ex.GetFullErrorMessage());
					ModelState.AddErrorMessageFromException<T>(ex, item);
				}
			}

			ViewBag.Title = string.Format("{0} {1}{2}", Dictionary.CreateNew, typeof(T).GetName(), GetStatelessFilterTitle());
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
			ViewBag.Title = string.Format("{0} {1}", Dictionary.Edit, typeof(T).GetName());
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
					return RedirectToAction("Index", this.GetFilterRouteValueDictionary());
				}
				catch (Exception ex)
				{
					_logger.Error(ex.Message);
					ModelState.AddErrorMessageFromException<T>(ex, item);
				}
			}

			ViewBag.Title = string.Format("{0} {1}", Dictionary.Edit, typeof(T).GetName());
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
			ViewBag.Title = string.Format("{0} {1}", Dictionary.Delete, typeof(T).GetName());
			return View(item);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize]
		public virtual ActionResult DeleteConfirmed(int id = 0)
		{
			T item = null;
			if (ModelState.IsValid)
			{
				item = _repository.Find(id);
				if (item == null)
				{
					return HttpNotFound();
				}

				try
				{
					_repository.Delete(id);
					return RedirectToAction("Index", this.GetFilterRouteValueDictionary());
				}
				catch (Exception ex)
				{
					_logger.Error(ex.Message);
					ModelState.AddErrorMessageFromException(ex, item);
				}
			}

			ViewBag.Title = string.Format("{0} {1}", Dictionary.Delete, typeof(T).GetName());
			return View(item);
		}

		#endregion


		#region Helper Methods

		private string GetStatelessFilterTitle()
		{
			var statelessFilter = this.GetStatelessFilter();
			if (statelessFilter.IsSet())
			{
				var tableName = typeof(T).GetLinkedForeignTableName(statelessFilter.Key);
				return string.Format(" {0} {1}", Dictionary.For.ToLower(), _repository.GetName(tableName, statelessFilter.Id));
			}
			return string.Empty;
		}

		#endregion


	}
}