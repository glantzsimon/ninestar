
using System;
using System.Web.Routing;

namespace K9.SharedLibrary.Models
{
	public interface IObjectBase
	{
		int Id { get; set; }
		string Name { get; set; }
		string CreatedBy { get; set; }
		DateTime? CreatedOn { get; set; }
		string LastUpdatedBy { get; set; }
		DateTime? LastUpdatedOn { get; set; }
		string GetForeignKeyName();
		void UpdateAuditFields();
		void UpdateName();
		RouteValueDictionary GetForeignKeyFilterRouteValues();
	}
}
