using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DotNetOpenAuth.Messaging;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Extensions;

namespace K9.WebApplication.Options
{
	public class DataTableOptions<T> : IDataTableOptions where T : IObjectBase
	{
		private HashSet<PropertyInfo> _columnInfos;

		public string DataUrl { get; set; }
		public bool DisplayFooter { get; set; }
		public List<string> VisibleColumns { get; set; }
		public IColumnsConfig ColumnsConfig { get; set; }
		public bool AllowCreate { get; set; }
		public bool AllowEdit { get; set; }
		public bool AllowDelete { get; set; }
		public bool AllowDetail { get; set; }

		public DataTableOptions()
		{
			VisibleColumns = new List<string>();
		}

		public bool AllowCrud()
		{
			return AllowCreate || AllowEdit || AllowDelete || AllowDetail;
		}

		public List<string> GetColumnNames()
		{
			return GetColumns().Select(c => c.Name).ToList();
		}

		public string GetDataUrl()
		{
			return string.IsNullOrEmpty(DataUrl) ? typeof(T).GetDefaultDataUrl() : DataUrl;
		}

		public List<PropertyInfo> GetColumns()
		{
			if (_columnInfos == null)
			{
				var columns =
					typeof(T).GetProperties()
						.Where(p => !p.IsVirtualCollection() && !ColumnsConfig.ColumnsToIgnore.Contains(p.Name))
						.ToList();
				_columnInfos = new HashSet<PropertyInfo>();
				_columnInfos.AddRange(columns);
			}
			return _columnInfos.ToList();
		}

		public MvcHtmlString GetColumnsJson()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(GetColumns().Select(c => new
			{
				data = c.Name,
				title = c.GetDisplayName(),
				orderable = c.IsDataBound()
			}).ToArray()));
		}

		public MvcHtmlString GetColumnDefsJson()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(
				GetKeyColumns().Select(c => GetColumns().Select((co, index) => new
				{
					co.Name,
					Index = index,
					Type = co.PropertyType
				})
					.Where(_ => _.Name == c.Name)
						.Select(x => new
						{
							targets = new[] { x.Index },
							visible = false
						})
						.FirstOrDefault())));
		}

		private List<PropertyInfo> GetKeyColumns()
		{
			return GetColumns().GetPropertiesWithAttributes(typeof(KeyAttribute), typeof(ForeignKeyAttribute));
		}

	}
}