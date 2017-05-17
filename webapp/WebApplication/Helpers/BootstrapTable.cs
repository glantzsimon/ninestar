using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Constants.Html;
using K9.WebApplication.Extensions;
using K9.WebApplication.Options;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString BootstrapTable<T>(this HtmlHelper<T> html, string dataUrl = "", params string[] displayColumns)
		{
			var sb = new StringBuilder();
			var modelType = typeof (T);
			var properties = modelType.GetProperties().ToList();
			var columns = displayColumns.Any() ? properties.Where(p => displayColumns.Contains(p.Name)).ToList() : properties;
			var columnNames = columns.Select(p => p.Name).ToList();

			// Create table
			var table = new TagBuilder(Tags.Table);
			table.MergeAttribute(Attributes.Id, modelType.GetTableName());
			table.MergeAttribute(Attributes.Class, "bootstraptable display");
			table.MergeAttribute(Attributes.CellSpacing, "0");
			table.MergeAttribute(Attributes.Width, "100%");
			table.MergeAttribute(Attributes.DataUrl, string.IsNullOrEmpty(dataUrl) ? modelType.GetDefaultDataUrl() : dataUrl);

			// Add header
			var thead = new TagBuilder(Tags.Thead);
			thead.AddColumns(columnNames);
			
			// Add footer
			var tfoot = new TagBuilder(Tags.TFoot);
			tfoot.AddColumns(columnNames);

			table.InnerHtml += thead.ToString();
			table.InnerHtml += tfoot.ToString();

			sb.Append(table);
			sb.AppendLine(html.DataTable(dataUrl, columns).ToString());

			return MvcHtmlString.Create(sb.ToString());
		}

		private static TagBuilder AddColumns(this TagBuilder builder, List<string> columns)
		{
			var tr = new TagBuilder(Tags.Tr);	
			foreach (var column in columns)
			{
				var th = new TagBuilder(Tags.Th);	
				th.SetInnerText(column);
				tr.InnerHtml += th.ToString();
			}
			builder.InnerHtml += tr.ToString();
			return builder;
		}

		private static MvcHtmlString DataTable<T>(this HtmlHelper<T> html, string dataUrl, List<PropertyInfo> columns)
		{
			return html.Partial("_DataTables", new DataTableOptions
			{
				DataUrl = dataUrl,
				Columns = columns
			});
		}

	}
}