
using System.Collections.Generic;

namespace K9.WebApplication.Helpers
{
	public interface IDataTableAjaxHelper
	{
		int Draw { get; }
		int Start { get; }
		int Length { get; }
		string SearchValue { get; }
		List<IDataTableColumnInfo> ColumnInfos { get; }
	}

	public interface IDataTableColumnInfo
	{
		int SortColumn { get; }
		string SortDirection { get; }
	}
}