
using System.Collections.Generic;

namespace K9.SharedLibrary.Models
{
	public interface IIgnoreColumns
	{
		List<string> ColumnsToIgnore { get; }
	}
}
