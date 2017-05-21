
using System;
using System.Collections.Generic;
using System.Reflection;

namespace K9.SharedLibrary.Models
{
	public interface IObjectBase
	{
		int Id { get; }
		string Name { get; }
		string CreatedBy { get; set; }
		DateTime? CreatedOn { get; set; }
		string LastUpdatedBy { get; set; }
		DateTime? LastUpdatedOn { get; set; }
		void UpdateAuditFields();
	}
}
