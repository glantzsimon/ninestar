using System.Collections.Generic;
using System.Web.Mvc;
using K9.SharedLibrary.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K9.SharedLibrary.Tests.Unit
{
	[TestClass]
	public class ExtensionTests
	{
		[TestMethod]
		public void ViewDataDictionary_AddCssClass_ShouldAddCssClassesWithSpace()
		{
			var viewDataDictionary = new ViewDataDictionary(new { });

			viewDataDictionary.MergeAttribute("class", "test2");
			viewDataDictionary.MergeAttribute("class", "test3");

			Assert.AreEqual("test2 test3", viewDataDictionary["class"]);
		}

		[TestMethod]
		public void DelimitedString_ShouldReturnCorrectString()
		{
			var delimitedString = new List<string>
			{
				"Wolf",
				"Back",
				"Meow"
			}.ToDelimitedString();

			var delimitedStringCustom = new List<string>
			{
				"Wolf",
				"Back",
				"Meow"
			}.ToDelimitedString(" |");

			Assert.AreEqual("Wolf, Back, Meow", delimitedString);
			Assert.AreEqual("Wolf | Back | Meow", delimitedStringCustom);
		}

	}
}
