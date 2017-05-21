using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Extensions;

namespace K9.WebApplication.Options
{
	public class DataTableOptions : IDataTableOptions
	{
		public string TableId { get; set; }
		public string DataUrl { get; set; }
		public List<PropertyInfo> Columns { get; set; }

		public MvcHtmlString GetColumnsJson()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(Columns.Select(c => new
			{
				data = c.Name,
				name = c.GetDisplayName(),
				type = c.GetDataTableType()
			}).ToArray()));
		}

		public MvcHtmlString GetColumnDefsJson()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(GetKeyColumns()
				.Select(c => Columns.Select((co, index) => new { co.Name, Index = index })
					.Where(x => x.Name == c.Name)
					.Select(x => new { targets = new[] { x.Index }, visible = false })
					.FirstOrDefault())));
		}

		private List<PropertyInfo> GetKeyColumns()
		{
			return Columns.GetPropertiesWithAttributes(typeof(KeyAttribute), typeof(ForeignKeyAttribute));
		}

	}
}