
using System;
using System.Web.Routing;

namespace K9.SharedLibrary.Models
{
	public interface IObjectBase
	{
		int Id { get; }
		string Name { get; }
		string CreatedBy { get; set; }
		DateTime? CreatedOn { get; set; }
		string LastUpdatedBy { get; set; }
		string ForeignKeyName { get; }
		DateTime? LastUpdatedOn { get; set; }
		void UpdateAuditFields();
		void UpdateName();
		RouteValueDictionary GetForeignKeyFilterRouteValues();
	}
}
