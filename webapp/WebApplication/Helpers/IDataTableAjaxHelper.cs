
using System;
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
		int Index { get; }
		string Data { get; }
		string Name { get; }
		string SearchValue { get; }
		bool IsSearchRegex { get; }

		void UpdateData(string data);
		void UpdateName(string name);
		void UpdateSearchValue(string searchValue);
		void UpdateIsSearchRegex(bool value);
	}
}