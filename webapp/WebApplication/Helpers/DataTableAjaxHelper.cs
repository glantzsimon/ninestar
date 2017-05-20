
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace K9.WebApplication.Helpers
{
	public class DataTableAjaxHelper : IDataTableAjaxHelper
	{

		private const int DefaultTotalRows = 500;
		private const string DrawKey = "draw";
		private const string StartKey = "start";
		private const string LengthKey = "length";
		private const string SearchKey = "search[value]";
		private const string SearchRegexKey = "search[regex]";
		private const string SortedColumnIndexKey = "order[0][column]";
		private const string SortedColumnDirectionKey = "order[0][dir]";

		private readonly NameValueCollection _queryString;
		private readonly int _draw;
		private readonly int _start;
		private readonly int _length;
		private readonly string _searchValue;
		private readonly bool _isRegexSearch;
		private readonly List<IDataTableColumnInfo> _columnInfos = new List<IDataTableColumnInfo>();
		private readonly int _sortedColumnIndex;
		private readonly string _sortedColumnDirection;

		private delegate void DataColumnInfoUpdateDelegate(IDataTableColumnInfo columnInfo, object value);

		public DataTableAjaxHelper(NameValueCollection queryString)
		{
			_queryString = queryString;

			_draw = GetIntegerValueFromQueryString(DrawKey);
			_start = GetIntegerValueFromQueryString(StartKey);
			_length = GetIntegerValueFromQueryString(LengthKey);
			_searchValue = GetValueFromQueryString(SearchKey);
			_isRegexSearch = GetBooleanValueFromQueryString(SearchRegexKey);
			_sortedColumnIndex = GetIntegerValueFromQueryString(SortedColumnIndexKey);
			_sortedColumnDirection = GetValueFromQueryString(SortedColumnDirectionKey);
			SetColumnInfosFromQueryString();
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

		public object SortedColumnIndex
		{
			get { return _sortedColumnIndex; }

		}

		public string SortedColumnDirection
		{
			get { return _sortedColumnDirection; }
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

		private void SetColumnInfosFromQueryString()
		{
			var columnDataRegex = new Regex(@"columns\[[0-9]\]\[data\]");
			var columnNameRegex = new Regex(@"columns\[[0-9]\]\[name\]");
			var columnSearchValueRegex = new Regex(@"columns\[[0-9]\]\[search\]\[value\]");
			var columnIsRegexSearchRegex = new Regex(@"columns\[[0-9]\]\[search\]\[regex\]");

			AddColumnInfoData(_queryString.AllKeys.Where(k => columnDataRegex.IsMatch(k)).ToList(), (columnInfo, data) =>
			{
				columnInfo.UpdateData(data.ToString());
			});

			AddColumnInfoData(_queryString.AllKeys.Where(k => columnNameRegex.IsMatch(k)).ToList(), (columnInfo, data) =>
			{
				columnInfo.UpdateName(data.ToString());
			});

			AddColumnInfoData(_queryString.AllKeys.Where(k => columnSearchValueRegex.IsMatch(k)).ToList(), (columnInfo, data) =>
			{
				columnInfo.UpdateSearchValue(data.ToString());
			});

			AddColumnInfoData(_queryString.AllKeys.Where(k => columnIsRegexSearchRegex.IsMatch(k)).ToList(), (columnInfo, data) =>
			{
				columnInfo.UpdateIsSearchRegex(bool.Parse(data.ToString()));
			});
		}

		private void AddColumnInfoData(List<string> keys, DataColumnInfoUpdateDelegate columnUpdater)
		{
			foreach (var key in keys)
			{
				var columnData = _queryString[key];
				var columnIndex = GetColumnIndexFromKey(key);
				var columnInfo = GetColumnInfoAtIndex(columnIndex);
				columnUpdater(columnInfo, columnData);
			}
		}

		private IDataTableColumnInfo GetColumnInfoAtIndex(int index)
		{
			var columnInfo = ColumnInfos.FirstOrDefault(c => c.Index == index);
			if (columnInfo == null)
			{
				columnInfo = new DataTableColumnInfo(index);
				ColumnInfos.Add(columnInfo);
			}
			return columnInfo;
		}

		private int GetColumnIndexFromKey(string key)
		{
			var columnIndexRegex = new Regex(@"\[([0-9])\]");
			int index = 0;
			var columnIndexValue = columnIndexRegex.Match(key).Groups[1].Value;
			int.TryParse(columnIndexValue, out index);
			return index;
		}
	}

	public class DataTableColumnInfo : IDataTableColumnInfo
	{
		private readonly int _index;
		private string _data;
		private string _name;
		private string _searchValue;
		private bool _isSearchRegex;

		public DataTableColumnInfo(int index)
		{
			_index = index;
		}

		public int Index
		{
			get { return _index; } 
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

		public bool IsSearchRegex
		{
			get { return _isSearchRegex; }
		}

		public void UpdateData(string data)
		{
			_data = data;
		}

		public void UpdateName(string name)
		{
			_name = name;
		}

		public void UpdateSearchValue(string searchValue)
		{
			_searchValue = searchValue;
		}

		public void UpdateIsSearchRegex(bool value)
		{
			_isSearchRegex = value;
		}
	}
}