using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using DotNetOpenAuth.Messaging;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Extensions;
using K9.WebApplication.Helpers;

namespace K9.WebApplication.Options
{
	public class DataTableOptions<T> : IDataTableOptions where T : IObjectBase
	{
		private HashSet<PropertyInfo> _columnInfos;

		public string Action { get; set; }
		public string Controller { get; set; }
		public bool DisplayFooter { get; set; }
		public List<string> VisibleColumns { get; set; }
		public IColumnsConfig ColumnsConfig { get; set; }
		public IStatelessFilter StatelessFilter { get; set; }
		public bool AllowCreate { get; set; }
		public bool AllowEdit { get; set; }
		public bool AllowDelete { get; set; }
		public bool AllowDetail { get; set; }

		public string TableId
		{
			get { return string.Format("{0}Table", typeof(T).Name); }
		}

		public DataTableOptions()
		{
			VisibleColumns = new List<string>();
			AllowCreate = true;
			AllowDelete = true;
			AllowDetail = true;
			AllowEdit = true;
		}

		public bool AllowCrud()
		{
			return AllowCreate || AllowEdit || AllowDelete || AllowDetail;
		}

		public List<string> GetColumnNames()
		{
			return GetColumns().Select(c => c.Name).ToList();
		}

		public string GetDataUrl(UrlHelper urlHeler)
		{
			var actionName = string.IsNullOrEmpty(Action) ? "List" : Action;
			var controllerName = string.IsNullOrEmpty(Controller) ? typeof(T).GetPluralName() : Controller;
			return urlHeler.Action(actionName, controllerName, GetFilterRouteValues());
		}

		public List<PropertyInfo> GetColumns()
		{
			if (_columnInfos == null)
			{
				var columns =
					typeof(T).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
						.Where(p => !p.IsVirtual() && !ColumnsConfig.ColumnsToIgnore.Contains(p.Name))
						.ToList();
				_columnInfos = new HashSet<PropertyInfo>();
				_columnInfos.AddRange(columns);
			}
			return _columnInfos.ToList();
		}

		public MvcHtmlString GetColumnsJson()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(GetColumnInfos().Select(c => new
			{
				data = c.Data,
				title = c.Name,
				orderable = c.IsDatabound
			}).ToArray()));
		}

		public MvcHtmlString GetColumnDefsJson()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(
				GetColumnInfos().Where(c => c.HasColumnDef).Select(c => new
				{
					targets = new[] { c.Index },
					visible = c.IsVisible
				})));
		}

		public string GetButtonRenderFunction()
		{
			return string.Format("renderButtons{0}", TableId);
		}

		public List<DataTableColumnInfo> GetColumnInfos()
		{
			var keyColumnNames = GetKeyColumns().Select(k => k.Name).ToList();
			var columnsInfos = GetColumns().Select((c, index) =>
			{
				var info = new DataTableColumnInfo(index)
				{
					IsDatabound = c.IsDataBound(),
					IsVisible = !keyColumnNames.Contains(c.Name),
					HasColumnDef = keyColumnNames.Contains(c.Name)
				};
				info.UpdateData(c.Name);
				info.UpdateName(c.GetDisplayName());
				return info;
			}).ToList();

			return columnsInfos;
		}

		public RouteValueDictionary GetFilterRouteValues()
		{
			if (StatelessFilter != null)
			{
				return StatelessFilter.GetFilterRouteValues();
			}
			return null;
		}

		public string GetQueryStringJoiner()
		{
			var routeValues = GetFilterRouteValues();
			return routeValues != null && routeValues.Any() ? "&" : "?";
		}

		private List<PropertyInfo> GetKeyColumns()
		{
			return GetColumns().GetPropertiesWithAttributes(typeof(KeyAttribute), typeof(ForeignKeyAttribute));
		}

	}
}