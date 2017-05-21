using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using K9.WebApplication.Extensions;

namespace K9.WebApplication.Options
{
	public class DataTableOptions : IDataTableOptions
	{
		public string TableId { get; set; }
		public string DataUrl { get; set; }
		public List<PropertyInfo> Columns { get; set; }

		public MvcHtmlString GetColumnDefs()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(Columns.Select(c => new { data = c.Name, type = c.GetDataTableType() }).ToArray()));
		}
	}
}