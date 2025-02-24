using K9.SharedLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using K9.SharedLibrary.Enums;

namespace K9.SharedLibrary.Helpers.Html
{
    public static class HtmlTableHelper
    {
        private const string DefaultTableClassName = "k9-datatable bootstraptable table table-striped table-bordered dataTable no-footer dtr-inline";

        public static string ToHtmlTable(this IEnumerable<object> items, params string[] visibleColumns)
        {
            return ToHtmlTable(items, visibleColumns.Select(c => new ColumnInfo(c)).ToList());
        }
        
        public static string ToHtmlTable(this IEnumerable<object> items, List<ColumnInfo> visibleColumns, params string[] classNames)
        {
            var table = new TagBuilder("table");
            var columns = visibleColumns.Any() ? visibleColumns : GetColumns(items);

            table.AddCssClass(GetClassNameString(classNames));

            var thead = new TagBuilder("thead");
            var row = new TagBuilder("tr");

            foreach (var column in columns)
            {
                var header = new TagBuilder("th");
                header.SetInnerText(column.ColumnDisplayText);
                row.InnerHtml += header.ToString();
            }
            thead.InnerHtml += row.ToString();
            table.InnerHtml += thead.ToString();

            // Add data
            var tbody = new TagBuilder("tbody");
            for (int i = 0; i < items.Count(); i++)
            {
                var item = items.ElementAt(i);
                row = new TagBuilder("tr");
                foreach (var column in columns)
                {
                    var cell = new TagBuilder("td");
                    cell.MergeAttribute("style", "vertical-align: top;");
                    Guid id = GetId(item);
                    string controlId = $"{column.ColumnName}[{i}]";
                    object value = GetValue(item, column.ColumnName);

                    switch (column.ColumnType)
                    {
                        case Enums.EColumnType.Checkbox:
                            cell.InnerHtml = GetCheckBox(controlId, id.ToString());
                            break;

                        default:
                            cell.InnerHtml = value.ToString();
                            break;
                    }

                    // Add cell to row
                    row.InnerHtml += cell.ToString();
                }

                // Add row to table
                tbody.InnerHtml += row.ToString();
            }
            table.InnerHtml += tbody.ToString();

            return table.ToString();
        }

        private static List<ColumnInfo> GetColumns(IEnumerable<Object> items)
        {
            var firstItem = items.FirstOrDefault();
            if (firstItem == null)
            {
                return new List<ColumnInfo>();
            }
            if (firstItem.GetType() == typeof(Dictionary<string, object>))
            {
                var dict = firstItem as Dictionary<string, object>;
                return dict?.Keys.Select(k => new ColumnInfo(k)).ToList();
            }

            return firstItem.GetType().GetProperties().Select(p => new ColumnInfo(EColumnType.Text, p.Name, p.GetDisplayName().SplitOnCapitalLetter())).ToList();
        }

        private static object GetValue(object item, string name)
        {
            if (item.GetType() == typeof(Dictionary<string, object>))
            {
                var dict = item as Dictionary<string, object>;
                try
                {
                    return dict?[name].ToString();
                }
                catch (Exception)
                {
                    return "";
                }
            }

            try
            {
                return item.GetProperty(name).ToFormattedString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static Guid GetId(object item, string idPropertyName = "ID")
        {
            try
            {
                return Guid.Parse(GetValue(item, idPropertyName).ToString());
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        private static string ToFormattedString(this object value)
        {
            if (value is TimeSpan span)
            {
                return span.ToString();
            }

            if (value is DateTime time)
            {
                var date = time;
                return date.Date == date ? date.ToShortDateString() : date.ToShortTimeString();
            }
            return value.ToString();
        }

        /// <summary>
        /// If multiple classes are used, a string is returned with the class names separated by a space, otherwise a single class name is returned.
        /// </summary>
        /// <param name="classNames"></param>
        /// <returns></returns>
        private static string GetClassNameString(string[] classNames)
        {
            var list = classNames.ToList();
            list.Insert(0, DefaultTableClassName);

            var sb = new StringBuilder();
            list.ForEach(c => sb.AppendFormat("{0} ", c));
            return sb.ToString().Trim();
        }

        private static string GetCheckBox(string id, string value, bool @checked = false)
        {
            return GetCheckBox(id, id, value, @checked);
        }

        private static string GetCheckBox(string id, string name, string value, bool @checked = false)
        {
            var checkBuilder = new TagBuilder("input");
            checkBuilder.MergeAttribute("type", "checkbox");
            checkBuilder.MergeAttribute("id", id);
            checkBuilder.MergeAttribute("name", name);
            checkBuilder.MergeAttribute("value", value);

            if (@checked)
            {
                checkBuilder.MergeAttribute("checked", value);
            }

            return checkBuilder.ToString();
        }
    }
}