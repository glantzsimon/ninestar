
using System.Collections.Generic;

namespace K9.DataAccess.Models
{
	public interface IIgnoreColumns
	{
		List<string> ColumnsToIgnore { get; }
	}
}
