using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using DotNetOpenAuth.Messaging;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Extensions;
using K9.WebApplication.Helpers;

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

		public string TableId
		{
			get { return string.Format("{0}Table", typeof(T).Name); }
		}

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
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(GetColumnInfos().Select(c => new
			{
				data = c.Data,
				title = c.Name,
				orderable = c.IsDatabound,
				renderer = c.Renderer
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

		public string GetButtonRenderer()
		{
			if (AllowCrud())
			{
				var sb = new StringBuilder();
				sb.Append("function (data, type, row) { return '");

				if (AllowDetail)
				{
					sb.AppendFormat("<a href=\"/details/\"' + data.Id + ' class=\"btn btn-info\">{0}</a>", Dictionary.Details);
				}
				if (AllowEdit)
				{
					sb.AppendFormat("<a href=\"/edit/\"' + data.Id + ' class=\"btn btn-primary\">{0}</a>", Dictionary.Edit);
				}
				if (AllowDelete)
				{
					sb.AppendFormat("<a href=\"/delete/\"' + data.Id + ' class=\"btn btn-danger\">{0}</a>", Dictionary.Delete);
				}
				sb.Append("';}");
				return sb.ToString();
			}
			return string.Empty;
		}

		private List<DataTableColumnInfo> GetColumnInfos()
		{
			var infos = GetColumns().Select((c, index) =>
			{
				var info = new DataTableColumnInfo(index);
				info.UpdateData(c.Name);
				info.UpdateName(c.GetDisplayName());
				info.IsDatabound = c.IsDataBound();
				return info;
			}).ToList();

			if (AllowCrud())
			{
				var info = new DataTableColumnInfo(infos.Count);
				info.IsDatabound = false;
				info.Renderer = GetButtonRenderer();
				infos.Add(info);
			}

			return infos;
		}

		private List<PropertyInfo> GetKeyColumns()
		{
			return GetColumns().GetPropertiesWithAttributes(typeof(KeyAttribute), typeof(ForeignKeyAttribute));
		}

	}
}