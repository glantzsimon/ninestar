
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace K9.WebApplication.Helpers
{
	public class DataTableAjaxHelper : IDataTableAjaxHelper
	{

		private const string SortDirection = "asc";
		private const int TotalRows = 500;
		private const string DrawKey = "draw";
		private const string StartKey = "start";
		private const string LengthKey = "length";
		private const string SearchKey = "search[value]";
		private const string SearchRegexKey = "search[regex]";

		private readonly NameValueCollection _queryString;
		private readonly int _draw;
		private readonly int _start;
		private readonly int _length;
		private readonly string _searchValue;
		private readonly bool _isRegexSearch;
		private readonly List<IDataTableColumnInfo> _columnInfos;

		public DataTableAjaxHelper(NameValueCollection queryString)
		{
			_queryString = queryString;

			_draw = GetIntegerValueFromQueryString(DrawKey);
			_start = GetIntegerValueFromQueryString(StartKey);
			_length = GetIntegerValueFromQueryString(LengthKey);
			_searchValue = GetValueFromQueryString(SearchKey);
			_isRegexSearch = GetBooleanValueFromQueryString(SearchRegexKey);
		}

		public int Draw
		{
			get { return _draw; }
		}

		public int Start
		{
			get { return _start; }
		}

		public int Length
		{
			get { return _length; }
		}

		public string SearchValue
		{
			get { return _searchValue; }
		}

		public bool IsRegexSearch
		{
			get { return _isRegexSearch; }
		}

		public List<IDataTableColumnInfo> ColumnInfos
		{
			get { return _columnInfos; }
		}

		private string GetValueFromQueryString(string key)
		{
			return _queryString[key];
		}

		private int GetIntegerValueFromQueryString(string key)
		{
			int value = 0;
			int.TryParse(GetValueFromQueryString(key), out value);
			return value;
		}

		private bool GetBooleanValueFromQueryString(string key)
		{
			bool value = false;
			bool.TryParse(GetValueFromQueryString(key), out value);
			return value;
		}

		//private List<IDataTableColumnInfo> GetColumnInfosFromQueryString()
		//{
		//    var columnSortRegex = new Regex(@"order\[[0-9]\]\[column\]");
		//    var columnDirRegex = new Regex(@"order\[[0-9]\]\[dir\]");

		//    var columnOrderKeys = _queryString.AllKeys.Where(k => sortColumnRegex.IsMatch(k)).ToList();

		//    return columnOrderKeys.Select(key => new DataTableColumnInfo()).ToList();
		//}
	}

	public class DataTableColumnInfo : IDataTableColumnInfo
	{
		private readonly string _data;
		private readonly string _name;
		private readonly string _searchValue;
		private readonly string _searchRegex;
		private readonly int _sortColumn;
		private readonly string _sortDirection;

		public DataTableColumnInfo(string data, string name, string searchValue, string searchRegex, int sortColumn, string sortDirection)
		{
			_data = data;
			_name = name;
			_searchValue = searchValue;
			_searchRegex = searchRegex;
			_sortColumn = sortColumn;
			_sortDirection = sortDirection;
		}

		public string Data
		{
			get { return _data; }
		}

		public string Name
		{
			get { return _name; }
		}

		public string SearchValue
		{
			get { return _searchValue; }
		}

		public string SearchRegex
		{
			get { return _searchRegex; }
		}

		public int SortColumn
		{
			get { return _sortColumn; }
		}

		public string SortDirection
		{
			get { return _sortDirection; }
		}
	}
}