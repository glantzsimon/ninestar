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
		private HashSet<PropertyInfo> _columns;
		private HashSet<DataTableColumnInfo> _columnInfos;

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
			var controllerName = string.IsNullOrEmpty(Controller) ? typeof(T).GetListName() : Controller;
			return urlHeler.Action(actionName, controllerName, GetFilterRouteValues());
		}

		public List<PropertyInfo> GetColumns()
		{
			if (_columns == null)
			{
				var allColumns = typeof(T).GetProperties()
					.Where(p => !p.IsVirtual() && !ColumnsConfig.ColumnsToIgnore.Contains(p.Name)).ToList();

				var orderedColumns = VisibleColumns.Select(visibleColumn => allColumns.FirstOrDefault(c => c.Name == visibleColumn)).ToList();
				orderedColumns.AddRange(allColumns.Where(c => !orderedColumns.Contains(c)));

				_columns = new HashSet<PropertyInfo>();
				_columns.AddRange(orderedColumns);
			}
			return _columns.ToList();
		}

		public MvcHtmlString GetColumnsJson()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(GetColumnInfos().Select(c => new
			{
				data = c.Data,
				title = c.Name,
				orderable = true
			}).ToArray()));
		}

		public MvcHtmlString GetColumnDefsJson()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(
				GetColumnInfos().Select(c => new
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
			if (_columnInfos == null)
			{
				_columnInfos = new HashSet<DataTableColumnInfo>();

				var keyColumns = GetKeyColumns();
				var columnsInfos = GetColumns().Select((c, index) =>
				{
					var info = new DataTableColumnInfo(index)
					{
						IsDatabound = c.IsDataBound(),
						IsVisible =
							!keyColumns.Select(k => k.Name).Contains(c.Name) && (!VisibleColumns.Any() || VisibleColumns.Contains(c.Name))
					};
					info.UpdateData(c.Name);
					info.UpdateName(c.GetDisplayName());
					return info;
				}).ToList();

				_columnInfos.AddRange(columnsInfos);
			}
			return _columnInfos.ToList();
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