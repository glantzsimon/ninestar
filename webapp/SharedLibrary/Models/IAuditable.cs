
using System;

namespace K9.SharedLibrary.Models
{
	public interface IAuditable
	{
		string CreatedBy { get; set; }
		DateTime? CreatedOn { get; set; }
		string LastUpdatedBy { get; set; }
		DateTime? LastUpdatedOn { get; set; }
	}
}
