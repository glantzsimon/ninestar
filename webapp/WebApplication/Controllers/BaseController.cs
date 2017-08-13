using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.EventArgs;
using K9.WebApplication.Exceptions;
using K9.WebApplication.Extensions;
using K9.WebApplication.Filters;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.UnitsOfWork;
using K9.WebApplication.ViewModels;
using Newtonsoft.Json;
using NLog;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{

	public abstract class BaseController : Controller, IBaseController
	{
		private readonly ILogger _logger;
		private readonly IDataSetsHelper _dataSetsHelper;
		private readonly IRoles _roles;

		public IDataSetsHelper DropdownDataSets
		{
			get { return _dataSetsHelper; }
		}

		public IRoles Roles
		{
			get { return _roles; }
		}

		public ILogger Logger
		{
			get
			{
				return _logger;
			}
		}

		public abstract string GetObjectName();

		public BaseController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles)
		{
			_logger = logger;
			_dataSetsHelper = dataSetsHelper;
			_roles = roles;
		}
	}

	public abstract class BaseController<T> : Controller, IBaseController where T : class, IObjectBase
	{
		
		#region Events

		/// <summary>
		/// Event fires before the record is passed to the Create view
		/// </summary>
		public event EventHandler<CrudEventArgs> RecordBeforeCreate;
		/// <summary>
		/// Event fires when the record is posted and before the record is saved to the repository
		/// </summary>
		public event EventHandler<CrudEventArgs> RecordBeforeCreated;
		public event EventHandler<CrudEventArgs> RecordCreated;
		public event EventHandler<CrudEventArgs> RecordCreateError;

		/// <summary>
		/// Event fires before the record is passed to the Delete view
		/// </summary>
		public event EventHandler<CrudEventArgs> RecordBeforeDelete;
		/// <summary>
		/// Event fires when the record is posted and before the record is removed from the repository
		/// </summary>
		public event EventHandler<CrudEventArgs> RecordBeforeDeleted;
		public event EventHandler<CrudEventArgs> RecordDeleted;
		public event EventHandler<CrudEventArgs> RecordDeleteError;

		/// <summary>
		/// Event fires before the record is passed to the Edit view
		/// </summary>
		public event EventHandler<CrudEventArgs> RecordBeforeUpdate;
		/// <summary>
		/// Event fires when the record is posted and before the record is saved to the repository
		/// </summary>
		public event EventHandler<CrudEventArgs> RecordBeforeUpdated;
		public event EventHandler<CrudEventArgs> RecordUpdated;
		public event EventHandler<CrudEventArgs> RecordUpdateError;

		#endregion


		#region Properties

		public IControllerPackage<T> ControllerPackage { get; set; }

		public IRepository<T> Repository
		{
			get { return ControllerPackage.Repository; }
		}

		public IDataSetsHelper DropdownDataSets
		{
			get { return ControllerPackage.DataSetsHelper; }
		}

		public IRoles Roles
		{
			get { return ControllerPackage.Roles; }
		}

		public ILogger Logger
		{
			get
			{
				return ControllerPackage.Logger;
			}
		}

		public IDataTableAjaxHelper<T> AjaxHelper
		{
			get
			{
				return ControllerPackage.AjaxHelper;
			}
		}

		public IFileSourceHelper FileSourceHelper
		{
			get
			{
				return ControllerPackage.FileSourceHelper;
			}
		}

		#endregion


		#region EventHandlers

		protected override void Dispose(bool disposing)
		{
			if (Repository != null)
			{
				Repository.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion


		#region Constructors

		protected BaseController(IControllerPackage<T> controllerPackage)
		{
			ControllerPackage = controllerPackage;
		}

		#endregion


		#region Views

		[RequirePermissions(Permission = Permissions.View)]
		[OutputCache(NoStore = true, Duration = 0)]
		public virtual ActionResult Index()
		{
			SetTitle();
			ViewBag.Subtitle = string.Format("{0}{1}", typeof(T).GetPluralName(), GetStatelessFilterTitle()); ;
			return View("Index");
		}

		[RequirePermissions(Permission = Permissions.View)]
		[OutputCache(NoStore = true, Duration = 0)]
		public virtual ActionResult Details(int id = 0)
		{
			T item = Repository.Find(id);
			if (item == null)
			{
				return HttpNotFound();
			}

			if (!CheckLimitByUser(item))
			{
				return HttpForbidden();
			}

			SetTitle();

			var type = typeof(T);
			ViewBag.SubTitle = string.Format(Dictionary.DetailsText, type.GetName(), type.GetOfPreposition().ToLower());

			AddControllerBreadcrumb();

			return View(item);
		}

		#endregion


		#region DataTable

		[OutputCache(NoStore = true, Duration = 0)]
		public virtual ActionResult List()
		{
			AjaxHelper.LoadQueryString(HttpContext.Request.QueryString);
			AjaxHelper.StatelessFilter = this.GetStatelessFilter();

			try
			{
				var recordsTotal = Repository.GetCount(GetLimitByUserWhereClause());
				var limitByUserId = LimitByUser() ? WebSecurity.CurrentUserId : (int?)null;
				var recordsFiltered = Repository.GetCount(AjaxHelper.GetWhereClause(true, limitByUserId));
				var data = Repository.GetQuery(AjaxHelper.GetQuery(true, limitByUserId));
				var json = JsonConvert.SerializeObject(new
				{
					draw = AjaxHelper.Draw,
					recordsTotal,
					recordsFiltered,
					data
				}, new JsonSerializerSettings
				{
					DateFormatString = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.LongDatePattern,
					Culture = Thread.CurrentThread.CurrentUICulture
				});
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				Logger.Error(ex.Message);
				Logger.Info(AjaxHelper.GetQuery(true));
				return Content(string.Empty, "application/json");
			}
		}

		#endregion


		#region CRUD

		[Authorize]
		[RequirePermissions(Permission = Permissions.Create)]
		[OutputCache(NoStore = true, Duration = 0)]
		public virtual ActionResult Create()
		{
			var itemToCreate = Activator.CreateInstance<T>();

			if (typeof(T).ImplementsIUserData())
			{
				itemToCreate.SetProperty("UserId", WebSecurity.CurrentUserId);
			}

			var statelessFilter = this.GetStatelessFilter();

			SetTitle();
			ViewBag.SubTitle = string.Format("{0} {1}{2}", Dictionary.CreateNew, typeof(T).GetName(), GetStatelessFilterTitle());

			if (statelessFilter.IsSet())
			{
				itemToCreate.SetProperty(statelessFilter.Key, statelessFilter.Id);
			}

			AddControllerBreadcrumb();

			if (RecordBeforeCreate != null)
			{
				RecordBeforeCreate(this, new CrudEventArgs
				{
					Item = itemToCreate
				});
			}

			return View(itemToCreate);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		[RequirePermissions(Permission = Permissions.Create)]
		[OutputCache(NoStore = true, Duration = 0)]
		public virtual ActionResult Create(T item)
		{
			if (ModelState.IsValid)
			{
				try
				{
					if (RecordBeforeCreated != null)
					{
						RecordBeforeCreated(this, new CrudEventArgs
						{
							Item = item
						});
					}

					Repository.Create(item);

					SavePostedFiles(item);

					if (RecordCreated != null)
					{
						RecordCreated(this, new CrudEventArgs
						{
							Item = item
						});
					}

					return RedirectToAction("Index", this.GetFilterRouteValueDictionary());
				}
				catch (Exception ex)
				{
					Logger.Error(ex.GetFullErrorMessage());
					ModelState.AddErrorMessageFromException<T>(ex, item);

					LoadUploadedFiles(item);

					if (RecordCreateError != null)
					{
						RecordCreateError(this, new CrudEventArgs
						{
							Item = item
						});
					}
				}
			}

			SetTitle();
			ViewBag.SubTitle = string.Format("{0} {1}{2}", Dictionary.CreateNew, typeof(T).GetName(), GetStatelessFilterTitle());

			AddControllerBreadcrumb();

			return View(item);
		}

		[Authorize]
		[RequirePermissions(Permission = Permissions.Edit)]
		[OutputCache(NoStore = true, Duration = 0)]
		public virtual ActionResult Edit(int id = 0)
		{
			var item = Repository.Find(id);
			if (item == null)
			{
				return HttpNotFound();
			}

			if (!CheckLimitByUser(item))
			{
				return HttpForbidden();
			}

			if (item.IsSystemStandard)
			{
				return HttpForbidden();
			}

			SetTitle();
			ViewBag.SubTitle = string.Format("{0} {1}", Dictionary.Edit, typeof(T).GetName());

			AddControllerBreadcrumb();

			LoadUploadedFiles(item);

			if (RecordBeforeUpdate != null)
			{
				RecordBeforeUpdate(this, new CrudEventArgs
				{
					Item = item
				});
			}

			return View(item);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		[RequirePermissions(Permission = Permissions.Edit)]
		[OutputCache(NoStore = true, Duration = 0)]
		public virtual ActionResult Edit(T item)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var original = Repository.Find(item.Id);
					if (original.IsSystemStandard)
					{
						return HttpForbidden();
					}

					if (!CheckLimitByUser(original))
					{
						return HttpForbidden();
					}

					SavePostedFiles(item);

					if (RecordBeforeUpdated != null)
					{
						RecordBeforeUpdated(this, new CrudEventArgs
						{
							Item = item
						});
					}

					Repository.Update(item);

					if (RecordUpdated != null)
					{
						RecordUpdated(this, new CrudEventArgs
						{
							Item = item
						});
					}

					return RedirectToAction("Index", this.GetFilterRouteValueDictionary());
				}
				catch (Exception ex)
				{
					Logger.Error(ex.Message);
					ModelState.AddErrorMessageFromException<T>(ex, item);

					LoadUploadedFiles(item);

					if (RecordUpdateError != null)
					{
						RecordUpdateError(this, new CrudEventArgs
						{
							Item = item
						});
					}
				}
			}

			SetTitle();
			ViewBag.SubTitle = string.Format("{0} {1}", Dictionary.Edit, typeof(T).GetName());

			AddControllerBreadcrumb();

			return View(item);
		}

		[Authorize]
		[RequirePermissions(Permission = Permissions.Delete)]
		[OutputCache(NoStore = true, Duration = 0)]
		public virtual ActionResult Delete(int id)
		{
			T item = Repository.Find(id);
			if (item == null)
			{
				return HttpNotFound();
			}

			if (item.IsSystemStandard)
			{
				return HttpForbidden();
			}

			if (!CheckLimitByUser(item))
			{
				return HttpForbidden();
			}

			SetTitle();
			ViewBag.SubTitle = string.Format("{0} {1}", Dictionary.Delete, typeof(T).GetName());

			AddControllerBreadcrumb();

			if (RecordBeforeDelete != null)
			{
				RecordBeforeDelete(this, new CrudEventArgs
				{
					Item = item
				});
			}

			return View(item);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize]
		[RequirePermissions(Permission = Permissions.Delete)]
		[OutputCache(NoStore = true, Duration = 0)]
		public virtual ActionResult DeleteConfirmed(int id = 0)
		{
			T item = null;
			if (ModelState.IsValid)
			{
				item = Repository.Find(id);
				if (item == null)
				{
					return HttpNotFound();
				}

				if (item.IsSystemStandard)
				{
					return HttpForbidden();
				}

				if (!CheckLimitByUser(item))
				{
					return HttpForbidden();
				}

				try
				{
					if (RecordBeforeDeleted != null)
					{
						RecordBeforeDeleted(this, new CrudEventArgs
						{
							Item = item
						});
					}

					Repository.Delete(id);

					if (RecordDeleted != null)
					{
						RecordDeleted(this, new CrudEventArgs
						{
							Item = item
						});
					}

					return RedirectToAction("Index", this.GetFilterRouteValueDictionary());
				}
				catch (Exception ex)
				{
					Logger.Error(ex.Message);
					ModelState.AddErrorMessageFromException(ex, item);

					if (RecordDeleteError != null)
					{
						RecordDeleteError(this, new CrudEventArgs
						{
							Item = item
						});
					}
				}
			}

			SetTitle();
			ViewBag.SubTitle = string.Format("{0} {1}", Dictionary.Delete, typeof(T).GetName());

			AddControllerBreadcrumb();

			return View(item);
		}

		#endregion


		#region CRUD Multiple

		[Authorize]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditMultiple<T2, T3>(T2 parent)
			where T2 : class, IObjectBase
			where T3 : class, IObjectBase
		{
			if (parent == null)
			{
				return HttpNotFound();
			}

			if (parent.IsSystemStandard)
			{
				return HttpForbidden();
			}

			if (!CheckLimitByUser(parent))
			{
				return HttpForbidden();
			}

			SetTitle();
			ViewBag.SubTitle = string.Format("{0} {1}", Dictionary.Edit, typeof(T).GetPluralName());

			AddControllerBreadcrumb();

			return View("EditMultiple", MultiSelectViewModel.Create<T, T2, T3>(parent, Repository.GetAllBy<T2, T3>(parent.Id)));
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditMultiple<T2, T3>(MultiSelectViewModel model)
			where T2 : class, IObjectBase
			where T3 : class, IObjectBase
		{
			SetTitle();
			ViewBag.SubTitle = string.Format("{0} {1}", Dictionary.Edit, typeof(UserRole).GetPluralName());

			try
			{
				var itemsToDelete = model.Items.Where(x => x.Id > 0 && !x.IsSelected).Select(x => x.Id).ToList();
				Repository.DeleteBatch(itemsToDelete);

				var itemsToAdd = model.Items.Where(x => x.Id == 0 && x.IsSelected).Select(x =>
				{
					var item = Activator.CreateInstance<T>();
					item.SetProperty(typeof(T2).GetForeignKeyName(), model.ParentId);
					item.SetProperty(typeof(T3).GetForeignKeyName(), x.ChildId);
					return item;
				}).ToList();
				Repository.CreateBatch(itemsToAdd);

				return RedirectToAction("Index", this.GetFilterRouteValueDictionary());
			}
			catch (Exception ex)
			{
				Logger.Error(ex.Message);
				ModelState.AddModelError("", ex.Message);
			}

			AddControllerBreadcrumb();

			return View("EditMultiple", model);
		}

		#endregion


		#region Helper Methods

		protected void SetTitle()
		{
			ViewBag.Title = typeof(T).GetPluralName();
		}

		public ActionResult HttpForbidden()
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
			return View("Unauthorized");
		}

		public new ActionResult HttpNotFound()
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return View("NotFound");
		}

		private string GetStatelessFilterTitle()
		{
			var statelessFilter = this.GetStatelessFilter();
			if (statelessFilter.IsSet())
			{
				var tableName = typeof(T).GetLinkedForeignTableName(statelessFilter.Key);
				return string.Format(" {0} {1}", Dictionary.For.ToLower(), Repository.GetName(tableName, statelessFilter.Id));
			}
			return string.Empty;
		}

		public string GetObjectName()
		{
			return typeof(T).Name;
		}

		private bool LimitByUser()
		{
			return GetType().LimitedByUser() && WebSecurity.IsAuthenticated && !Roles.CurrentUserIsInRoles(RoleNames.Administrators);
		}

		private string GetLimitByUserWhereClause()
		{
			return LimitByUser() ? string.Format(" WHERE [UserId] = {0}", WebSecurity.CurrentUserId) : string.Empty;
		}

		private bool CheckLimitByUser(IObjectBase item)
		{
			if (LimitByUser())
			{
				if (!typeof(T).ImplementsIUserData())
				{
					throw new LimitByUserIdException();
				}
				if ((int)item.GetProperty("UserId") != WebSecurity.CurrentUserId)
				{
					return false;
				}
			}
			return true;
		}

		private void AddControllerBreadcrumb()
		{
			ViewBag.Crumbs = new List<Crumb>
			{
				new Crumb
				{
					Label = typeof (T).GetPluralName(),
					ActionName = "Index",
					ControllerName = GetType().GetControllerName()
				}
			};
		}

		private void SavePostedFiles(T item)
		{
			foreach (var fileSourcePropertyInfo in item.GetFileSourceProperties())
			{
				var fileSource = this.GetProperty(fileSourcePropertyInfo) as FileSource;
				if (fileSource != null)
				{
					FileSourceHelper.SaveFilesToDisk(fileSource, true);
				}
			}
		}

		private void LoadUploadedFiles(T item)
		{
			foreach (var fileSourcePropertyInfo in item.GetFileSourceProperties())
			{
				var fileSource = this.GetProperty(fileSourcePropertyInfo) as FileSource;
				FileSourceHelper.LoadFiles(fileSource, false);
			}
		}

		#endregion

	}
}