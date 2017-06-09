
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using NLog;

namespace K9.WebApplication.Helpers
{
	public class DataTableAjaxHelper<T> : IDataTableAjaxHelper<T> where T : class
	{
		private const int DefaultTotalRows = 500;
		private const string DrawKey = "draw";
		private const string StartKey = "start";
		private const string LengthKey = "length";
		private const string SearchKey = "search[value]";
		private const string SearchRegexKey = "search[regex]";
		private const string SortedColumnIndexKey = "order[0][column]";
		private const string SortedColumnDirectionKey = "order[0][dir]";

		private readonly ILogger _logger;
		private readonly IColumnsConfig _columnsConfig;
		private readonly List<IDataTableColumnInfo> _columnInfos = new List<IDataTableColumnInfo>();
		private NameValueCollection _queryString;
		private int _draw;
		private int _start;
		private int _length;
		private string _searchValue;
		private bool _isRegexSearch;
		private int _orderByColumnIndex;
		private string _orderByDirection;

		private delegate void DataColumnInfoUpdateDelegate(IDataTableColumnInfo columnInfo, object value);

		public DataTableAjaxHelper(ILogger logger, IColumnsConfig columnsConfig)
		{
			_logger = logger;
			_columnsConfig = columnsConfig;
		}

		public void LoadQueryString(NameValueCollection queryString)
		{
			_queryString = queryString;

			_draw = GetIntegerValueFromQueryString(DrawKey);
			_start = GetIntegerValueFromQueryString(StartKey);
			_length = GetIntegerValueFromQueryString(LengthKey);
			_searchValue = GetValueFromQueryString(SearchKey);
			_isRegexSearch = GetBooleanValueFromQueryString(SearchRegexKey);
			_orderByColumnIndex = GetIntegerValueFromQueryString(SortedColumnIndexKey);
			_orderByDirection = GetValueFromQueryString(SortedColumnDirectionKey).ToUpper();
			SetColumnInfosFromQueryString();
		}

		public string GetQuery(bool selectAllColumns = false)
		{
			return string.Format("WITH RESULTS AS " +
								 "(SELECT {0}, ROW_NUMBER() OVER " +
								 "(ORDER BY {1} {2}) AS RowNum " +
								 "FROM {3} " +
								 "{4}) " +
								 "SELECT * FROM RESULTS " +
								 "WHERE RowNum BETWEEN {5} AND {6}",
								 GetSelectColumns(selectAllColumns),
								 OrderByColumnName,
								 OrderByDirection,
								 GetFrom(),
								 GetWhereClause(),
								 Start,
								 PageEnd);
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
			get { return _length == 0 ? DefaultTotalRows : _length; }
		}

		public int PageEnd
		{
			get { return _start + _length; }
		}

		public string SearchValue
		{
			get { return _searchValue; }
		}

		public IStatelessFilter StatelessFilter { get; set; }

		public bool IsRegexSearch
		{
			get { return _isRegexSearch; }
		}

		public List<IDataTableColumnInfo> ColumnInfos
		{
			get { return _columnInfos; }
		}

		public int OrderByColumnIndex
		{
			get { return _orderByColumnIndex; }
		}

		public string OrderByColumnName
		{
			get
			{
				try
				{
					return ColumnInfos.Any() ? ColumnInfos[_orderByColumnIndex].Data : string.Empty;
				}
				catch (Exception ex)
				{
					_logger.Error(ex.Message);
					throw new Exception("Invalid order by column index");
				}
			}
		}

		public string OrderByDirection
		{
			get { return _orderByDirection; }
		}

		public string GetWhereClause()
		{
			var sb = new StringBuilder();
			if (!string.IsNullOrEmpty(SearchValue))
			{
				foreach (var columnInfo in GetDataBoundColumnInfosNotIgnored())
				{
					sb.Append(sb.Length == 0 ? "WHERE " : " OR ");
					sb.AppendFormat("{0} LIKE '{1}'", columnInfo.Data, GetLikeSearchValue());
				}
			}
			else
			{
				foreach (var columnInfo in GetDataBoundColumnInfosNotIgnored())
				{
					if (!string.IsNullOrEmpty(columnInfo.SearchValue))
					{
						sb.Append(sb.Length == 0 ? "WHERE " : " OR ");
						sb.AppendFormat("{0} LIKE '{1}'", columnInfo.Data, columnInfo.GetLikeSearchValue());
					}
				}
			}

			if (StatelessFilter != null && StatelessFilter.IsSet())
			{
				sb.Append(sb.Length == 0 ? "WHERE " : " AND ");
				sb.AppendFormat("{0} = {1}", StatelessFilter.Key, StatelessFilter.Id);
			}

			return sb.ToString();
		}

		private string GetSelectColumns(bool selectAllColumns = false)
		{
			var sb = new StringBuilder();
			var parentType = typeof(T);

			if (selectAllColumns)
			{
				sb.AppendFormat("{0}.*", parentType.Name);
			}
			else
			{
				foreach (var columnInfo in GetDataBoundColumnInfosNotIgnored())
				{
					sb.Append(sb.Length == 0 ? "" : ", ");
					sb.Append(columnInfo.Data);
				}
			}

			foreach (var item in GetForeignKeyColumns())
			{
				var linkedTableName = parentType.GetLinkedPropertyType(item.Key.Name).Name;
				sb.Append(", ");
				sb.AppendFormat("{0}.Name AS [{0}Name]", linkedTableName);
			}

			return sb.ToString();
		}

		private string GetFrom()
		{
			var foreignKeyColumns = GetForeignKeyColumns();
			return foreignKeyColumns.Any()
				? GetFromWithJoins()
				: typeof(T).Name;
		}

		private string GetFromWithJoins()
		{
			var sb = new StringBuilder();
			var parentType = typeof(T);
			var parentName = parentType.Name;
			sb.Append(parentName);

			foreach (var item in GetForeignKeyColumns())
			{
				var linkedTableName = parentType.GetLinkedPropertyType(item.Key.Name).Name;
				sb.AppendFormat(" JOIN {0} ON {0}.Id = {1}.{2}", linkedTableName, parentName, item.Value.Name);
			}

			return sb.ToString();
		}

		private Dictionary<ForeignKeyAttribute, PropertyInfo> _foreignKeyColumnDictionary;
		private Dictionary<ForeignKeyAttribute, PropertyInfo> GetForeignKeyColumns()
		{
			if (_foreignKeyColumnDictionary == null)
			{
				_foreignKeyColumnDictionary = typeof(T).GetPropertiesAndAttributesWithAttribute<ForeignKeyAttribute>();
			}
			return _foreignKeyColumnDictionary;
		}

		private List<IDataTableColumnInfo> GetDataBoundColumnInfos()
		{
			return ColumnInfos.Where(c => typeof(T).GetProperties(BindingFlags.Public)
				.Where(p => p.CanWrite).Select(p => p.Name).Contains(c.Data)).ToList();
		}

		private List<IDataTableColumnInfo> GetDataBoundColumnInfosNotIgnored()
		{
			return GetDataBoundColumnInfos().Where(c => !_columnsConfig.ColumnsToIgnore.Contains(c.Name)).ToList();
		}

		/// <summary>
		/// Takes into account whether it is a regex search or not
		/// </summary>
		/// <returns></returns>
		private string GetLikeSearchValue()
		{
			return IsRegexSearch ? string.Format("%[{0}]%", SearchValue) : string.Format("%{0}%", SearchValue);
		}

		private string GetValueFromQueryString(string key)
		{
			return _queryString[key] ?? string.Empty;
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

		public bool IsRegexSearch
		{
			get { return _isSearchRegex; }
		}

		public bool IsDatabound { get; set; }

		public string Renderer { get; set; }

		public bool IsVisible { get; set; }

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

		/// <summary>
		/// Takes into account whether it is a regex search or not
		/// </summary>
		/// <returns></returns>
		public string GetLikeSearchValue()
		{
			return IsRegexSearch ? string.Format("%[{0}]%", SearchValue) : string.Format("%{0}%", SearchValue);
		}
	}
}