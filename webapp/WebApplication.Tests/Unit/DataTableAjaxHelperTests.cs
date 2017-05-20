using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using K9.WebApplication.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K9.WebApplication.Tests.Unit
{
	[TestClass]
	public class DataTableAjaxHelperTests
	{
		
		[TestMethod]
		public void ShouldMap_DataTableQueryString_ToDataTableAjaxOptions()
		{
			var querystring = new NameValueCollection();
			querystring.Add("draw", "1");
			querystring.Add("start", "0");
			querystring.Add("length", "10");
			querystring.Add("search[value]", "search");
			querystring.Add("search[regex]", "true");
			querystring.Add("order[0][column]", "0");
			querystring.Add("order[0][dir]", "asc");
		
			querystring.Add("columns[0]", "1");
			querystring.Add("columns[0][data]", "TwoLetterCountryCode");
			querystring.Add("columns[0][name]", "Two Letter Country Code");
			querystring.Add("columns[0][search][value]", "gb");
			querystring.Add("columns[0][search][regex]", "false");
			querystring.Add("columns[1][data]", "ThreeLetterCountryCode");
			querystring.Add("columns[1][name]", "Three Letter Country Code");
			querystring.Add("columns[1][search][value]", "gb");
			querystring.Add("columns[1][search][regex]", "false");
			querystring.Add("columns[1][data]", "Id");
			querystring.Add("columns[1][name]", "Id");
			querystring.Add("columns[1][search][value]", "");
			querystring.Add("columns[1][search][regex]", "false");
			
			var helper = new DataTableAjaxHelper(querystring);

			Assert.AreEqual(1, helper.Draw);
			Assert.AreEqual(0, helper.Start);
			Assert.AreEqual(10, helper.Length);
			Assert.AreEqual("search", helper.SearchValue);
			Assert.IsTrue(helper.IsRegexSearch);
		}

	}

}
