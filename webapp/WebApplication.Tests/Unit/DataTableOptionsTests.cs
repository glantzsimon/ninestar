using System.Collections.Generic;
using System.Linq;
using K9.DataAccess.Config;
using K9.DataAccess.Models;
using K9.WebApplication.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K9.WebApplication.Tests.Unit
{
	[TestClass]
	public class DataTableOptionsTests
	{

		[TestMethod]
		public void GetColumns_ShouldReturnAllColumns()
		{
			var options = new DataTableOptions<Country>
			{
				ColumnsConfig = new ColumnsConfig()
			};

			var propertyInfos = options.GetColumns();

			Assert.AreEqual(4, propertyInfos.Count);
			Assert.AreEqual("TwoLetterCountryCode", propertyInfos.First().Name);
		}

		[TestMethod]
		public void GetColumns_ShouldOrderColumns_WhenVisibleColumnsIsSet_AndOrderByThem()
		{
			var options = new DataTableOptions<Country>
			{
				ColumnsConfig = new ColumnsConfig(),
				VisibleColumns = new List<string>
				{
					"ThreeLetterCountryCode",
					"Id",
					"TwoLetterCountryCode"
				}
			};

			var propertyInfos = options.GetColumns();

			Assert.AreEqual(4, propertyInfos.Count);
			Assert.AreEqual("ThreeLetterCountryCode", propertyInfos.First().Name);
			Assert.AreEqual("Id", propertyInfos[1].Name);
			Assert.AreEqual("TwoLetterCountryCode", propertyInfos[2].Name);
		}

	}

}
