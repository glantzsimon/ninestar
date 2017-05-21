using K9.DataAccess.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HtmlHelper = K9.SharedLibrary.Helpers.HtmlHelper;

namespace K9.WebApplication.Tests.Unit
{
	[TestClass]
	public class HtmlHelperTests
	{

		[TestMethod]
		public void HtmlHelper_ShouldWriteToStream()
		{
			var html = HtmlHelper.CreateHtmlHelper(new User());

			html.ViewContext.Writer.Write("test");

			Assert.AreEqual("test", html.GetOutputFromStream());
		}
		
	}
}
