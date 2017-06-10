using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using K9.DataAccess.Config;
using K9.DataAccess.Models;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;

namespace K9.WebApplication.Tests.Unit.Helpers
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
				{"columns[2][data]", "Test"},
				{"columns[2][name]", "Test"},
				{"columns[2][search][value]", ""},
				{"columns[2][search][regex]", "false"}
			};

			var helper = new DataTableAjaxHelper<Country>(new Mock<ILogger>().Object, new ColumnsConfig());
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
							"(SELECT Country.TwoLetterCountryCode, Country.ThreeLetterCountryCode, ROW_NUMBER() OVER " +
							"(ORDER BY Country.ThreeLetterCountryCode ASC) AS RowNum " +
							"FROM Country " +
							"WHERE Country.TwoLetterCountryCode LIKE '%[search]%' " +
							"OR Country.ThreeLetterCountryCode LIKE '%[search]%') " +
							"SELECT * FROM RESULTS " +
							"WHERE RowNum BETWEEN 0 AND 10", helper.GetQuery());
		}

		[TestMethod]
		public void DataTableAjaxHelper_ShouldReturnTheCorrectSqlQuery()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "40"},
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
				{"columns[2][data]", "Test"},
				{"columns[2][name]", "Test"},
				{"columns[2][search][value]", ""},
				{"columns[2][search][regex]", "false"}
			};

			var helper = new DataTableAjaxHelper<Country>(new Mock<ILogger>().Object, new ColumnsConfig());
			helper.LoadQueryString(querystring);

			Assert.AreEqual("WITH RESULTS AS " +
							"(SELECT Country.TwoLetterCountryCode, Country.ThreeLetterCountryCode, ROW_NUMBER() OVER " +
							"(ORDER BY Country.ThreeLetterCountryCode DESC) AS RowNum " +
							"FROM Country " +
							"WHERE Country.TwoLetterCountryCode LIKE '%[gb]%' " +
							"OR Country.ThreeLetterCountryCode LIKE '%gb%') " +
							"SELECT * FROM RESULTS " +
							"WHERE RowNum BETWEEN 40 AND 60", helper.GetQuery());
		}

		[TestMethod]
		public void ShouldMap_DataTableQueryString_ToDataTableAjaxOptions_WithNoWhereClause()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "0"},
				{"length", "10"},
				{"search[value]", ""},
				{"search[regex]", "false"},
				{"order[0][column]", "0"},
				{"order[0][dir]", "asc"},
				{"columns[0][data]", "TwoLetterCountryCode"},
				{"columns[0][name]", "Two Letter Country Code"},
				{"columns[0][search][value]", ""},
				{"columns[0][search][regex]", "true"}
			};

			var helper = new DataTableAjaxHelper<Country>(new Mock<ILogger>().Object, new ColumnsConfig());
			helper.LoadQueryString(querystring);

			Assert.AreEqual("WITH RESULTS AS " +
							"(SELECT Country.TwoLetterCountryCode, ROW_NUMBER() OVER " +
							"(ORDER BY Country.TwoLetterCountryCode ASC) AS RowNum " +
							"FROM Country ) " +
							"SELECT * FROM RESULTS " +
							"WHERE RowNum BETWEEN 0 AND 10", helper.GetQuery());
		}

		[TestMethod]
		public void ShouldMap_DataTableQueryString_ToDataTableAjaxOptions_AndReturnAllColumns()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "0"},
				{"length", "10"},
				{"search[value]", ""},
				{"search[regex]", "false"},
				{"order[0][column]", "0"},
				{"order[0][dir]", "asc"},
				{"columns[0][data]", "TwoLetterCountryCode"},
				{"columns[0][name]", "Two Letter Country Code"},
				{"columns[0][search][value]", ""},
				{"columns[0][search][regex]", "true"}
			};

			var helper = new DataTableAjaxHelper<Country>(new Mock<ILogger>().Object, new ColumnsConfig());
			helper.LoadQueryString(querystring);

			Assert.AreEqual("WITH RESULTS AS " +
							"(SELECT Country.*, ROW_NUMBER() OVER " +
							"(ORDER BY Country.TwoLetterCountryCode ASC) AS RowNum " +
							"FROM Country ) " +
							"SELECT * FROM RESULTS " +
							"WHERE RowNum BETWEEN 0 AND 10", helper.GetQuery(true));
		}

		[TestMethod]
		public void ShouldDetect_VirtualICollection_Properties()
		{
			var enrollmentsName = "Enrollments";
			var propertyInfo = typeof(Student).GetProperties().First(p => p.Name == enrollmentsName);

			Assert.AreEqual(enrollmentsName, propertyInfo.Name);
			Assert.IsTrue(propertyInfo.GetGetMethod().IsVirtual);
			Assert.IsTrue(propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>));
		}

		[TestMethod]
		public void GetAllProperties_IncludingInherited_FromModel()
		{
			var columnsConfig = new ColumnsConfig();
			var columns = typeof(Enrollment).GetProperties()
						.Where(p => !p.IsVirtual() && !columnsConfig.ColumnsToIgnore.Contains(p.Name))
						.ToList();

			Assert.IsTrue(columns.Select(c => c.Name).Contains("StudentId"));
		}

		[TestMethod]
		public void GetPropertiesAndAttributesWithAttribute_ShouldReturnAllForeignKeyAttributes_AndProperties()
		{
			var dictionary = typeof(Enrollment).GetPropertiesAndAttributesWithAttribute<ForeignKeyAttribute>();

			Assert.AreEqual(2, dictionary.Count);
		}

		[TestMethod]
		public void GetQuery_ShouldReturnSQL_IncludingLinkedTables()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "0"},
				{"length", "10"},
				{"search[value]", ""},
				{"search[regex]", "false"},
				{"order[0][column]", "0"},
				{"order[0][dir]", "asc"},
				{"columns[0][data]", "Id"},
				{"columns[0][name]", "Id"}
			};

			var helper = new DataTableAjaxHelper<Enrollment>(new Mock<ILogger>().Object, new ColumnsConfig());
			helper.LoadQueryString(querystring);

			Assert.AreEqual("WITH RESULTS AS " +
							"(SELECT Enrollment.*, Course.Name AS [CourseName], " +
							"Student.Name AS [StudentName], " +
							"ROW_NUMBER() OVER " +
							"(ORDER BY Enrollment.Id ASC) AS RowNum " +
							"FROM Enrollment " +
							"JOIN Course ON Course.Id = Enrollment.CourseId JOIN Student ON Student.Id = Enrollment.StudentId ) " +
							"SELECT * FROM RESULTS " +
							"WHERE RowNum BETWEEN 0 AND 10", helper.GetQuery(true));
		}

		[TestMethod]
		public void EnsureNameProperty_IsIncludedInProperties()
		{
			var props = typeof (Country).GetProperties();

			Assert.AreEqual(1, props.Count(p => p.Name == "Name"));
		}

	}

}
