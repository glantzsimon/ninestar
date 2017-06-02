using System.Collections.Generic;
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

		private static IColumnsConfig _columnsConfig;

		public static void SetIgnoreColumns(IColumnsConfig columnsConfig)
		{
			_columnsConfig = columnsConfig;
		}

		public static MvcHtmlString BootstrapTable<T>(this HtmlHelper<T> html, DataTableOptions<T> options = null) where T : IObjectBase
		{
			options = options ?? new DataTableOptions<T>();
			options.ColumnsConfig = _columnsConfig;

			var sb = new StringBuilder();
			var modelType = typeof(T);

			// Create table container
			var div = new TagBuilder(Tags.Div);
			div.MergeAttribute(Attributes.Class, "datatable-container");

			// Create table
			var table = new TagBuilder(Tags.Table);
			var tableName = modelType.GetTableName();
			var dataUrl = options.GetDataUrl();
			table.MergeAttribute(Attributes.Id, tableName);
			table.MergeAttribute(Attributes.Class, "bootstraptable table table-striped table-bordered");
			table.MergeAttribute(Attributes.CellSpacing, "0");
			table.MergeAttribute(Attributes.Width, "100%");
			table.MergeAttribute(Attributes.DataUrl, dataUrl);

			// Add header
			var thead = new TagBuilder(Tags.Thead);
			var columnNames = options.GetColumnNames();
			thead.AddColumns(options);
			table.InnerHtml += thead.ToString();

			// Add footer
			if (options.DisplayFooter)
			{
				var tfoot = new TagBuilder(Tags.TFoot);
				tfoot.AddColumns(options);
				table.InnerHtml += tfoot.ToString();
			}

			div.InnerHtml += table.ToString();

			sb.Append(div);
			sb.AppendLine(html.Partial("Controls/_DataTablesJs", options).ToString());

			return MvcHtmlString.Create(sb.ToString());
		}

		private static void AddColumns(this TagBuilder builder, IDataTableOptions options)
		{
			var tr = new TagBuilder(Tags.Tr);
			foreach (var column in options.GetColumnNames())
			{
				var th = new TagBuilder(Tags.Th);
				th.SetInnerText(column);
				tr.InnerHtml += th.ToString();
			}

			if (options.AllowCrud())
			{
				var th = new TagBuilder(Tags.Th);
				tr.InnerHtml += th.ToString();
			}

			builder.InnerHtml += tr.ToString();
		}

	}
}