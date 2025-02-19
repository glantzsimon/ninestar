
using System.Collections.Generic;

namespace K9.SharedLibrary.Models
{
	public interface IColumnsConfig
	{
		List<string> ColumnsToIgnore { get; }
	}
}
