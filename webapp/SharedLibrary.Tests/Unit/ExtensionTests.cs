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
	}
}
