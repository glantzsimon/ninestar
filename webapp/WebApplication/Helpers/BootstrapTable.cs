using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.SharedLibrary.Models;
using K9.WebApplication.Constants.Html;
using K9.WebApplication.Extensions;
using K9.WebApplication.Options;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		private static IIgnoreColumns _ignoreColumns;

		public static void SetIgnoreColumns(IIgnoreColumns ignoreColumns)
		{
			_ignoreColumns = ignoreColumns;
		}

		public static MvcHtmlString BootstrapTable<T>(this HtmlHelper<T> html, string dataUrl = "", bool displayFooter = false, params string[] displayColumns)
		{
			var sb = new StringBuilder();
			var modelType = typeof(T);
			var properties = modelType.GetProperties().Where(p => !_ignoreColumns.ColumnsToIgnore.Contains(p.Name)).ToList();
			var columns = displayColumns.Any() ? properties.Where(p => displayColumns.Contains(p.Name)).ToList() : properties;
			var columnNames = columns.Select(p => p.Name).ToList();

			// Create table container
			var div = new TagBuilder(Tags.Div);
			div.MergeAttribute(Attributes.Class, "datatable-container");

			// Create table
			var table = new TagBuilder(Tags.Table);
			var tableName = modelType.GetTableName();
			table.MergeAttribute(Attributes.Id, tableName);
			table.MergeAttribute(Attributes.Class, "bootstraptable table table-striped table-bordered");
			table.MergeAttribute(Attributes.CellSpacing, "0");
			table.MergeAttribute(Attributes.Width, "100%");
			table.MergeAttribute(Attributes.DataUrl, string.IsNullOrEmpty(dataUrl) ? modelType.GetDefaultDataUrl() : dataUrl);

			// Add header
			var thead = new TagBuilder(Tags.Thead);
			thead.AddColumns(columnNames);
			table.InnerHtml += thead.ToString();

			// Add footer
			if (displayFooter)
			{
				var tfoot = new TagBuilder(Tags.TFoot);
				tfoot.AddColumns(columnNames);
				table.InnerHtml += tfoot.ToString();
			}

			div.InnerHtml += table.ToString();

			sb.Append(div);
			sb.AppendLine(html.DataTable(tableName, dataUrl, columns).ToString());

			return MvcHtmlString.Create(sb.ToString());
		}

		private static void AddColumns(this TagBuilder builder, List<string> columns)
		{
			var tr = new TagBuilder(Tags.Tr);
			foreach (var column in columns)
			{
				var th = new TagBuilder(Tags.Th);
				th.SetInnerText(column);
				tr.InnerHtml += th.ToString();
			}
			builder.InnerHtml += tr.ToString();
		}

		private static MvcHtmlString DataTable<T>(this HtmlHelper<T> html, string tableId, string dataUrl, List<PropertyInfo> columns)
		{
			var dataTableOptions = new DataTableOptions
			{
				TableId = tableId,
				DataUrl = dataUrl,
				Columns = columns
			};
			return html.Partial("Controls/_DataTablesJs", dataTableOptions);
		}
	}
}