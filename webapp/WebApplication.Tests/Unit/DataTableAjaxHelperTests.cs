using System.Collections.Specialized;
using System.Linq;
using K9.DataAccess.Models;
using K9.WebApplication.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;

namespace K9.WebApplication.Tests.Unit
{
	[TestClass]
	public class DataTableAjaxHelperTests
	{
		
		[TestMethod]
		public void ShouldMap_DataTableQueryString_ToDataTableAjaxOptions()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "0"},
				{"length", "10"},
				{"search[value]", "search"},
				{"search[regex]", "true"},
				{"order[0][column]", "1"},
				{"order[0][dir]", "asc"},
				{"columns[0][data]", "TwoLetterCountryCode"},
				{"columns[0][name]", "Two Letter Country Code"},
				{"columns[0][search][value]", "gb"},
				{"columns[0][search][regex]", "true"},
				{"columns[1][data]", "ThreeLetterCountryCode"},
				{"columns[1][name]", "Three Letter Country Code"},
				{"columns[1][search][value]", "gb"},
				{"columns[1][search][regex]", "false"},
				{"columns[2][data]", "Id"},
				{"columns[2][name]", "Id"},
				{"columns[2][search][value]", ""},
				{"columns[2][search][regex]", "false"}
			};

			var helper = new DataTableAjaxHelper<Country>(new Mock<ILogger>().Object, new IgnoreColumns());
			helper.LoadQueryString(querystring);

			Assert.AreEqual(1, helper.Draw);
			Assert.AreEqual(0, helper.Start);
			Assert.AreEqual(10, helper.Length);
			Assert.AreEqual("search", helper.SearchValue);
			Assert.IsTrue(helper.IsRegexSearch);
			Assert.AreEqual(1, helper.OrderByColumnIndex);
			Assert.AreEqual("ASC", helper.OrderByDirection);
			Assert.AreEqual(3, helper.ColumnInfos.Count);

			var firstColumnInfo = helper.ColumnInfos.First();
			Assert.AreEqual("TwoLetterCountryCode", firstColumnInfo.Data);
			Assert.AreEqual("Two Letter Country Code", firstColumnInfo.Name);
			Assert.AreEqual("gb", firstColumnInfo.SearchValue);
			Assert.IsTrue(firstColumnInfo.IsRegexSearch);
			Assert.AreEqual("WITH RESULTS AS " +
							"(SELECT *, ROW_NUMBER() OVER " +
							"(ORDER BY ThreeLetterCountryCode ASC) AS RowNum " +
							"FROM Country " +
							"WHERE TwoLetterCountryCode LIKE '%[search]%' " +
							"OR ThreeLetterCountryCode LIKE '%[search]%') " +
							"SELECT * FROM RESULTS " +
							"WHERE RowNum BETWEEN 0 AND 10", helper.GetQuery());
		}

		[TestMethod]
		public void DataTableAjaxHelper_ShouldReturnTheCorrectSqlQuery()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "2"},
				{"length", "20"},
				{"search[value]", ""},
				{"search[regex]", "false"},
				{"order[0][column]", "1"},
				{"order[0][dir]", "desc"},
				{"columns[0][data]", "TwoLetterCountryCode"},
				{"columns[0][name]", "Two Letter Country Code"},
				{"columns[0][search][value]", "gb"},
				{"columns[0][search][regex]", "true"},
				{"columns[1][data]", "ThreeLetterCountryCode"},
				{"columns[1][name]", "Three Letter Country Code"},
				{"columns[1][search][value]", "gb"},
				{"columns[1][search][regex]", "false"},
				{"columns[2][data]", "Id"},
				{"columns[2][name]", "Id"},
				{"columns[2][search][value]", ""},
				{"columns[2][search][regex]", "false"}
			};

			var helper = new DataTableAjaxHelper<Country>(new Mock<ILogger>().Object, new IgnoreColumns());
			helper.LoadQueryString(querystring);

			Assert.AreEqual("WITH RESULTS AS " +
			                "(SELECT *, ROW_NUMBER() OVER " +
							"(ORDER BY ThreeLetterCountryCode DESC) AS RowNum " +
			                "FROM Country " +
			                "WHERE TwoLetterCountryCode LIKE '%[gb]%' " +
			                "OR ThreeLetterCountryCode LIKE '%gb%') " +
			                "SELECT * FROM RESULTS " +
							"WHERE RowNum BETWEEN 40 AND 60", helper.GetQuery());
		}

	}

}
