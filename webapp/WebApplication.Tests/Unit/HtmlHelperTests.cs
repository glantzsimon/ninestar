using K9.DataAccess.Models;
using K9.WebApplication.Helpers;
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

		[Ignore]
		[TestMethod]
		public void HtmlHelper_BootstrapEditoFor_ShouldRenderCorrectly()
		{
			var model = new UserAccount.LoginModel();
			var html = HtmlHelper.CreateHtmlHelper(model);

			html.BootstrapEditorFor(m => m.UserName);

			Assert.AreEqual("<input>", html.GetOutputFromStream());
		}
	}
}
