
using System.Collections.Generic;
using System.Collections.Specialized;

namespace K9.WebApplication.Helpers
{
	public interface IDataTableAjaxHelper<T> where T : class 
	{
		int Draw { get; }
		int Start { get; }
		int Length { get; }
		string SearchValue { get; }
		List<IDataTableColumnInfo> ColumnInfos { get; }
		void LoadQueryString(NameValueCollection queryString);
		string GetQuery(bool selectAllColumns = false);
	}

	public interface IDataTableColumnInfo
	{
		int Index { get; }
		string Data { get; }
		string Name { get; }
		string SearchValue { get; }
		bool IsRegexSearch { get; }

		void UpdateData(string data);
		void UpdateName(string name);
		void UpdateSearchValue(string searchValue);
		void UpdateIsSearchRegex(bool value);
		string GetLikeSearchValue();
	}
}