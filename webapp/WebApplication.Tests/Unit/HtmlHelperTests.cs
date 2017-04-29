using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using K9.DataAccess.Models;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HtmlHelper = K9.SharedLibrary.Helpers.HtmlHelper;

namespace K9.WebApplication.Tests.Unit
{
	[TestClass]
	public class HtmlHelperTests
	{
		//private MemoryStream _stream;
		//private StreamWriter _streamWriter;

		//public string GetOutputFromStream()
		//{
		//    _streamWriter.Flush();
		//    var result = Encoding.UTF8.GetString(_stream.ToArray());

		//    _stream.Close();
		//    _streamWriter.Close();

		//    return result;
		//}

		//private HtmlHelper<TModel> CreateHtmlHelper<TModel>(TModel model) where TModel : class
		//{
		//    var viewDataDictionary = new ViewDataDictionary(model);
		//    _stream = new MemoryStream();
		//    _streamWriter = new StreamWriter(_stream);
		//    var mockViewContext = new Mock<ViewContext>(
		//        new ControllerContext(
		//            new Mock<HttpContextBase>().Object,
		//            new RouteData(),
		//            new Mock<ControllerBase>().Object
		//        ),
		//        new Mock<IView>().Object,
		//        viewDataDictionary,
		//        new TempDataDictionary(),
		//        _streamWriter
		//    );
		//    mockViewContext.Setup(vc => vc.Writer).Returns(_streamWriter);
		//    var mockDataContainer = new Mock<IViewDataContainer>();
		//    mockDataContainer.Setup(c => c.ViewData).Returns(viewDataDictionary);
		//    return new HtmlHelper<TModel>(mockViewContext.Object, mockDataContainer.Object);
		//}

		[TestMethod]
		public void HtmlHelper_ShouldWriteToStream()
		{
			var html = HtmlHelper.CreateHtmlHelper(new User());

			html.ViewContext.Writer.Write("test");

			Assert.AreEqual("test", html.GetOutputFromStream());
		}

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
